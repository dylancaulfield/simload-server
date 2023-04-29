using System;
using System.Threading.Tasks;
using Xunit;

namespace SimLoad.LoadGenerator.Tests;

public class TestRunnerTests
{
    [Fact(Skip = "Used as proof of concept for multithreading")]
    public async Task AsyncVoidCallbackPatternWorksAsExpected()
    {
        var finished = false;

        Complete(() => { finished = true; });

        while (!finished) await Task.Delay(200);

        Assert.True(finished);
    }

    private static async void Complete(Action onCompleted)
    {
        await Task.Delay(1000);
        onCompleted();
    }
}