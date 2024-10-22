using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

/// <summary>
/// Represents an area in a room.
/// </summary>
public readonly record struct Area : IEnumerable<Point>
{
    public static readonly Area Empty = new() { Min = Point.MaxValue, Max = Point.MinValue };

    /// <summary>
    /// Gets the point with the minimum X and Y coordinates of the area.
    /// </summary>
    public Point Min { get; private init; }

    /// <summary>
    /// Gets the point with the maximum X and Y coordinates of the area.
    /// </summary>
    public Point Max { get; private init; }

    /// <summary>
    /// Gets the size of this area.
    /// </summary>
    public Point Size => (Max.X - Min.X + 1, Max.Y - Min.Y + 1);

    /// <summary>
    /// Constructs a new area at the specified point with the specified size.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The width or length is less than 1.</exception>
    public Area(Point origin, int width, int length)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 1);

        Min = origin;
        Max = (origin.X + width - 1, origin.Y + length - 1);
    }

    /// <summary>
    /// Constructs a new area from the specified corner points.
    /// </summary>
    public Area(int x1, int y1, int x2, int y2, bool normalize = true)
    {
        if (normalize)
        {
            if (x1 > x2) (x2, x1) = (x1, x2);
            if (y1 > y2) (y2, y1) = (y1, y2);
        }

        if (x1 > x2 || y1 > y2)
        {
            Min = Empty.Min;
            Max = Empty.Max;
        }
        else
        {
            Min = (x1, y1);
            Max = (x2, y2);
        }
    }

    /// <summary>
    /// Constructs a new area from the specified corner points.
    /// </summary>
    public Area(Point a, Point b)
        : this(a.X, a.Y, b.X, b.Y)
    { }

    /// <summary>
    /// Constructs a new area with the specified size.
    /// </summary>
    public Area(int width, int length)
        : this((0, 0), width, length)
    { }

    /// <summary>
    /// Constructs a new area with the specified size.
    /// </summary>
    public Area(Point size)
        : this((0, 0), size.X, size.Y)
    { }

    /// <summary>
    /// Returns a new area translated to the specified coordinates.
    /// </summary>
    public Area At(int x, int y) => new((x, y), Size.X, Size.Y);

    /// <summary>
    /// Returns a new area translated to the specified coordinates.
    /// </summary>
    public Area At(Point location) => new(location, Size.X, Size.Y);

    /// <summary>
    /// Returns a new area with the width and length reversed.
    /// </summary>
    public Area Flip() => new(Min, Size.Y, Size.X);

    /// <summary>
    /// Checks if this area contains the specified point.
    /// </summary>
    public bool Contains(Point? point) => point is { X: int x, Y: int y } && Contains(x, y);

    /// <summary>
    /// Checks if this area contains the specified point.
    /// </summary>
    public bool Contains(int x, int y)
    {
        return
            Min.X <= x && x <= Max.X &&
            Min.Y <= y && y <= Max.Y;
    }

    /// <summary>
    /// Gets whether another area is contained within this area.
    /// </summary>
    public bool Contains(Area? other)
    {
        return
            other is { Min: Point otherMin, Max: Point otherMax } &&
            otherMin.X >= Min.X && otherMax.X <= Max.X &&
            otherMin.Y >= Min.Y && otherMax.Y <= Max.Y;
    }

    /// <summary>
    /// Gets whether specified floor entity is contained within this area.
    /// </summary>
    public bool Contains(IFloorEntity e) => Contains(e.Area);

    /// <summary>
    /// Gets whether the specified area intersects with this area.
    /// </summary>
    public bool Intersects(Area? other)
    {
        return
            other is { Min: Point otherMin, Max: Point otherMax } &&
            otherMax.X >= Min.X && otherMin.X <= Max.X &&
            otherMax.Y >= Min.Y && otherMin.Y <= Max.Y;
    }

    /// <summary>
    /// Gets whether the specified area contains this area.
    /// </summary>
    /// <param name="other">The containing area to check whether this area is inside.</param>
    public bool IsInside(Area? other)
    {
        return
            other is { Min: Point otherMin, Max: Point otherMax } &&
            Min.X >= otherMin.X && Max.X <= otherMax.X &&
            Min.Y >= otherMin.Y && Max.Y <= otherMax.Y;
    }

    /// <summary>
    /// Gets whether the specified floor entity intersects with this area.
    /// </summary>
    public bool Intersects(IFloorEntity e) => Intersects(e.Area);

    /// <summary>
    /// Gets all tile coordinates contained in this area.
    /// </summary>
    private IEnumerable<Point> EnumeratePoints()
    {
        for (int y = Min.Y; y <= Max.Y; y++)
            for (int x = Min.X; x <= Max.X; x++)
                yield return new(x, y);
    }

    public IEnumerator<Point> GetEnumerator() => EnumeratePoints().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Implicitly casts the specified corner points to an area.
    /// </summary>
    /// <param name="corners"></param>
    public static implicit operator Area((Point A, Point B) corners)
        => new(corners.A, corners.B);

    /// <summary>
    /// Implicitly casts the specified points to an area.
    /// </summary>
    public static implicit operator Area((int X1, int Y1, int X2, int Y2) coords)
        => new(coords.X1, coords.Y1, coords.X2, coords.Y2);

    /// <summary>
    /// Implicitly casts the specified origin point, width, and length to an area.
    /// </summary>
    /// <param name="area"></param>
    public static implicit operator Area((Point Origin, int Width, int Length) area)
        => new(area.Origin, area.Width, area.Length);

    /// <summary>
    /// Gets all points contained within the specified area.
    /// </summary>
    public static List<Point> GetAllPoints(IEnumerable<Area> areas)
    {
        return areas
            .SelectMany(x => x)
            .Distinct()
            .OrderBy(x => x.Y)
            .ThenBy(x => x.X)
            .ToList();
    }

    /// <summary>
    /// Gets all distinct points contained within the specified areas.
    /// </summary>
    public static List<Point> GetAllPoints(params Area[] areas) => GetAllPoints((IEnumerable<Area>)areas);

    /// <summary>
    /// Gets a string representation of this area.
    /// </summary>
    public override string ToString() => $"({Min.X}, {Min.Y}, {Max.X}, {Max.Y})";

    /// <summary>
    /// Gets the intersection of two areas.
    /// </summary>
    /// <param name="a">The first area.</param>
    /// <param name="b">The second area.</param>
    /// <returns>The intersection of the two areas.</returns>
    public static Area Intersection(Area a, Area b) => new(
        Math.Max(a.Min.X, b.Min.X),
        Math.Max(a.Min.Y, b.Min.Y),
        Math.Min(a.Max.X, b.Max.X),
        Math.Min(a.Max.Y, b.Max.Y),
        false
    );
}
