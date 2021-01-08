using System;

namespace Xabbo.Core.Events
{
    public class EntitySlideEventArgs : EntityEventArgs
    {
        public Tile PreviousTile { get; set; }

        public EntitySlideEventArgs(IEntity entity, Tile previousTile)
            : base(entity)
        {
            PreviousTile = previousTile;
        }
    }
}
