using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimLoad.Server.Data.Entities.Test;

namespace SimLoad.Server.Results.Queries;

public interface IIntervalStatisticsQuery
{
    Task<List<IntervalStatistics>?> GetIntervalStatistics(
        Test test, CancellationToken cancellationToken = default);
}

public class IntervalStatisticsQuery : IIntervalStatisticsQuery
{
    private readonly IMongoCollection<IntervalStatistics> _statisticsView;

    public IntervalStatisticsQuery(
        IMongoDatabase database
    )
    {
        _statisticsView = database.GetCollection<IntervalStatistics>("intervalStatistics");
    }
    
    public async Task<List<IntervalStatistics>?> GetIntervalStatistics(
        Test test, CancellationToken cancellationToken = default)
    {
        return await _statisticsView
            .Find(s => s.TestId == test.Id)
            .ToListAsync(cancellationToken);
    }
}

[Serializable]
public class IntervalStatistics
{
    [BsonElement("_id")] public Guid TestId { get; set; }
    public DateTime Timestamp { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public int TotalRequests { get; set; }
    public double RequestsPerSecond { get; set; }
}