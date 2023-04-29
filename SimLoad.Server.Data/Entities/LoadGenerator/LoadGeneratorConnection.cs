using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.LoadGenerator;

[Table("load_generator_connection")]
public class LoadGeneratorConnection
{
    [Column("id")] public Guid Id { get; set; }

    [Column("lastupdated")] public DateTime LastUpdated { get; set; }

    [Column("available")] public bool Available { get; set; }

    [Column("ipAddress")] public string IpAddress { get; set; }

    [Column("organisationid")] public Guid OrganisationId { get; set; }

    public Organisation.Organisation Organisation { get; set; }
}