using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Users.Requests;
using SimLoad.Server.Users.Services;

namespace SimLoad.Server.Users;

[Route("/api/users")]
[ApiController]
[Authorize(Policy = "User")]
public class UserController : ControllerBase
{
    private readonly Lazy<ICreateUserService> _createUserService;
    private readonly Lazy<IGetUserService> _getUserService;

    public UserController(Lazy<ICreateUserService> createUserService, Lazy<IGetUserService> getUserService)
    {
        _createUserService = createUserService;
        _getUserService = getUserService;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetCurrentUser()
    {
        return await _getUserService.Value.GetUser();
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        return await _createUserService.Value.CreateUser(request);
    }
}