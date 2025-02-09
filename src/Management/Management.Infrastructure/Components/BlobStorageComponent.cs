using Azure.Storage.Blobs;
using ELTEKAps.Management.ApplicationServices.Components;

namespace ELTEKAps.Management.Infrastructure.Components;
public class BlobStorageComponent : IBlobStorageComponent
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageComponent(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async  Task<string> SaveFileToBlob(MemoryStream fileStream, string path)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(Constants.FilePaths.BlobStorage);

        bool isExist = await blobContainer.ExistsAsync();
        if (!isExist)
        {
            await blobContainer.CreateAsync();
        }

        var blob = blobContainer.GetBlobClient(path);

        await blob.UploadAsync(fileStream);

        return blob.Uri.ToString();
    }
}
