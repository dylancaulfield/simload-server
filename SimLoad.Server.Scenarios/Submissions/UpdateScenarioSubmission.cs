using System.ComponentModel.DataAnnotations;
using SimLoad.Common.Models.Scenario;

namespace SimLoad.Server.Scenarios.Submissions;

public class UpdateScenarioSubmission
{
    //[MinLength(1)] public string Name { get; set; }
    //public string Description { get; set; }
    public string Code { get; init; }
    public UpdateScenarioSubmissionOperations Operations { get; set; }
    //public DateTime LastUpdated { get; set; }
}

public class UpdateScenarioSubmissionOperations : IValidatableObject
{
    public Dictionary<Guid, Operation> Created { get; set; }
    public Dictionary<Guid, Operation> Updated { get; set; }
    public List<Guid> Deleted { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var operationId in Created.Keys)
        {
            if (Updated.Keys.Any(id => id == operationId))
                yield return new ValidationResult($"{operationId} is in created and updated",
                    new[] { nameof(Created), nameof(Updated) });

            if (Deleted.Any(id => id == operationId))
                yield return new ValidationResult($"{operationId} is in created and deleted",
                    new[] { nameof(Created), nameof(Deleted) });
        }

        foreach (var operationId in Updated.Keys)
            if (Deleted.Any(id => id == operationId))
                yield return new ValidationResult($"{operationId} is in updated and deleted",
                    new[] { nameof(Updated), nameof(Deleted) });
    }
}