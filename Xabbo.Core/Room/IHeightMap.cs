using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    /// <summary>
    /// Represents the stacking heightmap of a room.
    /// Stores whether a certain location contains a tile,
    /// whether a furni may be placed on that tile,
    /// and the height at which a furni may be placed.
    /// </summary>
    public interface IHeightmap : IEnumerable<IHeightmapTile>, IComposable
    {
        /// <summary>
        /// Gets the width of this heightmap.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Gets the length of this heightmap.
        /// </summary>
        int Length { get; }
        /// <summary>
        /// Gest the tile in this heightmap at the specified location.
        /// </summary>
        IHeightmapTile this[int x, int y] { get; }
        /// <summary>
        /// Gest the tile in this heightmap at the specified location.
        /// </summary>
        IHeightmapTile this[(int X, int Y) location] { get; }
    }
}
