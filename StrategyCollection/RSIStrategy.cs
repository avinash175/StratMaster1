using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class RSIStrategy : BasicStrategy
    {
        public object RSILow = 30;
        public object RSIHigh = 70;
        public object RSIPeriod = 14;        

        public RSIStrategy(string stratName, double alloc, double cost, double timeStep) 
            : base(stratName, alloc, cost, timeStep)
        {
            
        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            double rsiL = Convert.ToDouble(RSILow);
            double rsiH = Convert.ToDouble(RSIHigh);
            int rsiP = Convert.ToInt32(RSIPeriod);            

            for (int i = 0; i < numSec; i++)
            {
                double[] rsi = UF.MulArrayByConst(Technicals.RSI(data.InputData[i].Prices, rsiP),-1.0);
                base.CalculateNetPosition(data, rsi, i, -rsiL, -rsiH);                
            }

            base.RunStrategyBase(data);
        }
    }
}
