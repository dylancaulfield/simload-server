using SimLoad.Server.Projects.Submissions;

namespace SimLoad.Server.Projects.Requests;

public class CreateProjectRequest : CreateProjectSubmission
{
    public CreateProjectRequest(CreateProjectSubmission submission, Guid organisationId)
    {
        Name = submission.Name;
        Description = submission.Description;
        OrganisationId = organisationId;
    }

    public Guid OrganisationId { get; set; }
}