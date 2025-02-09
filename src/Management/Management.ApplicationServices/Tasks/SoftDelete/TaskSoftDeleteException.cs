namespace ELTEKAps.Management.ApplicationServices.Tasks.SoftDelete
{
    public class TaskSoftDeleteException : Exception
    {
        /// <summary>
        /// Thrown when an exception occurs when deleting a task in the repository.
        /// </summary>
        public TaskSoftDeleteException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
