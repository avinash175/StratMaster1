using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CommonLib
{
    public class StrategyStats
    {
        public List<double> TotalMTM { get; set; }
        public List<int> NumTrades { get; set; }
        public List<double> StrikeRate { get; set; }
        public List<double> AvgRet { get; set; }
        public List<double> WorstMTM2TV { get; set; }
        public List<double> Win2LoseRatio { get; set; }
        public List<TimeSeries> DrawDown { get; set; }
        public List<TimeSeries> DrawDownAbs { get; set; }
        public List<double> MaxDrawDown { get; set; }
        public List<double> MaxDrawDownAbs { get; set; }
        public List<double> AvgTradeDuration { get; set; }
        public List<double> PerDurationInTrade { get; set; }
        public List<TimeSeries> MTM { get; set; }
        public List<TimeSeries> GTV { get; set; }
        public List<TimeSeries> GE { get; set; }
        public List<List<Trade>> Trades { get; set; }
        public List<List<TimeStamp>> MOM { get; set; }
        public List<List<TimeStamp>> HOH { get; set; }
        public List<List<TimeStamp>> DODMTM { get; set; }
        public List<List<TimeStamp>> DODTV { get; set; }
        public List<List<TimeStamp>> YOY { get; set; }
        public List<List<TimeStamp>> MOMm2v { get; set; }
        public List<List<TimeStamp>> YOYm2v { get; set; }
        public List<double> PerPosMonths { get; set; }
        public List<double> AnnRet { get; set; }
        public List<double> AnnVol { get; set; }
        public List<double> Return2Risk { get; set; }
        public List<double> MTM2TV { get; set; }        
        
        public List<int> NumOfContNegMonths { get; set; }

        public StrategyStats()
        {
            TotalMTM = new List<double>();
            NumTrades = new List<int>();
            StrikeRate = new List<double>();            
            AvgRet = new List<double>();
            WorstMTM2TV = new List<double>();
            MaxDrawDown = new List<double>();
            MaxDrawDownAbs = new List<double>();
            MTM = new List<TimeSeries>();
            GTV = new List<TimeSeries>();
            GE = new List<TimeSeries>();
            DrawDown = new List<TimeSeries>();
            Trades = new List<List<Trade>>();            
        }
    }
}
