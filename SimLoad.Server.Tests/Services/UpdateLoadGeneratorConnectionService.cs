using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.LoadGenerator;
using SimLoad.Server.Tests.Requests;

namespace SimLoad.Server.Tests.Services;

public interface IUpdateLoadGeneratorConnectionService
{
    Task UpdateLoadGeneratorConnection(UpdateLoadGeneratorConnectionRequest request);
}

public class UpdateLoadGeneratorConnectionService : IUpdateLoadGeneratorConnectionService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    public UpdateLoadGeneratorConnectionService(LoadGeneratorDbContext loadGeneratorDbContext)
    {
        _loadGeneratorDbContext = loadGeneratorDbContext;
    }

    public async Task UpdateLoadGeneratorConnection(UpdateLoadGeneratorConnectionRequest request)
    {
        var loadGenerator =
            await _loadGeneratorDbContext.LoadGeneratorConnections.SingleOrDefaultAsync(lg =>
                lg.Id == request.LoadGeneratorId);

        if (loadGenerator is null)
        {
            var newLoadGeneratorConnection = new LoadGeneratorConnection
            {
                Id = request.LoadGeneratorId,
                OrganisationId = request.OrganisationId,
                Available = request.Available,
                IpAddress = request.IpAddress,
                LastUpdated = DateTime.UtcNow
            };
            _loadGeneratorDbContext.LoadGeneratorConnections.Add(newLoadGeneratorConnection);
        }

        if (loadGenerator is not null)
        {
            loadGenerator.Available = request.Available;
            loadGenerator.IpAddress = request.IpAddress;
            loadGenerator.LastUpdated = DateTime.UtcNow;
        }

        await _loadGeneratorDbContext.SaveChangesAsync();
    }
}