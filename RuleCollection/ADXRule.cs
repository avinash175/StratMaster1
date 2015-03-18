using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class ADXRule : BasicRule
    {
        public object Threshold = 20;        
        public object ADXPeriod = 14;

        public ADXRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            int adxP = Convert.ToInt32(ADXPeriod);
            int thresh = Convert.ToInt32(Threshold);   

            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] adx = Technicals.ADX(ts.OHLC, adxP);
            
            for (int i = 0; i < len; i++)
            {
                if (adx[i] > thresh)
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
