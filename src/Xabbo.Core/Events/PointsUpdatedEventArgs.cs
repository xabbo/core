namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.ProfileManager.ActivityPointUpdated"/> event.
/// </summary>
/// <param name="Type">The type of activity point that was updated.</param>
/// <param name="Amount">The updated amount of the activity point.</param>
/// <param name="Change">The relative change of the activity point.</param>
public sealed record ActivityPointUpdatedEventArgs(ActivityPointType Type, int Amount, int Change);
