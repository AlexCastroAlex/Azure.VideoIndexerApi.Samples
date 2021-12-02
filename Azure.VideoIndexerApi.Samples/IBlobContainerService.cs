
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.VideoIndexerApi.Samples
{
    public interface IBlobContainerService
    {
        string GetBlobReadSas(string blobName);
        Task UploadFileAsync(string path, string blobName);
        Task UploadDocumentToBlobAsync(string path, string blobName);
        CloudBlob DownloadFileFromBlob(string blobName, string path);
    }
}