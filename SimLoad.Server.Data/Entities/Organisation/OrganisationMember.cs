using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Organisation;

[Table("organisation_member")]
public class OrganisationMember : IMember<Organisation>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("userid")] public Guid UserId { get; set; }

    public User.User User { get; set; }

    [Column("organisationid")] public Guid OrganisationId { get; set; }

    public Organisation Organisation { get; set; }

    public OrganisationPermissions OrganisationPermissions { get; set; }
}