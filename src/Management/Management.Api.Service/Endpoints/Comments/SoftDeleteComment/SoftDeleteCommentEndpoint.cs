using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Comments.SoftDelete;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Comments.SoftDeleteComment
{
    [Authorize]
    public class SoftDeleteCommentEndpoint : EndpointBaseAsync
        .WithRequest<DeleteCommentRequest>
        .WithoutResult
    {
        private readonly ISoftDeleteCommentService _softDeleteCommentService;

        public SoftDeleteCommentEndpoint(ISoftDeleteCommentService softDeleteCommentService)
        {
            _softDeleteCommentService = softDeleteCommentService;
        }

        [HttpDelete("api/comments/{commentId:guid}")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Soft Delete Comment",
            Description = "Soft delete a comment by ID",
            OperationId = "SoftDeleteComment",
            Tags = new[] { Constants.ApiTags.Comment })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] DeleteCommentRequest request,
            CancellationToken cancellationToken = default)
        {
            var operationResult = await _softDeleteCommentService.RequestSoftDeleteComment(
                request.Id,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot soft delete comment",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(
                    title: "Unknown error requesting to soft delete comment",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class DeleteCommentRequest
    {
        [FromRoute(Name = "commentId")]
        public Guid Id { get; set; }
    }

}
