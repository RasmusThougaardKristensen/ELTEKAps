using ELTEKAps.Management.ApplicationServices.Customers;
using ELTEKAps.Management.ApplicationServices.Customers.Update;
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
    public class UpdateCustomerServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<UpdateCustomerService>> _mockLogger;
        private UpdateCustomerService _updateCustomerService;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<UpdateCustomerService>>();
            _updateCustomerService = new UpdateCustomerService(
                _mockCustomerRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestUpdateCustomer_ValidCustomer_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("Valid Customer")
                .WithPhoneNumber("+4578658798")
                .WithEmail("valid@example.com")
                .Build();
            var operationDetails = new OperationDetails("test-user");
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.UpdateCustomer)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerModel.Id))
                .ReturnsAsync(customerModel); // Simulate existing customer
            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _updateCustomerService.RequestUpdateCustomer(customerModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Upsert(customerModel), Times.Once);
        }

        [Test]
        public async Task RequestUpdateCustomer_InvalidCustomer_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("") // Invalid name
                .WithPhoneNumber("invalid-phone") // Invalid phone
                .WithEmail("invalid-email") // Invalid email
                .Build();
            var operationDetails = new OperationDetails("test-user");

            // Act
            var result = await _updateCustomerService.RequestUpdateCustomer(customerModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public async Task RequestUpdateCustomer_CustomerDoesNotExist_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("Valid Customer")
                .WithPhoneNumber("+4578658798")
                .WithEmail("valid@example.com")
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerModel.Id))
                .ReturnsAsync((CustomerModel)null); // Simulate non-existing customer

            // Act
            var result = await _updateCustomerService.RequestUpdateCustomer(customerModel, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public void RequestUpdateCustomer_ExceptionThrown_ThrowsCustomerOperationException()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("Valid Customer")
                .WithPhoneNumber("+4578658798")
                .WithEmail("valid@example.com")
                .Build();
            var operationDetails = new OperationDetails("test-user");

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerModel.Id))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerOperationException>(() =>
                _updateCustomerService.RequestUpdateCustomer(customerModel, operationDetails)
            );
        }

        [Test]
        public async Task UpdateCustomer_ValidCustomer_UpdatesCustomerSuccessfully()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("Valid Customer")
                .WithPhoneNumber("+4578658798")
                .WithEmail("valid@example.com")
                .Build();

            // Act
            await _updateCustomerService.UpdateCustomer(customerModel);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.Upsert(customerModel), Times.Once);
        }

        [Test]
        public void UpdateCustomer_ExceptionThrown_ThrowsCustomerUpdateException()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("Valid Customer")
                .WithPhoneNumber("+4578658798")
                .WithEmail("valid@example.com")
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.Upsert(It.IsAny<CustomerModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerUpdateException>(() =>
                _updateCustomerService.UpdateCustomer(customerModel)
            );
        }
    }
}
