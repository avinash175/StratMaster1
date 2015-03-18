using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Patterns
    {
        public static List<Dictionary<int, double>> GenerateTDPoints(TimeSeries series, int winSize = 10)
        {
            Dictionary<int, double> TDsp = new Dictionary<int, double>();
            Dictionary<int, double> TDdp = new Dictionary<int, double>();

            // Find TD points
            for (int i = 2; i < series.OHLC.dates.Length - 1; i++)
            {
                if (series.OHLC.high[i] >= series.OHLC.high[i + 1]
                    && series.OHLC.high[i] >= series.OHLC.high[i - 1]
                    && series.OHLC.high[i] >= series.OHLC.close[i - 2])
                {
                    TDsp.Add(i, series.OHLC.high[i]);
                }
                if (series.OHLC.low[i] <= series.OHLC.low[i + 1]
                    && series.OHLC.low[i] <= series.OHLC.low[i - 1]
                    && series.OHLC.low[i] <= series.OHLC.close[i - 2])
                {
                    TDdp.Add(i, series.OHLC.low[i]);
                }
            }

            for (int i = winSize; i < series.OHLC.dates.Length; i++)
            {
                Dictionary<int, double> temp = TDsp.Where(x => x.Key >= i - winSize && x.Key < i).ToDictionary(x => x.Key, x => x.Value);
                if (temp.Count > 1)
                {
                    double val = temp.Max(x => x.Value);
                    temp = temp.Where(x => x.Value != val).ToDictionary(x => x.Key, x => x.Value);

                    for (int k = 0; k < temp.Count; k++)
                    {
                        TDsp.Remove(temp.Keys.ElementAt(k));
                    }
                }

                temp = TDdp.Where(x => x.Key >= i - winSize && x.Key < i).ToDictionary(x => x.Key, x => x.Value);

                if (temp.Count > 1)
                {
                    double val = temp.Min(x => x.Value);
                    temp = temp.Where(x => x.Value != val).ToDictionary(x => x.Key, x => x.Value);

                    for (int k = 0; k < temp.Count; k++)
                    {
                        TDdp.Remove(temp.Keys.ElementAt(k));
                    }
                }
            }

            List<Dictionary<int, double>> ret = new List<Dictionary<int, double>>();
            ret.Add(TDsp);
            ret.Add(TDdp);

            return ret;// Filter points
        }

        public static TriangleOP[] FindTriangle(TimeSeries series, int winSize, double priceThresh,
            int distThresh = 30, double angleThresh = 1.0)
        {
            List<Dictionary<int, double>> points = GenerateTDPoints(series, winSize);
            Dictionary<int, double> TDsp = points[0];
            Dictionary<int, double> TDdp = points[1];

            bool supplyPresent = false;
            bool demandPresent = false;

            TriangleOP[] ret = new TriangleOP[series.OHLC.dates.Length];

            if (TDsp.Count < 2 || TDdp.Count < 2)
            {
                return ret;
            }

            int startIdx = Math.Max(TDsp.ElementAt(1).Key, TDdp.ElementAt(1).Key);

            int[] keys = TDsp.Keys.Select(x => x).ToArray();
            int[] keyd = TDdp.Keys.Select(x => x).ToArray();

            double sY1 = 0, sY2 = 0, dY1 = 0, dY2 = 0;
            double sX1 = 0, sX2 = 0, dX1 = 0, dX2 = 0;

            for (int i = startIdx; i < series.OHLC.dates.Length; i++)
            {

                int idxs = Array.BinarySearch(keys, i);

                if (idxs < 0)
                {
                    idxs = ~idxs;
                    idxs--;
                }
                else
                {
                    idxs--;
                }

                if (idxs > 0)
                {
                    sX2 = TDsp.ElementAt(idxs).Key;
                    sY2 = TDsp.ElementAt(idxs).Value;

                    while (idxs > 0)
                    {
                        idxs--;

                        sX1 = TDsp.ElementAt(idxs).Key;
                        sY1 = TDsp.ElementAt(idxs).Value;

                        //if (sY1 > sY2)
                        {
                            supplyPresent = true;
                            break;
                        }
                    }
                }

                int idxd = Array.BinarySearch(keyd, i);

                if (idxd < 0)
                {
                    idxd = ~idxd;
                    idxd--;
                }

                else
                {
                    idxd--;
                }

                if (idxd > 0)
                {
                    dX2 = TDdp.ElementAt(idxd).Key;
                    dY2 = TDdp.ElementAt(idxd).Value;

                    while (idxd > 0)
                    {
                        idxd--;

                        dX1 = TDdp.ElementAt(idxd).Key;
                        dY1 = TDdp.ElementAt(idxd).Value;

                        //if (dY1 < dY2)
                        {
                            demandPresent = true;
                            break;
                        }
                    }
                }

                if (supplyPresent && demandPresent)
                {
                    Line supplyLine = null, demandLine = null;

                    supplyLine = new Line(new Point(sX1, sY1), new Point(sX2, sY2));
                    demandLine = new Line(new Point(dX1, dY1), new Point(dX2, dY2));

                    TrendLines td = new TrendLines(supplyLine, demandLine);

                    if (series.OHLC.close[i] > supplyLine.Slope * i + supplyLine.Intercept
                        && ((dX1 < sX1 && sX1 < dX2 && dX2 < sX2) || (sX1 < dX1 && dX1 < sX2 && sX2 < dX2))
                        && (td.Intersection.X < Math.Min(dX1, sX1) || td.Intersection.X > Math.Max(sX2, dX2)))
                    {
                        ret[i] = new TriangleOP(td, new Point(i, series.OHLC.close[i]));
                        ret[i].ClassifyTriangle(priceThresh, distThresh, angleThresh);
                    }

                    if (series.OHLC.close[i] < demandLine.Slope * i + demandLine.Intercept
                        && ((dX1 < sX1 && sX1 < dX2 && dX2 < sX2) || (sX1 < dX1 && dX1 < sX2 && sX2 < dX2))
                        && (td.Intersection.X < Math.Min(dX1, sX1) || td.Intersection.X > Math.Max(sX2, dX2)))
                    {
                        ret[i] = new TriangleOP(td, new Point(i, series.OHLC.close[i]));
                        ret[i].ClassifyTriangle(priceThresh, distThresh, angleThresh);
                    }                    
                }
            }

            return ret;
        }

        public static HSOP[] HeadAndShoulders(TimeSeries series)
        {
            List<Dictionary<int, double>> points = GenerateTDPoints(series);
            Dictionary<int, double> TDsp = points[0];
            Dictionary<int, double> TDdp = points[1];

            HSOP[] ret = new HSOP[series.OHLC.dates.Length];

            int startIdx = Math.Max(TDsp.ElementAt(2).Key, TDdp.ElementAt(2).Key);

            int[] keys = TDsp.Keys.Select(x => x).ToArray();
            int[] keyd = TDdp.Keys.Select(x => x).ToArray();

            double sY1 = 0, sY2 = 0, sY3 = 0, dY1 = 0, dY2 = 0, dY3 = 0;
            double sX1 = 0, sX2 = 0, sX3 = 0, dX1 = 0, dX2 = 0, dX3 = 0;

            for (int i = startIdx; i < series.OHLC.dates.Length; i++)
            {
                int idxs = Array.BinarySearch(keys, i);

                if (idxs < 0)
                {
                    idxs = ~idxs;
                    idxs--;
                }
                else
                {
                    idxs--;
                }

                if (idxs > 1)
                {
                    sX1 = TDsp.ElementAt(idxs).Key;
                    sY1 = TDsp.ElementAt(idxs).Value;

                    idxs--;

                    sX2 = TDsp.ElementAt(idxs).Key;
                    sY2 = TDsp.ElementAt(idxs).Value;

                    idxs--;

                    sX3 = TDsp.ElementAt(idxs).Key;
                    sY3 = TDsp.ElementAt(idxs).Value;

                }

                int idxd = Array.BinarySearch(keyd, i);

                if (idxd < 0)
                {
                    idxd = ~idxd;
                    idxd--;
                }

                else
                {
                    idxd--;
                }

                if (idxd > 1)
                {
                    dX1 = TDdp.ElementAt(idxd).Key;
                    dY1 = TDdp.ElementAt(idxd).Value;

                    idxd--;

                    dX2 = TDdp.ElementAt(idxd).Key;
                    dY2 = TDdp.ElementAt(idxd).Value;

                    idxd--;

                    dX3 = TDdp.ElementAt(idxd).Key;
                    dY3 = TDdp.ElementAt(idxd).Value;

                }

                if ((sX3 < dX3 && dX3 < sX2 && sX2 < dX2 && dX2 < sX1 && sX1 < dX1)
                       || (dX3 < sX3 && sX3 < dX2 && dX2 < sX2 && sX2 < dX1 && dX1 < sX1))
                {
                    HSPattern td = new HSPattern(new Point(sX1, sY1), new Point(sX2, sY2), new Point(sX3, sY3),
                    new Point(dX1, dY1), new Point(dX2, dY2), new Point(dX3, dY3));

                    ret[i] = new HSOP(td, new Point(i, series.OHLC.close[i]));
                }
            }

            return ret;
        }

        //        public static List<Point> RoundingBottom(TimeSeries series, int n)
        //        {
        //            List<Dictionary<int, double>> points = GenerateTDPoints(series);
        //            Dictionary<int, double> TDsp = points[0];
        //            Dictionary<int, double> TDdp = points[1];

        //            Polynomial ret = new Polynomial(2);

        //            List<Point> Points = new List<Point>();

        //            int startIdx = Math.Max(TDsp.ElementAt(n-1).Key, TDdp.ElementAt(n-1).Key);

        //            int[] keys = TDsp.Keys.Select(x => x).ToArray();
        //            int[] keyd = TDdp.Keys.Select(x => x).ToArray();

        //            double sY1 = 0, sY2 = 0, sY3 = 0, sY4 = 0, sY5 = 0, dY1 = 0, dY2 = 0, dY3 = 0, dY4 = 0, dY5 = 0;
        //            double sX1 = 0, sX2 = 0, sX3 = 0, sX4 = 0, sX5 = 0, dX1 = 0, dX2 = 0, dX3 = 0, dX4 = 0, dX5 = 0;

        //            for (int i = startIdx; i < series.OHLC.dates.Length; i++)
        //            {

        //                int idxs = Array.BinarySearch(keys, i);

        //                if (idxs < 0)
        //                {
        //                    idxs = ~idxs;
        //                    idxs--;
        //                }
        //                else
        //                {
        //                    idxs--;
        //                }

        //                if (idxs > n)
        //                {
        //                    sX5 = TDsp.ElementAt(idxs).Key;
        //                    sY5 = TDsp.ElementAt(idxs).Value;

        //                    while (idxs > n-1)
        //                    {
        //                        idxs--;

        //                        sX4 = TDsp.ElementAt(idxs).Key;
        //                        sY4 = TDsp.ElementAt(idxs).Value;

        //                        idxs--;

        //                        sX3 = TDsp.ElementAt(idxs).Key;
        //                        sY3 = TDsp.ElementAt(idxs).Value;

        //                        idxs--;

        //                        sX2 = TDsp.ElementAt(idxs).Key;
        //                        sY2 = TDsp.ElementAt(idxs).Value;

        //                        idxs--;

        //                        sX1 = TDsp.ElementAt(idxs).Key;
        //                        sY1 = TDsp.ElementAt(idxs).Value;
        //                    }
        //                }

        //                int idxd = Array.BinarySearch(keyd, i);

        //                if (idxd < 0)
        //                {
        //                    idxd = ~idxd;
        //                    idxd--;
        //                }

        //                else
        //                {
        //                    idxd--;
        //                }

        //                if (idxd > n)
        //                {
        //                    dX5 = TDdp.ElementAt(idxd).Key;
        //                    dY5 = TDdp.ElementAt(idxd).Value;

        //                    while (idxd > n-2)
        //                    {
        //                        idxd--;

        //                        dX4 = TDdp.ElementAt(idxd).Key;
        //                        dY4 = TDdp.ElementAt(idxd).Value;

        //                        idxd--;

        //                        dX3 = TDdp.ElementAt(idxd).Key;
        //                        dY3 = TDdp.ElementAt(idxd).Value;

        //                        idxd--;

        //                        dX2 = TDdp.ElementAt(idxd).Key;
        //                        dY2 = TDdp.ElementAt(idxd).Value;

        //                        idxd--;

        //                        dX1 = TDdp.ElementAt(idxd).Key;
        //                        dY1 = TDdp.ElementAt(idxd).Value;
        //                    }
        //                }

        //                ret.Points.Add(new Point(sX1, sY1));
        //                ret.Points.Add(new Point(sX2, sY2));
        //                ret.Points.Add(new Point(sX3, sY3));
        //                ret.Points.Add(new Point(sX4, sY4));
        //                ret.Points.Add(new Point(sX5, sY5));

        //                ret.Points.Add(new Point(dX1, dY1));
        //                ret.Points.Add(new Point(dX2, dY2));
        //                ret.Points.Add(new Point(dX3, dY3));
        //                ret.Points.Add(new Point(dX4, dY4));
        //                ret.Points.Add(new Point(dX5, dY5));

        //                if (ret.Param[1] < 0.01 && ret.Param[2] > 0 && ret.RMSError < 0.1)
        //                {

        //                }

        //        }
        //    
        //



    }
}

                
        
    

