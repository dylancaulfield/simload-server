using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Project;

[Table("project_permission")]
public class ProjectPermissions : IPermissions<Project>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("project_edit")] public bool ProjectEdit { get; set; }

    [Column("project_delete")] public bool ProjectDelete { get; set; }

    [Column("project_addmember")] public bool ProjectAddMember { get; set; }

    [Column("project_removemember")] public bool ProjectRemoveMember { get; set; }

    [Column("project_changepermission")] public bool ProjectChangePermission { get; set; }

    [Column("scenario_create")] public bool ScenarioCreate { get; set; }

    [Column("projectmemberid")] public Guid ProjectMemberId { get; set; }

    public ProjectMember ProjectMember { get; set; }
}