using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Heightmap : IHeightmap, IEnumerable<HeightmapTile>, IParserComposer<Heightmap>
{
    private readonly List<HeightmapTile> _tiles;

    public int Width { get; }
    public int Length { get; }

    public HeightmapTile this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Length) throw new ArgumentOutOfRangeException(nameof(y));
            return _tiles[y * Width + x];
        }
    }
    public HeightmapTile this[Point point] => this[point.X, point.Y];
    IHeightmapTile IHeightmap.this[int x, int y] => this[x, y];
    IHeightmapTile IHeightmap.this[Point point] => this[point];

    public Heightmap(int width, int length)
    {
        Width = width;
        Length = length;
        _tiles = new List<HeightmapTile>();
        for (int i = 0; i < width * length; i++)
            _tiles.Add(new HeightmapTile(i % width, i / width, 0));
    }

    private Heightmap(in PacketReader p)
    {
        Width = p.ReadInt();
        int n = p.ReadLength();
        Length = n / Width;

        _tiles = [];
        for (int i = 0; i < n; i++)
        {
            short value = p.ReadShort();
            _tiles.Add(new HeightmapTile(i % Width, i / Width, value));
        }
    }

    public IEnumerator<HeightmapTile> GetEnumerator() => _tiles.GetEnumerator();

    IEnumerator<IHeightmapTile> IEnumerable<IHeightmapTile>.GetEnumerator() => _tiles.Cast<IHeightmapTile>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Width);
        p.WriteLength(Width * Length);
        foreach (HeightmapTile tile in _tiles)
            p.Compose(tile);
    }

    static Heightmap IParser<Heightmap>.Parse(in PacketReader p) => new(in p);
}
