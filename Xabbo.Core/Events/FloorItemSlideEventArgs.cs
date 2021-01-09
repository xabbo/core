using System;

namespace Xabbo.Core.Events
{
    public class FloorItemSlideEventArgs : FloorItemEventArgs
    {
        public Tile PreviousTile { get; }
        public long RollerId;

        public FloorItemSlideEventArgs(IFloorItem item, Tile previousTile, long rollerId)
            : base(item)
        {
            PreviousTile = previousTile;
            RollerId = rollerId;
        }
    }
}
