using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class TIRRule: BasicRule
    {
        public object Threshold = 0.8;        
        public object LongWinPeriod = 100;
        public object NumSW = 10;
        public object DelC = 0.1;
        public object UseRelativeDelC = 1;        

        public TIRRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            int lwP = Convert.ToInt32(LongWinPeriod);
            int numSW = Convert.ToInt32(NumSW);
            double delC = Convert.ToDouble(DelC);
            double thresh = Convert.ToDouble(Threshold);
            bool useRelDelC = Convert.ToInt32(UseRelativeDelC) == 1 ? true : false;

            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] tir = Quant.TimeInRange(ts.Prices, lwP, numSW, delC, useRelDelC);
            
            for (int i = 0; i < len; i++)
            {
                if (tir[i] > thresh)
                {
                    ret[i] = 1;
                }
            }

            if (Convert.ToInt32(IsReverse) != 0)
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
