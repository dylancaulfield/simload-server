namespace SimLoad.Server.Authorization.Requests;

public class RefreshAuthorizationRequest
{
    public Guid RefreshToken { get; set; }
}