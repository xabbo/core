using System;

namespace Xabbo.Core.Events
{
    public class WallItemUpdatedEventArgs : WallItemEventArgs
    {
        public WallItem PreviousItem { get; }

        public WallItemUpdatedEventArgs(WallItem previousItem, WallItem updatedItem)
            : base(updatedItem)
        {
            PreviousItem = previousItem;
        }
    }
}
