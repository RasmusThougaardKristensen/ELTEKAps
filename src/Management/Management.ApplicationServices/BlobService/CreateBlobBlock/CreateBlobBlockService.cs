using ELTEKAps.Management.ApplicationServices.Components;

namespace ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock
{
    public class CreateBlobBlockService : ICreateBlobBlockService
    {
        private readonly IBlobStorageComponent _blobStorageComponent;

        public CreateBlobBlockService(IBlobStorageComponent blobStorageComponent)
        {
            _blobStorageComponent = blobStorageComponent;
        }

        public async Task<string> CreateBlobBlock(string fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData))
            {
                throw new ArgumentException("Invalid file data.");
            }

            // Validate and extract the MIME type
            const string base64Prefix = "data:";
            const string base64Marker = ";base64,";
            var prefixEnd = fileData.IndexOf(base64Marker, StringComparison.OrdinalIgnoreCase);
            if (!fileData.StartsWith(base64Prefix) || prefixEnd == -1)
            {
                throw new FormatException("Invalid Base64 string format.");
            }

            var mimeType = fileData.Substring(base64Prefix.Length, prefixEnd - base64Prefix.Length);
            var base64Content = fileData.Substring(prefixEnd + base64Marker.Length);

            // Map MIME type to file extension
            string fileExtension = FileExtension(mimeType);

            // Decode Base64 data
            var fileBytes = DecodeBase64(base64Content);

            // Generate a random GUID for the file name
            var fileName = $"{Guid.NewGuid()}.{fileExtension}";

            using var memoryStream = new MemoryStream(fileBytes);

            var uri =  await _blobStorageComponent.SaveFileToBlob(memoryStream, fileName);

            return uri;
        }

        private string FileExtension(string? mimeType)
        {
            string fileExtension = mimeType switch
            {
                "image/jpeg" or "image/jpg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                _ => throw new NotSupportedException($"Unsupported MIME type: {mimeType}")
            };

            return fileExtension;
        }

        private byte[] DecodeBase64(string base64Content)
        {
            try
            {
                return Convert.FromBase64String(base64Content);
            }
            catch (FormatException ex)
            {
                throw new FormatException("The provided Base64 string is not valid.", ex);
            }
        }
    }
}
