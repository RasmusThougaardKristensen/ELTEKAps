using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.ApplicationServices.Users.Create;

public interface ICreateUserService
{
    public Task<UserModel> RequestCreateUser(string firebaseUid, string email, string displayName);
    Task<UserModel> UpdateDisplayName(string firebaseUid, string displayName);
}