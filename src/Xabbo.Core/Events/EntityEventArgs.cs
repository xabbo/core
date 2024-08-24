namespace Xabbo.Core.Events;

public class EntityEventArgs(IEntity entity)
{
    public IEntity Entity => entity;
}
