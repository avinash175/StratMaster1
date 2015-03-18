using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class EntropyStrategy : BasicStrategy
    {
        public object Window = 10;
        public object PercThresh = 0.6;

        public EntropyStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int window = Convert.ToInt32(Window);
            double percthresh = Convert.ToDouble(PercThresh);

            for(int i=0;i<numSec;i++)
            {
                double[] entropyind = Technicals.EntropyInd(data.InputData[i], window);
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    if (entropyind[j] > percthresh)
                    {
                        sig[j] = 1;
                    }
                    else if (entropyind[j] < 0.5-(percthresh-0.5))
                    {
                        sig[j] = -1;
                    }                    
                }

                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}
 
