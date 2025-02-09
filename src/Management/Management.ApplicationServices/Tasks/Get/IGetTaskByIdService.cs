using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Get;

public interface IGetTaskByIdService
{
    public Task<TaskModel?> GetTaskById(Guid taskId);
}