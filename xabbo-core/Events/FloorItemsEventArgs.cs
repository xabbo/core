using System;
using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class FloorItemsEventArgs : EventArgs
    {
        public IFloorItem[] Items { get; }

        public FloorItemsEventArgs(IEnumerable<IFloorItem> items)
        {
            if (items is IFloorItem[] array)
                Items = array;
            else
                Items = items.ToArray();
        }
    }
}
