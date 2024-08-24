namespace Xabbo.Core.Events;

public sealed class TradeAcceptEventArgs(IRoomUser user, bool accepted)
    : RoomUserEventArgs(user)
{
    public bool Accepted { get; } = accepted;
}
