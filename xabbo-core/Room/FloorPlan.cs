using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FloorPlan
    {
        public int Scale { get; set; }
        public int WallHeight { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        private int[] tiles;

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

        protected FloorPlan(Packet packet)
        {
            if (packet.ReadBoolean())
                Scale = 32;
            else
                Scale = 64;

            WallHeight = packet.ReadInteger();
            string map = packet.ReadString();

            string[] lines = map.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Length = lines.Length;
            Width = Length > 0 ? lines[0].Length : 0;
            for (int i = 0; i < Length; i++)
                if (lines[i].Length != Width)
                    throw new FormatException("Inconsistent width in floor plan string.");

            tiles = new int[Width * Length];
            for (int y = 0; y < Length; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[y * Width + x] = H.GetHeightFromCharacter(lines[y][x]);
                }
            }
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

        public static FloorPlan Parse(Packet packet) => new FloorPlan(packet);
    }
}
