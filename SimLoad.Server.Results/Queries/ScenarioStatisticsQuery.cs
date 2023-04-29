using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Results.Responses;
using SimLoad.Server.Results.Services;

namespace SimLoad.Server.Results.Queries;

public interface IScenarioStatisticsQuery
{
    Task<List<ScenarioStatistics>?> GetScenarioStatistics(
        Test test, CancellationToken cancellationToken = default);
}

public class ScenarioStatisticsQuery : IScenarioStatisticsQuery
{
    private readonly IMongoCollection<ScenarioStatistics> _statisticsView;

    public ScenarioStatisticsQuery(
        IMongoDatabase database
    )
    {
        _statisticsView = database.GetCollection<ScenarioStatistics>("scenarioStatistics");
    }
    
    public async Task<List<ScenarioStatistics>?> GetScenarioStatistics(
        Test test, CancellationToken cancellationToken = default)
    {
        return await _statisticsView
            .Find(s => s.TestId == test.Id)
            .ToListAsync(cancellationToken);
    }
    
}

[Serializable]
public class ScenarioStatistics
{
    [BsonElement("_id")] public Guid TestId { get; set; }
    public Guid ScenarioId { get; set; }
    public string ScenarioName { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public int TimesRan { get; set; }
}