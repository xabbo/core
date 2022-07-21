using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a 3-dimensional location.
/// </summary>
public struct Tile : IComposable
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point XY => new(X, Y);
    public float Z { get; set; }

    public Tile(int x, int y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Tile(int x, int y)
        : this(x, y, 0)
    { }

    public void Compose(IPacket packet)
    {
        packet.WriteInt(X);
        packet.WriteInt(Y);
        packet.WriteFloatAsString(Z);
    }

    public override int GetHashCode() => (X, Y, Z).GetHashCode();

    public override bool Equals(object? obj) => obj switch
    {
        Tile x => Equals(x),
        Point x => Equals(x),
        ValueTuple<int, int> x => Equals(x),
        ValueTuple<int, int, int> x => Equals(x),
        ValueTuple<int, int, float> x => Equals(x),
        _ => false
    };

    public bool Equals(Point point) =>
        X == point.X && Y == point.Y;

    public bool Equals(Tile tile, float epsilon = XabboConst.DefaultEpsilon) =>
        X == tile.X && Y == tile.Y && Math.Abs(tile.Z - Z) < epsilon;

    public override string ToString() => $"{X},{Y},{Z:0.0###############}";

    public static Tile Parse(IReadOnlyPacket packet) => new(packet.ReadInt(), packet.ReadInt(), packet.ReadFloatAsString());

    public static bool TryParse(string format, out Tile tile)
    {
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

    public static Tile Parse(string format)
    {
        if (!TryParse(format, out Tile tile))
            throw new FormatException($"Invalid tile format: \"{format}\".");

        return tile;
    }

    public static bool operator ==(Tile a, Tile b) => a.Equals(b);
    public static bool operator !=(Tile a, Tile b) => !(a == b);

    public static bool operator ==(Tile tile, Point point) => tile.Equals(point);
    public static bool operator !=(Tile tile, Point point) => !tile.Equals(point);

    public static Tile operator +(Tile tile, Point offset) => new(tile.X + offset.X, tile.Y + offset.Y, tile.Z);
    public static Tile operator +(Tile a, Tile b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Tile operator -(Tile tile, Point offset) => new(tile.X - offset.X, tile.Y - offset.Y, tile.Z);
    public static Tile operator -(Tile a, Tile b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static implicit operator Tile((int X, int Y, float Z) location) => new(location.X, location.Y, location.Z);
    public static implicit operator (int X, int Y, float Z)(Tile tile) => (tile.X, tile.Y, tile.Z);

    public static implicit operator Point(Tile tile) => tile.XY;
    public static implicit operator (int X, int Y)(Tile tile) => (tile.X, tile.Y);
}
