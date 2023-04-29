using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimLoad.Common.Options;
using SimLoad.LoadGenerator.Common;
using SimLoad.LoadGenerator.Registrations;
using SimLoad.LoadGenerator.Results;

namespace SimLoad.LoadGenerator;

public static class Program
{
    private static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile("appsettings.json", false, true);
                if (hostBuilderContext.HostingEnvironment.IsDevelopment())
                    configurationBuilder.AddJsonFile("appsettings.Development.json", true, true);
                configurationBuilder.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<LoadGenerator>();
                services.AddScoped<IClientInformationProvider, ClientInformationProvider>();
                services.AddOptions();
                services
                    .AddHttpClient("vuClient")
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var clientHandler = new HttpClientHandler();
                        clientHandler.CookieContainer = new CookieContainer();
                        clientHandler.UseCookies = true;
                        clientHandler.AllowAutoRedirect = false;
                        return clientHandler;
                    })
                    .AddHttpMessageHandler<RequestTimingHandler>();
                services.Configure<ApplicationServerOptions>(hostContext.Configuration.GetSection("ApplicationServer"));
            })
            .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
            {
                // loggingBuilder.AddJsonConsole(jsonOptions =>
                // {
                //     jsonOptions.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ";
                //     jsonOptions.IncludeScopes = false;
                //     jsonOptions.JsonWriterOptions = new JsonWriterOptions
                //     {
                //         Indented = true
                //     };
                //     jsonOptions.UseUtcTimestamp = true;
                // });
                loggingBuilder.AddConsole(options =>
                {
                    options.TimestampFormat = "dd/MM/yy @ HH:mm:ss ";
                    options.IncludeScopes = false;
                    options.UseUtcTimestamp = true;
                });
                loggingBuilder.AddConfiguration(hostBuilderContext.Configuration.GetSection("Logging"));
            })
            .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                containerBuilder.RegisterModule<LoadGeneratorModule>())
            .Build()
            .RunAsync();
    }
}