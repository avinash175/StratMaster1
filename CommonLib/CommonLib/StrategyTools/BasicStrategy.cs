using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CommonLib
{
    public abstract class BasicStrategy : IStrategy
    {
        public StrategyStats Stats { get; set; }
        public StrategyStats AggStats { get; set; }
        public string StrategyName;
        public double AllocationPerTrade;
        public double Cost;
        public bool UseSL;
        public double StopLoss;
        public bool UseTrailingSL;
        public double TrailingSL;
        public bool UseTarget;
        public double Target;
        public int MaxHoldPeriod;
        public int SkipPeriod;
        public bool UseBidAsk;
        public bool HoldOverNightPosition;
        public double TimeInterval;
        public bool UseAggStats;
        public bool UseNetPositionAsQty;
        public bool IsReverseStrategy;
        public bool AllowSignalFlip = true;

        public List<int> TotalBuyQty { get; set; }
        public List<double> AvgBuyPrice { get; set; }
        public List<int> TotalSellQty { get; set; }
        public List<double> AvgSellPrice { get; set; }

        public List<BasicRule> Rules { get; set; }

        public List<object[]> rows { get; set; }
        public List<int[]> NetPositions { get; set; }

        public BasicStrategy(string strategyName, double _alloc, double _cost, double _timeStep)
        {
            StrategyName = strategyName;
            AllocationPerTrade = _alloc;
            Cost = _cost;
            TimeInterval = _timeStep;
            Stats = new StrategyStats();
            AggStats = new StrategyStats();
            NetPositions = new List<int[]>();
            Rules = new List<BasicRule>();
            IsReverseStrategy = false;
            MaxHoldPeriod = Int32.MaxValue;            
        }

        public void RunStrategyBase(StrategyData data)
        {
            int numSec = data.InputData.Count;
            for (int i = 0; i < numSec; i++)
            {
                int[] netPos = NetPositions[i];
                List<double[]> res = SP.GenerateMTM(data.InputData[i], netPos, AllocationPerTrade,
                    1, -1, Cost, UseBidAsk, UseNetPositionAsQty);
                Stats.MTM.Add(new TimeSeries(data.InputData[i].Dates, res[0]));
                Stats.GTV.Add(new TimeSeries(data.InputData[i].Dates, res[1]));
                Stats.GE.Add(new TimeSeries(data.InputData[i].Dates, res[2]));
                Stats.Trades.Add(SP.GenerateTrades(data.InputData[i], netPos, UseBidAsk, Cost));
            }

            if (UseAggStats)
            {
                int numAgg = data.SecGrouping.Length;

                for (int i = 0; i < numAgg; i++)
                {
                    int numStks = data.SecGrouping[i].Length;
                    double[] sumMTM = new double[Stats.MTM[0].Dates.Length];
                    double[] sumGTV = new double[Stats.MTM[0].Dates.Length];
                    double[] sumGE = new double[Stats.MTM[0].Dates.Length];
                    for (int j = 0; j < numStks; j++)
                    {
                        int idx = data.SecGrouping[i][j];
                        for (int k = 0; k < sumMTM.Length; k++)
                        {
                            sumMTM[k] += Stats.MTM[idx].Prices[k];
                            sumGTV[k] += Stats.GTV[idx].Prices[k];
                            sumGE[k] += Stats.GE[idx].Prices[k];

                        }
                    }
                    AggStats.MTM.Add(new TimeSeries(data.InputData[i].Dates, sumMTM));
                    AggStats.GTV.Add(new TimeSeries(data.InputData[i].Dates, sumGTV));
                    AggStats.GE.Add(new TimeSeries(data.InputData[i].Dates, sumGE));
                }
            }            
        }

        public void CalculateNetPosition(StrategyData data, double[] signal, int idx, double longAbove = 0.0,
            double shortBelow = 0.0,double  longExBelow = Double.NegativeInfinity, double shortExAbove = Double.PositiveInfinity  )
        {
            int[] filter = GetRuleFilter(data, signal, idx);
            double[] sig = signal.Select((x, j) => x * filter[j]).ToArray();

            int[] netPosition = SP.CreateBuySellSig(sig, data.InputData[idx],
                longAbove, shortBelow,LExB: longExBelow, SExA: shortExAbove,  UseBidAsk: UseBidAsk, HoldOverNightPos: HoldOverNightPosition,
                UseTrailingSL: UseTrailingSL, UseSL: UseSL, UseTarget: UseTarget,
                Stoploss: StopLoss, Target: Target, TrailingSL: TrailingSL, skipPeriod: SkipPeriod,
                UseOpen: UseNetPositionAsQty, IsReverseStrategy: IsReverseStrategy, MaxHoldPeriod: MaxHoldPeriod,
                AllowSignalFlip: AllowSignalFlip);

            NetPositions.Add(netPosition);
        }

        public int[] GetRuleFilter(StrategyData data,double[] sig,int idx)
        {
            int[] ruleRet = UF.Ones(data.InputData[idx].Dates.Length).Select(x => (int)x).ToArray();
            for (int j = 0; j < Rules.Count; j++)
            {
                int[] rule = Rules[j].RunRule(data.InputData[idx], sig);
                ruleRet = ruleRet.Select((x, k) => x * rule[k]).ToArray();
            }
            return ruleRet;
        }

        public List<object[]> ComputeStatistics(StrategyData data)
        {
            if (data == null
                || Stats == null)
            {
                return null;
            }

            Stats.StrikeRate = Stats.Trades.Select(x => x.Count(y => y.Return > 0) / (double)x.Count).ToList();
            Stats.AvgRet = Stats.Trades.Select(x =>x.Count()>0? x.Average(y => y.Return):0.0).ToList();                       
            Stats.NumTrades = Stats.Trades.Select(x => x.Count).ToList();

            //Stats.WorstMTM2TV = Stats.AvgRet.Select((x, i) => Stats.NumTrades[i] * AllocationPerTrade * 
            //    x * (1.0 - 1.0 / Math.Sqrt(Stats.NumTrades[i])) /
            //    (2.0 * Stats.NumTrades[i] * AllocationPerTrade)).ToList();

            Stats.Win2LoseRatio = Stats.Trades.Select(x => x.Where(y => y.Return > 0).Count() > 0 && x.Where(y => y.Return < 0).Count() > 0 
                ? x.Where(y => y.Return > 0).Average(y => y.Return) 
                / x.Where(y => y.Return < 0).Average(y => Math.Abs(y.Return)):Double.NaN).ToList();

            Stats.TotalMTM = Stats.MTM.Select(x => x.Prices.Sum()).ToList();
            
            Stats.MTM2TV = Stats.TotalMTM.Select((x, i) => x / (2.0 * Stats.NumTrades[i] * AllocationPerTrade)).ToList();

            Stats.AvgTradeDuration = Stats.Trades.Select(x =>x.Count()>0? x.Select(y => y.ExitIdx - y.EntryIdx).Average():0.0).ToList();

            Stats.PerDurationInTrade = NetPositions.Select(x => ((double)x.Count(y => y != 0)) / x.Count()).ToList();

            Stats.DrawDown = Stats.MTM.Select(x => new TimeSeries(x.Dates,
                Technicals.DrawDown(UF.MTM2NAV(x.Prices, AllocationPerTrade).Skip(1).ToArray()))).ToList();

            Stats.DrawDownAbs = Stats.MTM.Select(x => new TimeSeries(x.Dates,
                Technicals.DrawDownAbs(UF.CummSum(x.Prices).ToArray()))).ToList();

            Stats.AnnRet = Stats.MTM.Select(x => (1.0 / TimeInterval) *
                UF.ExpectedValue(UF.MulArrayByConst(x.Prices, 1.0 / AllocationPerTrade))).ToList();

            Stats.AnnVol = Stats.MTM.Select(x => Math.Sqrt(1.0 / TimeInterval) *
                UF.StandardDeviation(UF.MulArrayByConst(x.Prices, 1.0 / AllocationPerTrade))).ToList();

            Stats.Return2Risk = Stats.AnnRet.Select((x, i) => x / Stats.AnnVol[i]).ToList();

            Stats.MaxDrawDown = Stats.DrawDown.Select(x => x.Prices.Max()).ToList();
            Stats.MaxDrawDownAbs = Stats.DrawDownAbs.Select(x => x.Prices.Max()).ToList();

            Stats.MOM = Stats.MTM.Select(x => x.Dates.Select((y, i) => new TimeStamp(y, x.Prices[i]))
                .GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price/AllocationPerTrade))).ToList()).ToList();

            Stats.HOH = Stats.Trades.Select(x => x.Count() > 0 ? x.GroupBy(y => (new DateTime(1, 1, 1,y.EntryDate.Hour,0,0)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Return))).ToList() : new List<TimeStamp>()).ToList();

            Stats.DODMTM = Stats.Trades.Select(x => x.Count() > 0 ? x.GroupBy(y => y.ExitDate.Date)
                .Select(z => new TimeStamp(z.Key, AllocationPerTrade * z.Sum(w => w.Return)))
                .ToList() : new List<TimeStamp>()).ToList();

            Stats.DODTV = Stats.Trades.Select(x =>  x.Count() > 0 ? x.GroupBy(y => y.ExitDate.Date)
                .Select(z => new TimeStamp(z.Key, 2.0 * z.Count() * AllocationPerTrade)).ToList() : new List<TimeStamp>()).ToList();

            Stats.MOMm2v = Stats.Trades.Select(x =>x.Count() > 0 ? x.GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Return) / (2.0 * z.Count()))).ToList(): new List<TimeStamp>()).ToList();

            Stats.YOY = Stats.MTM.Select(x => x.Dates.Select((y, i) => new TimeStamp(y, x.Prices[i]))
                .GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price / AllocationPerTrade))).ToList()).ToList();

            Stats.YOYm2v = Stats.Trades.Select(x => x.Count() > 0 ? x.GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Return) / (2.0 * z.Count()))).ToList():new List<TimeStamp>()).ToList();
            
            Stats.PerPosMonths = Stats.MOM.Select(x => x.Count(y => y.Price > 0) / (double)x.Count).ToList();

            if (UseAggStats)
            {
                AggStats.NumTrades = data.SecGrouping.Select(x => Stats.Trades.Where((y, i) => x.Contains(i)).
                    Select(z => z.Count).Sum()).ToList();

                AggStats.StrikeRate = data.SecGrouping.Select(x => Stats.Trades.Where((y, i) => x.Contains(i)).
                    Select(z => z.Count(w=>w.Return>0) + 0.0).Sum()).ToList();

                AggStats.StrikeRate = AggStats.StrikeRate.Select((x, i) => x / AggStats.NumTrades[i]).ToList();

                AggStats.TotalMTM = AggStats.MTM.Select(x => x.Prices.Sum()).ToList();

                AggStats.MTM2TV = AggStats.TotalMTM.Select((x, i) => x / (2.0 * AggStats.NumTrades[i] * AllocationPerTrade)).ToList();
                
                AggStats.DrawDown = AggStats.MTM.Select((x,i) => new TimeSeries(x.Dates,
                    Technicals.DrawDown(UF.MTM2NAV(x.Prices, data.SecGrouping[i].Length * AllocationPerTrade).Skip(1).ToArray()))).ToList();

                AggStats.DrawDownAbs = AggStats.MTM.Select((x, i) => new TimeSeries(x.Dates,
                    Technicals.DrawDownAbs(UF.CummSum(x.Prices).ToArray()))).ToList();

                AggStats.AnnRet = AggStats.MTM.Select((x,i) => (1.0 / TimeInterval) *
                    UF.ExpectedValue(UF.MulArrayByConst(x.Prices, 1.0 / (data.SecGrouping[i].Length*AllocationPerTrade)))).ToList();

                AggStats.AnnVol = AggStats.MTM.Select((x,i) => Math.Sqrt(1.0 / TimeInterval) *
                UF.StandardDeviation(UF.MulArrayByConst(x.Prices, 1.0 / (data.SecGrouping[i].Length*AllocationPerTrade)))).ToList();

                AggStats.Return2Risk = AggStats.AnnRet.Select((x, i) => x / AggStats.AnnVol[i]).ToList();

                AggStats.MaxDrawDown = AggStats.DrawDown.Select(x => x.Prices.Max()).ToList();
                AggStats.MaxDrawDownAbs = AggStats.DrawDownAbs.Select(x => x.Prices.Max()).ToList();

                List<TimeSeries> ts = AggStats.MTM.Select((x, i) => new TimeSeries(x.Dates, x.Prices.Select((y, j) => AggStats.GE[i].Prices[j] > 0 ?
                    y / AggStats.GE[i].Prices[j] : 0).ToArray())).ToList();

                AggStats.MOM = ts.Select(x => x.Dates.Select((y, i) => new TimeStamp(y, x.Prices[i]))
                    .GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                    .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price))).ToList()).ToList();

                AggStats.YOY = ts.Select(x => x.Dates.Select((y, i) => new TimeStamp(y, x.Prices[i]))
                    .GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                    .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price))).ToList()).ToList();

                AggStats.PerPosMonths = AggStats.MOM.Select(x => x.Count(y => y.Price > 0) / (double)x.Count).ToList();
            }

            rows = new List<object[]>();
            List<object> row;
            row = data.SecName.Select(x => (object)x).ToList();
            row.Insert(0, "Parameter");            
            rows.Add(row.ToArray());

            row = Stats.TotalMTM.Select(x => (object)x.ToString("N")).ToList();
            row.Insert(0, "MTM");
            rows.Add(row.ToArray());

            row = Stats.MTM2TV.Select(x => (object)x.ToString("0.0000%")).ToList();
            row.Insert(0, "MTM/TV");
            rows.Add(row.ToArray());
                        
            row = Stats.Return2Risk.Select(x => (object)x.ToString("0.00")).ToList();
            row.Insert(0, "Return/Risk");
            rows.Add(row.ToArray());

            row = Stats.NumTrades.Select(x => (object)x).ToList();
            row.Insert(0, "NumTrades");
            rows.Add(row.ToArray());

            row = Stats.StrikeRate.Select(x => (object)x.ToString("0.000%")).ToList();
            row.Insert(0, "Strike Rate");
            rows.Add(row.ToArray());

            row = Stats.MaxDrawDown.Select(x => (object)(x / 100).ToString("0.00%")).ToList();
            row.Insert(0, "Max DD");
            rows.Add(row.ToArray());

            row = Stats.MaxDrawDownAbs.Select(x => (object)(x).ToString("N")).ToList();
            row.Insert(0, "Max DD Abs");
            rows.Add(row.ToArray());

            row = Stats.PerPosMonths.Select(x => (object)x.ToString("0.00%")).ToList();
            row.Insert(0, "% Pos Months");
            rows.Add(row.ToArray());

            row = Stats.AnnRet.Select(x => (object)x.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Ret");
            rows.Add(row.ToArray());

            row = Stats.AnnVol.Select(x => (object)x.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Vol");
            rows.Add(row.ToArray());

            row = Stats.Win2LoseRatio.Select(x => (object)x.ToString("0.000")).ToList();
            row.Insert(0, "Win2Lose Ratio");
            rows.Add(row.ToArray());

            row = Stats.AvgTradeDuration.Select(x => (object)x.ToString("0.00")).ToList();
            row.Insert(0, "Avg Trade Bars");
            rows.Add(row.ToArray());

            row = Stats.PerDurationInTrade.Select(x => (object)x.ToString("0.00%")).ToList();
            row.Insert(0, "% Bars in Trade");
            rows.Add(row.ToArray());

            rows = UF.Col2Rows(rows);

            if (UseAggStats)
            {
                for (int i = 0; i < AggStats.NumTrades.Count; i++)
                {
                    string[] r1 = new string[] { "Agg"+i.ToString(), AggStats.TotalMTM[i].ToString("N"), 
                        AggStats.MTM2TV[i].ToString("0.000%"), AggStats.Return2Risk[i].ToString("0.00"),
                        AggStats.NumTrades[i].ToString(),AggStats.StrikeRate[i].ToString("0.000%"),
                        (AggStats.MaxDrawDown[i]/100).ToString("0.00%"),(AggStats.MaxDrawDownAbs[i]).ToString("N"),AggStats.PerPosMonths[i].ToString("0.00%"),
                        AggStats.AnnRet[i].ToString("0.00%"), AggStats.AnnVol[i].ToString("0.00%"),"NA", "NA", "NA"};
                    rows.Insert(1, r1);
                }
            }

            return rows;
        }

        public List<object[]> ComputeStatisticsYOY(StrategyData data, string selectedSec)
        {
            if (data == null
                || Stats == null)
            {
                return null;
            }

            int idx = data.SecName.FindIndex(x => x == selectedSec);

            Dictionary<DateTime, double> MTM2TV, StrikeRate, NumTrades, Win2Loss, AvgTradeDuration;
            List<TimeStamp> MTM2TVf = new List<TimeStamp>();
            List<TimeStamp> StrikeRatef = new List<TimeStamp>();
            List<TimeStamp> NumTradesf = new List<TimeStamp>();
            List<TimeStamp> Win2Lossf = new List<TimeStamp>();
            List<TimeStamp> AvgTradeDurationf = new List<TimeStamp>();
            List<TimeStamp> PerDurInTradef = new List<TimeStamp>();
           
            List<TimeStamp> TempRet = Stats.MTM[idx].Dates.Select((x,i) => new TimeStamp(x, Stats.MTM[idx].Prices[i] 
                / AllocationPerTrade)).ToList();

            List<TimeStamp> TempMTM = Stats.MTM[idx].Dates.Select((x, i) => new TimeStamp(x, Stats.MTM[idx].Prices[i])).ToList();

            List<TimeStamp> TempNet = Stats.MTM[idx].Dates.Select((x, i) => new TimeStamp(x, NetPositions[idx][i])).ToList();
            
            List<TimeStamp> MTM = TempRet.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price * AllocationPerTrade))).ToList();

            PerDurInTradef = TempNet.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, ((double)z.Count(x=>x.Price!=0))/z.Count())).ToList();

            List<TimeStamp> AnnualRet = TempRet.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(x => new TimeStamp(x.Key, (1.0 / TimeInterval) * 
                    UF.ExpectedValue(x.Select(w=>w.Price).ToArray()))).ToList();

            List<TimeStamp> AnnulStd = TempRet.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, Math.Sqrt(1.0 / TimeInterval) * UF.StandardDeviation(z.Select(w => w.Price).ToArray()
                    ))).ToList();

            List<TimeStamp> ReturnToRisk = AnnualRet.Select((x, i) => new TimeStamp(x.Date, x.Price / AnnulStd[i].Price)).ToList();

            List<TimeStamp> MaxDD = TempMTM.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, Technicals.DrawDown(UF.MTM2NAV(z.Select(x=>x.Price).ToArray(),
                    AllocationPerTrade)).Max())).ToList();

            List<TimeStamp> MaxDDAbs = TempMTM.GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, Technicals.DrawDownAbs(UF.CummSum(z.Select(x => x.Price).ToArray())).Max())).ToList();                        

            List<TimeStamp> PerPosMonths = Stats.MOM[idx].GroupBy(y => (new DateTime(y.Date.Year, 1, 1)))
                .Select(z => new TimeStamp(z.Key, ((double)z.Count(w => w.Price > 0))/z.Count())).ToList();
            
            if (Stats.Trades[idx].Count > 0)
            {
                MTM2TV = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                    .ToDictionary(z => z.Key, z => z.Sum(w => w.Return) / (2.0 * z.Count()));

                StrikeRate = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                    .ToDictionary(z => z.Key,z=>((double)z.Count(w => w.Return > 0))/z.Count());

                NumTrades = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                    .ToDictionary(z => z.Key,z => (double)z.Count());

                Win2Loss = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                    .ToDictionary(z => z.Key,z => z.Where(w=>w.Return>0).Count()>0 && z.Where(w=>w.Return<0).Count()>0 ?
                        Math.Abs(z.Where(w=>w.Return>0).Average(q=>q.Return)/
                        z.Where(w=>w.Return<0).Average(q=>q.Return)):Double.NaN);

                AvgTradeDuration = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, 1, 1)))
                    .ToDictionary(z => z.Key,z=>z.Select(x=>x.ExitIdx - x.EntryIdx).Average()); 
            }
            else
            {
                MTM2TV = AnnualRet.ToDictionary(x=>x.Date,y=>0.0);
                StrikeRate = AnnualRet.ToDictionary(x=>x.Date,y=>0.0);
                NumTrades = AnnualRet.ToDictionary(x=>x.Date,y=>0.0);
                Win2Loss = AnnualRet.ToDictionary(x=>x.Date,y=>0.0);
                AvgTradeDuration = AnnualRet.ToDictionary(x=>x.Date,y=>0.0); 
            }

            for (int i = 0; i < MTM.Count; i++)
            {
                if (!MTM2TV.ContainsKey(MTM[i].Date))
                {
                    MTM2TVf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    StrikeRatef.Add(new TimeStamp(MTM[i].Date, 0.0));
                    NumTradesf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    Win2Lossf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    AvgTradeDurationf.Add(new TimeStamp(MTM[i].Date, 0.0));                    
                }
                else
                {
                    MTM2TVf.Add(new TimeStamp(MTM[i].Date, MTM2TV[MTM[i].Date]));
                    StrikeRatef.Add(new TimeStamp(MTM[i].Date, StrikeRate[MTM[i].Date]));
                    NumTradesf.Add(new TimeStamp(MTM[i].Date, NumTrades[MTM[i].Date]));
                    Win2Lossf.Add(new TimeStamp(MTM[i].Date, Win2Loss[MTM[i].Date]));
                    AvgTradeDurationf.Add(new TimeStamp(MTM[i].Date, AvgTradeDuration[MTM[i].Date]));                    
                }
            }

            List<object[]> rows = new List<object[]>();
            List<object> row;
            
            row = MTM.Select(x => (object)x.Date.Year.ToString()).ToList();
            row.Insert(0, "Year");
            rows.Add(row.ToArray());

            row = MTM.Select(x => (object)x.Price.ToString("N")).ToList();
            row.Insert(0, "MTM");
            rows.Add(row.ToArray());

            row = MTM2TVf.Select(x => (object)x.Price.ToString("0.000%")).ToList();
            row.Insert(0, "MTM/TV");
            rows.Add(row.ToArray());

            row = ReturnToRisk.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Return/Risk");
            rows.Add(row.ToArray());

            row = NumTradesf.Select(x => (object)x.Price.ToString("0")).ToList();
            row.Insert(0, "NumTrades");
            rows.Add(row.ToArray());

            row = StrikeRatef.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Strike Rate");
            rows.Add(row.ToArray());

            row = MaxDD.Select(x => (object)(x.Price / 100).ToString("0.00%")).ToList();
            row.Insert(0, "Max DD");
            rows.Add(row.ToArray());

            row = MaxDDAbs.Select(x => (object)(x.Price).ToString("N")).ToList();
            row.Insert(0, "Max DD Abs");
            rows.Add(row.ToArray());

            row = PerPosMonths.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "% Pos Months");
            rows.Add(row.ToArray());

            row = AnnualRet.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Ret");
            rows.Add(row.ToArray());

            row = AnnulStd.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Vol");
            rows.Add(row.ToArray());           

            row = Win2Lossf.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Win2Lose Ratio");
            rows.Add(row.ToArray());

            row = AvgTradeDurationf.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Avg Trade Bars");
            rows.Add(row.ToArray());

            row = PerDurInTradef.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "% Bars in Trade");
            rows.Add(row.ToArray());

            rows = UF.Col2Rows(rows);
            return rows;
        }

        public List<object[]> ComputeStatisticsMOM(StrategyData data, string selectedSec)
        {
            if (data == null
                || Stats == null)
            {
                return null;
            }

            int idx = data.SecName.FindIndex(x => x == selectedSec);

            Dictionary<DateTime, double> MTM2TV, StrikeRate, NumTrades, Win2Loss, AvgTradeDuration;
            List<TimeStamp> MTM2TVf = new List<TimeStamp>();
            List<TimeStamp> StrikeRatef = new List<TimeStamp>();
            List<TimeStamp> NumTradesf = new List<TimeStamp>();
            List<TimeStamp> Win2Lossf = new List<TimeStamp>();
            List<TimeStamp> AvgTradeDurationf = new List<TimeStamp>();
            List<TimeStamp> PerDurInTradef = new List<TimeStamp>();

            List<TimeStamp> TempRet = Stats.MTM[idx].Dates.Select((x, i) => new TimeStamp(x, Stats.MTM[idx].Prices[i]
                / AllocationPerTrade)).ToList();

            List<TimeStamp> TempMTM = Stats.MTM[idx].Dates.Select((x, i) => new TimeStamp(x, Stats.MTM[idx].Prices[i])).ToList();

            List<TimeStamp> TempNet = Stats.MTM[idx].Dates.Select((x, i) => new TimeStamp(x, NetPositions[idx][i])).ToList();

            List<TimeStamp> MTM = TempRet.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, z.Sum(w => w.Price * AllocationPerTrade))).ToList();

            PerDurInTradef = TempNet.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, ((double)z.Count(x => x.Price != 0)) / z.Count())).ToList();

            List<TimeStamp> AnnualRet = TempRet.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(x => new TimeStamp(x.Key, (1.0 / TimeInterval) *
                    UF.ExpectedValue(x.Select(w => w.Price).ToArray()))).ToList();

            List<TimeStamp> AnnulStd = TempRet.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, Math.Sqrt(1.0 / TimeInterval) * UF.StandardDeviation(z.Select(w => w.Price).ToArray()
                    ))).ToList();

            List<TimeStamp> ReturnToRisk = AnnualRet.Select((x, i) => new TimeStamp(x.Date, x.Price / AnnulStd[i].Price)).ToList();

            List<TimeStamp> MaxDD = TempMTM.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, Technicals.DrawDown(UF.MTM2NAV(z.Select(x => x.Price).ToArray(),
                    AllocationPerTrade)).Max())).ToList();

            List<TimeStamp> MaxDDAbs = TempMTM.GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, Technicals.DrawDownAbs(UF.CummSum(z.Select(x => x.Price).ToArray())).Max())).ToList(); 

            List<TimeStamp> PerPosMonths = Stats.MOM[idx].GroupBy(y => (new DateTime(y.Date.Year, y.Date.Month, 1)))
                .Select(z => new TimeStamp(z.Key, ((double)z.Count(w => w.Price > 0)) / z.Count())).ToList();

            if (Stats.Trades[idx].Count > 0)
            {
                MTM2TV = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                    .ToDictionary(z => z.Key, z => z.Sum(w => w.Return) / (2.0 * z.Count()));

                StrikeRate = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                    .ToDictionary(z => z.Key, z => ((double)z.Count(w => w.Return > 0)) / z.Count());

                NumTrades = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                    .ToDictionary(z => z.Key, z => (double)z.Count());

                Win2Loss = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                    .ToDictionary(z => z.Key, z => z.Where(w => w.Return > 0).Count() > 0 && z.Where(w => w.Return < 0).Count() > 0 ?
                        Math.Abs(z.Where(w => w.Return > 0).Average(q => q.Return) /
                        z.Where(w => w.Return < 0).Average(q => q.Return)) : Double.NaN);

                AvgTradeDuration = Stats.Trades[idx].GroupBy(y => (new DateTime(y.ExitDate.Year, y.ExitDate.Month, 1)))
                    .ToDictionary(z => z.Key, z => z.Select(x => x.ExitIdx - x.EntryIdx).Average());
               
            }
            else
            {
                MTM2TV = AnnualRet.ToDictionary(x => x.Date, y => 0.0);
                StrikeRate = AnnualRet.ToDictionary(x => x.Date, y => 0.0);
                NumTrades = AnnualRet.ToDictionary(x => x.Date, y => 0.0);
                Win2Loss = AnnualRet.ToDictionary(x => x.Date, y => 0.0);
                AvgTradeDuration = AnnualRet.ToDictionary(x => x.Date, y => 0.0); 
            }

            for (int i = 0; i < MTM.Count; i++)
            {
                if (!MTM2TV.ContainsKey(MTM[i].Date))
                {
                    MTM2TVf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    StrikeRatef.Add(new TimeStamp(MTM[i].Date, 0.0));
                    NumTradesf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    Win2Lossf.Add(new TimeStamp(MTM[i].Date, 0.0));
                    AvgTradeDurationf.Add(new TimeStamp(MTM[i].Date, 0.0));                    
                }
                else
                {
                    MTM2TVf.Add(new TimeStamp(MTM[i].Date, MTM2TV[MTM[i].Date]));
                    StrikeRatef.Add(new TimeStamp(MTM[i].Date, StrikeRate[MTM[i].Date]));
                    NumTradesf.Add(new TimeStamp(MTM[i].Date, NumTrades[MTM[i].Date]));
                    Win2Lossf.Add(new TimeStamp(MTM[i].Date, Win2Loss[MTM[i].Date]));
                    AvgTradeDurationf.Add(new TimeStamp(MTM[i].Date, AvgTradeDuration[MTM[i].Date]));                    
                }
            }
            
            List<object[]> rows = new List<object[]>();
            List<object> row;

            row = MTM.Select(x => (object)x.Date.ToString("MMM-yyyy")).ToList();
            row.Insert(0, "Month");
            rows.Add(row.ToArray());

            row = MTM.Select(x => (object)x.Price.ToString("N")).ToList();
            row.Insert(0, "MTM");
            rows.Add(row.ToArray());

            row = MTM2TVf.Select(x => (object)x.Price.ToString("0.000%")).ToList();
            row.Insert(0, "MTM/TV");
            rows.Add(row.ToArray());

            row = ReturnToRisk.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Return/Risk");
            rows.Add(row.ToArray());

            row = NumTradesf.Select(x => (object)x.Price.ToString("0")).ToList();
            row.Insert(0, "NumTrades");
            rows.Add(row.ToArray());

            row = StrikeRatef.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Strike Rate");
            rows.Add(row.ToArray());

            row = MaxDD.Select(x => (object)(x.Price / 100).ToString("0.00%")).ToList();
            row.Insert(0, "Max DD");
            rows.Add(row.ToArray());

            row = MaxDDAbs.Select(x => (object)(x.Price).ToString("N")).ToList();
            row.Insert(0, "Max DD Abs");
            rows.Add(row.ToArray());

            row = PerPosMonths.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "% Pos Months");
            rows.Add(row.ToArray());

            row = AnnualRet.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Ret");
            rows.Add(row.ToArray());

            row = AnnulStd.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "Annualized Vol");
            rows.Add(row.ToArray());

            row = Win2Lossf.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Win2Lose Ratio");
            rows.Add(row.ToArray());

            row = AvgTradeDurationf.Select(x => (object)x.Price.ToString("0.00")).ToList();
            row.Insert(0, "Avg Trade Bars");
            rows.Add(row.ToArray());

            row = PerDurInTradef.Select(x => (object)x.Price.ToString("0.00%")).ToList();
            row.Insert(0, "% Bars in Trade");
            rows.Add(row.ToArray());

            rows = UF.Col2Rows(rows);
            return rows;
        }
        
        public void SaveStatistics(string fileNameStats, string fileNameMTM, StrategyData data, bool IsDoD = false)
        {
            if (fileNameStats == null
                || fileNameStats == ""
                || rows == null)
            {
                return;
            }

            FileWrite.Write(rows, fileNameStats, false, true);

            if (IsDoD)
            {
                FileWrite.Write(UF.Mingle(Stats.DODMTM.Select(x => x.Select(y=>y.Date).ToArray()).ToList(),
                    Stats.DODMTM.Select(x => x.Select(y => y.Price).ToArray()).ToList(),
                    Stats.DODTV.Select(x => x.Select(y => y.Price).ToArray()).ToList()),
                    fileNameMTM, false, false);
            }
            else
            {
                List<string[]> header = new List<string[]>();                
                for (int i = 0; i < Stats.MTM.Count; i++)
                {                    
                    header.Add(new string[] { data.SecName[i], "Net Position", "MTM", "GTV", "GE" });
                }

                string[] h = UF.Flatten(header);

                header.Clear();
                header.Add(h);
                
                FileWrite.Write(header, fileNameMTM, false, true);
                FileWrite.Write(UF.Mingle(Stats.MTM.Select(x => x.Dates).ToList(),
                    NetPositions, Stats.MTM.Select(x => x.Prices).ToList(), Stats.GTV.Select(x => x.Prices).ToList()
                    , Stats.GE.Select(x => x.Prices).ToList()), fileNameMTM, true, false);
            }
        }

        public abstract void RunStrategy(StrategyData data);
    }
}
