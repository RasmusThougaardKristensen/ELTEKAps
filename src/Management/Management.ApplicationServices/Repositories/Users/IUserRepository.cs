using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.ApplicationServices.Repositories.Users;
public interface IUserRepository : IBaseRepository<UserModel>
{
    public Task<UserModel?> GetUserByFirebaseId(string firebaseId);
    Task<IEnumerable<UserModel>> GetNonDeletedUsers();
}
