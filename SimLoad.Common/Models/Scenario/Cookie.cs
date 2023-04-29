namespace SimLoad.Common.Models.Scenario;

public class Cookie
{
    public string Domain { get; set; }
    public DateTime? Expires { get; set; }
    public bool HttpOnly { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public bool Secure { get; set; }
    public string Value { get; set; }
}