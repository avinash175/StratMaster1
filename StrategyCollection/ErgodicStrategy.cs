using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class ErgodicStrategy : BasicStrategy
    {
        public object ShortPeriod = 5;
        public object LongPeriod = 20;
        public object Period = 5;

        public ErgodicStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int shortperiod=Convert.ToInt32(ShortPeriod);
            int longperiod=Convert.ToInt32(LongPeriod);
            int period=Convert.ToInt32(Period);

            for(int i=0;i<numSec;i++)
            {
                List<double[]> ret=Technicals.ErgodicInd(data.InputData[i],shortperiod,longperiod,period);
                double[] erg=ret[0];
                double[] ergsig=ret[1];

                double[] sig=new double[data.InputData[i].Dates.Length];

                for(int j=2;j<sig.Length;j++)
                {
                    if(erg[j-1]<ergsig[j-1] && erg[j]>ergsig[j])
                    {
                        sig[j]=1;
                    }
                    else if(erg[j-1]>ergsig[j-1] && erg[j]<ergsig[j])
                    {
                        sig[j]=-1;
                    } 
                }

                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }

    
}
