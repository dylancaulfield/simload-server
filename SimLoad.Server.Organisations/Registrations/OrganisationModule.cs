using Autofac;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Permissions;
using SimLoad.Server.Organisations.Services;

namespace SimLoad.Server.Organisations.Registrations;

public class OrganisationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CreateOrganisationService>().As<ICreateOrganisationService>();
        builder.RegisterType<GetUserOrganisationsService>().As<IGetUserOrganisationsService>();
        builder.RegisterType<GetOrganisationService>().As<IGetOrganisationService>();

        builder.RegisterType<AddMemberToOrganisationService>().As<IAddMemberToOrganisationService>();
        builder.RegisterType<RemoveMemberFromOrganisationService>().As<IRemoveMemberFromOrganisationService>();

        builder.RegisterType<CreateLoadGeneratorCredentialService>().As<ICreateLoadGeneratorCredentialService>();
        builder.RegisterType<DeleteLoadGeneratorCredentialService>().As<IDeleteLoadGeneratorCredentialService>();
        builder.RegisterType<GetLoadGeneratorCredentialsService>().As<IGetLoadGeneratorCredentialsService>();

        builder.RegisterType<OrganisationPermissionEvaluator>()
            .As<IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>>();
    }
}