using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class HeightmapTile : IHeightmapTile, IComposer
{
    public int X { get; }
    public int Y { get; }
    public (int X, int Y) Location => (X, Y);

    public bool IsFloor { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsFree => IsFloor && !IsBlocked;
    public double Height { get; set; }

    public HeightmapTile(int x, int y, short value)
    {
        X = x;
        Y = y;

        Update(value);
    }

    public void Update(short value)
    {
        IsFloor = value >= 0;
        IsBlocked = (value & 0x4000) != 0;
        Height = value >= 0 ? ((value & 0x3FFF) / 256.0) : -1;
    }

    public void Compose(in PacketWriter p)
    {
        p.Write((short)(
            (IsFloor ? 0x0000 : 0x8000) |
            (IsBlocked ? 0x4000 : 0x0000) |
            ((int)(Height * 256.0) & 0x3FFF)
        ));
    }
}
