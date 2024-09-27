namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.FriendManager.MessageReceived"/> event.
/// </summary>
/// <param name="Friend">The friend that sent the message.</param>
/// <param name="Message">The message content.</param>
public sealed record FriendMessageEventArgs(IFriend Friend, ConsoleMessage Message) : FriendEventArgs(Friend);
