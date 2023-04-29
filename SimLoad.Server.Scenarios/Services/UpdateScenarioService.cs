using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SimLoad.Server.Common;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Entities.Scenario;
using SimLoad.Server.Scenarios.Requests;
using Scenario = SimLoad.Common.Models.Scenario.Scenario;

namespace SimLoad.Server.Scenarios.Services;

public interface IUpdateScenarioService
{
    Task<IActionResult> UpdateScenario(UpdateScenarioRequest request);
}

public class UpdateScenarioService : IUpdateScenarioService
{
    private readonly IMongoCollection<Scenario> _scenarioCollection;

    private readonly IPermissionEvaluator<Data.Entities.Scenario.Scenario, ScenarioMember, ScenarioPermissions>
        _permissionEvaluator;

    public UpdateScenarioService(IMongoCollection<Scenario> scenarioCollection,
        IPermissionEvaluator<Data.Entities.Scenario.Scenario, ScenarioMember, ScenarioPermissions> permissionEvaluator)
    {
        _scenarioCollection = scenarioCollection;
        _permissionEvaluator = permissionEvaluator;
    }
    // TODO: update scenario name in both dbs

    public async Task<IActionResult> UpdateScenario(UpdateScenarioRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ScenarioId, p => p.ScenarioEdit, async (scenario, member) =>
        {
            var currentScenario = await _scenarioCollection
                .Find(s => s.ProjectId == request.ProjectId && s.ScenarioId == request.ScenarioId)
                .SingleOrDefaultAsync();
            if (currentScenario is null)
                return new NotFoundObjectResult("Scenario does not exist");

            // if (request.LastUpdated != currentScenario.LastUpdated)
            //     return new BadRequestObjectResult(new ErrorResponse
            //     {
            //         Message = "These updates are based on an out of data version of the scenario"
            //     });
            //
            var newScenario = new Scenario
            {
                Id = currentScenario.Id,
                ProjectId = currentScenario.ProjectId,
                ScenarioId = currentScenario.ScenarioId,
                Name = currentScenario.Name,
                //Name = request.Name,
                //Description = request.Description,
                Code = request.Code,
                Operations = currentScenario.Operations,
            };

            // Create
            foreach (var createdOperation in request.Operations.Created)
                newScenario.Operations.Add(createdOperation.Key.ToString(), createdOperation.Value);

            // Update
            foreach (var updatedOperation in request.Operations.Updated)
                if (currentScenario.Operations.TryGetValue(updatedOperation.Key.ToString(), out _))
                    newScenario.Operations[updatedOperation.Key.ToString()] = updatedOperation.Value;

            // Delete
            foreach (var deleteOperationId in request.Operations.Deleted)
                newScenario.Operations.Remove(deleteOperationId.ToString());

            await _scenarioCollection.FindOneAndReplaceAsync(
                s => s.ProjectId == request.ProjectId && s.ScenarioId == request.ScenarioId, newScenario);

            return new OkResult();
        });
    }
}