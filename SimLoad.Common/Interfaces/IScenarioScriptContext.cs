using System.Diagnostics.CodeAnalysis;

namespace SimLoad.Common.Interfaces;

/// <summary>
///     How the JavaScript communicates with C#
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public interface IScenarioScriptContext
{
    /// <summary>
    ///     Pause script execution between requests for <paramref name="milliseconds" /> milliseconds
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds to wait for</param>
    public Task Delay(int milliseconds);

    /// <summary>
    ///     Make a HttpRequest request with id <paramref name="operationIdString" />
    /// </summary>
    /// <param name="operationIdString"></param>
    public Task Http(string operationIdString);

    /// <summary>
    ///     Log a message to the console
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message);

    /// <summary>
    ///     Set a variable that is local to the Virtual User
    /// </summary>
    /// <param name="key">Name of the variable</param>
    /// <param name="value">Value of the variable</param>
    public void set(string key, string value);

    /// <summary>
    ///     Get a variable that is local to the Virtual User
    /// </summary>
    /// <param name="key">Name of the variable</param>
    /// <returns>The variable if found, or an empty string</returns>
    public string get(string key);
}