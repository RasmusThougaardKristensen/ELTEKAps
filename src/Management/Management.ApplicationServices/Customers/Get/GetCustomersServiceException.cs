namespace ELTEKAps.Management.ApplicationServices.Customers.Get
{
    public class GetCustomersServiceException : Exception
    {
        /// <summary>
        /// Thrown when an exception occurs when querying a customer
        /// </summary>
        public GetCustomersServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
