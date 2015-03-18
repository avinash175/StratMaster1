using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    class BuyLosersOverNightStrategy :  BasicStrategy
    {
        // Specific 
        public object BuyLosers = 1;
        public object NumStks = 10;
        public object Threshold = -0.05;

        public BuyLosersOverNightStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int numPoints = data.InputData[0].Dates.Length;
            int buyLosers = Convert.ToInt32(BuyLosers);
            int numStks = Convert.ToInt32(NumStks);
            double thresh = Convert.ToDouble(Threshold);
            
            // find the stocks to short
            Dictionary<DateTime, List<int>> trdStkDict = new Dictionary<DateTime, List<int>>();
            DateTime[] dates = data.InputData[0].Dates.Select(x => x.Date).Distinct().ToArray();

            for (int i = 0; i < numSec; i++)
            {
                NetPositions.Add(new int[numPoints]);
            }

            for (int i = 0; i < dates.Length; i++)
            {
                DateTime dateStart = data.InputData[0].Dates.First(x => x.Date == dates[i].Date);
                DateTime dateEnd = data.InputData[0].Dates.Last(x => x.Date == dates[i].Date);

                int idxS = Array.BinarySearch(data.InputData[0].Dates, dateStart);
                int idxE = Array.BinarySearch(data.InputData[0].Dates, dateEnd);

                double[] ret = new double[numSec];

                int cnt = 0;

                for (int j = 0; j < numSec; j++)
                {
                    if (data.InputData[j].Prices[idxS] > 0)
                    {
                        ret[j] = (data.InputData[j].Prices[idxE] / data.InputData[j].Prices[idxS] - 1);
                        cnt = ret[j] < thresh ? cnt + 1 : cnt;
                    }                    
                    else
                        ret[j] = Double.PositiveInfinity;                                            
                }

                int[] selIdx = UF.BubbleSortIdx(ret, true).Take(Math.Min(numStks, cnt)).ToArray();

                for (int j = 0; j < selIdx.Length; j++)
                {
                    NetPositions[selIdx[j]][idxE] = buyLosers == 1 ? 1: -1;
                }

                //ret = new double[numSec];

                //for (int j = 0; j < numSec; j++)
                //{
                //    if (data.InputData[j].Prices[idxS] > 0)
                //        ret[j] = (data.InputData[j].Prices[idxE] / data.InputData[j].Prices[idxS] - 1);
                //    else
                //        ret[j] = Double.NegativeInfinity;
                //}

                //selIdx = UF.BubbleSortIdx(ret, false).Take(numStks).ToArray();

                //for (int j = 0; j < selIdx.Length; j++)
                //{
                //    NetPositions[selIdx[j]][idxE] = buyLosers == 1 ? +1 : -1;
                //}
            }
            base.RunStrategyBase(data);
        }
    }
}
