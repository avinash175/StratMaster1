using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class LuxorModified : BasicStrategy
    {
        public object MAFastPeriod = 3;
        public object MASlowPeriod = 20;
        public object BarThresh = 2;

        public LuxorModified(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int maSlowP = Convert.ToInt32(MASlowPeriod);
            int maFastP = Convert.ToInt32(MAFastPeriod);
            int barThresh = Convert.ToInt32(BarThresh);

            if (data.SeriesType == TypeOfSeries.OHLC ||
                data.SeriesType == TypeOfSeries.OHLCV)
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] maSlow = Technicals.MovAvg(data.InputData[i].Prices, maSlowP);
                    double[] maFast = Technicals.MovAvg(data.InputData[i].Prices, maFastP);
                    double[] sig = new double[maSlow.Length];

                    double hh = 0, ll = 0;
                    int barCross = -1000;

                    for (int j = 2; j < maSlow.Length; j++)
                    {
                        if (maFast[j - 2] < maSlow[j - 2]
                            && maFast[j - 1] > maSlow[j - 1]
                            && data.InputData[i].OHLC.high[j] > data.InputData[i].OHLC.high[j - 1])
                        {
                            sig[j] = 2;
                        }
                        else if (maFast[j - 2] > maSlow[j - 2]
                            && maFast[j - 1] < maSlow[j - 1]
                            && data.InputData[i].OHLC.low[j] < data.InputData[i].OHLC.low[j - 1])
                        {
                            sig[j] = -2;
                        }
                        else if (maFast[j - 2] < maSlow[j - 2]
                            && maFast[j - 1] > maSlow[j - 1])
                        {
                            sig[j] = 1;
                            hh = data.InputData[i].OHLC.high[j - 1];
                            ll = 0;
                            barCross = j;
                        }
                        else if (maFast[j - 2] > maSlow[j - 2]
                            && maFast[j - 1] < maSlow[j - 1])
                        {
                            sig[j] = -1;
                            ll = data.InputData[i].OHLC.low[j - 1];
                            hh = 0;
                            barCross = j;
                        }
                        else if (hh > 0
                            && data.InputData[i].OHLC.high[j] > hh
                            && j-barCross < barThresh)
                        {
                            sig[j] = 2;
                        }
                        else if (ll > 0
                            && data.InputData[i].OHLC.low[j] < ll
                            && j - barCross < barThresh)
                        {
                            sig[j] = -2;
                        }
                    }

                    base.CalculateNetPosition(data, sig, i, 1.5, -1.5, -0.5, 0.5);
                }
            }
            else
            {
                throw new Exception("Use OHLC data");
            }

            base.RunStrategyBase(data);
        }
    }
}
