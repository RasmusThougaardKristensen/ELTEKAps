namespace ELTEKAps.Management.ApplicationServices.Users.Create
{
    /// <summary>
    /// Thrown when an exception occurs when creating a user in the repository.
    /// </summary>
    public class UserCreationException : Exception
    {
        public UserCreationException(string message) : base(message) { }
        public UserCreationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
