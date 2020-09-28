using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class WallItemsEventArgs : EventArgs
    {
        public IReadOnlyCollection<IWallItem> Items { get; }

        public WallItemsEventArgs(IEnumerable<IWallItem> items)
        {
            Items = new List<IWallItem>(items).AsReadOnly();
        }
    }
}
