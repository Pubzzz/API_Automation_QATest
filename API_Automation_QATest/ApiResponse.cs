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

        /*the [JsonProperty] attributes in your class will tell the Newtonsoft.Json library to map the 
         * JSON properties "CPU model" and "Hard disk size" to the 
         * class properties CPUModel and HardDiskSize, respectively.
         * */
        [JsonProperty("CPU model")]
        public string CPUModel { get; set; }

        [JsonProperty("Hard disk size")]
        public string HardDiskSize { get; set; }
    }
}