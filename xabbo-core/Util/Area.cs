using System;

namespace Xabbo.Core
{
    public class Area
    {
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }

        public Position PointA => new Position(X1, Y1);
        public Position PointB => new Position(X2, Y2);

        public int Width => X2 - X1 + 1;
        public int Length => Y2 - Y1 + 1;

        public Area(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2)
            {
                int temp = x1;
                x1 = x2;
                x2 = temp;
            }

            if (y1 > y2)
            {
                int temp = y1;
                y1 = y2;
                y2 = temp;
            }

            X1 = x1; Y1 = y1;
            X2 = x2; Y2 = y2;
        }

        public Area(Position a, Position b)
            : this(a.X, a.Y, b.X, b.Y)
        { }

        public Area(Position point, int width, int length)
        {
            if (width < 1) throw new ArgumentOutOfRangeException("width");
            if (length < 1) throw new ArgumentOutOfRangeException("length");

            X1 = point.X;
            Y1 = point.Y;
            X2 = X1 + width - 1;
            Y2 = Y1 + length - 1;
        }

        public bool Contains(int x, int y)
        {
            return
                PointA.X <= x && y <= PointB.X &&
                PointA.Y <= y && y <= PointB.Y;
        }
        public bool Contains(Position p) => Contains(p.X, p.Y);
        public bool Contains(Tile t) => Contains(t.X, t.Y);

        public override int GetHashCode() => (X1, Y1, X2, Y2).GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is Area other &&
                other.X1 == X1 &&
                other.Y1 == Y1 &&
                other.X2 == X2 &&
                other.Y2 == Y2;
        }
    }
}
