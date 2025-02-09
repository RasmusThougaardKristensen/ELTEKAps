namespace ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedUtc { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; } = false;

    public BaseEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted)
    {
        Id = id;
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
        Deleted = deleted;
    }
}
