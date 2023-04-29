using System;
using System.Collections.Generic;
using System.Linq;
using SimLoad.Common.Models.Scenario;
using SimLoad.LoadGenerator.Scenarios;
using Xunit;

namespace SimLoad.LoadGenerator.Tests;

public class ScenarioProviderTests
{
    [Fact]
    public void ScenarioProviderReturnsTheCorrectNumberOfEachScenario()
    {
        // Arrange
        var scenarioA = (new Scenario(), Guid.NewGuid());
        var scenarioB = (new Scenario(), Guid.NewGuid());
        var scenarioC = (new Scenario(), Guid.NewGuid());

        var loadTest = new LoadTest
        {
            Scenarios = new Dictionary<Guid, Scenario>
            {
                { scenarioA.Item2, scenarioA.Item1 },
                { scenarioB.Item2, scenarioB.Item1 },
                { scenarioC.Item2, scenarioC.Item1 }
            },
            ScenarioWeights = new Dictionary<Guid, int>
            {
                { scenarioA.Item2, 2 },
                { scenarioB.Item2, 4 },
                { scenarioC.Item2, 6 }
            }
        };
        var scenarioProvider = new ScenarioProvider(loadTest);

        // Act
        var scenarios = new List<Scenario>();

        for (var i = 0; i < 12; i++) scenarios.Add(scenarioProvider.GetNextScenario());

        var scenarioAsReturned = scenarios.Where(s => s == scenarioA.Item1);
        var scenarioBsReturned = scenarios.Where(s => s == scenarioB.Item1);
        var scenarioCsReturned = scenarios.Where(s => s == scenarioC.Item1);

        // Assert
        Assert.Equal(2, scenarioAsReturned.Count());
        Assert.Equal(4, scenarioBsReturned.Count());
        Assert.Equal(6, scenarioCsReturned.Count());
    }
}