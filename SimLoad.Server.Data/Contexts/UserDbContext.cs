using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.User;

namespace SimLoad.Server.Data.Contexts;

public class UserDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public UserDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}