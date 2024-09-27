using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents information about a room.
/// </summary>
public interface IRoomInfo
{
    /// <summary>
    /// The ID of the room.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// The name of the room.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The ID of the room owner.
    /// </summary>
    Id OwnerId { get; }

    /// <summary>
    /// The name of the room owner.
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    /// The room access mode.
    /// </summary>
    RoomAccess Access { get; }

    /// <summary>
    /// Gets whether the room is open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Gets whether users must ring the doorbell.
    /// </summary>
    bool IsDoorbell { get; }

    /// <summary>
    /// Gets whether the room is password protected.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Gets whether the room is invisible.
    /// </summary>
    bool IsInvisible { get; }

    /// <summary>
    /// The number of users in the room.
    /// </summary>
    int Users { get; }

    /// <summary>
    /// The maximum number of users allowed in the room.
    /// </summary>
    int MaxUsers { get; }

    /// <summary>
    /// The room description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The room trading permissions.
    /// </summary>
    TradePermissions Trading { get; }

    int Score { get; }
    int Ranking { get; }

    /// <summary>
    /// The room category.
    /// </summary>
    RoomCategory Category { get; }

    /// <summary>
    /// A list of tags that can be used to search for the room.
    /// </summary>
    IReadOnlyList<string> Tags { get; }

    RoomFlags Flags { get; }
    bool HasOfficialRoomPic { get; }

    /// <summary>
    /// Whether the room is a group home room.
    /// </summary>
    bool IsGroupRoom { get; }

    /// <summary>
    /// Whether the room current has an event.
    /// </summary>
    bool HasEvent { get; }

    /// <summary>
    /// Whether to show the room owner name in client.
    /// </summary>
    bool ShowOwnerName { get; }

    /// <summary>
    /// Whether other users' pets are allowed in the room.
    /// </summary>
    bool AllowPets { get; }

    string OfficialRoomPicRef { get; }

    /// <summary>
    /// The room's group ID.
    /// </summary>
    Id GroupId { get; }

    /// <summary>
    /// The room's group name.
    /// </summary>
    string GroupName { get; }

    /// <summary>
    /// The room's group badge code.
    /// </summary>
    string GroupBadge { get; }

    /// <summary>
    /// The current event name.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// The current event description.
    /// </summary>
    string EventDescription { get; }

    /// <summary>
    /// The remaining time of the event in minutes.
    /// </summary>
    int EventMinutesRemaining { get; }
}
