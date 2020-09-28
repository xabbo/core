﻿using System;
using System.Text;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FloorPlan : IFloorPlan
    {
        public int Scale { get; set; }
        public int WallHeight { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        private readonly int[] tiles;

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
            Scale = 64;
            WallHeight = -1;

            tiles = ParseString(map, out int width, out int length);
            Width = width;
            Length = length;
        }

        protected FloorPlan(Packet packet)
        {
            if (packet.ReadBool())
                Scale = 32;
            else
                Scale = 64;

            WallHeight = packet.ReadInt();
            string map = packet.ReadString();

            tiles = ParseString(map, out int width, out int length);
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

        public void SetHeight(int x, int y, int height)
        {
            if (height < 0) throw new ArgumentOutOfRangeException("height");
            tiles[y * Width + x] = height;
        }

        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return false;
            return GetHeight(x, y) >= 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < Length; y++)
            {
                if (y > 0) sb.Append('\n');
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(H.GetCharacterFromHeight(tiles[y * Width + x]));
                }
            }

            return sb.ToString();
        }

        public static FloorPlan Parse(Packet packet) => new FloorPlan(packet);

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
