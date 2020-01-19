using System;
using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class FloorItemsEventArgs : EventArgs
    {
        public FloorItem[] Items { get; }

        public FloorItemsEventArgs(IEnumerable<FloorItem> items)
        {
            if (items is FloorItem[] array)
                Items = array;
            else
                Items = items.ToArray();
        }
    }
}
