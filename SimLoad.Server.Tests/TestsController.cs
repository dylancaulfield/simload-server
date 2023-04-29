using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Tests.Requests;
using SimLoad.Server.Tests.Services;
using SimLoad.Server.Tests.Submissions;

namespace SimLoad.Server.Tests;

[Route("/api/organisations/{organisationId:guid}/projects/{projectId:guid}/tests")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly Lazy<IGetTestOptionsService> _getTestOptionsService;
    private readonly Lazy<IGetTestService> _getTestService;
    private readonly Lazy<IGetTestsService> _getTestsService;
    private readonly Lazy<IStartTestService> _startTestService;
    private readonly Lazy<IGetTestMetaDataService> _getTestMetaDataService;

    public TestsController(Lazy<IGetTestOptionsService> getTestOptionsService, Lazy<IGetTestService> getTestService,
        Lazy<IGetTestsService> getTestsService, Lazy<IStartTestService> startTestService,
        Lazy<IGetTestMetaDataService> getTestMetaDataService)
    {
        _getTestOptionsService = getTestOptionsService;
        _getTestService = getTestService;
        _getTestsService = getTestsService;
        _startTestService = startTestService;
        _getTestMetaDataService = getTestMetaDataService;
    }

    [HttpGet]
    [Authorize(Policy = Policies.User)]
    [Route("")]
    public async Task<IActionResult> GetTests([FromRoute] Guid projectId)
    {
        return await _getTestsService.Value.GetTests(projectId);
    }

    [HttpGet]
    [Authorize(Policy = Policies.User)]
    [Route("{testId:guid}")]
    public async Task<IActionResult> GetTestMetaData([FromRoute] Guid projectId, [FromRoute] Guid testId)
    {
        var reqeust = new GetTestMetadataRequest
        {
            ProjectId = projectId,
            TestId = testId
        };
        return await _getTestMetaDataService.Value.GetTestMetaData(reqeust);
    }

    [HttpGet]
    [Authorize(Policy = Policies.User)]
    [Route("options")]
    public async Task<IActionResult> GetTestOptions([FromRoute] Guid projectId)
    {
        return await _getTestOptionsService.Value.GetTestOptions(projectId);
    }

    [HttpPost]
    [Authorize(Policy = Policies.User)]
    [Route("")]
    public async Task<IActionResult> StartTest([FromRoute] Guid organisationId, [FromRoute] Guid projectId,
        [FromBody] StartTestSubmission submission)
    {
        var request = new StartTestRequest(submission, organisationId, projectId);
        return await _startTestService.Value.CreateTest(request);
    }

    [HttpDelete]
    [Authorize(Policy = Policies.User)]
    [Route("{testId:guid}")]
    public async Task<IActionResult> StopTest([FromRoute] Guid organisationId, [FromRoute] Guid projectId,
        [FromRoute] Guid testId)
    {
        return new OkResult();
    }

    [HttpGet]
    [Authorize(Policy = Policies.LoadGenerator)]
    [Route("/api/tests/{testId:guid}/load-generators/{loadGeneratorId:guid}")]
    public async Task<IActionResult> GetTest([FromRoute] Guid testId,
        [FromRoute] Guid loadGeneratorId)
    {
        var request = new GetTestRequest
        {
            TestId = testId,
            LoadGeneratorId = loadGeneratorId
        };
        return await _getTestService.Value.GetTest(request);
    }
}