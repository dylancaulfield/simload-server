namespace SimLoad.Common.Models.Scenario;

/// <summary>
///     All of the possible properties any request type can have
/// </summary>
public class Request
{
    public string Scheme { get; set; }
    public string Method { get; set; }
    public string Host { get; set; }
    public string Path { get; set; }
    public List<QueryParam> Query { get; set; }
    public List<Header> Headers { get; set; }
    public List<Cookie> Cookies { get; set; }

    /// <summary>
    ///     Http request body or WebSocket message body
    /// </summary>
    public string? Body { get; set; }
}