using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.Common.Interfaces;

public interface ILoadTest
{
    public DateTime StartTime { get; }
    public TimeSpan Duration { get; }
    public Dictionary<Guid, Scenario> Scenarios { get; }
    public Dictionary<Guid, int> ScenarioWeights { get; }
    public VirtualUserGraph VirtualUserGraph { get; }
}