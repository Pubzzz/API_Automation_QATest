using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using AutoFixture.Xunit2;
using Xunit.Extensions.Ordering;
using Moq;
using System.Threading;
using Moq.Protected;
using System.Net;

namespace API_Automation_QATest
{
    [Collection("Sequential")]
    public class APITests
    {
        private readonly HttpClient _client;
        private APIClientClass _apiClient;

        public APITests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.restful-api.dev/");
            _apiClient = new APIClientClass(_client);
        }

        [Order(1)]
        [Fact]
        public async Task Test_GetAllObjects()
        {
            // Arrange: Create a mock HTTP handler and an HTTP client with the mock handler.
            var mockHttp = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHttp.Object);
            var apiClient = new APIClientClass(httpClient);

            // Arrange: Set up a mock response for GetAllObjects with an OK status code.
            var expectedApiResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            // Arrange: Configure the mock HTTP handler to return the mock response when a request is made.
            mockHttp
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("objects")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedApiResponse);
            // Act: Make the HTTP request to GetAllObjects using the API client.
            var response = await _apiClient.GetAllObjects();
            // Assert: Ensure that the HTTP response has a successful status code (OK).
            response.EnsureSuccessStatusCode();
        }

        [Order(2)]
        [Fact]
        public async Task Test_GetListofObjectsByIds()
        {
            int objectId1 = 3;
            int objectId2 = 5;
            int objectId3 = 10;

            var response = await _apiClient.GetObjectsByIds(new[] { objectId1, objectId2, objectId3 });
            response.EnsureSuccessStatusCode();
        }

        [Order(3)]
        [Fact]
        public async Task Test_GetSingleObjectById()
        {
            string objectId = "7";

            var response = await _apiClient.GetObjectbyID(objectId);
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineAutoData("Sample Name", 2023, 2000.00, "Intel Core i9", "1 TB")]
        public async Task Test_AddObject(
            string name, int year, decimal price, string cpuModel, string hardDiskSize)
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

            // Act: Add the object using the API client
            var response = await _apiClient.AddObject(jsonData);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<APIResponse>(responseContent);

            Assert.Equal(name, responseObject.name);
            Assert.Equal(year, responseObject.data.year);
            Assert.Equal(price, responseObject.data.price);
            Assert.Equal(cpuModel, responseObject.data.CPUModel);
            Assert.Equal(hardDiskSize, responseObject.data.HardDiskSize);
        }



        [Theory]
        [InlineAutoData("Sample Name", "Sample Name EDITED", 2023, 2024, 2000.00, 2001.00, "Intel Core i9", "Intel Core i10", "1 TB", "2 TB")]
        [InlineAutoData("Another Name", "Another Name EDITED", 2022, 2025, 1800.00, 1900.00, "AMD Ryzen", "AMD Ryzen 2", "512 GB", "1 TB")]
        public async Task Test_UpdateObject(
            string originalName, string updatedName, int originalYear, int updatedYear,
            decimal originalPrice, decimal updatedPrice, string originalCpuModel,
            string updatedCpuModel, string originalHardDiskSize, string updatedHardDiskSize)
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

            // Act: Add the original object
            var originalResponse = await _apiClient.AddObject(originalJsonData);
            var originalResponseContent = await originalResponse.Content.ReadAsStringAsync();
            var originalResponseObject = JsonConvert.DeserializeObject<APIResponse>(originalResponseContent);
            var NewObjectid = originalResponseObject.id;

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

            // Act: Update the object
            var updateResponse = await _apiClient.UpdateObject(NewObjectid, updatedJsonData);

            // Assert
            Assert.Equal(updatedName, updateResponse.name);
            Assert.Equal(updatedYear, updateResponse.data.year);
            Assert.Equal(updatedPrice, updateResponse.data.price);
            Assert.Equal(updatedCpuModel, updateResponse.data.CPUModel);
            Assert.Equal(updatedHardDiskSize, updateResponse.data.HardDiskSize);
        }


        [Order(6)]
        [Fact]
        public async Task Test_UpdateObjectPartially()
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

            var newResponseObject = await _apiClient.AddObject(newJsonObject);
            Assert.NotNull(newResponseObject);

            // Deserialize the response content to get the new object's ID
            var newResponseContent = await newResponseObject.Content.ReadAsStringAsync();
            var newApiResponse = JsonConvert.DeserializeObject<APIResponse>(newResponseContent);

            var objectId = newApiResponse.id;

            var updateJson = new
            {
                name = "Sample Name EDITED AGAIN"
            };

            // Act: Update the object partially
            var updateResponseObject = await _apiClient.UpdateObject(objectId, updateJson);

            // No need to deserialize updateResponseObject since it's an HttpResponseMessage

            // Assert
            Assert.NotNull(updateResponseObject);
            Assert.Equal("Sample Name EDITED AGAIN", updateResponseObject.name);

        }


        [Theory]
        [InlineData("Sample Name", 2024, 2001.00, "Intel Core i10", "2 TB")]
        public async Task Test_DeleteObject(string name, int year, decimal price, string cpuModel, string hardDiskSize)
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

            var response = await _apiClient.AddObject(jsonData);

            // Deserialize the response content to get the new object's ID
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(responseContent);
            var objectId = apiResponse.id;

            var deleteResponseObject = await _apiClient.DeleteObject(objectId);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"Object with id = {objectId} has been deleted.", deleteResponseObject.message);

        }

        }
    }
