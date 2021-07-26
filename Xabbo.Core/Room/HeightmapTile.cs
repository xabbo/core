using System;

namespace Xabbo.Core
{
    public class HeightmapTile : IHeightmapTile
    {
        public int X { get; }
        public int Y { get; }
        public (int X, int Y) Location => (X, Y);

        public bool IsTile { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsFree => IsTile && !IsBlocked;
        public double Height { get; set; }

        public HeightmapTile(int x, int y, short value)
        {
            X = x;
            Y = y;

            Update(value);
        }

        public void Update(short value)
        {
            IsTile = value >= 0;
            IsBlocked = (value & 0x4000) > 0;
            Height = (value < 0) ? -1 : ((value & 0x3FFF) / 256.0);
        }
    }
}
