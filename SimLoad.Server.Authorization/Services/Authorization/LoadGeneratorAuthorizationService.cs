using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Authorization.Requests;
using SimLoad.Server.Authorization.Services.Tokens;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Authentication;

namespace SimLoad.Server.Authorization.Services.Authorization;

public interface ILoadGeneratorAuthorizationService
{
    Task<IActionResult> AuthorizeLoadGenerator(AuthorizationRequest request);
    Task<IActionResult> RefreshLoadGenerator(RefreshToken refreshToken);
}

public class LoadGeneratorAuthorizationService : ILoadGeneratorAuthorizationService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;
    private readonly RefreshTokenDbContext _refreshTokenDbContext;
    private readonly ITokenGenerationService _tokenGenerationService;

    public LoadGeneratorAuthorizationService(
        LoadGeneratorDbContext loadGeneratorDbContext,
        RefreshTokenDbContext refreshTokenDbContext,
        ITokenGenerationService tokenGenerationService
    )
    {
        _loadGeneratorDbContext = loadGeneratorDbContext;
        _refreshTokenDbContext = refreshTokenDbContext;
        _tokenGenerationService = tokenGenerationService;
    }

    public async Task<IActionResult> AuthorizeLoadGenerator(AuthorizationRequest request)
    {
        var credential = await _loadGeneratorDbContext.LoadGeneratorCredentials
            .SingleOrDefaultAsync(c => c.ApiKey == request.ApiKey);
        if (credential is null)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid API Key"
            });

        var tokens = _tokenGenerationService.GenerateLoadGeneratorTokens(credential);

        var refreshToken = new RefreshToken
        {
            Id = tokens.RefreshToken,
            LoadGeneratorCredentialId = credential.Id,
            Expires = DateTime.UtcNow.AddDays(365)
        };

        _refreshTokenDbContext.RefreshTokens.Add(refreshToken);
        await _refreshTokenDbContext.SaveChangesAsync();

        return new OkObjectResult(tokens);
    }

    public async Task<IActionResult> RefreshLoadGenerator(RefreshToken refreshToken)
    {
        var credential = await _loadGeneratorDbContext.LoadGeneratorCredentials
            .SingleOrDefaultAsync(c => c.Id == refreshToken.LoadGeneratorCredentialId);
        if (credential is null)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Invalid Load Generator"
            });

        var newTokens = _tokenGenerationService.GenerateLoadGeneratorTokens(credential);

        var newRefreshToken = new RefreshToken
        {
            Id = newTokens.RefreshToken,
            LoadGeneratorCredentialId = credential.Id,
            Expires = DateTime.UtcNow.AddDays(365)
        };

        _refreshTokenDbContext.RefreshTokens.Remove(refreshToken);
        _refreshTokenDbContext.RefreshTokens.Add(newRefreshToken);

        await _refreshTokenDbContext.SaveChangesAsync();

        return new OkObjectResult(newTokens);
    }
}