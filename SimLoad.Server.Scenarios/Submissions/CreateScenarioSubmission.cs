using System.ComponentModel.DataAnnotations;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.Server.Scenarios.Submissions;

public class CreateScenarioSubmission
{
    [MinLength(1)] public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; init; }
    public Dictionary<Guid, Operation> Operations { get; set; }
}