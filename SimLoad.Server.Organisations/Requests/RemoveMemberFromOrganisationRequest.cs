namespace SimLoad.Server.Organisations.Requests;

public class RemoveMemberFromOrganisationRequest
{
    public Guid OrganisationId { get; set; }
    public Guid OrganisationMemberId { get; set; }
}