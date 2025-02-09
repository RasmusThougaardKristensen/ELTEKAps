using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using ELTEKAps.Management.Infrastructure.Repositories.Users;
using System.Security.Claims;

namespace ELTEKAps.Management.Api.Service.Models
{
    public class TaskEnvironment
    {
        private readonly ClaimsPrincipal _claimsPrincipal;

        public TaskEnvironment(ClaimsPrincipal claimsPrincipal, TaskContext dbContext)
        {
            _claimsPrincipal = claimsPrincipal;
            this.DbContext = dbContext;
        }
        private UserEntity? _user;
        public UserEntity GetUser()
        {
            if (_user == null)
            {
                var userId = _claimsPrincipal.FindFirstValue("id");
                _user = this.DbContext.Users!.FirstOrDefault(x => x.FirebaseId == userId);

                if (_user == null)
                {
                    throw new InvalidOperationException($"No user found with Firebase ID: {userId}");
                }
            }
            return _user!;
        }

        public TaskContext DbContext { get; set; }
    }
}
