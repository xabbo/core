namespace Xabbo.Core.Events;

public sealed class PointsUpdatedEventArgs(ActivityPointType type, int amount, int change)
{
    public ActivityPointType Type { get; } = type;
    public int Amount { get; } = amount;
    public int Change { get; } = change;
}
