using ELTEKAps.Management.ApplicationServices.PowerAutomate;
using Management.Messages.External.Tasks.Update;
using Rebus.Handlers;

namespace Management.Worker.Service.Tasks;

public class TaskUpdateEventHandler : IHandleMessages<TaskUpdateSucceedEvent>
{
    private IPowerAutomateService _powerAutomateService;
    public TaskUpdateEventHandler(IPowerAutomateService powerAutomateService)
    {
        _powerAutomateService = powerAutomateService;
    }

    public async Task Handle(TaskUpdateSucceedEvent message)
    {
        await _powerAutomateService.TaskUpdated(message.TaskId);
    }
}
