using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Test;

[Table("test_virtual_user_graph")]
public class TestVirtualUserGraph
{
    [Column("id")] public Guid Id { get; set; }

    [Column("loadgeneratorid")] public Guid LoadGeneratorId { get; set; }

    [Column("x")] public int X { get; set; }

    [Column("y")] public int Y { get; set; }

    [Column("testid")] public Guid TestId { get; set; }

    public Test Test { get; set; }
}