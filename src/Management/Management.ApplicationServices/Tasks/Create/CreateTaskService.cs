using ELTEKAps.Management.ApplicationServices.Operations;
using ELTEKAps.Management.ApplicationServices.Repositories.Customers;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Domain.Tasks;
using Management.Messages.External.Tasks.Create;
using Management.Messages.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Create
{
    public class CreateTaskService : ICreateTaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IOperationService _operationService;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CreateTaskService> _logger;
        private readonly IBus _bus;

        public CreateTaskService(
            ITaskRepository taskRepository,
            IOperationService operationService,
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            ILogger<CreateTaskService> logger,
            IBus bus)
        {
            _taskRepository = taskRepository;
            _operationService = operationService;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _logger = logger;
            _bus = bus;
        }

        public async Task<OperationResult> RequestCreateTask(TaskModel requestCreateTaskModel, OperationDetails operationDetails)
        {
            _logger.LogInformation("Request to create new Task with ID: {TaskId}", requestCreateTaskModel.Id);

            var user = await _userRepository.GetById(requestCreateTaskModel.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with Id {UserId} does not exist", requestCreateTaskModel.UserId);
                return OperationResult.InvalidState($"Cannot create task because user does not exist");
            }

            var customer = await _customerRepository.GetById(requestCreateTaskModel.CustomerId);
            if (customer == null)
            {
                return OperationResult.InvalidState($"Customer with ID {requestCreateTaskModel.CustomerId} does not exist.");
            }

            // Queue the operation
            var operation = await _operationService.QueueOperation(
                OperationBuilder.CreateTask(requestCreateTaskModel, operationDetails.CreatedBy)
            );

            _logger.LogInformation(
                "Operation queued with Request ID: {RequestId} for Task ID: {TaskId}",
                operation.RequestId,
                requestCreateTaskModel.Id
            );

            await _bus.Send(new RequestCreateTaskCommand(operation.RequestId));

            return OperationResult.Accepted(operation);
        }

        public async Task<string?> CreateTask(string requestId)
        {

            var operation = await _operationService.GetOperationByRequestId(requestId);

            if (operation == null)
            {
                _logger.LogWarning("Operation with Request ID: {RequestId} does not exist", requestId);
                return null;
            }

            await _operationService.UpdateOperationStatus(requestId, OperationStatus.Processing);

            var createTask = CreateTaskOperationHelper.MapOperation(operation);

            try
            {
                var createdTask = await _taskRepository.Upsert(createTask);
                operation.OverrideTaskId(createTask.Id);
                await _operationService.UpdateOperation(operation);
                await _operationService.UpdateOperationStatus(requestId, OperationStatus.Completed);
                await _bus.Publish(new CreateTaskSucceedEvent(createdTask.Id));
                _logger.LogInformation("Task with ID: {TaskId} has been created", createTask.Id);
                return createdTask.Id.ToString();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to update Task with id {TaskId}", createTask.Id);
                await _operationService.UpdateOperationStatus(requestId, OperationStatus.Failed);
                await _bus.Publish(new CreateTaskFailedEvent(operation.RequestId, exception.Message));
                throw;
            }
        }
    }
}
