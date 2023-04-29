using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Data.Entities.Organisation;

namespace SimLoad.Server.Data.Contexts;

public class OrganisationDbContext : DbContext
{
    private readonly PostgresOptions _postgresOptions;

    public OrganisationDbContext(IOptions<PostgresOptions> postgresOptions)
    {
        _postgresOptions = postgresOptions.Value;
    }

    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<OrganisationMember> OrganisationMembers { get; set; }
    public DbSet<OrganisationPermissions> OrganisationPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(_postgresOptions.ConnectionString);
    }
}