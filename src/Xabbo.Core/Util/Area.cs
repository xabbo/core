﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

/// <summary>
/// Represents an area in a room.
/// </summary>
public readonly struct Area : IEnumerable<Point>
{
    /// <summary>
    /// Gets the X coordinate of the origin.
    /// </summary>
    public readonly int X1;

    /// <summary>
    /// Gets the Y coordinate of the origin.
    /// </summary>
    public readonly int Y1;

    /// <summary>
    /// The width (on the X axis) of this area.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// The length (on the Y axis) of this area.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// Gets the size of this area.
    /// </summary>
    public Point Size => (Width, Length);

    /// <summary>
    /// The origin point of this area.
    /// </summary>
    public Point Origin => new(X1, Y1);

    /// <summary>
    /// The point opposite the origin of this area.
    /// </summary>
    public Point Opposite => new(X2, Y2);

    /// <summary>
    /// Gets the X coordinate of the corner opposite the origin.
    /// </summary>
    public int X2 => Origin.X + Width - 1;

    /// <summary>
    /// Gets the Y coordinate of the corner opposite the origin.
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

        X1 = origin.X;
        Y1 = origin.Y;
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

        X1 = x1;
        Y1 = y1;
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
    /// Gets whether the specified area is contained within this area.
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
    /// Gets whether specified floor entity is contained within this area.
    /// </summary>
    public bool Contains(IFloorEntity e) => Contains(e.Area);

    /// <summary>
    /// Gets whether the specified area intersects with this area.
    /// </summary>
    public bool Intersects(Area area)
    {
        return
            X1 <= area.X2 && Y1 <= area.Y2 &&
            X2 >= area.X1 && Y2 >= area.Y1;
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

    /// <summary>
    /// Implicitly casts the specified corner points to an area.
    /// </summary>
    /// <param name="corners"></param>
    public static implicit operator Area((Point A, Point B) corners)
        => new(corners.A, corners.B);

    /// <summary>
    /// Implicitly casts the specified points to an area.
    /// </summary>
    public static implicit operator Area((int X1, int Y1, int X2, int Y2) points)
        => new(points.X1, points.Y1, points.X2, points.Y2);

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
    public override string ToString() => $"({X1}, {Y1}, {X2}, {Y2})";
}
