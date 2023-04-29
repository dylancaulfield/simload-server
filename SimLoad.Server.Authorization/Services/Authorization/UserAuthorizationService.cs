using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Authorization.Models;
using SimLoad.Server.Authorization.Requests;
using SimLoad.Server.Authorization.Services.Tokens;
using SimLoad.Server.Common.Encryption;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Authentication;

namespace SimLoad.Server.Authorization.Services.Authorization;

public interface IUserAuthorizationService
{
    Task<IActionResult> AuthorizeUser(AuthorizationRequest request);
    Task<IActionResult> RefreshUser(RefreshToken refreshToken);
}

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly IEncryptor _encryptor;
    private readonly RefreshTokenDbContext _refreshTokenDbContext;
    private readonly ITokenGenerationService _tokenGenerationService;
    private readonly UserDbContext _userDbContext;

    public UserAuthorizationService(RefreshTokenDbContext refreshTokenDbContext, UserDbContext userDbContext,
        IEncryptor encryptor, ITokenGenerationService tokenGenerationService)
    {
        _refreshTokenDbContext = refreshTokenDbContext;
        _userDbContext = userDbContext;
        _encryptor = encryptor;
        _tokenGenerationService = tokenGenerationService;
    }

    public async Task<IActionResult> AuthorizeUser(AuthorizationRequest request)
    {
        LoginSession session;

        try
        {
            session = _encryptor.Decrypt<LoginSession>(request.Session!);
        }
        catch (Exception)
        {
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid Session"
            });
        }

        if (session.ConfirmationCode != request.Code)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid Confirmation Code"
            });

        if (session.IsExpired)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Session Has Expired"
            });

        var user = await _userDbContext.Users.SingleOrDefaultAsync(u => u.Id == session.UserId);
        if (user is null)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid User"
            });

        var tokenResponse = _tokenGenerationService.GenerateUserTokens(user);

        var refreshToken = new RefreshToken
        {
            Id = tokenResponse.RefreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(60)
        };

        _refreshTokenDbContext.RefreshTokens.Add(refreshToken);
        await _refreshTokenDbContext.SaveChangesAsync();

        return new OkObjectResult(tokenResponse);
    }

    public async Task<IActionResult> RefreshUser(RefreshToken refreshToken)
    {
        var user = await _userDbContext.Users.SingleOrDefaultAsync(u => u.Id == refreshToken.UserId!.Value);
        if (user is null)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid User"
            });

        var newTokens = _tokenGenerationService.GenerateUserTokens(user);

        var newRefreshToken = new RefreshToken
        {
            Id = newTokens.RefreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(60)
        };

        _refreshTokenDbContext.RefreshTokens.Remove(refreshToken);
        _refreshTokenDbContext.RefreshTokens.Add(newRefreshToken);

        await _refreshTokenDbContext.SaveChangesAsync();

        return new OkObjectResult(newTokens);
    }
}