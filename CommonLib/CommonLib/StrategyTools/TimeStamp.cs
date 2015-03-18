using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class TimeStamp
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Price { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }        
        public double Volume { get; set; }        

        public TimeStamp(DateTime date, double price)
        {
            Price = price;
            Date = date;
        }

        public TimeStamp(DateTime date, double bid, double ask, double price)
        {
            Price = price;
            Date = date;
            Bid = bid;
            Ask = ask;
            Price = price;
        }

        public TimeStamp(DateTime date,double open, double high, 
            double low, double price, double vol = 0)
        {
            Price = price;
            Date = date;
            Open = open;
            Low = low;
            High = high;
            Volume = vol;
        }
    }    
}
