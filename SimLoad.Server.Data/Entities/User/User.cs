using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.User;

[Table("user")]
public class User
{
    [Column("id")] public Guid Id { get; set; }

    [Column("displayname")] public string DisplayName { get; set; }

    public List<UserEmail> EmailAddresses { get; set; }
}