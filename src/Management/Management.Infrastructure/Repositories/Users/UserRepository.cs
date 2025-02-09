using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Users;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ELTEKAps.Management.Infrastructure.Repositories.Users;
public class UserRepository : BaseRepository<UserModel, UserEntity>, IUserRepository
{
    public UserRepository(TaskContext context) : base(context)
    {
    }
    public async Task<UserModel?> GetUserByFirebaseId(string firebaseId)
    {
        var user = await GetUserDbSet()
            .Where(user => user.FirebaseId == firebaseId)
        .FirstOrDefaultAsync();

        return user is null ? null : Map(user);
    }

    public async Task<IEnumerable<UserModel>> GetNonDeletedUsers()
    {
        var users = await GetUserDbSet()
            .Where(user => !user.Deleted)
            .ToListAsync();


        return users.Select(Map);
    }

    private DbSet<UserEntity> GetUserDbSet()
    {
        if (Context.Users is null)
            throw new InvalidOperationException("Database configuration not setup correctly");
        return Context.Users;
    }

    protected override UserModel Map(UserEntity entity)
    {
        return UserMapper.Map(entity);
    }

    protected override UserEntity Map(UserModel model)
    {
        return UserMapper.Map(model);
    }
}
