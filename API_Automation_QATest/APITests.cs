using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API_Automation_QATest
{
    public class APITests
    {
        private readonly HttpClient _client;
        private int NewObjectid;

        public APITests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://restful-api.dev/");
        }

        [Fact]
        public async Task Test_GetAllObjects()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("objects");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Add more assertions as needed for the response content.
            Console.WriteLine("Test Executed");
        }

        [Fact]
        public async Task Test_GetSingleObjectById()
        {
            // Arrange
            int objectId = 1;

            // Act
            var response = await _client.GetAsync($"objects/{objectId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Add more assertions as needed for the response content.
            Console.WriteLine("Test Executed");
        }

        [Fact]
        public async Task Test_AddObjectUsingPost()
        {
            // Arrange
            var jsonData = "{\"name\":\"Sample Name\",\"data\":{\"year\":2023,\"price\":2000.00,\"CPU model\":\"Intel Core i9\",\"Hard disk size\":\"1 TB\"}}";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("objects", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            // Add more assertions as needed for the response content.

            if (response.StatusCode == HttpStatusCode.Created)
            {
                // Parse the response content to extract the ID.
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                // Access the ID property from the response object.
                NewObjectid = responseObject.Id;

            }
            Console.WriteLine("Test Executed");
        }

        [Fact]
        public async Task Test_UpdateObjectPartially()
        {
            // Arrange
            int objectId = NewObjectid;
            var json = "{\"name\": \"Sample Name EDITED\"";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"objects/{objectId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Add more assertions as needed for the response content.
            Console.WriteLine("Test Executed");
        }

        [Fact]
        public async Task Test_DeleteObjectUsingDelete()
        {
            // Arrange
            int objectId = 1;

            // Act
            var response = await _client.DeleteAsync($"objects/{objectId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Console.WriteLine("Test Executed");
        }
    }
}
