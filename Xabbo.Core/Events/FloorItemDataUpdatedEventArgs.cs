using System;

namespace Xabbo.Core.Events
{
    public class FloorItemDataUpdatedEventArgs : FloorItemEventArgs
    {
        public IItemData PreviousData { get; }

        public FloorItemDataUpdatedEventArgs(IFloorItem item, IItemData previousData)
            : base(item)
        {
            PreviousData = previousData;
        }
    }
}
