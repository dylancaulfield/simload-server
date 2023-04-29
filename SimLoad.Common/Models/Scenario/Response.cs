namespace SimLoad.Common.Models.Scenario;

public class Response
{
    public string? Body { get; set; } = string.Empty;
    public List<Header> Headers { get; set; }
    public List<Cookie> Cookies { get; set; }
    public string? RedirectUrl { get; set; } = string.Empty;
    public int Status { get; set; }
}