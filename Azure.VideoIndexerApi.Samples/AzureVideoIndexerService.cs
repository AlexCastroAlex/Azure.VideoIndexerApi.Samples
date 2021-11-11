using Azure.VideoIndexerApi.Samples.Configuration;
using Azure.VideoIndexerApi.Samples.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using VideoIndexerApiClient.Entities;

namespace Azure.VideoIndexerApi.Samples
{
    public class AzureVideoIndexerService : IAzureVideoIndexerService
    {
        private static string ApiUrl = "https://api.videoindexer.ai";

        // Location of Service (trial, or Azure region)
        private string m_Location;

        // ID of video indexer account
        private string m_AccountId;

        // API key for obtaining access tokens
        private string m_ApiKey;

        // API key for obtaining access tokens
        private bool m_AllowEdit;
        private readonly VideoIndexerConfig _options;

        // Cached access token
        private string m_AccountAccessToken;

        // Timeout for cached access token
        private DateTime m_AccountAccessTokenTimeStamp;


        public AzureVideoIndexerService(IOptions<VideoIndexerConfig> options)
        {
            // Set the location, account id and API key
            m_Location = options.Value.location;
            m_AccountId = options.Value.accountId;
            m_ApiKey = options.Value.ApiKey;
            m_AllowEdit = true;
            m_AccountAccessTokenTimeStamp = DateTime.MinValue;
        }


        public async Task<string> GetAccounts()
        {

            var client = GetHttpClient();

            // Add the API key to the default request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", m_ApiKey);

            var accountAccessTokenRequestResult = await
                client.GetAsync($"{ ApiUrl }/auth/{ m_Location }/Accounts");

            var json = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result;

            return json;
        }






        public async Task<string> GetAccountAccessTokenAsync()
        {
            // Check to see if we can reuse the cached access token
            if ((DateTime.UtcNow - m_AccountAccessTokenTimeStamp).TotalMinutes > 55)
            {
                var client = GetHttpClient();

                // Add the API key to the default request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", m_ApiKey);

                // Call the API to get the access token
                var accountAccessTokenRequestResult = await
                    client.GetAsync($"{ ApiUrl }/auth/{ m_Location }/Accounts/{ m_AccountId }/AccessToken?allowEdit={ m_AllowEdit }");

                // Parse the access token
                m_AccountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");
                m_AccountAccessTokenTimeStamp = DateTime.UtcNow;
            }
            return m_AccountAccessToken;
        }

        public async Task<string> GetReadOnlyVideoAccessTokenAsync(string videoId)
        {

            var client = GetHttpClient();

            // Add the API key to the default request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", m_ApiKey);

            var accountAccessTokenRequestResult = await
                client.GetAsync($"{ ApiUrl }/auth/{ m_Location }/Accounts/{ m_AccountId }/Videos/{ videoId }/AccessToken?allowEdit=false");

            var accessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");


            return accessToken;
        }

        public async Task<MediaAssetResults> ListVideosAsync()
        {
            // Get an access token and create the client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Get the results in JSON format
            var listVideosRequestResult = client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos?accessToken={accessToken}").Result;
            var listVideostJson = listVideosRequestResult.Content.ReadAsStringAsync().Result;

            // Parse and return the results
            var results = JsonConvert.DeserializeObject<MediaAssetResults>(listVideostJson);
            return results;
        }





        public async Task<string> IndexAsync(string blobSasUrl, string name)
        {
            // Get an access token and create the client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Create the API Method URL
            var content = new MultipartFormDataContent();
            var apiUrl = $"{ApiUrl}/{m_Location}/Accounts/{ m_AccountId }/Videos?accessToken={ accessToken }&privacy=private&videoUrl={ WebUtility.UrlEncode(blobSasUrl) }&name={ WebUtility.UrlEncode(name) }";

            // Call the Upload Video API Method
            var uploadRequestResult = client.PostAsync(apiUrl, content).Result;

            // Process and return the results
            var uploadResult = uploadRequestResult.Content.ReadAsStringAsync().Result;
            return uploadResult;
        }

        public async Task GetIndexAsync(string videoId)
        {
            // https://api.videoindexer.ai/{location}/Accounts/{accountId}/Videos/{videoId}/Index[?language][&reTranslate][&accessToken]

            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            var apiUrl = $"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{videoId}/Index?accessToken={accessToken}";

            var result = client.GetAsync(apiUrl).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;


            Console.WriteLine(resultJson);
        }


        public async Task UpdateVideoFaceAsync(string videoId, int faceId, string newName)
        {
            // https://api.videoindexer.ai/{location}/Accounts/{accountId}/Videos/{videoId}/Index/Faces/{faceId}[?newName][&personId][&createNewPerson][&accessToken]

            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            var apiUrl = $"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{videoId}/Index/Faces/{faceId}?newName={newName}&accessToken={accessToken}";
            var response = await client.PutAsync(apiUrl, new ByteArrayContent(new byte[0]));

        }


        public async Task GetIndexAsync(string videoId, string language)
        {
            // https://api.videoindexer.ai/{location}/Accounts/{accountId}/Videos/{videoId}/Index[?language][&reTranslate][&accessToken]

            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            var apiUrl = $"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{videoId}/Index?accessToken={accessToken}&language={language}";

            var result = client.GetAsync(apiUrl).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;


            Console.WriteLine(resultJson);
        }




        public async Task<SearchResults> SearchAsync(string searchText)
        {
            // Get an access token and create the client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Call the Search method and retrieve the response content
            var searchRequestResult = client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/Search?accessToken={accessToken}&scope=account&query={searchText}").Result;
            var searchResultJson = searchRequestResult.Content.ReadAsStringAsync().Result;

            //Console.WriteLine(searchResultJson);

            // Parse the JSON content to a results object and return it
            var results = JsonConvert.DeserializeObject<SearchResults>(searchResultJson);
            return results;

        }


        public async Task<SearchResults> SearchFaceAsync(string name)
        {
            // Get an access token and create the client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Call the Search method and retrieve the response content
            var searchRequestResult = client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/Search?accessToken={accessToken}&scope=account&face={name}").Result;
            var searchResultJson = searchRequestResult.Content.ReadAsStringAsync().Result;

            //Console.WriteLine(searchResultJson);

            // Parse the JSON content to a results object and return it
            var results = JsonConvert.DeserializeObject<SearchResults>(searchResultJson);
            return results;

        }




        public async Task<string> GetVideoPlayerWidgetAsync(string videoId)
        {
            // Get the access token and client
            var accessToken = await GetReadOnlyVideoAccessTokenAsync(videoId);
            var client = GetHttpClient();

            // Call the Get Get Video Player Widget method
            var result = client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{videoId}/PlayerWidget?accessToken={ accessToken }").Result;

            // Return the absolute URI from the redirect response
            var url = result.Headers.Location.AbsoluteUri;
            return url;
        }




        public async Task<string> GetVideoCaptions(string videoId, string format)
        {
            // Get the access token and client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Call the Get Video Captions method
            var response = await client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{ videoId }/Captions?format={ format }&accessToken={accessToken}");

            // Process and return the captions
            var responseText = response.Content.ReadAsStringAsync().Result;
            return responseText;
        }



        public async Task<string> GetVideoCaptions(string videoId, string format, string language)
        {
            // Get the access token and client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            // Call the Get Video Captions method
            var response = await client.GetAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{ videoId }/Captions?format={ format }&language={language}&accessToken={accessToken}");

            // Process and return the captions
            var responseText = response.Content.ReadAsStringAsync().Result;
            return responseText;
        }





        public async Task UpdateVideoTranscript(string videoId, string transcript)
        {
            // Encode the transcript to a byte array
            byte[] byteData = Encoding.UTF8.GetBytes(transcript);

            // Get the access token and client
            var accessToken = await GetAccountAccessTokenAsync();
            var client = GetHttpClient();

            HttpResponseMessage response;

            // Create the request content
            using (var content = new ByteArrayContent(byteData))
            {
                // Set the content type to Web Video Text Tracks Format (VTT)
                content.Headers.ContentType = new MediaTypeHeaderValue("text/vtt");

                // Send a put request
                response = await client.PutAsync($"{ApiUrl}/{m_Location}/Accounts/{m_AccountId}/Videos/{ videoId }/Index/Transcript?accessToken={accessToken}", content);
            }
        }


        private HttpClient GetHttpClient()
        {
            // Create a new HTTP Client
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            var client = new HttpClient(handler);
            return client;
        }
    }
}
