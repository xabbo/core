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
    /// Gets the size of the heightmap.
    /// </summary>
    Point Size { get; }

    /// <summary>
    /// Gets the area of the heightmap.
    /// </summary>
    Area Area { get; }

    /// <summary>
    /// Gets the heightmap tile at the specified coordinates.
    /// </summary>
    IHeightmapTile this[int x, int y] { get; }

    /// <summary>
    /// Gets the heightmap tile at the specified coordinates.
    /// </summary>
    IHeightmapTile this[Point point] { get; }

    /// <summary>
    /// Gets whether a furni can be placed at the specified area.
    /// </summary>
    bool CanPlaceAt(Area area);

    /// <summary>
    /// Attempts to find a point where a furniture of the specified size can be placed.
    /// </summary>
    /// <param name="size">The size of the furniture to place.</param>
    /// <param name="entry">The room entry tile. This tile will be ignored if specified.</param>
    /// <returns></returns>
    Point? FindPlaceablePoint(Point size, Point? entry = null);
}
