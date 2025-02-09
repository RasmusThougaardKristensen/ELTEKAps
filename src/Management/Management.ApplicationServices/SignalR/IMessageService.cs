namespace Management.Worker.Service.SignalR
{
    public interface IMessageService
    {
        Task NotifyTaskCompletion(string requestId, string taskId);
    }
}
