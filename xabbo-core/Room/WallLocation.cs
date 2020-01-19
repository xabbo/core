using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class WallLocation : IWritable
    {
        public static WallLocation Parse(string locationString)
        {
            if (!TryParse(locationString, out WallLocation wallLocation))
                throw new FormatException("Wall location string is of an invalid format");
            return wallLocation;
        }

        public static bool TryParse(string locationString, out WallLocation location)
        {
            location = null;

            if (locationString.IndexOf(':') != 0)
                return false;

            string[] a = locationString.Split(' ');
            if (a.Length != 3) return false;

            string[] b = a[0].Substring(3).Split(',');
            if (b.Length != 2) return false;

            if (!int.TryParse(b[0], out int wallX)) return false;
            if (!int.TryParse(b[1], out int wallY)) return false;

            b = a[1].Substring(2).Split(',');
            if (b.Length != 2) return false;
            if (!int.TryParse(b[0], out int x)) return false;
            if (!int.TryParse(b[1], out int y)) return false;

            WallOrientation orientation;
            if (a[2].Length != 1) return false;
            orientation = (WallOrientation)a[2].ToLower()[0];
            if (!Enum.IsDefined(typeof(WallOrientation), orientation))
                return false;

            location = new WallLocation()
            {
                WallX = wallX,
                WallY = wallY,
                X = x,
                Y = y,
                Orientation = orientation
            };

            return true;
        }

        public int WallX { get; set; }
        public int WallY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public WallOrientation Orientation { get; set; }

        public WallLocation() { }

        public WallLocation(int wallX, int wallY, int x, int y, WallOrientation orientation)
        {
            WallX = wallX;
            WallY = wallY;
            X = x;
            Y = y;
            Orientation = orientation;
        }

        public override string ToString() => ToString(WallX, WallY, X, Y, Orientation);

        public override int GetHashCode() => (WallX, WallY, X, Y, Orientation).GetHashCode();

        public override bool Equals(object obj)
        {
            var other = obj as WallLocation;
            return
                other != null &&
                WallX == other.WallX &&
                WallY == other.WallY &&
                X == other.X &&
                Y == other.Y &&
                Orientation == other.Orientation;
        }

        public void Write(Packet packet)
        {
            packet.WriteString(ToString());
        }

        public static bool operator ==(WallLocation a, WallLocation b)
        {
            if (a is null) return b is null;
            else return a.Equals(b);
        }
        public static bool operator !=(WallLocation a, WallLocation b) => !(a == b);

        public static string ToString(int wallX, int wallY, int x, int y, WallOrientation orientation) => $":w={wallX},{wallY} l={x},{y} {(char)orientation}";
    }
}
