using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Results.Responses;
using SimLoad.Server.Results.Services;

namespace SimLoad.Server.Results.Queries;

public interface IOverallStatisticsQuery
{
    Task<OverallStatisticsResponse?> GetOverallStatistics(
        Test test, CancellationToken cancellationToken = default);
}

public class OverallStatisticsQuery : IOverallStatisticsQuery
{
    private readonly IMongoCollection<OverallStatistics> _statisticsView;
    private readonly ITargetUserCountCalculator _targetUserCountCalculator;

    public OverallStatisticsQuery(
        IMongoDatabase database,
        ITargetUserCountCalculator targetUserCountCalculator
    )
    {
        _statisticsView = database.GetCollection<OverallStatistics>("overallStatistics");
        _targetUserCountCalculator = targetUserCountCalculator;
    }

    public async Task<OverallStatisticsResponse?> GetOverallStatistics(
        Test test, CancellationToken cancellationToken = default)
    {
        var statistics = await _statisticsView
            .Find(s => s.TestId == test.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (statistics is null) return null;

        var targetUserCount = _targetUserCountCalculator.CalculateTargetUserCount(test);
        var elapsedPercentage = (DateTime.UtcNow - test.StartTime).TotalSeconds / test.Duration.TotalSeconds * 100;
        var requestsPerSecond = statistics.RequestCount / (DateTime.UtcNow - test.StartTime).TotalSeconds;

        return new OverallStatisticsResponse(statistics, targetUserCount, (int)elapsedPercentage, requestsPerSecond);
    }
}

[Serializable]
public class OverallStatistics
{
    [BsonElement("_id")] public Guid TestId { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public int RequestCount { get; set; }
    public int UniqueUsers { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }

}