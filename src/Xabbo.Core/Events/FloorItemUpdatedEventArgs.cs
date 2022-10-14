using System;

namespace Xabbo.Core.Events;

public class FloorItemUpdatedEventArgs : FloorItemEventArgs
{
    public IFloorItem PreviousItem { get; }

    public FloorItemUpdatedEventArgs(IFloorItem previous, IFloorItem updated)
        : base(updated)
    {
        PreviousItem = previous;
    }
}
