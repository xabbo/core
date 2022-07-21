using System;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a wall location.
/// </summary>
public struct WallLocation : IComposable
{
    /// <summary>
    /// Represents a wall location with all coordinates at zero, and the orientation set to the left wall.
    /// </summary>
    public static readonly WallLocation Zero = new(0, 0, 0, 0, 'l');

    /// <summary>
    /// Gets the wall X coordinate.
    /// </summary>
    public int WX { get; set; }
    /// <summary>
    /// Gets the wall Y coordinate.
    /// </summary>
    public int WY { get; set; }
    /// <summary>
    /// Gets the location X coordinate.
    /// </summary>
    public int LX { get; set; }
    /// <summary>
    /// Gets the location Y coordinate.
    /// </summary>
    public int LY { get; set; }
    /// <summary>
    /// Gets the wall orientation.
    /// </summary>
    public WallOrientation Orientation { get; set; }

    /// <summary>
    /// Constructs a new wall location with the specified coordinates and orientation.
    /// </summary>
    public WallLocation(int wx, int wy, int lx, int ly, WallOrientation orientation)
    {
        WX = wx;
        WY = wy;
        LX = lx;
        LY = ly;
        Orientation = orientation;
    }

    /// <summary>
    /// Offsets the wall coordinates by the specified values
    /// attempting to keep the location fixed in place,
    /// relative to the original wall location.
    /// </summary>
    /// <param name="wxOffset">The amount to offset wall X by.</param>
    /// <param name="wyOffset">The amount to offset wall Y by.</param>
    /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
    public WallLocation Offset(int wxOffset, int wyOffset, int scale)
    {
        int halfTileWidth = scale / 2;

        return Add(
            wxOffset,
            wyOffset,
            -(wxOffset - wyOffset) * halfTileWidth,
            -(wxOffset + wyOffset) * halfTileWidth / 2
        );
    }

    /// <summary>
    /// Attempts to adjust all coordinates to a valid location using the room scale,
    /// and returns the updated wall location. Does not take into account
    /// the floor plan which affects the offset of the LY coordinate.
    /// </summary>
    /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
    public WallLocation Adjust(int scale)
    {
        int
            lx = LX,
            ly = LY,
            wx = WX,
            wy = WY;

        int halfTileWidth = scale / 2;
        int wallOffset = (lx / halfTileWidth);
        if (lx < 0) wallOffset -= 1;

        if (Orientation == WallOrientation.Left)
        {
            wy -= wallOffset;
            ly += wallOffset * halfTileWidth / 2;
        }
        else
        {
            wx += wallOffset;
            ly -= wallOffset * halfTileWidth / 2;
        }

        lx -= wallOffset * halfTileWidth;


        return new WallLocation(wx, wy, lx, ly, Orientation);
    }

    /// <summary>
    /// Flips the wall orientation between left and right, and returns the new wall location.
    /// </summary>
    public WallLocation Flip() => new WallLocation(WX, WY, LX, LY, Orientation.IsLeft ? WallOrientation.Right : WallOrientation.Left);

    /// <summary>
    /// Returns a new wall location with the specified orientation.
    /// </summary>
    public WallLocation Orient(WallOrientation orientation) => new WallLocation(WX, WY, LX, LY, orientation);

    /// <summary>
    /// Adds the specified offset values and returns the new wall location.
    /// </summary>
    public WallLocation Add(int wx, int wy, int lx, int ly) => new WallLocation(
        WX + wx, WY + wy,
        LX + lx, LY + ly,
        Orientation
    );

    /// <summary>
    /// Adds the specified location offset values and returns the new wall location.
    /// </summary>
    public WallLocation Add(int lxOffset, int lyOffset) => new WallLocation(
        WX, WY, LX + lxOffset, LY + lyOffset, Orientation
    );

    public override int GetHashCode() => (WX, WY, LX, LY, Orientation.Value).GetHashCode();

    public bool Equals(WallLocation other)
    {
        return
            WX == other.WX &&
            WY == other.WY &&
            LX == other.LX &&
            LY == other.LY &&
            Orientation == other.Orientation;
    }

    public override bool Equals(object? obj)
        => obj is WallLocation loc && Equals(loc);

    public override string ToString() => ToString(WX, WY, LX, LY, Orientation);
    public static string ToString(int wx, int wy, int lx, int ly, WallOrientation orientation) => $":w={wx},{wy} l={lx},{ly} {orientation.Value}";

    public void Compose(IPacket packet)
    {
        if (packet.Protocol == ClientType.Flash)
        {
            packet.WriteString(ToString());
        }
        else if (packet.Protocol == ClientType.Unity)
        {
            packet
                .WriteInt(WX)
                .WriteInt(WY)
                .WriteInt(LX)
                .WriteInt(LY)
                .WriteString(Orientation.Value.ToString());
        }
        else
        {
            throw new Exception("Unknown client protocol.");
        }
    }

    public static WallLocation Parse(IReadOnlyPacket packet) => Parse(packet.ReadString());

    public static WallLocation Parse(string locationString)
    {
        if (locationString is null)
            throw new ArgumentNullException(nameof(locationString));

        if (TryParse(locationString, out WallLocation wallLocation))
        {
            return wallLocation;
        }
        else
        {
            throw new FormatException("Wall location string is of an invalid format.");
        }
    }

    public static bool TryParse(string locationString, out WallLocation location)
    {
        location = default;

        if (locationString.IndexOf(':') != 0)
            return false;

        string[] parts = locationString.Split(' ');
        if (parts.Length != 3 ||
            parts[0].Length < 6 ||
            parts[1].Length < 5 ||
            parts[2].Length != 1 ||
            !parts[0].StartsWith(":w=") ||
            !parts[1].StartsWith("l="))
        {
            return false;
        }

        string[] positions = parts[0][3..].Split(',');
        if (positions.Length != 2) return false;
        if (!int.TryParse(positions[0], out int wallX)) return false;
        if (!int.TryParse(positions[1], out int wallY)) return false;

        positions = parts[1][2..].Split(',');
        if (positions.Length != 2) return false;
        if (!int.TryParse(positions[0], out int x)) return false;
        if (!int.TryParse(positions[1], out int y)) return false;

        WallOrientation orientation;
        switch (parts[2][0])
        {
            case 'l': orientation = WallOrientation.Left; break;
            case 'r': orientation = WallOrientation.Right; break;
            default: return false;
        }

        location = new WallLocation(wallX, wallY, x, y, orientation);
        return true;
    }

    public static bool operator ==(WallLocation a, WallLocation b) => a.Equals(b);
    public static bool operator !=(WallLocation a, WallLocation b) => !(a == b);

    public static WallLocation operator +(WallLocation location, (int X, int Y) offset)
        => location.Add(offset.X, offset.Y);
    public static WallLocation operator +(WallLocation location, (int WallX, int WallY, int X, int Y) offset)
        => location.Add(offset.WallX, offset.WallY, offset.X, offset.Y);
    public static WallLocation operator -(WallLocation location, (int X, int Y) offset)
        => location.Add(-offset.X, -offset.Y);
    public static WallLocation operator -(WallLocation location, (int WallX, int WallY, int X, int Y) offset)
        => location.Add(-offset.WallX, -offset.WallY, -offset.X, -offset.Y);

    public static implicit operator WallLocation(string s) => Parse(s);
}
