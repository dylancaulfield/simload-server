using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Tests.Responses;

namespace SimLoad.Server.Tests.Services;

public interface IGetTestOptionsService
{
    Task<IActionResult> GetTestOptions(Guid projectId);
}

public class GetTestOptionsService : IGetTestOptionsService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _organisationPermissionEvaluator;

    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _projectPermissionEvaluator;
    private readonly ScenarioDbContext _scenarioDbContext;

    public GetTestOptionsService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> organisationPermissionEvaluator,
        IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> projectPermissionEvaluator,
        LoadGeneratorDbContext loadGeneratorDbContext, ScenarioDbContext scenarioDbContext)
    {
        _organisationPermissionEvaluator = organisationPermissionEvaluator;
        _projectPermissionEvaluator = projectPermissionEvaluator;
        _loadGeneratorDbContext = loadGeneratorDbContext;
        _scenarioDbContext = scenarioDbContext;
    }

    public async Task<IActionResult> GetTestOptions(Guid projectId)
    {
        return await _projectPermissionEvaluator.Evaluate(projectId, p => true, async (project, member) =>
        {
            return await _organisationPermissionEvaluator.Evaluate(project.OrganisationId, p => p.TestRun,
                async (organisation, organisationMember) =>
                {
                    var loadGenerators = await _loadGeneratorDbContext.LoadGeneratorConnections
                        .Where(lgc => lgc.Organisation == organisation
                                      && lgc.LastUpdated.AddSeconds(45) >= DateTime.UtcNow)
                        .Select(lgc => new GetTestOptionsResponseLoadGenerator
                        {
                            Id = lgc.Id,
                            IpAddress = lgc.IpAddress,
                            Available = lgc.Available,
                            LastUpdated = lgc.LastUpdated
                        })
                        .ToListAsync();

                    var scenarios = await _scenarioDbContext.Scenarios
                        .Where(s => s.Project == project)
                        .Select(s => new GetTestOptionsResponseScenario
                        {
                            Id = s.Id,
                            Name = s.Name
                        })
                        .ToListAsync();

                    var response = new GetTestOptionsResponse
                    {
                        LoadGenerators = loadGenerators,
                        Scenarios = scenarios
                    };

                    return new OkObjectResult(response);
                });
        });
    }
}