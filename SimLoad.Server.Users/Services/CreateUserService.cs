using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.User;
using SimLoad.Server.Users.Requests;

namespace SimLoad.Server.Users.Services;

public interface ICreateUserService
{
    Task<IActionResult> CreateUser(CreateUserRequest request);
}

public class CreateUserService : ICreateUserService
{
    private readonly UserDbContext _userDbContext;

    public CreateUserService(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            DisplayName = request.DisplayName
        };

        var userEmail = new UserEmail
        {
            User = user,
            UserId = userId,
            EmailAddress = request.EmailAddress
        };

        user.EmailAddresses = new List<UserEmail> { userEmail };

        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();

        return new OkObjectResult(userId);
    }
}