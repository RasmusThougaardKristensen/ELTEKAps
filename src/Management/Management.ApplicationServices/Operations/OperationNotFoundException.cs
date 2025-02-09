namespace ELTEKAps.Management.ApplicationServices.Operations
{
    /// <summary>
    /// Thrown when an operation to be updated does not exist in the system.
    /// </summary>
    public class OperationNotFoundException : Exception
    {
        public OperationNotFoundException(string message) : base(message) { }
    }
}
