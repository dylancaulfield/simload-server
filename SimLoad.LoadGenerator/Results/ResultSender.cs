using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Options;
using SimLoad.LoadGenerator.Lifecycle;

namespace SimLoad.LoadGenerator.Results;

public interface IResultSender
{
    void StartSendingResults();
}

public class ResultSender : IResultSender
{
    private readonly ApplicationServerOptions _applicationServerOptions;
    private readonly ICancellationTokenProvider _cancellationTokenProvider;
    private readonly ChannelReader<SerializableResult> _channelReader;
    private readonly ICredentialsProvider _credentialsProvider;
    private readonly HttpClient _httpClient;
    private readonly Instruction _instruction;
    private readonly ILogger<ResultSender> _logger;
    private readonly string _url;

    public ResultSender(
        Instruction instruction,
        IOptions<ApplicationServerOptions> applicationServerOptions,
        IHttpClientFactory httpClientFactory,
        ILogger<ResultSender> logger,
        ICredentialsProvider credentialsProvider,
        ChannelReader<SerializableResult> channelReader,
        ICancellationTokenProvider cancellationTokenProvider)
    {
        _logger = logger;
        _instruction = instruction;
        _applicationServerOptions = applicationServerOptions.Value;
        _httpClient = httpClientFactory.CreateClient();
        _url = $"{_applicationServerOptions.BaseUrl}/api/tests/{_instruction.TestId}/results";
        _credentialsProvider = credentialsProvider;
        _channelReader = channelReader;
        _cancellationTokenProvider = cancellationTokenProvider;
    }

    public async void StartSendingResults()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _credentialsProvider.GetBearerToken());

        _logger.LogTrace("Beginning to send results");

        var time = DateTime.UtcNow;
        var batch = new List<SerializableResult>();

        await foreach (var result in _channelReader.ReadAllAsync())
        {
            _logger.LogTrace("Read result from channel");
            batch.Add(result);

            _cancellationTokenProvider.CancellationToken?.ThrowIfCancellationRequested();

            if (DateTime.UtcNow <= time.AddSeconds(15)) continue;

            time = DateTime.UtcNow;
            await SendResults(batch);
            batch.Clear();

            await Task.Delay(1000);
        }

        _logger.LogTrace("Stopped sending results");
    }

    private async Task SendResults(List<SerializableResult> results)
    {
        _logger.LogTrace("Posting batch of {resultCount} results", results.Count);

        var request = new BatchRequest
        {
            Results = results
        };
        var response = await _httpClient.PutAsJsonAsync(_url, request);
        if (!response.IsSuccessStatusCode) _logger.LogCritical("There was an error sending results to the server");
    }
}

internal class BatchRequest
{
    public List<SerializableResult> Results { get; set; }
}