using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Line
    {
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        public double Slope { get; set; }
        public double Intercept { get; set; }

        public Line(Point p1, Point p2)
        {
            P1 = new Point(p1.X, p1.Y);
            P2 = new Point(p2.X, p2.Y);
            Slope = P1.Slope(P2);
            Intercept = P1.Intercept(P2);
        }

        public Line(Line rhs)
        {
            P1 = new Point(rhs.P1.X, rhs.P1.Y);
            P2 = new Point(rhs.P2.X, rhs.P2.Y);

            Slope = rhs.Slope;
            Intercept = rhs.Intercept;
        }

        public bool IsEqual(Line y)
        {
            if (this == null && y == null)
                return true;
            else if (this == null || y == null)
                return false;
            else if (this.P1.IsEqual(y.P1) && this.P2.IsEqual(y.P2))
                return true;
            else
                return false;
        }
    }
}
