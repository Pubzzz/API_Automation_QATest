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
        private string NewObjectid;

        public APITests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.restful-api.dev/");
        }

        [Fact]
        public async Task Test_GetAllObjects()
        {
            try
            {
                var response = await _client.GetAsync("objects");
                response.EnsureSuccessStatusCode(); // Ensure HTTP status code is success (200).

                // Add more assertions as needed for the response content.
                // For example, check the response content or data format.
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception (e.g., network error).
                // You can log the error or fail the test.
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_GetListofObjectsByIds()
        {
            try
            {
                int objectId1 = 3;
                int objectId2 = 5;
                int objectId3 = 10;

                var response = await _client.GetAsync($"objects/?id={objectId1}&id={objectId2}&id={objectId3}");
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_GetSingleObjectById()
        {
            try
            {
                int objectId = 7;

                var response = await _client.GetAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }


        [Fact]
        public async Task Test_AddObject()
        {
            try
            {
                var jsonData = "{\"name\":\"Sample Name\",\"data\":{\"year\":2023,\"price\":2000.00,\"CPU model\":\"Intel Core i9\",\"Hard disk size\":\"1 TB\"}}";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("objects", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                // Access the "id" property from the response object (it's a string).
                NewObjectid = responseObject.id;
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_VerifyObjectCreated()
        {
            try
            {
                string objectId = NewObjectid;

                var response = await _client.GetAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                string name = responseObject.name;
                int year = responseObject.data.year;
                decimal price = responseObject.data.price;
                string cpumodel = responseObject.data.CPUModel;
                string harddisksize = responseObject.data.HardDiskSize;

                // Assert that the retrieved values match the expected values
                Assert.Equal("Sample Name", responseObject.name);
                Assert.Equal(2023, responseObject.data.year);
                Assert.Equal(2000.00m, responseObject.data.price);
                Assert.Equal("Intel Core i9", responseObject.data.CPUModel);
                Assert.Equal("1 TB", responseObject.data.HardDiskSize);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_UpdateObject()
        {
            try
            {
                string objectId = NewObjectid;
                var json = "{\"name\":\"Sample Name EDITED\",\"data\":{\"year\":2024,\"price\":2001.00,\"CPU model\":\"Intel Core i10\",\"Hard disk size\":\"2 TB\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"objects/{objectId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            { 
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_VerifyObjectUpdated()
        {
            try
            {
                string objectId = NewObjectid;

                var response = await _client.GetAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                string name = responseObject.name;
                int year = responseObject.data.year;
                decimal price = responseObject.data.price;
                string cpumodel = responseObject.data.CPUModel;
                string harddisksize = responseObject.data.HardDiskSize;

                Assert.Equal("Sample Name EDITED", responseObject.name);
                Assert.Equal(2024, responseObject.data.year);
                Assert.Equal(2001.00m, responseObject.data.price);
                Assert.Equal("Intel Core i10", responseObject.data.CPUModel);
                Assert.Equal("2 TB", responseObject.data.HardDiskSize);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_UpdateObjectPartially()
        {
            try
            {
                string objectId = NewObjectid;
                var json = "{\"name\": \"Sample Name EDITED AGAIN\"}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"objects/{objectId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_VerifyObjectUpdatedPartially()
        {
            try
            {
                string objectId = NewObjectid;

                var response = await _client.GetAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                string name = responseObject.name;
                int year = responseObject.data.year;
                decimal price = responseObject.data.price;
                string cpumodel = responseObject.data.CPUModel;
                string harddisksize = responseObject.data.HardDiskSize;

                Assert.Equal("Sample Name EDITED AGAIN", responseObject.name);
                Assert.Equal(2024, responseObject.data.year);
                Assert.Equal(2001.00m, responseObject.data.price);
                Assert.Equal("Intel Core i10", responseObject.data.CPUModel);
                Assert.Equal("2 TB", responseObject.data.HardDiskSize);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Fact]
        public async Task Test_DeleteObject()
        {
            try
            {
                string objectId = NewObjectid;

                var response = await _client.DeleteAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
        [Fact]
        public async Task Test_VerifyObjectDeleted()
        {
            try
            {
                string objectId = NewObjectid;

                var response = await _client.GetAsync($"objects/{objectId}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
    }
}
