using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class CCIStrategy : BasicStrategy 
    {
        public object Window = 20;
        public object Thresh = 100;

        public CCIStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int window = Convert.ToInt32(Window);
            double thresh = Convert.ToDouble(Thresh);

            for (int i = 0; i < numSec; i++)
            {
                double[] cci = Technicals.CCI(data.InputData[i], window);
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    if (cci[j] > thresh)
                    {
                        sig[j] = 1;
                    }
                    else if (cci[j] < -thresh)
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
