using System.ComponentModel.DataAnnotations;
using SimLoad.Common.Models;

namespace SimLoad.Server.Tests.Submissions;

public class StartTestSubmission : IValidatableObject
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public List<Guid> LoadGeneratorIds { get; set; }
    public List<StartTestSubmissionScenario> Scenarios { get; set; }
    public VirtualUserGraph VirtualUserGraph { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Duration < TimeSpan.FromMinutes(1))
            yield return new ValidationResult("Duration must be at least 1 minute", new[] { nameof(Duration) });

        if (LoadGeneratorIds.Count == 0)
            yield return new ValidationResult("At least one load generator must be specified",
                new[] { nameof(LoadGeneratorIds) });

        if (Scenarios.Count == 0)
            yield return new ValidationResult("At least one scenario must be specified", new[] { nameof(Scenarios) });

        if (!Scenarios.All(s => s.Weight > 0))
            yield return new ValidationResult("Scenario weights must all be > 0", new[] { nameof(Scenarios) });

        if (VirtualUserGraph.Points.Count < 2)
            yield return new ValidationResult("User count graph must include two points",
                new[] { nameof(VirtualUserGraph) });

        if (VirtualUserGraph.Points.Count >= 2)
        {
            var first = VirtualUserGraph.Points.First();
            var last = VirtualUserGraph.Points.Last();

            if (first.X != 0)
                yield return new ValidationResult("First point must be at x = 0",
                    new[] { nameof(VirtualUserGraph.Points) });

            if (last.X != 100)
                yield return new ValidationResult("Last point must be at x = 100",
                    new[] { nameof(VirtualUserGraph.Points) });
        }

        if (VirtualUserGraph.Points.Count >= 1)
            foreach (var point in VirtualUserGraph.Points)
                if (VirtualUserGraph.Points.Count(p => p.X == point.X) > 1)
                    yield return new ValidationResult("Duplicate X values", new[] { nameof(VirtualUserGraph.Points) });

        if (VirtualUserGraph.Points.Count >= 2)
        {
            var previous = VirtualUserGraph.Points.First();

            foreach (var point in VirtualUserGraph.Points)
            {
                if (point.Y < 0)
                    yield return new ValidationResult("Y must be greater than or equal to 0",
                        new[] { nameof(VirtualUserGraph.Points) });

                if (point.X < previous.X)
                    yield return new ValidationResult("Points must be sorted",
                        new[] { nameof(VirtualUserGraph.Points) });

                previous = point;
            }
        }
    }
}

public class StartTestSubmissionScenario
{
    public Guid Id { get; set; }
    public int Weight { get; set; }
}