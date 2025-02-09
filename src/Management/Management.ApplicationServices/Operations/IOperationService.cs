using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Operations;
public interface IOperationService
{
    Task<Operation> QueueOperation(Operation operation);
    Task<Operation?> GetOperationByRequestId(string requestId);
    Task<Operation?> UpdateOperationStatus(string requestId, OperationStatus operationStatus);
    Task<ICollection<Operation>> GetTaskOperations(Guid UserId);
    Task UpdateOperation(Operation operation);
}
