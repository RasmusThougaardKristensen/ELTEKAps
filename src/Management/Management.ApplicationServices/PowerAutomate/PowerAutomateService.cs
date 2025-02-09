using ELTEKAps.Management.ApplicationServices.Components;
using ELTEKAps.Management.ApplicationServices.Repositories.Tasks;
using ELTEKAps.Management.ApplicationServices.Repositories.Users;

namespace ELTEKAps.Management.ApplicationServices.PowerAutomate;
public class PowerAutomateService : IPowerAutomateService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPowerAutomateComponent _powerAutomateComponent;

    public PowerAutomateService(ITaskRepository taskRepository, IUserRepository userRepository, IPowerAutomateComponent powerAutomateComponent)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _powerAutomateComponent = powerAutomateComponent;
    }

    public async Task TaskUpdated(Guid taskId)
    {
        var task = await _taskRepository.GetTaskById(taskId);
        if (task is null)
        {
            throw new Exception();
        }

        var user = await _userRepository.GetById(task.UserId);
        if (user is null)
        {
            throw new Exception();
        }

        await _powerAutomateComponent.Notify(user.Email, user.Name, task.Title);
    }

    public Task TaskUpdateFailed(Guid taskId, string errorMessage)
    {
        throw new NotImplementedException();
    }
}
