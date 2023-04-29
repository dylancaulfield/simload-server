using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.Scenario;

namespace SimLoad.Server.Data.Contexts;

public class ScenarioDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public ScenarioDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<Scenario> Scenarios { get; set; }
    public DbSet<ScenarioMember> ScenarioMembers { get; set; }
    public DbSet<ScenarioPermissions> ScenarioPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}