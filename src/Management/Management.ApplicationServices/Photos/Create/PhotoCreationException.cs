namespace ELTEKAps.Management.ApplicationServices.Photos.Create
{
    public class PhotoCreationException : Exception
    {
        public PhotoCreationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
