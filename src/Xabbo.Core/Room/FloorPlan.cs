using System;
using System.Collections.Generic;
using System.Text;

using Xabbo.Messages;

namespace Xabbo.Core;

public class FloorPlan : IFloorPlan, IParserComposer<FloorPlan>
{
    private static readonly char[] _newlineChars = ['\r', '\n'];

    public string? OriginalString { get; private set; }

    public bool UseLegacyScale { get; set; }
    public int Scale => UseLegacyScale ? 32 : 64;
    public int WallHeight { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }

    private readonly int[] _tiles;
    public IReadOnlyList<int> Tiles => _tiles;

    public int this[int x, int y]
    {
        get => GetHeight(x, y);
        set => SetHeight(x, y, value);
    }

    public int this[Point point]
    {
        get => this[point.X, point.Y];
        set => this[point.X, point.Y] = value;
    }

    public FloorPlan(int width, int length)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        Width = width;
        Length = length;

        _tiles = new int[width * length];
    }

    public FloorPlan(string map)
    {
        OriginalString = map;

        UseLegacyScale = false;
        WallHeight = -1;

        _tiles = ParseString(map, out int width, out int length);
        Width = width;
        Length = length;
    }

    public int GetHeight(int x, int y)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x));
        if (y < 0 || y >= Length)
            throw new ArgumentOutOfRangeException(nameof(y));

        return _tiles[y * Width + x];
    }

    public int GetHeight((int X, int Y) location) => GetHeight(location.X, location.Y);

    public int GetHeight(Tile tile) => GetHeight(tile.X, tile.Y);

    public void SetHeight(int x, int y, int height)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x));
        if (y < 0 || y >= Length)
            throw new ArgumentOutOfRangeException(nameof(y));
        if (height < -1)
            throw new ArgumentOutOfRangeException(nameof(height));

        _tiles[y * Width + x] = height;
    }

    public void SetHeight((int X, int Y) location, int height) => SetHeight(location.X, location.Y, height);

    public void SetHeight(Tile location, int height) => SetHeight(location.X, location.Y, height);

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Length)
            return false;

        return GetHeight(x, y) >= 0;
    }

    public bool IsWalkable((int X, int Y) location) => IsWalkable(location.X, location.Y);

    public bool IsWalkable(Tile location) => IsWalkable(location.X, location.Y);

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int y = 0; y < Length; y++)
        {
            if (y > 0) sb.Append('\r');
            for (int x = 0; x < Width; x++)
            {
                sb.Append(H.GetCharacterFromHeight(_tiles[y * Width + x]));
            }
        }

        return sb.ToString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client != ClientType.Shockwave)
        {
            p.WriteBool(UseLegacyScale);
            p.WriteInt(WallHeight);
            p.WriteString(ToString());
        }
        else
        {
            p.WriteContent(ToString());
        }
    }

    static FloorPlan IParser<FloorPlan>.Parse(in PacketReader p)
    {
        bool useLegacyScale = false;
        int wallHeight = 0;

        if (p.Client != ClientType.Shockwave)
        {
            useLegacyScale = p.ReadBool();
            wallHeight = p.ReadInt();
        }

        string map = p.Client switch
        {
            ClientType.Shockwave => p.ReadContent(),
            _ => p.ReadString()
        };

        return new FloorPlan(map)
        {
            UseLegacyScale = useLegacyScale,
            WallHeight = wallHeight
        };
    }

    public static FloorPlan ParseString(string map) => new(map);

    private static int[] ParseString(string map, out int width, out int length)
    {
        string[] lines = map.Split(_newlineChars, StringSplitOptions.RemoveEmptyEntries);
        length = lines.Length;
        width = 0;
        for (int i = 0; i < length; i++)
        {
            if (lines[i].Length > width)
                width = lines[i].Length;
        }

        int[] tiles = new int[width * length];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = -1;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char c = lines[y][x];
                if (c != 'x' && c != 'X')
                {
                    tiles[y * width + x] = H.GetHeightFromCharacter(lines[y][x]);
                }
            }
        }

        return tiles;
    }
}
