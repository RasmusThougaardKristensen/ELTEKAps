using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.Domain.Tasks;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Get
{
    public class GetTasksService : IGetTasksService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<GetTasksService> _logger;

        public GetTasksService(
            ITaskRepository taskRepository,
            ILogger<GetTasksService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskModel>> GetTasks()
        {
            try
            {
                _logger.LogInformation("Fetching all tasks");

                var tasks = await _taskRepository.GetNonDeletedTasks();
                _logger.LogInformation("Fetched {Count} tasks", tasks.Count());

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks");
                throw new TaskQueryException("An error occurred while fetching tasks.", ex);
            }
        }
    }
}
