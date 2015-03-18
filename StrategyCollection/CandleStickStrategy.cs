using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class CandleStickStrategy : BasicStrategy
    {
        public object ThreshRetBps = 2.0;
        public object ThreshStrikeRate = 0.5;
        public object MinOccurance = 20;
        public object NumCandles = 1;
        public object NumRetFwd = 1;

        public CandleStickStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;            
            double thresh = Convert.ToDouble(ThreshRetBps)/10000.0;
            int minOcuu = Convert.ToInt32(MinOccurance);
            double threshStkRate = Convert.ToDouble(ThreshStrikeRate);

            for (int i = 0; i < numSec; i++)
            {
                CandleStickSeries cde = new CandleStickSeries(data.InputData[i].OHLC);

                Dictionary<string, int> histBars = cde.CandleSticks.GroupBy(x=>x.HistoryBars).
                    Select(x=>new {Key=x.Key, Count = x.Count(), AvgRet = x.Sum(y=>y.FwdReturn)/x.Count(),
                        NumPos = x.Count(y=>y.FwdReturn > 0)}).Where(x=>x.Count > minOcuu && Math.Abs(x.AvgRet) >= thresh 
                        && (x.AvgRet > 0? x.NumPos/((double)x.Count) > threshStkRate :
                         (x.Count - x.NumPos)/((double)x.Count) > threshStkRate)).ToDictionary(x=>x.Key,y=>Math.Sign(y.AvgRet));
                                        
                double[] sig = cde.CandleSticks.Select(x=>histBars.ContainsKey(x.HistoryBars)?
                    histBars[x.HistoryBars] : 0.0 ).ToArray();

                base.CalculateNetPosition(data, sig, i);               
            }

            base.RunStrategyBase(data);
        }
    }
}
