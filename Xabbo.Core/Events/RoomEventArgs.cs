using System;

using Xabbo.Core.Game;

namespace Xabbo.Core.Events
{
    public class RoomEventArgs : EventArgs
    {
        public IRoom Room { get; }

        public RoomEventArgs(IRoom room)
        {
            Room = room;
        }
    }
}
