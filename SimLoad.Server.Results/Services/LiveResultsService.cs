using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Results.Queries;
using SimLoad.Server.Results.Requests;
using SimLoad.Server.Results.Responses;
using SimLoad.Server.Results.Utils;

namespace SimLoad.Server.Results.Services;

public interface ILiveResultsService
{
    Task<IActionResult> GetLiveResults(ResultsRequest request, CancellationToken cancellationToken);
}

public class LiveResultsService : ILiveResultsService
{
    private readonly Lazy<IOverallStatisticsQuery> _overallStatisticsQuery;
    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly TestDbContext _testDbContext;

    public LiveResultsService(
        Lazy<IOverallStatisticsQuery> overallStatisticsQuery,
        IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator,
        TestDbContext testDbContext
    )
    {
        _overallStatisticsQuery = overallStatisticsQuery;
        _permissionEvaluator = permissionEvaluator;
        _testDbContext = testDbContext;
    }

    public async Task<IActionResult> GetLiveResults(ResultsRequest request, CancellationToken cancellationToken)
    {
        
        return await _permissionEvaluator.Evaluate(request.ProjectId, p => true, async (_, _) =>
        {
            var test = await _testDbContext.Tests
                .Include(t => t.VirtualUserGraphs)
                .Where(t => t.Id == request.TestId)
                .FirstOrDefaultAsync(cancellationToken);
            if (test is null) return new NotFoundResult();

            if (test.IsComplete())
            {
                return new NoContentResult();
            }

            var statistics = await _overallStatisticsQuery.Value.GetOverallStatistics(test, cancellationToken);
            if (statistics is null) return new AcceptedResult();

            return new OkObjectResult(statistics);

        });
        
    }

}

