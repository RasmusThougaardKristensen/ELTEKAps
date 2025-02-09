namespace ELTEKAps.Management.ApplicationServices.Photos
{
    public class PhotoOperationException : Exception
    {
        public PhotoOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
