using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a floor furniture.
/// </summary>
public interface IFloorItem : IFurni, IFloorEntity
{
    /// <summary>
    /// Gets the X coordinate of the floor item.
    /// </summary>
    int X { get; }

    /// <summary>
    /// Gets the Y coordinate of the floor item.
    /// </summary>
    int Y { get; }

    /// <summary>
    /// Gets the XY coordinates of the floor item.
    /// </summary>
    Point XY { get; }

    /// <summary>
    /// Gets the Z coordinate of the floor item.
    /// </summary>
    double Z { get; }

    /// <summary>
    /// The height of the floor item.
    /// </summary>
    float Height { get; }

    /// <summary>
    /// The extra field of the floor item.
    /// This may be the consumed state (of perishable items like cabbages),
    /// or if this is a teleporter, the linked teleporter ID.
    /// </summary>
    long Extra { get; }

    /// <summary>
    /// The data of the floor item.
    /// </summary>
    IItemData Data { get; }

    /// <summary>
    /// The floor item's colors.
    /// </summary>
    /// <remarks>
    /// Only available on Shockwave.
    /// </remarks>
    string Colors { get; }

    /// <summary>
    /// The floor item's runtime data.
    /// </summary>
    /// <remarks>
    /// Only available on Shockwave.
    /// </remarks>
    string RuntimeData { get; }
}
