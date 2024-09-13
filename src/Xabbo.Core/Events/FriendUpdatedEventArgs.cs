namespace Xabbo.Core.Events;

public sealed record FriendUpdatedEventArgs(IFriend Previous, IFriend Friend) : FriendEventArgs(Friend);
