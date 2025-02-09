namespace Management.Messages.External.Tasks.Create;
public class CreateTaskSucceedEvent
{
    public Guid TaskId { get; set; }

    public CreateTaskSucceedEvent(Guid taskId)
    {
        TaskId = taskId;
    }
}
