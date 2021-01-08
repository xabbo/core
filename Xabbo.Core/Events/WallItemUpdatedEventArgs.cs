using System;

namespace Xabbo.Core.Events
{
    public class WallItemUpdatedEventArgs : WallItemEventArgs
    {
        public IWallItem PreviousItem { get; }

        public WallItemUpdatedEventArgs(IWallItem previousItem, IWallItem updatedItem)
            : base(updatedItem)
        {
            PreviousItem = previousItem;
        }
    }
}
