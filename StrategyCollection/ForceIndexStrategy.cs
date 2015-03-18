using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class ForceIndexStrategy : BasicStrategy
    {
        public object Window = 10;
        public object Thresh = 0;

        public ForceIndexStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int window = Convert.ToInt32(Window);
            double thresh = Convert.ToDouble(Thresh)*10000000;

            for (int i = 0; i < numSec; i++)
            {
                double[] fi = Technicals.ForceIndex(data.InputData[i],window);
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    if (fi[j] > thresh)
                    {
                        sig[j] = 1;
                    }
                    else if (fi[j] < -thresh)
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
