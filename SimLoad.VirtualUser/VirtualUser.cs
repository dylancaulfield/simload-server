using System.Diagnostics.CodeAnalysis;
using SimLoad.Common.Interfaces;

namespace SimLoad.VirtualUser;

public interface IVirtualUser
{
    Task RunScenario();
}

/// <summary>
///     Compile C# and begin execution
/// </summary>
public class VirtualUser : IVirtualUser
{
    private readonly IScriptCompiler _compiler;
    private readonly IScenarioScriptContext _scenarioScriptContext;

    public VirtualUser(IScriptCompiler compiler, IScenarioScriptContext scenarioScriptContext)
    {
        _compiler = compiler;
        _scenarioScriptContext = scenarioScriptContext;
    }

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    public async Task RunScenario()
    {
        var script = _compiler.GetScript();
        if (script is null) return;

        var scriptInstance = script.GetInstance(_scenarioScriptContext);
        if (scriptInstance is null) return;

        var task = (Task?)script.RunMethod.Invoke(scriptInstance, new object[] { });
        if (task is null) return;
        await task;
    }
}