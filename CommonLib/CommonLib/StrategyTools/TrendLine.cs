using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    /// <summary>
    /// ySupply = MSupply * x + CSupply
    /// yDemand = MDemand * x + CDemand
    /// </summary>
    public class TrendLines
    {
        public Line SupplyLine { get; set; }
        public Line DemandLine { get; set; }
        public Point Intersection { get; set; }

        public TrendLines(Line supplyLine = null, Line demandLine = null)
        {
            SupplyLine = supplyLine;
            DemandLine = demandLine;
            if (SupplyLine != null && DemandLine != null)
            {
                double x = (DemandLine.Intercept - SupplyLine.Intercept) /
                    (SupplyLine.Slope - DemandLine.Slope);

                Intersection = new Point(x, SupplyLine.Slope * x + SupplyLine.Intercept);
            }
        }

        public bool IsEqual(TrendLines y)
        {
            if (this == null && y == null)
                return true;
            else if (this == null || y == null)
                return false;
            else if (this.SupplyLine.IsEqual(y.SupplyLine) && this.DemandLine.IsEqual(y.DemandLine))
                return true;
            else
                return false;
        }
    }
}
