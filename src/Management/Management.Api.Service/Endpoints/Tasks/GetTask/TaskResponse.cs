using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;

public class TaskResponse
{
    public Guid Id { get; }
    public DateTime CreatedUtc { get; }
    public string Title { get; }
    public string Description { get; }
    public Status Status { get; }
    public string Location { get; }
    public Guid? CustomerId { get; }
    public Guid UserId { get; }
    public IEnumerable<TaskCommentResponse> Comments { get; }
    public IEnumerable<TaskPhotoResponse> Photos { get; }


    public TaskResponse(Guid id, DateTime createdUtc, string title, string description, Status status, string location, Guid? customerId, Guid userId, IEnumerable<TaskCommentResponse> comments, IEnumerable<TaskPhotoResponse> photos)
    {
        Id = id;
        CreatedUtc = createdUtc;
        Title = title;
        Description = description;
        Status = status;
        Location = location;
        CustomerId = customerId;
        UserId = userId;
        Comments = comments;
        Photos = photos;
    }
}

public class TaskCommentResponse
{
    public Guid Id { get; }
    public string Comment { get; }
    public DateTime CreatedUtc { get; }

    public TaskCommentResponse(Guid id, string comment, DateTime createdUtc)
    {
        Id = id;
        Comment = comment;
        CreatedUtc = createdUtc;
    }
}

public class TaskPhotoResponse
{
    public Guid Id { get; }
    public string PhotoData { get; }
    public DateTime CreatedUtc { get; }

    public TaskPhotoResponse(Guid id, string photoData, DateTime createdUtc)
    {
        Id = id;
        PhotoData = photoData;
        CreatedUtc = createdUtc;
    }
}

