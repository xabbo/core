using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IHeightmapTile"/>
public class HeightmapTile : IHeightmapTile, IComposer
{
    public Point Location { get; }
    public int X => Location.X;
    public int Y => Location.Y;
    public bool IsFloor { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsFree => IsFloor && !IsBlocked;
    public float Height { get; set; }

    /// <summary>
    /// Constructs a new heightmap tile with the specified coordinates and encoded value.
    /// </summary>
    /// <param name="location">The location of the tile.</param>
    /// <param name="value">The encoded value.</param>
    public HeightmapTile(Point location, short value)
    {
        Location = location;
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
