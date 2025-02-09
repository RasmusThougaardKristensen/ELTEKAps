using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.ApplicationServices.Tasks.Get
{
    public interface IGetTasksService
    {
        public Task<IEnumerable<TaskModel>> GetTasks();
    }
}
