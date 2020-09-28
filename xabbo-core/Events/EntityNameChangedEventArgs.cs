using System;

namespace Xabbo.Core.Events
{
    public class EntityNameChangedEventArgs : EntityEventArgs
    {
        public string PreviousName { get; }

        public EntityNameChangedEventArgs(IEntity entity, string previousName)
            : base(entity)
        {
            PreviousName = previousName;
        }
    }
}
