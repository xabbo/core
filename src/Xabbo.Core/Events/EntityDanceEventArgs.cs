using System;

namespace Xabbo.Core.Events;

public class EntityDanceEventArgs : EntityEventArgs
{
    public int PreviousDance { get; }

    public EntityDanceEventArgs(IEntity entity, int previousDance)
        : base(entity)
    {
        PreviousDance = previousDance;
    }
}
