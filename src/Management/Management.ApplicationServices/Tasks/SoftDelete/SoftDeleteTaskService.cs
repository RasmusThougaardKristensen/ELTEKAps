using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete
{
    public class SoftDeleteTaskService : ISoftDeleteTaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<SoftDeleteTaskService> _logger;

        public SoftDeleteTaskService(
            ITaskRepository taskRepository,
            IOperationService operationService,
            ILogger<SoftDeleteTaskService> logger)
        {
            _taskRepository = taskRepository;
            _operationService = operationService;
            _logger = logger;
        }

        public async Task<OperationResult> RequestSoftDeleteTask(Guid taskId, OperationDetails operationDetails)
        {
            try
            {
                _logger.LogInformation("Request soft-delete for Task with ID: {TaskId}", taskId);

                // Check if the task exists
                var existingTask = await _taskRepository.GetById(taskId);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID: {TaskId} does not exist", taskId);
                    return OperationResult.InvalidState("Task does not exist.");
                }

                if (existingTask.Deleted)
                {
                    _logger.LogWarning("Task with ID: {TaskId} is already soft-deleted", taskId);
                    return OperationResult.InvalidState("Task is already soft-deleted.");
                }

                // Queue the soft delete operation
                var operation = await _operationService.QueueOperation(
                    OperationBuilder.SoftDeleteTask(taskId, operationDetails.CreatedBy)
                );

                _logger.LogInformation("Operation queued with Request ID: {RequestId} for Task ID: {TaskId}",
                    operation.RequestId, taskId);

                await SoftDeleteTask(taskId);

                return OperationResult.Accepted(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during soft-delete request for Task ID: {TaskId}", taskId);
                throw new TaskOperationException("Failed to queue and soft-delete the task.", ex);
            }
        }

        public async Task SoftDeleteTask(Guid taskId)
        {
            try
            {
                _logger.LogInformation("Soft-deleting Task with ID: {TaskId}", taskId);

                // Retrieve the task
                var task = await _taskRepository.GetById(taskId);
                if (task == null)
                {
                    _logger.LogWarning("Task with ID: {TaskId} was not found for soft-delete", taskId);
                    return;
                }

                task.SoftDelete();

                await _taskRepository.Upsert(task);

                _logger.LogInformation("Task with ID: {TaskId} has been soft-deleted", taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting Task with ID: {TaskId}", taskId);
                throw new TaskSoftDeleteException("Failed to soft-delete the task.", ex);
            }
        }
    }
}
