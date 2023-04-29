using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace SimLoad.Common.Models.Scenario;

/// <summary>
///     A single scenario within a test
/// </summary>
public class Scenario
{
    [JsonIgnore] public ObjectId Id { get; set; }

    public string Name { get; set; }
    public Guid ScenarioId { get; set; }
    public Guid ProjectId { get; set; }
    public string Code { get; init; }
    public Dictionary<string, Operation> Operations { get; set; }
}