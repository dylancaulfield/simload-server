using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Options;
using SimLoad.LoadGenerator.Common;
using SimLoad.LoadGenerator.Tests;

namespace SimLoad.LoadGenerator.Lifecycle;

public interface ITestRetrievalService
{
    Task<ILoadTest?> RetrieveTest(Instruction instruction);
}

public class TestRetrievalService : ITestRetrievalService
{
    private readonly ApplicationServerOptions _applicationServerOptions;
    private readonly ICredentialsProvider _credentialsProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Guid _loadGeneratorId;

    public TestRetrievalService(IHttpClientFactory httpClientFactory,
        IClientInformationProvider clientInformationProvider,
        IOptions<ApplicationServerOptions> applicationServerOptions, ICredentialsProvider credentialsProvider)
    {
        _loadGeneratorId = clientInformationProvider.GetClientInformation().LoadGeneratorId;
        _httpClientFactory = httpClientFactory;
        _applicationServerOptions = applicationServerOptions.Value;
        _credentialsProvider = credentialsProvider;
    }

    public async Task<ILoadTest?> RetrieveTest(Instruction instruction)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"{_applicationServerOptions.BaseUrl}/api/tests/{instruction.TestId}/load-generators/{_loadGeneratorId}");

        var accessToken = await _credentialsProvider.GetBearerToken();
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        using var client = _httpClientFactory.CreateClient(nameof(TestRetrievalService));
        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<LoadTest>();
    }
}