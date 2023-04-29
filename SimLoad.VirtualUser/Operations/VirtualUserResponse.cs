namespace SimLoad.VirtualUser.Operations;

public class VirtualUserResponse
{
    public int StatusCode { get; set; }
    public List<VirtualUserResponseHeader> Headers { get; set; }
    public Lazy<Task<string>> Body => new(() => HttpContent.ReadAsStringAsync());
    public HttpContent HttpContent { get; set; }
}

public class VirtualUserResponseHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}