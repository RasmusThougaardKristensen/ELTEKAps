using Ardalis.ApiEndpoints;
using ELTEKAps.Management.ApplicationServices.Tasks.Get;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;

[Authorize]
public class GetTaskEndpoint : EndpointBaseAsync.WithRequest<Guid>.WithActionResult<TaskResponse>
{
    private readonly IGetTaskByIdService _taskByIdService;

    public GetTaskEndpoint(IGetTaskByIdService taskByIdService)
    {
        _taskByIdService = taskByIdService;
    }

    [HttpGet("api/tasks/{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get Task by Task id",
        Description = "Get Task by Task id",
        OperationId = "GetTask",
        Tags = new[] { Constants.ApiTags.Task })
    ]
    public override async Task<ActionResult<TaskResponse>> HandleAsync([FromRoute] Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskByIdService.GetTaskById(taskId);

        if (task == null)
        {
            return Problem(title: "Task could not be found",
                detail: $"Task having id: '{taskId}' not found", statusCode: StatusCodes.Status404NotFound);
        }

        var taskResponse = TaskResponseMapper.ToResponseModel(task);

        return new ActionResult<TaskResponse>(taskResponse);
    }
}
