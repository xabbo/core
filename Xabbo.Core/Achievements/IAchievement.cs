using System;

namespace Xabbo.Core
{
    public interface IAchievement
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Gets the current level.
        /// </summary>
        int Level { get; }
        /// <summary>
        /// Gets the badge ID.
        /// </summary>
        string BadgeId { get; }
        /// <summary>
        /// Gets the base progress for the current level.
        /// </summary>
        int BaseProgress { get; }
        /// <summary>
        /// Gets the maximum progress for the current level.
        /// </summary>
        int MaxProgress { get; }
        /// <summary>
        /// Gets the number of reward points that will be given upon leveling up this achievement.
        /// </summary>
        int LevelRewardPoints { get; }
        /// <summary>
        /// Gets the type of reward points that will be given upon leveling up this achievement.
        /// </summary>
        int LevelRewardPointType { get; }
        /// <summary>
        /// Gets the total current progress of this achievement.
        /// </summary>
        int CurrentProgress { get; }
        /// <summary>
        /// Gets if this the final level of this achievement has been reached.
        /// </summary>
        bool IsComplete { get; }
        /// <summary>
        /// Gets the category of this achievement.
        /// </summary>
        string Category { get; }
        /// <summary>
        /// Gets the sub-category of this achievement.
        /// </summary>
        string Subcategory { get; }
        /// <summary>
        /// Gets the maximum level of this achievement.
        /// </summary>
        int MaxLevel { get; }
        int DisplayMethod { get; }
        short State { get; }
    }
}
