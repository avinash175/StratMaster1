using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class SpikeTradingStrategy : BasicStrategy
    {
        public object LBPeriod = 60;
        public object PerThresh = 80;
        public object AbsThresh = 0.1;

        public SpikeTradingStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;            
            int lbperiod = Convert.ToInt32(LBPeriod);
            double perThresh = Convert.ToDouble(PerThresh);
            double absThresh = Convert.ToDouble(AbsThresh);
                       
            for (int i = 0; i < numSec; i++)
            {
                double[] sig = new double[data.InputData[i].Dates.Length];                
                double[] ROC = Technicals.ROC(data.InputData[i].Prices, 1);

                for (int j = 1; j < ROC.Length; j++)
                {
                    if (data.InputData[i].Dates[j].Date != data.InputData[i].Dates[j - 1].Date)
                    {
                        ROC[j] = 0;
                    }
                }

                for (int j = lbperiod; j < data.InputData[i].Dates.Length; j++)
                {
                    List<double> per = UF.PercentileUpDown(UF.GetRange(ROC, j - lbperiod + 1, j), perThresh/100.0);
                    double up = per[0];
                    double down = per[1];

                    if (ROC[j] >= up && ROC[j] > absThresh)
                    {
                        sig[j] = 2;
                    }
                    else if (ROC[j] <= down && ROC[j] < -absThresh)
                    {
                        sig[j] = -2;
                    }
                }
                 
                base.CalculateNetPosition(data, sig, i);                
            }

            base.RunStrategyBase(data);
        }

    }
}
