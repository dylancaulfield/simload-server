namespace SimLoad.Server.Results.Responses;

public class FinalResultsResponse
{
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public int TotalRequests { get; set; }
    public double RequestsPerSecond { get; set; }
    public int ScenariosRan { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public double TotalErrorRate { get; set; }
    public IEnumerable<FinalResultInterval> MinuteIntervals { get; set; }
    public IEnumerable<FinalResultScenario> Scenarios { get; set; }
}

public class FinalResultInterval
{
    public DateTime Minute { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public int TargetUserCount { get; set; }
    public int TotalRequests { get; set; }
    public double RequestsPerSecond { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public double TotalErrorRate { get; set; }
}

public class FinalResultScenario
{
    public Guid ScenarioId { get; set; }
    public string ScenarioName { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public double TotalErrorRate { get; set; }
    public int TimesRan { get; set; }

    public IEnumerable<FinalResultOperation> Operations { get; set; }
    
}

public class FinalResultOperation
{
    public Guid ScenarioId { get; set; }
    public string ScenarioName { get; set; }
    public Guid OperationId { get; set; }
    public string Method { get; set; }
    public string FullUrl { get; set; }
    public double MeanResponseTime { get; set; }
    public int MaxResponseTime { get; set; }
    public int MinResponseTime { get; set; }
    public double ClientErrorRate { get; set; }
    public double ServerErrorRate { get; set; }
    public double TotalErrorRate { get; set; }
}