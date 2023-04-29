using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Requests;

namespace SimLoad.Server.Organisations.Services;

public interface IDeleteLoadGeneratorCredentialService
{
    Task<IActionResult> DeleteLoadGeneratorCredential(DeleteLoadGeneratorCredentialRequest request);
}

public class DeleteLoadGeneratorCredentialService : IDeleteLoadGeneratorCredentialService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    public DeleteLoadGeneratorCredentialService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        LoadGeneratorDbContext loadGeneratorDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _loadGeneratorDbContext = loadGeneratorDbContext;
    }

    public async Task<IActionResult> DeleteLoadGeneratorCredential(DeleteLoadGeneratorCredentialRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.LoadGeneratorCredentialDelete,
            async (_, _) =>
            {
                var loadGeneratorCredential =
                    await _loadGeneratorDbContext.LoadGeneratorCredentials.SingleOrDefaultAsync(lgc =>
                        lgc.OrganisationId == request.OrganisationId && lgc.Id == request.LoadGeneratorCredentialId);
                if (loadGeneratorCredential is null) return new NotFoundResult();

                _loadGeneratorDbContext.Remove(loadGeneratorCredential);
                await _loadGeneratorDbContext.SaveChangesAsync();

                return new OkResult();
            });
    }
}