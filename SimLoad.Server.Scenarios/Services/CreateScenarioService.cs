using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Data.Entities.Scenario;
using SimLoad.Server.Scenarios.Requests;
using Scenario = SimLoad.Common.Models.Scenario.Scenario;

namespace SimLoad.Server.Scenarios.Services;

public interface ICreateScenarioService
{
    Task<IActionResult> CreateScenario(CreateScenarioRequest request);
}

public class CreateScenarioService : ICreateScenarioService
{
    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly IMongoCollection<Scenario> _scenarioCollection;
    private readonly ScenarioDbContext _scenarioDbContext;

    public CreateScenarioService(IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator,
        ScenarioDbContext scenarioDbContext, IMongoCollection<Scenario> scenarioCollection)
    {
        _permissionEvaluator = permissionEvaluator;
        _scenarioDbContext = scenarioDbContext;
        _scenarioCollection = scenarioCollection;
    }

    public async Task<IActionResult> CreateScenario(CreateScenarioRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ProjectId, p => p.ScenarioCreate, async (_, pm) =>
        {
            var scenarioModel = new Scenario
            {
                Id = ObjectId.GenerateNewId(),
                Name = request.Name,
                ScenarioId = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                Code = request.Code,
                Operations = request.Operations
                    .ToDictionary(k => k.Key.ToString(), v => v.Value)
            };

            var scenarioMetadata = new Data.Entities.Scenario.Scenario
            {
                Id = scenarioModel.ScenarioId,
                ProjectId = request.ProjectId,
                Name = request.Name,
                Description = request.Description,
                ScenarioMembers = new List<ScenarioMember>(),
                LastUpdated = DateTime.UtcNow
            };

            var scenarioMember = new ScenarioMember
            {
                Id = Guid.NewGuid(),
                ProjectMemberId = pm.Id,
                Scenario = scenarioMetadata,
                ScenarioId = scenarioMetadata.Id
            };

            var scenarioPermission = new ScenarioPermissions
            {
                Id = Guid.NewGuid(),
                ScenarioMember = scenarioMember,
                ScenarioMemberId = scenarioMember.Id,
                ScenarioEdit = true,
                ScenarioDelete = true,
                ScenarioAddMember = true,
                ScenarioChangePermission = true,
                ScenarioRemoveMember = true
            };

            scenarioMember.Permissions = scenarioPermission;
            scenarioMetadata.ScenarioMembers.Add(scenarioMember);

            await _scenarioCollection.InsertOneAsync(scenarioModel);
            _scenarioDbContext.Add(scenarioMetadata);

            await _scenarioDbContext.SaveChangesAsync();

            return new OkObjectResult(scenarioModel.ScenarioId);
        });
    }
}