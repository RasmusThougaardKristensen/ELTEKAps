using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Update
{
    public interface IUpdateTaskService
    {
        /// <summary>
        /// Requests an update for an existing task by validating its existence, 
        /// checking the associated customer, and queuing an update operation.
        /// </summary>
        /// <param name="newTaskModel">The updated task model containing the new task details.</param>
        /// <param name="operationDetails">Details about the operation, including the user who initiated it.</param>
        /// <returns>An <see cref="OperationResult"/> indicating whether the request was accepted or failed due to invalid state.</returns>
        Task<OperationResult> RequestUpdateTask(TaskModel taskModel, OperationDetails operationDetails);

        /// <summary>
        /// Updates an existing task based on an operation request. 
        /// It retrieves the operation, updates its status, applies changes to the task, and handles success or failure accordingly.
        /// </summary>
        /// <param name="requestId">The unique identifier of the operation request.</param>
        /// <param name="taskId">The unique identifier of the task to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="TaskRepositoryException">Thrown if updating the task fails.</exception>
        Task UpdateTask(string requestId, Guid taskId);
    }
}
