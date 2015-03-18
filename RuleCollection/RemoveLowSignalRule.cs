using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace RuleCollection
{
    public class RemoveLowSignalRule : BasicRule
    {
        public object Threshold = 0.0;        
       
        public RemoveLowSignalRule(string ruleName)
            : base(ruleName)
        {

        }

        public override int[] RunRule(CommonLib.TimeSeries ts, double[] sig)
        {            
            double thresh = Convert.ToDouble(Threshold);

            int len = sig.Length;
            int[] ret = sig.Select(x => x < thresh ? 0 : 1).ToArray();

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
