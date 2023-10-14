using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
//using AutoFixture;
//using AutoFixture.Xunit2;
using System.Collections.Generic;
using Xunit.Extensions.Ordering;
namespace API_Automation_QATest
{
    [Collection("Sequential")]
    public class APITests
    {
        private readonly HttpClient _client;
        private string NewObjectid;

        public APITests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.restful-api.dev/");
        }
        [Order(1)]
        [Fact]
        public async Task Test_GetAllObjects()
        {
            try
            {
                var response = await _client.GetAsync("objects");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
        [Order(2)]
        [Fact]
        public async Task Test_GetListofObjectsByIds()
        {
            try
            {
                int objectId1 = 3;
                int objectId2 = 5;
                int objectId3 = 10;

                var response = await _client.GetAsync($"objects?id={objectId1}&id={objectId2}&id={objectId3}");
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
        [Order(3)]
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

        [Order(4)]
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

                NewObjectid = responseObject.id;

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

        [Order(5)]
        [Fact]
        public async Task Test_UpdateObject()
        {
            try
            {
                NewObjectid = "ff8081818b1b4123018b2f35f8da1bcb";

                var json = "{\"name\":\"Sample Name EDITED\",\"data\":{\"year\":2024,\"price\":2001.00,\"CPU model\":\"Intel Core i10\",\"Hard disk size\":\"2 TB\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"objects/{NewObjectid}", content);
                response.EnsureSuccessStatusCode();


                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

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
        [Order(6)]
        [Fact]
        public async Task Test_UpdateObjectPartially()
        {
            try
            {
                NewObjectid = "ff8081818b1b4123018b2f35f8da1bcb";

                var json = "{\"name\": \"Sample Name EDITED AGAIN\"}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"objects/{NewObjectid}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

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
        [Order(7)]
        [Fact]
        public async Task Test_DeleteObject()
        {
            try
            {
                NewObjectid = "ff8081818b1b4123018b2f35f8da1bcb";

                var response = await _client.DeleteAsync($"objects/{NewObjectid}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                string message = responseObject.message;

                Assert.Equal($"Object with id = {NewObjectid} has been deleted.", responseObject.message);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
    }
}
