using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class ChaikinStrategy : BasicStrategy
    {
        public object Thresh = 0;
        public object Smallwindow = 3;
        public object Largewindow = 10;

        public ChaikinStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            double thresh = Convert.ToDouble(Thresh);

            int smallwindow = Convert.ToInt32(Smallwindow);
            int largewindow = Convert.ToInt32(Largewindow);

            for (int i = 0; i < numSec; i++)
            {
                double[] chaikinind = Technicals.ChaikinInd(data.InputData[i], smallwindow, largewindow);

                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 0; j < sig.Length; j++)
                {
                    if (chaikinind[j] > thresh)
                    {
                        sig[j] = 1;
                    }
                    else if (chaikinind[j] < -thresh)
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
