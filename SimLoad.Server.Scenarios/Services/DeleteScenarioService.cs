using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Scenario;
using SimLoad.Server.Scenarios.Requests;

namespace SimLoad.Server.Scenarios.Services;

public interface IDeleteScenarioService
{
    Task<IActionResult> DeleteScenario(DeleteScenarioRequest request);
}

public class DeleteScenarioService : IDeleteScenarioService
{
    private readonly IPermissionEvaluator<Scenario, ScenarioMember, ScenarioPermissions>
        _permissionEvaluator;

    private readonly IMongoCollection<SimLoad.Common.Models.Scenario.Scenario> _scenarioCollection;

    private readonly ScenarioDbContext _scenarioDbContext;

    public DeleteScenarioService(
        IPermissionEvaluator<Scenario, ScenarioMember, ScenarioPermissions> permissionEvaluator,
        ScenarioDbContext scenarioDbContext,
        IMongoCollection<SimLoad.Common.Models.Scenario.Scenario> scenarioCollection)
    {
        _permissionEvaluator = permissionEvaluator;
        _scenarioDbContext = scenarioDbContext;
        _scenarioCollection = scenarioCollection;
    }

    public async Task<IActionResult> DeleteScenario(DeleteScenarioRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ScenarioId, p => p.ScenarioDelete, async (_, _) =>
        {
            await _scenarioCollection.DeleteManyAsync(s =>
                s.ProjectId == request.ProjectId && s.ScenarioId == request.ScenarioId);

            var scenarioToDelete = await _scenarioDbContext.Scenarios.SingleAsync(s => s.Id == request.ScenarioId);
            _scenarioDbContext.Scenarios.Remove(scenarioToDelete);

            await _scenarioDbContext.SaveChangesAsync();

            return new OkResult();
        });


        await _scenarioCollection.DeleteManyAsync(s =>
            s.ProjectId == request.ProjectId && s.ScenarioId == request.ScenarioId);

        return new OkResult();
    }
}