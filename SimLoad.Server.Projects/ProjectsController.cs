using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Projects.Requests;
using SimLoad.Server.Projects.Services;
using SimLoad.Server.Projects.Submissions;

namespace SimLoad.Server.Projects;

[Route("/api/organisations/{organisationId:guid}/projects")]
[ApiController]
[Authorize(Policy = Policies.User)]
public class ProjectsController : ControllerBase
{
    private readonly Lazy<ICreateProjectService> _createProjectService;
    private readonly Lazy<IGetProjectByIdService> _getProjectByIdService;
    private readonly Lazy<IGetUserProjectsService> _getUserProjectsService;

    public ProjectsController(
        Lazy<IGetUserProjectsService> getUserProjectsService,
        Lazy<ICreateProjectService> createProjectService,
        Lazy<IGetProjectByIdService> getProjectByIdService)
    {
        _getUserProjectsService = getUserProjectsService;
        _createProjectService = createProjectService;
        _getProjectByIdService = getProjectByIdService;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetProjects([FromRoute] Guid organisationId)
    {
        return await _getUserProjectsService.Value.GetUserProjects(organisationId);
    }

    [HttpGet]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> GetProject([FromRoute] Guid projectId)
    {
        return await _getProjectByIdService.Value.GetProject(projectId);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectSubmission submission,
        [FromRoute] Guid organisationId)
    {
        var request = new CreateProjectRequest(submission, organisationId);
        return await _createProjectService.Value.CreateProject(request);
    }
}