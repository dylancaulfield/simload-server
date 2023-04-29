using Autofac;
using SimLoad.Common.Interfaces;
using SimLoad.VirtualUser.Contexts;
using SimLoad.VirtualUser.Operations;

namespace SimLoad.VirtualUser.Registrations;

// Module that contains the services the virtual user will use
public class VirtualUserModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<VirtualUser>().As<IVirtualUser>().InstancePerLifetimeScope();
        builder.RegisterType<HttpRequestSender>().As<IHttpRequestSender>().InstancePerLifetimeScope();
        builder.RegisterType<ResultFactory>().As<IResultFactory>().InstancePerLifetimeScope();
        builder.RegisterType<ScriptCompiler>().As<IScriptCompiler>().InstancePerLifetimeScope();
        builder.RegisterType<VirtualUserScenarioScriptContext>().As<IScenarioScriptContext>()
            .InstancePerLifetimeScope();
    }
}