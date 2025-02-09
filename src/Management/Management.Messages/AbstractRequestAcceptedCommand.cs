namespace Management.Messages;
public abstract class AbstractRequestAcceptedCommand
{
    public string RequestId { get; }
    protected AbstractRequestAcceptedCommand(string requestId)
    {
        RequestId = requestId;
    }
}
