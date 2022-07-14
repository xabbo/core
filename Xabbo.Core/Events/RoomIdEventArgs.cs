using System;

namespace Xabbo.Core.Events;

public class RoomIdEventArgs : EventArgs
{
    public long RoomId { get; }
    
    public RoomIdEventArgs(long roomId)
    {
        RoomId = roomId;
    }
}