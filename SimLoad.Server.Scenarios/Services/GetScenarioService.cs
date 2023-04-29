using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Scenario;
using SimLoad.Server.Scenarios.Requests;
using SimLoad.Server.Scenarios.Responses;

namespace SimLoad.Server.Scenarios.Services;

public interface IGetScenarioService
{
    Task<IActionResult> GetScenario(GetScenarioRequest request);
}

public class GetScenarioService : IGetScenarioService
{
    private readonly IPermissionEvaluator<Scenario, ScenarioMember, ScenarioPermissions>
        _permissionEvaluator;

    private readonly IMongoCollection<SimLoad.Common.Models.Scenario.Scenario> _scenarioCollection;

    private readonly ScenarioDbContext _scenarioDbContext;

    public GetScenarioService(
        IPermissionEvaluator<Scenario, ScenarioMember, ScenarioPermissions> permissionEvaluator,
        ScenarioDbContext scenarioDbContext,
        IMongoCollection<SimLoad.Common.Models.Scenario.Scenario> scenarioCollection)
    {
        _permissionEvaluator = permissionEvaluator;
        _scenarioDbContext = scenarioDbContext;
        _scenarioCollection = scenarioCollection;
    }

    public async Task<IActionResult> GetScenario(GetScenarioRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ScenarioId, p => p.ScenarioEdit, async (_, member) =>
        {
            var scenario = await _scenarioCollection
                .Find(s => s.ProjectId == request.ProjectId && s.ScenarioId == request.ScenarioId)
                .SingleOrDefaultAsync();

            var scenarioMetadata = await _scenarioDbContext.Scenarios
                .SingleAsync(s => s.Id == request.ScenarioId);


            var response = new GetScenarioResponse
            {
                Name = scenarioMetadata.Name,
                Description = scenarioMetadata.Description ?? string.Empty,
                Code = scenario.Code,
                Operations = scenario.Operations
                    .ToDictionary(k => Guid.Parse(k.Key), v => v.Value),
                LastUpdated = scenarioMetadata.LastUpdated
            };

            return new OkObjectResult(response);
        });
    }
}