using System;

namespace Xabbo.Core
{
    public interface IFloorItem : IFurni
    {
        /// <summary>
        /// Gets the location of the floor item.
        /// </summary>
        Tile Location { get; }
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
        (int X, int Y) XY { get; }
        /// <summary>
        /// Gets the Z coordinate of the floor item.
        /// </summary>
        double Z { get; }
        /// <summary>
        /// Gets the XYZ coordinates of the floor item.
        /// </summary>
        (int X, int Y, double Z) XYZ { get; }
        /// <summary>
        /// Gets the direction of the floor item.
        /// </summary>
        int Direction { get; }
        /// <summary>
        /// Gets the height of the floor item.
        /// </summary>
        float Height { get; }
        /// <summary>
        /// Gets the extra field of the floor item.
        /// This may be the consumed state (of perishable items like cabbages),
        /// or if this is a teleporter, the linked teleporter ID.
        /// </summary>
        int Extra { get;  }
        /// <summary>
        /// Gets the data of the floor item.
        /// </summary>
        IItemData Data { get; }
    }
}
