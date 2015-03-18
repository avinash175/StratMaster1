using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{    
    public class RedGreen:BasicStrategy
    {
        public object RedGreenWindow = 40;   //nifty: 45       
        public object MAWindow = 8;     //nifty: 90

        public RedGreen(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int wd1 = Convert.ToInt32(RedGreenWindow);
            int wd2 = Convert.ToInt32(RedGreenWindow);
            int wd3 = Convert.ToInt32(MAWindow);            

            for (int i = 0; i < numSec; i++)
            {
                int len = data.InputData[i].Dates.Length;
                double[] sig = new double[len];

                double[] ma = Technicals.MovAvg(data.InputData[i].OHLC.close, wd3);
                int cp = 0;
                for (int j = wd1 + 2; j < len; j++)
                {                    
                    double g = (data.InputData[i].OHLC.high[j - wd1] + data.InputData[i].OHLC.high[j - wd1 - 1]) / 2;                   
                    double r = (data.InputData[i].OHLC.low[j - wd2] + data.InputData[i].OHLC.low[j - wd2 - 1]) / 2;        
                    double ltp = data.InputData[i].OHLC.close[j];

                    if (ltp >= g && ltp >= ma[j])
                    {
                        sig[j] = 2.0;
                        cp = 1;
                    }
                    else if (ltp <= r && ltp <= ma[j])
                    {
                        sig[j] = -2.0;
                        cp = -1;
                    }
                    if (cp == 1 && ltp <= ma[j])
                    {
                        sig[j] = -1.0;
                        cp = 0;
                    }
                    else if (cp == -1 && ltp >= ma[j])
                    {
                        sig[j] = 1.0;
                        cp = 0;
                    }
                }

                base.CalculateNetPosition(data, sig, i, 1.5, -1.5, 0, 0);
            }

            base.RunStrategyBase(data);
        }
    }
}
