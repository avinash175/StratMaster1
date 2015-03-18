using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class CandleStick
    {
        public UpDownType UpDown { get; set; }
        public RangeType Range { get; set; }
        public LengthType BodyLength { get; set; }
        public LengthType UpperShadow { get; set; }
        public LengthType LowerShadow { get; set; }
        public int CandleType {get;set;} // 11234 -> DRBUL
        public string HistoryBars { get; set; }
        public double FwdReturn { get; set; }

        public CandleStick()
        {
            Range = RangeType.MEDIUM;
            HistoryBars = "";
        }

        public override string ToString()
        {
            string str = UpDown.ToString() + "," + Range.ToString() + "," + BodyLength.ToString()
                + "," + UpperShadow.ToString() + "," + LowerShadow.ToString() + "," 
                + CandleType.ToString() + "," + HistoryBars;
            return str;
        }
    }

    public class CandleStickSeries
    {
        public DateTime[] Dates { get; set; }
        public CandleStick[] CandleSticks { get; set; }

        public CandleStickSeries(OHLCDataSet data)
        {
            Dates = data.dates;
            
            CandleSticks = new CandleStick[Dates.Length];
            for (int i = 0; i < Dates.Length; i++)
            {
                CandleSticks[i] = new CandleStick();
                CandleSticks[i].CandleType = 0;
                double range = data.high[i] - data.low[i];
                double body = Math.Abs(data.open[i] - data.close[i]);
                double upper = Math.Min(data.high[i] - data.close[i],
                    data.high[i] - data.open[i]);
                double lower = Math.Min(data.close[i] - data.low[i],
                    data.open[i] - data.low[i]);
                CandleSticks[i].UpDown = data.open[i] > data.close[i] ?
                    UpDownType.BLACK : UpDownType.WHITE;

                CandleSticks[i].CandleType += data.open[i] > data.close[i] ?
                    10000 : 20000;

                if (body / range < 0.33)
                {
                    CandleSticks[i].BodyLength = LengthType.SMALL;
                    CandleSticks[i].CandleType += 100;
                }                
                else if (body / range < 0.66)
                {
                    CandleSticks[i].BodyLength = LengthType.MEDIUM;
                    CandleSticks[i].CandleType += 200;
                }
                else
                {
                    CandleSticks[i].BodyLength = LengthType.LARGE;
                    CandleSticks[i].CandleType += 300;
                }
                
                if (upper / range < 0.33)
                {
                    CandleSticks[i].UpperShadow = LengthType.SMALL;
                    CandleSticks[i].CandleType += 10;
                }                
                else if (upper / range < 0.66)
                {
                    CandleSticks[i].UpperShadow = LengthType.MEDIUM;
                    CandleSticks[i].CandleType += 20;
                }
                else
                {
                    CandleSticks[i].UpperShadow = LengthType.LARGE;
                    CandleSticks[i].CandleType += 30;
                }                

                if (lower / range < 0.33)
                {
                    CandleSticks[i].LowerShadow = LengthType.SMALL;
                    CandleSticks[i].CandleType += 1;
                }                
                else if (lower / range < 0.66)
                {
                    CandleSticks[i].LowerShadow = LengthType.MEDIUM;
                    CandleSticks[i].CandleType += 2;
                }                
                else
                {
                    CandleSticks[i].LowerShadow = LengthType.LARGE;
                    CandleSticks[i].CandleType += 3;
                }

                if (i > 100)
                {
                    double[] ra = new double[100];
                    for (int j = i - 100; j < i; j++)
                    {
                        ra[j - (i - 100)] = data.high[j] - data.low[j];
                    }

                    CandleSticks[i].Range = range < UF.Percentile(ra, 0.33) ?
                        RangeType.SMALL : range > UF.Percentile(ra, 0.66) ?
                        RangeType.LARGE : RangeType.MEDIUM;
                }
                if(i>1)
                {
                    CandleSticks[i].HistoryBars = CandleSticks[i-1].CandleType.ToString() +
                        CandleSticks[i].CandleType.ToString();
                }
                if (i < Dates.Length - 1)
                {
                    CandleSticks[i].FwdReturn = data.dates[i+1].DayOfYear == data.dates[i+1].DayOfYear?
                        data.close[i + 1] / data.close[i] - 1 : 0;
                }

                CandleSticks[i].CandleType += CandleSticks[i].Range == RangeType.SMALL ? 1000 :
                        CandleSticks[i].Range == RangeType.MEDIUM ? 2000 : 3000;                
            }
        }

        public Dictionary<int,int> FreqDist()
        {
            return CandleSticks.GroupBy(x => x.CandleType).Select(x => new 
                { Key = x.Key, Value = x.Count() }).ToDictionary(x=>x.Key,x=>x.Value);            
        }

        public Dictionary<string, int> FreqDistHistBars()
        {
            return CandleSticks.GroupBy(x => x.HistoryBars).Select(x => new 
                { Key = x.Key, Value = x.Count() }).ToDictionary(x => x.Key, x => x.Value);
        }


        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < CandleSticks.Length; i++)
            {
                str.Append(CandleSticks[i].ToString()+"\n");                
            }
            return str.ToString();
        }
    }

    public enum UpDownType
    {
        WHITE,
        BLACK        
    }

    public enum RangeType
    {
        SMALL,
        MEDIUM,
        LARGE
    }

    //public enum LengthType
    //{
    //    VERY_SMALL,
    //    SMALL,
    //    MEDIUM,
    //    LARGE,
    //    VERY_LARGE
    //}

    public enum LengthType
    {        
        SMALL,
        MEDIUM,
        LARGE        
    }
}
