using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class ConsolidationStrategy : BasicStrategy
    {
        public object ATRLookBack = 30;
        public object MaxMinLB = 4;
        public object MaxMinLBPrev = 12;
        public object RangeThresh = 0.3;
        public object EntryFactor = 1.0;

        public ConsolidationStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int atrlb = Convert.ToInt32(ATRLookBack);
            int maxminlb = Convert.ToInt32(MaxMinLB);
            int maxminlbPrev = Convert.ToInt32(MaxMinLBPrev);
            double rangeThresh = Convert.ToDouble(RangeThresh);
            double entryFac = Convert.ToDouble(EntryFactor);
            
            if (data.SeriesType == TypeOfSeries.OHLC ||
                data.SeriesType == TypeOfSeries.OHLCV)
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] atr = Technicals.ATR(data.InputData[i].OHLC, atrlb);
                    double[] HH = Technicals.Extrema(data.InputData[i].OHLC.high, true, maxminlb);
                    double[] LL = Technicals.Extrema(data.InputData[i].OHLC.low, false, maxminlb);
                    double[] HHPrev = Technicals.Extrema(data.InputData[i].OHLC.high, true, maxminlbPrev);
                    double[] LLPrev = Technicals.Extrema(data.InputData[i].OHLC.low, false, maxminlbPrev);

                    double[] sig = new double[atr.Length];
                                        
                    for (int j = maxminlb + maxminlbPrev; j < atr.Length; j++)
                    {
                        if ((HH[j-1] - LL[j-1])/(HHPrev[j-maxminlb-1]-LLPrev[j-maxminlb-1]) < rangeThresh
                            && HH[j-1] - LL[j-1] < atr[j-1]
                            && data.InputData[i].OHLC.high[j] >= HH[j-1] + (entryFac * atr[j-1]))
                        {
                            sig[j] = 1;
                        }
                        else if ((HH[j-1] - LL[j-1]) / (HHPrev[j - maxminlb -1] - LLPrev[j - maxminlb-1]) < rangeThresh
                            && HH[j-1] - LL[j-1] < atr[j-1]
                            && data.InputData[i].OHLC.low[j] <= LL[j-1] - (entryFac * atr[j-1]))
                        {
                            sig[j] = -1;
                        }
                    }

                    base.CalculateNetPosition(data, sig, i);
                }
            }
            else
            {
                throw new Exception("Use OHLC Data");
            }

            base.RunStrategyBase(data);
        }
    
    }
}
