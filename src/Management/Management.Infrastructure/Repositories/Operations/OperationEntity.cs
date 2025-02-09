using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;

namespace ELTEKAps.Management.Infrastructure.Repositories.Operations;
public class OperationEntity : BaseEntity
{
    public string RequestId { get; }
    public Guid TaskId { get; }
    public string CreatedBy { get; }
    public OperationName OperationName { get; }
    public DateTime? CompletedUtc { get; }
    public OperationStatus Status { get; }
    public string? Data { get; }


    public OperationEntity(Guid id, DateTime createdUtc, DateTime modifiedUtc, bool deleted, string requestId, Guid taskId,
       string createdBy, OperationName operationName, OperationStatus status, DateTime? completedUtc, string? data) :
       base(id, createdUtc, modifiedUtc, deleted)
    {
        ModifiedUtc = modifiedUtc;
        RequestId = requestId;
        TaskId = taskId;
        CreatedBy = createdBy;
        OperationName = operationName;
        Status = status;
        CompletedUtc = completedUtc;
        Data = data;
    }
}
