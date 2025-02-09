namespace Management.Messages.Tasks;
public sealed class RequestCreateTaskCommand
{
    public string RequestId { get; }

    public RequestCreateTaskCommand(string requestId)
    {
        RequestId = requestId;
    }
}
