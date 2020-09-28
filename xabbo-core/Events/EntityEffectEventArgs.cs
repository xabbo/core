using System;

namespace Xabbo.Core.Events
{
    public class EntityEffectEventArgs : EntityEventArgs
    {
        public int PreviousEffect { get; }

        public EntityEffectEventArgs(IEntity entity, int previousEffect)
            : base(entity)
        {
            PreviousEffect = previousEffect;
        }
    }
}
