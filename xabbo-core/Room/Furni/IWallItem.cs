using System;

namespace Xabbo.Core
{
    public interface IWallItem : IFurni
    {
        IWallLocation Location { get; }
        int WallX { get; }
        int WallY { get; }
        int X { get; }
        int Y { get; }
        WallOrientation Orientation { get; }

        string Data { get; }
    }
}
