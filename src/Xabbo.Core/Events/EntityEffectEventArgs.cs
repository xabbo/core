namespace Xabbo.Core.Events;

public sealed class EntityEffectEventArgs(IEntity entity, int previousEffect)
    : EntityEventArgs(entity)
{
    public int PreviousEffect { get; } = previousEffect;
}
