using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.ApplicationServices.Users.Get;

public interface IGetUsersService
{
    Task<IEnumerable<UserModel>> GetUsers();
}