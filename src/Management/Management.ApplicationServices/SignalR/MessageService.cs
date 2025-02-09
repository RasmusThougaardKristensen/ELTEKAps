using Management.Worker.Service.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ELTEKAps.Management.ApplicationServices.SignalR
{
    public class MessageService : IMessageService
    {
        private readonly IHubContext<WorkerHub> _hubContext;
        private readonly ILogger<MessageService> _logger;


        public MessageService(IHubContext<WorkerHub> hubContext, ILogger<MessageService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyTaskCompletion(string requestId, string taskId)
        {
            this._logger.LogInformation("Notifying task completion. RequestId: {RequestId}, TaskId: {TaskId}", requestId, taskId);
            await _hubContext.Clients.All.SendAsync("NotifyTaskRequestId", requestId, taskId);
        }
    }
}
