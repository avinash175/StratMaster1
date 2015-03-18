using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    // Analysis  Pending
    // Trys to catch retracement
    public class ReversalStrategy: BasicStrategy
    {
        public object WindowL_1 = 5;
        public object WindowL_2 = 5;
        public object WindowL_3 = 5;
        public object LongPeriodMA = 60;

        public object R1_2 = 0.5;
        public object R2_3 = 0.8;

        public ReversalStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            
            int l1 = Convert.ToInt32(WindowL_1);
            int l2 = Convert.ToInt32(WindowL_2);
            int l3 = Convert.ToInt32(WindowL_3);
            int maP = Convert.ToInt32(LongPeriodMA);

            double r12 = Convert.ToDouble(R1_2);
            double r23 = Convert.ToDouble(R2_3);
                        
            for (int i = 0; i < numSec; i++)
            {
                double[] sig = new double[data.InputData[i].Dates.Length];
                double[] ma = Technicals.MovAvg(data.InputData[i].Prices, maP);

                for (int j = l1 + l2 + l3; j < data.InputData[i].Dates.Length; j++)
                {
                    double r1 = data.InputData[i].Prices[j] / data.InputData[i].Prices[j - l1] - 1;
                    double r2 = data.InputData[i].Prices[j - l1] / data.InputData[i].Prices[j - l1 - l2] - 1;
                    double r3 = data.InputData[i].Prices[j - l1 - l2] / data.InputData[i].Prices[j - l1 - l2 - l3] - 1;

                    if (r2 > 0 && r3 > 0)
                    {
                        if (r2 < r23 * r3 
                            && r1 < r12 * r2
                            //&& r1+r2+r3>0
                            && data.InputData[i].Prices[j] < data.InputData[i].OHLC.low[j-1]
                            && data.InputData[i].Prices[j] < ma[j])
                        {
                            sig[j] = -1;
                        }
                    }
                    else if(r2<0 && r3<0)
                    {
                        if (r2 > r23 * r3
                            && r1 > r12 * r2
                            //&& r1+r2+r3<0
                            && data.InputData[i].Prices[j] > data.InputData[i].OHLC.high[j-1]
                            && data.InputData[i].Prices[j] > ma[j])
                        {
                            sig[j] = 1;
                        }
                    }
                }

                base.CalculateNetPosition(data, sig, i);                
            }

            base.RunStrategyBase(data);
        }
    }
}
