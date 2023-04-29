using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;
using SimLoad.Server.Data.Entities.Test;

namespace SimLoad.Server.Results.Services;

public interface ITargetUserCountCalculator
{
    int CalculateTargetUserCount(Test test, DateTime? time = null);
}

public class TargetUserCountCalculator : ITargetUserCountCalculator
{
    
    public int CalculateTargetUserCount(Test test, DateTime? time = null)
    {

        var points = test.VirtualUserGraphs.Select(g => new Point(g.X, g.Y)).ToList();
        var xValues = points.Select(p => p.X).Distinct();
        points = xValues
            .Select(x =>
                new Point(x,
                    points
                        .Where(p => p.X == x)
                        .Sum(p => p.Y)))
            .ToList();

        var loadTest = new LoadTest(points, test);
        var calculator = new UserCountDeltaCalculator(loadTest);
        return calculator.CalculateDelta(time ?? DateTime.UtcNow, 0);

    }
    
    private class LoadTest : ILoadTest
    {

        public LoadTest(List<Point> points, Test test)
        {
            Duration = test.Duration;
            StartTime = test.StartTime;
            VirtualUserGraph = new VirtualUserGraph
            {
                Points = points
            };
        }
    
        public DateTime StartTime { get; }
        public TimeSpan Duration { get; }
        public Dictionary<Guid, Scenario> Scenarios { get; }
        public Dictionary<Guid, int> ScenarioWeights { get; }
        public VirtualUserGraph VirtualUserGraph { get; }
    }
}
