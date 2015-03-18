using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class TestStrategy: BasicStrategy
    {
        public object EMAPeriod = 20;
        public object LongAbove = 0.0;
        public object ShortBelow = 0.0;

        public TestStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            double la = Convert.ToDouble(LongAbove);
            double sb = Convert.ToDouble(ShortBelow);
            int emaP = Convert.ToInt32(EMAPeriod);

            for (int i = 0; i < numSec; i++)
            {
                double[] ema = Technicals.ExpMovAvg(data.InputData[i].Prices, emaP);
                double[] diff = UF.ArraySub(data.InputData[i].Prices, ema);
                diff = diff.Select((x, j) => data.InputData[i].Prices[j] == 0 ? 0.0 :
                    x / data.InputData[i].Prices[j]).ToArray();

                base.CalculateNetPosition(data, diff, i, la, sb);                
            }

            base.RunStrategyBase(data);
        }

    }
}
