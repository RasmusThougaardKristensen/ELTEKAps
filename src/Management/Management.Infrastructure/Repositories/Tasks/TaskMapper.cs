using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.Infrastructure.Repositories.Comments;
using ELTEKAps.Management.Infrastructure.Repositories.Photos;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;

internal static class TaskMapper
{
    internal static TaskEntity Map(TaskModel model)
    {
        return new TaskEntity(
            model.Id,
            model.CreatedUtc,
            model.ModifiedUtc,
            model.Deleted,
            model.Status,
            model.Description,
            model.Location,
            model.CustomerId,
            model.UserId,
            model.Title
        );
    }

    internal static TaskModel Map(TaskEntity entity)
    {
        return new TaskModel(
            entity.Id,
            entity.CreatedUtc,
            entity.ModifiedUtc,
            entity.Deleted,
            CommentMapper.MapToModelList(entity.CommentEntities),
            PhotoMapper.MapToModelList(entity.PhotoEntities),
            entity.UserId,
            entity.CustomerId,
            entity.Status,
            entity.Description,
            entity.Location,
            entity.Title
        );
    }
}
