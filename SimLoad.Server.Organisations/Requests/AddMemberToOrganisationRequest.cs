using SimLoad.Server.Organisations.Submissions;

namespace SimLoad.Server.Organisations.Requests;

public class AddMemberToOrganisationRequest : AddMemberToOrganisationSubmission
{
    public Guid OrganisationId { get; set; }
}