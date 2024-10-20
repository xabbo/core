using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IHeightmapTile"/>
public class HeightmapTile : IHeightmapTile, IComposer
{
    public int X { get; }
    public int Y { get; }
    public Point Location => (X, Y);
    public bool IsFloor { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsFree => IsFloor && !IsBlocked;
    public float Height { get; set; }

    /// <summary>
    /// Constructs a new heightmap tile with the specified coordinates and encoded value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value">The encoded value.</param>
    public HeightmapTile(int x, int y, short value)
    {
        X = x;
        Y = y;

        Update(value);
    }

    /// <summary>
    /// Updates the tile with the specified encoded value.
    /// </summary>
    /// <param name="value">The encoded value.</param>
    public void Update(short value)
    {
        IsFloor = value >= 0;
        IsBlocked = (value & 0x4000) != 0;
        Height = value >= 0 ? ((value & 0x3FFF) / 256f) : -1;
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteShort((short)(
            (IsFloor ? 0x0000 : 0x8000) |
            (IsBlocked ? 0x4000 : 0x0000) |
            ((int)(Height * 256) & 0x3FFF)
        ));
    }
}
