using SimLoad.Common.Models.Scenario;

namespace SimLoad.Server.Scenarios.Responses;

public class GetScenarioResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; init; }
    public Dictionary<Guid, Operation> Operations { get; set; }
    public DateTime LastUpdated { get; set; }
}