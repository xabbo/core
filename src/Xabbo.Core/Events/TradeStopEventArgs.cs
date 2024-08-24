namespace Xabbo.Core.Events;

public sealed class TradeStopEventArgs(IRoomUser user, int reason)
    : RoomUserEventArgs(user)
{
    public int Reason { get; } = reason;
}