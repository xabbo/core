namespace Xabbo.Core;

/// <summary>
/// Represents a user achievement.
/// </summary>
public interface IAchievement
{
    /// <summary>
    /// The ID of the achievement.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The current level of the achievement.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// The badge code of the achievement.
    /// </summary>
    string BadgeCode { get; }

    /// <summary>
    /// The current progress of the achievement.
    /// </summary>
    int CurrentProgress { get; }

    /// <summary>
    /// The base progress for the current level of the achievement.
    /// </summary>
    int BaseProgress { get; }

    /// <summary>
    /// The maximum progress for the current level of the achievement.
    /// </summary>
    int MaxProgress { get; }

    /// <summary>
    /// The number of reward points that will be given upon leveling up the achievement.
    /// </summary>
    int LevelRewardPoints { get; }

    /// <summary>
    /// The type of reward points that will be given upon leveling up the achievement.
    /// </summary>
    int LevelRewardPointType { get; }

    /// <summary>
    /// Whether the final level of this achievement has been reached.
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    /// The category of the achievement.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// The subcategory of the achievement.
    /// </summary>
    string Subcategory { get; }

    /// <summary>
    /// The maximum level of the achievement.
    /// </summary>
    int MaxLevel { get; }

    int DisplayMethod { get; }

    short State { get; }
}
