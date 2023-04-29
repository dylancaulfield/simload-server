using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Results.Requests;
using SimLoad.Server.Results.Services;

namespace SimLoad.Server.Results;

[Route("/api/projects/{projectId:guid}/tests/{testId:guid}/results")]
[Authorize]
[ApiController]
public class ResultsController : ControllerBase
{
    private readonly Lazy<IResultBatchUploadService> _resultBatchUploadService;
    private readonly Lazy<ILiveResultsService> _liveResultService;
    private readonly Lazy<IFinalResultsService> _finalResultService;

    public ResultsController(Lazy<IResultBatchUploadService> resultBatchUploadService,
        Lazy<ILiveResultsService> liveResultService, Lazy<IFinalResultsService> finalResultService)
    {
        _resultBatchUploadService = resultBatchUploadService;
        _liveResultService = liveResultService;
        _finalResultService = finalResultService;
    }

    [HttpGet]
    [Route("live")]
    [Authorize(Policy = Policies.User)]
    public async Task<IActionResult> GetLiveResults([FromRoute] Guid projectId, [FromRoute] Guid testId,
        CancellationToken cancellationToken)
    {
        var request = new ResultsRequest
        {
            ProjectId = projectId,
            TestId = testId
        };
        return await _liveResultService.Value.GetLiveResults(request, cancellationToken);
    }

    [HttpGet]
    [Route("")]
    [Authorize(Policy = Policies.User)]
    public async Task<IActionResult> GetCompleteResult([FromRoute] Guid projectId, [FromRoute] Guid testId,
        CancellationToken cancellationToken)
    {
        var request = new ResultsRequest
        {
            ProjectId = projectId,
            TestId = testId
        };
        return await _finalResultService.Value.GetFinalResults(request);
    }

    [HttpPut]
    [Route("/api/tests/{testId:guid}/results")]
    [Authorize(Policy = Policies.LoadGenerator)]
    public async Task<IActionResult> UploadResultBatch([FromBody] ResultBatchRequest batchRequest)
    {
        await _resultBatchUploadService.Value.UploadResultBatch(batchRequest.Results);
        return Ok(batchRequest.Results.Count);
    }
}