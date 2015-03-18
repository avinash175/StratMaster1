using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class TrianglePatternStrategy2: BasicStrategy
    {
        public object LookBackWinSize = 100;
        public object FilterWinSize = 10;
        public object DistanceThresh = 30;
        public object AngleThresh = 1;
        public object TrendPerRet = 2.0;
        public object PriceThreshInPer = 0.1;
        public object TypeOfTraingle = 0;
        
        public TrianglePatternStrategy2(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            //if (numSec == 1)
            //{
            //    for (int i = 0; i < 12; i++)
            //    {
            //        data.InputData.Add(data.InputData[0]);
            //        data.SecName.Add(data.SecName[0]);
            //    }
            //}

            numSec = data.InputData.Count;

            int lbWin = Convert.ToInt32(LookBackWinSize);
            int filWin = Convert.ToInt32(FilterWinSize);
            int typeOfTriangle = Convert.ToInt32(TypeOfTraingle);
            double priceThresh = Convert.ToDouble(PriceThreshInPer);
            int distThresh = Convert.ToInt32(DistanceThresh);
            double angleThresh = Convert.ToDouble(AngleThresh);
            double trendPerRes = Convert.ToDouble(TrendPerRet)/100.0;

            List<List<string>> comments = new List<List<string>>();

            for (int i = 0; i < numSec; i++)
            {
                comments.Add(new List<string>());
                double[] sig = new double[data.InputData[i].Dates.Length];

                TriangleOP tri = null;
                //typeOfTriangle = i;

                string[] tringleTypes = Enum.GetNames(typeof(TriangleType));

                TriangleOP lastTri = null;
                bool first = true;

                for (int j = lbWin; j < sig.Length; j++)
                {
                    TimeSeries ts = UF.GetRange(data.InputData[i], j - lbWin + 1, j);
                    tri = Patterns.FindTriangle(ts, filWin, priceThresh, distThresh, angleThresh).Last();

                    TriangleType tt = TriangleType.NO_PATTERN;
                    bool trendAgree = true;

                    if (tri != null && tri.TrendLine != null)
                    {
                        if (tri.TypeOfTriangle == TriangleType.FLAG_BULLISH 
                            || tri.TypeOfTriangle== TriangleType.CONTINUATION_WEDGE_BULLISH)
                        {
                            tt = TriangleType.PENNANT_BULLISH;
                        }
                        else if (tri.TypeOfTriangle == TriangleType.FLAG_BEARISH
                            || tri.TypeOfTriangle == TriangleType.CONTINUATION_WEDGE_BEARISH)
                        {
                            tt = TriangleType.PENNANT_BEARISH;
                        }
                        else
                        {
                            tt = tri.TypeOfTriangle;
                        }

                        double firstPt = Math.Min(tri.TrendLine.SupplyLine.P1.X, tri.TrendLine.DemandLine.P1.X);
                        double length = tri.PresentPoint.X - firstPt;

                        int idxBegin = (int)Math.Max(firstPt - length, 0);
                        int idxEnd = (int)firstPt;

                        if (tt == TriangleType.PENNANT_BULLISH
                            && (data.InputData[i].Prices[idxEnd]/data.InputData[i].Prices[idxBegin] - 1) < trendPerRes)
                        {
                            trendAgree = false;
                        }

                        if (tt == TriangleType.PENNANT_BEARISH
                            && (data.InputData[i].Prices[idxEnd] / data.InputData[i].Prices[idxBegin] - 1) > -trendPerRes)
                        {
                            trendAgree = false;
                        }

                        if (trendAgree && (tt.ToString() == tringleTypes[typeOfTriangle] || typeOfTriangle == 0))
                        {
                            if (first)
                            {
                                sig[j] = tri.LS == LongShortType.LONG ? 1 : -1;
                                lastTri = tri;
                                comments[i].Add(tri.TypeOfTriangle.ToString());
                                first = false;
                            }
                            else if (!lastTri.TrendLine.IsEqual(tri.TrendLine))
                            {
                                sig[j] = tri.LS == LongShortType.LONG ? 1 : -1;
                                lastTri = tri;
                                comments[i].Add(tri.TypeOfTriangle.ToString());
                            }
                        }
                    }
                }
                
                base.CalculateNetPosition(data, sig, i);
            }
            base.RunStrategyBase(data);

            for (int i = 0; i < base.Stats.Trades.Count; i++)
			{
                for (int j = 0; j < base.Stats.Trades[i].Count; j++)
                {
                    if (j < comments[i].Count)
                    {
                        base.Stats.Trades[i][j].Description = comments[i][j];
                    }
                }
			}            
        }
    }
}
