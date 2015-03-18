using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class MassIndexStrategy : BasicStrategy
    {
        public object Emaperiod = 9;
        public object Miperiod = 25;

        public MassIndexStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int emaperiod = Convert.ToInt32(Emaperiod);
            int miperiod = Convert.ToInt32(Miperiod);

            for (int i = 0; i < numSec; i++)
            {
                double[][] ret = Technicals.MassIndex(data.InputData[i], emaperiod, miperiod);
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 1; j < sig.Length; j++)
                {
                    if (ret[1].ElementAt(j) > miperiod && ret[0].ElementAt(j) == 1)
                    {
                        sig[j] = 1;
                    }
                    else if (ret[1].ElementAt(j) > miperiod && ret[0].ElementAt(j) == -1)
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
 