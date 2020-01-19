using System;

namespace Xabbo.Core.Events
{
    public class FloorItemEventArgs  : EventArgs
    {
        public FloorItem Item { get; }

        public FloorItemEventArgs(FloorItem item)
        {
            Item = item;
        }
    }
}
