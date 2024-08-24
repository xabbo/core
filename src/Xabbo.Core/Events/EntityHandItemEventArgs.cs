namespace Xabbo.Core.Events;

public sealed class EntityHandItemEventArgs(IEntity entity, int previousItem)
    : EntityEventArgs(entity)
{
    public int PreviousItem { get; } = previousItem;
}
