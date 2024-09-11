namespace Xabbo.Core.Events;

public sealed class TradeStopEventArgs(IUser user, int reason)
    : RoomUserEventArgs(user)
{
    public int Reason { get; } = reason;
}