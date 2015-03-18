using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Linq;
using CommonLib;

namespace CommonLib
{
    public class CurrData
    {
        public CurrDailyData Today;
        public CurrDailyData Yest;
        public string[] FileNames;
        public int TodayIdx;
    }

    public class CurrDailyData
    {
        public DateTime Date;
        public DateTime[] Time { get; set; }
        public double[] LTP { get; set; }
        public double[] Bid { get; set; }
        public double[] Ask { get; set; }
        public double[] LTPVol { get; set; }
        public double[] BidVol { get; set; }
        public double[] AskVol { get; set; }
        public int TimeStep = 10;

        private string fileName;

        public CurrDailyData()
        {

        }

        public CurrDailyData(string _fileName)
        {
            fileName = _fileName;
        }
                
        public void ReadData()
        {
            StreamReader sr = new StreamReader(fileName);

            string line = sr.ReadLine();
            CultureInfo ci = new CultureInfo("en-US");
            Date = DateTime.ParseExact(line.Split(',')[1], "yyyyMMdd", ci);
            sr.ReadLine();           

            List<string[]> Lines = new List<string[]>();

            while (sr.Peek()>=0)
            {
                line = sr.ReadLine();
                string[] fields = line.Split(',');

                if (fields[2].Trim()=="")
                {                    
                    continue;
                }

                Lines.Add(fields);                
            }       

            sr.Close();

            int[] idx = Lines.Select((x,i) => new {x,i}).Where(x => DateTime.ParseExact(x.x[1].Trim(),
                "HH:mm:ss", ci).Second % TimeStep == 0).Select(x => x.i).ToArray();
           
            Time = Lines.Select(x => DateTime.ParseExact(x[1].Trim(), "HH:mm:ss", ci)).
                Where(x => x.Second % TimeStep == 0).ToArray();

            LTP = Lines.Select(x => Double.Parse(x[2])).ToArray();
            LTP = UF.ReplaceZeroByPrev(LTP);
            LTP = LTP.Where((x,i)=> idx.Contains(i)).ToArray();

            Bid = Lines.Where(x => DateTime.ParseExact(x[1].Trim(), "HH:mm:ss", ci).Second % TimeStep == 0).
                Select(x => Double.Parse(x[4])).ToArray();
            Ask = Lines.Where(x => DateTime.ParseExact(x[1].Trim(), "HH:mm:ss", ci).Second % TimeStep == 0).
                Select(x => Double.Parse(x[6])).ToArray();            
        }

        public CurrDailyData Copy()
        {
            return (CurrDailyData)this.MemberwiseClone();
        }
    }
}
