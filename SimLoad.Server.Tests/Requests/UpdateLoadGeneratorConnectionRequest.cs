using SimLoad.Common.Interfaces;

namespace SimLoad.Server.Tests.Requests;

public class UpdateLoadGeneratorConnectionRequest : IUpdateLoadGeneratorConnectionSubmission
{
    public Guid OrganisationId { get; set; }
    public string IpAddress { get; set; }
    public Guid LoadGeneratorId { get; set; }
    public bool Available { get; set; }
}