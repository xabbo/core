using System;

namespace Xabbo.Core.Events;

public class DoorbellEventArgs : EventArgs
{
    public string Name { get; }
    public bool? IsAccepted { get; set; }

    public DoorbellEventArgs(string name)
    {
        Name = name;
    }

    public void Accept() => IsAccepted = true;
    public void Decline() => IsAccepted = false;
}
