namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.FriendManager.FriendAdded"/> or
/// <see cref="Game.FriendManager.FriendRemoved"/> event.
/// </summary>
/// <param name="Friend">The friend involved in the event.</param>
public record FriendEventArgs(IFriend Friend);
