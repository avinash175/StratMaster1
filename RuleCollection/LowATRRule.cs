using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class LowATRRule : BasicRule
    {
        public object LBWinSmall = 10;
        public object LBWinLarge = 100;
        
        public LowATRRule(string ruleName) : base(ruleName)
        {

        }

        public override int[] RunRule(TimeSeries ts, double[] sig)
        {
            if (ts.OHLC == null)
            {
                throw new Exception("Use OHLC data for using this rule");
            }

            int lbwins = Convert.ToInt32(LBWinSmall);
            int lbwinl = Convert.ToInt32(LBWinLarge);            
            
            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] atrs = Technicals.ATR(ts.OHLC, lbwins);
            double[] atrl = Technicals.ATR(ts.OHLC, lbwinl);
            
            for (int i = 0; i < len; i++)
            {                
                if(atrs[i] < atrl[i])
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
