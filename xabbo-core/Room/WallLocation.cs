using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class WallLocation : IWallLocation, IWritable
    {
        public static WallLocation Zero => new WallLocation();

        public int WallX { get; set; }
        public int WallY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public WallOrientation Orientation { get; set; }

        public WallLocation()
        {
            Orientation = WallOrientation.Left;
        }

        public WallLocation(IWallLocation origin)
        {
            WallX = origin.WallX;
            WallY = origin.WallY;
            X = origin.X;
            Y = origin.Y;
            Orientation = origin.Orientation;
        }

        public WallLocation(int wallX, int wallY, int x, int y, WallOrientation orientation)
        {
            WallX = wallX;
            WallY = wallY;
            X = x;
            Y = y;
            Orientation = orientation;
        }

        /// <summary>
        /// Offsets the wall location by the specified values
        /// and attempts to keep the offset X and Y locations in place,
        /// relative to the original wall location.
        /// </summary>
        /// <param name="offsetX">The amount to offset the wall X by.</param>
        /// <param name="offsetY">The amount to offset the wall Y by.</param>
        /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
        public void Offset(int offsetX, int offsetY, int scale)
        {
            int halfTileWidth = scale / 2;
            WallX += offsetX;
            WallY += offsetY;
            X -= (offsetX - offsetY) * halfTileWidth;
            Y -= (offsetX + offsetY) * halfTileWidth / 2;
        }

        /// <summary>
        /// Attempts to adjust the wall X, Y and offset X, Y positions
        /// to a valid location using the room scale.
        /// </summary>
        /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
        public void Adjust(int scale)
        {
            while (X > (scale / 2))
            {
                X -= scale / 2;
                if (Orientation == WallOrientation.Left)
                {
                    WallY--;
                    Y += scale / 4;
                }
                else
                {
                    WallX++;
                    Y -= scale / 4;
                }
            }

            while (X < (-scale / 2))
            {
                X += scale / 2;
                if (Orientation == WallOrientation.Left)
                {
                    WallY++;
                    Y -= scale / 4;
                }
                else
                {
                    WallX--;
                    Y += scale / 4;
                }
            }
        }

        public void Write(Packet packet) => packet.WriteString(ToString());

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

        public static string ToString(int wallX, int wallY, int x, int y, WallOrientation orientation) => $":w={wallX},{wallY} l={x},{y} {(char)orientation}";

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

            string[] parts = locationString.Split(' ');
            if (parts.Length != 3 ||
                parts[0].Length < 6 ||
                parts[1].Length < 5 ||
                parts[2].Length != 1 ||
                !parts[0].StartsWith(":w=") ||
                !parts[1].StartsWith("l="))
            {
                return false;
            }

            string[] positions = parts[0].Substring(3).Split(',');
            if (positions.Length != 2) return false;
            if (!int.TryParse(positions[0], out int wallX)) return false;
            if (!int.TryParse(positions[1], out int wallY)) return false;

            positions = parts[1].Substring(2).Split(',');
            if (positions.Length != 2) return false;
            if (!int.TryParse(positions[0], out int x)) return false;
            if (!int.TryParse(positions[1], out int y)) return false;

            WallOrientation orientation;
            switch (parts[2][0])
            {
                case 'l': orientation = WallOrientation.Left; break;
                case 'r': orientation = WallOrientation.Right; break;
                default: return false;
            }

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

        public static bool operator ==(WallLocation a, WallLocation b)
        {
            if (a is null) return b is null;
            else return a.Equals(b);
        }
        public static bool operator !=(WallLocation a, WallLocation b) => !(a == b);

        public static WallLocation operator +(WallLocation location, (int X, int Y) offset)
            => new WallLocation(location.WallX, location.WallY, location.X + offset.X, location.Y + offset.Y, location.Orientation);

        public static implicit operator string(WallLocation location) => location.ToString();
        public static implicit operator WallLocation(string s) => WallLocation.Parse(s);
    }
}
