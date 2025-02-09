using ELTEKAps.Management.ApplicationServices.Customers.Get;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.Domain.Customers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Customers
{
    [TestFixture]
    public class GetCustomersServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<ILogger<GetCustomersService>> _mockLogger;
        private GetCustomersService _getCustomersService;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<GetCustomersService>>();
            _getCustomersService = new GetCustomersService(_mockCustomerRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetCustomers_ReturnsNonDeletedCustomers()
        {
            // Arrange
            var customersList = new List<CustomerModel>
            {
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    "John Doe",
                    "+1234567890",
                    "john@example.com"
                ),
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    "Jane Smith",
                    "+0987654321",
                    "jane@example.com"
                )
            };

            _mockCustomerRepository
                .Setup(repo => repo.GetNonDeletedCustomers())
                .ReturnsAsync(customersList);

            // Act
            var result = await _getCustomersService.GetCustomers();

            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Count(), Is.EqualTo(customersList.Count), "Returned count should match expected count");
            CollectionAssert.AreEquivalent(customersList, result, "Returned customers should match the expected list");

            _mockCustomerRepository.Verify(repo => repo.GetNonDeletedCustomers(), Times.Once);
        }

        [Test]
        public void GetCustomers_RepositoryThrowsException_ThrowsCustomerQueryException()
        {
            // Arrange
            var repositoryException = new Exception("Repository error");
            _mockCustomerRepository
                .Setup(repo => repo.GetNonDeletedCustomers())
                .ThrowsAsync(repositoryException);

            // Act & Assert
            Assert.ThrowsAsync<GetCustomersServiceException>(async () => await _getCustomersService.GetCustomers());
            _mockCustomerRepository.Verify(repo => repo.GetNonDeletedCustomers(), Times.Once);
        }
    }
}
