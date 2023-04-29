namespace SimLoad.VirtualUser.Contexts;

public class VirtualUserId
{
    public VirtualUserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }
}