using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.LoadGenerator.Tests;

public class LoadTest : ILoadTest
{
    public DateTime StartTime { get; init; }
    public TimeSpan Duration { get; init; }
    public Dictionary<Guid, Scenario> Scenarios { get; init; }
    public Dictionary<Guid, int> ScenarioWeights { get; init; }
    public VirtualUserGraph VirtualUserGraph { get; init; }
}