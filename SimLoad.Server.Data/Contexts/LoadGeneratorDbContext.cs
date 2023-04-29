using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.LoadGenerator;

namespace SimLoad.Server.Data.Contexts;

public class LoadGeneratorDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public LoadGeneratorDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<LoadGeneratorCredential> LoadGeneratorCredentials { get; set; }
    public DbSet<LoadGeneratorConnection> LoadGeneratorConnections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}