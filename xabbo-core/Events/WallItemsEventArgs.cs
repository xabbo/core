using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.Events
{
    public class WallItemsEventArgs : EventArgs
    {
        public WallItem[] Items { get; }

        public WallItemsEventArgs(IEnumerable<WallItem> items)
        {
            if (items is WallItem[] array)
                Items = array;
            else
                Items = items.ToArray();
        }
    }
}
