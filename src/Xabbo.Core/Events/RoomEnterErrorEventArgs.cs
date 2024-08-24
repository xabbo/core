namespace Xabbo.Core.Events;

public sealed class RoomEnterErrorEventArgs(RoomEnterError error)
{
    public RoomEnterError Error { get; } = error;
}
