using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models.Scenario;
using SimLoad.LoadGenerator.Common;
using SimLoad.VirtualUser.Contexts;

namespace SimLoad.VirtualUser;

public interface IScriptCompiler
{
    Script? GetScript();
}

public class ScriptCompiler : IScriptCompiler
{
    private readonly ILogger<ScriptCompiler> _logger;
    private readonly Scenario _scenario;
    private readonly IScenarioScriptContext _scenarioScriptContext;
    private readonly IScriptCache _scriptCache;

    public ScriptCompiler(
        IScenarioScriptContext scenarioScriptContext,
        Scenario scenario,
        ILogger<ScriptCompiler> logger,
        IScriptCache scriptCache
    )
    {
        _scenarioScriptContext = scenarioScriptContext;
        _scenario = scenario;
        _logger = logger;
        _scriptCache = scriptCache;
    }

    public Script? GetScript()
    {
        var cachedAssembly = _scriptCache.GetScript(_scenario.ScenarioId);
        if (cachedAssembly is not null) return cachedAssembly;

        var compilation = Compile();
        var memoryStream = new MemoryStream();
        var result = compilation.Emit(memoryStream);
        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics.Where(diagnostic =>
                         diagnostic.IsWarningAsError ||
                         diagnostic.Severity == DiagnosticSeverity.Error))
                _logger.LogError(diagnostic.Id, diagnostic.GetMessage());
            return null;
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        var assemblyLoadContext = new AssemblyLoadContext(Path.GetRandomFileName(), true);
        var assembly = assemblyLoadContext.LoadFromStream(memoryStream);
        memoryStream.Dispose();

        var scriptObjectType = assembly.GetType("SimLoad.VirtualUser.Dynamic.Script")!;
        var scriptObjectRunMethod = scriptObjectType.GetMethods()
            .FirstOrDefault(m => m.Name == "Run" && m.ReturnType == typeof(Task));
        if (scriptObjectRunMethod is null) return null;

        var script = new Script
        {
            AssemblyLoadContext = assemblyLoadContext,
            RunMethod = scriptObjectRunMethod
        };

        _scriptCache.SetScript(_scenario.ScenarioId, script);

        return script;
    }

    private CSharpCompilation Compile()
    {
        var code = CSharpScriptConstants.Top + _scenario.Code;

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IScenarioScriptContext).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
        };

        return CSharpCompilation.Create(
            Script.ScriptAssemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}