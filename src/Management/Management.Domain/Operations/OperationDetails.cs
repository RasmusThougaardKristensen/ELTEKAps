namespace ELTEKAps.Management.Domain.Operations;
/// <summary>
/// Represents details about an operation.
/// </summary>
public class OperationDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationDetails"/> class.
    /// </summary>
    /// <param name="createdBy">The user who created the operation.</param>
    /// <param name="correlationRequestId">The correlation request ID associated with the operation (optional).</param>
    public OperationDetails(string createdBy, string? correlationRequestId = null)
    {
        CreatedBy = createdBy;
        CorrelationRequestId = correlationRequestId;
    }

    public string? CorrelationRequestId { get; }
    public string CreatedBy { get; }
}
