using ELTEKAps.Management.Domain.Operations;

namespace ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete
{
    public interface ISoftDeleteTaskService
    {
        Task<OperationResult> RequestSoftDeleteTask(Guid taskId, OperationDetails operationDetails);
        Task SoftDeleteTask(Guid taskId);
    }

}
