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
    public class GetTasksServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<ILogger<GetTasksService>> _mockLogger;
        private GetTasksService _getTasksService;

        [SetUp]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockLogger = new Mock<ILogger<GetTasksService>>();
            _getTasksService = new GetTasksService(_mockTaskRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetTasks_ReturnsAllNonDeletedTasks()
        {
            // Arrange
            var tasksList = new List<TaskModel>
            {
                TaskModelFixture.Builder().Build(),
                TaskModelFixture.Builder().Build()
            };

            _mockTaskRepository
                .Setup(repo => repo.GetNonDeletedTasks())
                .ReturnsAsync(tasksList);

            // Act
            var result = await _getTasksService.GetTasks();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(tasksList.Count));
        }

        [Test]
        public void GetTasks_RepositoryThrowsException_ThrowsTaskQueryException()
        {
            // Arrange
            _mockTaskRepository
                .Setup(repo => repo.GetNonDeletedTasks())
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<TaskQueryException>(async () =>
            {
                await _getTasksService.GetTasks();
            });
        }
    }
}
