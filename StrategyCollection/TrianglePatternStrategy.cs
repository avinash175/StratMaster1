using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class TrianglePatternStrategy: BasicStrategy
    {
        public object LookBackWinSize = 100;
        public object FilterWinSize = 10;
        public object DistanceThresh = 30;
        public object AngleThresh = 1;
        public object PriceThreshInPer = 0.1;
        public object TypeOfTraingle = 0;

        public TrianglePatternStrategy(string stratName, double alloc, double cost, double timeStep)
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

                    if(tri!=null && tri.TrendLine!=null 
                        && (tri.TypeOfTriangle.ToString() == tringleTypes[typeOfTriangle] || typeOfTriangle == 0))
                    {
                        if (first)
                        {
                            sig[j] = tri.LS == LongShortType.LONG ? 1 : -1;
                            lastTri = tri;
                            comments[i].Add(tri.TypeOfTriangle.ToString());
                            first = false;
                        }
                        else if(!lastTri.TrendLine.IsEqual(tri.TrendLine))
                        {
                            sig[j] = tri.LS == LongShortType.LONG ? 1 : -1;
                            lastTri = tri;
                            comments[i].Add(tri.TypeOfTriangle.ToString());
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
