using System.Text;
using Amazon.Runtime;
using Amazon.SecurityToken.Model;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AWS.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using SimLoad.Common;
using SimLoad.Common.Interfaces;
using SimLoad.Common.Options;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Registrations;
using SimLoad.Server.Tests.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new ServerModule());
});

// Add services to the container.
if (true)//builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });
}

builder.Services.AddLogging(options =>
{
    if (!builder.Environment.IsDevelopment())
    {
        options.AddAWSProvider(new AWSLoggerConfig
        {
            Credentials = new BasicAWSCredentials(
                builder.Configuration["AWS:AccessKey"],
                builder.Configuration["AWS:SecretKey"]),
            Region = builder.Configuration["AWS:Region"],
            LogGroup = "SimLoadServer"
        });
    }

    options.AddConsole();
});
builder.Services.AddRouting();
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddOptions();
builder.Services.AddRequiredScopeAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddSignalR(options => { options.EnableDetailedErrors = true; })
    .AddStackExchangeRedis(builder.Configuration["Redis:ConnectionString"]);

// Auth
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.User, policy => policy.RequireRole(AuthorizationType.User.ToString()));
    options.AddPolicy(Policies.LoadGenerator,
        policy => policy.RequireRole(AuthorizationType.LoadGenerator.ToString()));
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var symmetricKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Encryption:SymmetricKey"]));

        options.IncludeErrorDetails = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = symmetricKey,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"]
        };
    });

// Configuration Options
builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("AWS"));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<PostgresOptions>(builder.Configuration.GetSection("Postgres"));
builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection("Encryption"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Configure SignalR
app.MapHub<TestsHub>(TestsHubEndpoint.Endpoint);

var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

app.Run();