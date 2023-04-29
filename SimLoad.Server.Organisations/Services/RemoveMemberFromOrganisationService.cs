using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Requests;

namespace SimLoad.Server.Organisations.Services;

public interface IRemoveMemberFromOrganisationService
{
    Task<IActionResult> RemoveMemberFromOrganisation(RemoveMemberFromOrganisationRequest request);
}

public class RemoveMemberFromOrganisationService : IRemoveMemberFromOrganisationService
{
    private readonly OrganisationDbContext _organisationDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    public RemoveMemberFromOrganisationService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        OrganisationDbContext organisationDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _organisationDbContext = organisationDbContext;
    }

    public async Task<IActionResult> RemoveMemberFromOrganisation(RemoveMemberFromOrganisationRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.OrganisationRemoveMember,
            async (_, _) =>
            {
                var organisationMember = await _organisationDbContext.OrganisationMembers
                    .SingleOrDefaultAsync(om => om.Id == request.OrganisationMemberId);
                if (organisationMember is null)
                    return new BadRequestObjectResult(
                        new ErrorResponse { Message = "Organisation member not found" }
                    );

                _organisationDbContext.OrganisationMembers.Remove(organisationMember);
                await _organisationDbContext.SaveChangesAsync();

                return new OkResult();
            });
    }
}