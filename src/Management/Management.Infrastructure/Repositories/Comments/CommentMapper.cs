using ELTEKAps.Management.Domain.Comments;

namespace ELTEKAps.Management.Infrastructure.Repositories.Comments;
internal static class CommentMapper
{
    internal static CommentEntity MapToEntity(CommentModel model)
    {
        return new CommentEntity(
            model.Id,
            model.CreatedUtc,
            model.ModifiedUtc,
            model.Deleted,
            model.Comment,
            model.TaskId
            );
    }

    internal static CommentModel MapToModel(CommentEntity entity)
    {
        return new CommentModel(
            entity.Id,
            entity.CreatedUtc,
            entity.ModifiedUtc,
            entity.Deleted,
            entity.Comment,
            entity.TaskId
            );
    }

    internal static IEnumerable<CommentModel> MapToModelList(IEnumerable<CommentEntity> commentEntities)
    {
        return commentEntities.Select(MapToModel);
    }

    internal static ICollection<CommentEntity> MapToModelEntity(IEnumerable<CommentModel> commentsModels)
    {
        return commentsModels.Select(MapToEntity).ToList();
    }
}