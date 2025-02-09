using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Tasks;
using ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.TestFixtures.Operations;
using ELTEKAps.Management.TestFixtures.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Tasks
{
    [TestFixture]
    public class SoftDeleteTaskServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<SoftDeleteTaskService>> _mockLogger;
        private SoftDeleteTaskService _softDeleteTaskService;

        [SetUp]
        public void SetUp()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<SoftDeleteTaskService>>();

            _softDeleteTaskService = new SoftDeleteTaskService(
                _mockTaskRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        #region RequestSoftDeleteTask Tests

        [Test]
        public async Task RequestSoftDeleteTask_ValidTask_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingTask = TaskModelFixture.Builder()
                .WithId(taskId)
                .WithDeleted(false)
                .Build();

            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.SoftDeleteTask)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync(existingTask);
            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _softDeleteTaskService.RequestSoftDeleteTask(taskId, operationDetails);

            // Assert
            Assert.Multiple(() =>
            {

                Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
                Assert.That(result.GetOperation().RequestId, Is.EqualTo(operation.RequestId));
            });

            _mockTaskRepository.Verify(repo => repo.GetById(taskId), Times.Exactly(2));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockTaskRepository.Verify(repo =>
                repo.Upsert(It.Is<TaskModel>(t => t.Id == taskId && t.Deleted)), Times.Once);
        }

        [Test]
        public async Task RequestSoftDeleteTask_TaskDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync((TaskModel)null);

            // Act
            var result = await _softDeleteTaskService.RequestSoftDeleteTask(taskId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockTaskRepository.Verify(repo => repo.Upsert(It.IsAny<TaskModel>()), Times.Never);
        }

        [Test]
        public async Task RequestSoftDeleteTask_TaskAlreadySoftDeleted_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingTask = TaskModelFixture.Builder()
                .WithId(taskId)
                .WithDeleted(true)
                .Build();

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _softDeleteTaskService.RequestSoftDeleteTask(taskId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockTaskRepository.Verify(repo => repo.Upsert(It.IsAny<TaskModel>()), Times.Never);
        }

        [Test]
        public void RequestSoftDeleteTask_ExceptionThrown_ThrowsTaskOperationException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingTask = TaskModelFixture.Builder()
                .WithId(taskId)
                .WithDeleted(false)
                .Build();

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync(existingTask);

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<TaskOperationException>(() =>
                _softDeleteTaskService.RequestSoftDeleteTask(taskId, operationDetails)
            );
        }

        #endregion

        #region SoftDeleteTask Tests

        [Test]
        public async Task SoftDeleteTask_ValidTask_UpsertsTaskAsSoftDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = TaskModelFixture.Builder()
                .WithId(taskId)
                .WithDeleted(false)
                .Build();

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync(existingTask);

            // Act
            await _softDeleteTaskService.SoftDeleteTask(taskId);

            // Assert
            _mockTaskRepository.Verify(repo => repo.GetById(taskId), Times.Once);
            _mockTaskRepository.Verify(repo =>
                repo.Upsert(It.Is<TaskModel>(t => t.Id == taskId && t.Deleted)), Times.Once);
        }

        [Test]
        public async Task SoftDeleteTask_TaskNotFound_DoesNotCallUpsert()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync((TaskModel)null);

            // Act
            await _softDeleteTaskService.SoftDeleteTask(taskId);

            // Assert
            _mockTaskRepository.Verify(repo => repo.GetById(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.Upsert(It.IsAny<TaskModel>()), Times.Never);
        }

        [Test]
        public void SoftDeleteTask_ExceptionThrown_ThrowsTaskSoftDeleteException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = TaskModelFixture.Builder()
                .WithId(taskId)
                .WithDeleted(false)
                .Build();

            _mockTaskRepository
                .Setup(repo => repo.GetById(taskId))
                .ReturnsAsync(existingTask);

            _mockTaskRepository
                .Setup(repo => repo.Upsert(It.IsAny<TaskModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<TaskSoftDeleteException>(() =>
                _softDeleteTaskService.SoftDeleteTask(taskId)
            );
        }

        #endregion
    }
}
