using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Common;
using SimLoad.Server.Authorization.Requests;
using SimLoad.Server.Data.Contexts;

namespace SimLoad.Server.Authorization.Services.Authorization;

public interface ISimLoadAuthorizationService
{
    Task<IActionResult> Authorize(AuthorizationRequest request);
    Task<IActionResult> Refresh(RefreshAuthorizationRequest request);
}

public class SimLoadAuthorizationService : ISimLoadAuthorizationService
{
    private readonly Lazy<ILoadGeneratorAuthorizationService> _loadGeneratorAuthorizationService;
    private readonly Lazy<RefreshTokenDbContext> _refreshTokenDbContext;
    private readonly Lazy<IUserAuthorizationService> _userAuthorizationService;
    private readonly Lazy<UserDbContext> _userDbContext;

    public SimLoadAuthorizationService(
        Lazy<ILoadGeneratorAuthorizationService> loadGeneratorAuthorizationService,
        Lazy<IUserAuthorizationService> userAuthorizationService,
        Lazy<RefreshTokenDbContext> refreshTokenDbContext,
        Lazy<UserDbContext> userDbContext
    )
    {
        _loadGeneratorAuthorizationService = loadGeneratorAuthorizationService;
        _userAuthorizationService = userAuthorizationService;
        _refreshTokenDbContext = refreshTokenDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> Authorize(AuthorizationRequest request)
    {
        var response = request.AuthorizationType switch
        {
            AuthorizationType.User => await _userAuthorizationService.Value.AuthorizeUser(request),
            AuthorizationType.LoadGenerator => await _loadGeneratorAuthorizationService.Value.AuthorizeLoadGenerator(
                request)
        };

        return response;
    }

    public async Task<IActionResult> Refresh(RefreshAuthorizationRequest request)
    {
        var refreshToken =
            await _refreshTokenDbContext.Value.RefreshTokens.SingleOrDefaultAsync(t => t.Id == request.RefreshToken);
        if (refreshToken is null || refreshToken.Expires < DateTime.UtcNow)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid Refresh Token"
            });

        return refreshToken.AuthorizationType switch
        {
            AuthorizationType.User => await _userAuthorizationService.Value.RefreshUser(refreshToken),
            AuthorizationType.LoadGenerator => await _loadGeneratorAuthorizationService.Value.RefreshLoadGenerator(
                refreshToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}