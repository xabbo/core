using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a 3-dimensional location.
/// </summary>
public readonly struct Tile(int x, int y, float z) : IParserComposer<Tile>
{
    public readonly int X = x;
    public readonly int Y = y;
    public readonly float Z = z;

    public Point XY => new(X, Y);

    public Tile(Point xy, float z) : this(xy.X, xy.Y, z) { }
    public Tile(Point xy) : this(xy.X, xy.Y) { }
    public Tile(int x, int y) : this(x, y, 0) { }

    public readonly override int GetHashCode() => (X, Y, Z).GetHashCode();

    public readonly override bool Equals(object? obj) => obj switch
    {
        Tile x => Equals(x),
        Point x => Equals(x),
        ValueTuple<int, int> x => Equals(x),
        ValueTuple<int, int, int> x => Equals(x),
        ValueTuple<int, int, float> x => Equals(x),
        _ => false
    };

    public readonly bool Equals(Point point) =>
        X == point.X && Y == point.Y;

    public readonly bool Equals(Tile tile, float epsilon = XabboConst.DefaultEpsilon) =>
        X == tile.X && Y == tile.Y && CompareZ(tile.Z, Z, epsilon);

    public readonly override string ToString() => $"({X}, {Y}, {Z:0.0#######})";

    public readonly void Compose(in PacketWriter p)
    {
        p.WriteInt(X);
        p.WriteInt(Y);
        p.WriteFloat(Z);
    }

    static Tile IParser<Tile>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt(), p.ReadFloat());

    public static bool TryParseString(string format, out Tile tile)
    {
        if (format.StartsWith('(') &&
            format.EndsWith(')'))
        {
            format = format[1..^1];
        }

        tile = default;
        string[] split = format.Split(',');
        if (split.Length != 3 ||
            !int.TryParse(split[0], out int x) ||
            !int.TryParse(split[1], out int y) ||
            !float.TryParse(split[2], out float z))
        {
            return false;
        }

        tile = new(x, y, z);
        return true;
    }

    public static Tile ParseString(string format)
    {
        if (!TryParseString(format, out Tile tile))
            throw new FormatException($"Invalid tile format: \"{format}\".");

        return tile;
    }

    /// <summary>
    /// Compares two Z coordinates and returns whether they are close enough to each other to be
    /// considered equivalent when two furni are placed at either coordinate.
    /// </summary>
    public static bool CompareZ(float z1, float z2, float epsilon = XabboConst.DefaultEpsilon) => Math.Abs(z1 - z2) < epsilon;

    public static bool operator ==(Tile a, Tile b) => a.Equals(b);
    public static bool operator !=(Tile a, Tile b) => !(a == b);

    public static bool operator ==(Tile tile, Point point) => tile.Equals(point);
    public static bool operator !=(Tile tile, Point point) => !tile.Equals(point);

    public static bool operator ==(Point point, Tile tile) => tile.Equals(point);
    public static bool operator !=(Point point, Tile tile) => !tile.Equals(point);

    public static Tile operator +(Tile tile, Point offset) => new(tile.X + offset.X, tile.Y + offset.Y, tile.Z);
    public static Tile operator +(Tile a, Tile b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Tile operator -(Tile tile, Point offset) => new(tile.X - offset.X, tile.Y - offset.Y, tile.Z);
    public static Tile operator -(Tile a, Tile b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static implicit operator Tile((int X, int Y, float Z) location) => new(location.X, location.Y, location.Z);
    public static implicit operator Tile((int X, int Y, double Z) location) => new(location.X, location.Y, (float)location.Z);
}
