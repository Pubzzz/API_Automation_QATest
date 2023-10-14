namespace API_Automation_QATest
{ 
    public class ApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DataModel Data { get; set; }
}

public class DataModel
{
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string CPUModel { get; set; }
    public string HardDiskSize { get; set; }
}
}