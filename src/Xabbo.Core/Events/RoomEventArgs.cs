using Xabbo.Core.Game;

namespace Xabbo.Core.Events;

public sealed class RoomEventArgs(IRoom room)
{
    public IRoom Room { get; } = room;
}
