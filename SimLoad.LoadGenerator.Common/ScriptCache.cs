namespace SimLoad.LoadGenerator.Common;

public interface IScriptCache
{
    Script? GetScript(Guid scenarioId);
    void SetScript(Guid scenarioId, Script script);
}

public class ScriptCache : IScriptCache, IDisposable
{
    private readonly Dictionary<Guid, Script> _scripts = new();

    public void Dispose()
    {
        foreach (var script in _scripts) script.Value.AssemblyLoadContext.Unload();
        _scripts.Clear();
    }

    public Script? GetScript(Guid scenarioId)
    {
        return _scripts.GetValueOrDefault(scenarioId);
    }

    public void SetScript(Guid scenarioId, Script script)
    {
        _scripts[scenarioId] = script;
    }
}