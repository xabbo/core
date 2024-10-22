using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Heightmap : IHeightmap, IEnumerable<HeightmapTile>, IParserComposer<Heightmap>
{
    private readonly List<HeightmapTile> _tiles;

    public Point Size { get; }

    public Area Area => new(Size);

    public HeightmapTile this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Size.X) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Size.Y) throw new ArgumentOutOfRangeException(nameof(y));
            return _tiles[y * Size.X + x];
        }
    }
    public HeightmapTile this[Point point] => this[point.X, point.Y];
    IHeightmapTile IHeightmap.this[int x, int y] => this[x, y];
    IHeightmapTile IHeightmap.this[Point point] => this[point];

    public Heightmap(Point size)
    {
        Size = size;
        _tiles = [];
        for (int i = 0; i < Size.X * Size.Y; i++)
            _tiles.Add(new HeightmapTile((i % Size.X, i / Size.X), 0));
    }

    private Heightmap(in PacketReader p)
    {
        int width = p.ReadInt();
        int n = p.ReadLength();
        int length = n / width;

        Size = (width, length);

        _tiles = [];
        for (int i = 0; i < n; i++)
        {
            short value = p.ReadShort();
            _tiles.Add(new HeightmapTile((i % width, i / length), value));
        }
    }

    /// <summary>
    /// Gets whether a floor item that would occupy the specified area can likely be placed.
    /// </summary>
    /// <param name="area">The area that the floor item would occupy if placed.</param>
    /// <returns>Whether the item can likely be placed.</returns>
    public bool CanPlaceAt(Area area)
    {
        float? surfaceHeight = null;
        foreach (Point p in area)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Size.X || p.Y >= Size.Y)
                return false;
            var tile = this[p.X, p.Y];
            if (!tile.IsFree)
                return false;
            if (surfaceHeight is null)
                surfaceHeight = tile.Height;
            else if (!Tile.CompareZ(surfaceHeight.Value, tile.Height))
                return false;
        }
        return false;
    }

    /// <summary>
    /// Finds all potential locations a floor item of the specified size could be placed within the
    /// specified area. If the entry point it not null, it will be excluded from the placement area.
    /// </summary>
    /// <param name="area">The area to search for placeable locations.</param>
    /// <param name="size">The size of the floor item.</param>
    /// <param name="entry">The entry tile to optionally ignore.</param>
    /// <returns>
    /// All points within the specified area where a floor item of the specified size could likely
    /// be placed.
    /// </returns>
    public IEnumerable<Point> FindPlaceablePoints(Area area, Point size, Point? entry = null)
    {
        for (int y = area.Min.Y; y <= area.Max.Y - size.Y + 1; y++)
        {
            for (int x = area.Min.X; x <= area.Max.X - size.X + 1; x++)
            {
                if (entry == (x, y))
                    continue;
                var placementArea = new Area(size).At(x, y);
                if (!placementArea.Contains(entry) && CanPlaceAt(placementArea))
                    yield return (x, y);
            }
        }
    }

    public Point? FindPlaceablePoint(Point size, Point? entry = null)
        => FindPlaceablePoints(((0, 0), Size), size, entry).FirstOrDefault();

    public IEnumerator<HeightmapTile> GetEnumerator() => _tiles.GetEnumerator();

    IEnumerator<IHeightmapTile> IEnumerable<IHeightmapTile>.GetEnumerator() => _tiles.Cast<IHeightmapTile>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Size.X);
        p.WriteLength((Length)(Size.X * Size.Y));
        foreach (HeightmapTile tile in _tiles)
            p.Compose(tile);
    }

    static Heightmap IParser<Heightmap>.Parse(in PacketReader p) => new(in p);
}
