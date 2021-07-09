using System;

namespace Xabbo.Core.Events
{
    public class EntityActionEventArgs : EntityEventArgs
    {
        public Actions Action { get; }

        public EntityActionEventArgs(IEntity entity, Actions action)
            :  base(entity)
        {
            Action = action;
        }
    }
}
