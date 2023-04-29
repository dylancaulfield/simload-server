using System.ComponentModel.DataAnnotations.Schema;
using SimLoad.Common;

namespace SimLoad.Server.Data.Entities.Authentication;

[Table("refresh_token")]
public class RefreshToken
{
    [Column("id")] public Guid Id { get; set; }

    [Column("userid")] public Guid? UserId { get; set; }

    [Column("loadgeneratorcredentialid")] public Guid? LoadGeneratorCredentialId { get; set; }

    [Column("expires")] public DateTime Expires { get; set; }

    public AuthorizationType AuthorizationType =>
        UserId.HasValue ? AuthorizationType.User : AuthorizationType.LoadGenerator;
}