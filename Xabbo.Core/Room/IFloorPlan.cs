using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public interface IFloorPlan : IComposable
    {
        /// <summary>
        /// Gets the original string that this floor plan was parsed from.
        /// </summary>
        string? OriginalString { get; }

        int Scale { get; }
        int WallHeight { get; }
        int Width { get; }
        int Length { get; }
        int this[int x, int y] { get; }
        bool IsWalkable(int x, int y);
    }
}
