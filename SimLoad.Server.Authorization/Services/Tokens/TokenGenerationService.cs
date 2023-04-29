using Microsoft.Extensions.Options;
using SimLoad.Common.Options;
using SimLoad.Server.Authorization.Responses;
using SimLoad.Server.Data.Entities.LoadGenerator;
using SimLoad.Server.Data.Entities.User;

namespace SimLoad.Server.Authorization.Services.Tokens;

public interface ITokenGenerationService
{
    AuthorizationResponse GenerateUserTokens(User user);
    AuthorizationResponse GenerateLoadGeneratorTokens(LoadGeneratorCredential loadGeneratorCredential);
}

public class TokenGenerationService : ITokenGenerationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILoadGeneratorTokenGenerationService _loadGeneratorTokenGenerationService;
    private readonly IUserTokenGenerationService _userTokenGenerationService;

    public TokenGenerationService(
        IUserTokenGenerationService userTokenGenerationService,
        ILoadGeneratorTokenGenerationService loadGeneratorTokenGenerationService,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _userTokenGenerationService = userTokenGenerationService;
        _loadGeneratorTokenGenerationService = loadGeneratorTokenGenerationService;
        _jwtOptions = jwtOptions.Value;
    }

    public AuthorizationResponse GenerateUserTokens(User user)
    {
        return new AuthorizationResponse
        {
            AccessToken = _userTokenGenerationService.GenerateUserToken(user),
            Expires = DateTime.UtcNow.AddHours(_jwtOptions.HoursUntilExpiry),
            RefreshToken = Guid.NewGuid()
        };
    }

    public AuthorizationResponse GenerateLoadGeneratorTokens(LoadGeneratorCredential loadGeneratorCredential)
    {
        return new AuthorizationResponse
        {
            AccessToken = _loadGeneratorTokenGenerationService.GenerateLoadGeneratorToken(loadGeneratorCredential),
            Expires = DateTime.UtcNow.AddHours(_jwtOptions.HoursUntilExpiry),
            RefreshToken = Guid.NewGuid()
        };
    }
}