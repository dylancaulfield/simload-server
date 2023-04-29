using System.ComponentModel.DataAnnotations.Schema;
using SimLoad.Server.Data.Entities.LoadGenerator;

namespace SimLoad.Server.Data.Entities.Organisation;

[Table("organisation")]
public class Organisation
{
    [Column("id")] public Guid Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    public List<Project.Project> Projects { get; set; }
    public List<OrganisationMember> OrganisationMembers { get; set; }
    public List<LoadGeneratorCredential> LoadGeneratorCredentials { get; set; }
    public List<LoadGeneratorConnection> LoadGeneratorConnections { get; set; }
}