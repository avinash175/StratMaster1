using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class UpTrendRule : BasicRule
    {
        public object LBWinSmall = 5;
        public object LBWinMid = 15;
        public object LBWinLarge = 60;  

        public UpTrendRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            int lbwins = Convert.ToInt32(LBWinSmall);
            int lbwinm = Convert.ToInt32(LBWinMid);
            int lbwinl = Convert.ToInt32(LBWinLarge);

            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] smaS = Technicals.MovAvg(ts.Prices, lbwins);
            double[] smaM = Technicals.MovAvg(ts.Prices, lbwinm);
            double[] smaL = Technicals.MovAvg(ts.Prices, lbwinl);

            for (int i = lbwinl; i < len; i++)
            {
                if (smaS[i] >= smaM[i] && smaM[i] >= smaL[i])                     
                {
                    ret[i] = 1;
                }
                else if(smaS[i] <= smaM[i] && smaM[i] <= smaL[i])
                {
                    ret[i] = -1;
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
