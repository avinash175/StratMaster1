using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class TriangleOP
    {
        public TrendLines TrendLine { get; set; }
        public LongShortType LS { get; set; }
        public TriangleType TypeOfTriangle { get; set; }
        public Point PresentPoint { get; set; }


        public TriangleOP(TrendLines td, Point pp)
        {
            TrendLine = td;
            PresentPoint = pp;
        }

        public void ClassifyTriangle(double priceThresh = 0.1, int distanceThresh = 30, double angleThresh = 1)
        {
            if (TrendLine == null || TrendLine.SupplyLine == null || TrendLine.DemandLine == null)
                return;

            if (Math.Abs((TrendLine.SupplyLine.P1.X - TrendLine.DemandLine.P1.X)) > distanceThresh
               || Math.Abs((TrendLine.DemandLine.P1.X - TrendLine.SupplyLine.P2.X)) > distanceThresh
               || Math.Abs((TrendLine.SupplyLine.P2.X - TrendLine.DemandLine.P2.X)) > distanceThresh)
            {
                TrendLine = null;
                return;
            }

            if ((PresentPoint.Y < (1.0 + priceThresh / 100.0) * (TrendLine.SupplyLine.Slope * PresentPoint.X + TrendLine.SupplyLine.Intercept))
               && (PresentPoint.Y > (1.0 - priceThresh / 100.0) * (TrendLine.DemandLine.Slope * PresentPoint.X + TrendLine.DemandLine.Intercept)))
            {
                TrendLine = null;
                return;
            }

            double supplyTheta = Math.Atan(TrendLine.SupplyLine.Slope) * 180.0 / Math.PI;
            double demandTheta = Math.Atan(TrendLine.DemandLine.Slope) * 180.0 / Math.PI;

            if (Math.Abs(supplyTheta) < angleThresh && demandTheta > angleThresh)
            {
                TypeOfTriangle = TriangleType.ASCENDING_CONTINUATION_TRIANGLE_BULLISH;
            }
            else if (Math.Abs(demandTheta) < angleThresh && supplyTheta < -angleThresh)
            {
                TypeOfTriangle = TriangleType.DESCENDING_CONTINUATION_TRIANGLE_BEARISH;
            }
            else if (Math.Abs(demandTheta) < angleThresh && Math.Abs(supplyTheta) < angleThresh)
            {
                TypeOfTriangle = TriangleType.DOUBLE_EXTRIMA;
            }
            else if (supplyTheta < 0 && demandTheta > 0)
            {
                if (Math.Abs(supplyTheta + demandTheta) < angleThresh)
                {
                    TypeOfTriangle = TriangleType.SYMMETRICAL_CONTINUATION_TRIANGLE;
                }

                else
                {
                    TypeOfTriangle = TriangleType.NORMAL_TRIANGLE;
                }
            }
            else if (supplyTheta > 0 && demandTheta < 0)
            {
                TypeOfTriangle = TriangleType.MEGAPHONE;
            }
            else if (supplyTheta < 0 && demandTheta < 0)
            {
                if (Math.Abs(supplyTheta - demandTheta) < angleThresh)
                {
                    TypeOfTriangle = TriangleType.FLAG_BULLISH;
                }
                else if (Math.Abs(supplyTheta) > 45 || Math.Abs(demandTheta) > 45)
                {
                    TypeOfTriangle = TriangleType.CONTINUATION_WEDGE_BULLISH;
                }
                else
                {
                    TypeOfTriangle = TriangleType.PENNANT_BULLISH;
                }
            }
            else if (supplyTheta > 0 && demandTheta > 0)
            {
                if (Math.Abs(supplyTheta - demandTheta) < angleThresh)
                {
                    TypeOfTriangle = TriangleType.FLAG_BEARISH;
                }
                else if (Math.Abs(supplyTheta) > 45 || Math.Abs(demandTheta) > 45)
                {
                    TypeOfTriangle = TriangleType.CONTINUATION_WEDGE_BEARISH;
                }
                else
                {
                    TypeOfTriangle = TriangleType.PENNANT_BEARISH;
                }
            }

            if (PresentPoint.Y > TrendLine.SupplyLine.Slope * PresentPoint.X + TrendLine.SupplyLine.Intercept)
            {
                LS = LongShortType.LONG;
            }
            else if (PresentPoint.Y < TrendLine.DemandLine.Slope * PresentPoint.X + TrendLine.DemandLine.Intercept)
            {
                LS = LongShortType.SHORT;
            }
        }
    }

    public enum TriangleType
    {
        NO_PATTERN, //0
        ASCENDING_CONTINUATION_TRIANGLE_BULLISH, //1
        DESCENDING_CONTINUATION_TRIANGLE_BEARISH, //2
        DOUBLE_EXTRIMA, //3
        NORMAL_TRIANGLE, //4
        CONTINUATION_WEDGE_BULLISH, //5
        CONTINUATION_WEDGE_BEARISH, //6
        PENNANT_BULLISH, //7
        PENNANT_BEARISH, //8
        FLAG_BULLISH, //9
        FLAG_BEARISH, //10
        MEGAPHONE, //11
        SYMMETRICAL_CONTINUATION_TRIANGLE // 12
    }
}
