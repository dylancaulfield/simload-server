namespace SimLoad.Server.Scenarios.Requests;

public class GetScenarioRequest
{
    public Guid ProjectId { get; set; }
    public Guid ScenarioId { get; set; }
}