using System;
using System.Globalization;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Tile
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

        public static Tile Parse(Packet packet) => new Tile(
            packet.ReadInteger(),
            packet.ReadInteger(),
            double.Parse(packet.ReadString(), CultureInfo.InvariantCulture)
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

        public bool Equals(Tile other, double threshold = 0.001)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Math.Abs(Z - other.Z) < threshold;
        }
    }
}
