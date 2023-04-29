using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Organisations.Requests;
using SimLoad.Server.Organisations.Services;
using SimLoad.Server.Organisations.Submissions;

namespace SimLoad.Server.Organisations;

[ApiController]
[Route("/api/organisations/{organisationId:guid}/members")]
[Authorize(Policy = Policies.User)]
public class OrganisationMemberController
{
    private readonly Lazy<IAddMemberToOrganisationService> _addMemberService;
    private readonly Lazy<IRemoveMemberFromOrganisationService> _removeMemberService;

    public OrganisationMemberController(
        Lazy<IAddMemberToOrganisationService> addMemberService,
        Lazy<IRemoveMemberFromOrganisationService> removeMemberService
    )
    {
        _addMemberService = addMemberService;
        _removeMemberService = removeMemberService;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddMember([FromRoute] Guid organisationId,
        [FromBody] AddMemberToOrganisationSubmission submission)
    {
        var request = new AddMemberToOrganisationRequest
        {
            OrganisationId = organisationId,
            EmailAddress = submission.EmailAddress
        };
        return await _addMemberService.Value.AddMemberToOrganisation(request);
    }

    [HttpDelete]
    [Route("{organisationMemberId:guid}")]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid organisationId,
        [FromRoute] Guid organisationMemberId)
    {
        var request = new RemoveMemberFromOrganisationRequest
        {
            OrganisationId = organisationId,
            OrganisationMemberId = organisationMemberId
        };
        return await _removeMemberService.Value.RemoveMemberFromOrganisation(request);
    }

    // [HttpPut]
    // [Route("{memberId:guid}/permissions")]
    // public async Task<IActionResult> UpdateMemberPermissions([FromRoute] Guid organisationId, [FromRoute] Guid memberId, [FromBody] UpdateMemberPermissionsRequest request)
    // {
    //     return await _updateMemberPermissionsService.Value.UpdateMemberPermissions(organisationId, memberId, request);
    // }
}