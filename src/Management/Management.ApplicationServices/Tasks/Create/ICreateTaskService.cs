using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Create
{
    public interface ICreateTaskService
    {
        /// <summary>
        /// Queues an operation to create a new Task.
        /// </summary>
        /// <param name="taskModel">TaskModel containing data to create.</param>
        /// <param name="operationDetails">Operation details such as who initiated it.</param>
        /// <returns>An OperationResult indicating the outcome.</returns>
        Task<OperationResult> RequestCreateTask(TaskModel taskModel, OperationDetails operationDetails);

        Task<string?> CreateTask(string requestId);
    }
}
