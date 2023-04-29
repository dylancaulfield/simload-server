using Amazon;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Options;
using SimLoad.Common.Options;

namespace SimLoad.Server.Common.SimpleEmailService;

public interface ISimpleEmailServiceFactory
{
    IAmazonSimpleEmailService GetSimpleEmailService();
}

public class SimpleEmailServiceFactory : ISimpleEmailServiceFactory
{
    private readonly AmazonSimpleEmailServiceClient _client;

    public SimpleEmailServiceFactory(IOptions<AwsOptions> awsOptions)
    {
        _client = new AmazonSimpleEmailServiceClient(awsOptions.Value.AccessKey, awsOptions.Value.SecretKey,
            RegionEndpoint.EUWest1);
    }

    public IAmazonSimpleEmailService GetSimpleEmailService()
    {
        return _client;
    }
}