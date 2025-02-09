namespace Management.Messages.Tasks.Update;
public class RequestUpdateTaskCommand
{
    public string RequestId { get; }
    public Guid TaskId { get; }

    public RequestUpdateTaskCommand(string requestId, Guid taskId)
    {
        RequestId = requestId;
        TaskId = taskId;
    }
}
