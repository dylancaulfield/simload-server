using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.LoadGenerator;

[Table("load_generator_credential")]
public class LoadGeneratorCredential
{
    [Column("id")] public Guid Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("apikey")] public Guid ApiKey { get; set; }

    [Column("organisationid")] public Guid OrganisationId { get; set; }

    public Organisation.Organisation Organisation { get; set; }
}