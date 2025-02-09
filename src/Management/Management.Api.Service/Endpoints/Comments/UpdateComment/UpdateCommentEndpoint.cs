using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Comments.Update;
using ELTEKAps.Management.Domain.Comments;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Comments.UpdateComment
{
    [Authorize]
    public class UpdateCommentEndpoint : EndpointBaseAsync
        .WithRequest<PutCommentRequest>
        .WithoutResult
    {
        private readonly IUpdateCommentService _updateCommentService;

        public UpdateCommentEndpoint(IUpdateCommentService updateCommentService)
        {
            _updateCommentService = updateCommentService;
        }

        [HttpPut("api/comments/{commentId:guid}")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update Comment",
            Description = "Update an existing comment",
            OperationId = "UpdateComment",
            Tags = new[] { Constants.ApiTags.Comment })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] PutCommentRequest request,
            CancellationToken cancellationToken = default)
        {
            // Create the updated CommentModel using the request data
            var comment = new CommentModel(
                request.Id,
                DateTime.UtcNow,
                DateTime.UtcNow,
                deleted: false,
                comment: request.Details.Comment,
                taskId: request.Details.TaskId
            );

            var operationResult = await _updateCommentService.RequestUpdateComment(
                comment,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot update comment",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(
                    title: "Unknown error requesting to update comment",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class PutCommentRequest
    {
        [FromRoute(Name = "commentId")]
        public Guid Id { get; set; }

        [FromBody]
        public PutCommentDetails Details { get; set; }
    }

    public class PutCommentDetails
    {
        [SwaggerSchema(Description = "The updated comment text")]
        public string Comment { get; set; }

        [SwaggerSchema(Description = "The ID of the associated task")]
        public Guid TaskId { get; set; }
    }

}
