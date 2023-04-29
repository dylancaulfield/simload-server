using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Data.ValueConverters;

namespace SimLoad.Server.Data.Contexts;

public class TestDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public TestDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<Test> Tests { get; set; }
    public DbSet<TestScenarioWeight> TestScenarioWeights { get; set; }
    public DbSet<TestVirtualUserGraph> TestVirtualUserGraphs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<TimeSpan>()
            .HaveConversion<TimeSpanConverter>();
    }
}