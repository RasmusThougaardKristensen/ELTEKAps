using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Comments.SoftDelete
{
    public interface ISoftDeleteCommentService
    {
        Task<OperationResult> RequestSoftDeleteComment(Guid commentId, OperationDetails operationDetails);
        Task SoftDeleteComment(Guid commentId);
    }
}
