using System;

namespace Xabbo.Core.Events;

public class EntityEventArgs : EventArgs
{
    public IEntity Entity { get; }

    public EntityEventArgs(IEntity entity)
    {
        Entity = entity;
    }
}
