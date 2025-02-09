using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Operations;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.TestFixtures.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Operations
{
    [TestFixture]
    public class OperationServiceTests
    {
        private Mock<IOperationRepository> _mockOperationRepository;
        private Mock<ILogger<OperationService>> _mockLogger;
        private OperationService _operationService;

        [SetUp]
        public void SetUp()
        {
            _mockOperationRepository = new Mock<IOperationRepository>();
            _mockLogger = new Mock<ILogger<OperationService>>();

            _operationService = new OperationService(
                _mockLogger.Object,
                _mockOperationRepository.Object
            );
        }

        #region QueueOperation

        [Test]
        public async Task QueueOperation_OperationStatusQueued_UpsertsOperationAndReturnsIt()
        {
            // Arrange
            var queuedOperation = OperationFixture.Builder()
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockOperationRepository
                .Setup(repo => repo.Upsert(It.IsAny<Operation>()))
                .ReturnsAsync(queuedOperation);

            // Act
            var result = await _operationService.QueueOperation(queuedOperation);

            // Assert
            Assert.That(result, Is.EqualTo(queuedOperation));
            _mockOperationRepository.Verify(repo => repo.Upsert(queuedOperation), Times.Once);
        }

        [Test]
        public void QueueOperation_OperationStatusNotQueued_ThrowsOperationServiceException()
        {
            // Arrange
            var notQueuedOperation = OperationFixture.Builder()
                .WithStatus(OperationStatus.Processing)
                .Build();

            // Act & Assert
            Assert.ThrowsAsync<OperationServiceException>(async () =>
                await _operationService.QueueOperation(notQueuedOperation)
            );

            _mockOperationRepository.Verify(repo => repo.Upsert(It.IsAny<Operation>()), Times.Never);
        }

        #endregion

        #region GetOperationByRequestId

        [Test]
        public async Task GetOperationByRequestId_OperationFound_ReturnsOperation()
        {
            // Arrange
            var existingOperation = OperationFixture.Builder()
                .WithRequestId("test-request-id")
                .Build();

            _mockOperationRepository
                .Setup(repo => repo.GetByRequestId("test-request-id"))
                .ReturnsAsync(existingOperation);

            // Act
            var result = await _operationService.GetOperationByRequestId("test-request-id");

            // Assert
            Assert.That(result, Is.EqualTo(existingOperation));
            _mockOperationRepository.Verify(repo => repo.GetByRequestId("test-request-id"), Times.Once);
        }

        [Test]
        public void GetOperationByRequestId_InvalidRequestId_ThrowsOperationServiceException()
        {
            // Act & Assert
            Assert.ThrowsAsync<OperationServiceException>(async () =>
                await _operationService.GetOperationByRequestId("")
            );
        }

        #endregion

        #region UpdateOperation

        [Test]
        public async Task UpdateOperation_ValidOperation_CallsUpsert()
        {
            // Arrange
            var operationToUpdate = OperationFixture.Builder()
                .WithStatus(OperationStatus.Processing)
                .Build();

            // Act
            await _operationService.UpdateOperation(operationToUpdate);

            // Assert
            _mockOperationRepository.Verify(repo => repo.Upsert(operationToUpdate), Times.Once);
        }

        #endregion

        #region UpdateOperationStatus

        [Test]
        public async Task UpdateOperationStatus_OperationFound_UpdatesAndReturnsOperation()
        {
            // Arrange
            var existingOperation = OperationFixture.Builder()
                .WithRequestId("status-update-id")
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockOperationRepository
                .Setup(repo => repo.GetByRequestId("status-update-id"))
                .ReturnsAsync(existingOperation);

            _mockOperationRepository
                .Setup(repo => repo.Upsert(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => op);

            // Act
            var updatedOperation = await _operationService.UpdateOperationStatus("status-update-id", OperationStatus.Completed);

            // Assert
            Assert.That(updatedOperation, Is.Not.Null);
            Assert.That(updatedOperation!.Status, Is.EqualTo(OperationStatus.Completed));
            _mockOperationRepository.Verify(repo => repo.GetByRequestId("status-update-id"), Times.Once);
            _mockOperationRepository.Verify(repo => repo.Upsert(It.IsAny<Operation>()), Times.Once);
        }

        [Test]
        public void UpdateOperationStatus_OperationNotFound_ThrowsOperationNotFoundException()
        {
            // Arrange
            _mockOperationRepository
                .Setup(repo => repo.GetByRequestId("unknown-request-id"))
                .ReturnsAsync((Operation)null);

            // Act & Assert
            Assert.ThrowsAsync<OperationNotFoundException>(async () =>
                await _operationService.UpdateOperationStatus("unknown-request-id", OperationStatus.Completed)
            );
            _mockOperationRepository.Verify(repo => repo.Upsert(It.IsAny<Operation>()), Times.Never);
        }

        [Test]
        public void UpdateOperationStatus_QueuedStatus_ThrowsOperationStatusUpdateException()
        {
            // Arrange
            var existingOperation = OperationFixture.Builder()
                .WithRequestId("status-update-id")
                .WithStatus(OperationStatus.Processing)
                .Build();

            _mockOperationRepository
                .Setup(repo => repo.GetByRequestId("status-update-id"))
                .ReturnsAsync(existingOperation);

            // Act & Assert
            Assert.ThrowsAsync<OperationStatusUpdateException>(() =>
                _operationService.UpdateOperationStatus("status-update-id", OperationStatus.Queued)
            );

            _mockOperationRepository.Verify(repo => repo.Upsert(It.IsAny<Operation>()), Times.Never);
        }

        #endregion

        #region GetTaskOperations

        [Test]
        public async Task GetTaskOperations_ReturnsCollectionOfOperations()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var operations = new List<Operation>
            {
                OperationFixture.Builder().WithTaskId(taskId).Build(),
                OperationFixture.Builder().WithTaskId(taskId).Build()
            };

            _mockOperationRepository
                .Setup(repo => repo.GetTaskOperations(taskId))
                .ReturnsAsync(operations);

            // Act
            var result = await _operationService.GetTaskOperations(taskId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            _mockOperationRepository.Verify(repo => repo.GetTaskOperations(taskId), Times.Once);
        }

        #endregion
    }
}
