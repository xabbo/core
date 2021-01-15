using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public interface IFloorPlan : IPacketData
    {
        string OriginalString { get; }

        int Scale { get; }
        int WallHeight { get; }
        int Width { get; }
        int Length { get; }
        int this[int x, int y] { get; }
        bool IsWalkable(int x, int y);
    }
}
