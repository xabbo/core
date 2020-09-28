using System;

namespace Xabbo.Core.Events
{
    public class FloorItemSlideEventArgs : FloorItemEventArgs
    {
        public ITile PreviousTile { get; }
        public int RollerId;

        public FloorItemSlideEventArgs(IFloorItem item, ITile previousTile, int rollerId)
            : base(item)
        {
            PreviousTile = previousTile;
            RollerId = rollerId;
        }
    }
}
