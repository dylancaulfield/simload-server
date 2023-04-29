using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.LoadGenerator;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Requests;

namespace SimLoad.Server.Organisations.Services;

public interface ICreateLoadGeneratorCredentialService
{
    Task<IActionResult> CreateLoadGeneratorCredential(CreateLoadGeneratorCredentialRequest request);
}

public class CreateLoadGeneratorCredentialService : ICreateLoadGeneratorCredentialService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    public CreateLoadGeneratorCredentialService(LoadGeneratorDbContext loadGeneratorDbContext,
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator)
    {
        _loadGeneratorDbContext = loadGeneratorDbContext;
        _permissionEvaluator = permissionEvaluator;
    }

    public async Task<IActionResult> CreateLoadGeneratorCredential(CreateLoadGeneratorCredentialRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.LoadGeneratorCredentialCreate,
            async (organisation, _) =>
            {
                var credential = new LoadGeneratorCredential
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    OrganisationId = organisation.Id,
                    ApiKey = Guid.NewGuid()
                };

                _loadGeneratorDbContext.LoadGeneratorCredentials.Add(credential);

                await _loadGeneratorDbContext.SaveChangesAsync();

                return new OkObjectResult(credential.Id);
            });
    }
}