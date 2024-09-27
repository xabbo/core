using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// A collection of achievements.
/// </summary>
public interface IAchievements : IReadOnlyCollection<IAchievement>
{
    IAchievement? this[int id] { get; }
}
