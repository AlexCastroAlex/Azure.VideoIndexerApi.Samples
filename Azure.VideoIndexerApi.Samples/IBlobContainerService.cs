
namespace Azure.VideoIndexerApi.Samples
{
    public interface IBlobContainerService
    {
        string GetBlobReadSas(string blobName);
        Task UploadFileAsync(string path, string blobName);
    }
}