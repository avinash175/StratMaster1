using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class RSIRule: BasicRule
    {
        public object RSILow = 30;
        public object RSIHigh = 70;
        public object RSIPeriod = 14;

        public RSIRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            double rsiL = Convert.ToDouble(RSILow);
            double rsiH = Convert.ToDouble(RSIHigh);
            int rsiP = Convert.ToInt32(RSIPeriod);   

            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] rsi = Technicals.RSI(ts.Prices, rsiP);
            
            for (int i = 0; i < len; i++)
            {
                if (rsi[i] > rsiH || rsi[i] < rsiL)
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
