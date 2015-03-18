using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class TradingCost
    {
        public static readonly double ZeroCost = 0.0;        
        public static readonly double BPHalf = 0.5 / 10000.0;
        public static readonly double BP01 = 1.0 / 10000.0;        
        public static readonly double BP02 = 2.0 / 10000.0;
        public static readonly double BP05 = 5.0 / 10000.0;
        public static readonly double BP10 = 10.0 / 10000.0;
        public static readonly double BP15 = 15.0 / 10000.0;
    }

    public class TimeStep // in terms of year
    {        
        public static readonly double Hourly = 1.0 / (252.0 * 7.0);
        public static readonly double Min30 = 1.0 / (252.0 * 7.0 * 2);
        public static readonly double Min15 = 1.0 / (252.0 * 7.0 * 4);
        public static readonly double Daily = 1.0 / 252.0;
        public static readonly double Weekly = 1.0 / 52.0;
        public static readonly double Min10 = 1.0 / (252.0 * 7.0 * 6);
        public static readonly double Min05 = 1.0 / (252.0 * 7.0 * 12);
        public static readonly double Min01 = 1.0 / (252.0 * 7.0 * 60);
        public static readonly double Sec30 = 1.0 / (252.0 * 7.0 * 60 * 2);
        public static readonly double Sec01 = 1.0 / (252.0 * 7.0 * 60 * 60);
    }
}
