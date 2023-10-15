using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API_Automation_QATest
{
    class APIClientClass
    {
        private readonly HttpClient _httpClient;

        public APIClientClass(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<HttpResponseMessage> GetAllObjects()
        {
            return await _httpClient.GetAsync("objects");
        }

        public async Task<HttpResponseMessage> GetObjectsByIds(int[] objectIds)
        {
            if (objectIds == null || objectIds.Length == 0)
            {
                throw new ArgumentException("Object IDs cannot be null or empty.", nameof(objectIds));
            }

            string idsQuery = string.Join("&", objectIds.Select(id => $"id={id}"));


            string requestUri = $"objects?{idsQuery}";

            return await _httpClient.GetAsync(requestUri);
        }
        public async Task<HttpResponseMessage> GetObjectbyID(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentException("Object ID cannot be null or empty.", nameof(objectId));
            }

            string requestUri = $"objects/{objectId}";

            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> AddObject(object jsonData)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync("objects", jsonContent);
        }


        public async Task<APIResponse> UpdateObject(string objectId, object data)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"objects/{objectId}", jsonContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<APIResponse>(responseContent);
        }

        public async Task<APIResponse> GetObject(string objectId)
        {
            var response = await _httpClient.GetAsync($"objects/{objectId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get object: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<APIResponse>(responseContent);
        }

        public async Task<APIResponse> DeleteObject(string objectId)
        {
            var response = await _httpClient.DeleteAsync($"objects/{objectId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to delete object: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<APIResponse>(responseContent);
        }

    }
}
