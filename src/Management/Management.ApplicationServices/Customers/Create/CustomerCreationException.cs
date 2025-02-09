namespace ELTEKAps.Management.ApplicationServices.Customers.Create
{
    public class CustomerCreationException : Exception
    {
        /// <summary>
        /// Thrown when an exception occurs when creating a customer in the repository.
        /// </summary>
        public CustomerCreationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
