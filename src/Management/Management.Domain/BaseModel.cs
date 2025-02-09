namespace ELTEKAps.Management.Domain;
public class BaseModel
{
    public Guid Id { get; private protected init; }
    public DateTime CreatedUtc { get; protected init; }
    public DateTime ModifiedUtc { get; protected set; }
    public bool Deleted { get; private set; } = false;

    protected BaseModel(Guid id)
    {
        Id = id;
    }

    protected BaseModel()
    {

    }

    public BaseModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted)
    {
        Id = id;
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
        Deleted = deleted;
    }

    public BaseModel(Guid id, DateTime createdUtc, DateTime modifiedUtc) : this(id)
    {
        Id = id;
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
    }

    public void SoftDelete()
    {
        Deleted = true;
        ModifiedUtc = DateTime.UtcNow;
    }
}