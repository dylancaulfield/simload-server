namespace SimLoad.Common.Options;

/// <summary>
///     Mongo Connection Options
/// </summary>
public class MongoOptions
{
    public string? ConnectionString { get; init; }
    public string? Database { get; init; }
}