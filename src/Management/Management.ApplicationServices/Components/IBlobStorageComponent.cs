namespace ELTEKAps.Management.ApplicationServices.Components;
public interface IBlobStorageComponent
{
    Task<string> SaveFileToBlob(MemoryStream fileStream, string path);
}
