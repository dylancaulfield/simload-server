using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SimLoad.Common.Interfaces;

namespace SimLoad.Common.Models;

/// <summary>
///     Holds the metrics and properties of a request sent by the
///     load generator during a test
/// </summary>
public class Result
{
    public Result(IResult result)
    {
        Id = ObjectId.GenerateNewId();
        Timestamp = result.Time;
        TestId = result.TestId;
        OperationId = result.OperationId;
        VirtualUserId = result.VirtualUserId;
        ScenarioId = result.ScenarioId;
        LoadGeneratorId = result.LoadGeneratorId;
        Method = result.Method;
        Host = result.Host;
        Path = result.Path;
        Query = result.Query;
        ScenarioName = result.ScenarioName;
        ResponseCode = result.ResponseCode;
        Duration = result.Duration;
    }

    /// <summary>
    ///     ObjectId for MongoDB
    /// </summary>
    [BsonId]
    public ObjectId Id { get; }

    /// <summary>
    ///     Timestamp of the request
    /// </summary>
    [BsonElement("timestamp")]
    public BsonDateTime Timestamp { get; }

    /// <summary>
    ///     ID of the test this result was part of
    /// </summary>
    [BsonElement("testId")]
    public Guid TestId { get; }

    /// <summary>
    ///     ID of the operation
    /// </summary>
    [BsonElement("operationId")]
    public Guid OperationId { get; }

    /// <summary>
    ///     ID of the virtual user who ran the scenario
    /// </summary>
    [BsonElement("virtualUserId")]
    public Guid VirtualUserId { get; }

    /// <summary>
    ///     ID of the scenario this result was created from
    /// </summary>
    [BsonElement("scenarioId")]
    public Guid ScenarioId { get; }

    /// <summary>
    ///     ID of the Load Generator who created the result
    /// </summary>
    [BsonElement("loadGeneratorId")]
    public Guid LoadGeneratorId { get; }

    /// <summary>
    ///     HTTP method or WS if WebSocket
    /// </summary>
    [BsonElement("method")]
    public string Method { get; }

    /// <summary>
    ///     Hostname including port
    /// </summary>
    [BsonElement("host")]
    public string Host { get; }

    /// <summary>
    ///     Request path
    /// </summary>
    [BsonElement("path")]
    public string Path { get; }

    /// <summary>
    ///     Query string of the request
    /// </summary>
    [BsonElement("query")]
    public string Query { get; }

    /// <summary>
    ///     Name of the scenario this result was created in
    /// </summary>
    [BsonElement("scenarioName")]
    public string ScenarioName { get; }

    /// <summary>
    ///     HTTP Status Code
    /// </summary>
    [BsonElement("responseCode")]
    public int ResponseCode { get; }

    /// <summary>
    ///     Total duration of the request in ms
    /// </summary>
    [BsonElement("duration")]
    public int Duration { get; }
}