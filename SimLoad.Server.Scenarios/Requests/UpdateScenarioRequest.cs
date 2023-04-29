using SimLoad.Server.Scenarios.Submissions;

namespace SimLoad.Server.Scenarios.Requests;

public class UpdateScenarioRequest : UpdateScenarioSubmission
{
    public UpdateScenarioRequest(UpdateScenarioSubmission submission, Guid projectId, Guid scenarioId)
    {
        ProjectId = projectId;
        ScenarioId = scenarioId;
        //Name = submission.Name;
        //Description = submission.Description;
        Code = submission.Code;
        Operations = submission.Operations;
        //LastUpdated = submission.LastUpdated;
    }

    public Guid ProjectId { get; set; }
    public Guid ScenarioId { get; set; }
}