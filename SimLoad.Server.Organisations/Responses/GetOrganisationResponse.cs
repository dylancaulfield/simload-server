namespace SimLoad.Server.Organisations.Responses;

public class GetOrganisationResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<GetOrganisationProjectResponse> Projects { get; set; }
    public List<GetOrganisationLoadGeneratorCredentialsResponse> LoadGeneratorCredentials { get; set; }
    public List<GetOrganisationMemberResponse> Members { get; set; }
}

public class GetOrganisationProjectResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class GetOrganisationLoadGeneratorCredentialsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class GetOrganisationMemberResponse
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
}