using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class DojiBreakOutStrategy : BasicStrategy
    {
        public object NTicksFwd = 1;
        public object Eps = 1.0/10000.0;
        public object LbBkt = 5;
        
        public DojiBreakOutStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int nDaysFwd = Convert.ToInt32(NTicksFwd);
            int lbbkt = Convert.ToInt32(LbBkt);           
            double eps = Convert.ToDouble(Eps);

            if (data.SeriesType == TypeOfSeries.OHLC
                || data.SeriesType == TypeOfSeries.OHLCV)
            {
                for (int i = 0; i < numSec; i++)
                {
                    int[] doji = Technicals.DojiIndicator(data.InputData[i].OHLC, nDaysFwd, eps);                    
                    int[] BrkOutInd = Technicals.BreakOutIndicator(data.InputData[i].Prices, lbbkt);

                    double[] Sig = doji.Select((x, j) => (double)x * BrkOutInd[j]).ToArray();

                    base.CalculateNetPosition(data, Sig, i);
                }
                base.RunStrategyBase(data);
            }
            else
            {
                throw new Exception("Use OHLC data for this strategy");
            }
        }
    }
}

