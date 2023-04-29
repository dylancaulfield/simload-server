using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Test;

[Table("test_scenario_weight")]
public class TestScenarioWeight
{
    [Column("id")] public Guid Id { get; set; }

    [Column("scenarioid")] public Guid ScenarioId { get; set; }

    [Column("weight")] public int Weight { get; set; }

    [Column("testid")] public Guid TestId { get; set; }

    public Test Test { get; set; }
}