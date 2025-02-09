namespace ELTEKAps.Management.ApplicationServices.Comments
{
    /// <summary>
    /// Thrown when an exception occurs while queuing the update operation for a comment.
    /// </summary>
    public class CommentOperationException : Exception
    {
        public CommentOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
