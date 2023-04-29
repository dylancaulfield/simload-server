using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Requests;

namespace SimLoad.Server.Organisations.Services;

public interface IAddMemberToOrganisationService
{
    Task<IActionResult> AddMemberToOrganisation(AddMemberToOrganisationRequest request);
}

public class AddMemberToOrganisationService : IAddMemberToOrganisationService
{
    private readonly OrganisationDbContext _organisationDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    private readonly UserDbContext _userDbContext;

    public AddMemberToOrganisationService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        UserDbContext userDbContext, OrganisationDbContext organisationDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _userDbContext = userDbContext;
        _organisationDbContext = organisationDbContext;
    }

    public async Task<IActionResult> AddMemberToOrganisation(AddMemberToOrganisationRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.OrganisationAddMember,
            async (org, member) =>
            {
                var user = await _userDbContext.Users.SingleOrDefaultAsync(
                    u => u.EmailAddresses.Any(
                        e => e.EmailAddress == request.EmailAddress));
                if (user is null)
                    return new BadRequestObjectResult(
                        new ErrorResponse { Message = "User not found" }
                    );

                var organisation = await _organisationDbContext.Organisations
                    .Include(o => o.OrganisationMembers)
                    .SingleAsync(o => o.Id == org.Id);
                if (organisation.OrganisationMembers.Any(om => om.UserId == user.Id))
                    return new BadRequestObjectResult(
                        new ErrorResponse { Message = "User is already a member of this organisation" }
                    );

                var organisationMember = new OrganisationMember
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisation.Id,
                    Organisation = organisation,
                    UserId = user.Id
                };

                var permissions = new OrganisationPermissions
                {
                    Id = Guid.NewGuid(),
                    OrganisationMember = organisationMember,
                    OrganisationMemberId = organisationMember.Id
                };

                organisationMember.OrganisationPermissions = permissions;
                _organisationDbContext.OrganisationMembers.Add(organisationMember);

                await _organisationDbContext.SaveChangesAsync();

                return new OkObjectResult(organisationMember.Id);
            });
    }
}