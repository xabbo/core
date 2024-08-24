namespace Xabbo.Core.Events;

public sealed class EntityNameChangedEventArgs(IEntity entity, string previousName)
    : EntityEventArgs(entity)
{
    public string PreviousName { get; } = previousName;
}
