using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.CreateTask;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Tasks.Update;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.UpdateTask
{
    [Authorize]
    public class UpdateTaskEndpoint : EndpointBaseAsync
        .WithRequest<UpdateTaskRequest>
        .WithoutResult
    {
        private readonly IUpdateTaskService _updateTaskService;
        private readonly TaskEnvironment _env;

        public UpdateTaskEndpoint(IUpdateTaskService updateTaskService, TaskEnvironment env)
        {
            _updateTaskService = updateTaskService;
            _env = env;
        }

        [HttpPatch("api/tasks/{taskId:guid}")]
        [ProducesResponseType(typeof(OperationAcceptedResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Update Task",
            Description = "Update task with description, customer, status and location",
            OperationId = "UpdateTask",
            Tags = new[] { Constants.ApiTags.Task }
        )]
        public override async Task<ActionResult> HandleAsync(
            [FromRoute] UpdateTaskRequest request,
            CancellationToken cancellationToken)
        {

            var requestUpdateTask = TaskModel.RequestUpdate(
                request.TaskId,
                request.Details.UserId,
                request.Details.CustomerId,
                request.Details.Status,
                request.Details.Description,
                request.Details.Location,
                request.Details.Title
                );

            // Queue or directly perform the update via the service
            var operationResult = await _updateTaskService.RequestUpdateTask(
                requestUpdateTask,
                new OperationDetails(_env.GetUser().Id.ToString()));

            // Return the appropriate HTTP status based on the operation result
            return operationResult.Status switch
            {
                OperationResultStatus.Accepted => new AcceptedResult(
                    // Typically you expose an endpoint to check the operation's status
                    new Uri($"/api/operations/{operationResult.GetOperation().RequestId}", UriKind.Relative),
                    new OperationAcceptedResponse(operationResult.GetOperation().RequestId)
                ),

                OperationResultStatus.InvalidState => Problem(
                    title: "Cannot update task",
                    detail: operationResult.GetMessage(),
                    statusCode: StatusCodes.Status400BadRequest
                ),

                _ => Problem(
                    title: "Unknown error requesting to update task",
                    detail: "Unknown error - check logs",
                    statusCode: StatusCodes.Status500InternalServerError
                )
            };
        }
    }

    public class UpdateTaskRequest : TaskRequest<UpdateTaskDetails>
    {
        [FromRoute(Name = "taskId")]
        public Guid TaskId { get; set; }
    }

    public class UpdateTaskDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string Location { get; set; }
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}
