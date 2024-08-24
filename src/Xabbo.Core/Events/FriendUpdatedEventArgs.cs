namespace Xabbo.Core.Events;

public sealed class FriendUpdatedEventArgs(IFriend previous, IFriend current)
    : FriendEventArgs(current)
{
    public IFriend Previous { get; } = previous;
}
