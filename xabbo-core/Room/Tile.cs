using System;
using System.Globalization;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Tile : ITile, IWritable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Z { get; set; }

        public (int X, int Y) XY => (X, Y);

        public Tile() { }

        public Tile(int x, int y)
            : this(x, y, 0)
        { }

        public Tile(int x, int y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Write(Packet packet)
        {
            packet.WriteInt(X);
            packet.WriteInt(Y);
            packet.WriteString(Z.ToString());
        }

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Tile other)
                return Equals(other);
            else if (obj is ValueTuple<int, int> t2)
                return Equals(t2);
            else if (obj is ValueTuple<int, int, double> t3)
                return Equals(t3);
            else
                return false;
        }

        public bool Equals((int X, int Y) location)
        {
            return
                X == location.X &&
                Y == location.Y;
        }

        public bool Equals((int X, int Y, double Z) location, double epsilon = XabboConst.DEFAULT_EPSILON)
        {
            return
                X == location.X &&
                Y == location.Y &&
                Math.Abs(location.Z - Z) < epsilon;
        }

        public bool Equals(Tile other, double epsilon = XabboConst.DEFAULT_EPSILON)
        {
            return
                other != null &&
                X == other.X &&
                Y == other.Y &&
                Math.Abs(Z - other.Z) < epsilon;
        }

        public static Tile Parse(Packet packet) => new Tile(
            packet.ReadInt(),
            packet.ReadInt(),
            packet.ReadDouble()
        );

        public static Tile Parse(string format)
        {
            var split = format.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 3)
                throw new FormatException($"Tile format string is invalid: '{format}'");

            int x, y;
            double z;

            if (!int.TryParse(split[0], out x))
                throw new FormatException($"The x value of the tile format string is invalid: '{format}'");
            if (!int.TryParse(split[1], out y))
                throw new FormatException($"The y value of the tile format string is invalid: {format}");
            if (!double.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z))
                throw new FormatException($"The z value of the tile format string is invalid: {format}");

            return new Tile(x, y, z);
        }

        public override string ToString() => $"{X}, {Y}, {Z}";

        public static bool operator ==(Tile a, Tile b)
        {
            if (a is null)
                return b is null;
            else
                return a.Equals(b);
        }
        public static bool operator !=(Tile a, Tile b) => !(a == b);

        public static bool operator ==(Tile tile, (int X, int Y) location) => tile.Equals(location);
        public static bool operator !=(Tile tile, (int X, int Y) location) => !tile.Equals(location);

        public static bool operator ==(Tile tile, (int X, int Y, double Z) location) => tile.Equals(location);
        public static bool operator !=(Tile tile, (int X, int Y, double Z) location) => !tile.Equals(location);

        public static Tile operator +(Tile a, Tile b) => new Tile(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Tile operator -(Tile a, Tile b) => new Tile(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static implicit operator Tile((int X, int Y) location) => new Tile(location.X, location.Y, 0.0);
        public static implicit operator (int X, int Y)(Tile tile) => (tile.X, tile.Y);

        public static implicit operator Tile((int X, int Y, double Z) location) => new Tile(location.X, location.Y, location.Z);
        public static implicit operator (int X, int Y, double Z)(Tile tile) => (tile.X, tile.Y, tile.Z);
    }
}
