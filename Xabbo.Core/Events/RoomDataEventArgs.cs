using System;

namespace Xabbo.Core.Events
{
    public class RoomDataEventArgs : EventArgs
    {
        public IRoomData Data { get; }

        public RoomDataEventArgs(IRoomData data)
        {
            Data = data;
        }
    }
}
