using SimLoad.Server.Tests.Submissions;

namespace SimLoad.Server.Tests.Requests;

public class StartTestRequest : StartTestSubmission
{
    public StartTestRequest(StartTestSubmission submission, Guid organisationId, Guid projectId)
    {
        Name = submission.Name;
        Description = submission.Description;
        Duration = submission.Duration;
        LoadGeneratorIds = submission.LoadGeneratorIds;
        Scenarios = submission.Scenarios;
        VirtualUserGraph = submission.VirtualUserGraph;
        OrganisationId = organisationId;
        ProjectId = projectId;
    }

    public Guid OrganisationId { get; set; }
    public Guid ProjectId { get; set; }
}