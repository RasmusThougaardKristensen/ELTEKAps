using ELTEKAps.Management.ApplicationServices.Tasks.Create;
using Management.Messages.Tasks;
using Management.Worker.Service.SignalR;
using Rebus.Handlers;

namespace Management.Worker.Service.Tasks.Create;

public class RequestCreateTaskCommandHandler : IHandleMessages<RequestCreateTaskCommand>
{
    private readonly ICreateTaskService _createTaskService;
    private readonly ILogger<RequestCreateTaskCommandHandler> _logger;
    private readonly IMessageService _messageService;

    public RequestCreateTaskCommandHandler(ICreateTaskService createTaskService, ILogger<RequestCreateTaskCommandHandler> logger, IMessageService messageService)
    {
        _createTaskService = createTaskService;
        _logger = logger;
        _messageService = messageService;
    }

    public async Task Handle(RequestCreateTaskCommand message)
    {
        _logger.LogInformation("Trying to create task for requestId {RequestId}", message.RequestId);

        var taskId = await _createTaskService.CreateTask(message.RequestId);

        _logger.LogInformation("Trying to create task for requestId {RequestId}", message.RequestId);

        if (taskId != null) await this._messageService.NotifyTaskCompletion(message.RequestId, taskId);
    }
}
