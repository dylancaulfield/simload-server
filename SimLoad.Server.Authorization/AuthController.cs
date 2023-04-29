using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Authorization.Requests;
using SimLoad.Server.Authorization.Services;
using SimLoad.Server.Authorization.Services.Authorization;
using SimLoad.Server.Common;

namespace SimLoad.Server.Authorization;

[Route("/api/auth")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly Lazy<ISimLoadAuthorizationService> _authorizationService;
    private readonly Lazy<ICreateLoginSessionService> _createSessionService;
    private readonly IRequestWrapper _requestWrapper;

    public AuthController(
        Lazy<ICreateLoginSessionService> createSessionService,
        Lazy<ISimLoadAuthorizationService> authorizationService,
        IRequestWrapper requestWrapper
    )
    {
        _createSessionService = createSessionService;
        _authorizationService = authorizationService;
        _requestWrapper = requestWrapper;
    }

    /// <summary>
    ///     Create
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("session")]
    public async Task<IActionResult> CreateLoginSession([FromBody] CreateSessionRequest request)
    {
        return await _requestWrapper.Wrap(async () => await _createSessionService.Value.CreateLoginSession(request));
    }

    /// <summary>
    ///     Authenticate a user with a session ID and confirmation code
    ///     or a load generator with an API key
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Access, ID and Refresh Tokens</returns>
    [HttpPost]
    [Route("token")]
    public async Task<IActionResult> Authenticate([FromBody] AuthorizationRequest request)
    {
        return await _requestWrapper.Wrap(async () => await _authorizationService.Value.Authorize(request));
    }

    /// <summary>
    ///     Refresh the access and refresh tokens
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshAuthorizationRequest request)
    {
        return await _requestWrapper.Wrap(async () => await _authorizationService.Value.Refresh(request));
    }
}