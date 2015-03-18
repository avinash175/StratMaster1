using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class FRAMAStrategy : BasicStrategy
    {
        public object Length = 15;
        public object Thresh = 0.1;

        public FRAMAStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int length = Convert.ToInt32(Length);
            double thresh = Convert.ToDouble(Thresh) / 100.0;

            for (int i = 0; i < numSec; i++)
            {
                double[] frama = Technicals.FRAMA(data.InputData[i], length);

                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 1; j < sig.Length; j++)
                {
                    bool longflag = false;
                    bool shortflag = false;

                    if (data.InputData[i].Prices[j - 1] < frama[j - 1] && data.InputData[i].Prices[j] > frama[j])
                    {
                        longflag = true;
                    }
                    else if (data.InputData[i].Prices[j - 1] > frama[j - 1] && data.InputData[i].Prices[j] < frama[j])
                    {
                        shortflag = true;
                    }

                    if (longflag && data.InputData[i].Prices[j] - frama[j] > thresh * data.InputData[i].Prices[j])
                    {
                        sig[j] = 1;
                        longflag = false;
                    }
                    else if (shortflag && frama[j] - data.InputData[i].Prices[j] > thresh * data.InputData[i].Prices[j])
                    {
                        sig[j] = -1;
                        shortflag = false;
                    }
                }

                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}
