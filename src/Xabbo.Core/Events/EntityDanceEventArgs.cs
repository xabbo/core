namespace Xabbo.Core.Events;

public sealed class EntityDanceEventArgs(IEntity entity, Dances previousDance)
    : EntityEventArgs(entity)
{
    public Dances PreviousDance { get; } = previousDance;
}
