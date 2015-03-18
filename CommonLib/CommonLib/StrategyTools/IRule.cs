using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CommonLib
{
    public interface IRule
    {
        int[] RunRule(TimeSeries ts, double[] sig);
    }
}
