using System;

namespace Xabbo.Core.Events;

public class RoomEnterErrorEventArgs : EventArgs
{
    public RoomEnterError Error { get; }

    public RoomEnterErrorEventArgs(RoomEnterError error)
    {
        Error = error;
    }
}
