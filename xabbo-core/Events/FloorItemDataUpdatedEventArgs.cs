using System;

namespace Xabbo.Core.Events
{
    public class FloorItemDataUpdatedEventArgs : FloorItemEventArgs
    {
        public StuffData PreviousData { get; }

        public FloorItemDataUpdatedEventArgs(FloorItem item, StuffData previousData)
            : base(item)
        {
            PreviousData = previousData;
        }
    }
}
