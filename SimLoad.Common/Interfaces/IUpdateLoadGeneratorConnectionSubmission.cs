namespace SimLoad.Common.Interfaces;

public interface IUpdateLoadGeneratorConnectionSubmission
{
    Guid LoadGeneratorId { get; set; }
    bool Available { get; set; }
}