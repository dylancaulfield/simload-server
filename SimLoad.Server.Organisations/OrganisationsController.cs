using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Organisations.Requests;
using SimLoad.Server.Organisations.Services;

namespace SimLoad.Server.Organisations;

[ApiController]
[Route("/api/organisations")]
[Authorize(Policy = Policies.User)]
public class OrganisationsController : ControllerBase
{
    private readonly Lazy<ICreateOrganisationService> _createOrganisationService;
    private readonly Lazy<IGetOrganisationService> _getOrganisationService;
    private readonly Lazy<IGetUserOrganisationsService> _getUserOrganisationsService;

    public OrganisationsController(
        Lazy<ICreateOrganisationService> createOrganisationService,
        Lazy<IGetUserOrganisationsService> getUserOrganisationsService,
        Lazy<IGetOrganisationService> getOrganisationService
    )
    {
        _createOrganisationService = createOrganisationService;
        _getUserOrganisationsService = getUserOrganisationsService;
        _getOrganisationService = getOrganisationService;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationRequest request)
    {
        return await _createOrganisationService.Value.CreateOrganisation(request);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetUserOrganisations()
    {
        return await _getUserOrganisationsService.Value.GetUserOrganisations();
    }

    [HttpGet]
    [Route("{organisationId:guid}")]
    public async Task<IActionResult> GetOrganisation([FromRoute] Guid organisationId)
    {
        return await _getOrganisationService.Value.GetOrganisation(organisationId);
    }
}