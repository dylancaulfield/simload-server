using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Projects.Requests;

namespace SimLoad.Server.Projects.Services;

public interface ICreateProjectService
{
    Task<IActionResult> CreateProject(CreateProjectRequest request);
}

public class CreateProjectService : ICreateProjectService
{
    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    private readonly ProjectDbContext _projectDbContext;

    public CreateProjectService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        ProjectDbContext projectDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _projectDbContext = projectDbContext;
    }

    public async Task<IActionResult> CreateProject(CreateProjectRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.OrganisationCreateProject,
            async (organisation, organisationMember) =>
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisation.Id,
                    Name = request.Name,
                    Description = request.Description ?? string.Empty,
                    ProjectMembers = new List<ProjectMember>()
                };

                var projectMember = new ProjectMember
                {
                    Id = Guid.NewGuid(),
                    OrganisationMemberId = organisationMember.Id,
                    Project = project,
                    ProjectId = project.Id
                };

                var projectPermissions = new ProjectPermissions
                {
                    Id = Guid.NewGuid(),
                    ProjectMemberId = projectMember.Id,
                    ProjectDelete = true,
                    ProjectEdit = true,
                    ScenarioCreate = true,
                    ProjectAddMember = true,
                    ProjectChangePermission = true,
                    ProjectRemoveMember = true
                };

                projectMember.Permissions = projectPermissions;
                project.ProjectMembers = new List<ProjectMember> { projectMember };

                _projectDbContext.Add(project);

                await _projectDbContext.SaveChangesAsync();

                return new OkObjectResult(project.Id);
            });
    }
}