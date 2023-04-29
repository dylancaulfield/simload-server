using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Options;

namespace SimLoad.LoadGenerator.Lifecycle;

public interface IHubConnectionFactory
{
    HubConnection GetConnection();
}

public class HubConnectionFactory : IHubConnectionFactory, IRetryPolicy, IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ApplicationServerOptions _applicationServerOptions;

    public HubConnectionFactory(
        ICredentialsProvider credentialsProvider,
        IOptions<ApplicationServerOptions> applicationServerOptions,
        ILogger<HubConnection> logger
    )
    {
        _applicationServerOptions = applicationServerOptions.Value;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{applicationServerOptions.Value.BaseUrl}{TestsHubEndpoint.Endpoint}",
                options =>
                {
                    options.AccessTokenProvider = () => credentialsProvider.GetBearerToken()!;
                    options.SkipNegotiation = true;
                    options.Transports = HttpTransportType.WebSockets;
                })
            .WithAutomaticReconnect(this)
            .Build();

        _hubConnection.Reconnecting += error =>
        {
            logger.LogWarning("Reconnecting to SignalR hub");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectId =>
        {
            logger.LogInformation("Reconnected to SignalR hub");
            return Task.CompletedTask;
        };

    }
    
    public HubConnection GetConnection()
    {
        return _hubConnection;
    }
    
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(_applicationServerOptions.RetryDelay);
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }

}