using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Users;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Users.Get
{
    public class GetUserByFirebaseIdService : IGetUserByFirebaseIdService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByFirebaseIdService> _logger;

        public GetUserByFirebaseIdService(
            IUserRepository userRepository,
            ILogger<GetUserByFirebaseIdService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserModel?> GetUserByFireBaseId(string firebaseId)
        {
            try
            {
                _logger.LogInformation("Fetching user by Firebase ID: {FirebaseId}", firebaseId);

                var user = await _userRepository.GetUserByFirebaseId(firebaseId);

                if (user == null)
                {
                    _logger.LogWarning("User with Firebase ID: {FirebaseId} not found", firebaseId);
                }
                else
                {
                    _logger.LogInformation("User with Firebase ID: {FirebaseId} retrieved successfully", firebaseId);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with Firebase ID: {FirebaseId}", firebaseId);
                throw new UserQueryException($"Error fetching user with Firebase ID: {firebaseId}", ex);
            }
        }
    }
}
