using Azure.VideoIndexerApi.Samples.Entities;
using VideoIndexerApiClient.Entities;

namespace Azure.VideoIndexerApi.Samples
{
    public interface IAzureVideoIndexerService
    {
        Task<string> GetAccountAccessTokenAsync();
        Task<string> GetAccounts();
        Task GetIndexAsync(string videoId);
        Task GetIndexAsync(string videoId, string language);
        Task<string> GetReadOnlyVideoAccessTokenAsync(string videoId);
        Task<string> GetVideoCaptions(string videoId, string format);
        Task<string> GetVideoCaptions(string videoId, string format, string language);
        Task<string> GetVideoPlayerWidgetAsync(string videoId);
        Task<string> IndexAsync(string blobSasUrl, string name);
        Task<MediaAssetResults> ListVideosAsync();
        Task<SearchResults> SearchAsync(string searchText);
        Task<SearchResults> SearchFaceAsync(string name);
        Task UpdateVideoFaceAsync(string videoId, int faceId, string newName);
        Task UpdateVideoTranscript(string videoId, string transcript);
    }
}