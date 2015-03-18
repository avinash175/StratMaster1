using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class BollingerStrategy : BasicStrategy
    {
        public object SMAPeriod = 20;        
        public object Sigma = 1.0;        
        
        public BollingerStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            double sigma = Convert.ToDouble(Sigma);
            int smaP = Convert.ToInt32(SMAPeriod);            

            for (int i = 0; i < numSec; i++)
            {
                List<double[]> temp = Technicals.BollingerBand(data.InputData[i].Prices, smaP, sigma);
                double[] sma = temp[0];
                double[] up = temp[1];
                double[] down = temp[2];

                double[] sig = data.InputData[i].Prices.Select((x, j) => x > up[j] ? 1.0 : x < down[j] ? -1.0 : 0.0).ToArray();
                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}

