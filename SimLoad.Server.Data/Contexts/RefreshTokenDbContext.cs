using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.Authentication;

namespace SimLoad.Server.Data.Contexts;

public class RefreshTokenDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public RefreshTokenDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}