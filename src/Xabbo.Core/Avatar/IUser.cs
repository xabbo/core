namespace Xabbo.Core;

/// <summary>
/// Represents a user in a room.
/// </summary>
public interface IUser : IAvatar
{
    /// <summary>
    /// The gender of the user.
    /// </summary>
    Gender Gender { get; }

    /// <summary>
    /// The ID of the user's selected group.
    /// </summary>
    Id GroupId { get; }

    int GroupStatus { get; }

    /// <summary>
    /// The name of the user's selected group.
    /// </summary>
    string GroupName { get; }

    string FigureExtra { get; }

    /// <summary>
    /// The achievement score of the user.
    /// </summary>
    int AchievementScore { get; }

    /// <summary>
    /// Whether the user is a staff member.
    /// </summary>
    bool IsStaff { get; }

    /// <summary>
    /// The rights level of the user in the room.
    /// </summary>
    RightsLevel RightsLevel { get; }

    /// <summary>
    /// Whether the user has rights in the room.
    /// </summary>
    bool HasRights { get; }
}
