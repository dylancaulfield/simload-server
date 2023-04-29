using System.Net.Http.Headers;
using Autofac;
using SimLoad.VirtualUser;
using SimLoad.VirtualUser.Contexts;

namespace SimLoad.LoadGenerator.Scenarios;

/// <summary>
///     Runs a scenario
/// </summary>
public interface IScenarioRunner
{
    /// <summary>
    ///     Creates a child scope, runs a scenario provided by the scenario provider,
    ///     disposes of the scope when complete and calls the onComplete callback
    /// </summary>
    /// <param name="onComplete"></param>
    void RunScenario(Action onComplete);
}

/// <summary>
///     Runs a scenario
/// </summary>
public class ScenarioRunner : IScenarioRunner
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IScenarioProvider _scenarioProvider;

    public ScenarioRunner(ILifetimeScope lifetimeScope, IScenarioProvider scenarioProvider)
    {
        _lifetimeScope = lifetimeScope;
        _scenarioProvider = scenarioProvider;
    }

    /// <summary>
    ///     Creates a child scope, runs a scenario provided by the scenario provider,
    ///     disposes of the scope when complete and calls the onComplete callback
    /// </summary>
    /// <param name="onScenarioComplete">Callback which is called when the scenario ends</param>
    public async void RunScenario(Action onScenarioComplete)
    {
        await using (var virtualUserScope = _lifetimeScope.BeginLifetimeScope(builder =>
                     {
                         builder.RegisterInstance(_scenarioProvider.GetNextScenario());
                         builder.Register(c =>
                         {
                             var factory = c.Resolve<IHttpClientFactory>();
                             var client = factory.CreateClient("vuClient");
                             client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                             {
                                 NoCache = true
                             };
                             return client;
                         }).InstancePerLifetimeScope();
                         builder.RegisterInstance(new VirtualUserId(Guid.NewGuid()));
                     }))
        {
            var virtualUser = virtualUserScope.Resolve<IVirtualUser>();
            await virtualUser.RunScenario();
        }

        onScenarioComplete();
    }
}