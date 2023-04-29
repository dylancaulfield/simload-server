using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Scenario;

[Table("scenario")]
public class Scenario
{
    [Column("id")] public Guid Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("lastupdated")] public DateTime LastUpdated { get; set; }

    public List<ScenarioMember> ScenarioMembers { get; set; }

    [Column("projectid")] public Guid ProjectId { get; set; }

    public Project.Project Project { get; set; }
}