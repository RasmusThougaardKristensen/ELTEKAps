using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;

namespace ELTEKAps.Management.Infrastructure.Repositories.Users;
public class UserEntity : BaseEntity
{
    public string FirebaseId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<TaskEntity> TaskEntities { get; set; }

    public UserEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string firebaseId, string name, string email)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        FirebaseId = firebaseId;
        Name = name;
        Email = email;
    }
}
