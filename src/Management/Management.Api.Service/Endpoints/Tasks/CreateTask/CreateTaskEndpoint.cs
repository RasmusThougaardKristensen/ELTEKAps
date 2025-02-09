using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Operations.GetOperation;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.Api.Service.Models;
using ELTEKAps.Management.ApplicationServices.Tasks.Create;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.CreateTask;

[Authorize]
public class CreateTaskEndpoint : EndpointBaseAsync.WithRequest<CreateTaskRequest>.WithoutResult
{
    private readonly ICreateTaskService _createTaskService;
    private readonly TaskEnvironment _env;

    public CreateTaskEndpoint(ICreateTaskService createTaskService, TaskEnvironment env)
    {
        _createTaskService = createTaskService;
        _env = env;
    }

    [HttpPost("api/tasks")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Create Task",
        Description = "Create a new Task with the specified details",
        OperationId = "CreateTask",
        Tags = new[] { Constants.ApiTags.Task }
    )]
    public override async Task<ActionResult> HandleAsync([FromRoute] CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var requestCreateTaskModel = TaskModel.RequestCreate(
            request.Details.UserId,
            request.Details.CustomerId,
            request.Details.Status,
            request.Details.Description,
            request.Details.Location,
            request.Details.Title
        );

        var operationResult = await _createTaskService.RequestCreateTask(
            requestCreateTaskModel,
            new OperationDetails(this._env.GetUser().Id.ToString())
        );

        return operationResult.Status switch
        {
            OperationResultStatus.Accepted => new AcceptedResult(
                new Uri(GetOperationByRequestIdEndpoint.GetRelativePath(operationResult.GetOperation()), UriKind.Relative),
                new OperationAcceptedResponse(operationResult.GetOperation().RequestId)),

            OperationResultStatus.InvalidState => Problem(
                title: "Cannot create task",
                detail: operationResult.GetMessage(),
                statusCode: StatusCodes.Status400BadRequest
            ),

            _ => Problem(
                title: "Unknown error requesting to create task",
                detail: "Unknown error - check logs",
                statusCode: StatusCodes.Status500InternalServerError
            ),
        };
    }
}

public abstract class TaskRequest<T>
{
    [FromBody] public T Details { get; set; }
}

public sealed class CreateTaskRequest : TaskRequest<CreateTaskDetails> { }

[SwaggerSchema(Nullable = false)]
public sealed class CreateTaskDetails
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public Status Status { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
}