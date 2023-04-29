using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using SimLoad.Common;
using SimLoad.Common.Options;
using SimLoad.Server.Common.Encryption;
using SimLoad.Server.Data.Entities.LoadGenerator;

namespace SimLoad.Server.Authorization.Services.Tokens;

public interface ILoadGeneratorTokenGenerationService
{
    string GenerateLoadGeneratorToken(LoadGeneratorCredential loadGeneratorCredential);
}

public class LoadGeneratorTokenGenerationService : ILoadGeneratorTokenGenerationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly JwtSecurityTokenHandler _securityTokenHandler;

    private readonly ISigningCredentialsProvider _signingCredentialsProvider;

    public LoadGeneratorTokenGenerationService(
        ISigningCredentialsProvider signingCredentialsProvider,
        JwtSecurityTokenHandler securityTokenHandler,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _signingCredentialsProvider = signingCredentialsProvider;
        _securityTokenHandler = securityTokenHandler;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateLoadGeneratorToken(LoadGeneratorCredential loadGeneratorCredential)
    {
        var claims = new List<Claim>
        {
            new("organisationId", loadGeneratorCredential.OrganisationId.ToString()),
            new("loadGeneratorCredentialId", loadGeneratorCredential.Id.ToString()),
            new(ClaimTypes.Role, AuthorizationType.LoadGenerator.ToString())
        };

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, DateTime.UtcNow,
            DateTime.UtcNow.AddHours(_jwtOptions.HoursUntilExpiry), _signingCredentialsProvider.Credentials);

        return _securityTokenHandler.WriteToken(jwt);
    }
}