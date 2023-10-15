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

        [Theory]
        [InlineData("Sample Name", 2023, 2000.00, "Intel Core i9", "1 TB")]
        [InlineData("Another Sample Name", 2022, 1800.00, "AMD Ryzen", "512 GB")]
        public async Task Test_AddObject(
    string name, int year, decimal price, string cpuModel, string hardDiskSize)
        {
            try
            {
                var jsonData = new
                {
                    name = name,
                    data = new
                    {
                        year = year,
                        price = price,
                        CPUModel = cpuModel,
                        HardDiskSize = hardDiskSize
                    }
                };
                var content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("objects", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                Assert.Equal(name, responseObject.name);
                Assert.Equal(year, responseObject.data.year);
                Assert.Equal(price, responseObject.data.price);
                Assert.Equal(cpuModel, responseObject.data.CPUModel);
                Assert.Equal(hardDiskSize, responseObject.data.HardDiskSize);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }


        [Theory]
        [InlineData("Sample Name", "Sample Name EDITED", 2023, 2024, 2000.00, 2001.00, "Intel Core i9", "Intel Core i10", "1 TB", "2 TB")]
        [InlineData("Another Name", "Another Name EDITED", 2022, 2025, 1800.00, 1900.00, "AMD Ryzen", "AMD Ryzen 2", "512 GB", "1 TB")]
        public async Task Test_UpdateObject(
    string originalName, string updatedName, int originalYear, int updatedYear,
    decimal originalPrice, decimal updatedPrice, string originalCpuModel,
    string updatedCpuModel, string originalHardDiskSize, string updatedHardDiskSize)
        {
            try
            {
                var originalJsonData = new
                {
                    name = originalName,
                    data = new
                    {
                        year = originalYear,
                        price = originalPrice,
                        CPUModel = originalCpuModel,
                        HardDiskSize = originalHardDiskSize
                    }
                };
                var originalContent = new StringContent(JsonConvert.SerializeObject(originalJsonData), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("objects", originalContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                NewObjectid = responseObject.id;

                var updatedJsonData = new
                {
                    name = updatedName,
                    data = new
                    {
                        year = updatedYear,
                        price = updatedPrice,
                        CPUModel = updatedCpuModel,
                        HardDiskSize = updatedHardDiskSize
                    }
                };
                var updatedContent = new StringContent(JsonConvert.SerializeObject(updatedJsonData), Encoding.UTF8, "application/json");

                var updateResponse = await _client.PutAsync($"objects/{NewObjectid}", updatedContent);
                updateResponse.EnsureSuccessStatusCode();

                var updateResponseContent = await updateResponse.Content.ReadAsStringAsync();
                var updateResponseObject = JsonConvert.DeserializeObject<ApiResponse>(updateResponseContent);

                Assert.Equal(updatedName, updateResponseObject.name);
                Assert.Equal(updatedYear, updateResponseObject.data.year);
                Assert.Equal(updatedPrice, updateResponseObject.data.price);
                Assert.Equal(updatedCpuModel, updateResponseObject.data.CPUModel);
                Assert.Equal(updatedHardDiskSize, updateResponseObject.data.HardDiskSize);
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
                var newJsonObject = new
                {
                    name = "Sample Name",
                    data = new
                    {
                        year = 2024,
                        price = 2001.00,
                        CPUModel = "Intel Core i10",
                        HardDiskSize = "2 TB"
                    }
                };

                var newJson = JsonConvert.SerializeObject(newJsonObject);
                var newContent = new StringContent(newJson, Encoding.UTF8, "application/json");

                var newResponse = await _client.PostAsync("objects", newContent);
                newResponse.EnsureSuccessStatusCode();

                var newResponseContent = await newResponse.Content.ReadAsStringAsync();
                var newResponseObject = JsonConvert.DeserializeObject<ApiResponse>(newResponseContent);

                Assert.NotNull(newResponseObject.id);

                var objectId = newResponseObject.id;
                var updateJson = new
                {
                    name = "Sample Name EDITED AGAIN"
                };

                var updateContent = new StringContent(JsonConvert.SerializeObject(updateJson), Encoding.UTF8, "application/json");

                var updateResponse = await _client.PutAsync($"objects/{objectId}", updateContent);
                updateResponse.EnsureSuccessStatusCode();

                var updateResponseContent = await updateResponse.Content.ReadAsStringAsync();
                var updateResponseObject = JsonConvert.DeserializeObject<ApiResponse>(updateResponseContent);

                Assert.NotNull(updateResponseObject);

                Assert.Equal("Sample Name EDITED AGAIN", updateResponseObject.name);

            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [Theory]
        [InlineData("Sample Name", 2024, 2001.00, "Intel Core i10", "2 TB")]
        public async Task Test_DeleteObject(string name, int year, decimal price, string cpuModel, string hardDiskSize)
        {
            try
            {
                var jsonData = new
                {
                    name = name,
                    data = new
                    {
                        year = year,
                        price = price,
                        CPUModel = cpuModel,
                        HardDiskSize = hardDiskSize
                    }
                };

                var Json = JsonConvert.SerializeObject(jsonData);
                var content = new StringContent(Json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("objects", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                var objectId = responseObject.id;

                var deleteResponse = await _client.DeleteAsync($"objects/{objectId}");
                deleteResponse.EnsureSuccessStatusCode();

                var deleteResponseContent = await deleteResponse.Content.ReadAsStringAsync();
                var deleteResponseObject = JsonConvert.DeserializeObject<ApiResponse>(deleteResponseContent);

                string message = deleteResponseObject.message;

                Assert.Equal($"Object with id = {objectId} has been deleted.", deleteResponseObject.message);
            }
            catch (HttpRequestException ex)
            {
                Assert.True(false, ex.Message);
            }
        }
    }
}
