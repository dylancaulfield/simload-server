using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.Project;

namespace SimLoad.Server.Data.Contexts;

public class ProjectDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public ProjectDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectPermissions> ProjectPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}