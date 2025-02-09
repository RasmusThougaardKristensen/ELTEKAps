using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.Domain.Tasks;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Get;
public class GetTaskByIdService : IGetTaskByIdService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetTaskByIdService> _logger;

    public GetTaskByIdService(
        ITaskRepository taskRepository,
        ILogger<GetTaskByIdService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskModel?> GetTaskById(Guid taskId)
    {
        try
        {
            _logger.LogInformation("Fetching task by ID: {TaskId}", taskId);

            var task = await _taskRepository.GetTaskById(taskId);

            if (task == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found", taskId);
            }
            else
            {
                _logger.LogInformation("Task with ID: {TaskId} retrieved successfully", taskId);
            }

            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching task with ID: {TaskId}", taskId);
            throw new TaskQueryException($"Error fetching task with ID: {taskId}", ex);
        }
    }
}
