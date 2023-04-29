using SimLoad.Server.Scenarios.Submissions;

namespace SimLoad.Server.Scenarios.Requests;

public class CreateScenarioRequest : CreateScenarioSubmission
{
    public CreateScenarioRequest(CreateScenarioSubmission submission, Guid projectId)
    {
        ProjectId = projectId;
        Name = submission.Name;
        Description = submission.Description;
        Code = submission.Code;
        Operations = submission.Operations;
    }

    public Guid ProjectId { get; set; }
}