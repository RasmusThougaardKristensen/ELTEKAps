using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Users;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ELTEKAps.Management.ApplicationServices.Users.Create
{
    public class CreateUserService : ICreateUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserService> _logger;

        public CreateUserService(IUserRepository userRepository, ILogger<CreateUserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserModel> RequestCreateUser(string firebaseUid, string email, string displayName)
        {
            _logger.LogInformation("Request to create user with Firebase ID: {FirebaseId}", firebaseUid);

            var validationErrors = ValidateUserData(firebaseUid, email, displayName).ToList();
            if (validationErrors.Any())
            {
                var errorMessage = string.Join("; ", validationErrors);
                _logger.LogWarning("Validation failed. Errors: {ErrorMessage}", errorMessage);
                throw new UserCreationException(errorMessage);
            }

            try
            {
                var userModel = UserModel.Create(firebaseUid, displayName, email);

                // Create or update the user in the repository.
                var createdUser = await _userRepository.Upsert(userModel);

                _logger.LogInformation("User created with ID: {UserId}", createdUser.Id);
                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with Firebase ID: {FirebaseId}", firebaseUid);
                throw new UserCreationException("Failed to create user.", ex);
            }
        }

        public async Task<UserModel> UpdateDisplayName(string firebaseUid, string displayName)
        {
            _logger.LogInformation("Request to update user display name for Firebase ID: {FirebaseId}", firebaseUid);

            var userModel = await _userRepository.GetUserByFirebaseId(firebaseUid);
            if (userModel == null)
            {
                var notFoundMessage = $"User with Firebase ID '{firebaseUid}' was not found.";
                _logger.LogWarning(notFoundMessage);
                throw new UserNotFoundException(notFoundMessage);
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                var invalidMessage = "Invalid or empty display name provided.";
                _logger.LogWarning(invalidMessage);
                throw new UserUpdateException(invalidMessage);
            }

            try
            {
                userModel.Name = displayName;
                var updatedUser = await _userRepository.Upsert(userModel);

                _logger.LogInformation(
                    "User with ID {UserId} updated display name to '{DisplayName}'",
                    updatedUser.Id,
                    displayName);

                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user display name for Firebase ID: {FirebaseId}", firebaseUid);
                throw new UserUpdateException("Failed to update user display name.", ex);
            }
        }

        private IEnumerable<string> ValidateUserData(string firebaseUid, string email, string displayName)
        {
            if (string.IsNullOrWhiteSpace(firebaseUid))
                yield return "Firebase UID must be provided.";

            if (string.IsNullOrWhiteSpace(email))
            {
                yield return "Email must be provided.";
            }
            else
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(email))
                    yield return "Email is not in a valid format.";
            }

            if (string.IsNullOrWhiteSpace(displayName))
                yield return "Display name must be provided.";
        }
    }
}
