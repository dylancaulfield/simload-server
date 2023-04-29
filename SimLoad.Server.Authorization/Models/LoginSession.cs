namespace SimLoad.Server.Authorization.Models;

public class LoginSession
{
    public Guid UserId { get; init; }
    public string ConfirmationCode { get; init; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public bool IsExpired => Timestamp.AddMinutes(5) < DateTime.UtcNow;
}