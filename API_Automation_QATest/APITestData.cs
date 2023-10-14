//using AutoFixture;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace API_Automation_QATest.Tests
{
    public class APITestData : TheoryData<string, string, int, decimal, string, string, string>
    {
        public APITestData()
        {
            //Add("Sample Name", "Sample Name EDITED", 2023, 2024, 2000.00m, 2001.00m, "Intel Core i9", "Intel Core i10", "1 TB", "2 TB");
        }
    }
}
