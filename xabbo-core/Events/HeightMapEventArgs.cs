using System;

namespace Xabbo.Core.Events
{
    public class FloorPlanEventArgs : EventArgs
    {
        public FloorPlan Map { get; }

        public FloorPlanEventArgs(FloorPlan map)
        {
            Map = map;
        }
    }
}
