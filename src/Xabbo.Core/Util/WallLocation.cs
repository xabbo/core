using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a wall item location.
/// </summary>
/// <param name="Wall">The wall coordinates.</param>
/// <param name="Offset">The location coordinates.</param>
/// <param name="Orientation">The wall orientation.</param>
public readonly record struct WallLocation(Point Wall, Point Offset, WallOrientation Orientation) : IParserComposer<WallLocation>
{
    /// <summary>
    /// Represents a wall location with all coordinates at zero, and the orientation set to the left wall.
    /// </summary>
    public static readonly WallLocation Zero = new(0, 0, 0, 0, 'l');

    /// <summary>
    /// Constructs a new wall location with the specified coordinates and orientation.
    /// </summary>
    public WallLocation(int wx, int wy, int lx, int ly, WallOrientation orientation)
        : this((wx, wy), (lx, ly), orientation)
    { }

    /// <summary>
    /// Offsets the wall coordinates by the specified values
    /// attempting to keep the location fixed in place,
    /// relative to the original wall location.
    /// </summary>
    /// <param name="wallOffset">The amount to offset the wall coordaintes by.</param>
    /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
    public readonly WallLocation OffsetWall(Point wallOffset, int scale)
    {
        int halfTileWidth = scale / 2;

        return Add(
            wallOffset,
            new Point(
                -(wallOffset.X - wallOffset.Y) * halfTileWidth,
                -(wallOffset.X + wallOffset.Y) * halfTileWidth / 2
            )
        );
    }

    /// <summary>
    /// Attempts to adjust all coordinates to a valid location using the room scale, and returns the updated wall location.
    /// This does not take into account the floor plan, which affects the Location's Y coordinate depending on the wall height and tile location.
    /// </summary>
    /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
    public readonly WallLocation Adjust(int scale)
    {
        Point wall = Wall, loc = Offset;

        int halfTileWidth = scale / 2;
        int wallOffset = loc.X / halfTileWidth;
        if (loc.X < 0) wallOffset -= 1;

        if (Orientation == WallOrientation.Left)
        {
            wall -= (0, wallOffset);
            loc += (0, wallOffset * halfTileWidth / 2);
        }
        else
        {
            wall += (wallOffset, 0);
            loc -= (0, wallOffset * halfTileWidth / 2);
        }

        loc -= (wallOffset * halfTileWidth, 0);

        return new WallLocation(wall, loc, Orientation);
    }

    /// <summary>
    /// Flips the wall orientation between left and right, and returns the new wall location.
    /// </summary>
    public readonly WallLocation Flip() => this with { Orientation = Orientation.Opposite };

    /// <summary>
    /// Returns a new wall location with the specified orientation.
    /// </summary>
    public readonly WallLocation Orient(WallOrientation orientation) => this with { Orientation = orientation };

    /// <summary>
    /// Adds the specified offset values and returns the new wall location.
    /// </summary>
    public readonly WallLocation Add(Point wall, Point offset) => this with
    {
        Wall = Wall + wall,
        Offset = offset + offset
    };

    /// <summary>
    /// Adds the specified location offset values and returns the new wall location.
    /// </summary>
    public WallLocation Add(Point offset) => this with { Offset = Offset + offset };

    public readonly override string ToString() => FormatString(Wall.X, Wall.Y, Offset.X, Offset.Y, Orientation);

    public static string FormatString(int wx, int wy, int lx, int ly, WallOrientation orientation) => $":w={wx},{wy} l={lx},{ly} {orientation.Value}";

    public readonly void Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Flash or ClientType.Shockwave:
                p.WriteString(ToString());
                break;
            case ClientType.Unity:
                p.WriteInt(Wall.X);
                p.WriteInt(Wall.Y);
                p.WriteInt(Offset.X);
                p.WriteInt(Offset.Y);
                p.WriteString(Orientation);
                break;
            default:
                throw new Exception("Unknown client protocol.");
        }
    }

    static WallLocation IParser<WallLocation>.Parse(in PacketReader p) => ParseString(p.ReadString());

    public static WallLocation ParseString(string locationString)
    {
        ArgumentNullException.ThrowIfNull(locationString);

        if (TryParse(locationString, out WallLocation wallLocation))
        {
            return wallLocation;
        }
        else
        {
            throw new FormatException($"Invalid wall location format: '{locationString}'.");
        }
    }

    public static bool TryParse(string locationString, out WallLocation location)
    {
        location = default;

        if (!locationString.StartsWith(':'))
            return false;

        string[] parts = locationString.Split(' ');
        if (parts.Length < 3 ||
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

    public static WallLocation operator +(WallLocation location, Point offset) => location.Add(offset);
    public static WallLocation operator +(WallLocation location, (int X, int Y) offset) => location.Add(offset);
    public static WallLocation operator +(WallLocation location, (Point Wall, Point Offset) offset) => location.Add(offset.Wall, offset.Offset);

    public static WallLocation operator -(WallLocation location, Point offset) => location.Add(-offset, Point.Zero);
    public static WallLocation operator -(WallLocation location, (int X, int Y) offset) => location.Add((-offset.X, -offset.Y), Point.Zero);

    public static implicit operator WallLocation(string s) => ParseString(s);
    public static implicit operator WallLocation((Point Wall, Point Offset, WallOrientation Orientation) x) => new(x.Wall, x.Offset, x.Orientation);
}
