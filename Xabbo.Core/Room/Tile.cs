using System;
using System.Globalization;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public struct Tile : IComposable
    {
        public static readonly Tile Zero = new Tile(0, 0, 0);

        public int X { get; }
        public int Y { get; }
        public float Z { get; }
        public (int X, int Y) XY => (X, Y);
        public (int X, int Y, float Z) XYZ => (X, Y, Z);

        public Tile(int x, int y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Tile(int x, int y)
            : this(x, y, 0)
        { }

        public void Compose(IPacket packet)
        {
            packet.WriteInt(X);
            packet.WriteInt(Y);
            packet.WriteFloatAsString(Z);
        }

        public Tile Add(int x, int y) => Add(x, y, 0);
        public Tile Add(int x, int y, float z) => new Tile(X + x, Y + y, Z + z);
        public Tile Add(Tile other) => Add(other.X, other.Y, other.Z);

        public Tile Subtract(int x, int y) => Subtract(x, y, 0);
        public Tile Subtract(int x, int y, float z) => new Tile(X - x, Y - y, Z - z);
        public Tile Subtract(Tile other) => Subtract(other.X, other.Y, other.Z);

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is Tile other)
                return Equals(other);
            else if (obj is ValueTuple<int, int> t2)
                return Equals(t2);
            else if (obj is ValueTuple<int, int, float> t3)
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

        public bool Equals((int X, int Y, float Z) location, float epsilon = XabboConst.DEFAULT_EPSILON)
        {
            return
                X == location.X &&
                Y == location.Y &&
                Math.Abs(location.Z - Z) < epsilon;
        }

        public bool Equals((int X, int Y, int Z) location, float epsilon = XabboConst.DEFAULT_EPSILON)
        {
            return
                X == location.X &&
                Y == location.Y &&
                Math.Abs(location.Z - Z) < epsilon;
        }

        public bool Equals(Tile other, float epsilon = XabboConst.DEFAULT_EPSILON) => Equals(other.XYZ, epsilon);

        public override string ToString() => $"{X},{Y},{Z:0.0###############}";

        public static bool operator ==(Tile a, Tile b) => a.Equals(b);
        public static bool operator !=(Tile a, Tile b) => !(a == b);

        public static bool operator ==(Tile tile, (int X, int Y) location) => tile.Equals(location);
        public static bool operator !=(Tile tile, (int X, int Y) location) => !tile.Equals(location);

        public static bool operator ==(Tile tile, (int X, int Y, float Z) location) => tile.Equals(location);
        public static bool operator ==(Tile tile, (int X, int Y, int Z) location) => tile.Equals(location);
        public static bool operator !=(Tile tile, (int X, int Y, float Z) location) => !tile.Equals(location);
        public static bool operator !=(Tile tile, (int X, int Y, int Z) location) => !tile.Equals(location);

        public static Tile operator +(Tile location, (int X, int Y) offset) => location.Add(offset.X, offset.Y);
        public static Tile operator +(Tile location, (int X, int Y, int Z) offset) => location.Add(offset.X, offset.Y, offset.Z);
        public static Tile operator +(Tile location, (int X, int Y, float Z) offset) => location.Add(offset.X, offset.Y, offset.Z);
        public static Tile operator +(Tile a, Tile b) => a.Add(b);

        public static Tile operator -(Tile location, (int X, int Y) offset) => location.Subtract(offset.X, offset.Y);
        public static Tile operator -(Tile location, (int X, int Y, int Z) offset) => location.Subtract(offset.X, offset.Y, offset.Z);
        public static Tile operator -(Tile location, (int X, int Y, float Z) offset) => location.Subtract(offset.X, offset.Y, offset.Z);
        public static Tile operator -(Tile a, Tile b) => a.Subtract(b);

        public static implicit operator Tile((int X, int Y, float Z) location) => new Tile(location.X, location.Y, location.Z);
        public static implicit operator (int X, int Y, float Z)(Tile tile) => (tile.X, tile.Y, tile.Z);

        public static implicit operator Tile((int X, int Y) location) => new Tile(location.X, location.Y);
        public static implicit operator (int X, int Y)(Tile tile) => (tile.X, tile.Y);

        public static Tile Parse(IReadOnlyPacket packet)
        {
            return new Tile(packet.ReadInt(), packet.ReadInt(), packet.ReadFloatAsString());
        }

        public static Tile Parse(string format)
        {
            var split = format.Split(',');
            if (split.Length != 3 ||
                !int.TryParse(split[0], out int x) ||
                !int.TryParse(split[1], out int y) ||
                !float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                throw new FormatException($"Invalid tile format: '{format}'");
            }

            return new Tile(x, y, z);
        }
    }
}
