using System;

namespace Xabbo.Core
{
    /// <summary>
    /// Represents an entity with an X, Y, and Z location and direction in a room.
    /// </summary>
    public interface IRoomEntity
    {
        /// <summary>
        /// Gets the location of this entity.
        /// </summary>
        Tile Location { get; }

        /// <summary>
        /// Gets the direction of this entity.
        /// </summary>
        int Direction { get; }
    }
}
