namespace Xabbo.Core;

/// <summary>
/// Represents a type of room access.
/// </summary>
public enum RoomAccess
{
    None = -1,
    /// <summary>
    /// The room is open.
    /// </summary>
    Open = 0,
    /// <summary>
    /// Users must ring the doorbell.
    /// </summary>
    Doorbell = 1,
    /// <summary>
    /// The room is password protected.
    /// </summary>
    Password = 2,
    /// <summary>
    /// The room is invisible.
    /// </summary>
    Invisible = 3,
    /// <summary>
    /// The room is only open to friends.
    /// </summary>
    Friends = 7
}
