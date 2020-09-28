using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class HeightMap : IHeightMap, IWritable
    {
        public int Width { get; }
        public int Length { get; }
        public short[] Values { get; }
        IReadOnlyList<short> IHeightMap.Values => Values;

        public short this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x");
                if (y < 0 || y >= Length) throw new ArgumentOutOfRangeException("y");
                return Values[y * Width + x];
            }

            set
            {
                if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x");
                if (y < 0 || y >= Length) throw new ArgumentOutOfRangeException("y");
                Values[y * Width + x] = value;
            }
        }

        public HeightMap(int width, int length)
        {
            Width = width;
            Length = length;
            Values = new short[width * length];
        }

        private HeightMap(Packet packet)
        {
            Width = packet.ReadInt();
            int n = packet.ReadInt();
            Length = n / Width;

            Values = new short[n];
            for (int i = 0; i < n; i++)
                Values[i] = packet.ReadShort();
        }

        public double GetHeight(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return -1;

            short value = Values[y * Width + x];
            if (value < 0) return -1;

            return (value & 0x3FFF) / 256.0;
        }

        public bool IsBlocked(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return false;

            return (Values[y * Width + x] & 0x4000) > 0;
        }

        public bool IsTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return false;

            return Values[y * Width + x] >= 0;
        }

        public bool IsFree(int x, int y) => IsTile(x, y) && !IsBlocked(x, y);

        public static HeightMap Parse(Packet packet) => new HeightMap(packet);

        public void Write(Packet packet)
        {
            if (Values == null)
                throw new NullReferenceException("Values cannot be null.");
            if (Values.Length != Width * Length)
                throw new FormatException($"The length of Values must be equal to Width * Height");

            packet.WriteInt(Width);
            packet.WriteInt(Width * Length);
            for (int i = 0; i < Values.Length; i++)
                packet.WriteShort(Values[i]);
        }
    }
}
