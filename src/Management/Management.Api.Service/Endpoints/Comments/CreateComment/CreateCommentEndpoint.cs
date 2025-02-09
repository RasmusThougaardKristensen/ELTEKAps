using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.CreateTask;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Comments.Create;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Comments.CreateComment
{
    [Authorize]
    public class CreateCommentEndpoint : EndpointBaseAsync.WithRequest<CreateCommentRequest>.WithoutResult
    {
        private readonly ICreateCommentService _createCommentService;

        public CreateCommentEndpoint(ICreateCommentService createCommentService)
        {
            _createCommentService = createCommentService;
        }

        [HttpPost("api/tasks/{taskId:guid}/comments")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create Comment",
            Description = "Create a new comment for a task",
            OperationId = "CreateComment",
            Tags = new[] { Constants.ApiTags.Comment })
        ]
        public override async Task<ActionResult> HandleAsync([FromRoute] CreateCommentRequest request, CancellationToken cancellationToken = default)
        {
            var comment = CommentModel.Create(request.Details.Comment, request.TaskId);

            var operationResult = await _createCommentService.RequestCreateComment(
                comment!,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot create comment",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest),
                _ => Problem(title: "Unknown error requesting to create comment", detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError),
            };
        }
    }


    public class CreateCommentRequest : TaskRequest<CreateCommentDetails>
    {
        [FromRoute(Name = "taskId")]
        public Guid TaskId { get; set; }
    }

    public class CreateCommentDetails
    {
        [SwaggerSchema(Description = "The comment text")]
        public string Comment { get; set; }
    }
}
