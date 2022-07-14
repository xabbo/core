using System;

namespace Xabbo.Core.Events;

public class FloorItemEventArgs  : EventArgs
{
    public IFloorItem Item { get; }

    public FloorItemEventArgs(IFloorItem item)
    {
        Item = item;
    }
}
