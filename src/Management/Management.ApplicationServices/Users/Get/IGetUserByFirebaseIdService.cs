using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.ApplicationServices.Users.Get;

public interface IGetUserByFirebaseIdService
{
    public Task<UserModel?> GetUserByFireBaseId(string firebaseId);
}