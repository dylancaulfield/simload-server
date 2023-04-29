using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimLoad.Server.Data.Entities.Test;

namespace SimLoad.Server.Results.Queries;

public interface IOperationStatisticsQuery
{
    Task<List<OperationStatistics>?> GetOperationStatistics(
        Test test, CancellationToken cancellationToken = default);
}

public class OperationStatisticsQuery : IOperationStatisticsQuery
{
    private readonly IMongoCollection<OperationStatistics> _statisticsView;

    public OperationStatisticsQuery(
        IMongoDatabase database
    )
    {
        _statisticsView = database.GetCollection<OperationStatistics>("operationStatistics");
    }
    
    public async Task<List<OperationStatistics>?> GetOperationStatistics(
        Test test, CancellationToken cancellationToken = default)
    {
        return await _statisticsView
            .Find(s => s.TestId == test.Id)
            .ToListAsync(cancellationToken);
    }
}


[Serializable]
public class OperationStatistics
{
    [BsonElement("_id")] public Guid TestId { get; set; }
    public Guid OperationId { get; set; }
    public Guid ScenarioId { get; set; }
    public string ScenarioName { get; set; }
    public string Method { get; set; }
    public string FullUrl { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
}