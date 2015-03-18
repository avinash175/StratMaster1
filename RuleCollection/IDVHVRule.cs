using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class IDVHVRule : BasicRule
    {
        public object IDVPeriod = 5;
        public object HVPeriod = 10;
        public object FacMul = 1.0;

        public IDVHVRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            int hvP = Convert.ToInt32(HVPeriod);
            int idvP = Convert.ToInt32(IDVPeriod);
            double mul = Convert.ToDouble(FacMul);

            int len = ts.Dates.Length;
            int[] ret = new int[len];

            double[] idv = Technicals.ATR(ts.OHLC, idvP);
            double[] hv = Technicals.MovAvg(Technicals.AbsChange(ts.Prices, 1), hvP);

            ret = idv.Select((x, i) => x > hv[i] * mul ? 1 : 0).ToArray();

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
