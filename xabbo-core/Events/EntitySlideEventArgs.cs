using System;

namespace Xabbo.Core.Events
{
    public class EntitySlideEventArgs : EntityEventArgs
    {
        public ITile PreviousTile { get; set; }

        public EntitySlideEventArgs(IEntity entity, ITile previousTile)
            : base(entity)
        {
            PreviousTile = previousTile;
        }
    }
}
