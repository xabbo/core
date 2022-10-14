using System;

namespace Xabbo.Core.Events;

public class EntityHandItemEventArgs : EntityEventArgs
{
    public int PreviousItem { get; }

    public EntityHandItemEventArgs(IEntity entity, int previousItem)
        : base(entity)
    {
        PreviousItem = previousItem;
    }
}
