namespace Xabbo.Core.Events;

public sealed record CreditsUpdatedEventArgs(int Credits, int? PreviousCredits);
