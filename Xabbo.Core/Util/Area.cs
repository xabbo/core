using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

public struct Area
{
    public int X1 { get; }
    public int Y1 { get; }
    public int X2 { get; }
    public int Y2 { get; }

    public int Width => X2 - X1 + 1;
    public int Length => Y2 - Y1 + 1;

    public (int X, int Y) A => (X1, Y1);
    public (int X, int Y) B => (X2, Y2);

    public Area(int x1, int y1, int x2, int y2)
    {
        if (x1 > x2)
        {
            int temp = x1;
            x1 = x2;
            x2 = temp;
        }

        if (y1 > y2)
        {
            int temp = y1;
            y1 = y2;
            y2 = temp;
        }

        X1 = x1; Y1 = y1;
        X2 = x2; Y2 = y2;
    }

    public Area((int X, int Y) pointA, (int X, int Y) pointB)
        : this(pointA.X, pointA.Y, pointB.X, pointB.Y)
    { }

    public Area((int X, int Y) point, int width, int length)
    {
        if (width < 1) throw new ArgumentOutOfRangeException("width");
        if (length < 1) throw new ArgumentOutOfRangeException("length");

        X1 = point.X;
        Y1 = point.Y;
        X2 = X1 + width - 1;
        Y2 = Y1 + length - 1;
    }

    public Area(int width, int length)
        : this((0, 0), width, length)
    { }

    /// <summary>
    /// Returns a new area with the width and length reversed.
    /// </summary>
    public Area Flip() => new Area(A, Length, Width);

    /// <summary>
    /// Returns a new area of the same size with its origin set at the specified coordinates.
    /// </summary>
    public Area Move(int originX, int originY) => new Area((originX, originY), Width, Length);

    /// <summary>
    /// Returns a new area of the same size with its origin set at the specified coordinates.
    /// </summary>
    public Area Move((int X, int Y) origin) => Move(origin.X, origin.Y);

    /// <summary>
    /// Returns a new area of the same size with its origin shifted by the specified amounts.
    /// </summary>
    public Area Shift(int offsetX, int offsetY) => new Area((X1 + offsetX, Y1 + offsetY), Width, Length);

    /// <summary>
    /// Returns a new area of the same size with its origin shifted by the specified amounts.
    /// </summary>
    public Area Shift((int X, int Y) offset) => Shift(offset.X, offset.Y);

    /// <summary>
    /// Gets all X/Y coordinates within this area.
    /// </summary>
    public IEnumerable<(int X, int Y)> EnumeratePoints()
    {
        for (int y = Y1; y <= Y2; y++)
            for (int x = X1; x <= X2; x++)
                yield return (x, y);
    }

    /// <summary>
    /// Gets if this area contains the specified tile.
    /// </summary>
    public bool Contains(Tile tile) => Contains(tile.X, tile.Y);

    /// <summary>
    /// Gets if this area contains the specified point.
    /// </summary>
    public bool Contains((int X, int Y) position) => Contains(position.X, position.Y);

    /// <summary>
    /// Gets if this area contains the specified point.
    /// </summary>
    public bool Contains(int x, int y)
    {
        return
            A.X <= x && x <= B.X &&
            A.Y <= y && y <= B.Y;
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
    /// Gets if the specified area intersects this area.
    /// </summary>
    public bool Intersects(Area area)
    {
        return
            X1 <= area.X2 &&
            Y1 <= area.Y2 &&
            X2 >= area.X1 &&
            Y2 >= area.Y1;
    }

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

    public static List<(int X, int Y)> GetAllPoints(IEnumerable<Area> areas)
    {
        return areas
            .SelectMany(x => x.EnumeratePoints())
            .Distinct()
            .OrderBy(x => x.Y)
            .ThenBy(x => x.X)
            .ToList();
    }

    public static List<(int X, int Y)> GetAllPoints(params Area[] areas) => GetAllPoints((IEnumerable<Area>)areas);
}
