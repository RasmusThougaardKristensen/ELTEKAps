namespace Management.Messages.External.Tasks.Update;
public sealed class TaskUpdateFailedEvent
{
    public TaskUpdateFailedEvent(Guid taskId, string errorMessage)
    {
        TaskId = taskId;
        ErrorMessage = errorMessage;
    }

    public Guid TaskId { get; set; }
    public string ErrorMessage { get; set; }
}
