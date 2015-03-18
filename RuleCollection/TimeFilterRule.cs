using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class TimeFilterRule : BasicRule
    {
        public object StartTime = 9.0;
        public object EndTime = 17.0;
        
        public TimeFilterRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {
            double stTime = Convert.ToDouble(StartTime);
            double endTime = Convert.ToDouble(EndTime);

            int len = ts.Dates.Length;
            int[] ret = ts.Dates.Select(x => (x.Hour + x.Minute/60.0) >= stTime &&
                (x.Hour + x.Minute / 60.0) <= endTime ? 1 : 0).ToArray();

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
