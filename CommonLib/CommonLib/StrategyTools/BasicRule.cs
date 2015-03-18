using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CommonLib
{
    public abstract class BasicRule : IRule
    {
        public string RuleName { get; set; }
        public object IsReverse = 0;

        public BasicRule(string ruleName)
        {
            RuleName = ruleName;
        }       

        public abstract int[] RunRule(TimeSeries ts, double[] sig);        
    }
}
