using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class REIStrategy : BasicStrategy
    {
        public object Period = 8;
        public object Thresh = 60;
        public object UseOHLC = 1;
  
        public REIStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int period = Convert.ToInt32(Period);
            double thresh = Convert.ToDouble(Thresh);
            bool useOHLC = Convert.ToInt32(UseOHLC) == 1? true : false;
            
            for(int i=0;i<numSec;i++)
            {
                double[] rei = Technicals.REI(data.InputData[i], period, useOHLC);
                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 1; j < sig.Length; j++)
                {
                    if (rei[j-1] < -thresh && rei[j]>-thresh)
                    {
                        sig[j] = 1;
                    }
                    else if (rei[j-1] > thresh && rei[j]<thresh)
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
