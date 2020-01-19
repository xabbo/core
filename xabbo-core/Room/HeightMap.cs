using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class HeightMap
    {
        public int Width { get; set; }
        public int Length { get; set; }

        public short[] Values { get; set; }

        public static double GetHeight(short value)
        {
            if (value < 0)
                return -1;
            return (value & 0x3FFF) / 256.0;
        }

        public static bool IsBlocked(short value)
        {
            return (value & 0x4000) > 0;
        }

        public static bool IsTile(short value)
        {
            return value >= 0;
        }

        public HeightMap() { }

        public double GetHeight(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return -1;
            return GetHeight(Values[y * Width + x]);
        }

        public bool IsBlocked(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return true;
            return IsBlocked(Values[y * Width + x]);
        }

        public bool IsTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return false;
            return IsTile(Values[y * Width + x]);
        }

        private HeightMap(Packet packet)
        {
            Width = packet.ReadInteger();
            int n = packet.ReadInteger();
            Length = n / Width;

            Values = new short[n];
            for (int i = 0; i < n; i++)
                Values[i] = packet.ReadShort();
        }

        public static HeightMap Parse(Packet packet) => new HeightMap(packet);

        public void Write(Packet packet)
        {
            if (Values == null)
                throw new NullReferenceException("Values cannot be null.");
            if (Values.Length != Width * Length)
                throw new FormatException($"The length of Values must be equal to Width * Height");

            packet.WriteInteger(Width);
            packet.WriteInteger(Width * Length);
            for (int i = 0; i < Values.Length; i++)
                packet.WriteShort(Values[i]);
        }
    }
}
