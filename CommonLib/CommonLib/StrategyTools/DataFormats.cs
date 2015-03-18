using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class DFMultiScrip
    {
        public DateTime Date { get; set; }
        public string Scrip { get; set; }
        public double[] Values { get; set; }

        public void ParseLine(string line)
        {
            string[] fields = line.Split(',');
            Date = DateTime.FromOADate(Convert.ToDouble(fields[0]));
            Scrip = fields[1];
            Values = new double[fields.Length - 2];

            for (int i = 2; i < fields.Length; i++)
            {
                Values[i-2] = Convert.ToDouble(fields[i]);
            }
        }
    }

    public class DFSingleScrip
    {
        public DateTime Date { get; set; }        
        public double[] Values { get; set; }

        public void ParseLine(string line)
        {
            string[] fields = line.Split(',');
            Date = DateTime.FromOADate(Convert.ToDouble(fields[0]));           
            Values = new double[fields.Length - 1];

            for (int i = 1; i < fields.Length; i++)
            {
                Values[i - 1] = Convert.ToDouble(fields[i]);
            }
        }
    }
    
}
