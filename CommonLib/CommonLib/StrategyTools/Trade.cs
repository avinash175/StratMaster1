using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Trade : IComparable
    {
        public string ScripName { get; set; }
        public bool PositionType; // true for Long and false for short
        public LongShortType LongShort { get; set; }
        public DateTime EntryDate { get; set; }
        public double EntryPrice { get; set; }
        public DateTime ExitDate { get; set; }
        public double ExitPrice { get; set; }
        public double Quantity { get; set; }
        public double Return { get; set; }
        public string Description {get; set;}
        public int EntryIdx, ExitIdx;        
        
        public bool isOpen = false;

        public Trade()
        {
        }

        public Trade(bool _PositionType, DateTime _EntryDate, double _EntryPrice,
            DateTime _ExitDate, double _ExitPrice,double qty = 0, string des = "")
        {
            PositionType = _PositionType;
            EntryDate = _EntryDate;
            EntryPrice = _EntryPrice;
            ExitDate = _ExitDate;
            ExitPrice = _ExitPrice;
            isOpen = false;
            Description = des;
            Quantity = qty;

            LongShort = PositionType ? LongShortType.LONG : LongShortType.SHORT;

            Return = (ExitPrice - EntryPrice) / EntryPrice;

            if (PositionType == false) // for shorts
                Return = -Return;
        }

        public Trade(Trade rhs, double cost = 0.0)
        {
            Description = rhs.Description;
            PositionType = rhs.PositionType;
            EntryDate = rhs.EntryDate;
            EntryPrice = rhs.EntryPrice;
            ExitDate = rhs.ExitDate;
            ExitPrice = rhs.ExitPrice;
            EntryIdx = rhs.EntryIdx;
            ExitIdx = rhs.ExitIdx;
            ScripName = rhs.ScripName;
            isOpen = rhs.isOpen;
            LongShort = rhs.PositionType ? LongShortType.LONG : LongShortType.SHORT;
            Quantity = rhs.Quantity;
            
            Return = (ExitPrice - EntryPrice) / EntryPrice;

            if (PositionType == false) // for shorts
                Return = -Return;

            Return = Return - 2*cost;
        }

        public override string ToString()
        {
            int buySell = this.PositionType ? 1 : -1;
            string ret = this.ScripName + "," + this.EntryDate.ToString() + ","
                + this.EntryPrice.ToString() + "," + this.ExitDate.ToString() + ","
                + this.ExitPrice.ToString() + ","
                + buySell.ToString() + "," + this.Return.ToString();
            return ret;
        }

        public int CompareTo(object obj)
        {
            Trade input = (Trade)obj;
            return EntryDate.CompareTo(input.EntryDate);
        }
    }
}
