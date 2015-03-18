using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class FVEStrategy : BasicStrategy
    {
        public object Window = 20;
        public object Fac = 0.003;
        public object Thresh = 1.0;

        public FVEStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int window = Convert.ToInt32(Window);
            double thresh = Convert.ToDouble(Thresh); 
            double fac = Convert.ToDouble(Fac);

            for (int i = 0; i < numSec; i++)
            {
                double[] fve = Technicals.FVE(data.InputData[i], window, fac); 
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    //Long
                    if (fve[j] > thresh)
                    {
                        sig[j] = 2.0;
                    }
                    //Short
                    else if (fve[j] < -thresh)
                    {
                        sig[j] = -2.0;
                    }
                }

                base.CalculateNetPosition(data, sig, i, 1.5, -1.5, -0.5, 0.5);
            }

            base.RunStrategyBase(data);
        }
    }
}
