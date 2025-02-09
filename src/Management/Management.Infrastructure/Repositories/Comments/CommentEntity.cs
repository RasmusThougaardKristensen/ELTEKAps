using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;

namespace ELTEKAps.Management.Infrastructure.Repositories.Comments;
public class CommentEntity : BaseEntity
{
    public string Comment { get; set; }
    public Guid TaskId { get; set; }
    public TaskEntity TaskEntity { get; set; }

    public CommentEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string comment, Guid taskId)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        Comment = comment;
        TaskId = taskId;
    }
}
