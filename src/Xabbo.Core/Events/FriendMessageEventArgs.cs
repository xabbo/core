namespace Xabbo.Core.Events;

public sealed class FriendMessageEventArgs(IFriend friend, string message) : FriendEventArgs(friend)
{
    public string Message { get; } = message;
}
