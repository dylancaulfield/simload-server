using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.LoadGenerator.Common;

namespace SimLoad.LoadGenerator.Lifecycle;

public interface IUpdateConnectionService
{
    Task UpdateConnection(bool available);
}

public class UpdateConnectionService : IUpdateConnectionService
{
    private readonly ApplicationServerOptions _applicationServerOptions;
    private readonly IClientInformationProvider _clientInformationProvider;
    private readonly HubConnection _hubConnection;
    private readonly ILogger<UpdateConnectionService> _logger;

    public UpdateConnectionService(
        IOptions<ApplicationServerOptions> applicationServerOptions,
        IClientInformationProvider clientInformationProvider,
        ILogger<UpdateConnectionService> logger,
        IHubConnectionFactory hubConnectionFactory
    )
    {
        _applicationServerOptions = applicationServerOptions.Value;
        _clientInformationProvider = clientInformationProvider;
        _logger = logger;
        _hubConnection = hubConnectionFactory.GetConnection();
    }

    public async Task UpdateConnection(bool available)
    {
        _logger.LogTrace("Updating connection as {available}", available ? "available" : "unavailable");

        await _hubConnection.InvokeAsync("UpdateConnection",
            _clientInformationProvider.GetClientInformation().LoadGeneratorId, available);
    }
}