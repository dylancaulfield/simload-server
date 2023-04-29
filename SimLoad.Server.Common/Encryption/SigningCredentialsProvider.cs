using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimLoad.Common.Options;

namespace SimLoad.Server.Common.Encryption;

public interface ISigningCredentialsProvider
{
    SigningCredentials Credentials { get; }
}

public class SigningCredentialsProvider : ISigningCredentialsProvider
{
    private readonly EncryptionOptions _encryptionOptions;

    public SigningCredentialsProvider(IOptions<EncryptionOptions> encryptionOptions)
    {
        _encryptionOptions = encryptionOptions.Value;
    }

    public SigningCredentials Credentials => new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_encryptionOptions.SymmetricKey)),
        SecurityAlgorithms.HmacSha512);
}