using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Scenarios.Requests;
using SimLoad.Server.Scenarios.Services;
using SimLoad.Server.Scenarios.Submissions;

namespace SimLoad.Server.Scenarios;

[ApiController]
[Route("/api")]
[Authorize(Policy = Policies.User)]
public class ScenariosController : ControllerBase
{
    private readonly Lazy<ICreateScenarioService> _createScenarioService;
    private readonly Lazy<IDeleteScenarioService> _deleteScenarioService;
    private readonly Lazy<IGetScenarioService> _getScenarioService;
    private readonly Lazy<IGetScenariosService> _getScenariosService;
    private readonly Lazy<IUpdateScenarioService> _updateScenarioService;

    public ScenariosController(Lazy<ICreateScenarioService> createScenarioService,
        Lazy<IGetScenariosService> getScenariosService, Lazy<IGetScenarioService> getScenarioService,
        Lazy<IUpdateScenarioService> updateScenarioService, Lazy<IDeleteScenarioService> deleteScenarioService)
    {
        _createScenarioService = createScenarioService;
        _getScenariosService = getScenariosService;
        _getScenarioService = getScenarioService;
        _updateScenarioService = updateScenarioService;
        _deleteScenarioService = deleteScenarioService;
    }

    [HttpPost]
    [Route("projects/{projectId:guid}/scenarios")]
    public async Task<IActionResult> CreateScenario([FromRoute] Guid projectId,
        [FromBody] CreateScenarioSubmission submission)
    {
        var createScenarioRequest = new CreateScenarioRequest(submission, projectId);
        return await _createScenarioService.Value.CreateScenario(createScenarioRequest);
    }

    [HttpGet]
    [Route("projects/{projectId:guid}/scenarios")]
    public async Task<IActionResult> GetScenarios([FromRoute] Guid projectId)
    {
        return await _getScenariosService.Value.GetScenarios(new GetScenariosRequest
        {
            ProjectId = projectId
        });
    }

    [HttpGet]
    [Route("projects/{projectId:guid}/scenarios/{scenarioId}")]
    public async Task<IActionResult> GetScenario([FromRoute] Guid projectId, [FromRoute] Guid scenarioId)
    {
        return await _getScenarioService.Value.GetScenario(new GetScenarioRequest
        {
            ProjectId = projectId,
            ScenarioId = scenarioId
        });
    }

    [HttpPut]
    [Route("projects/{projectId:guid}/scenarios/{scenarioId}")]
    public async Task<IActionResult> UpdateScenario([FromRoute] Guid projectId, [FromRoute] Guid scenarioId,
        [FromBody] UpdateScenarioSubmission submission)
    {
        return await _updateScenarioService.Value.UpdateScenario(new UpdateScenarioRequest(submission, projectId,
            scenarioId));
    }

    [HttpDelete]
    [Route("projects/{projectId:guid}/scenarios/{scenarioId}")]
    public async Task<IActionResult> DeleteScenario([FromRoute] Guid projectId, [FromRoute] Guid scenarioId)
    {
        return await _deleteScenarioService.Value.DeleteScenario(new DeleteScenarioRequest
        {
            ProjectId = projectId,
            ScenarioId = scenarioId
        });
    }
}