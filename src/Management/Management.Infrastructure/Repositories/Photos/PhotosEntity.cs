using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;

namespace ELTEKAps.Management.Infrastructure.Repositories.Photos;
public class PhotosEntity : BaseEntity
{
    public string PhotoData { get; set; }
    public Guid TaskId { get; set; }
    public TaskEntity TaskEntity { get; set; }

    public PhotosEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string photoData, Guid taskId)
        : base(id, createdUtc, modifiedUtc, deleted)
    {
        PhotoData = photoData;
        TaskId = taskId;
    }
}
