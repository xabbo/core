using System;

namespace Xabbo.Core.Events
{
    public class WallItemEventArgs : EventArgs
    {
        public WallItem Item { get; }

        public WallItemEventArgs(WallItem item)
        {
            Item = item;
        }
    }
}
