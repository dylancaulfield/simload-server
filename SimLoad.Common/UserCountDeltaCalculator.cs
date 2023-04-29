using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;

namespace SimLoad.Common;

public interface IUserCountDeltaCalculator
{
    int CalculateDelta(DateTime now, int runningUsers);
}

public class UserCountDeltaCalculator : IUserCountDeltaCalculator
{
    private readonly ILoadTest _loadTest;

    public UserCountDeltaCalculator(ILoadTest loadTest)
    {
        _loadTest = loadTest;
    }

    public int CalculateDelta(DateTime now, int runningUsers)
    {
        var elapsedPercentage = (now - _loadTest.StartTime).TotalSeconds / _loadTest.Duration.TotalSeconds * 100;
        var firstPoint = _loadTest.VirtualUserGraph.Points.LastOrDefault(p => elapsedPercentage >= p.X);
        var secondPoint = _loadTest.VirtualUserGraph.Points.FirstOrDefault(p => elapsedPercentage <= p.X);
        if (firstPoint is null || secondPoint is null) return 0;

        var dividend = (double)(secondPoint.Y - firstPoint.Y);
        var divisor = (double)(secondPoint.X - firstPoint.X);

        double m = 0;
        if (dividend != 0 && divisor != 0) m = dividend / divisor;

        // Find midPoint to avoid 0
        var midPoint = new Point
        {
            X = (firstPoint.X + secondPoint.X) / 2,
            Y = (firstPoint.Y + secondPoint.Y) / 2
        };

        if (midPoint.X == 0 || midPoint.Y == 0) return 0;

        var c = midPoint.Y - m * midPoint.X;
        var y = m * elapsedPercentage + c;

        var delta = (int)Math.Ceiling(y - runningUsers);
        return delta > 0 ? delta : 0;
    }
}