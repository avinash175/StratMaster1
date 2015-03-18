using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class OHLCDataSet
    {
        public double[] open, high, low, close, volume;
        public DateTime[] dates;
        public int numOfElements;

        // Constructors
        public OHLCDataSet(int _numOfElements)
        {
            numOfElements = _numOfElements;

            open = new double[numOfElements];
            high = new double[numOfElements];
            low = new double[numOfElements];
            close = new double[numOfElements];
            volume = new double[numOfElements];

            dates = new DateTime[numOfElements];
        }

        public OHLCDataSet(DateTime[] _dates, double[] _open, double[] _high,
            double[] _low, double[] _close, double[] _volume = null)
        {
            numOfElements = _open.Length;

            open = _open;
            high = _high;
            low = _low;
            close = _close;
            volume = _volume;
            dates = _dates;
        }

        //Copy constructor
        public OHLCDataSet(OHLCDataSet series)
        {
            this.numOfElements = series.numOfElements;
            open = new double[numOfElements];
            high = new double[numOfElements];
            low = new double[numOfElements];
            close = new double[numOfElements];
            dates = new DateTime[numOfElements];
            UF.Copy1DArrayL2R(series.open, ref this.open);
            UF.Copy1DArrayL2R(series.high, ref this.high);
            UF.Copy1DArrayL2R(series.low, ref this.low);
            UF.Copy1DArrayL2R(series.close, ref this.close);
            UF.Copy1DArrayL2RDates(series.dates, ref this.dates);

            if (series.volume != null)
            {
                volume = new double[numOfElements];
                series.volume.CopyTo(volume, 0);
            }
        }

        public OHLCBar[] Convert2OHLCBars()
        {
            OHLCBar[] ret = new OHLCBar[dates.Length];
            for (int i = 0; i < dates.Length; i++)
            {
                ret[i] = new OHLCBar(dates[i], open[i], high[i], low[i], close[i]);
            }
            return ret;
        }

        public OHLCDataSet ChangeTimeSpan(TimeInterval ti)
        {
            int sec = (int)ti;
            //TimeSpan ts = new TimeSpan(((long)sec) * 10000000L);
            double interval = Technicals.Change(dates.Take(5).ToArray(), 1).Skip(1).Min() * 24*60*60;
            DateTime time = new DateTime(0, 0, 0, 0, 0, sec);

            if (time.ToOADate() > interval)
            {
                return null;
            }

            List<int[]> idxGrps = dates.Select((x, i) => new { Date = x, Idx = i}).
                GroupBy(x => x.Date.Date).Select(x => x.Select(y=>y.Idx).ToArray()).ToList();

            for (int i = 0; i < idxGrps.Count; i++)
            {
                int[] idxDay = idxGrps[i];

            }

            return this;

        }
    }

    public class OHLCBar
    {
        public DateTime date {get;set;}
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }

        public OHLCBar(DateTime _date, double _open, double _high,
            double _low, double _close)
        {
            date = _date;
            open = _open;
            high = _high;
            low = _low;
            close = _close;
        }
    }
}
