using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core
{
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
                _tiles.Add(new HeightmapTile(i % width, i / width));
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

        private static bool IsTile(short value) => value >= 0;
        private static bool IsBlocked(short value) => (value & 0x4000) > 0;
        private static double GetHeight(short value)
        {
            if (value < 0) return -1;
            else return (value & 0x3FFF) / 256.0;
        }

        /*public void SetHeight(int x, int y, double height)
        {
            if (height < 0 || height > 63)
                throw new ArgumentOutOfRangeException("height", "Height must be from 0 - 63.");

            this[x ,y] &= ~0x3FFF;
            this[x, y] |= (short)(height * 256.0);
        }
        public void SetHeight((int X, int Y) location, double height) => SetHeight(location.X, location.Y, height);*/

        /*public void SetBlocked(int x, int y, bool isBlocked)
        {
            if (isBlocked)
                this[x, y] |= 0x4000;
            else
                this[x, y] &= ~0x4000;
        }*/

        /*public void SetIsTile(int x, int y, bool isTile)
        {
            unchecked
            {
                if (isTile)
                    this[x, y] &= (short)~0x8000;
                else
                    this[x, y] |= (short)0x8000;
            }
        }
        public void SetIsTile((int X, int Y) location, bool isTile) => SetIsTile(location.X, location.Y, isTile);*/

        public void Compose(IPacket packet)
        {
            throw new NotImplementedException();

            /*if (Values == null)
                throw new NullReferenceException("Values cannot be null.");
            if (Values.Length != Width * Length)
                throw new FormatException($"The length of Values must be equal to Width * Height");

            packet.WriteInt(Width);
            packet.WriteLegacyShort((short)(Width * Length));
            for (int i = 0; i < Values.Length; i++)
                packet.WriteShort(Values[i]);*/
        }

        public static Heightmap Parse(IReadOnlyPacket packet) => new Heightmap(packet);


        public IEnumerator<HeightmapTile> GetEnumerator() => _tiles.GetEnumerator();

        IEnumerator<IHeightmapTile> IEnumerable<IHeightmapTile>.GetEnumerator() => _tiles.Cast<IHeightmapTile>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
