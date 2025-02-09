using ELTEKAps.Management.Domain.Users;

namespace ELTEKAps.Management.Api.Service.Endpoints.Users;

public static class UserResponseMapper
{
    public static UserResponse ToResponseModel(UserModel user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email
            );
    }

    public static IEnumerable<UserResponse> ToResponseModels(IEnumerable<UserModel> users)
    {
        if (users == null) throw new ArgumentNullException(nameof(users));

        return users.Select(ToResponseModel);
    }
}
