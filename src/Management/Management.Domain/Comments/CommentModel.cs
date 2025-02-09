namespace ELTEKAps.Management.Domain.Comments;
public class CommentModel : BaseModel
{
    public string Comment { get; }
    public Guid TaskId { get; set; }
    public CommentModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string comment, Guid taskId)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        Comment = comment;
        TaskId = taskId;
    }

    public static CommentModel? Create(string comment, Guid taskId)
    {
        return new CommentModel(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            false,
            comment!,
            taskId
            );
    }
}
