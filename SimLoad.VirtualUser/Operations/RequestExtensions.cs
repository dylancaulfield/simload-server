using SimLoad.Common.Models.Scenario;

namespace SimLoad.VirtualUser.Operations;

public static class RequestExtensions
{
    public static HttpRequestMessage ToHttpRequestMessage(this Request request)
    {

        var host = request.Host.Split(":");
        
        var uriBuilder = new UriBuilder();
        uriBuilder.Scheme = request.Scheme;
        uriBuilder.Host = host[0];
        uriBuilder.Path = request.Path;
        uriBuilder.Port = host.Length > 1 ? Int16.Parse(host[1]) : uriBuilder.Port;
        uriBuilder.Query = string.Join("&", request.Query.Select(x => $"{x.Name}={x.Value}"));
        var uri = uriBuilder.Uri;

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = new HttpMethod(request.Method),
            RequestUri = uri,
            Content = new StringContent(request.Body ?? string.Empty)
        };

        foreach (var header in request.Headers)
            try
            {
                httpRequestMessage.Headers.Add(header.Name, header.Value);
            }
            catch (FormatException e)
            {
            }

        return httpRequestMessage;
    }
}