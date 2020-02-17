using System;

namespace Xabbo.Core.Events
{
    public class FloorItemUpdatedEventArgs : FloorItemEventArgs
    {
        public FloorItem PreviousItem { get; }

        public FloorItemUpdatedEventArgs(FloorItem previous, FloorItem updated)
            : base(updated)
        {
            PreviousItem = previous;
        }
    }
}
