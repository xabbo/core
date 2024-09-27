using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core;

/// <summary>
/// Represents a set of combined areas.
/// </summary>
public class AreaSet : ICollection<Area>
{
    private readonly HashSet<Area> _areas = new();

    public int Count => _areas.Count;
    public bool IsReadOnly => false;

    public IReadOnlyList<Point> Tiles { get; private set; } = Array.Empty<Point>();

    public AreaSet() { }

    public AreaSet(params Area[] areas)
        : this((IEnumerable<Area>)areas)
    { }

    public AreaSet(IEnumerable<Area> areas)
    {
        _areas = new HashSet<Area>(areas);
        UpdateTiles();
    }

    public bool Contains(Point point) => Contains(point.X, point.Y);
    public bool Contains(int x, int y) => _areas.Any(area => area.Contains(x, y));

    private void UpdateTiles() => Tiles = Area.GetAllPoints(_areas).AsReadOnly();

    public void Add(Area item)
    {
        if (_areas.Add(item))
            UpdateTiles();
    }

    public void Clear()
    {
        _areas.Clear();
        UpdateTiles();
    }

    public bool Contains(Area item) => _areas.Contains(item);

    public void CopyTo(Area[] array, int arrayIndex) => _areas.CopyTo(array, arrayIndex);

    public IEnumerator<Area> GetEnumerator() => _areas.GetEnumerator();

    public bool Remove(Area item)
    {
        bool removed = _areas.Remove(item);
        if (removed) UpdateTiles();
        return removed;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Creates a new AreaSet consisting of the specified areas.
    /// </summary>
    public static AreaSet Of(params Area[] areas) => new(areas);
}
