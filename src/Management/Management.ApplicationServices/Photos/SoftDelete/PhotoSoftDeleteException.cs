namespace ELTEKAps.Management.ApplicationServices.Photos.SoftDelete
{
    public class PhotoSoftDeleteException : Exception
    {
        public PhotoSoftDeleteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
