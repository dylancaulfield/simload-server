using SimLoad.Server.Organisations.Submissions;

namespace SimLoad.Server.Organisations.Requests;

public class CreateLoadGeneratorCredentialRequest : CreateLoadGeneratorCredentialSubmission
{
    public CreateLoadGeneratorCredentialRequest(CreateLoadGeneratorCredentialSubmission submission, Guid organisationId)
    {
        Name = submission.Name;
        OrganisationId = organisationId;
    }

    public Guid OrganisationId { get; set; }
}