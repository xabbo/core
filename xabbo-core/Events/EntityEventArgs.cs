using System;

namespace Xabbo.Core.Events
{
    public class EntityEventArgs : EventArgs
    {
        public Entity Entity { get; }

        public EntityEventArgs(Entity entity)
        {
            Entity = entity;
        }
    }
}
