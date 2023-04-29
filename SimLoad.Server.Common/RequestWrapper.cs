using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimLoad.Server.Common;

internal class ErrorResponse
{
    public string Message { get; set; }
    public string? Stacktrace { get; set; }
}

public interface IRequestWrapper
{
    Task<IActionResult> Wrap(Func<Task<IActionResult>> request);
}

public class RequestWrapper : IRequestWrapper
{
    public async Task<IActionResult> Wrap(Func<Task<IActionResult>> request)
    {
        try
        {
            return await request();
        }
        catch (Exception e)
        {
            var response = new ErrorResponse
            {
                Message = e.Message,
                Stacktrace = e.StackTrace
            };

            var result = new OkObjectResult(response);
            result.StatusCode = StatusCodes.Status500InternalServerError;
            return result;
        }
    }
}