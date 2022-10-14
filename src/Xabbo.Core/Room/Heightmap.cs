using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public class Heightmap : IHeightmap, IEnumerable<HeightmapTile>
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
    public HeightmapTile this[(int X, int Y) location] => this[location.X, location.Y];
    IHeightmapTile IHeightmap.this[int x, int y] => this[x, y];
    IHeightmapTile IHeightmap.this[(int X, int Y) location] => this[location];

    public Heightmap(int width, int length)
    {
        Width = width;
        Length = length;
        _tiles = new List<HeightmapTile>();
        for (int i = 0; i < width * length; i++)
            _tiles.Add(new HeightmapTile(i % width, i / width, 0));
    }

    private Heightmap(IReadOnlyPacket packet)
    {
        Width = packet.ReadInt();
        short n = packet.ReadLegacyShort();
        Length = n / Width;

        _tiles = new List<HeightmapTile>();
        for (int i = 0; i < n; i++)
        {
            short value = packet.ReadShort();
            _tiles.Add(new HeightmapTile(i % Width, i / Width, value));
        }
    }

    public void Compose(IPacket packet)
    {
        packet
            .WriteInt(Width)
            .WriteLegacyShort((short)(Width * Length));

        foreach (HeightmapTile tile in _tiles)
        {
            tile.Compose(packet);
        }
    }

    public static Heightmap Parse(IReadOnlyPacket packet) => new Heightmap(packet);


    public IEnumerator<HeightmapTile> GetEnumerator() => _tiles.GetEnumerator();

    IEnumerator<IHeightmapTile> IEnumerable<IHeightmapTile>.GetEnumerator() => _tiles.Cast<IHeightmapTile>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
