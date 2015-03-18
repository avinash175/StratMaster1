using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CommonLib
{
    public class Option
    {
        public string Instrument { get; set; }
        public string Symbol { get; set; }
        public double UndPrice { get; set; }
        public double Strike { get; set; }
        public DateTime Now { get; set; }
        public DateTime Expiry { get; set; }        
        public TypeOfOption CallPut { get; set; }
        public double ValLTP { get; set; }
        public double ValBid { get; set; }
        public double ValAsk { get; set; }
        public double Rate;

        public double IV { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Vega { get; set; }

        public Option()
        {

        } 

        public Option(Option rhs)
        {
             // get all the fields in the class
             FieldInfo[] fields_of_class = this.GetType().GetFields( 
              BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
 
            // copy each value over to 'this'
             foreach( FieldInfo fi in fields_of_class )
             {
                 fi.SetValue( this, fi.GetValue( rhs ) );
             }
        }

        public Option(string symbol, double strike, DateTime expiry,  TypeOfOption to)
        {
            Symbol = symbol;
            Expiry = expiry;
            Strike = strike;
            CallPut = to;
        }

    }

    public enum TypeOfOption
    {
        CALL,
        PUT,
        FUT,
        STK
    }
}
