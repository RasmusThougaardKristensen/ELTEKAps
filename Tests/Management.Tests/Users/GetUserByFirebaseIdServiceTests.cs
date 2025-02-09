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
    public class GetUserByFirebaseIdServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<GetUserByFirebaseIdService>> _mockLogger;
        private GetUserByFirebaseIdService _getUserByFirebaseIdService;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<GetUserByFirebaseIdService>>();
            _getUserByFirebaseIdService = new GetUserByFirebaseIdService(_mockUserRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetUserByFirebaseId_UserFound_ReturnsUser()
        {
            // Arrange
            var firebaseId = "firebase_123";
            var user = UserModelFixture.Builder()
                .WithFirebaseId(firebaseId)
                .Build();

            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseId))
                .ReturnsAsync(user);

            // Act
            var result = await _getUserByFirebaseIdService.GetUserByFireBaseId(firebaseId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirebaseId, Is.EqualTo(firebaseId));
        }

        [Test]
        public async Task GetUserByFirebaseId_UserNotFound_ReturnsNull()
        {
            // Arrange
            var firebaseId = "firebase_123";
            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseId))
                .ReturnsAsync((UserModel)null);

            // Act
            var result = await _getUserByFirebaseIdService.GetUserByFireBaseId(firebaseId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserByFirebaseId_RepositoryThrowsException_ThrowsUserQueryException()
        {
            // Arrange
            var firebaseId = "firebase_123";
            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<UserQueryException>(async () =>
            {
                await _getUserByFirebaseIdService.GetUserByFireBaseId(firebaseId);
            });
        }
    }
}
