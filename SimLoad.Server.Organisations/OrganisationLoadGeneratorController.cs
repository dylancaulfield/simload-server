using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Organisations.Requests;
using SimLoad.Server.Organisations.Services;
using SimLoad.Server.Organisations.Submissions;

namespace SimLoad.Server.Organisations;

[ApiController]
[Route("/api/organisations/{organisationId:guid}/load-generators")]
public class OrganisationLoadGeneratorController
{
    private readonly Lazy<ICreateLoadGeneratorCredentialService> _createLoadGeneratorCredentialService;
    private readonly Lazy<IDeleteLoadGeneratorCredentialService> _deleteLoadGeneratorCredentialService;

    private readonly Lazy<IGetLoadGeneratorCredentialsService> _getLoadGeneratorCredentialsService;

    public OrganisationLoadGeneratorController(
        Lazy<IGetLoadGeneratorCredentialsService> getLoadGeneratorCredentialsService,
        Lazy<ICreateLoadGeneratorCredentialService> createLoadGeneratorCredentialService,
        Lazy<IDeleteLoadGeneratorCredentialService> deleteLoadGeneratorCredentialService)
    {
        _getLoadGeneratorCredentialsService = getLoadGeneratorCredentialsService;
        _createLoadGeneratorCredentialService = createLoadGeneratorCredentialService;
        _deleteLoadGeneratorCredentialService = deleteLoadGeneratorCredentialService;
    }

    [HttpGet]
    [Route("credentials")]
    [Authorize(Policy = Policies.User)]
    public async Task<IActionResult> GetLoadGeneratorCredentials([FromRoute] Guid organisationId)
    {
        return await _getLoadGeneratorCredentialsService.Value.GetLoadGeneratorCredentials(organisationId);
    }

    [HttpPost]
    [Route("credentials")]
    [Authorize(Policy = Policies.User)]
    public async Task<IActionResult> CreateLoadGeneratorCredential([FromRoute] Guid organisationId,
        [FromBody] CreateLoadGeneratorCredentialSubmission submission)
    {
        var request = new CreateLoadGeneratorCredentialRequest(submission, organisationId);
        return await _createLoadGeneratorCredentialService.Value.CreateLoadGeneratorCredential(request);
    }

    [HttpDelete]
    [Route("credentials/{loadGeneratorCredentialId:guid}")]
    [Authorize(Policy = Policies.User)]
    public async Task<IActionResult> RemoveLoadGeneratorCredential([FromRoute] Guid organisationId,
        [FromRoute] Guid loadGeneratorCredentialId)
    {
        var request = new DeleteLoadGeneratorCredentialRequest
        {
            OrganisationId = organisationId,
            LoadGeneratorCredentialId = loadGeneratorCredentialId
        };
        return await _deleteLoadGeneratorCredentialService.Value.DeleteLoadGeneratorCredential(request);
    }
}