using System;

namespace Xabbo.Core.Events;

public class FurniEventArgs(IFurni item) : EventArgs
{
    public IFurni Item { get; } = item;
}
