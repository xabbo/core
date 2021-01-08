using System;

namespace Xabbo.Core.Events
{
    public class EntityIdleEventArgs : EntityEventArgs
    {
        public bool WasIdle { get; }

        public EntityIdleEventArgs(IEntity entity, bool wasIdle)
            : base(entity)
        {
            WasIdle = wasIdle;
        }
    }
}
