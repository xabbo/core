using System;

namespace Xabbo.Core.Events
{
    public class FloorItemSlideEventArgs : FloorItemEventArgs
    {
        public Tile PreviousTile { get; }
        public int RollerId;

        public FloorItemSlideEventArgs(FloorItem item, Tile previousTile, int rollerId)
            : base(item)
        {
            PreviousTile = previousTile;
            RollerId = rollerId;
        }
    }
}
