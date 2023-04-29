namespace SimLoad.Server.Scenarios.Requests;

public class DeleteScenarioRequest
{
    public Guid ProjectId { get; set; }
    public Guid ScenarioId { get; set; }
}