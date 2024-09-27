namespace Xabbo.Core;

/// <summary>
/// Represents extended information about a room.
/// </summary>
public interface IRoomData : IRoomInfo
{
    /// <summary>
    /// Whether the user is entering the room.
    /// </summary>
    bool IsEntering { get; }

    /// <summary>
    /// Whether to forward the user to the room in client.
    /// </summary>
    bool Forward { get; }

    /// <summary>
    /// Whether the user is a member of the room's group.
    /// </summary>
    bool IsGroupMember { get; }

    /// <summary>
    /// Whether the room is muted.
    /// </summary>
    bool IsRoomMuted { get; }

    /// <summary>
    /// The moderation settings for the room.
    /// </summary>
    IModerationSettings Moderation { get; }

    /// <summary>
    /// Whether the room can be muted.
    /// </summary>
    bool CanMute { get; }

    /// <summary>
    /// The chat settings for the room.
    /// </summary>
    IChatSettings ChatSettings { get; }
}
