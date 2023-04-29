using System.Diagnostics;

namespace SimLoad.LoadGenerator.Results;

public class RequestTimingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        var _ = await response.Content.ReadAsStringAsync(cancellationToken);
        response.Version = new Version((int)stopwatch.ElapsedMilliseconds, 0);
        return response;
    }
}