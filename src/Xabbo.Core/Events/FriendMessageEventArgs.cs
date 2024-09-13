namespace Xabbo.Core.Events;

public sealed record FriendMessageEventArgs(IFriend Friend, ConsoleMessage Message) : FriendEventArgs(Friend);
