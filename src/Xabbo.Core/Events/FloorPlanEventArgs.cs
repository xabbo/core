using System;

namespace Xabbo.Core.Events;

public class FloorPlanEventArgs : EventArgs
{
    public IFloorPlan Map { get; }

    public FloorPlanEventArgs(IFloorPlan map)
    {
        Map = map;
    }
}
