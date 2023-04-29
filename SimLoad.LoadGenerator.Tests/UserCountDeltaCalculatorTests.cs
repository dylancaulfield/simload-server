using System;
using System.Collections.Generic;
using SimLoad.Common;
using SimLoad.Common.Models;
using Xunit;

namespace SimLoad.LoadGenerator.Tests;

public class UserCountDeltaCalculatorTests
{
    private readonly UserCountDeltaCalculator _userCountDeltaCalculator;

    public UserCountDeltaCalculatorTests()
    {
        var loadTest = new LoadTest
        {
            StartTime = new DateTime(),
            Duration = TimeSpan.FromMinutes(1),
            VirtualUserGraph = new VirtualUserGraph
            {
                Points = new List<Point>
                {
                    new() { X = 0, Y = 0 },
                    new() { X = 25, Y = 50 },
                    new() { X = 50, Y = 100 },
                    new() { X = 75, Y = 50 },
                    new() { X = 100, Y = 0 }
                }
            }
        };
        _userCountDeltaCalculator = new UserCountDeltaCalculator(loadTest);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(7, 25)]
    [InlineData(60, 0)]
    public void UserCountDeltaCalculatorReturnsCorrectDeltasWithNoRunningUsers(int seconds, int expectedDelta)
    {
        var now = new DateTime().AddSeconds(seconds);
        var delta = _userCountDeltaCalculator.CalculateDelta(now, 0);

        Assert.Equal(expectedDelta, delta);
    }

    [Theory]
    [InlineData(0, 5, 0)]
    [InlineData(15, 30, 20)]
    [InlineData(30, 5, 95)]
    [InlineData(45, 55, 0)]
    public void UserCountDeltaCalculatorReturnsCorrectDeltas(int seconds, int runningUsers, int expectedDelta)
    {
        var now = new DateTime().AddSeconds(seconds);
        var delta = _userCountDeltaCalculator.CalculateDelta(now, runningUsers);

        Assert.Equal(expectedDelta, delta);
    }
}