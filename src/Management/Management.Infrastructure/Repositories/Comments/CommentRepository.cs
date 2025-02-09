using ELTEKAps.Management.ApplicationServices.Repositories.Comments;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ELTEKAps.Management.Infrastructure.Repositories.Comments;
internal class CommentRepository : BaseRepository<CommentModel, CommentEntity>, ICommentRepository
{
    public CommentRepository(TaskContext context) : base(context)
    {
    }

    private DbSet<CommentEntity> GetCommentDbSet()
    {
        if (Context.Comments is null)
            throw new InvalidOperationException("Database configuration not setup correctly");
        return Context.Comments;
    }

    protected override CommentModel Map(CommentEntity entity)
    {
        return CommentMapper.MapToModel(entity);
    }

    protected override CommentEntity Map(CommentModel model)
    {
        return CommentMapper.MapToEntity(model);
    }
}
