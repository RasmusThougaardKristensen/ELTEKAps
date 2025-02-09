using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.ApplicationServices.Users.Get;
using ELTEKAps.Management.Domain.Users;
using ELTEKAps.Management.TestFixtures.Users;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Users
{
    [TestFixture]
    public class GetUsersServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<GetUsersService>> _mockLogger;
        private GetUsersService _getUsersService;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<GetUsersService>>();
            _getUsersService = new GetUsersService(_mockUserRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetUsers_ReturnsNonDeletedUsers()
        {
            // Arrange
            var usersList = new List<UserModel>
            {
                UserModelFixture.Builder().WithDeleted(false).Build(),
                UserModelFixture.Builder().WithDeleted(false).Build()
            };

            _mockUserRepository
                .Setup(repo => repo.GetNonDeletedUsers())
                .ReturnsAsync(usersList);

            // Act
            var result = await _getUsersService.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(usersList.Count));
        }

        [Test]
        public void GetUsers_RepositoryThrowsException_ThrowsUserQueryException()
        {
            // Arrange
            _mockUserRepository
                .Setup(repo => repo.GetNonDeletedUsers())
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<UserQueryException>(async () =>
            {
                await _getUsersService.GetUsers();
            });
        }
    }
}
