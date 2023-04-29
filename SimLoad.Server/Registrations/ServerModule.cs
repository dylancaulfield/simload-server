using System.IdentityModel.Tokens.Jwt;
using Autofac;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimLoad.Common.Models;
using SimLoad.Common.Options;
using SimLoad.Server.Authorization.Services;
using SimLoad.Server.Authorization.Services.Authorization;
using SimLoad.Server.Authorization.Services.Tokens;
using SimLoad.Server.Common;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Common.Encryption;
using SimLoad.Server.Common.SimpleEmailService;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Data.Entities.Scenario;
using SimLoad.Server.Data.Registrations;
using SimLoad.Server.Organisations.Registrations;
using SimLoad.Server.Projects.Permissions;
using SimLoad.Server.Projects.Services;
using SimLoad.Server.Results.Queries;
using SimLoad.Server.Results.Services;
using SimLoad.Server.Scenarios.Permissions;
using SimLoad.Server.Scenarios.Services;
using SimLoad.Server.Tests.Services;
using SimLoad.Server.Users.Services;
using Scenario = SimLoad.Common.Models.Scenario.Scenario;

namespace SimLoad.Server.Registrations;

public class ServerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // SimLoad.Server.Common
        builder.RegisterType<RequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
        builder.Register(c =>
        {
            var mongoOptions = c.Resolve<IOptions<MongoOptions>>().Value;
            var mongoSettings = MongoClientSettings.FromConnectionString(mongoOptions.ConnectionString);
            mongoSettings.LinqProvider = LinqProvider.V3;
            var client = new MongoClient(mongoSettings);
            return client.GetDatabase(mongoOptions.Database);
        }).SingleInstance();
        builder.Register(c =>
        {
            var database = c.Resolve<IMongoDatabase>();
            return database.GetCollection<Scenario>("scenarios");
        });
        builder.Register(c =>
        {
            var database = c.Resolve<IMongoDatabase>();
            return database.GetCollection<Result>("results");
        });
        builder.RegisterType<SimpleEmailServiceFactory>().As<ISimpleEmailServiceFactory>().SingleInstance();
        builder.Register(c =>
        {
            var simpleEmailServiceFactory = c.Resolve<ISimpleEmailServiceFactory>();
            return simpleEmailServiceFactory.GetSimpleEmailService();
        }).SingleInstance();
        builder.RegisterType<Encryptor>().As<IEncryptor>();
        builder.RegisterType<SigningCredentialsProvider>().As<ISigningCredentialsProvider>().SingleInstance();
        builder.RegisterInstance(new JwtSecurityTokenHandler()).SingleInstance();
        builder.RegisterType<RequestWrapper>().As<IRequestWrapper>();

        // SimLoad.Server.Authorization
        builder.RegisterType<CreateLoginSessionService>().As<ICreateLoginSessionService>();
        builder.RegisterType<SendVerificationCodeService>().As<ISendVerificationCodeService>();
        builder.RegisterType<SimLoadAuthorizationService>().As<ISimLoadAuthorizationService>();
        builder.RegisterType<UserAuthorizationService>().As<IUserAuthorizationService>();
        builder.RegisterType<LoadGeneratorAuthorizationService>().As<ILoadGeneratorAuthorizationService>();
        builder.RegisterType<TokenGenerationService>().As<ITokenGenerationService>();
        builder.RegisterType<UserTokenGenerationService>().As<IUserTokenGenerationService>();
        builder.RegisterType<LoadGeneratorTokenGenerationService>().As<ILoadGeneratorTokenGenerationService>();

        // SimLoad.Server.Users
        builder.RegisterType<CreateUserService>().As<ICreateUserService>();
        builder.RegisterType<GetUserService>().As<IGetUserService>();

        // SimLoad.Server.Results
        builder.RegisterType<OverallStatisticsQuery>().As<IOverallStatisticsQuery>();
        builder.RegisterType<ScenarioStatisticsQuery>().As<IScenarioStatisticsQuery>();
        builder.RegisterType<OperationStatisticsQuery>().As<IOperationStatisticsQuery>();
        builder.RegisterType<IntervalStatisticsQuery>().As<IIntervalStatisticsQuery>();
        builder.RegisterType<LiveResultsService>().As<ILiveResultsService>();
        builder.RegisterType<FinalResultsService>().As<IFinalResultsService>();
        builder.RegisterType<ResultBatchUploadService>().As<IResultBatchUploadService>();
        builder.RegisterType<TargetUserCountCalculator>().As<ITargetUserCountCalculator>();

        // SimLoad.Server.Organisations
        builder.RegisterModule(new OrganisationModule());


        // SimLoad.Server.Projects
        builder.RegisterType<CreateProjectService>().As<ICreateProjectService>();
        builder.RegisterType<GetUserProjectsService>().As<IGetUserProjectsService>();
        builder.RegisterType<GetProjectByIdService>().As<IGetProjectByIdService>();
        builder.RegisterType<ProjectPermissionEvaluator>()
            .As<IPermissionEvaluator<Project, ProjectMember, ProjectPermissions>>();

        // SimLoad.Server.Scenarios
        builder.RegisterType<CreateScenarioService>().As<ICreateScenarioService>();
        builder.RegisterType<DeleteScenarioService>().As<IDeleteScenarioService>();
        builder.RegisterType<GetScenarioService>().As<IGetScenarioService>();
        builder.RegisterType<GetScenariosService>().As<IGetScenariosService>();
        builder.RegisterType<UpdateScenarioService>().As<IUpdateScenarioService>();
        builder.RegisterType<ScenarioPermissionEvaluator>()
            .As<IPermissionEvaluator<Data.Entities.Scenario.Scenario, ScenarioMember, ScenarioPermissions>>();

        // SimLoad.Server.Tests
        builder.RegisterType<StartTestService>().As<IStartTestService>();
        builder.RegisterType<GetTestService>().As<IGetTestService>();
        builder.RegisterType<GetTestsService>().As<IGetTestsService>();
        builder.RegisterType<GetTestOptionsService>().As<IGetTestOptionsService>();
        builder.RegisterType<UpdateLoadGeneratorConnectionService>().As<IUpdateLoadGeneratorConnectionService>();
        builder.RegisterType<GetTestMetaDataService>().As<IGetTestMetaDataService>();

        // SimLoad.Server.Data
        builder.RegisterModule(new DataModule());
    }
}