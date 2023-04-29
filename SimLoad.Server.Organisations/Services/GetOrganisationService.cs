using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Responses;

namespace SimLoad.Server.Organisations.Services;

public interface IGetOrganisationService
{
    Task<IActionResult> GetOrganisation(Guid organisationId);
}

public class GetOrganisationService : IGetOrganisationService
{
    private readonly OrganisationDbContext _organisationDbContext;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    public GetOrganisationService(
        IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator,
        OrganisationDbContext organisationDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _organisationDbContext = organisationDbContext;
    }

    public async Task<IActionResult> GetOrganisation(Guid organisationId)
    {
        return await _permissionEvaluator.Evaluate(organisationId, p => true, async (_, _) =>
        {
            var organisation = await _organisationDbContext.Organisations
                .Include(o => o.OrganisationMembers)
                .Include(o => o.LoadGeneratorCredentials)
                .Include(o => o.Projects)
                .Where(o => o.Id == organisationId)
                .Select(o => new GetOrganisationResponse
                {
                    Name = o.Name,
                    Description = o.Description ?? string.Empty,
                    Members = o.OrganisationMembers.Select(
                        om => new GetOrganisationMemberResponse
                        {
                            Id = om.Id,
                            DisplayName = om.User.DisplayName
                        }).ToList(),
                    Projects = o.Projects.Select(
                        p => new GetOrganisationProjectResponse
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description
                        }).ToList(),
                    LoadGeneratorCredentials = o.LoadGeneratorCredentials.Select(
                        lg => new GetOrganisationLoadGeneratorCredentialsResponse
                        {
                            Id = lg.Id,
                            Name = lg.Name
                        }).ToList()
                })
                .SingleOrDefaultAsync();

            return new OkObjectResult(organisation);
        });
    }
}