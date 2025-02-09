namespace ELTEKAps.Management.ApplicationServices.Comments.Update
{
    /// <summary>
    /// Thrown when an exception occurs when updating a comment in the repository.
    /// </summary>
    public class CommentUpdateException : Exception
    {
        public CommentUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
