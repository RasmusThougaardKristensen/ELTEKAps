using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.Domain.Tasks;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace ELTEKAps.Management.Infrastructure.Repositories.Tasks;
public class TaskRepository : BaseRepository<TaskModel, TaskEntity>, ITaskRepository
{
    public TaskRepository(TaskContext context) : base(context)
    {
    }

    public async Task<TaskModel?> GetTaskById(Guid taskId)
    {
        var task = await GetTaskDbSet()
            .Where(task => task.Id == taskId)
            .Include(task => task.PhotoEntities)
            .Include(task => task.CommentEntities)
            .Select(task => new TaskEntity(task.Id, task.CreatedUtc, task.ModifiedUtc, task.Deleted, task.Status, task.Description, task.Location, task.CustomerId, task.UserId, task.Title)
            {
                CommentEntities = task.CommentEntities.Where(comment => comment.Deleted == false).ToList(),
                PhotoEntities = task.PhotoEntities.Where(photo => photo.Deleted == false).ToList()
            })
            .FirstOrDefaultAsync();

        return task is null ? null : Map(task);
    }

    public async Task<IEnumerable<TaskModel>> GetNonDeletedTasks()
    {
        var tasks = await GetTaskDbSet()
            .Where(task => task.Deleted == false)
            .ToListAsync();

        return tasks.Select(Map);
    }

    public async Task UpdateTaskInformation(TaskModel newTask)
    {
        try
        {
            await Upsert(newTask);
        }
        catch (Exception exception)
        {
            throw new TaskRepositoryException(exception, $"An error occurred while updating task information for task with Id {newTask.Id}");
        }
    }

    private DbSet<TaskEntity> GetTaskDbSet()
    {
        if (Context.Tasks is null)
            throw new InvalidOperationException("Database configuration not setup correctly");
        return Context.Tasks;
    }

    protected override TaskModel Map(TaskEntity entity)
    {
        return TaskMapper.Map(entity);
    }

    protected override TaskEntity Map(TaskModel model)
    {
        return TaskMapper.Map(model);
    }


}
