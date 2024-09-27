namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.FriendManager.FriendUpdated"/> event.
/// </summary>
/// <param name="Previous">The previous state of the friend.</param>
/// <param name="Friend">The updated state of the friend.</param>
public sealed record FriendUpdatedEventArgs(IFriend Previous, IFriend Friend) : FriendEventArgs(Friend);
