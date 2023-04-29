using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Users.Responses;

namespace SimLoad.Server.Users.Services;

public interface IGetUserService
{
    Task<IActionResult> GetUser();
}

public class GetUserService : IGetUserService
{
    private readonly UserDbContext _userDbContext;
    private readonly Guid? _userId;

    public GetUserService(IRequestContext requestContext, UserDbContext userDbContext)
    {
        _userId = requestContext.UserId;
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> GetUser()
    {
        var user = await _userDbContext.Users
            .Include(u => u.EmailAddresses)
            .SingleOrDefaultAsync(u => u.Id == _userId);
        if (user is null) return new NotFoundResult();

        var response = new GetUserResponse
        {
            DisplayName = user.DisplayName,
            EmailAddresses = user.EmailAddresses.Select(e => e.EmailAddress).ToList()
        };

        return new OkObjectResult(response);
    }
}