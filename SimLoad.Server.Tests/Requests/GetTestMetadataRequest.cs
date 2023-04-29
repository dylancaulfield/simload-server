namespace SimLoad.Server.Tests.Requests;

public class GetTestMetadataRequest
{
    public Guid ProjectId { get; set; }
    public Guid TestId { get; set; }
}