namespace Xabbo.Core.Events;

public sealed class TradeAcceptEventArgs(IUser user, bool accepted)
    : RoomUserEventArgs(user)
{
    public bool Accepted { get; } = accepted;
}
