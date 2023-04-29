using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;

namespace SimLoad.Server.Organisations.Permissions;

public class
    OrganisationPermissionEvaluator : IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
{
    private readonly OrganisationDbContext _organisationDbContext;
    private readonly Guid? _userId;

    public OrganisationPermissionEvaluator(OrganisationDbContext organisationDbContext, IRequestContext requestContext)
    {
        _organisationDbContext = organisationDbContext;
        _userId = requestContext.UserId;
    }

    public async Task<IActionResult> Evaluate(Guid entityId, Func<OrganisationPermissions, bool> hasPermission,
        Func<Organisation, OrganisationMember, Task<IActionResult>> onPermissionAllowed)
    {
        var organisation = await _organisationDbContext.Organisations.FindAsync(entityId);
        if (organisation is null) return new NotFoundResult();

        var organisationPermissions = await _organisationDbContext.OrganisationPermissions
            .Include(op => op.OrganisationMember)
            .SingleOrDefaultAsync(pm => pm.OrganisationMember.UserId == _userId
                                        && pm.OrganisationMember.OrganisationId == entityId);
        if (organisationPermissions is null || !hasPermission(organisationPermissions)) return new ForbidResult();

        return await onPermissionAllowed(organisation, organisationPermissions.OrganisationMember);
    }
}