namespace SimLoad.Common.Models.Scenario;

public class Operation
{
    public Request OriginalRequest { get; set; }
    public Response? OriginalResponse { get; set; }
    public Request Request { get; set; }
    public OperationType Type { get; set; }
}

/// <summary>
///     Type of request to be sent, determines what properties are required
/// </summary>
public enum OperationType
{
    Http = 0,
    RawWebSocketChannel = 1,
    RawWebSocketMessage = 2,
    SignalRChannel = 3,
    SignalRMessage = 4,
    GraphQLChannel = 5,
    GraphQLMessage = 6
}