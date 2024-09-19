namespace Xabbo.Core.Events;

public sealed record ActivityPointUpdatedEventArgs(ActivityPointType Type, int Amount, int Change);
