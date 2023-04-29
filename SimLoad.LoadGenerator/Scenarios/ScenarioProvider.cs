using SimLoad.Common.Interfaces;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.LoadGenerator.Scenarios;

/// <summary>
///     Provides the next scenario to be run taking into account the scenario
///     weighting and randomly selecting scenarios in proportion
/// </summary>
public interface IScenarioProvider
{
    /// <summary>
    ///     Get the next scenario that should be run
    /// </summary>
    /// <returns></returns>
    Scenario GetNextScenario();
}

/// <summary>
///     Provides the next scenario to be run taking into account the scenario
///     weighting and maintaining a record of provided scenarios
/// </summary>
public class ScenarioProvider : IScenarioProvider
{
    /// <summary>
    ///     The LoadTest that is being run
    /// </summary>
    private readonly ILoadTest _loadTest;

    /// <summary>
    ///     Used to randomize scenario ids
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    ///     The total sum of the weighting of each scenario
    /// </summary>
    private readonly int _weightingSum;

    /// <summary>
    ///     The current index into the list of random scenarioIds
    /// </summary>
    private int _index;

    /// <summary>
    ///     List of random scenarioIds that is refreshed when the index reaches the weighting sum
    /// </summary>
    private List<Guid> _scenarioIds = new();

    /// <summary>
    ///     Initialise the random scenarioId list
    /// </summary>
    /// <param name="loadTest"></param>
    public ScenarioProvider(ILoadTest loadTest)
    {
        _loadTest = loadTest;

        foreach (var weighting in loadTest.ScenarioWeights)
        {
            for (var i = 0; i < weighting.Value; i++) _scenarioIds.Add(weighting.Key);

            _weightingSum += weighting.Value;
        }

        RandomizeList();
    }

    /// <summary>
    ///     Returns the next scenario corresponding with the id in the random list
    /// </summary>
    /// <returns></returns>
    public Scenario GetNextScenario()
    {
        var scenarioId = _scenarioIds[_index++];
        var nextScenario = _loadTest!.Scenarios[scenarioId];

        if (_index == _weightingSum)
        {
            RandomizeList();
            _index = 0;
        }

        return nextScenario;
    }

    /// <summary>
    ///     Randomizes the list of scenarioIds
    /// </summary>
    private void RandomizeList()
    {
        _scenarioIds = _scenarioIds.OrderBy(id => _random.Next()).ToList();
    }
}