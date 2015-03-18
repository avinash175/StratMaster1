using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class RangeTradingStrategy : BasicStrategy
    {
        public object LBPeriod = 1;
        public object SizeFac = 1.0;
              
        public RangeTradingStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int lbp = Convert.ToInt32(LBPeriod);
            double sf = Convert.ToDouble(SizeFac);
            
            if (data.SeriesType != TypeOfSeries.OHLC &&
                data.SeriesType != TypeOfSeries.OHLCV)
            {
                throw new Exception("Use OHLC data");
            }
                       
            for (int i = 0; i < numSec; i++)
            {
                double[] sig = new double[data.InputData[i].Dates.Length];
                OHLCDataSet ohlc = data.InputData[i].OHLC;

                double[] range = ohlc.high.Select((x, j) => x - ohlc.low[j]).ToArray();
                
                for (int j = lbp+1; j < ohlc.dates.Length; j++)
                {
                    // If the range is expanding
                    bool rangeExpanding = true;
                    int k = j;

                    while(j - k < lbp)
                    {
                        if (range[k] < sf * range[k - 1])
                        {
                            rangeExpanding = false;
                            break;
                        }
                        k--;
                    }

                    if (rangeExpanding)
                    {
                        if (ohlc.close[j] > ohlc.high[j - 1]
                            && ohlc.close[j] > ohlc.open[j])
                        {
                            sig[j] = 1;
                        }
                        else if (ohlc.close[j] < ohlc.low[j - 1]
                            && ohlc.close[j] < ohlc.open[j])
                        {
                            sig[j] = -1;
                        }
                    }
                }                                
                base.CalculateNetPosition(data, sig, i);                
            }
            base.RunStrategyBase(data);
        }

    }
}
