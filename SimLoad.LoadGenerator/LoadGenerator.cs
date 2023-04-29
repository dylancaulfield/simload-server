using System.Threading.Channels;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.LoadGenerator.Common;
using SimLoad.LoadGenerator.Lifecycle;
using SimLoad.LoadGenerator.Results;
using SimLoad.LoadGenerator.Tests;

namespace SimLoad.LoadGenerator;

/// <summary>
///     Main service that handles listening for instructions and starting/stopping tests
/// </summary>
public class LoadGenerator : IHostedService
{
    private readonly ClientInformation _clientInformation;
    private readonly HubConnection _hubConnection;
    private readonly ILogger<LoadGenerator> _logger;
    private readonly ILifetimeScope _parentLifetimeScope;
    private readonly object _stateLock = new();
    private readonly ITestRetrievalService _testRetrievalService;
    private readonly IUpdateConnectionService _updateConnectionService;

    private ILifetimeScope? _childLifetimeScope;
    private CancellationTokenSource? _cts;
    private bool _isRunning;

    public LoadGenerator(
        ILogger<LoadGenerator> logger,
        IClientInformationProvider clientInformationProvider,
        ILifetimeScope lifetimeScope,
        ITestRetrievalService testRetrievalService,
        IUpdateConnectionService updateConnectionService,
        IHubConnectionFactory hubConnectionFactory)
    {
        _logger = logger;
        _clientInformation = clientInformationProvider.GetClientInformation();
        _parentLifetimeScope = lifetimeScope;
        _testRetrievalService = testRetrievalService;
        _updateConnectionService = updateConnectionService;
        _hubConnection = hubConnectionFactory.GetConnection();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogTrace("Starting LoadGenerator service");

        _hubConnection.On<Instruction>("ReceiveInstruction", async instruction =>
        {
            _logger.LogTrace("Instruction received");

            // Instruction is not for us
            if (instruction.LoadGeneratorId != _clientInformation.LoadGeneratorId) return;

            // Instruction is for us, and says to cancel
            if (instruction.Cancel)
            {
                _logger.LogTrace("Cancelling running test");

                lock (_stateLock)
                {
                    _cts?.Cancel();
                }

                return;
            }

            // Instruction is for us, check if we're already running a test using a lock
            lock (_stateLock)
            {
                if (_isRunning) return;
            }

            // Retrieve test
            var test = await _testRetrievalService.RetrieveTest(instruction);
            if (test is null) return;

            // Instruction is for us, we're not running a test, so begin the retrieved test
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _childLifetimeScope = _parentLifetimeScope.BeginLifetimeScope(builder =>
            {
                var channel = Channel.CreateUnbounded<SerializableResult>();
                builder.RegisterInstance(new ScriptCache()).As<IScriptCache>().SingleInstance();
                builder.RegisterInstance(channel.Writer).As<ChannelWriter<SerializableResult>>();
                builder.RegisterInstance(channel.Reader).As<ChannelReader<SerializableResult>>();
                builder.RegisterInstance(instruction).As<Instruction>();
                builder.RegisterInstance(test).As<ILoadTest>();
                builder.RegisterInstance(new CancellationTokenProvider(_cts)).As<ICancellationTokenProvider>();
            });

            var testRunner = _childLifetimeScope.Resolve<ITestRunner>();
            var resultSender = _childLifetimeScope.Resolve<IResultSender>();
            resultSender.StartSendingResults();

            _logger.LogTrace("Running new test");

            // Update connection as unavailable
            lock (_stateLock)
            {
                _isRunning = true;
            }

            await _updateConnectionService.UpdateConnection(false);

            testRunner.RunTest(() =>
            {
                _logger.LogTrace("Test completed, disposing of child lifetime scope");

                // Update connection as available
                _updateConnectionService.UpdateConnection(true);

                // Dispose of the child lifetime scope
                _childLifetimeScope?.Dispose();

                // Set the isRunning flag to false so we can start another test
                lock (_stateLock)
                {
                    _isRunning = false;
                }
            });
        });

        _logger.LogTrace("Starting SignalR connection");
        await _hubConnection.StartAsync(CancellationToken.None);
        _logger.LogTrace("SignalR connection started");

        // Run program loop until cancelled
        while (true)
        {
            lock (_stateLock)
            {
#pragma warning disable CS4014
                _updateConnectionService.UpdateConnection(!_isRunning);
#pragma warning restore CS4014
            }

            await Task.Delay(30000, CancellationToken.None);

            if (!cancellationToken.IsCancellationRequested) continue;
            lock (_stateLock)
            {
                if (!_isRunning) break;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lock (_stateLock)
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }
    }
}