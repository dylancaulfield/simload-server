using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Test;

[Table("test")]
public class Test
{
    [Column("id")] public Guid Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("starttime")] public DateTime StartTime { get; set; }

    [Column("duration")] public TimeSpan Duration { get; set; }


    [Column("projectid")] public Guid ProjectId { get; set; }

    public Project.Project Project { get; set; }

    public List<TestScenarioWeight> ScenarioWeights { get; set; }
    public List<TestVirtualUserGraph> VirtualUserGraphs { get; set; }
}