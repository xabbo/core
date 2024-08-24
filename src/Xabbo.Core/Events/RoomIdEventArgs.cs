namespace Xabbo.Core.Events;

public sealed class RoomIdEventArgs(Id roomId)
{
    public Id RoomId { get; } = roomId;
}