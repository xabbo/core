using System;
using System.Collections.Generic;

namespace Xabbo.Core.Events
{
    public class RoomItemsEventArgs : EventArgs
    {
        public IEnumerable<Furni> Items { get; }

        public RoomItemsEventArgs(IEnumerable<Furni> items)
        {
            Items = items;
        }
    }
}
