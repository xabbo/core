namespace Xabbo.Core;

/// <summary>
/// Defines who can mute, kick or ban in a room.
/// </summary>
public interface IModerationSettings
{
    /// <summary>
    /// Specifies who is allowed to mute other users in the room.
    /// </summary>
    ModerationPermissions Mute { get; }
    /// <summary>
    /// Specifies who is allowed to kick other users from the room.
    /// </summary>
    ModerationPermissions Kick { get; }
    /// <summary>
    /// Specifies who is allowed to ban other users from the room.
    /// </summary>
    ModerationPermissions Ban { get; }
}
