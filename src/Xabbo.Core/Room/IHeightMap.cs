using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents the stacking heightmap of a room.
/// Stores whether a certain location contains a floor tile,
/// whether a furni may be placed on that tile,
/// and the height at which a furni may be placed.
/// </summary>
public interface IHeightmap : IEnumerable<IHeightmapTile>
{
    /// <summary>
    /// Gets the width of the heightmap.
    /// </summary>
    /// <remarks>
    /// Width spans the X axis of the room.
    /// </remarks>
    int Width { get; }

    /// <summary>
    /// Gets the length of the heightmap.
    /// </summary>
    /// <remarks>
    /// Length spans the Y axis of the room.
    /// </remarks>
    int Length { get; }

    /// <summary>
    /// Gets the heightmap tile at the specified coordinates.
    /// </summary>
    IHeightmapTile this[int x, int y] { get; }

    /// <summary>
    /// Gets the heightmap tile at the specified coordinates.
    /// </summary>
    IHeightmapTile this[Point point] { get; }
}
