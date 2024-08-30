using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a tile in the heightmap.
/// </summary>
public interface IHeightmapTile
{
    /// <summary>
    /// Gets the X coordinate of this tile.
    /// </summary>
    int X { get; }
    /// <summary>
    /// Gets the Y coordinate of this tile.
    /// </summary>
    int Y { get; }
    /// <summary>
    /// Gets the coordinates of this tile.
    /// </summary>
    (int X, int Y) Location { get; }
    /// <summary>
    /// Gets if this is a floor tile.
    /// </summary>
    bool IsFloor { get; }
    /// <summary>
    /// Gets if this tile is blocked by an existing furni.
    /// </summary>
    bool IsBlocked { get; }
    /// <summary>
    /// Gets if this is a floor tile and is not blocked by an existing furni.
    /// </summary>
    bool IsFree { get; }
    /// <summary>
    /// Gets the height for this tile at which furni may be placed.
    /// </summary>
    double Height { get; }
}
