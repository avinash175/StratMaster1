using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class MACDStrategy : BasicStrategy
    {
        public object SmallWindow = 12;
        public object LargeWindow = 26;

        public MACDStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int sw = Convert.ToInt32(SmallWindow);
            int lw = Convert.ToInt32(LargeWindow);            

            for (int i = 0; i < numSec; i++)
            {
                double[] macd = Technicals.MACD(data.InputData[i].Prices,sw,lw);
                base.CalculateNetPosition(data, macd, i);
            }

            base.RunStrategyBase(data);
        }
    }
}
