namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.FriendManager.FriendRequestReceived"/> event.
/// </summary>
/// <param name="UserId">The ID of the user who sent the friend request.</param>
/// <param name="UserName">The name of the user who sent the friend request.</param>
/// <param name="FigureString">The figure string of the user who sent the friend request.</param>
public sealed record FriendRequestEventArgs(Id UserId, string UserName, string? FigureString);
