namespace Xabbo.Core.Events;

public sealed class RoomDataEventArgs(IRoomData data)
{
    public IRoomData Data { get; } = data;
}
