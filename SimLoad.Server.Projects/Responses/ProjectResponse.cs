namespace SimLoad.Server.Projects.Responses;

public record ProjectResponse
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ProjectResponseMember> Members { get; set; }
    public List<ProjectResponseScenario> Scenarios { get; set; }
}

public record ProjectResponseMember
{
    public string DisplayName { get; set; }
}

public record ProjectResponseScenario
{
    public Guid ScenarioId { get; set; }
    public string Name { get; set; }
}