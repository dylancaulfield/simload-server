using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SimLoad.Server.Tests.Requests;
using SimLoad.Server.Tests.Services;

namespace SimLoad.Server.Tests.Hubs;

public class TestsHub : Hub
{
    private readonly IUpdateLoadGeneratorConnectionService _updateLoadGeneratorConnectionService;
    private readonly ILogger<TestsHub> _logger;

    public TestsHub(IUpdateLoadGeneratorConnectionService updateLoadGeneratorConnectionService, ILogger<TestsHub> logger)
    {
        _updateLoadGeneratorConnectionService = updateLoadGeneratorConnectionService;
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var organisationId = Context.User?.Claims.First(c => c.Type == "organisationId").Value;
        if (organisationId is null)
        {
            _logger.LogWarning("No organisationId claim found in TestsHub connection, aborting connection");
            Context.Abort();
        }

        Context.Items.Add("organisationId", organisationId);
        Context.Items.Add("ipAddress", Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? "");

        Groups.AddToGroupAsync(Context.ConnectionId, organisationId!);
        return Task.CompletedTask;
    }

    public async Task UpdateConnection(Guid loadGeneratorId, bool available)
    {
        var organisationId = Context.Items["organisationId"]!.ToString()!;
        var ipAddress = Context.Items["ipAddress"]!.ToString()!;

        var request = new UpdateLoadGeneratorConnectionRequest
        {
            OrganisationId = Guid.Parse(organisationId),
            LoadGeneratorId = loadGeneratorId,
            Available = available,
            IpAddress = ipAddress
        };

        await _updateLoadGeneratorConnectionService.UpdateLoadGeneratorConnection(request);
    }
}