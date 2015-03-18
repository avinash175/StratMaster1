using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class MAMAStrategy : BasicStrategy
    {
        public object Thresh=0.1;
        public object Slowlimit = 0.05;
        public object Fastlimit = 0.5;

        public MAMAStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            double thresh = Convert.ToDouble(Thresh) / 100.0;
            double slowlimit = Convert.ToDouble(Slowlimit);
            double fastlimit = Convert.ToDouble(Fastlimit);

            for (int i = 0; i < numSec; i++)
            {
                List<double[]> ret = Technicals.MamaFama(data.InputData[i],slowlimit,fastlimit);
                double[] mama = ret[0];
                double[] fama = ret[1];

                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 1; j < sig.Length; j++)
                {
                    bool longflag = false;
                    bool shortflag = false;

                    if (mama[j - 1] < fama[j - 1] && mama[j] > fama[j])
                    {
                        longflag = true;
                    }
                    else if (mama[j - 1] > fama[j - 1] && mama[j] < fama[j])
                    {
                        shortflag = true;
                    }

                    if (longflag && mama[j] - fama[j] > thresh * data.InputData[i].Prices[j])
                    {
                        sig[j] = 1;
                        longflag = false;
                    }
                    else if (shortflag && fama[j] - mama[j] > thresh * data.InputData[i].Prices[j])
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
