namespace Xabbo.Core.Events;

public sealed class EntityActionEventArgs(IEntity entity, Actions action)
    : EntityEventArgs(entity)
{
    public Actions Action { get; } = action;
}
