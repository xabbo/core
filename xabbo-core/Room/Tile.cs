using System;
using System.Globalization;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Tile : IWritable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Z { get; set; }

        public Tile(int x, int y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Write(Packet packet)
        {
            packet.WriteInteger(X);
            packet.WriteInteger(Y);
            packet.WriteString(Z.ToString());
        }

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is Tile other && Equals(other);
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
            packet.ReadInteger(),
            packet.ReadInteger(),
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

        public static bool operator ==(Tile a, Tile b)
        {
            if (a is null)
                return b is null;
            else
                return a.Equals(b);
        }
        public static bool operator !=(Tile a, Tile b) => !(a == b);
    }
}
