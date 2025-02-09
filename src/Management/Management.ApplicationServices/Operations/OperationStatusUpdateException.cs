namespace ELTEKAps.Management.ApplicationServices.Operations
{
    /// <summary>
    /// Thrown when attempting to set an operation to an invalid status.
    /// </summary>
    public class OperationStatusUpdateException : Exception
    {
        public OperationStatusUpdateException(string message) : base(message) { }
    }
}
