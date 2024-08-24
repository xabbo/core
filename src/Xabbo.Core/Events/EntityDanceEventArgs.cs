namespace Xabbo.Core.Events;

public sealed class EntityDanceEventArgs(IEntity entity, int previousDance)
    : EntityEventArgs(entity)
{
    public int PreviousDance { get; } = previousDance;
}
