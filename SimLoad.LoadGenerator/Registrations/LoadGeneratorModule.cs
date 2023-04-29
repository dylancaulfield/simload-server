using Autofac;
using SimLoad.Common;
using SimLoad.LoadGenerator.Lifecycle;
using SimLoad.LoadGenerator.Results;
using SimLoad.LoadGenerator.Scenarios;
using SimLoad.LoadGenerator.Tests;
using SimLoad.VirtualUser.Registrations;

namespace SimLoad.LoadGenerator.Registrations;

/// <summary>
///     Module containing services for the main load generator service
/// </summary>
public class LoadGeneratorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Load generator level
        builder.RegisterType<HubConnectionFactory>().As<IHubConnectionFactory>().SingleInstance();
        builder.RegisterType<CancellationTokenProvider>().As<ICancellationTokenProvider>();
        builder.RegisterType<TestRetrievalService>().As<ITestRetrievalService>();
        builder.RegisterType<CredentialsProvider>().As<ICredentialsProvider>().SingleInstance();
        builder.RegisterType<UpdateConnectionService>().As<IUpdateConnectionService>();

        // Test runner level
        builder.RegisterType<ResultSender>().As<IResultSender>().InstancePerLifetimeScope();
        builder.RegisterType<TestRunner>().As<ITestRunner>().InstancePerLifetimeScope();
        builder.RegisterType<UserCountDeltaCalculator>().As<IUserCountDeltaCalculator>();
        builder.RegisterType<RequestTimingHandler>();

        // Scenario runner level
        builder.RegisterType<ScenarioRunner>().As<IScenarioRunner>().InstancePerLifetimeScope();
        builder.RegisterType<ScenarioProvider>().As<IScenarioProvider>().InstancePerLifetimeScope();

        // Virtual user level
        builder.RegisterModule(new VirtualUserModule());
    }
}