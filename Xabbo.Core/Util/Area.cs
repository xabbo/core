using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

/// <summary>
/// Represents an area in a 2-dimensional space.
/// </summary>
public struct Area : IEnumerable<Point>
{
    /// <summary>
    /// The origin point of this area.
    /// </summary>
    public Point Origin { get; set; }

    /// <summary>
    /// The width (on the X plane) of this area.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The length (on the Y plane) of this area.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets the size of this area.
    /// </summary>
    public Point Size => (Width, Length);

    /// <summary>
    /// Gets the X coordinate of the origin point.
    /// </summary>
    public int X1 => Origin.X;

    /// <summary>
    /// Gets the Y coordinate of the origin point.
    /// </summary>
    public int Y1 => Origin.Y;

    /// <summary>
    /// Gets the X coordinate of the end point.
    /// </summary>
    public int X2 => Origin.X + Width - 1;

    /// <summary>
    /// Gets the Y coordinate of the end point.
    /// </summary>
    public int Y2 => Origin.Y + Length - 1;

    /// <summary>
    /// Gets the corner opposite the origin point of this area.
    /// </summary>
    public Point Endpoint => Origin + (Width, Length);

    /// <summary>
    /// Constructs a new area at the specified point with the specified size.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The width or length is less than 1.</exception>
    public Area(Point origin, int width, int length)
    {
        if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
        if (length < 1) throw new ArgumentOutOfRangeException(nameof(length));

        Origin = origin;
        Width = width;
        Length = length;
    }

    /// <summary>
    /// Constructs a new area from the specified corner points.
    /// </summary>
    public Area(int x1, int y1, int x2, int y2)
    {
        if (x1 > x2) (x2, x1) = (x1, x2);
        if (y1 > y2) (y2, y1) = (y1, y2);

        Origin = new(x1, y1);
        Width = x2 - x1 + 1;
        Length = y2 - y1 + 1;
    }

    /// <summary>
    /// Constructs a new area from the specified corner points.
    /// </summary>
    public Area(Point a, Point b)
        : this(a.X, a.Y, b.X, b.Y)
    { }

    /// <summary>
    /// Constructs a new area with the specified dimensions.
    /// </summary>
    public Area(int width, int length)
        : this((0, 0), width, length)
    { }

    /// <summary>
    /// Returns a new area with the width and length reversed.
    /// </summary>
    public Area Flip() => new(Origin, Length, Width);

    /// <summary>
    /// Checks if this area contains the specified point.
    /// </summary>
    public bool Contains(Point point) => Contains(point.X, point.Y);

    /// <summary>
    /// Checks if this area contains the specified point.
    /// </summary>
    public bool Contains(int x, int y)
    {
        return
            X1 <= x && x <= X2 &&
            Y1 <= y && y <= Y2;
    }

    /// <summary>
    /// Gets if the specified area is contained within this area.
    /// </summary>
    public bool Contains(Area area)
    {
        return
            area.X1 >= X1 &&
            area.X2 <= X2 &&
            area.Y1 >= Y1 &&
            area.Y2 <= Y2;
    }

    /// <summary>
    /// Gets if this area contains the specified floor entity.
    /// </summary>
    public bool Contains(IFloorEntity e) => Contains(e.Area);

    /// <summary>
    /// Gets if this area intersects with the specified area.
    /// </summary>
    public bool Intersects(Area area)
    {
        return
            X1 <= area.X2 && Y1 <= area.Y2 &&
            X2 >= area.X1 && Y2 >= area.Y1;
    }

    /// <summary>
    /// Gets if this area intersects with the specified floor entity.
    /// </summary>
    public bool Intersects(IFloorEntity e) => Intersects(e.Area);

    /// <summary>
    /// Gets all X, Y coordinates within this area.
    /// </summary>
    private IEnumerable<Point> EnumeratePoints()
    {
        for (int y = Y1; y <= Y2; y++)
            for (int x = X1; x <= X2; x++)
                yield return new(x, y);
    }

    public IEnumerator<Point> GetEnumerator() => EnumeratePoints().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() => (X1, Y1, X2, Y2).GetHashCode();
    public override bool Equals(object? obj)
    {
        return obj is Area other && Equals(other);
    }

    public bool Equals(Area other)
    {
        return
            other.X1 == X1 &&
            other.Y1 == Y1 &&
            other.X2 == X2 &&
            other.Y2 == Y2;
    }

    public static bool operator ==(Area a, Area b) => a.Equals(b);
    public static bool operator !=(Area a, Area b) => !(a == b);

    public static implicit operator Area((Point A, Point B) corners)
        => new(corners.A, corners.B);

    public static implicit operator Area((int X1, int Y1, int X2, int Y2) points)
        => new(points.X1, points.Y1, points.X2, points.Y2);

    public static implicit operator Area((Point Origin, int Width, int Length) area)
        => new(area.Origin, area.Width, area.Length);

    public static List<Point> GetAllPoints(IEnumerable<Area> areas)
    {
        return areas
            .SelectMany(x => x)
            .Distinct()
            .OrderBy(x => x.Y)
            .ThenBy(x => x.X)
            .ToList();
    }

    public static List<Point> GetAllPoints(params Area[] areas) => GetAllPoints((IEnumerable<Area>)areas);

    public override string ToString() => $"({X1}, {Y1}, {X2}, {Y2})";
}
