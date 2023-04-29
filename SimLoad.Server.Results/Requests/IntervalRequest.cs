namespace SimLoad.Server.Results.Requests;

public class IntervalRequest
{
    public Guid TestId { get; set; }
    public DateTime IntervalStart { get; set; }
    public DateTime IntervalEnd { get; set; }
}