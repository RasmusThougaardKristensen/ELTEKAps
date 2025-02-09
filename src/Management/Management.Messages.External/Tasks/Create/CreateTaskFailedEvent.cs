namespace Management.Messages.External.Tasks.Create;
public class CreateTaskFailedEvent
{
    public string RequestId { get; }
    public string errorMessage { get; }

    public CreateTaskFailedEvent(string requestId, string errorMessage)
    {
        RequestId = requestId;
        this.errorMessage = errorMessage;
    }
}
