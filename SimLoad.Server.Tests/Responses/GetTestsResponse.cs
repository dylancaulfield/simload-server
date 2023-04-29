namespace SimLoad.Server.Tests.Responses;

public class GetTestsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public TestStatus Status { get; set; }
}

public enum TestStatus
{
    InProgress,
    Complete
}