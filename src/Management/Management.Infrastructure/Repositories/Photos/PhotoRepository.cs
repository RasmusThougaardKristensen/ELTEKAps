using ELTEKAps.Management.ApplicationServices.Repositories.Photos;
using ELTEKAps.Management.Domain.Photos;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;

namespace ELTEKAps.Management.Infrastructure.Repositories.Photos;
public class PhotoRepository : BaseRepository<PhotoModel, PhotosEntity>, IPhotoRepository
{
    public PhotoRepository(TaskContext context) : base(context)
    {
    }

    protected override PhotoModel Map(PhotosEntity entity)
    {
        return PhotoMapper.MapToModel(entity);
    }

    protected override PhotosEntity Map(PhotoModel model)
    {
        return PhotoMapper.MapToEntity(model);
    }
}
