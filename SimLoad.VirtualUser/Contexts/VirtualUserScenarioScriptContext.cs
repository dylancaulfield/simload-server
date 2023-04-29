using Microsoft.Extensions.Logging;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models.Scenario;
using SimLoad.VirtualUser.Operations;

namespace SimLoad.VirtualUser.Contexts;

/// <summary>
///     Holds the context shared between Python and C#
/// </summary>
public class VirtualUserScenarioScriptContext : IScenarioScriptContext
{
    private readonly CancellationToken? _cancellationToken;
    private readonly IHttpRequestSender _httpRequestSender;
    private readonly ILogger<VirtualUserScenarioScriptContext> _logger;
    private readonly Scenario _scenario;

    public VirtualUserScenarioScriptContext(
        ILogger<VirtualUserScenarioScriptContext> logger,
        Scenario scenario,
        ICancellationTokenProvider cancellationTokenProvider,
        IHttpRequestSender httpRequestSender
    )
    {
        _logger = logger;
        _httpRequestSender = httpRequestSender;
        _scenario = scenario;
        _cancellationToken = cancellationTokenProvider.CancellationToken!.Value;
    }

    public async Task Delay(int milliseconds)
    {
        _cancellationToken?.ThrowIfCancellationRequested();
        await Task.Delay(milliseconds);
    }

    public async Task Http(string operationId)
    {
        _cancellationToken?.ThrowIfCancellationRequested();

        if (!_scenario.Operations.TryGetValue(operationId, out var operation))
        {
            _logger.LogWarning("Unable to find request with ID: {id}", operationId);
            return;
        }

        _logger.LogTrace("Executing request: {request}", operationId);
        await _httpRequestSender.SendRequest(operation!, Guid.Parse(operationId));
    }

    public void Log(string message)
    {
        _logger.LogInformation(message);
    }

    public void set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public string get(string key)
    {
        throw new NotImplementedException();
    }
}