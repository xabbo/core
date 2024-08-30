using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents the stacking heightmap of a room.
/// Stores whether a certain location contains a floor tile,
/// whether a furni may be placed on that tile,
/// and the stack height at which a furni may be placed.
/// </summary>
public interface IHeightmap : IEnumerable<IHeightmapTile>
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
    /// Gets the tile in this heightmap at the specified location.
    /// </summary>
    IHeightmapTile this[int x, int y] { get; }

    /// <summary>
    /// Gets the tile in this heightmap at the specified location.
    /// </summary>
    IHeightmapTile this[(int X, int Y) location] { get; }
}
