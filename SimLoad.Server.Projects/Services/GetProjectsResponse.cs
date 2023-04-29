using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Projects.Responses;

namespace SimLoad.Server.Projects.Services;

public interface IGetUserProjectsService
{
    Task<IActionResult> GetUserProjects(Guid organisationId);
}

public class GetUserProjectsService : IGetUserProjectsService
{
    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    private readonly ProjectDbContext _projectDbContext;

    public GetUserProjectsService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        ProjectDbContext projectDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _projectDbContext = projectDbContext;
    }

    public async Task<IActionResult> GetUserProjects(Guid organisationId)
    {
        return await _permissionEvaluator.Evaluate(organisationId, p => true, async (org, m) =>
        {
            var projects = await _projectDbContext.Projects
                .Where(p => p.Organisation.Id == org.Id && p.ProjectMembers.Any(pm => pm.OrganisationMember.Id == m.Id))
                .Select(p => new GetProjectsResponse
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            return new OkObjectResult(projects);
        });
    }
}