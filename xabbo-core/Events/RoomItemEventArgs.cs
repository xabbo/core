using System;

namespace Xabbo.Core.Events
{
    public class RoomItemEventArgs : EventArgs
    {
        public Furni Item { get; }

        public RoomItemEventArgs(Furni item)
        {
            Item = item;
        }
    }
}
