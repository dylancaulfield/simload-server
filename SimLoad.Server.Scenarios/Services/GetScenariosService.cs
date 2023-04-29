using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Scenarios.Requests;
using SimLoad.Server.Scenarios.Responses;

namespace SimLoad.Server.Scenarios.Services;

public interface IGetScenariosService
{
    Task<IActionResult> GetScenarios(GetScenariosRequest request);
}

public class GetScenariosService : IGetScenariosService
{
    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly ScenarioDbContext _scenarioDbContext;

    public GetScenariosService(IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator,
        ScenarioDbContext scenarioDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _scenarioDbContext = scenarioDbContext;
    }

    public async Task<IActionResult> GetScenarios(GetScenariosRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ProjectId, p => true, async (project1, member) =>
        {
            var scenarios = await _scenarioDbContext.Scenarios
                .Where(s => s.ProjectId == request.ProjectId)
                .Select(s => new GetScenariosResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    LastUpdated = s.LastUpdated
                })
                .ToListAsync();

            return new OkObjectResult(scenarios);
        });
    }
}