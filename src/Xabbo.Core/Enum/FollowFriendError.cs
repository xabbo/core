namespace Xabbo.Core;

/// <summary>
/// Represents an error received when attempting to follow a friend.
/// </summary>
public enum FollowFriendError
{
    /// <summary>
    /// The followed user is not a friend.
    /// </summary>
    NotFriend = 0,
    /// <summary>
    /// The friend is not currently in a room.
    /// </summary>
    Offline = 1,
    /// <summary>
    /// The friend is not currently in a room.
    /// </summary>
    NotInRoom = 2,
    /// <summary>
    /// The friend cannot be followed.
    /// </summary>
    CannotFollow = 3
}
