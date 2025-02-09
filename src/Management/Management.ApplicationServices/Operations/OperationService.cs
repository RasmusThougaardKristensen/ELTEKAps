using ELTEKAps.Management.ApplicationServices.Repositories.Operations;
using ELTEKAps.Management.Domain.Operations;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.Operations;
public class OperationService : IOperationService
{
    private readonly ILogger<OperationService> _logger;
    private readonly IOperationRepository _operationRepository;

    public OperationService(
        ILogger<OperationService> logger,
        IOperationRepository operationRepository)
    {
        _logger = logger;
        _operationRepository = operationRepository;
    }

    public async Task<Operation> QueueOperation(Operation operation)
    {
        try
        {
            ValidateOperation(operation);

            _logger.LogInformation(
                "Queueing operation with RequestId: {RequestId} and Status: {Status}",
                operation.RequestId,
                operation.Status);

            // Ensure the operation is in "Queued" status before storing
            if (operation.Status != OperationStatus.Queued)
            {
                var message = $"Operation must be queued before passing it to {nameof(QueueOperation)}.";
                _logger.LogWarning(message);
                throw new OperationServiceException(message);
            }

            var storedOperation = await _operationRepository.Upsert(operation);

            _logger.LogInformation(
                "Successfully queued operation with RequestId: {RequestId}.",
                storedOperation.RequestId);

            return storedOperation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queueing operation with RequestId: {RequestId}", operation?.RequestId);
            throw new OperationServiceException("An error occurred while queueing the operation.", ex);
        }
    }

    public async Task<Operation?> GetOperationByRequestId(string requestId)
    {
        ValidateRequestId(requestId);

        _logger.LogTrace("Fetching operation for RequestId: {RequestId}", requestId);

        try
        {
            var operation = await _operationRepository.GetByRequestId(requestId);
            if (operation != null)
            {
                _logger.LogTrace(
                    "Found operation: {OperationName} with RequestId: {RequestId}",
                    operation.Name,
                    requestId);
            }
            else
            {
                _logger.LogInformation("No operation found for RequestId: {RequestId}", requestId);
            }

            return operation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching operation by RequestId: {RequestId}", requestId);
            throw new OperationServiceException("An error occurred while fetching the operation.", ex);
        }
    }

    public async Task UpdateOperation(Operation operation)
    {
        ValidateOperation(operation);

        _logger.LogTrace(
            "Updating operation with RequestId: {RequestId} to Status: {Status}",
            operation.RequestId,
            operation.Status);

        try
        {
            await _operationRepository.Upsert(operation);

            _logger.LogTrace(
                "Successfully updated operation with RequestId: {RequestId}",
                operation.RequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error updating operation with RequestId: {RequestId}",
                operation.RequestId);
            throw new OperationServiceException("An error occurred while updating the operation.", ex);
        }
    }

    public async Task<Operation?> UpdateOperationStatus(string requestId, OperationStatus operationStatus)
    {
        ValidateRequestId(requestId);

        _logger.LogTrace(
            "Updating status of operation with RequestId: {RequestId} to {Status}",
            requestId,
            operationStatus);

        try
        {
            var operation = await _operationRepository.GetByRequestId(requestId);
            if (operation is null)
            {
                _logger.LogWarning(
                    "Cannot update operation status. No operation found with RequestId: {RequestId}",
                    requestId);
                throw new OperationNotFoundException(
                    $"Operation with RequestId '{requestId}' does not exist.");
            }

            switch (operationStatus)
            {
                case OperationStatus.Processing:
                    operation.Processing();
                    break;
                case OperationStatus.Completed:
                    operation.Complete();
                    break;
                case OperationStatus.Failed:
                    operation.Failed();
                    break;
                case OperationStatus.Queued:
                default:
                    var message = $"Invalid operation status: '{operationStatus}'. " +
                                  $"Only '{OperationStatus.Processing}', '{OperationStatus.Completed}', " +
                                  $"and '{OperationStatus.Failed}' are supported.";
                    _logger.LogWarning(message);
                    throw new OperationStatusUpdateException(message);
            }

            var updatedOperation = await _operationRepository.Upsert(operation);

            _logger.LogTrace("Operation status updated for RequestId: {RequestId} to {Status}",
                requestId, operationStatus);

            return updatedOperation;
        }
        catch (OperationNotFoundException)
        {
            throw;
        }
        catch (OperationStatusUpdateException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error updating operation status for RequestId: {RequestId} to {Status}",
                requestId,
                operationStatus);
            throw new OperationServiceException("Failed to update the operation's status.", ex);
        }
    }

    public async Task<ICollection<Operation>> GetTaskOperations(Guid taskId)
    {
        _logger.LogTrace("Fetching all operations for TaskId: {TaskId}", taskId);

        try
        {
            var operations = await _operationRepository.GetTaskOperations(taskId);
            _logger.LogTrace(
                "Found {Count} operations for TaskId: {TaskId}",
                operations.Count,
                taskId);

            return operations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching operations for TaskId: {TaskId}",
                taskId);
            throw new OperationServiceException("Failed to retrieve operations for the specified task.", ex);
        }
    }

    /// <summary>
    /// Validates a RequestId to ensure it is not null or whitespace.
    /// Throws <see cref="OperationServiceException"/> if invalid.
    /// </summary>
    private void ValidateRequestId(string requestId)
    {
        if (string.IsNullOrWhiteSpace(requestId))
        {
            const string message = "RequestId cannot be null or whitespace.";
            _logger.LogWarning(message);
            throw new OperationServiceException(message);
        }
    }

    /// <summary>
    /// Validates an <see cref="Operation"/> instance.
    /// Throws <see cref="OperationServiceException"/> if invalid.
    /// </summary>
    private void ValidateOperation(Operation? operation)
    {
        if (operation == null)
        {
            const string message = "Operation cannot be null.";
            _logger.LogWarning(message);
            throw new OperationServiceException(message);
        }

        if (string.IsNullOrWhiteSpace(operation.RequestId))
        {
            const string message = "Operation's RequestId cannot be null or whitespace.";
            _logger.LogWarning(message);
            throw new OperationServiceException(message);
        }
    }
}