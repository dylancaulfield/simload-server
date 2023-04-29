using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using SimLoad.Common;
using SimLoad.Common.Options;
using SimLoad.Server.Common.Encryption;
using SimLoad.Server.Data.Entities.User;

namespace SimLoad.Server.Authorization.Services.Tokens;

public interface IUserTokenGenerationService
{
    string GenerateUserToken(User user);
}

public class UserTokenGenerationService : IUserTokenGenerationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly JwtSecurityTokenHandler _securityTokenHandler;
    private readonly ISigningCredentialsProvider _signingCredentialsProvider;

    public UserTokenGenerationService(
        ISigningCredentialsProvider signingCredentialsProvider,
        JwtSecurityTokenHandler securityTokenHandler,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _signingCredentialsProvider = signingCredentialsProvider;
        _securityTokenHandler = securityTokenHandler;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateUserToken(User user)
    {
        var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new(ClaimTypes.Role, AuthorizationType.User.ToString())
        };

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, DateTime.UtcNow,
            DateTime.UtcNow.AddHours(_jwtOptions.HoursUntilExpiry), _signingCredentialsProvider.Credentials);

        return _securityTokenHandler.WriteToken(jwt);
    }
}