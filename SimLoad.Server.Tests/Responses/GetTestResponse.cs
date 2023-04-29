using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.Server.Tests.Responses;

/// <summary>
///     This is a load test tailored for a specific load generator
/// </summary>
public class GetTestResponse : ILoadTest
{
    public DateTime StartTime { get; init; }
    public TimeSpan Duration { get; init; }
    public Dictionary<Guid, Scenario> Scenarios { get; init; }
    public Dictionary<Guid, int> ScenarioWeights { get; init; }
    public VirtualUserGraph VirtualUserGraph { get; init; }

    public int CalculateTargetUserCount()
    {
        return 0;
    }
}