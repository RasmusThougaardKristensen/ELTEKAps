using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.ApplicationServices.Users.Create;
using ELTEKAps.Management.Domain.Users;
using ELTEKAps.Management.TestFixtures.Users;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ELTEKAps.Management.Tests.Users
{
    [TestFixture]
    public class CreateUserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<CreateUserService>> _mockLogger;
        private CreateUserService _createUserService;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<CreateUserService>>();
            _createUserService = new CreateUserService(_mockUserRepository.Object, _mockLogger.Object);
        }

        #region RequestCreateUser Tests

        [Test]
        public async Task RequestCreateUser_ValidData_ReturnsCreatedUser()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var email = "valid.email@example.com";
            var displayName = "Valid User";

            var expectedUser = UserModelFixture.Builder()
                .WithFirebaseId(firebaseUid)
                .WithEmail(email)
                .WithName(displayName)
                .Build();

            _mockUserRepository
                .Setup(repo => repo.Upsert(It.IsAny<UserModel>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _createUserService.RequestCreateUser(firebaseUid, email, displayName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.FirebaseId, Is.EqualTo(firebaseUid));
                Assert.That(result.Email, Is.EqualTo(email));
                Assert.That(result.Name, Is.EqualTo(displayName));
            });
            _mockUserRepository.Verify(repo => repo.Upsert(It.IsAny<UserModel>()), Times.Once);
        }

        [Test]
        public void RequestCreateUser_InvalidData_EmptyFirebaseUid_ThrowsUserCreationException()
        {
            // Arrange
            var firebaseUid = ""; // Invalid
            var email = "valid.email@example.com";
            var displayName = "Valid User";

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserCreationException>(async () =>
                await _createUserService.RequestCreateUser(firebaseUid, email, displayName));
            StringAssert.Contains("Firebase UID must be provided", ex.Message);
            _mockUserRepository.Verify(repo => repo.Upsert(It.IsAny<UserModel>()), Times.Never);
        }

        [Test]
        public void RequestCreateUser_InvalidData_InvalidEmail_ThrowsUserCreationException()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var email = "invalidEmail"; // Not a valid email format
            var displayName = "Valid User";

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserCreationException>(async () =>
                await _createUserService.RequestCreateUser(firebaseUid, email, displayName));
            StringAssert.Contains("Email is not in a valid format", ex.Message);
            _mockUserRepository.Verify(repo => repo.Upsert(It.IsAny<UserModel>()), Times.Never);
        }

        [Test]
        public void RequestCreateUser_InvalidData_EmptyDisplayName_ThrowsUserCreationException()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var email = "valid.email@example.com";
            var displayName = ""; // Invalid

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserCreationException>(async () =>
                await _createUserService.RequestCreateUser(firebaseUid, email, displayName));
            StringAssert.Contains("Display name must be provided", ex.Message);
            _mockUserRepository.Verify(repo => repo.Upsert(It.IsAny<UserModel>()), Times.Never);
        }

        [Test]
        public void RequestCreateUser_RepositoryThrowsException_ThrowsUserCreationException()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var email = "valid.email@example.com";
            var displayName = "Valid User";

            _mockUserRepository
                .Setup(repo => repo.Upsert(It.IsAny<UserModel>()))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserCreationException>(async () =>
                await _createUserService.RequestCreateUser(firebaseUid, email, displayName));
            StringAssert.Contains("Failed to create user", ex.Message);
        }

        #endregion

        #region UpdateDisplayName Tests

        [Test]
        public async Task UpdateDisplayName_ValidData_ReturnsUpdatedUser()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var newDisplayName = "New Display Name";

            var existingUser = UserModelFixture.Builder()
                .WithFirebaseId(firebaseUid)
                .WithName("Old Name")
                .Build();

            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseUid))
                .ReturnsAsync(existingUser);

            var updatedUser = UserModelFixture.Builder()
                .WithFirebaseId(firebaseUid)
                .WithName(newDisplayName)
                .Build();

            _mockUserRepository
                .Setup(repo => repo.Upsert(It.IsAny<UserModel>()))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _createUserService.UpdateDisplayName(firebaseUid, newDisplayName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(newDisplayName));
            _mockUserRepository.Verify(repo => repo.GetUserByFirebaseId(firebaseUid), Times.Once);
            _mockUserRepository.Verify(repo => repo.Upsert(It.IsAny<UserModel>()), Times.Once);
        }

        [Test]
        public void UpdateDisplayName_UserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            var firebaseUid = "nonexistentFirebaseId";
            var newDisplayName = "New Display Name";

            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseUid))
                .ReturnsAsync((UserModel)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _createUserService.UpdateDisplayName(firebaseUid, newDisplayName));
            StringAssert.Contains($"User with Firebase ID '{firebaseUid}' was not found", ex.Message);
        }

        [Test]
        public void UpdateDisplayName_InvalidDisplayName_ThrowsUserUpdateException()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var newDisplayName = "   "; // Only whitespace

            var existingUser = UserModelFixture.Builder()
                .WithFirebaseId(firebaseUid)
                .WithName("Old Name")
                .Build();

            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseUid))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserUpdateException>(async () =>
                await _createUserService.UpdateDisplayName(firebaseUid, newDisplayName));
            StringAssert.Contains("Invalid or empty display name provided", ex.Message);
        }

        [Test]
        public void UpdateDisplayName_RepositoryThrowsException_ThrowsUserUpdateException()
        {
            // Arrange
            var firebaseUid = "validFirebaseId";
            var newDisplayName = "New Display Name";

            var existingUser = UserModelFixture.Builder()
                .WithFirebaseId(firebaseUid)
                .WithName("Old Name")
                .Build();

            _mockUserRepository
                .Setup(repo => repo.GetUserByFirebaseId(firebaseUid))
                .ReturnsAsync(existingUser);

            _mockUserRepository
                .Setup(repo => repo.Upsert(It.IsAny<UserModel>()))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserUpdateException>(async () =>
                await _createUserService.UpdateDisplayName(firebaseUid, newDisplayName));
            StringAssert.Contains("Failed to update user display name", ex.Message);
        }

        #endregion
    }
}
