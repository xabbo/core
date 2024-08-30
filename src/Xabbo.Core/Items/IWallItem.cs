using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a wall furniture.
/// </summary>
public interface IWallItem : IFurni
{
    /// <summary>
    /// Gets the location of the wall item.
    /// </summary>
    WallLocation Location { get; }

    /// <summary>
    /// Gets the wall X coordinate of the wall item.
    /// </summary>
    int WX { get; }

    /// <summary>
    /// Gets the wall Y coordinate of the wall item.
    /// </summary>
    int WY { get; }

    /// <summary>
    /// Gets the X location of the wall item.
    /// </summary>
    int LX { get; }

    /// <summary>
    /// Gets the Y location of the wall item.
    /// </summary>
    int LY { get; }

    /// <summary>
    /// Gets the orientation of the wall item.
    /// </summary>
    WallOrientation Orientation { get; }

    /// <summary>
    /// Gets the data of the wall item.
    /// </summary>
    string Data { get; }
}
