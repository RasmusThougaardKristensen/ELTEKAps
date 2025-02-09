using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Comments;
using ELTEKAps.Management.Infrastructure.Repositories.Photos;

namespace ELTEKAps.Management.Infrastructure.Repositories.Tasks;

public class TaskEntity : BaseEntity
{
    public Status Status { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }

    public ICollection<CommentEntity> CommentEntities { get; set; } = new List<CommentEntity>();
    public ICollection<PhotosEntity> PhotoEntities { get; set; } = new List<PhotosEntity>();

    public TaskEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted,
       Status status, string description, string location, Guid customerId, Guid userId, string title)
       : base(id, createdUtc, modifiedUtc, deleted)
    {
        Status = status;
        Description = description;
        Location = location;
        CustomerId = customerId;
        UserId = userId;
        Title = title;
    }
}
