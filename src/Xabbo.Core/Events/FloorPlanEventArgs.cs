using System;

namespace Xabbo.Core.Events;

public sealed class FloorPlanEventArgs(IFloorPlan floorPlan) : EventArgs
{
    /// <summary>
    /// Gets the floor plan.
    /// </summary>
    public IFloorPlan FloorPlan { get; } = floorPlan;
}
