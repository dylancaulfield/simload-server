using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;

namespace SimLoad.LoadGenerator.Lifecycle;

public interface ICredentialsProvider
{
    Task<string> GetBearerToken();
}

public class CredentialsProvider : ICredentialsProvider
{
    private readonly ApplicationServerOptions _applicationServerOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CredentialsProvider> _logger;

    private string? _accessToken;
    private DateTime _accessTokenExpiry = DateTime.MinValue;
    private Guid? _refreshToken;

    public CredentialsProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<ApplicationServerOptions> applicationServerOptions,
        ILogger<CredentialsProvider> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _applicationServerOptions = applicationServerOptions.Value;
        _logger = logger;
    }

    public async Task<string> GetBearerToken()
    {
        var shouldRefresh = false;
        shouldRefresh |= _accessToken is null;
        shouldRefresh |= DateTime.UtcNow.AddSeconds(20) > _accessTokenExpiry;

        if (shouldRefresh) await RefreshTokens();

        return _accessToken!;
    }

    private async Task RefreshTokens()
    {
        _logger.LogTrace("Refreshing tokens");

        var tokenResponse = new TokenResponse();
        using var client = _httpClientFactory.CreateClient(nameof(CredentialsProvider));

        if (_refreshToken is null)
        {
            var request = CreateApiKeyRequest();
            var response =
                await client.PostAsJsonAsync($"{_applicationServerOptions.BaseUrl}/api/auth/token", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Authentication failed with the provided API_KEY");
                Environment.Exit(1);
            }

            tokenResponse = (await response.Content.ReadFromJsonAsync<TokenResponse>())!;
        }

        if (_refreshToken is not null)
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = _refreshToken.ToString()!
            };
            var response =
                await client.PostAsJsonAsync($"{_applicationServerOptions.BaseUrl}/api/auth/refresh", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Authentication failed. Did you revoke credentials?");
                Environment.Exit(1);
            }


            tokenResponse = (await response.Content.ReadFromJsonAsync<TokenResponse>())!;
        }

        _accessToken = tokenResponse.AccessToken;
        _accessTokenExpiry = tokenResponse.Expires;
        _refreshToken = tokenResponse.RefreshToken;
    }

    private ApiKeyRequest CreateApiKeyRequest()
    {
        var apiKey = Environment.GetEnvironmentVariable("API_KEY");

        if (apiKey is null)
        {
            _logger.LogError("API_KEY missing from environment variables");
            Environment.Exit(1);
        }

        return new ApiKeyRequest
        {
            ApiKey = apiKey
        };
    }
}

internal class ApiKeyRequest
{
    public string ApiKey { get; set; }
}

internal class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}

internal class TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime Expires { get; set; }
    public Guid RefreshToken { get; set; }
}