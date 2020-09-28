using System;

namespace Xabbo.Core
{
    public interface IWallLocation
    {
        int WallX { get; }
        int WallY { get; }
        int X { get; }
        int Y { get; }
        WallOrientation Orientation { get; }
    }
}
