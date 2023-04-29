using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;

namespace SimLoad.Server.Projects.Permissions;

public class ProjectPermissionEvaluator : IPermissionEvaluator<Project, ProjectMember, ProjectPermissions>
{
    private readonly ProjectDbContext _projectDbContext;
    private readonly Guid? _userId;

    public ProjectPermissionEvaluator(ProjectDbContext projectDbContext, IRequestContext requestContext)
    {
        _projectDbContext = projectDbContext;
        _userId = requestContext.UserId;
    }

    public async Task<IActionResult> Evaluate(Guid entityId, Func<ProjectPermissions, bool> hasPermission,
        Func<Project, ProjectMember, Task<IActionResult>> onPermissionAllowed)
    {
        var project = await _projectDbContext.Projects.FindAsync(entityId);
        if (project is null) return new NotFoundResult();

        var projectPermissions = await _projectDbContext.ProjectPermissions
            .Include(pp => pp.ProjectMember)
            .SingleOrDefaultAsync(pm => pm.ProjectMember.OrganisationMember.UserId == _userId
                                        && pm.ProjectMember.ProjectId == entityId);
        if (projectPermissions is null || !hasPermission(projectPermissions)) return new ForbidResult();

        return await onPermissionAllowed(project, projectPermissions.ProjectMember);
    }
}