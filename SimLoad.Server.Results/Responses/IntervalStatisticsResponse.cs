namespace SimLoad.Server.Results.Responses;

public class IntervalStatisticsResponse
{
    public List<IntervalStatistics> MinuteIntervals { get; set; } = new();
}

public class IntervalStatistics : BasicStatistics
{

    public IntervalStatistics(BasicStatistics statistics, DateTime time)
    {
        MaxResponseTime = statistics.MaxResponseTime;
        MinResponseTime = statistics.MinResponseTime;
        MeanResponseTime = statistics.MeanResponseTime;
        RequestsPerSecond = statistics.RequestsPerSecond;
        ErrorRate = statistics.ErrorRate;
        Time = time;
    }
    
    public DateTime Time { get; set; }
}

public class BasicStatistics
{
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double MeanResponseTime { get; set; }
    public double RequestsPerSecond { get; set; }
    public double ErrorRate { get; set; }
}