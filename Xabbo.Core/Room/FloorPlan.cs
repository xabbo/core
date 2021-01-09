using System;
using System.Collections.Generic;
using System.Text;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FloorPlan : IFloorPlan
    {
        public string OriginalString { get; }

        public bool UseLegacyScale { get; set; }
        public int Scale => UseLegacyScale ? 32 : 64;
        public int WallHeight { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        private readonly int[] tiles;
        public IReadOnlyList<int> Tiles => tiles;

        public int this[int x, int y]
        {
            get => GetHeight(x, y);
            set => SetHeight(x, y, value);
        }

        public FloorPlan(int width, int length)
        {
            Width = width;
            Length = length;
        }

        protected FloorPlan(string map)
        {
            OriginalString = map;

            UseLegacyScale = false;
            WallHeight = -1;

            tiles = ParseString(map, out int width, out int length);
            Width = width;
            Length = length;
        }

        protected FloorPlan(IReadOnlyPacket packet)
        {
            UseLegacyScale = packet.ReadBool();

            WallHeight = packet.ReadInt();
            OriginalString = packet.ReadString();

            tiles = ParseString(OriginalString, out int width, out int length);
            Width = width;
            Length = length;
        }

        public int GetHeight(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Length)
                throw new ArgumentOutOfRangeException("y");

            return tiles[y * Width + x];
        }
        public int GetHeight((int X, int Y) location) => GetHeight(location.X, location.Y);
        public int GetHeight(Tile tile) => GetHeight(tile.X, tile.Y);

        public void SetHeight(int x, int y, int height)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Length)
                throw new ArgumentOutOfRangeException("y");
            if (height < -1)
                throw new ArgumentOutOfRangeException("height");

            tiles[y * Width + x] = height;
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

        public void Write(IPacket packet)
        {
            packet.WriteBool(UseLegacyScale);
            packet.WriteInt(WallHeight);
            packet.WriteString(ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Length; y++)
            {
                if (y > 0) sb.Append('\r');
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(H.GetCharacterFromHeight(tiles[y * Width + x]));
                }
            }

            return sb.ToString();
        }

        public static FloorPlan Parse(IReadOnlyPacket packet) => new FloorPlan(packet);

        public static FloorPlan Parse(string map) => new FloorPlan(map);

        private static int[] ParseString(string map, out int width, out int length)
        {
            string[] lines = map.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
}
