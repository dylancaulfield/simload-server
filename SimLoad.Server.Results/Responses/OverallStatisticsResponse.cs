using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Results.Queries;
using SimLoad.Server.Results.Services;

namespace SimLoad.Server.Results.Responses;

[Serializable]
public class OverallStatisticsResponse
{
    public OverallStatisticsResponse(OverallStatistics statistics, int targetUserCount, int progress, double requestsPerSeconds)
    {
        MaxResponseTime = statistics.MaxResponseTime;
        MinResponseTime = statistics.MinResponseTime;
        MeanResponseTime = statistics.MeanResponseTime;
        RequestsPerSecond = requestsPerSeconds;
        ClientErrorRate = statistics.ClientErrorRate;
        ServerErrorRate = statistics.ServerErrorRate;
        TotalErrorRate = statistics.ClientErrorRate + statistics.ServerErrorRate;
        TargetUserCount = targetUserCount;
        TotalRequests = statistics.RequestCount;
        Progress = progress;
        ScenariosRan = statistics.UniqueUsers;
    }

    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public int TargetUserCount { get; set; }
    public int TotalRequests { get; set; }
    public double RequestsPerSecond { get; set; }
    public int Progress { get; set; }
    public int ScenariosRan { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public double TotalErrorRate { get; set; }
    
}