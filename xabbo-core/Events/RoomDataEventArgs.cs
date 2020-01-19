using System;

namespace Xabbo.Core.Events
{
    public class RoomDataEventArgs : EventArgs
    {
        public RoomData Data { get; }

        public RoomDataEventArgs(RoomData data)
        {
            Data = data;
        }
    }
}
