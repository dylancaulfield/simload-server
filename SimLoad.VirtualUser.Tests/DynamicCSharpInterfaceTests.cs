using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimLoad.Common.Interfaces;
using Xunit;

namespace SimLoad.VirtualUser.Tests;

public class DynamicCSharpInterfaceTests
{
    private const string Code = @"

        using System;
        using System.Runtime;
        using System.Threading.Tasks;
        using SimLoad.Common.Interfaces;

        namespace SimLoad.VirtualUser.Dynamic;

        public class Script
        {
            
            private readonly IScenarioScriptContext context;

            public Script(IScenarioScriptContext context)
            {
                this.context = context;
            }

            public async Task Run() 
            {
                await context.Delay(5000);
            }

        }
        
    ";

    [Fact]
    public async Task CSharpDynamicCodeWorks()
    {
        var sharedDict = new Dictionary<string, bool>();
        var context = new TestCSharpScenarioScriptContext(sharedDict);

        var syntaxTree = CSharpSyntaxTree.ParseText(Code);
        var assemblyName = Path.GetRandomFileName();
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

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();

        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // handle exceptions
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
                Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

            Assert.True(false);
        }
        else
        {
            // load this 'virtual' DLL so that we can use
            ms.Seek(0, SeekOrigin.Begin);

            var assemblyLoadContext = new AssemblyLoadContext("assemblyLoadContext", true);

            var assembly = assemblyLoadContext.LoadFromStream(ms);

            // create instance of the desired class and call the desired function
            var type = assembly.GetType("SimLoad.VirtualUser.Dynamic.Script")!;

            var constructorInfo = type.GetConstructors().FirstOrDefault(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 1;
            });
            Assert.NotNull(constructorInfo);

            var script = constructorInfo.Invoke(new object[] { context });

            var runMethod = type.GetMethods().FirstOrDefault(m => m.Name == "Run" && m.ReturnType == typeof(Task));
            Assert.NotNull(runMethod);

            var task = (Task)runMethod.Invoke(script,
                new object[] { });

            await task;

            assemblyLoadContext.Unload();
        }
    }
}

internal class TestCSharpScenarioScriptContext : IScenarioScriptContext
{
    private readonly Dictionary<string, bool> _sharedValues;

    public TestCSharpScenarioScriptContext(Dictionary<string, bool> sharedValues)
    {
        _sharedValues = sharedValues;
    }

    public async Task Delay(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    public Task Http(string operationIdString)
    {
        throw new NotImplementedException();
    }

    public void Log(string message)
    {
        throw new NotImplementedException();
    }

    public void set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public string get(string key)
    {
        throw new NotImplementedException();
    }
}