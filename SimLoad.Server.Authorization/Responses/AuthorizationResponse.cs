namespace SimLoad.Server.Authorization.Responses;

public class AuthorizationResponse
{
    public string AccessToken { get; init; }
    public DateTime Expires { get; set; }
    public Guid RefreshToken { get; init; }
}