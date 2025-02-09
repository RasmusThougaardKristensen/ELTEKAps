using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.Domain.Comments;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks;

public class TaskCommentMapper
{
    public static TaskCommentResponse ToResponseModel(CommentModel comments)
    {
        return new TaskCommentResponse(
            comments.Id,
            comments.Comment,
            comments.CreatedUtc
        );
    }
}