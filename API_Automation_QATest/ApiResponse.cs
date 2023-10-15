using Newtonsoft.Json;

namespace API_Automation_QATest
{ 
    public class APIResponse
{
    public string id { get; set; }
    public string name { get; set; }
    public DataModel data { get; set; }

    public string message { get; set; }
}

    public class DataModel
    {
        public int year { get; set; }
        public decimal price { get; set; }

        [JsonProperty("CPUmodel")]
        public string CPUModel { get; set; }

        [JsonProperty("Harddisksize")]
        public string HardDiskSize { get; set; }
    }
}