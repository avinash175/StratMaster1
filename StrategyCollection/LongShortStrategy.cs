using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class LongShortContraStrategy : BasicStrategy
    {

        public object IsReverse = 0;
        public object NumStks = 10;
        public object LookBack = 10;       

        public LongShortContraStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            if (numSec < 2)
                throw new Exception("Use more than one security for this strategy");

            int numPoints = data.InputData[0].Dates.Length;
            int isreverse = Convert.ToInt32(IsReverse);
            int numStks = Convert.ToInt32(NumStks);
            int winLen = Convert.ToInt32(LookBack); 
        
            for (int i = 0; i < numSec; i++)
            {
                NetPositions.Add(new int[numPoints]);
            }

            for (int i = winLen; i < numPoints; i++)
            {                
                double[] ret = new double[numSec];

                for (int j = 0; j < numSec; j++)
                {
                    if (data.InputData[j].Prices[i] > 0)
                    {
                        ret[j] = (data.InputData[j].Prices[i] / data.InputData[j].Prices[i - winLen] - 1);
                        
                    }
                    else
                        ret[j] = Double.PositiveInfinity;
                }                

                int[] buyIdx = UF.BubbleSortIdx(ret, true).Take(numStks).ToArray();
                int[] selIdx = UF.BubbleSortIdx(ret.Select(x=>x==Double.PositiveInfinity?-x:x).ToArray()
                    , false).Take(numStks).ToArray();

                for (int j = 0; j < selIdx.Length; j++)
                {
                    NetPositions[selIdx[j]][i] = isreverse == 0 ? -1 : 1;
                }
                for (int j = 0; j < buyIdx.Length; j++)
                {
                    NetPositions[buyIdx[j]][i] = isreverse == 0 ? 1 : -1;
                }              

            }

            for (int i = 0; i < numSec; i++)
            {
                double[] sig = NetPositions[i].Select(x => (double)x).ToArray();
                base.CalculateNetPosition(data, sig, i);
            }

            base.RunStrategyBase(data);
        }
    }
}
