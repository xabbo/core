using System;

namespace Xabbo.Core.Events
{
    public class EntityTypingEventArgs : EntityEventArgs
    {
        public bool WasTyping { get; }

        public EntityTypingEventArgs(IEntity entity, bool wasTyping)
            : base(entity)
        {
            WasTyping = wasTyping;
        }
    }
}
