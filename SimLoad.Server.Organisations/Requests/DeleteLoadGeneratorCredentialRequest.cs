namespace SimLoad.Server.Organisations.Requests;

public class DeleteLoadGeneratorCredentialRequest
{
    public Guid OrganisationId { get; set; }
    public Guid LoadGeneratorCredentialId { get; set; }
}