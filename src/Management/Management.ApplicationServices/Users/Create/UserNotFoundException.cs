namespace ELTEKAps.Management.ApplicationServices.Users.Create
{
    /// <summary>
    /// Thrown when a user is not found in the repository.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
