using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Comments.Create
{
    public interface ICreateCommentService
    {
        Task<OperationResult> RequestCreateComment(CommentModel commentModel, OperationDetails operationDetails);
        Task CreateComment(CommentModel commentModel);
    }
}
