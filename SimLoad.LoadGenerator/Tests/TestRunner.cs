using Microsoft.Extensions.Logging;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.LoadGenerator.Scenarios;

namespace SimLoad.LoadGenerator.Tests;

/// <summary>
///     Manages the state of the test run, number of virtual users, if time is up
/// </summary>
public interface ITestRunner
{
    void RunTest(Action onComplete);
}

/// <summary>
///     Manages the state of the test run, number of virtual users, if time is up
/// </summary>
public class TestRunner : ITestRunner
{
    /// <summary>
    ///     Token used to cancel the test
    /// </summary>
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    ///     Holds the load test being run
    /// </summary>
    private readonly ILoadTest _loadTest;

    private readonly ILogger<TestRunner> _logger;

    /// <summary>
    ///     Use to lock access to the running users counter between threads
    /// </summary>
    private readonly object _runningUsersCountLock = new();

    /// <summary>
    ///     Runs a scenario and calls back when complete
    /// </summary>
    private readonly IScenarioRunner _scenarioRunner;

    /// <summary>
    ///     Calculates the number of users that need to be started
    /// </summary>
    private readonly IUserCountDeltaCalculator _userCountDeltaCalculator;

    /// <summary>
    ///     Holds the number of actively running users/scenarios
    /// </summary>
    private int _runningUsers;

    public TestRunner(
        ILogger<TestRunner> logger,
        IScenarioRunner scenarioRunner,
        ILoadTest loadTest,
        ICancellationTokenProvider cancellationTokenProvider,
        IUserCountDeltaCalculator userCountDeltaCalculator
    )
    {
        _logger = logger;
        _scenarioRunner = scenarioRunner;
        _loadTest = loadTest;
        _cancellationToken = cancellationTokenProvider.CancellationToken!.Value;
        _userCountDeltaCalculator = userCountDeltaCalculator;
    }

    public async void RunTest(Action onComplete)
    {
        // Load test loop
        while (true)
        {
            var delta = 0;

            // Wait for 1 second
            await Task.Delay(1000, CancellationToken.None);

            // Start running scenarios using the difference between the currently running users
            // and the number of users supposed to be currently running
            lock (_runningUsersCountLock)
            {
                delta = _userCountDeltaCalculator.CalculateDelta(DateTime.UtcNow, _runningUsers);
            }

            StartScenarios(delta);

            // Check if time is up or cancellation has been requested
            if (!ShouldTestEnd()) continue;

            await WaitForRunningScenariosToEnd();
            break;
        }

        onComplete();
    }

    /// <summary>
    ///     Determine if test duration has elapsed or if cancellation has been requested
    /// </summary>
    /// <returns></returns>
    private bool ShouldTestEnd()
    {
        return _cancellationToken.IsCancellationRequested
               || _loadTest.StartTime + _loadTest.Duration < DateTime.UtcNow;
    }

    // TODO: Remove this and cut off exactly at end time
    /// <summary>
    ///     Return when all scenarios have completed
    /// </summary>
    /// <returns></returns>
    private async Task WaitForRunningScenariosToEnd()
    {
        _logger.LogTrace("Waiting for running scenarios to end");

        while (true)
        {
            lock (_runningUsersCountLock)
            {
                if (_runningUsers == 0) break;
            }

            await Task.Delay(1000);
        }
    }

    /// <summary>
    ///     Begin a new scenario
    /// </summary>
    private void StartScenarios(int numScenarios)
    {
        if (numScenarios <= 0) return;

        _logger.LogTrace("Starting {numScenarios} scenarios", numScenarios);

        lock (_runningUsersCountLock)
        {
            _runningUsers += numScenarios;
        }

        for (var i = 0; i < numScenarios; i++)
            _scenarioRunner.RunScenario(() =>
            {
                lock (_runningUsersCountLock)
                {
                    _runningUsers--;
                }
            });
    }
}