namespace ELTEKAps.Management.ApplicationServices.Comments.Create
{
    /// <summary>
    /// Thrown when an exception occurs when creating a comment in the repository.
    /// </summary>
    public class CommentCreationException : Exception
    {
        public CommentCreationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
