namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.ProfileManager.CreditsUpdated"/> event.
/// </summary>
/// <param name="Credits">The user's current credit amount.</param>
/// <param name="PreviousCredits">
/// The user's previous credit amount.
/// May be null if this is the first update.
/// </param>
public sealed record CreditsUpdatedEventArgs(int Credits, int? PreviousCredits);
