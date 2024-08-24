namespace Xabbo.Core.Events;

public sealed class DoorbellEventArgs(string name)
{
    public string Name { get; } = name;
    public bool? IsAccepted { get; set; }
    public void Accept() => IsAccepted = true;
    public void Decline() => IsAccepted = false;
}
