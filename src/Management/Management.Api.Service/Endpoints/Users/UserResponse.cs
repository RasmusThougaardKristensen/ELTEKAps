namespace ELTEKAps.Management.Api.Service.Endpoints.Users;

public class UserResponse
{
    public Guid UserId { get; }
    public string Name { get; }
    public string Email { get; }

    public UserResponse(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }
}
