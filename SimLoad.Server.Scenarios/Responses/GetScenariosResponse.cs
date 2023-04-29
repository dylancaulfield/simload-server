namespace SimLoad.Server.Scenarios.Responses;

public class GetScenariosResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime LastUpdated { get; set; }
}