namespace Xabbo.Core;

/// <summary>
/// Represents an error received after failing to enter a room.
/// </summary>
public enum RoomEnterError
{
    /// <summary>
    /// The password was incorrect.
    /// </summary>
    WrongPassword = -100002,
    Unknown = -1,
    /// <summary>
    /// The room is full.
    /// </summary>
    Full = 1,
    CannotEnter = 2,
    // 3 client disconnects
    /// <summary>
    /// The user is banned from the room.
    /// </summary>
    Banned = 4,
    // The following are not used in the RoomEnterError packet
}
