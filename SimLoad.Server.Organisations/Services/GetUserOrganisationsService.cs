using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Organisations.Responses;

namespace SimLoad.Server.Organisations.Services;

public interface IGetUserOrganisationsService
{
    Task<IActionResult> GetUserOrganisations();
}

public class GetUserOrganisationsService : IGetUserOrganisationsService
{
    private readonly OrganisationDbContext _organisationDbContext;
    private readonly Guid? _userId;

    public GetUserOrganisationsService(OrganisationDbContext organisationDbContext, IRequestContext requestContext)
    {
        _organisationDbContext = organisationDbContext;
        _userId = requestContext.UserId;
    }

    public async Task<IActionResult> GetUserOrganisations()
    {
        var organisations = await _organisationDbContext.Organisations
            .Where(o => o.OrganisationMembers.Any(om => om.UserId == _userId))
            .Select(o => new GetUserOrganisationResponse
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description ?? string.Empty
            })
            .ToListAsync();

        return new OkObjectResult(organisations);
    }
}