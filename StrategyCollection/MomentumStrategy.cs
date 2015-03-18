using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class MomentumStrategy : BasicStrategy
    {
        public object EMAPeriod = 20;
       
        public MomentumStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            
            int emaP = Convert.ToInt32(EMAPeriod);
                       
            for (int i = 0; i < numSec; i++)
            {
                double[] ema = Technicals.ExpMovAvg(data.InputData[i].Prices, emaP);
                
                double[] sig = UF.ArraySub(data.InputData[i].Prices, ema);

                sig = sig.Select((x, j) => data.InputData[i].Prices[j] == 0 ? 0.0 :
                    x / data.InputData[i].Prices[j]).ToArray();

                base.CalculateNetPosition(data, sig, i);                
            }

            base.RunStrategyBase(data);
        }
    }
}
