using System;

namespace Xabbo.Core.Events;

public sealed class FloorPlanEventArgs(IFloorPlan floorPlan) : EventArgs
{
    public IFloorPlan FloorPlan { get; } = floorPlan;
}
