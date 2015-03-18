using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class TimeSeries
    {
        public string Name { get; set; }
        public DateTime[] Dates;
        public double[] Prices;
        public double[] Bid;
        public double[] Ask;
        public double[] Extra1;
        public double[] Extra2;
        public double[] Extra3;
        public double[] Extra4;
        public Option OptionDetails;
        public OHLCDataSet OHLC;
        
        public TimeSeries()
        {

        }
        public TimeSeries(string name)
        {
            Name = name;
        }
        public TimeSeries(DateTime[] _dates, double[] _price)
        {
            Dates = _dates;
            Prices = _price;
        }

        public TimeSeries(DateTime[] _dates, double[] open, double[] high,
            double[] low, double[] close, double[] vol = null)
        {
            Dates = _dates;
            Prices = close;
            OHLC = new OHLCDataSet(_dates, open, high, low, close, vol);
        }

        public TimeSeries(DateTime[] _dates, double[] bid, double[] ask, double[] price)
        {
            Dates = _dates;
            Prices = price;
            Bid = bid;
            Ask = ask;            
        }

        public TimeStamp[] ConvertToTimeStampArray()
        {
            TimeStamp[] ret = null;
            if (Dates != null)
            {
                ret = new TimeStamp[Dates.Length];
                if (OHLC == null && Bid == null)
                {
                    for (int i = 0; i < Dates.Length; i++)
                    {
                        ret[i] = new TimeStamp(Dates[i], Prices[i]);
                    }
                }
                else if (OHLC == null)
                {
                    for (int i = 0; i < Dates.Length; i++)
                    {
                        ret[i] = new TimeStamp(Dates[i], Bid[i], Ask[i], Prices[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < Dates.Length; i++)
                    {
                        ret[i] = new TimeStamp(Dates[i], OHLC.open[i],OHLC.high[i], 
                            OHLC.low[i], OHLC.close[i],OHLC.volume == null? 0 : OHLC.volume[i]);
                    }
                }
            }
            return ret;
        }
    }

    public enum TypeOfSeries
    {
        OHLC,
        LTP,
        BID_ASK_LTP,
        BID_ASK_LTP_VOL,
        OHLCV,        
    }
}
