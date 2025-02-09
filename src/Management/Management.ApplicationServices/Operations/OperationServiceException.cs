namespace ELTEKAps.Management.ApplicationServices.Operations
{
    /// <summary>
    /// A general exception indicating something went wrong in <see cref="OperationService"/>.
    /// </summary>
    public class OperationServiceException : Exception
    {
        public OperationServiceException(string message) : base(message) { }

        public OperationServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
