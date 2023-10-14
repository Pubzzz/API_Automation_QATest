namespace API_Automation_QATest
{ 
    public class ApiResponse
{
    public string id { get; set; }
    public string name { get; set; }
    public DataModel data { get; set; }
}

public class DataModel
{
    public int year { get; set; }
    public decimal price { get; set; }
    public string CPUModel { get; set; }
    public string HardDiskSize { get; set; }
}
}