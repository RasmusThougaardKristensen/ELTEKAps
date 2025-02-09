using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.Infrastructure.Repositories.Users;
internal static class UserMapper
{
    internal static UserEntity Map(UserModel model)
    {
        return new UserEntity(
            model.Id,
            model.CreatedUtc,
            model.ModifiedUtc,
            model.Deleted,
            model.FirebaseId,
            model.Name,
            model.Email
            );
    }

    internal static UserModel Map(UserEntity userEntity)
    {
        return new UserModel(
            userEntity.Id,
            userEntity.CreatedUtc,
            userEntity.ModifiedUtc,
            userEntity.Deleted,
            userEntity.FirebaseId,
            userEntity.Name,
            userEntity.Email
            );
    }
}
