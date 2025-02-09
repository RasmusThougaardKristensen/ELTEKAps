namespace ELTEKAps.Management.ApplicationServices.Users.Create
{
    /// <summary>
    /// Thrown when an exception occurs when updating a user in the repository.
    /// </summary>
    public class UserUpdateException : Exception
    {
        public UserUpdateException(string message) : base(message) { }
        public UserUpdateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
