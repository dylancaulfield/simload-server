using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;
using SimLoad.Server.Common;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Tests.Requests;
using SimLoad.Server.Tests.Responses;

namespace SimLoad.Server.Tests.Services;

public interface IGetTestService
{
    Task<IActionResult> GetTest(GetTestRequest request);
}

public class GetTestService : IGetTestService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;
    private readonly IRequestContext _requestContext;
    private readonly IMongoCollection<Scenario> _scenariosCollections;
    private readonly TestDbContext _testDbContext;

    public GetTestService(
        IRequestContext requestContext,
        IMongoCollection<Scenario> scenariosCollections,
        LoadGeneratorDbContext loadGeneratorDbContext,
        TestDbContext testDbContext
    )
    {
        _requestContext = requestContext;
        _scenariosCollections = scenariosCollections;
        _loadGeneratorDbContext = loadGeneratorDbContext;
        _testDbContext = testDbContext;
    }

    public async Task<IActionResult> GetTest(GetTestRequest request)
    {
        var credential = await _loadGeneratorDbContext.LoadGeneratorCredentials
            .SingleOrDefaultAsync(
                lgc => lgc.Id == _requestContext.LoadGeneratorCredentialId);
        if (credential is null) return new UnauthorizedResult();

        var test = await _testDbContext.Tests
            .Include(t => t.Project.Organisation)
            .Include(t => t.ScenarioWeights)
            .Include(t => t.VirtualUserGraphs)
            .SingleOrDefaultAsync(t => t.Id == request.TestId);
        if (test is null) return new NotFoundResult();
        if (test.Project.Organisation.Id != credential.OrganisationId) return new UnauthorizedResult();

        var scenariosIds = test.ScenarioWeights.Select(sw => sw.ScenarioId).ToList();
        var scenarios = await _scenariosCollections
            .Find(s => scenariosIds.Contains(s.ScenarioId))
            .ToListAsync();

        var virtualUserGraph = new VirtualUserGraph
        {
            Points = test.VirtualUserGraphs
                .Where(vug => vug.LoadGeneratorId == request.LoadGeneratorId)
                .OrderBy(vug => vug.X)
                .Select(vug => new Point
                {
                    X = vug.X,
                    Y = vug.Y
                })
                .ToList()
        };

        var response = new GetTestResponse
        {
            StartTime = test.StartTime,
            Duration = test.Duration,
            ScenarioWeights = test.ScenarioWeights.ToDictionary(sw => sw.ScenarioId, sw => sw.Weight),
            Scenarios = scenarios.ToDictionary(s => s.ScenarioId, s => s),
            VirtualUserGraph = virtualUserGraph
        };

        return new OkObjectResult(response);
    }
}