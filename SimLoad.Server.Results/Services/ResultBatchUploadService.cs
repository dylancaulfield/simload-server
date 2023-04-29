using MongoDB.Driver;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;

namespace SimLoad.Server.Results.Services;

public interface IResultBatchUploadService
{
    Task UploadResultBatch(List<SerializableResult> results);
}

/// <summary>
///     Service to upload a batch of results to MongoDB
/// </summary>
public class ResultBatchUploadService : IResultBatchUploadService
{
    private readonly IMongoCollection<Result> _resultsCollection;

    public ResultBatchUploadService(IMongoCollection<Result> resultsCollection)
    {
        _resultsCollection = resultsCollection;
    }

    public async Task UploadResultBatch(List<SerializableResult> results)
    {
        var resultsToInsert = results.Select(r => new Result(r)).ToList();

        await _resultsCollection.InsertManyAsync(resultsToInsert);
    }
}