using Autofac;
using SimLoad.Server.Data.Contexts;

namespace SimLoad.Server.Data.Registrations;

public class DataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UserDbContext>().InstancePerDependency();
        builder.RegisterType<LoadGeneratorDbContext>().InstancePerDependency();
        builder.RegisterType<RefreshTokenDbContext>().InstancePerDependency();
        builder.RegisterType<ProjectDbContext>().InstancePerDependency();
        builder.RegisterType<ScenarioDbContext>().InstancePerDependency();
        builder.RegisterType<OrganisationDbContext>().InstancePerDependency();
        builder.RegisterType<TestDbContext>().InstancePerDependency();
    }
}