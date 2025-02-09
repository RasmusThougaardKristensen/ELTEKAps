namespace ELTEKAps.Management.ApplicationServices.Tasks
{
    /// <summary>
    /// Thrown when an exception occurs when deleting a task in the repository.
    /// </summary>
    public class TaskOperationException : Exception
    {
        public TaskOperationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
