namespace ELTEKAps.Management.ApplicationServices.Tasks.Get
{
    /// <summary>
    /// Thrown when an exception occurs when querying a task in the repository.
    /// </summary>
    public class TaskQueryException : Exception
    {
        public TaskQueryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
