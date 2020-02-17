using System;

namespace Xabbo.Core.Events
{
    public class FloorItemSlideEventArgs : FloorItemEventArgs
    {
        public Tile PreviousTile { get; }

        public FloorItemSlideEventArgs(FloorItem item, Tile previousTile)
            : base(item)
        {
            PreviousTile = previousTile;
        }
    }
}
