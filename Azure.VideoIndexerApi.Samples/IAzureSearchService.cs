using Azure.Search.Documents.Models;

namespace Azure.VideoIndexerApi.Samples
{
    public interface IAzureSearchService
    {
        Task<SearchResults<SearchDocument>> GetAllDocuments();
        Task AddDocumentToBlobAndIndex(string filepath);

        Task<List<string>> SuggestAsync(bool highlights, bool fuzzy, string term);
    }
}