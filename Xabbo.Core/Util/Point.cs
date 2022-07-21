using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core;

/// <summary>
/// Represents a 2-dimensional location.
/// </summary>
public struct Point
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public Point() { }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override int GetHashCode() => (X, Y).GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Point p && Equals(p);
    public bool Equals(Point p) => X == p.X && Y == p.Y;

    public static bool operator ==(Point a, Point b) => (a.X == b.X && a.Y == b.Y);
    public static bool operator !=(Point a, Point b) => !(a == b);
    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

    public static implicit operator Point((int X, int Y) tuple) => new(tuple.X, tuple.Y);
    public static implicit operator (int X, int Y)(Point point) => (point.X, point.Y);
}
