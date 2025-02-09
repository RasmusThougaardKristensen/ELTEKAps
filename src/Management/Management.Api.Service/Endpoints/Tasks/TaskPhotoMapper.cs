using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.Domain.Photos;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks;

public class TaskPhotoMapper
{
    public static TaskPhotoResponse ToResponseModel(PhotoModel photos)
    {
        return new TaskPhotoResponse(
            photos.Id,
            photos.PhotoData,
            photos.CreatedUtc
        );
    }
}
