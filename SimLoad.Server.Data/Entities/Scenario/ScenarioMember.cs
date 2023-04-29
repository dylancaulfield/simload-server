using System.ComponentModel.DataAnnotations.Schema;
using SimLoad.Server.Data.Entities.Project;

namespace SimLoad.Server.Data.Entities.Scenario;

[Table("scenario_member")]
public class ScenarioMember : IMember<Scenario>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("projectmemberid")] public Guid ProjectMemberId { get; set; }

    public ProjectMember ProjectMember { get; set; }

    [Column("scenarioid")] public Guid ScenarioId { get; set; }

    public Scenario Scenario { get; set; }

    public ScenarioPermissions Permissions { get; set; }
}