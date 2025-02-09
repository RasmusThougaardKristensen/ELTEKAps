using ELTEKAps.Management.ApplicationServices.Customers;
using ELTEKAps.Management.ApplicationServices.Customers.SoftDelete;
using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.TestFixtures.Customers;
using ELTEKAps.Management.TestFixtures.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Customers
{
    [TestFixture]
    public class SoftDeleteCustomerServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<SoftDeleteCustomerService>> _mockLogger;
        private SoftDeleteCustomerService _softDeleteCustomerService;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<SoftDeleteCustomerService>>();
            _softDeleteCustomerService = new SoftDeleteCustomerService(
                _mockCustomerRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestSoftDeleteCustomer_ValidCustomer_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingCustomer = CustomerModelFixture.Builder()
                .WithId(customerId)
                .WithDeleted(false)
                .Build();
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.SoftDeleteCustomer)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync(existingCustomer);

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _softDeleteCustomerService.RequestSoftDeleteCustomer(customerId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            Assert.That(result.GetOperation(), Is.Not.Null);
            Assert.That(result.GetOperation().RequestId, Is.EqualTo(operation.RequestId));

            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Exactly(2));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.Is<CustomerModel>(c => c.Id == customerId && c.Deleted)), Times.Once);
        }

        [Test]
        public async Task RequestSoftDeleteCustomer_CustomerDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync((CustomerModel)null);

            // Act
            var result = await _softDeleteCustomerService.RequestSoftDeleteCustomer(customerId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            Assert.That(result.GetMessage(), Is.EqualTo("Customer does not exist."));

            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public async Task RequestSoftDeleteCustomer_CustomerAlreadySoftDeleted_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");
            var existingCustomer = CustomerModelFixture.Builder()
                .WithId(customerId)
                .WithDeleted(true)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync(existingCustomer);

            // Act
            var result = await _softDeleteCustomerService.RequestSoftDeleteCustomer(customerId, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            Assert.That(result.GetMessage(), Is.EqualTo("Customer is already soft-deleted."));

            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public void RequestSoftDeleteCustomer_ExceptionThrown_ThrowsCustomerOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var operationDetails = new OperationDetails("test-user");

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerOperationException>(() =>
                _softDeleteCustomerService.RequestSoftDeleteCustomer(customerId, operationDetails)
            );
        }

        [Test]
        public async Task SoftDeleteCustomer_ValidCustomerId_SoftDeletesCustomerSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = CustomerModelFixture.Builder()
                .WithId(customerId)
                .WithDeleted(false)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync(existingCustomer);

            // Act
            await _softDeleteCustomerService.SoftDeleteCustomer(customerId);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.Is<CustomerModel>(c => c.Id == customerId && c.Deleted)), Times.Once);
        }

        [Test]
        public async Task SoftDeleteCustomer_CustomerDoesNotExist_LogsWarningAndDoesNotUpsert()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync((CustomerModel)null);

            // Act
            await _softDeleteCustomerService.SoftDeleteCustomer(customerId);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public void SoftDeleteCustomer_ExceptionThrown_ThrowsCustomerSoftDeleteException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = CustomerModelFixture.Builder()
                .WithId(customerId)
                .WithDeleted(false)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync(existingCustomer);

            _mockCustomerRepository
                .Setup(repo => repo.Upsert(It.IsAny<CustomerModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerSoftDeleteException>(() =>
                _softDeleteCustomerService.SoftDeleteCustomer(customerId)
            );
        }
    }
}
