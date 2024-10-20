using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a tile in the heightmap.
/// </summary>
public interface IHeightmapTile
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
    /// Gets the coordinates of the tile.
    /// </summary>
    Point Location { get; }

    /// <summary>
    /// Whether the tile is a floor tile.
    /// </summary>
    bool IsFloor { get; }

    /// <summary>
    /// Whether the tile is blocked by a furniture.
    /// </summary>
    bool IsBlocked { get; }

    /// <summary>
    /// Gets whether the tile is a floor tile and is not blocked by a furniture.
    /// </summary>
    bool IsFree { get; }

    /// <summary>
    /// Gets the height for this tile at which furni may be placed.
    /// </summary>
    float Height { get; }
}
