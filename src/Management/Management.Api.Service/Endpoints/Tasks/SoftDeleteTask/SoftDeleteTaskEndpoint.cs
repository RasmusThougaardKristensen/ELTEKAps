using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete; // Example
using ELTEKAps.Management.Domain.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.SoftDeleteTask
{
    [Authorize]
    public class SoftDeleteTaskEndpoint : EndpointBaseAsync
        .WithRequest<DeleteTaskRequest>
        .WithoutResult
    {
        private readonly ISoftDeleteTaskService _softDeleteTaskService;

        public SoftDeleteTaskEndpoint(ISoftDeleteTaskService softDeleteTaskService)
        {
            _softDeleteTaskService = softDeleteTaskService;
        }

        [HttpDelete("api/tasks/{taskId:guid}")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Soft Delete Task",
            Description = "Soft delete a Task by ID (excluding comments/photos)",
            OperationId = "SoftDeleteTask",
            Tags = new[] { Constants.ApiTags.Task })
        ]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] DeleteTaskRequest request,
            CancellationToken cancellationToken = default)
        {
            var operationResult = await _softDeleteTaskService.RequestSoftDeleteTask(
                request.TaskId,
                new OperationDetails(Guid.NewGuid().ToString())
            );

            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),
                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot soft delete task",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(
                    title: "Unknown error requesting to soft delete task",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class DeleteTaskRequest
    {
        [FromRoute(Name = "taskId")]
        public Guid TaskId { get; set; }
    }

}
