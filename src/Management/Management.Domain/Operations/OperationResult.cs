namespace ELTEKAps.Management.Domain.Operations;
/// <summary>
/// Represents the result of an operation.
/// </summary>
public sealed class OperationResult
{
    public OperationResultStatus Status { get; }
    private Operation? Operation { get; }
    private string? Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <param name="status">The status of the operation result.</param>
    /// <param name="operation">The associated operation, if applicable.</param>
    /// <param name="message">The error message, if applicable.</param>
    private OperationResult(OperationResultStatus status, Operation? operation, string? message)
    {
        Status = status;
        Operation = operation;
        Message = message;
    }

    /// <summary>
    /// Creates an operation result indicating the operation was accepted.
    /// </summary>
    /// <param name="operation">The accepted operation.</param>
    /// <returns>An operation result indicating the operation was accepted.</returns>
    public static OperationResult Accepted(Operation operation) => new(OperationResultStatus.Accepted, operation, null);

    /// <summary>
    /// Creates an operation result indicating the operation was completed.
    /// </summary>
    /// <param name="operation">The completed operation.</param>
    /// <returns>An operation result indicating the operation was completed.</returns>
    public static OperationResult Completed(Operation operation) => new(OperationResultStatus.Completed, operation, null);

    /// <summary>
    /// Creates an operation result indicating an invalid state.
    /// </summary>
    /// <param name="message">The error message indicating the invalid state.</param>
    /// <returns>An operation result indicating an invalid state.</returns>
    public static OperationResult InvalidState(string message) => new(OperationResultStatus.InvalidState, null, message);

    /// <summary>
    /// Gets the associated operation.
    /// </summary>
    /// <returns>The associated operation.</returns>
    public Operation GetOperation()
    {
        if (Status is not (OperationResultStatus.Accepted or OperationResultStatus.Completed) || Operation is null)
            throw new InvalidOperationException("Can't get operation on not ok result");
        return Operation;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <returns>The error message.</returns>
    public string GetMessage()
    {
        if (Status is not OperationResultStatus.InvalidState || Message is null)
        {
            throw new InvalidOperationException("Can't get error message on not failed result");
        }

        return Message;
    }
}

/// <summary>
/// Represents the status of an operation result.
/// </summary>
public enum OperationResultStatus
{
    Accepted,
    InvalidState,
    Completed
}