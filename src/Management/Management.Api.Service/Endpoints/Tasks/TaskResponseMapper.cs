using ELTEKAps.Management.Api.Service.Endpoints.Tasks.GetTask;
using ELTEKAps.Management.Domain.Tasks;

namespace ELTEKAps.Management.Api.Service.Endpoints.Tasks;

public class TaskResponseMapper
{
    public static TaskResponse ToResponseModel(TaskModel task)
    {
        return new TaskResponse(
            task.Id,
            task.CreatedUtc,
            task.Title,
            task.Description,
            task.Status,
            task.Location,
            task.CustomerId,
            task.UserId,
            new List<TaskCommentResponse>(task.Comments.Select(TaskCommentMapper.ToResponseModel)),
            new List<TaskPhotoResponse>(task.Photos.Select(TaskPhotoMapper.ToResponseModel))
        );
    }

    public static IEnumerable<TaskResponse> ToResponseModels(IEnumerable<TaskModel> tasks)
    {
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));

        return tasks.Select(ToResponseModel);
    }
}
