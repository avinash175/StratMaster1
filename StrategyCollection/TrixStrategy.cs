using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class TrixStrategy : BasicStrategy
    {
        public object Window = 10;

        public TrixStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int window=Convert.ToInt32(Window);

            for(int i=0;i<numSec;i++)
            {
                double[] trix=Technicals.Trix(data.InputData[i],window);
                double[] sig=new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    if (trix[j] > 0)
                        sig[j] = 1;
                    else if (trix[j] < 0)
                        sig[j] = -1;                 
                }

                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}
