using SimLoad.Server.Data.Entities.Test;

namespace SimLoad.Server.Results.Utils;

public static class TestExtensions
{

    public static bool IsComplete(this Test test)
    {
        var endTime = test.StartTime.Add(test.Duration);
        return DateTime.UtcNow > endTime;
    }
    
}