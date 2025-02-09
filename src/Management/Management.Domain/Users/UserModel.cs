
namespace ELTEKAps.Management.Domain.Users;
public class UserModel : BaseModel
{
    public string FirebaseId { get; }
    public string Name { get; set; }
    public string Email { get; }


    public UserModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string firebaseId, string name, string email)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        FirebaseId = firebaseId;
        Name = name;
        Email = email;
    }

    public UserModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, string firebaseId, string name, string email)
        : base(id, createdUtc, modifiedUtc)
    {
        FirebaseId = firebaseId;
        Name = name;
        Email = email;
    }

    public static UserModel Create(string firebaseId, string name, string email)
    {
        return new UserModel(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            firebaseId,
            name,
            email
            );
    }
}
