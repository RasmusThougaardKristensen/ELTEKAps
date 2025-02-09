using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
public interface ITaskRepository : IBaseRepository<TaskModel>
{
    Task<TaskModel> GetTaskById(Guid taskId);
    Task<IEnumerable<TaskModel>> GetNonDeletedTasks();
    Task UpdateTaskInformation(TaskModel newTask);
}
