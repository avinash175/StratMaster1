using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class HSPattern
    {
        public Point[] Supplypoints { get; set; }
        public Point[] Demandpoints { get; set; }

        public HSPattern(Point sp1, Point sp2, Point sp3, Point dp1, Point dp2, Point dp3)
        {
            Supplypoints = new Point[3];
            Supplypoints[0] = new Point(sp3.X, sp3.Y);
            Supplypoints[1] = new Point(sp2.X, sp2.Y);
            Supplypoints[2] = new Point(sp1.X, sp1.Y);

            Demandpoints = new Point[3];
            Demandpoints[0] = new Point(dp3.X, dp3.Y);
            Demandpoints[1] = new Point(dp2.X, dp2.Y);
            Demandpoints[2] = new Point(dp1.X, dp1.Y);
        }
    }
}
