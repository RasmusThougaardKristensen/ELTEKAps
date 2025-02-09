namespace ELTEKAps.Management.Domain.BlobStorage
{
    public class BlobFileModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
