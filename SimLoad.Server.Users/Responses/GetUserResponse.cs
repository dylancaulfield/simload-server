namespace SimLoad.Server.Users.Responses;

public class GetUserResponse
{
    public string DisplayName { get; set; }
    public List<string> EmailAddresses { get; set; }
}