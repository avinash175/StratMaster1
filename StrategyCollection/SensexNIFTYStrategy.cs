using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class SensexNIFTYStrategy : BasicStrategy
    {
        public object TMALength = 100;
        public object PerBand = 0.01;
        public object MaxSpread = 8;
        public object StartTime = 9.33;

        public SensexNIFTYStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            
            int tmaP = Convert.ToInt32(TMALength);
            double pband = Convert.ToDouble(PerBand);
            double maxSprd = Convert.ToDouble(MaxSpread);
            TimeSpan startTime = DateTime.FromOADate(Convert.ToDouble(StartTime)/24.0).TimeOfDay;
            
            for (int i = 0; i < numSec; i++)
            {
                data.InputData[i].Bid = data.InputData[i].OHLC.high;
                data.InputData[i].Ask = data.InputData[i].OHLC.low;

                data.InputData[i].Prices = data.InputData[i].Bid.Select((x, j) => 
                    (x + data.InputData[i].Ask[j]) / 2.0).ToArray();

                double[] nifty = data.InputData[i].OHLC.open;

                double[] tma = Technicals.MovAvg(nifty, tmaP);

                double[] sig = new double[tma.Length];

                for (int j = 5; j < tma.Length; j++)
                {
                    if (nifty[j] > tma[j] * (1 + pband))
                    {
                        sig[j] = 1;
                    }
                    else if (nifty[j] < tma[j] * (1 - pband))
                    {
                        sig[j] = -1;
                    }
                    
                    if (data.InputData[i].Ask[j] - data.InputData[i].Bid[j] <= maxSprd
                        && data.InputData[i].Dates[j].TimeOfDay > startTime)
                    {
                        if (nifty[j] > tma[j] * (1 + pband)
                            && nifty[j - 1] < tma[j - 1] * (1 + pband))
                        {
                            sig[j] = 2;
                        }
                        else if (nifty[j] < tma[j] * (1 - pband)
                            && nifty[j - 1] > tma[j - 1] * (1 - pband))
                        {
                            sig[j] = -2;
                        }
                    }
                }

                base.CalculateNetPosition(data, sig, i, 1.5, -1.5, -0.5, 0.5);                
            }

            base.RunStrategyBase(data);
        }
    }
}
