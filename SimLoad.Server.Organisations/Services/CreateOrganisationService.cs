using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Organisations.Requests;

namespace SimLoad.Server.Organisations.Services;

public interface ICreateOrganisationService
{
    Task<IActionResult> CreateOrganisation(CreateOrganisationRequest request);
}

public class CreateOrganisationService : ICreateOrganisationService
{
    private readonly OrganisationDbContext _organisationDbContext;
    private readonly Guid? userId;

    public CreateOrganisationService(OrganisationDbContext organisationDbContext, IRequestContext requestContext)
    {
        _organisationDbContext = organisationDbContext;
        userId = requestContext.UserId;
    }

    public async Task<IActionResult> CreateOrganisation(CreateOrganisationRequest request)
    {
        var organisation = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        var member = new OrganisationMember
        {
            Id = Guid.NewGuid(),
            OrganisationId = organisation.Id,
            Organisation = organisation,
            UserId = userId!.Value
        };

        var organisationPermission = new OrganisationPermissions
        {
            Id = Guid.NewGuid(),
            OrganisationMember = member,
            OrganisationMemberId = member.Id,
            OrganisationDelete = true,
            OrganisationEdit = true,
            OrganisationAddMember = true,
            OrganisationChangePermission = true,
            OrganisationCreateProject = true,
            OrganisationRemoveMember = true,
            LoadGeneratorCredentialCreate = true,
            LoadGeneratorCredentialDelete = true,
            TestRun = true
        };

        member.OrganisationPermissions = organisationPermission;
        organisation.OrganisationMembers = new List<OrganisationMember> { member };

        _organisationDbContext.Add(organisation);
        await _organisationDbContext.SaveChangesAsync();

        return new OkObjectResult(organisation.Id);
    }
}