using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Project;

[Table("project")]
public class Project
{
    [Column("id")] public Guid Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    public List<ProjectMember> ProjectMembers { get; set; }

    public List<Scenario.Scenario> Scenarios { get; set; }

    [Column("organisationid")] public Guid OrganisationId { get; set; }

    public Organisation.Organisation Organisation { get; set; }
}