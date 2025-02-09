using ELTEKAps.Management.ApplicationServices.Tasks.Update;
using Management.Messages.Tasks.Update;
using Management.Worker.Service.SignalR;
using Rebus.Bus;
using Rebus.Handlers;

namespace Management.Worker.Service.Tasks.Update;

public class RequestUpdateTaskCommandHandler : IHandleMessages<RequestUpdateTaskCommand>
{
    private readonly IUpdateTaskService _updateTaskService;
    private readonly IBus _bus;
    private readonly ILogger<RequestUpdateTaskCommandHandler> _logger;
    private readonly IMessageService _messageService;


    public RequestUpdateTaskCommandHandler(IUpdateTaskService updateTaskService, IBus bus, ILogger<RequestUpdateTaskCommandHandler> logger,
        IMessageService messageService)
    {
        _updateTaskService = updateTaskService;
        _bus = bus;
        _logger = logger;
        _messageService = messageService;
    }

    public async Task Handle(RequestUpdateTaskCommand message)
    {
        _logger.LogInformation("Trying to update task with id {TaskId}", message.TaskId);

        await _updateTaskService.UpdateTask(message.RequestId, message.TaskId);

        _logger.LogInformation("Completed to update Task with id {TaskId}", message.TaskId);

        await this._messageService.NotifyTaskCompletion(message.RequestId, message.TaskId.ToString());
    }
}
