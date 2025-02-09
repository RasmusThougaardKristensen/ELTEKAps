using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Repositories.Operations;
public interface IOperationRepository : IBaseRepository<Operation>
{
    Task<Operation?> GetByRequestId(string requestId);
    Task<ICollection<Operation>> GetTaskOperations(Guid taskId);
}