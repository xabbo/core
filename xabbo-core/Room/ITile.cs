using System;

namespace Xabbo.Core
{
    /// <summary>
    /// Represents a location in a room.
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Gets the X coordinate of the tile.
        /// </summary>
        int X { get; }
        /// <summary>
        /// Gets the Y coordinate of the tile.
        /// </summary>
        int Y { get; }
        /// <summary>
        /// Gets the Z coordinate of the tile.
        /// </summary>
        double Z { get; }
        /// <summary>
        /// Gets the X/Y coordinates of the tile.
        /// </summary>
        (int X, int Y) XY { get; }
    }
}
