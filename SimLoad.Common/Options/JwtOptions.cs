namespace SimLoad.Common.Options;

public class JwtOptions
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int HoursUntilExpiry { get; set; }
}