namespace SimLoad.Server.Tests.Responses;

public class GetTestOptionsResponse
{
    public List<GetTestOptionsResponseLoadGenerator> LoadGenerators { get; set; }
    public List<GetTestOptionsResponseScenario> Scenarios { get; set; }
}

public class GetTestOptionsResponseLoadGenerator
{
    public Guid Id { get; set; }
    public bool Available { get; set; }
    public string IpAddress { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class GetTestOptionsResponseScenario
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}