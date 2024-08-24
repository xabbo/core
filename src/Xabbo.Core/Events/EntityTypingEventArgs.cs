namespace Xabbo.Core.Events;

public sealed class EntityTypingEventArgs(IEntity entity, bool wasTyping)
    : EntityEventArgs(entity)
{
    public bool WasTyping { get; } = wasTyping;
}
