using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Heightmap : IHeightMap
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

        public Heightmap(int width, int length)
        {
            Width = width;
            Length = length;
            Values = new short[width * length];
        }

        private Heightmap(IReadOnlyPacket packet)
        {
            Width = packet.ReadInt();
            int n = packet.ReadShort();
            Length = n / Width;

            Values = new short[n];
            for (int i = 0; i < n; i++)
                Values[i] = packet.ReadShort();
        }

        public double GetHeight(int x, int y)
        {
            short value = Values[y * Width + x];
            if (value < 0) return -1;

            return (value & 0x3FFF) / 256.0;
        }
        public double GetHeight((int X, int Y) location) => GetHeight(location.X, location.Y);

        public void SetHeight(int x, int y, double height)
        {
            if (height < 0 || height > 63)
                throw new ArgumentOutOfRangeException("height", "Height must be from 0 - 63.");

            this[x ,y] &= ~0x3FFF;
            this[x, y] |= (short)(height * 256.0);
        }
        public void SetHeight((int X, int Y) location, double height) => SetHeight(location.X, location.Y, height);

        public bool IsBlocked(int x, int y) => (this[x, y] & 0x4000) > 0;
        public bool IsBlocked((int X, int Y) location) => IsBlocked(location.X, location.Y);

        public void SetBlocked(int x, int y, bool isBlocked)
        {
            if (isBlocked)
                this[x, y] |= 0x4000;
            else
                this[x, y] &= ~0x4000;
        }

        public bool IsTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return false;

            return Values[y * Width + x] >= 0;
        }
        public bool IsTile((int X, int Y) location) => IsTile(location.X, location.Y);

        public void SetIsTile(int x, int y, bool isTile)
        {
            unchecked
            {
                if (isTile)
                    this[x, y] &= (short)~0x8000;
                else
                    this[x, y] |= (short)0x8000;
            }
        }
        public void SetIsTile((int X, int Y) location, bool isTile) => SetIsTile(location.X, location.Y, isTile);

        public bool IsFree(int x, int y) => IsTile(x, y) && !IsBlocked(x, y);
        public bool IsFree((int X, int Y) location) => IsFree(location.X, location.Y);

        public static Heightmap Parse(IReadOnlyPacket packet) => new Heightmap(packet);

        public void Write(IPacket packet)
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
