namespace ELTEKAps.Management.ApplicationServices.Customers
{
    public class CustomerOperationException : Exception
    {
        public CustomerOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
