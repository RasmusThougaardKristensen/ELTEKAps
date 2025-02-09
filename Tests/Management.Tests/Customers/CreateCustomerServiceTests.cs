using ELTEKAps.Management.ApplicationServices.Customers;
using ELTEKAps.Management.ApplicationServices.Customers.Create;
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
    public class CreateCustomerServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IOperationService> _mockOperationService;
        private Mock<ILogger<CreateCustomerService>> _mockLogger;
        private CreateCustomerService _createCustomerService;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockOperationService = new Mock<IOperationService>();
            _mockLogger = new Mock<ILogger<CreateCustomerService>>();
            _createCustomerService = new CreateCustomerService(
                _mockCustomerRepository.Object,
                _mockOperationService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RequestCreateCustomer_ValidData_ReturnsAcceptedOperationResult()
        {
            // Arrange
            var customerName = "John Doe";
            var phoneNumber = "+4545658798";
            var email = "john.doe@example.com";
            var operationDetails = new OperationDetails("test-user");
            var operation = OperationFixture.Builder()
                .WithOperationName(OperationName.CreateCustomer)
                .WithStatus(OperationStatus.Queued)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetCustomerByEmail(email))
                .ReturnsAsync((CustomerModel)null); // No existing customer

            _mockOperationService
                .Setup(service => service.QueueOperation(It.IsAny<Operation>()))
                .ReturnsAsync(operation);

            // Act
            var result = await _createCustomerService.RequestCreateCustomer(customerName, phoneNumber, email, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Accepted));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Once);
            _mockOperationService.Verify(service => service.UpdateOperationStatus(operation.RequestId, OperationStatus.Completed), Times.Once);
        }

        [Test]
        public async Task RequestCreateCustomer_ExistingCustomer_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerName = "John Doe";
            var phoneNumber = "+4545658798";
            var email = "john.doe@example.com";
            var operationDetails = new OperationDetails("test-user");
            var existingCustomer = CustomerModelFixture.Builder()
                .WithEmail(email)
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.GetCustomerByEmail(email))
                .ReturnsAsync(existingCustomer); // Existing customer

            // Act
            var result = await _createCustomerService.RequestCreateCustomer(customerName, phoneNumber, email, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public async Task RequestCreateCustomer_InvalidData_ReturnsInvalidStateOperationResult()
        {
            // Arrange
            var customerName = ""; // Invalid name
            var phoneNumber = ""; // Invalid phone number
            var email = ""; // Invalid email
            var operationDetails = new OperationDetails("test-user");

            // Act
            var result = await _createCustomerService.RequestCreateCustomer(customerName, phoneNumber, email, operationDetails);

            // Assert
            Assert.That(result.Status, Is.EqualTo(OperationResultStatus.InvalidState));
            _mockOperationService.Verify(service => service.QueueOperation(It.IsAny<Operation>()), Times.Never);
            _mockCustomerRepository.Verify(repo => repo.Upsert(It.IsAny<CustomerModel>()), Times.Never);
        }

        [Test]
        public void RequestCreateCustomer_ExceptionThrown_ThrowsCustomerOperationException()
        {
            // Arrange
            var customerName = "John Doe";
            var phoneNumber = "+4545658798";
            var email = "john.doe@example.com";
            var operationDetails = new OperationDetails("test-user");

            _mockCustomerRepository
                .Setup(repo => repo.GetCustomerByEmail(email))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerOperationException>(() =>
                _createCustomerService.RequestCreateCustomer(customerName, phoneNumber, email, operationDetails)
            );
        }

        [Test]
        public async Task CreateCustomer_ValidCustomer_CreatesCustomerSuccessfully()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("John Doe")
                .WithPhoneNumber("+4545658798")
                .WithEmail("john.doe@example.com")
                .Build();

            // Act
            await _createCustomerService.CreateCustomer(customerModel);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.Upsert(customerModel), Times.Once);
        }

        [Test]
        public void CreateCustomer_ExceptionThrown_ThrowsCustomerCreationException()
        {
            // Arrange
            var customerModel = CustomerModelFixture.Builder()
                .WithName("John Doe")
                .WithPhoneNumber("+4545658798")
                .WithEmail("john.doe@example.com")
                .Build();

            _mockCustomerRepository
                .Setup(repo => repo.Upsert(It.IsAny<CustomerModel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<CustomerCreationException>(() =>
                _createCustomerService.CreateCustomer(customerModel)
            );
        }
    }
}
