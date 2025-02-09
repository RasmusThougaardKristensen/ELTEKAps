namespace ELTEKAps.Management.ApplicationServices.Users.Get
{
    /// <summary>
    /// Thrown when an exception occurs when querying a user in the repository.
    /// </summary>
    public class UserQueryException : Exception
    {
        public UserQueryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
