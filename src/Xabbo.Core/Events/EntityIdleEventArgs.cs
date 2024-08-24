namespace Xabbo.Core.Events;

public sealed class EntityIdleEventArgs(IEntity entity, bool wasIdle)
    : EntityEventArgs(entity)
{
    public bool WasIdle { get; } = wasIdle;
}
