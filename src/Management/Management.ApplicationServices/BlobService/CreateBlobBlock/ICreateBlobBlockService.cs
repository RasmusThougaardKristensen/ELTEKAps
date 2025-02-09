namespace ELTEKAps.Management.ApplicationServices.BlobService.CreateBlobBlock
{
    public interface ICreateBlobBlockService
    {
        Task<string> CreateBlobBlock(string fileData);

    }
}
