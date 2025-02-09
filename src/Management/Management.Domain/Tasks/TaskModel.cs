using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Photos;

namespace ELTEKAps.Management.Domain.Tasks;
public class TaskModel : BaseModel
{
    public IEnumerable<CommentModel> Comments { get; }
    public IEnumerable<PhotoModel> Photos { get; set; }
    public Guid UserId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Status Status { get; private set; }
    public string Description { get; private set; }
    public string Location { get; private set; }
    public string Title { get; private set; }


    public TaskModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, IEnumerable<CommentModel> comments, IEnumerable<PhotoModel> photos, Guid userId, Guid customerId, Status status, string description, string location, string title)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        Comments = comments;
        Photos = photos;
        UserId = userId;
        CustomerId = customerId;
        Status = status;
        Description = description;
        Location = location;
        Title = title;
    }

    public TaskModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, Guid userId, Guid customerId, Status status, string description, string location, string title)
        : base(id, createdUtc, modifiedUtc)
    {
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
        UserId = userId;
        CustomerId = customerId;
        Status = status;
        Description = description;
        Location = location;
        Title = title;
    }

    public TaskModel(Guid id, Guid userId, Guid customerId, Status status, string description, string location, string title)
        : base(id)
    {
        Id = id;
        UserId = userId;
        CustomerId = customerId;
        Status = status;
        Description = description;
        Location = location;
        Title = title;
    }

    public TaskModel(Guid userId, Guid customerId, Status status, string description, string location, string title)
    {
        UserId = userId;
        CustomerId = customerId;
        Status = status;
        Description = description;
        Location = location;
        Title = title;
    }

    public static TaskModel RequestCreate(Guid userId, Guid customerId, Status status, string description, string location, string title)
    {
        return new TaskModel(
            userId,
            customerId,
            status,
            description,
            location,
            title
        );
    }

    public static TaskModel Create(Guid userId, Guid customerId, Status status, string description, string location, string title)
    {
        return new TaskModel(
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            userId,
            customerId,
            status,
            description,
            location,
            title
        );
    }

    public static TaskModel RequestUpdate(Guid id, Guid userId, Guid customerId, Status status, string description, string location, string title)
    {
        return new TaskModel(id, userId, customerId, status, description, location, title);
    }

    public void UpdateTaskInformation(TaskModel newTaskInformation)
    {
        CustomerId = newTaskInformation.CustomerId;
        Title = newTaskInformation.Title;
        Description = newTaskInformation.Description;
        Status = newTaskInformation.Status;
        Location = newTaskInformation.Location;
        UserId = newTaskInformation.UserId;
    }
}
