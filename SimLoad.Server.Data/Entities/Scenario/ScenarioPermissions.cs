using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Scenario;

[Table("scenario_permission")]
public class ScenarioPermissions : IPermissions<Scenario>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("scenario_edit")] public bool ScenarioEdit { get; set; }

    [Column("scenario_delete")] public bool ScenarioDelete { get; set; }

    [Column("scenario_addmember")] public bool ScenarioAddMember { get; set; }

    [Column("scenario_removemember")] public bool ScenarioRemoveMember { get; set; }

    [Column("scenario_changepermission")] public bool ScenarioChangePermission { get; set; }

    [Column("scenariomemberid")] public Guid ScenarioMemberId { get; set; }

    public ScenarioMember ScenarioMember { get; set; }
}