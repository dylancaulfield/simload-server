using System.Reflection;
using System.Runtime.Loader;
using SimLoad.Common.Interfaces;

namespace SimLoad.LoadGenerator.Common;

public class Script
{
    public const string ScriptAssemblyName = "ScriptAssembly";

    public AssemblyLoadContext AssemblyLoadContext { get; set; }
    public MethodInfo RunMethod { get; set; }

    public object? GetInstance(IScenarioScriptContext scenarioScriptContext)
    {
        var assembly = AssemblyLoadContext.LoadFromAssemblyName(new AssemblyName(ScriptAssemblyName));

        var scriptObjectType = assembly.GetType("SimLoad.VirtualUser.Dynamic.Script")!;
        var scriptObjectConstructor = scriptObjectType.GetConstructors().FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 1;
        });
        if (scriptObjectConstructor is null) return null;
        return scriptObjectConstructor.Invoke(new object[] { scenarioScriptContext });
    }
}