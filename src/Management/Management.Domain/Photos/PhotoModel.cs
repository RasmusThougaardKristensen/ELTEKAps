namespace ELTEKAps.Management.Domain.Photos;
public class PhotoModel : BaseModel
{
    public string PhotoData { get; set; }
    public Guid TaskId { get; set; }

    public PhotoModel(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string photoData, Guid taskId) : base(id, createdUtc, modifiedUtc, deleted)
    {
        PhotoData = photoData;
        TaskId = taskId;
    }

    public static PhotoModel? Create(string? photo, Guid taskId)
    {
        if (photo == string.Empty)
        {
            return null;
        }

        return new PhotoModel(
           Guid.NewGuid(),
           DateTime.UtcNow,
           DateTime.UtcNow,
           false,
           photo!,
           taskId
           );
    }
}
