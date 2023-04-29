using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Responses;

namespace SimLoad.Server.Organisations.Services;

public interface IGetLoadGeneratorCredentialsService
{
    Task<IActionResult> GetLoadGeneratorCredentials(Guid organisationId);
}

public class GetLoadGeneratorCredentialsService : IGetLoadGeneratorCredentialsService
{
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    public GetLoadGeneratorCredentialsService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        LoadGeneratorDbContext loadGeneratorDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _loadGeneratorDbContext = loadGeneratorDbContext;
    }

    public async Task<IActionResult> GetLoadGeneratorCredentials(Guid organisationId)
    {
        return await _permissionEvaluator.Evaluate(organisationId,
            p => p.LoadGeneratorCredentialCreate || p.LoadGeneratorCredentialDelete,
            async (_, _) =>
            {
                var credentials = await _loadGeneratorDbContext.LoadGeneratorCredentials
                    .Where(lgc => lgc.OrganisationId == organisationId)
                    .Select(lgc => new GetLoadGeneratorCredentialsResponse
                    {
                        Id = lgc.Id,
                        Name = lgc.Name,
                        ApiKey = lgc.ApiKey
                    })
                    .ToListAsync();

                return new OkObjectResult(credentials);
            });
    }
}