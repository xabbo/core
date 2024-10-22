using System.Diagnostics.CodeAnalysis;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a 2-dimensional location.
/// </summary>
public readonly struct Point(int x, int y) : IParserComposer<Point>
{
    public static readonly Point Zero = new(0, 0);
    public static readonly Point MinValue = new(int.MinValue, int.MinValue);
    public static readonly Point MaxValue = new(int.MaxValue, int.MaxValue);

    public readonly int X = x;
    public readonly int Y = y;

    /// <summary>
    /// Returns a new point with the X and Y coordinates flipped.
    /// </summary>
    public Point Flip() => new(Y, X);

    public override string ToString() => $"({X}, {Y})";

    public override int GetHashCode() => (X, Y).GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Point p && Equals(p);
    public bool Equals(Point p) => X == p.X && Y == p.Y;

    static Point IParser<Point>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt());

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(X);
        p.WriteInt(Y);
    }

    public static bool operator ==(Point a, Point b) => (a.X == b.X && a.Y == b.Y);
    public static bool operator !=(Point a, Point b) => !(a == b);
    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

    public static Point operator -(Point p) => new(-p.X, -p.Y);

    public static implicit operator Point((int X, int Y) xy) => new(xy.X, xy.Y);
    public static implicit operator Point(Tile tile) => new(tile.X, tile.Y);
}
