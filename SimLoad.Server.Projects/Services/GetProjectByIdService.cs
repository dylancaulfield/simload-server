using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Projects.Responses;

namespace SimLoad.Server.Projects.Services;

public interface IGetProjectByIdService
{
    Task<IActionResult> GetProject(Guid projectId);
}

public class GetProjectByIdService : IGetProjectByIdService
{
    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly ProjectDbContext _projectDbContext;

    public GetProjectByIdService(IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator,
        ProjectDbContext projectDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _projectDbContext = projectDbContext;
    }

    public async Task<IActionResult> GetProject(Guid projectId)
    {
        return await _permissionEvaluator.Evaluate(projectId, p => true, async (_, _) =>
        {
            var project = await _projectDbContext.Projects
                .Include(p => p.Scenarios)
                .Include(p => p.ProjectMembers)
                .ThenInclude(pm => pm.OrganisationMember)
                .ThenInclude(om => om.User)
                .SingleAsync(p => p.Id == projectId);

            var projectResponse = new ProjectResponse
            {
                ProjectId = project.Id,
                Name = project.Name,
                Description = project.Description ?? string.Empty,
                Scenarios = project.Scenarios.Select(s => new ProjectResponseScenario
                {
                    ScenarioId = s.Id,
                    Name = s.Name
                }).ToList(),
                Members = project.ProjectMembers.Select(m => new ProjectResponseMember
                {
                    DisplayName = m.OrganisationMember.User.DisplayName
                }).ToList()
            };

            return new OkObjectResult(projectResponse);
        });
    }
}