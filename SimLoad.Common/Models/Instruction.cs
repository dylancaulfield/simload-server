namespace SimLoad.Common.Models;

/// <summary>
///     Contains the properties of a test, such as testId, start time, duration and user count
///     and is used to request the test content from the server
/// </summary>
public class Instruction
{
    public Guid ProjectId { get; set; }
    public Guid TestId { get; init; }
    public Guid LoadGeneratorId { get; init; }
    public bool Cancel { get; init; } = false;
}