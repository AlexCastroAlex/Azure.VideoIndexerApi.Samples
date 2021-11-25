using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Azure.VideoIndexerApi.Samples
{
    public class AzureSearchService : IAzureSearchService
    {

        private SearchClient _searchClient;
        public AzureSearchService()
        {
            string indexName = "azureblob-index";

            // Get the service endpoint and API key from the environment
            Uri endpoint = new Uri("https://azsaco.search.windows.net");
            string key = "285A027999C3494D58BE3E8601E2705B";

            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(key);
            _searchClient = new SearchClient(endpoint, indexName, credential);
        }

        public async Task GetAllDocuments()
        {
            var documents = _searchClient.Search<SearchDocument>("test");
        }
    }
}
