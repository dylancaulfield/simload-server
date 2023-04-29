using System.ComponentModel.DataAnnotations.Schema;
using SimLoad.Server.Data.Entities.Organisation;

namespace SimLoad.Server.Data.Entities.Project;

[Table("project_member")]
public class ProjectMember : IMember<Project>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("projectid")] public Guid ProjectId { get; set; }

    public Project Project { get; set; }

    [Column("organisationmemberid")] public Guid OrganisationMemberId { get; set; }

    public OrganisationMember OrganisationMember { get; set; }

    public ProjectPermissions Permissions { get; set; }
}