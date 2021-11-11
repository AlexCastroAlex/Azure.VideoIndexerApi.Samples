using Azure.VideoIndexerApi.Samples.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.VideoIndexerApi.Samples
{
    public class BlobContainerService : IBlobContainerService
    {
        private CloudBlobContainer m_BlobContainer;
        public BlobContainerService(IOptions<BlobConfig> options)
        {
            m_BlobContainer = CloudStorageAccount.Parse(options.Value.ConnectionString).CreateCloudBlobClient().GetContainerReference(options.Value.ContainerName);
        }

        public Task UploadFileAsync(string path, string blobName)
        {
            var blob = m_BlobContainer.GetBlockBlobReference(blobName);
            return blob.UploadFromFileAsync(path);
        }


        public string GetBlobReadSas(string blobName)
        {
            var sharedAccessBlobPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            };
            var blob = m_BlobContainer.GetBlockBlobReference(blobName);
            var sas = blob.GetSharedAccessSignature(sharedAccessBlobPolicy);
            var sasUrl = blob.StorageUri.PrimaryUri.AbsoluteUri + sas;

            return sasUrl;
        }
    }
}
