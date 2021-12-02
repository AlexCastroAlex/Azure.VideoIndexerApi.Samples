using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace Azure.VideoIndexerApi.Samples
{
    public class AzureSearchService : IAzureSearchService
    {

        private SearchClient _searchClient;
        private SearchIndexerClient _searchIndexerClient;
        private readonly IBlobContainerService _blobContainerService;

        public AzureSearchService(IBlobContainerService blobContainerService)
        {
            string indexName = "azureblob-index";

            // Get the service endpoint and API key from the environment
            Uri endpoint = new Uri("https://azsaco.search.windows.net");
            string key = "285A027999C3494D58BE3E8601E2705B";


            // Create a client
            AzureKeyCredential credential = new AzureKeyCredential(key);
            _searchClient = new SearchClient(endpoint, indexName, credential);
            _blobContainerService = blobContainerService;
            _searchIndexerClient = new SearchIndexerClient(endpoint, new AzureKeyCredential(key));
            _searchIndexerClient.CreateOrUpdateDataSourceConnection(new SearchIndexerDataSourceConnection(
                name: "documentblobindexer",
                type:SearchIndexerDataSourceType.AzureBlob,
                connectionString: "DefaultEndpointsProtocol=https;AccountName=saaco;AccountKey=JJW6AK4ljdiT5hPp9pPI6zm1aKtf/Iwdni/7XVc/t6qC39F9xbmOx8l1+2oC3lvIohRGx4AaYan72EkQm5QpXw==;EndpointSuffix=core.windows.net",
                container:new SearchIndexerDataContainer("documentblobindexer")
                ));;

        }

        public async Task<SearchResults<SearchDocument>> GetAllDocuments()
        {
            //rajout des options et des facets
            var option = new SearchOptions()
            {
                

            };
            option.Select.Add("people");
            option.Select.Add("metadata_storage_path");
            option.Select.Add("content");
            option.Facets.Add("people,count:20");
            option.Facets.Add("organizations,count:20");
            //end rajout des options et des facets
            var documents = await  _searchClient.SearchAsync<SearchDocument>("zifan", option);
            var searchResult = documents.Value.GetResults();
            var storagepath = DecodeBase64StoragePath(documents.Value.GetResults().First().Document["metadata_storage_path"].ToString());
            var document = _blobContainerService.DownloadFileFromBlob(String.Empty, storagepath);

            return documents.Value;
        }

        /// <summary>
        /// add document to blob and trigger indexer
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task AddDocumentToBlobAndIndex(string filepath)
        {
            var name = Path.GetFileName(filepath);
            await _blobContainerService.UploadDocumentToBlobAsync(filepath, name);
            _searchIndexerClient.RunIndexer("azureblob-indexer");
        }


        /// <summary>
        /// Suggest 
        /// </summary>
        /// <param name="highlights"></param>
        /// <param name="fuzzy"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<List<string>> SuggestAsync(bool highlights, bool fuzzy, string term)
        {

            // Setup the suggest parameters.
            var options = new SuggestOptions()
            {
                UseFuzzyMatching = fuzzy,
                Size = 8,
            };

            if (highlights)
            {
                options.HighlightPreTag = "<b>";
                options.HighlightPostTag = "</b>";
            }

            // Only one suggester can be specified per index. It is defined in the index schema.
            // The name of the suggester is set when the suggester is specified by other API calls.
            // The suggester for the hotel database is called "sg", and simply searches the hotel name.
            var suggestResult = await _searchClient.SuggestAsync<SearchDocument>(term, "sg", options).ConfigureAwait(false);

            // Convert the suggested query results to a list that can be displayed in the client.
            List<string> suggestions = suggestResult.Value.Results.Select(x => x.Text).ToList();

            // Return the list of suggestions.
            return suggestions;
        }



        private string DecodeBase64StoragePath(string encodedString)
        {
            var encodedStringWithoutTrailingCharacter = encodedString.Substring(0, encodedString.Length - 1);
            var encodedBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(encodedStringWithoutTrailingCharacter);
            return HttpUtility.UrlDecode(encodedBytes, Encoding.UTF8);
        }

    }

    public class SearchDocumentAzure
    {

    }

}
