namespace Management.Messages.External.Tasks.Update;
public sealed class TaskUpdateSucceedEvent
{
    public Guid TaskId { get; set; }

    public TaskUpdateSucceedEvent(Guid taskId)
    {
        TaskId = taskId;
    }
}
