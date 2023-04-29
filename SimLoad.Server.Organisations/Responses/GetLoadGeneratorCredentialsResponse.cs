namespace SimLoad.Server.Organisations.Responses;

public class GetLoadGeneratorCredentialsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ApiKey { get; set; }
}