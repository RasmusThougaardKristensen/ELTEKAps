using ELTEKAps.Management.ApplicationServices.Customers.Get;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Customers
{
    [TestFixture]
    public class GetCustomerByIdServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<ILogger<GetCustomerByIdService>> _mockLogger;
        private GetCustomerByIdService _getCustomerByIdService;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<GetCustomerByIdService>>();
            _getCustomerByIdService = new GetCustomerByIdService(_mockCustomerRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetCustomerById_CustomerExists_ReturnsCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            // Create a customer with the specified customerId.
            var customer = new CustomerModel(
                customerId,
                DateTime.UtcNow,
                DateTime.UtcNow,
                "John Doe",
                "+1234567890",
                "john@example.com"
            );

            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _getCustomerByIdService.GetCustomerById(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(customerId));
            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Once);
        }

        [Test]
        public async Task GetCustomerById_CustomerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ReturnsAsync((CustomerModel?)null);

            // Act
            var result = await _getCustomerByIdService.GetCustomerById(customerId);

            // Assert
            Assert.That(result, Is.Null);
            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Once);
        }

        [Test]
        public void GetCustomerById_ExceptionThrown_ThrowsCustomerQueryException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var exception = new Exception("Repository error");
            _mockCustomerRepository
                .Setup(repo => repo.GetById(customerId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = Assert.ThrowsAsync<GetCustomersServiceException>(async () =>
                await _getCustomerByIdService.GetCustomerById(customerId)
            );

            Assert.That(ex.Message, Does.Contain(customerId.ToString()));
            _mockCustomerRepository.Verify(repo => repo.GetById(customerId), Times.Once);
        }
    }
}
