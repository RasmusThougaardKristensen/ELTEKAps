using ELTEKAps.Management.Domain.Photos;

namespace ELTEKAps.Management.Infrastructure.Repositories.Photos;
internal static class PhotoMapper
{
    internal static PhotosEntity MapToEntity(PhotoModel model)
    {
        return new PhotosEntity(
            model.Id,
            model.CreatedUtc,
            model.ModifiedUtc,
            model.Deleted,
            model.PhotoData,
            model.TaskId
            );
    }

    internal static PhotoModel MapToModel(PhotosEntity entity)
    {
        return new PhotoModel(
            entity.Id,
            entity.CreatedUtc,
            entity.ModifiedUtc,
            entity.Deleted,
            entity.PhotoData,
            entity.TaskId
            );
    }

    internal static IEnumerable<PhotoModel> MapToModelList(IEnumerable<PhotosEntity> photosEntities)
    {
        return photosEntities.Select(MapToModel);
    }

    internal static ICollection<PhotosEntity> MapToModelEntity(IEnumerable<PhotoModel> photosModels)
    {
        return photosModels.Select(MapToEntity).ToList();
    }
}
