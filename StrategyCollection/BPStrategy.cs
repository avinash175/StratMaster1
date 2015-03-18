using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class BPStrategy : BasicStrategy
    {
        public object Period1 = 5;
        //public object Period2 = 10;
        //public object Period3 = 15;
        //public object Period4 = 20;

        public object UseSMA = 0;

        public BPStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int period1 = Convert.ToInt32(Period1);
            //int period2 = Convert.ToInt32(Period2);
            //int period3 = Convert.ToInt32(Period3);
            //int period4 = Convert.ToInt32(Period4);

            bool useSMA = Convert.ToInt32(UseSMA) == 0 ? false : true;

            for (int i = 0; i < numSec; i++)
            { 
                double[] swak = Quant.SWAK(data.InputData[i].Prices, period1, FilterType.BUTTER);
                double[] swak1 = Technicals.MovAvg(data.InputData[i].Prices, period1); //Quant.SWAK(data.InputData[i].Prices, period1, FilterType.SMA);
                //double[] swak2 = Quant.SWAK(data.InputData[i].OHLC.close, period3, FilterType.BP);
                //double[] swak3 = Quant.SWAK(data.InputData[i].OHLC.close, period4, FilterType.BP);

                double[] sig = new double[data.InputData[i].Dates.Length];

                for (int j = 1; j < sig.Length; j++)
                {
                    if (useSMA)
                    {
                        if (data.InputData[i].Prices[j] > swak1[j] && data.InputData[i].Prices[j - 1] < swak1[j - 1])
                        {
                            sig[j] = 1;
                        }
                        else if (data.InputData[i].Prices[j] < swak1[j] && data.InputData[i].Prices[j - 1] > swak1[j - 1])
                        {
                            sig[j] = -1;
                        }
                    }

                    else
                    {
                        if (data.InputData[i].Prices[j] > swak[j] && data.InputData[i].Prices[j - 1] < swak[j - 1])
                        {
                            sig[j] = 1;
                        }
                        else if (data.InputData[i].Prices[j] < swak[j] && data.InputData[i].Prices[j - 1] > swak[j - 1])
                        {
                            sig[j] = -1;
                        }
                    }
                }

                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}


 