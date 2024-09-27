namespace Xabbo.Core.Events;

/// <summary>
/// Contains event arguments for the <see cref="Game.ProfileManager.AchievementUpdated"/> event.
/// </summary>
/// <param name="Achievement">The achievement that was updated.</param>
public sealed record AchievementUpdatedEventArgs(Achievement Achievement);
