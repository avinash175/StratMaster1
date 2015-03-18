using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class LuxorStrategy : BasicStrategy
    {
        public object MAFastPeriod = 3;
        public object MASlowPeriod = 20;        
                
        public LuxorStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int maSlowP = Convert.ToInt32(MASlowPeriod);
            int maFastP = Convert.ToInt32(MAFastPeriod);
            
            if (data.SeriesType == TypeOfSeries.OHLC ||
                data.SeriesType == TypeOfSeries.OHLCV)
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] maSlow = Technicals.MovAvg(data.InputData[i].Prices, maSlowP);
                    double[] maFast = Technicals.MovAvg(data.InputData[i].Prices, maFastP);
                    double[] sig = new double[maSlow.Length];
                                        
                    for (int j = 2; j < maSlow.Length; j++)
                    {
                        if (maFast[j-2] < maSlow[j-2] 
                            && maFast[j-1] > maSlow[j-1]
                            && data.InputData[i].OHLC.high[j] > data.InputData[i].OHLC.high[j-1])
                        {
                            sig[j] = 1;
                        }
                        else if (maFast[j - 2] > maSlow[j - 2]
                            && maFast[j - 1] < maSlow[j - 1]
                            && data.InputData[i].OHLC.low[j] < data.InputData[i].OHLC.low[j - 1])
                        {
                            sig[j] = -1;
                        }
                    }

                    base.CalculateNetPosition(data, sig, i);
                }
            }
            else
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] maSlow = Technicals.MovAvg(data.InputData[i].Prices, maSlowP);
                    double[] maFast = Technicals.MovAvg(data.InputData[i].Prices, maFastP);
                    double[] sig = new double[maSlow.Length];

                    for (int j = 2; j < maSlow.Length; j++)
                    {
                        if (maFast[j - 2] < maSlow[j - 2]
                            && maFast[j - 1] > maSlow[j - 1]
                            && data.InputData[i].Prices[j] > data.InputData[i].Prices[j - 1])
                        {
                            sig[j] = 1;
                        }
                        else if (maFast[j - 2] > maSlow[j - 2]
                            && maFast[j - 1] < maSlow[j - 1]
                            && data.InputData[i].Prices[j] < data.InputData[i].Prices[j - 1])
                        {
                            sig[j] = -1;
                        }
                    }

                    base.CalculateNetPosition(data, sig, i);
                }
            }

            base.RunStrategyBase(data);
        }
    }
}
