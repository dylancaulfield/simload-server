using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Models.Scenario;
using SimLoad.VirtualUser.Contexts;

namespace SimLoad.VirtualUser.Operations;

public interface IHttpRequestSender
{
    Task<VirtualUserResponse> SendRequest(Operation operation, Guid operationId);
}

public class HttpRequestSender : IHttpRequestSender
{
    private readonly ICancellationTokenProvider _cancellationTokenProvider;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpRequestSender> _logger;
    private readonly ChannelWriter<SerializableResult> _resultChannelWriter;
    private readonly IResultFactory _resultFactory;
    private readonly VirtualUserId _virtualUserId;

    public HttpRequestSender(
        IResultFactory resultFactory,
        ILogger<HttpRequestSender> logger,
        VirtualUserId virtualUserId,
        HttpClient httpClient,
        ChannelWriter<SerializableResult> resultChannelWriter,
        ICancellationTokenProvider cancellationTokenProvider
    )
    {
        _virtualUserId = virtualUserId;
        _resultFactory = resultFactory;
        _logger = logger;
        _httpClient = httpClient;
        _resultChannelWriter = resultChannelWriter;
        _cancellationTokenProvider = cancellationTokenProvider;
    }

    public async Task<VirtualUserResponse> SendRequest(Operation operation, Guid operationId)
    {
        var httpRequest = operation.Request.ToHttpRequestMessage();

        _cancellationTokenProvider.CancellationToken?.ThrowIfCancellationRequested();

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest);

            // Hijacked version to hold response time in RequestTimingHandler
            var responseTime = response.Version.Major;

            var result = _resultFactory.CreateHttpResult(httpRequest, operationId, _virtualUserId.Value,
                (int)response.StatusCode, responseTime);

            await _resultChannelWriter.WriteAsync(result);
            _logger.LogTrace("Result written to channel");

            var headers = (from header in response.Headers
                from value in header.Value
                select new VirtualUserResponseHeader { Name = header.Key, Value = value }).ToList();

            var virtualUserResponse = new VirtualUserResponse
            {
                HttpContent = response.Content,
                StatusCode = (int)response.StatusCode,
                Headers = headers
            };

            return virtualUserResponse;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error occurred while sending HTTP request: {exception}", exception.Message);
            return new VirtualUserResponse
            {
                StatusCode = 500,
                Headers = new List<VirtualUserResponseHeader>()
            };
        }
    }
}