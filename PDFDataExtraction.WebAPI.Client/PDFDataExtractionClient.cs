using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PDFDataExtraction.Generic;

namespace PDFDataExtraction.WebAPI.Client
{
    public class PDFDataExtractionClient
    {
        private readonly string _apiEndpoint;
        private readonly IHttpClientFactory _httpClientFactory;

        public PDFDataExtractionClient(string apiEndpoint)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            
            _apiEndpoint = apiEndpoint;
        }

        public async Task<Document> ExtractDocumentFromPDF(Stream inputFileStream)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var requestUri = _apiEndpoint + "/api/PDFTextExtraction/detailed";
            
            using (var response = await SendFileToApi(inputFileStream, httpClient, requestUri))
            {
                response.EnsureSuccessStatusCode();
                var responseBodyAsString = await response.Content.ReadAsStringAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<Document>(responseBodyAsString);
                return deserializedResponse;
            }
        }
        
        public async Task<string> ExtractTextFromPDF(Stream inputFileStream)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var requestUri = _apiEndpoint + "/api/PDFTextExtraction/simple";
            
            using (var response = await SendFileToApi(inputFileStream, httpClient, requestUri))
            {
                response.EnsureSuccessStatusCode();
                var responseBodyAsString = await response.Content.ReadAsStringAsync();
                return responseBodyAsString;
            }
        }

        private static async Task<HttpResponseMessage> SendFileToApi(Stream inputFileStream, HttpClient httpClient, string requestUri)
        {
            using (var request = new MultipartFormDataContent())
            {
                request.Add(new StreamContent(inputFileStream), "file", "file");
                var response = await httpClient.PostAsync(requestUri, request);
                return response;
            }
        }
    }
}