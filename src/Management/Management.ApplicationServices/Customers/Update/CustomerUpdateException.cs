namespace ELTEKAps.Management.ApplicationServices.Customers.Update
{
    public class CustomerUpdateException : Exception
    {
        /// <summary>
        /// Thrown when an exception occurs when updating a customer in the repository.
        /// </summary>
        public CustomerUpdateException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
