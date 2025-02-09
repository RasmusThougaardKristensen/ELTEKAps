using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Comments.Update
{
    public interface IUpdateCommentService
    {
        Task<OperationResult> RequestUpdateComment(CommentModel commentModel, OperationDetails operationDetails);
        Task UpdateComment(CommentModel commentModel);
    }
}
