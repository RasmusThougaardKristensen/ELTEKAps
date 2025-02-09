using Ardalis.ApiEndpoints;
using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.ApplicationServices.Tasks.Get;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTasks
{
    [Authorize]
    public class GetTasksEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult<IEnumerable<TaskResponse>>
    {
        private readonly IGetTasksService _getTasksService;

        public GetTasksEndpoint(IGetTasksService getTasksService)
        {
            _getTasksService = getTasksService;
        }

        [HttpGet("api/tasks")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Get All Tasks",
            Description = "Retrieve a list of all tasks",
            OperationId = "GetTasks",
            Tags = new[] { Constants.ApiTags.Task })
        ]
        public override async Task<ActionResult<IEnumerable<TaskResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await _getTasksService.GetTasks();

            var taskResponses = TaskResponseMapper.ToResponseModels(tasks).ToList();

            return new ActionResult<IEnumerable<TaskResponse>>(taskResponses);
        }
    }
}