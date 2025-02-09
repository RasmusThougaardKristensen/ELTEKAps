using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Users;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Users.Get
{
    public sealed class GetUsersService : IGetUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUsersService> _logger;

        public GetUsersService(
            IUserRepository userRepository,
            ILogger<GetUsersService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all non-deleted users");

                var users = await _userRepository.GetNonDeletedUsers();

                _logger.LogInformation("Fetched {Count} users", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                throw new UserQueryException("An error occurred while fetching users.", ex);
            }
        }
    }
}
