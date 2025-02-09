using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Tasks.Get;
using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.TestFixtures.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Tasks
{
    [TestFixture]
    public class GetTaskByIdServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<ILogger<GetTaskByIdService>> _mockLogger;
        private GetTaskByIdService _getTaskByIdService;

        [SetUp]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockLogger = new Mock<ILogger<GetTaskByIdService>>();
            _getTaskByIdService = new GetTaskByIdService(_mockTaskRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetTaskById_TaskFound_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskModel = TaskModelFixture.Builder().WithId(taskId).Build();

            _mockTaskRepository
                .Setup(repo => repo.GetTaskById(taskId))
                .ReturnsAsync(taskModel);

            // Act
            var result = await _getTaskByIdService.GetTaskById(taskId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(taskId));
        }

        [Test]
        public async Task GetTaskById_TaskNotFound_ReturnsNull()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockTaskRepository
                .Setup(repo => repo.GetTaskById(taskId))
                .ReturnsAsync((TaskModel)null);

            // Act
            var result = await _getTaskByIdService.GetTaskById(taskId);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
