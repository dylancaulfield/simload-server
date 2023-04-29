namespace SimLoad.Common.Interfaces;

public interface IResult
{
    public DateTime Time { get; set; }

    /// <summary>
    ///     ID of the test this result was part of
    /// </summary>
    Guid TestId { get; }

    /// <summary>
    ///     ID of the operation
    /// </summary>
    Guid OperationId { get; }

    /// <summary>
    ///     ID of the virtual user who ran the scenario
    /// </summary>
    Guid VirtualUserId { get; }

    /// <summary>
    ///     ID of the scenario this result was created from
    /// </summary>
    Guid ScenarioId { get; }

    /// <summary>
    ///     ID of the Load Generator who created the result
    /// </summary>
    Guid LoadGeneratorId { get; }

    /// <summary>
    ///     HTTP method or WS if WebSocket
    /// </summary>
    string Method { get; }

    /// <summary>
    ///     Hostname including port
    /// </summary>
    string Host { get; }

    /// <summary>
    ///     Request path
    /// </summary>
    string Path { get; }

    /// <summary>
    ///     Query string of the request
    /// </summary>
    string Query { get; }

    /// <summary>
    ///     Name of the scenario this result was created in
    /// </summary>
    string ScenarioName { get; }

    /// <summary>
    ///     HTTP Status Code
    /// </summary>
    int ResponseCode { get; }

    /// <summary>
    ///     Total duration of the request in ms
    /// </summary>
    int Duration { get; }
}

public class SerializableResult : IResult
{
    public SerializableResult(DateTime time, Guid testId, Guid operationId, Guid virtualUserId, Guid scenarioId,
        Guid loadGeneratorId, string method, string host, string path, string query, string scenarioName,
        int responseCode, int duration)
    {
        Time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, 0, DateTimeKind.Utc);
        TestId = testId;
        OperationId = operationId;
        VirtualUserId = virtualUserId;
        ScenarioId = scenarioId;
        LoadGeneratorId = loadGeneratorId;
        Method = method;
        Host = host;
        Path = path;
        Query = query;
        ScenarioName = scenarioName;
        ResponseCode = responseCode;
        Duration = duration;
    }

    public DateTime Time { get; set; }
    public Guid TestId { get; }
    public Guid OperationId { get; }
    public Guid VirtualUserId { get; }
    public Guid ScenarioId { get; }
    public Guid LoadGeneratorId { get; }
    public string Method { get; }
    public string Host { get; }
    public string Path { get; }
    public string Query { get; }
    public string ScenarioName { get; }
    public int ResponseCode { get; }
    public int Duration { get; }
}