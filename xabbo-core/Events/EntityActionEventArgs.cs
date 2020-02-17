using System;

namespace Xabbo.Core.Events
{
    public class EntityActionEventArgs : EntityEventArgs
    {
        public Actions Action { get; }

        public EntityActionEventArgs(Entity entity, Actions action)
            :  base(entity)
        {
            Action = action;
        }
    }
}
