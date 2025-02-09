namespace ELTEKAps.Management.ApplicationServices.Customers.SoftDelete
{
    public class CustomerSoftDeleteException : Exception
    {
        /// <summary>
        /// Thrown when an exception occurs when deleting a customer in the repository.
        /// </summary>
        public CustomerSoftDeleteException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
