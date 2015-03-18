using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class LowHistVolRule: BasicRule
    {
        public object LBWinSmall = 22;
        public object LBWinLarge = 100;       

        public LowHistVolRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(TimeSeries ts, double[] sig)
        {
            int lbwins = Convert.ToInt32(LBWinSmall);
            int lbwinl = Convert.ToInt32(LBWinLarge);
            
            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] roc = Technicals.ROC(ts.Prices,1).Select(x=>x/100.0).ToArray();
            
            for (int i = 0; i < len; i++)
            {
                double retWinS = UF.StandardDeviation(UF.GetRange(roc, Math.Max(i - lbwins, 0), i));
                double retWinL = UF.StandardDeviation(UF.GetRange(roc, Math.Max(i - lbwinl, 0), i));

                if (retWinS < retWinL)
                {
                    ret[i] = 1;
                }          
            }

            if (Convert.ToInt32(IsReverse)!=0)
            {
                int max = ret.Max();
                int min = ret.Min();
                min = min == max ? 0 : min;
                ret = ret.Select(x => x == max ? min : x == min ? max : x)
                    .Select(x => x < 0 ? 0 : x).ToArray();
            }

            return ret;
        }
    }
}
