namespace ELTEKAps.Management.ApplicationServices.Comments.Delete
{
    /// <summary>
    /// Thrown when an exception occurs when deleting a comment in the repository.
    /// </summary>
    public class CommentSoftDeleteException : Exception
    {
        public CommentSoftDeleteException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
