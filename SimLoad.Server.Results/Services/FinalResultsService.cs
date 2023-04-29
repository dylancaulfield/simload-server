using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Common.Models.Scenario;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Results.Queries;
using SimLoad.Server.Results.Requests;
using SimLoad.Server.Results.Responses;
using SimLoad.Server.Results.Utils;

namespace SimLoad.Server.Results.Services;

public interface IFinalResultsService
{
    Task<IActionResult> GetFinalResults(ResultsRequest request);
}

public class FinalResultsService : IFinalResultsService
{
    private readonly IOverallStatisticsQuery _overallStatisticsQuery;
    private readonly IScenarioStatisticsQuery _scenarioStatisticsQuery;
    private readonly IOperationStatisticsQuery _operationStatisticsQuery;
    private readonly IIntervalStatisticsQuery _intervalStatisticsQuery;
    private readonly ITargetUserCountCalculator _targetUserCountCalculator;
    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly TestDbContext _testDbContext;

    public FinalResultsService(IOverallStatisticsQuery overallStatisticsQuery,
        IScenarioStatisticsQuery scenarioStatisticsQuery, IOperationStatisticsQuery operationStatisticsQuery,
        IIntervalStatisticsQuery intervalStatisticsQuery, ITargetUserCountCalculator targetUserCountCalculator,
        IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator,
        TestDbContext testDbContext)
    {
        _overallStatisticsQuery = overallStatisticsQuery;
        _scenarioStatisticsQuery = scenarioStatisticsQuery;
        _operationStatisticsQuery = operationStatisticsQuery;
        _intervalStatisticsQuery = intervalStatisticsQuery;
        _targetUserCountCalculator = targetUserCountCalculator;
        _permissionEvaluator = permissionEvaluator;
        _testDbContext = testDbContext;
    }

    public async Task<IActionResult> GetFinalResults(ResultsRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.ProjectId, p => true, async (_, _) =>
        {
            var test = await _testDbContext.Tests
                .Include(t => t.VirtualUserGraphs)
                .Where(t => t.Id == request.TestId)
                .FirstOrDefaultAsync(CancellationToken.None);
            if (test is null) return new NotFoundResult();

            if (!test.IsComplete())
            {
                return new NoContentResult();
            }

            var overallStatisticsTask = _overallStatisticsQuery.GetOverallStatistics(test);
            var scenarioStatisticsTask = _scenarioStatisticsQuery.GetScenarioStatistics(test);
            var operationStatisticsTask = _operationStatisticsQuery.GetOperationStatistics(test);
            var intervalStatisticsTask = _intervalStatisticsQuery.GetIntervalStatistics(test);
            await Task.WhenAll(overallStatisticsTask, scenarioStatisticsTask, operationStatisticsTask,
                intervalStatisticsTask);
            var overallStatistics = await overallStatisticsTask;
            var scenarioStatistics = await scenarioStatisticsTask;
            var operationStatistics = await operationStatisticsTask;
            var intervalStatistics = await intervalStatisticsTask;
            if (overallStatistics is null || scenarioStatistics is null ||
                operationStatistics is null || intervalStatistics is null) return new AcceptedResult();

            var scenarioOperations = new Dictionary<Guid, List<OperationStatistics>>();
            foreach (var operation in (await operationStatisticsTask)!)
            {
                if (scenarioOperations.TryGetValue(operation.ScenarioId, out var operationList))
                {
                    operationList.Add(operation);
                    continue;
                }

                scenarioOperations[operation.ScenarioId] = new List<OperationStatistics> { operation };
            }

            var response = new FinalResultsResponse
            {
                MeanResponseTime = overallStatistics.MeanResponseTime,
                MaxResponseTime = overallStatistics.MaxResponseTime,
                MinResponseTime = overallStatistics.MinResponseTime,
                TotalRequests = overallStatistics.TotalRequests,
                RequestsPerSecond = overallStatistics.RequestsPerSecond,
                ScenariosRan = overallStatistics.ScenariosRan,
                ClientErrorRate = overallStatistics.ClientErrorRate,
                ServerErrorRate = overallStatistics.ServerErrorRate,
                TotalErrorRate = overallStatistics.TotalErrorRate,
                MinuteIntervals = intervalStatistics.Select(i => new FinalResultInterval
                {
                    Minute = i.Timestamp,
                    MeanResponseTime = i.MeanResponseTime,
                    MaxResponseTime = i.MaxResponseTime,
                    MinResponseTime = i.MinResponseTime,
                    TargetUserCount = _targetUserCountCalculator.CalculateTargetUserCount(test, i.Timestamp),
                    TotalRequests = i.TotalRequests,
                    RequestsPerSecond = i.RequestsPerSecond,
                    ClientErrorRate = i.ClientErrorRate,
                    ServerErrorRate = i.ServerErrorRate,
                    TotalErrorRate = i.ClientErrorRate + i.ServerErrorRate
                }),
                Scenarios = scenarioStatistics.Select(s => new FinalResultScenario
                {
                    ScenarioId = s.ScenarioId,
                    ScenarioName = s.ScenarioName,
                    MeanResponseTime = s.MeanResponseTime,
                    MaxResponseTime = s.MaxResponseTime,
                    MinResponseTime = s.MinResponseTime,
                    ClientErrorRate = s.ClientErrorRate,
                    ServerErrorRate = s.ServerErrorRate,
                    TotalErrorRate = s.ClientErrorRate + s.ServerErrorRate,
                    TimesRan = s.TimesRan,
                    Operations = scenarioOperations[s.ScenarioId].Select(so => new FinalResultOperation
                    {
                        ScenarioId = so.ScenarioId,
                        ScenarioName = so.ScenarioName,
                        OperationId = so.OperationId,
                        Method = so.Method,
                        FullUrl = so.FullUrl,
                        MeanResponseTime = so.MeanResponseTime,
                        MaxResponseTime = so.MaxResponseTime,
                        MinResponseTime = so.MinResponseTime,
                        ClientErrorRate = so.ClientErrorRate,
                        ServerErrorRate = so.ServerErrorRate,
                        TotalErrorRate = so.ClientErrorRate + so.ServerErrorRate
                    })
                })
            };

            return new OkObjectResult(response);
            
        });
    }
}