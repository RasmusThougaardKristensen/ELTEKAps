using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using Management.Messages.External.Tasks.Update;
using Management.Messages.Tasks.Update;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Update;

public class UpdateTaskService : IUpdateTaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOperationService _operationService;
    private readonly ILogger<UpdateTaskService> _logger;
    private readonly IBus _bus;

    public UpdateTaskService(
        ITaskRepository taskRepository,
        IOperationService operationService,
        ILogger<UpdateTaskService> logger,
        ICustomerRepository customerRepository,
        IBus bus,
        IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _operationService = operationService;
        _logger = logger;
        _customerRepository = customerRepository;
        _bus = bus;
        _userRepository = userRepository;
    }

    public async Task<OperationResult> RequestUpdateTask(TaskModel newTaskModel, OperationDetails operationDetails)
    {
        _logger.LogInformation("Request update Task with ID: {TaskModelId}", newTaskModel.Id);

        var task = await _taskRepository.GetById(newTaskModel.Id);
        if (task == null)
        {
            _logger.LogWarning("Task with ID: {TaskId} does not exist", newTaskModel.Id);
            return OperationResult.InvalidState("Task does not exist");
        }

        var user = await _userRepository.GetById(newTaskModel.UserId);
        if (user == null)
        {
            _logger.LogWarning("User with ID: {UserId} does not exist", newTaskModel.UserId);
            return OperationResult.InvalidState("User does not exist");
        }

        if (user.Deleted is true)
        {
            _logger.LogWarning("Cannot update task because of user with ID: {CustomerId} is marked as deleted", newTaskModel.CustomerId);
            return OperationResult.InvalidState("Cannot update task. User is deleted");
        }

        var customer = await _customerRepository.GetById(newTaskModel.CustomerId);
        if (customer is null)
        {
            _logger.LogWarning("Customer with ID: {CustomerId} does not exist", newTaskModel.CustomerId);
            return OperationResult.InvalidState("Customer does not exist");
        }

        if (customer.Deleted is true)
        {
            _logger.LogWarning("Cannot update task because of customer with ID: {CustomerId} is marked as deleted", newTaskModel.CustomerId);
            return OperationResult.InvalidState("Cannot update task. Customer is deleted");
        }

        var operation = await _operationService.QueueOperation(OperationBuilder.UpdateTask(task, newTaskModel.UserId, newTaskModel.CustomerId, newTaskModel.Title, newTaskModel.Description, newTaskModel.Status, newTaskModel.Location, operationDetails.CreatedBy));

        _logger.LogInformation("Operation queued with Request ID: {OperationRequestId} for Task ID: {TaskModelId}", operation.RequestId, task.Id);
        await _bus.Send(new RequestUpdateTaskCommand(operation.RequestId, task.Id));

        return OperationResult.Accepted(operation);
    }


    public async Task UpdateTask(string requestId, Guid taskId)
    {
        var operation = await _operationService.GetOperationByRequestId(requestId);

        if (operation == null)
        {
            _logger.LogWarning("Operation with Request ID: {RequestId} does not exist", requestId);
            return;
        }

        await _operationService.UpdateOperationStatus(requestId, OperationStatus.Processing);

        var updatedTask = await SetNewTaskInformation(taskId, operation);

        try
        {
            await _taskRepository.UpdateTaskInformation(updatedTask);
            await _operationService.UpdateOperationStatus(requestId, OperationStatus.Completed);
            await _bus.Publish(new TaskUpdateSucceedEvent(taskId));
        }
        catch (TaskRepositoryException exception)
        {
            _logger.LogError(exception, "Failed to update Task with id {TaskId}", taskId);
            await _operationService.UpdateOperationStatus(requestId, OperationStatus.Failed);
            await _bus.Publish(new TaskUpdateFailedEvent(taskId, exception.Message));
            throw;
        }
    }

    /// <summary>
    /// Maps the operation details to a new task model and updates the current task with the new information.
    /// </summary>
    /// <param name="taskId">The ID of the task to be updated.</param>
    /// <param name="operation">The operation containing the updated task details.</param>
    /// <returns>The updated task model.</returns>
    private async Task<TaskModel> SetNewTaskInformation(Guid taskId, Operation operation)
    {
        var newTask = UpdateTaskOperationHelper.MapOperation(taskId, operation);

        var currentTask = await _taskRepository.GetById(taskId);

        currentTask.UpdateTaskInformation(newTask);

        return currentTask;
    }
}