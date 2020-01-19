using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core
{
    public class Area
    {
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }

        public int Width => PointA.X - PointB.X + 1;

        public Point PointA => new Point(X1, Y1);
        public Point PointB => new Point(X2, Y2);

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

        public Area(Point a, Point b)
        {
            if (a.X > b.X)
            {
                int temp = a.X;
                a.X = b.X;
                b.X = temp;
            }

            if (a.Y > b.Y)
            {
                int temp = a.Y;
                a.Y = b.Y;
                b.Y = temp;
            }

            X1 = a.X; Y1 = a.Y;
            X2 = b.X; Y2 = b.Y;
        }

        public Area(Point point, int width, int length)
        {
            if (width < 1) throw new ArgumentOutOfRangeException("width");
            if (length < 1) throw new ArgumentOutOfRangeException("length");

            X1 = point.X; Y1 = point.Y;
            X2 = X1 + width - 1;
            Y2 = Y1 + length - 1;
        }

        public bool Contains(int x, int y)
        {
            return
                PointA.X <= x && y <= PointB.X &&
                PointA.Y <= y && y <= PointB.Y;
        }

        public bool Contains(Point p) => Contains(p.X, p.Y);

        public bool Contains(Tile t) => Contains(t.X, t.Y);
    }
}
