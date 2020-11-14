using System;

namespace Xabbo.Core
{
    public interface IWallItem : IFurni
    {
        /// <summary>
        /// Gets the location of the wall item.
        /// </summary>
        WallLocation Location { get; }
        /// <summary>
        /// Gets the wall X coordinate of the wall item.
        /// </summary>
        int WallX { get; }
        /// <summary>
        /// Gets the wall Y coordinate of the wall item.
        /// </summary>
        int WallY { get; }
        /// <summary>
        /// Gets the X location of the wall item.
        /// </summary>
        int X { get; }
        /// <summary>
        /// Gets the Y location of the wall item.
        /// </summary>
        int Y { get; }
        /// <summary>
        /// Gets the orientation of the wall item.
        /// </summary>
        WallOrientation Orientation { get; }
        /// <summary>
        /// Gets the data of the wall item.
        /// </summary>
        string Data { get; }
    }
}
