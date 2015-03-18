using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class HSOP
    {
        public HSPattern HS { get; set; }
        public LongShortType LS { get; set; }
        public TypeOfHS TypeOfHS { get; set; }
        public Point PresentPoint {get;set;}

        public HSOP(HSPattern hd, Point pp)
        {
            HS = hd;
            PresentPoint = pp;
        }

        public void Classify(double angleThresh = 5, double priceThreshPer = 0.1)
        {

            priceThreshPer /= 100;

            if (HS == null ) 
                return;

            if (HS.Supplypoints == null || HS.Demandpoints == null)
            {
                HS = null;
                return;
            }
            
            if (HS.Supplypoints[0].X < HS.Demandpoints[0].X)
            {
                if(Math.Abs(Math.Atan(HS.Supplypoints[1].Slope(HS.Supplypoints[2]))*180/Math.PI) > angleThresh
                   || (HS.Demandpoints[1].Y / HS.Demandpoints[0].Y - 1) < 3 *(priceThreshPer)
                   || Math.Abs(HS.Demandpoints[2].Y / HS.Demandpoints[0].Y - 1) > priceThreshPer
                   || Math.Max(HS.Demandpoints[0].Y,HS.Demandpoints[2].Y) > Math.Min(HS.Supplypoints[0].Y,HS.Supplypoints[1].Y))
                {
                    HS = null;
                    return;
                }
                else if(PresentPoint.Y<Math.Max(HS.Demandpoints[2].Y,HS.Supplypoints[2].Y))
                {
                    HS = null;
                    return;
                }
                else 
                {
                   TypeOfHS = TypeOfHS.HEAD_AND_SHOULDERS_BOTTOM;
                   LS = LongShortType.LONG;
                }
            }

            else if (HS.Demandpoints[0].X < HS.Supplypoints[0].X)
            {
                if (Math.Abs(Math.Atan(HS.Demandpoints[1].Slope(HS.Demandpoints[2]))*180/Math.PI) > angleThresh
                    || (HS.Supplypoints[1].Y / HS.Supplypoints[0].Y - 1) < 3 * (priceThreshPer)
                    || Math.Abs(HS.Supplypoints[2].Y / HS.Supplypoints[0].Y - 1) < priceThreshPer
                    || Math.Max(HS.Demandpoints[0].Y,HS.Demandpoints[1].Y) > Math.Min(HS.Supplypoints[0].Y,HS.Supplypoints[2].Y))
                {
                    HS = null;
                    return;
                }
                else if (PresentPoint.Y > Math.Min(HS.Demandpoints[2].Y, HS.Supplypoints[2].Y))
                {
                    HS = null;
                    return;
                }
                else
                {
                    TypeOfHS = TypeOfHS.HEAD_AND_SHOULDERS_TOP;
                    LS = LongShortType.SHORT;
                }
            }
        }
    }

    public enum TypeOfHS
    {
        NO_PATTERN,
        HEAD_AND_SHOULDERS_TOP,
        HEAD_AND_SHOULDERS_BOTTOM
    }
}
