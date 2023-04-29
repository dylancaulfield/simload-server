using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimLoad.Server.Data.Entities.User;

[Table("user_email")]
[PrimaryKey(nameof(UserId), nameof(EmailAddress))]
public class UserEmail
{
    [Column("userid")] public Guid UserId { get; set; }

    [Column("emailaddress")] public string EmailAddress { get; set; }

    public User User { get; set; }
}