using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public struct WallLocation : IPacketData
    {
        /// <summary>
        /// Represents a wall location with all coordinates at zero, and the orientation set to the left wall.
        /// </summary>
        public static readonly WallLocation Zero = new WallLocation(0, 0, 0, 0, 'l');

        /// <summary>
        /// Gets the wall X coordinate.
        /// </summary>
        public int WallX { get; }
        /// <summary>
        /// Gets the wall Y coordinate.
        /// </summary>
        public int WallY { get; }
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// Gets the wall orientation.
        /// </summary>
        public WallOrientation Orientation { get; }

        public WallLocation(int wallX, int wallY, int offsetX, int offsetY, WallOrientation orientation)
        {
            WallX = wallX;
            WallY = wallY;
            X = offsetX;
            Y = offsetY;
            Orientation = orientation;
        }

        public WallLocation(int wallX, int wallY, int offsetX, int offsetY)
            : this(wallX, wallY, offsetX, offsetY, 'l')
        { }

        /// <summary>
        /// Offsets the wall location by the specified values
        /// and attempts to keep the offset X and Y locations in place,
        /// relative to the original wall location.
        /// </summary>
        /// <param name="wallOffsetX">The amount to offset the wall X by.</param>
        /// <param name="wallOffsetY">The amount to offset the wall Y by.</param>
        /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
        public WallLocation Offset(int wallOffsetX, int wallOffsetY, int scale)
        {
            int halfTileWidth = scale / 2;

            return Add(
                wallOffsetX,
                wallOffsetY,
                -(wallOffsetX - wallOffsetY) * halfTileWidth,
                -(wallOffsetX + wallOffsetY) * halfTileWidth / 2
            );
        }

        /// <summary>
        /// Attempts to adjust all coordinates to a valid location using the room scale,
        /// and returns the updated wall location. Does not take into account the wall
        /// boundaries defined in the floor plan which affects the starting position of the Y coordinate.
        /// </summary>
        /// <param name="scale">The scale value of the room as specified in the floor plan.</param>
        public WallLocation Adjust(int scale)
        {
            int
                x = X,
                y = Y,
                wallX = WallX,
                wallY = WallY;

            while (x > (scale / 2))
            {
                x -= scale / 2;
                if (Orientation == WallOrientation.Left)
                {
                    wallY--;
                    y += scale / 4;
                }
                else
                {
                    wallX++;
                    y -= scale / 4;
                }
            }

            while (x < (-scale / 2))
            {
                x += scale / 2;
                if (Orientation == WallOrientation.Left)
                {
                    wallY++;
                    y -= scale / 4;
                }
                else
                {
                    wallX--;
                    y += scale / 4;
                }
            }

            return new WallLocation(wallX, wallY, x, y, Orientation);
        }

        /// <summary>
        /// Flips the wall orientation between left and right, and returns the new wall location.
        /// </summary>
        public WallLocation Flip() => new WallLocation(WallX, WallY, X, Y, Orientation.IsLeft ? WallOrientation.Right : WallOrientation.Left);

        /// <summary>
        /// Returns a new wall location with the specified orientation.
        /// </summary>
        public WallLocation Orient(WallOrientation orientation) => new WallLocation(WallX, WallY, X, Y, orientation);

        /// <summary>
        /// Adds the specified offset values and returns the new wall location.
        /// </summary>
        public WallLocation Add(int wallX, int wallY, int locationX, int locationY) => new WallLocation(
            WallX + wallX, WallY + wallY,
            X + locationX, Y + locationY,
            Orientation
        );

        /// <summary>
        /// Adds the specified offset values and returns the new wall location.
        /// </summary>
        public WallLocation Add(int offsetX, int offsetY) => new WallLocation(
            WallX, WallY, X + offsetX, Y + offsetY, Orientation
        );

        public override int GetHashCode() => (WallX, WallY, X, Y, Orientation.Value).GetHashCode();

        public bool Equals(WallLocation other)
        {
            return
                WallX == other.WallX &&
                WallY == other.WallY &&
                X == other.X &&
                Y == other.Y &&
                Orientation == other.Orientation;
        }

        public override bool Equals(object? obj)
            => obj is WallLocation loc && Equals(loc);

        public override string ToString() => ToString(WallX, WallY, X, Y, Orientation);
        public static string ToString(int wallX, int wallY, int x, int y, WallOrientation orientation) => $":w={wallX},{wallY} l={x},{y} {orientation.Value}";

        public void Write(IPacket packet)
        {
            packet.WriteInt(WallX);
            packet.WriteInt(WallY);
            packet.WriteInt(X);
            packet.WriteInt(Y);
            packet.WriteString(Orientation.Value.ToString());
        }

        public static WallLocation Parse(IReadOnlyPacket packet) => Parse(packet.ReadString());

        public static WallLocation Parse(string locationString)
        {
            if (locationString == null)
                return default;
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

            location = new WallLocation(wallX, wallY, x, y, orientation);
            return true;
        }

        public static bool operator ==(WallLocation a, WallLocation b) => a.Equals(b);
        public static bool operator !=(WallLocation a, WallLocation b) => !(a == b);

        public static WallLocation operator +(WallLocation location, (int X, int Y) offset)
            => location.Add(offset.X, offset.Y);
        public static WallLocation operator +(WallLocation location, (int WallX, int WallY, int X, int Y) offset)
            => location.Add(offset.WallX, offset.WallY, offset.X, offset.Y);
        public static WallLocation operator -(WallLocation location, (int X, int Y) offset)
            => location.Add(-offset.X, -offset.Y);
        public static WallLocation operator -(WallLocation location, (int WallX, int WallY, int X, int Y) offset)
            => location.Add(-offset.WallX, -offset.WallY, -offset.X, -offset.Y);

        public static implicit operator WallLocation(string s) => Parse(s);
    }
}
