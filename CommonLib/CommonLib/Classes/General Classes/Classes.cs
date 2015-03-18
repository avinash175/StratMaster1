using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace CommonLib
{    
    public class OneStrategyOutPut
    {
        public DateTime[] dates;
        public double[] Prices;
        public double[] BidPx;
        public double[] AskPx;
        public bool UseBidAsk;
        public string assetName; // Name of the asset
        public double[] BuySellSig; // 1 for Buy, -1 for sell
        public double Target;
        public LongShortTrade[] Trades;
        public double Stoploss;        
        public bool TrailingStop; // To do 
        public bool HoldOverNightPos;
        public SignalGroup SigGroup;

        public OneStrategyOutPut()
        {
            Target = Double.PositiveInfinity;
            Stoploss = Double.NegativeInfinity;
            TrailingStop = false;
            UseBidAsk = false;
        }

        public OneStrategyOutPut(double _target, double _stoploss, bool _TSL, bool HoldONP, bool _useBidAsk)
        {
            Target = _target;
            Stoploss = _stoploss;
            TrailingStop = _TSL;
            HoldOverNightPos = HoldONP;
            UseBidAsk = _useBidAsk;
        }

        public void CreateBuySellSig()
        {
            int n = SigGroup.Sigs[0].Sig.Length;
            double[] Sig = new double[n];
            double LEA = SigGroup.LEA;
            double LExB = SigGroup.LExB;
            double SEB = SigGroup.SEB;
            double SExA = SigGroup.SExA;
            UF.Copy1DArrayL2R(SigGroup.Sigs[0].Sig, ref Sig);
            BuySellSig = new double[n];
            bool InTrade = false;
            double EntryPx = 0.0;
            // Create Buy Sell signal
            for (int i = 0; i < n; i++)
            {
                if (InTrade)
                {
                    BuySellSig[i] = BuySellSig[i - 1];
                    if (BuySellSig[i] == 1.0)
                    {
                        double ret = 0;
                        if (UseBidAsk)
                        {
                            ret = (BidPx[i] - EntryPx) / EntryPx;
                        }
                        else
                        {
                            ret = (Prices[i] - EntryPx) / EntryPx;
                        }
                        if (Sig[i] <= LExB || ret >= Target || ret <= -Stoploss)
                        {
                            BuySellSig[i] = 0.0;
                            InTrade = false;
                            EntryPx = 0.0;
                        }
                    }
                    else if (BuySellSig[i] == -1.0)
                    {
                        double ret = 0;
                        if (UseBidAsk)
                        {
                            ret = -(AskPx[i] - EntryPx) / EntryPx;
                        }
                        else
                        {
                            ret = -(Prices[i] - EntryPx) / EntryPx;
                        }
                        
                        if (Sig[i] >= SExA || ret >= Target || ret <= -Stoploss)
                        {
                            BuySellSig[i] = 0.0;
                            InTrade = false;
                            EntryPx = 0.0;
                        }                        
                    }
                }
                else
                {
                    if (Sig[i] > LEA)
                    {
                        BuySellSig[i] = 1.0;
                        InTrade = true;
                        if (UseBidAsk)
                        {
                            EntryPx = AskPx[i];
                        }
                        else
                        {
                            EntryPx = Prices[i];
                        }
                    }
                    else if (Sig[i] < SEB)
                    {
                        BuySellSig[i] = -1.0;
                        InTrade = true;
                        if (UseBidAsk)
                        {
                            EntryPx = BidPx[i];
                        }
                        else
                        {
                            EntryPx = Prices[i];
                        }
                    }
                }
                if (!HoldOverNightPos && InTrade)
                {
                    if (i < n - 1)
                    {
                        if (dates[i].Date != dates[i + 1].Date)
                        {
                            BuySellSig[i] = 0.0;
                            InTrade = false;
                            EntryPx = 0.0;
                        }
                    }
                }
            }// for

            ArrayList TradesArr = new ArrayList();
            InTrade = false;
            int entIdx = 0;
            LongShortTrade temp = new LongShortTrade();
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    if (BuySellSig[i] == 1)
                    {                        
                        temp.EntryDate = dates[i];
                        temp.LongShortFlag = 1;
                        if (UseBidAsk)
                            temp.EntryPx = AskPx[i];
                        else
                            temp.EntryPx = Prices[i];
                        entIdx = i;
                        InTrade = true;
                    }
                    else if (BuySellSig[i] == -1)
                    {                        
                        temp.EntryDate = dates[i];
                        temp.LongShortFlag = -1;
                        if (UseBidAsk)
                            temp.EntryPx = BidPx[i];
                        else
                            temp.EntryPx = Prices[i];
                        entIdx = i;
                        InTrade = true;
                    }

                }
                else
                {
                    if (InTrade && BuySellSig[i] ==1)// Long trade
                    {
                        if (BuySellSig[i - 1] == -1)
                        {
                            temp.ExitDate = dates[i];
                            if (UseBidAsk)
                                temp.ExitPx = AskPx[i];
                            else
                                temp.ExitPx = Prices[i];
                            temp.duration = i - entIdx;
                            TradesArr.Add(new LongShortTrade(temp));

                            temp = new LongShortTrade();
                            temp.EntryDate = dates[i];
                            temp.LongShortFlag = 1;
                            if (UseBidAsk)
                                temp.EntryPx = AskPx[i];
                            else
                                temp.EntryPx = Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }

                    }
                    else if (InTrade && BuySellSig[i] == -1)
                    {
                        if (BuySellSig[i - 1] == 1)
                        {
                            temp.ExitDate = dates[i];
                            if (UseBidAsk)
                                temp.ExitPx = BidPx[i];
                            else
                                temp.ExitPx = Prices[i];
                            temp.duration = i - entIdx;
                            TradesArr.Add(new LongShortTrade(temp));

                            temp = new LongShortTrade();
                            temp.EntryDate = dates[i];
                            temp.LongShortFlag = -1;
                            if (UseBidAsk)
                                temp.EntryPx = BidPx[i];
                            else
                                temp.EntryPx = Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }

                    }
                    else if (InTrade && BuySellSig[i] == 0)
                    {
                        if (BuySellSig[i - 1] == 1) //Long exit
                        {
                            temp.ExitDate = dates[i];
                            if (UseBidAsk)
                                temp.ExitPx = BidPx[i];
                            else
                                temp.ExitPx = Prices[i];
                            temp.duration = i - entIdx;
                            TradesArr.Add(new LongShortTrade(temp));
                            temp = new LongShortTrade();
                            InTrade = false;

                        }
                        else if (BuySellSig[i - 1] == -1)// Short exit
                        {
                            temp.ExitDate = dates[i];
                            if (UseBidAsk)
                                temp.ExitPx = AskPx[i];
                            else
                                temp.ExitPx = Prices[i];
                            temp.duration = i - entIdx;
                            TradesArr.Add(new LongShortTrade(temp));
                            temp = new LongShortTrade();
                            InTrade = false;
                        }

                    }
                    else if (!InTrade)
                    {
                        if (BuySellSig[i] == 1) // long Entry
                        {
                            temp = new LongShortTrade();
                            temp.EntryDate = dates[i];
                            temp.LongShortFlag = 1;
                            if (UseBidAsk)
                                temp.EntryPx = AskPx[i];
                            else
                                temp.EntryPx = Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }
                        else if (BuySellSig[i] == -1) // short Entry
                        {
                            temp = new LongShortTrade();
                            temp.EntryDate = dates[i];
                            temp.LongShortFlag = -1;
                            if (UseBidAsk)
                                temp.EntryPx = BidPx[i];
                            else
                                temp.EntryPx = Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }
                    }
                }
            }

            Trades = new LongShortTrade[TradesArr.Count];

            TradesArr.CopyTo(Trades);
        }
    }

    public class OneMTM_GV_GEOD
    {
        public DateTime[] dates;
        public double[] MTM;
        public double[] GExp;
        public double[] GTV;
        public double[] NumStks;        
        public double NetMTM;
        public double TotalTradedVolume;
        public double MTM2TV;
        public string assetName;
        public double TrCostImpact;
        public double InvestedAmt;
        public double lotSize;

        public void UpdateMTMFromStratOut(OneStrategyOutPut strat, double InvAmt, double TrCost, bool UseBidAsk)
        {
            InvestedAmt = InvAmt;
            TrCostImpact = TrCost;
            if (lotSize == 0)
            {
                lotSize = 1;
            }
            int n = strat.dates.Length;
            dates = new DateTime[n];
            MTM = new double[n];
            GExp = new double[n];
            GTV = new double[n];
            NumStks = new double[n];

            for (int i = 0; i < n; i++)
            {
                dates[i] = strat.dates[i];
            }
           
            if (UseBidAsk)
            {
                for (int i = 0; i < n; i++)
                {
                    if (i > 0) // after 1st day
                    {
                        if (strat.BuySellSig[i] == 1.0) // Today its buy
                        {
                            if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                NumStks[i] = NumStks[i - 1];
                                MTM[i] = NumStks[i - 1] * (strat.BidPx[i] - strat.BidPx[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.AskPx[i] / lotSize);
                                GTV[i] = (Math.Abs(NumStks[i]) + Math.Abs(NumStks[i - 1])) * strat.AskPx[i];
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i - 1] * (strat.AskPx[i] - strat.AskPx[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 0.0) // New trade (Buy)
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.AskPx[i] / lotSize);
                                GTV[i] = strat.AskPx[i] * Math.Abs(NumStks[i]);
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.BidPx[i] - strat.AskPx[i]); ;
                                GExp[i] = GTV[i];
                                
                            }
                        }
                        else if (strat.BuySellSig[i] == -1.0)// Today its short
                        {
                            if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                NumStks[i] = NumStks[i - 1];
                                MTM[i] = NumStks[i - 1] * (strat.AskPx[i] - strat.AskPx[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.BidPx[i] / lotSize);
                                GTV[i] = (Math.Abs(NumStks[i]) + Math.Abs(NumStks[i - 1])) * strat.BidPx[i];
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i - 1] * (strat.BidPx[i] - strat.BidPx[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 0.0) // New trade (Buy)
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.BidPx[i] / lotSize);
                                GTV[i] = strat.BidPx[i] * Math.Abs(NumStks[i]);
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.AskPx[i] - strat.BidPx[i]); ; ;
                                GExp[i] = GTV[i];
                                
                            }

                        }
                        else if (strat.BuySellSig[i] == 0.0)// today no trade or exit
                        {
                            if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                MTM[i] = NumStks[i - 1] * (strat.BidPx[i] - strat.BidPx[i - 1]);
                                GTV[i] = strat.BidPx[i] * Math.Abs(NumStks[i - 1]);
                            }
                            else if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                MTM[i] = NumStks[i - 1] * (strat.AskPx[i] - strat.AskPx[i - 1]);
                                GTV[i] = strat.AskPx[i] * Math.Abs(NumStks[i - 1]);
                            }
                        }
                    }
                    else // Today is 1st day
                    {
                        if (strat.BuySellSig[i] == 1.0) // Today its buy
                        {
                            NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.AskPx[i] / lotSize);
                            GTV[i] = strat.AskPx[i] * Math.Abs(NumStks[i]);
                            MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.BidPx[i] - strat.AskPx[i]); ;
                            GExp[i] = GTV[i];
                        }
                        else if (strat.BuySellSig[i] == -1.0)// Today its short
                        {
                            NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.BidPx[i] / lotSize);
                            GTV[i] = strat.BidPx[i] * Math.Abs(NumStks[i]);
                            MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.AskPx[i] - strat.BidPx[i]); ; ;
                            GExp[i] = GTV[i];
                        }                       
                    }
                    
                }
            }
            else // Use Px
            {
                for (int i = 0; i < n; i++)
                {
                    if (i > 0) // after 1st day
                    {
                        if (strat.BuySellSig[i] == 1.0) // Today its buy
                        {
                            if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                NumStks[i] = NumStks[i - 1];
                                MTM[i] = NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                                GTV[i] = (Math.Abs(NumStks[i]) + Math.Abs(NumStks[i - 1])) * strat.Prices[i];
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 0.0) // New trade (Buy)
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                                GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i]);
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.Prices[i] - strat.Prices[i]); ;
                                GExp[i] = GTV[i];                                
                            }
                        }
                        else if (strat.BuySellSig[i] == -1.0)// Today its short
                        {
                            if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                NumStks[i] = NumStks[i - 1];
                                MTM[i] = NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                                GTV[i] = (Math.Abs(NumStks[i]) + Math.Abs(NumStks[i - 1])) * strat.Prices[i];
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GExp[i] = Math.Abs(NumStks[i]) * strat.Prices[i];
                            }
                            else if (strat.BuySellSig[i - 1] == 0.0) // New trade (Buy)
                            {
                                NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                                GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i]);
                                MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.Prices[i] - strat.Prices[i]); ; ;
                                GExp[i] = GTV[i];                                
                            }

                        }
                        else if (strat.BuySellSig[i] == 0.0)// today no trade or exit
                        {
                            if (strat.BuySellSig[i - 1] == 1.0) // yest we were long
                            {
                                MTM[i] = NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i - 1]);
                            }
                            else if (strat.BuySellSig[i - 1] == -1.0) // yest we were short
                            {
                                MTM[i] = NumStks[i - 1] * (strat.Prices[i] - strat.Prices[i - 1]);
                                GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i - 1]);
                            }
                        }
                    }
                    else // Today is 1st day
                    {
                        if (strat.BuySellSig[i] == 1.0) // Today its buy
                        {
                            NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                            GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i]);
                            MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.Prices[i] - strat.Prices[i]); ;
                            GExp[i] = GTV[i];
                        }
                        else if (strat.BuySellSig[i] == -1.0)// Today its short
                        {
                            NumStks[i] = strat.BuySellSig[i] * lotSize * Math.Floor(InvestedAmt / strat.Prices[i] / lotSize);
                            GTV[i] = strat.Prices[i] * Math.Abs(NumStks[i]);
                            MTM[i] = -TrCostImpact * GTV[i] + NumStks[i] * (strat.Prices[i] - strat.Prices[i]); ; ;
                            GExp[i] = GTV[i];
                        }
                    }
                } // for
            }// else

            TotalTradedVolume = UF.SumArray(GTV);
            NetMTM = UF.SumArray(MTM);
            MTM2TV = NetMTM/TotalTradedVolume;
        }
                        
    }

    public class Strategy
    {
        public int numSeries;
        public OneStrategyOutPut[] StratArr;
        public OneMTM_GV_GEOD[] MTMArr;
        public OneMTM_GV_GEOD MTMFull;

        public Strategy()
        {
        }

        public Strategy(int n)
        {
            numSeries = n;
            StratArr = new OneStrategyOutPut[n];
            MTMArr = new OneMTM_GV_GEOD[n];
            for (int i = 0; i < n; i++)
            {
                StratArr[i] = new OneStrategyOutPut();
                MTMArr[i] = new OneMTM_GV_GEOD();
            }
            MTMFull = new OneMTM_GV_GEOD();
        }

        public void GenerateFullMTM()
        {            
            int n = MTMArr[0].dates.Length;
            MTMFull.dates = new DateTime[n];
            MTMFull.GExp = new double[n];
            MTMFull.GTV = new double[n];
            MTMFull.InvestedAmt = MTMArr[0].InvestedAmt*n;
            MTMFull.MTM = new double[n];
            MTMFull.lotSize = MTMArr[0].lotSize;
            MTMFull.TrCostImpact = MTMArr[0].TrCostImpact;
            
            for (int i = 0; i < numSeries; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    MTMFull.dates[j] = MTMArr[i].dates[j];
                    MTMFull.GExp[j]+= MTMArr[i].GExp[j];
                    MTMFull.MTM[j] += MTMArr[i].MTM[j];
                    MTMFull.GTV[j] += MTMArr[i].GTV[j];
                    MTMFull.TotalTradedVolume += MTMArr[i].TotalTradedVolume;
                    MTMFull.NetMTM += MTMArr[i].NetMTM;
                }
            }
            MTMFull.MTM2TV = MTMFull.NetMTM / MTMFull.TotalTradedVolume;                       
        }

        public double[,] GenerateReults()
        {
            double[,] Results;            
            ArrayList AllYears =new ArrayList();
            ArrayList AllMonths = new ArrayList();
            for (int i = 0; i < MTMFull.dates.Length; i++)
            {
                if (!AllYears.Contains(MTMFull.dates[i].Year))
                {
                    AllYears.Add(MTMFull.dates[i].Year);
                }
                string monthStr = MTMFull.dates[i].Month.ToString() + MTMFull.dates[i].Year.ToString();
                if (!AllMonths.Contains(monthStr))
                {
                    AllMonths.Add(monthStr);                    
                }

            }

            Results = new double[3,1];
            Results[0, 0] = MTMFull.NetMTM;
            Results[1, 0] = MTMFull.TotalTradedVolume;
            Results[2, 0] = MTMFull.MTM2TV;  
            return Results;
        }
    }

    public class OneSignal
    {
        public double[] Sig;
    }

    public class SignalGroup
    {
        public OneSignal[] Sigs;
        public double LEA;
        public double LExB;
        public double SEB;
        public double SExA;

        public SignalGroup()
        {
        }

        public SignalGroup(int n, double _LEA, double _LExB, double _SEB, double _SExA)
        {

            Sigs = new OneSignal[n];
            for (int i = 0; i < n; i++)
            {
                Sigs[i] = new OneSignal();
            }
            LEA = _LEA;
            LExB = _LExB;
            SEB = _SEB;
            SExA = _SExA;
        }
    }

    public class Level1Entry
    {
        public DateTime Date;
        public DateTime[] TimeArr;
        public int[] TimeIdx;
        public double[] LTP;
        public double[] LTV;
        public double[] bidPx;
        public double[] bidVol;
        public double[] askPx;
        public double[] askVol;
        public double[] LTPCash;
        public double[] LTVCash;
        public int[] TimeIdxCash;

        public double[] SpreadAskCF;
        public double[] SpreadBidCF;
        public int[] TimeIdxSpread;        

        public Level1Entry()
        {
        }

        public Level1Entry(int n)
        {
            TimeArr = new DateTime[n];
            TimeIdx = new int[n];
            LTP = new double[n];
            LTV = new double[n];
            bidPx = new double[n];
            askPx = new double[n];
            bidVol = new double[n];
            askVol = new double[n];
            LTVCash = new double[n];
        }

        public Level1Entry(Level1Entry CopyObj)
        {
            Date = CopyObj.Date;
            UF.Copy1DArrayL2RDates(CopyObj.TimeArr, ref TimeArr);
            UF.Copy1DArrayL2R(CopyObj.TimeIdx, ref TimeIdx);
            UF.Copy1DArrayL2R(CopyObj.LTP, ref LTP);
            UF.Copy1DArrayL2R(CopyObj.LTV, ref LTV);
            UF.Copy1DArrayL2R(CopyObj.LTVCash, ref LTVCash);
            UF.Copy1DArrayL2R(CopyObj.bidPx, ref bidPx);
            UF.Copy1DArrayL2R(CopyObj.askPx, ref askPx);
            UF.Copy1DArrayL2R(CopyObj.bidVol, ref bidVol);
            UF.Copy1DArrayL2R(CopyObj.askVol, ref askVol);
        }

        public Level1Entry FillMissingBidAsk()
        {            
            int len = TimeIdx.Length;
            if (len == 0)
                return this;
            int start = TimeIdx[0];
            int end = TimeIdx[len-1];
            Level1Entry temp = new Level1Entry(end - start + 1);
            UF.Copy1DArrayL2R(TimeIdxCash, ref temp.TimeIdxCash);
            UF.Copy1DArrayL2R(LTPCash, ref temp.LTPCash);
            temp.Date = Date;
            int cnt=0;
            for (int i = start; i <= end ; i++)
            {
                if (TimeIdx[cnt] == i)// only in the begining
                {
                    temp.TimeArr[i - start] = TimeArr[cnt];
                    temp.TimeIdx[i - start] = TimeIdx[cnt];
                    temp.LTP[i - start] = LTP[cnt];
                    temp.LTV[i - start] = LTV[cnt];
                    temp.bidPx[i - start] = bidPx[cnt];
                    temp.bidVol[i - start] = bidVol[cnt];
                    temp.askPx[i - start] = askPx[cnt];
                    temp.askVol[i - start] = askVol[cnt];                    
                }
                else if (TimeIdx[cnt] < i)
                {
                    if (cnt < len - 1)
                    {
                        if (TimeIdx[cnt + 1] == i)// when the time Idx is present
                        {
                            cnt++;
                            temp.TimeArr[i - start] = TimeArr[cnt];
                            temp.TimeIdx[i - start] = TimeIdx[cnt];
                            temp.LTP[i - start] = LTP[cnt];
                            temp.LTV[i - start] = LTV[cnt];
                            temp.bidPx[i - start] = bidPx[cnt];
                            temp.bidVol[i - start] = bidVol[cnt];
                            temp.askPx[i - start] = askPx[cnt];
                            temp.askVol[i - start] = askVol[cnt];
                        }
                        else // when time Idx not present
                        {                            
                            temp.TimeIdx[i - start] = i;
                            temp.TimeArr[i - start] = TimeArr[0].AddSeconds(i - start);
                            temp.LTP[i - start] = 0;
                            temp.LTV[i - start] = 0;
                            temp.bidPx[i - start] = bidPx[cnt];
                            temp.bidVol[i - start] = bidVol[cnt];
                            temp.askPx[i - start] = askPx[cnt];
                            temp.askVol[i - start] = askVol[cnt];
                        }
                    }
                    else
                    {
                        temp.TimeArr[i - start] = TimeArr[cnt];
                        temp.TimeIdx[i - start] = TimeIdx[cnt]; 
                        temp.LTP[i - start] = LTP[cnt];
                        temp.LTV[i - start] = LTV[cnt];
                        temp.bidPx[i - start] = bidPx[cnt];
                        temp.bidVol[i - start] = bidVol[cnt];
                        temp.askPx[i - start] = askPx[cnt];
                        temp.askVol[i - start] = askVol[cnt];
                    }
                }
            }
            return temp;
        }

        public Level1Entry FillMissingLTPCash() // Use after FillMissingBidAsk
        {
            int len = TimeIdxCash.Length;
            if (len == 0)
                return this;
            
            int start = TimeIdxCash[0];
            int end = TimeIdxCash[len - 1];
            Level1Entry temp = new Level1Entry(this);
            temp.Date = Date;
            temp.LTPCash = new double[end - start + 1];
            temp.TimeIdxCash = new int[end - start + 1];
            int cnt = 0;
            for (int i = start; i <= end; i++)
            {
                if (TimeIdxCash[cnt] == i)// only in the begining
                {
                    temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];                     
                    temp.LTPCash[i - start] = LTPCash[cnt];
                }
                else if (TimeIdxCash[cnt] < i)
                {
                    if (cnt < len - 1)
                    {
                        if (TimeIdxCash[cnt + 1] == i)// when the time Idx is present
                        {
                            cnt++;
                            temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                            if (LTPCash[cnt] > 0)
                                temp.LTPCash[i - start] = LTPCash[cnt];
                            else
                                temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];
                        }
                        else // when time Idx not present
                        {
                            temp.TimeIdxCash[i - start] = i;
                            temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];                            
                        }
                    }
                    else
                    {
                        temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                        if (LTPCash[cnt] > 0)
                            temp.LTPCash[i - start] = LTPCash[cnt];
                        else
                            temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];                     
                    }
                }
            }
            return temp;
        }

        public Level1Entry FillMissingLTPVolCash() // Use after FillMissingBidAsk
        {
            int len = TimeIdxCash.Length;
            if (len == 0)
                return this;

            int start = TimeIdxCash[0];
            int end = TimeIdxCash[len - 1];
            Level1Entry temp = new Level1Entry(this);
            temp.Date = Date;
            temp.LTPCash = new double[end - start + 1];
            temp.TimeIdxCash = new int[end - start + 1];
            temp.LTVCash = new double[end - start + 1];

            int cnt = 0;
            for (int i = start; i <= end; i++)
            {
                if (TimeIdxCash[cnt] == i)// only in the begining
                {
                    temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                    temp.LTPCash[i - start] = LTPCash[cnt];
                    temp.LTVCash[i - start] = LTVCash[cnt];
                }
                else if (TimeIdxCash[cnt] < i)
                {
                    if (cnt < len - 1)
                    {
                        if (TimeIdxCash[cnt + 1] == i)// when the time Idx is present
                        {
                            cnt++;
                            temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                            double px1 = temp.LTPCash[i - start - 1];
                            if (LTPCash[cnt] > 0 
                                && (Math.Abs(px1 - LTPCash[cnt])/px1 < 0.01||px1==0))
                            {
                                temp.LTPCash[i - start] = LTPCash[cnt];
                                temp.LTVCash[i - start] = LTVCash[cnt];
                            }
                            else
                                temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];
                        }
                        else // when time Idx not present
                        {
                            temp.TimeIdxCash[i - start] = i;
                            temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];
                        }
                    }
                    else
                    {
                        temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                        double px1 = temp.LTPCash[i - start - 1];
                        if (LTPCash[cnt] > 0
                            && (Math.Abs(px1 - LTPCash[cnt]) / px1 < 0.01||px1==0))
                        {
                            temp.LTPCash[i - start] = LTPCash[cnt];
                            temp.LTVCash[i - start] = LTVCash[cnt];
                        }
                        else
                            temp.LTPCash[i - start] = temp.LTPCash[i - start - 1];
                    }
                }
            }
            return temp;
        }

        public void GenerateSpreads() // Use after FillMissingLTPCash
        {
            TimeIdxSpread = NF.IntersectSortedInt(TimeIdx, TimeIdxCash);
            TimeIdxSpread = UF.GetRange(TimeIdxSpread, 30, TimeIdxSpread.Length - 1);
            SpreadAskCF = new double[TimeIdxSpread.Length];
            SpreadBidCF = new double[TimeIdxSpread.Length];
            for (int i = 0; i < TimeIdxSpread.Length; i++)
            {                
                int idxF = NF.FindCommonIndex(TimeIdx, TimeIdxSpread[i]);
                int idxC = NF.FindCommonIndex(TimeIdxCash, TimeIdxSpread[i]);

                SpreadAskCF[i] = askPx[idxF] - LTPCash[idxC];
                SpreadBidCF[i] = bidPx[idxF] - LTPCash[idxC];
            }
        }

        public void PrintToFile(string FileName)
        {
            FileWrite fw = new FileWrite(FileName);            
           
            //fw.DataSaveWriteOneVar(TimeArr);
            fw.DataSaveWriteOneVar(TimeIdx);
            //fw.DataSaveWriteOneVar(LTP);
            //fw.DataSaveWriteOneVar(LTV);
            fw.DataSaveWriteOneVar(bidPx);
            //fw.DataSaveWriteOneVar(bidVol);
            fw.DataSaveWriteOneVar(askPx);
            //fw.DataSaveWriteOneVar(askVol);
            fw.DataSaveWriteOneVar(TimeIdxCash);
            fw.DataSaveWriteOneVar(LTPCash);
            
        }        
    }

    public class Level1EntryCF
    {
        public DateTime Date;

        public DateTime[] TimeArrFut;
        public int[] TimeIdxFut;

        public DateTime[] TimeArrCash;
        public int[] TimeIdxCash;
        
        public double[] LTPFut;        
        public double[] bidFut;        
        public double[] askFut; 
       
        public double[] LTPCash;
        public double[] bidCash;
        public double[] askCash;

        public SpreadData spread { get; set; }

        public Level1EntryCF()
        {
        }

        public Level1EntryCF(int nFut, int nCash)
        {

            TimeArrFut = new DateTime[nFut];
            TimeIdxFut = new int[nFut];
            LTPFut = new double[nFut];
            bidFut = new double[nFut];
            askFut = new double[nFut];

            TimeArrCash = new DateTime[nCash];
            TimeIdxCash = new int[nCash];
            LTPCash = new double[nCash];
            bidCash = new double[nCash];
            askCash = new double[nCash];

        }

        public Level1EntryCF(Level1EntryCF CopyObj)
        {
            Date = CopyObj.Date;
            UF.Copy1DArrayL2RDates(CopyObj.TimeArrFut, ref TimeArrFut);
            UF.Copy1DArrayL2R(CopyObj.TimeIdxFut, ref TimeIdxFut);
            UF.Copy1DArrayL2R(CopyObj.LTPFut, ref LTPFut);            
            UF.Copy1DArrayL2R(CopyObj.bidFut, ref bidFut);
            UF.Copy1DArrayL2R(CopyObj.askFut, ref askFut);

            UF.Copy1DArrayL2RDates(CopyObj.TimeArrCash, ref TimeArrCash);
            UF.Copy1DArrayL2R(CopyObj.TimeIdxCash, ref TimeIdxCash);
            UF.Copy1DArrayL2R(CopyObj.LTPCash, ref LTPCash);
            UF.Copy1DArrayL2R(CopyObj.bidCash, ref bidCash);
            UF.Copy1DArrayL2R(CopyObj.askCash, ref askCash);            
        }

        public Level1EntryCF FillMissingBidAskFut()
        {
            int len = TimeIdxFut.Length;
            if (len == 0)
                return this;
            int start = TimeIdxFut[0];
            int end = TimeIdxFut[len - 1];
            Level1EntryCF temp = new Level1EntryCF(end - start + 1, TimeArrCash.Length);
            UF.Copy1DArrayL2RDates(TimeArrCash, ref temp.TimeArrCash);
            UF.Copy1DArrayL2R(TimeIdxCash, ref temp.TimeIdxCash);
            UF.Copy1DArrayL2R(LTPCash, ref temp.LTPCash);
            UF.Copy1DArrayL2R(bidCash, ref temp.bidCash);
            UF.Copy1DArrayL2R(askCash, ref temp.askCash);

            temp.Date = Date;
            int cnt = 0;
            for (int i = start; i <= end; i++)
            {
                if (TimeIdxFut[cnt] == i)// only in the begining
                {
                    temp.TimeArrFut[i - start] = TimeArrFut[cnt];
                    temp.TimeIdxFut[i - start] = TimeIdxFut[cnt];
                    temp.LTPFut[i - start] = LTPFut[cnt];                    
                    temp.bidFut[i - start] = bidFut[cnt];                    
                    temp.askFut[i - start] = askFut[cnt];
                    
                }
                else if (TimeIdxFut[cnt] < i)
                {
                    if (cnt < len - 1)
                    {
                        if (TimeIdxFut[cnt + 1] == i)// when the time Idx is present
                        {
                            cnt++;
                            temp.TimeArrFut[i - start] = TimeArrFut[cnt];
                            temp.TimeIdxFut[i - start] = TimeIdxFut[cnt];
                            temp.LTPFut[i - start] = LTPFut[cnt];                            
                            temp.bidFut[i - start] = bidFut[cnt];                            
                            temp.askFut[i - start] = askFut[cnt];                            
                        }
                        else // when time Idx not present
                        {
                            temp.TimeIdxFut[i - start] = i;
                            temp.TimeArrFut[i - start] = TimeArrFut[0].AddSeconds(i - start);
                            temp.LTPFut[i - start] = 0;                            
                            temp.bidFut[i - start] = bidFut[cnt];                            
                            temp.askFut[i - start] = askFut[cnt];                           
                        }
                    }
                    else
                    {
                        temp.TimeArrFut[i - start] = TimeArrFut[cnt];
                        temp.TimeIdxFut[i - start] = TimeIdxFut[cnt];
                        temp.LTPFut[i - start] = LTPFut[cnt];                        
                        temp.bidFut[i - start] = bidFut[cnt];                        
                        temp.askFut[i - start] = askFut[cnt];                        
                    }
                }
            }
            return temp;
        }

        public Level1EntryCF FillMissingBidAskCash() // Use after FillMissingBidAsk
        {
            int len = TimeIdxCash.Length;
            if (len == 0)
                return this;
            int start = TimeIdxCash[0];
            int end = TimeIdxCash[len - 1];
            Level1EntryCF temp = new Level1EntryCF(TimeArrFut.Length, end - start + 1);

            UF.Copy1DArrayL2RDates(TimeArrFut, ref temp.TimeArrFut);
            UF.Copy1DArrayL2R(TimeIdxFut, ref temp.TimeIdxFut);
            UF.Copy1DArrayL2R(LTPFut, ref temp.LTPFut);
            UF.Copy1DArrayL2R(bidFut, ref temp.bidFut);
            UF.Copy1DArrayL2R(askFut, ref temp.askFut);

            temp.Date = Date;
            int cnt = 0;
            for (int i = start; i <= end; i++)
            {
                if (TimeIdxCash[cnt] == i)// only in the begining
                {
                    temp.TimeArrCash[i - start] = TimeArrCash[cnt];
                    temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                    temp.LTPCash[i - start] = LTPCash[cnt];
                    temp.bidCash[i - start] = bidCash[cnt];
                    temp.askCash[i - start] = askCash[cnt];

                }
                else if (TimeIdxCash[cnt] < i)
                {
                    if (cnt < len - 1)
                    {
                        if (TimeIdxCash[cnt + 1] == i)// when the time Idx is present
                        {
                            cnt++;
                            temp.TimeArrCash[i - start] = TimeArrCash[cnt];
                            temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                            temp.LTPCash[i - start] = LTPCash[cnt];
                            temp.bidCash[i - start] = bidCash[cnt];
                            temp.askCash[i - start] = askCash[cnt];
                        }
                        else // when time Idx not present
                        {
                            temp.TimeIdxCash[i - start] = i;
                            temp.TimeArrCash[i - start] = TimeArrCash[0].AddSeconds(i - start);
                            temp.LTPCash[i - start] = 0;
                            temp.bidCash[i - start] = bidCash[cnt];
                            temp.askCash[i - start] = askCash[cnt];
                        }
                    }
                    else
                    {
                        temp.TimeArrCash[i - start] = TimeArrCash[cnt];
                        temp.TimeIdxCash[i - start] = TimeIdxCash[cnt];
                        temp.LTPCash[i - start] = LTPCash[cnt];
                        temp.bidCash[i - start] = bidCash[cnt];
                        temp.askCash[i - start] = askCash[cnt];
                    }
                }
            }
            return temp;
        }

        public void GenerateSpreads() // Use after FillMissingLTPCash
        {
            
            int[] CommonIdx = NF.IntersectSortedInt(TimeIdxFut, TimeIdxCash);
            CommonIdx = UF.GetRange(CommonIdx, 60, CommonIdx.Length - 1);
            spread = new SpreadData(CommonIdx.Length);
            for (int i = 0; i < CommonIdx.Length; i++)
            {
                int idxF = NF.FindCommonIndex(TimeIdxFut, CommonIdx[i]);
                int idxC = NF.FindCommonIndex(TimeIdxCash, CommonIdx[i]);

                spread.ShortRoll[i] = 10000 * (bidFut[idxF] - askCash[idxC]) / askCash[idxC];
                spread.LongRoll[i] = 10000 * (bidCash[idxC] - askFut[idxF]) / askCash[idxC];

                spread.dateTime[i] = TimeArrCash[idxC];
            }
        }

        public void PrintToFile(string FileName)
        {
            FileWrite fw = new FileWrite(FileName);

            //fw.DataSaveWriteOneVar(TimeArr);
            fw.DataSaveWriteOneVar(TimeIdxFut);
            //fw.DataSaveWriteOneVar(LTP);
            //fw.DataSaveWriteOneVar(LTV);
            fw.DataSaveWriteOneVar(bidFut);
            //fw.DataSaveWriteOneVar(bidVol);
            fw.DataSaveWriteOneVar(askFut);
            //fw.DataSaveWriteOneVar(askVol);
            fw.DataSaveWriteOneVar(TimeIdxCash);
            fw.DataSaveWriteOneVar(LTPCash);

        }
    }
    
    public class NetPosition
    {
        public Trade[] AllTrades;
        public int CurrentPosition; // +1 for Long, 0 for no position and -1 for short
        public DateTime StartDate;
        public int numberOfTrades;
        public double currentStopLoss;
    }

    public class LongShortTrade
    {
        public string Scrip { get; set; }
        public LongShortType LongShort { get; set; }
        public DateTime EntryDate { get; set; }
        public double EntryPx { get; set; }
        public DateTime ExitDate { get; set; }        
        public double ExitPx { get; set; }
        public double NIFTYEnterPr, SectorEnterPr, NIFTYExitPr, SectorExitPr;
        public string SectorName;
        public int TimeStop;
        public double Weight;
        public int LongShortFlag; // +1: Long, -1: Short
        public double Return { get; set; }
        public int duration;
        

        public LongShortTrade()
        {

        }

        public LongShortTrade(LongShortTrade Input)
        {
            Scrip = Input.Scrip;
            EntryPx = Input.EntryPx;
            ExitPx = Input.ExitPx;
            EntryDate = Input.EntryDate;
            ExitDate = Input.ExitDate;            
            LongShortFlag = Input.LongShortFlag;
            duration = Input.duration;
            NIFTYEnterPr = Input.NIFTYEnterPr;
            NIFTYExitPr = Input.NIFTYExitPr;
            SectorEnterPr = Input.SectorEnterPr;
            SectorExitPr = Input.SectorExitPr;
            SectorName = Input.SectorName;
            TimeStop = Input.TimeStop;
            Weight = Input.Weight;
            LongShort = LongShortFlag == 1 ? LongShortType.LONG : LongShortType.SHORT;

            Return = (ExitPx - EntryPx) / EntryPx;
            if (LongShortFlag == -1)
            {
                Return = -Return;
            }
        }
    }

    public class LongShortTradeList
    {
        public LongShortTrade[] ListOfTrades;
        public string InstrumentName;
    }

    public class LongShortPortfolioTradeList
    {
        public LongShortTradeList[] PortfolioTrades;

        public LongShortPortfolioTradeList()
        {
        }

        public LongShortPortfolioTradeList(int n)
        {
            PortfolioTrades = new LongShortTradeList[n];
        }
    }

    public class SpreadData
    {
        public double[] ShortRoll {get; set;}
        public double[] LongRoll { get; set; }
        public DateTime[] dateTime { get; set; }
        public SpreadType TypeOfSpread { get; set; }
        public string Scrip { get; set; }
        public DateTime Expiry { get; set; }

        public SpreadData()
        {

        }

        public SpreadData(int n)
        {
            ShortRoll = new double[n];
            LongRoll = new double[n];
            dateTime = new DateTime[n];
        }

        public SpreadData(List<DateTime> dates, List<double> shortRoll,
            List<double> longRoll, string scrip, DateTime expiry)
        {
            dateTime = dates.ToArray();
            ShortRoll = shortRoll.ToArray();
            LongRoll = longRoll.ToArray();
            Scrip = scrip;
            Expiry = expiry;
        }
    }

    

    public class ArrayCollection<T>
    {
        public List<T[]> Coll { get; set; }       

        public ArrayCollection()
        {
            Coll = new List<T[]>();            
        }
    }

    public class GridStr
    {
        public string Str { get; set; }

        public GridStr(string _str)
        {
            Str = _str;
        }
    }

    public class GridStrColl
    {
        public GridStr[] Grids { get; set; }

        public GridStrColl(GridStr[] _grids)
        {
            Grids = _grids;
        }
    }

    public class dump
    {
        public static double[] GetMSTRATSColours(OHLCDataSet ohlc)
        {
            int i, n;
            int EMAday1, EMAday2, DIday1, DIday2;
            double MulFactor, StopFactorLong, StopFactorShort;
            bool conGreen, conRed, conBlue, conPurple, conBlack;
            bool Green_S, Green_L, Red_S, Red_L, Blue_S, Blue_L;

            EMAday1 = 9;
            DIday1 = 14;

            EMAday2 = 38;
            DIday2 = 31;

            MulFactor = 0.5;
            StopFactorLong = 1.2;
            StopFactorShort = 0.6;

            n = ohlc.numOfElements;
            double[] output = new double[n];

            double[] ShortEMA = Technicals.ExpMovAvg(ohlc.close, EMAday1);
            double[] LongEMA = Technicals.ExpMovAvg(ohlc.close, EMAday2);

            double[] ShortDI_P = Technicals.DIPlus(ohlc, DIday1);
            double[] LongDI_P = Technicals.DIPlus(ohlc, DIday2);

            double[] ShortDI_M = Technicals.DIMinus(ohlc, DIday1);
            double[] LongDI_M = Technicals.DIMinus(ohlc, DIday2);


            for (i = 0; i < 50; i++)
                output[i] = 5.0;

            for (i = 50; i < n; i++)
            {
                conGreen = false; conRed = false; conBlue = false; conPurple = false; conBlack = false;

                Green_S = (ohlc.close[i] > ShortEMA[i]) && (ShortDI_P[i] > ShortDI_M[i]);
                Green_L = (ohlc.close[i] > LongEMA[i]) && (LongDI_P[i] > LongDI_M[i]);

                Red_S = (ohlc.close[i] < ShortEMA[i]) && (ShortDI_P[i] < ShortDI_M[i]);
                Red_L = (ohlc.close[i] < LongEMA[i]) && (LongDI_P[i] < LongDI_M[i]);

                Blue_S = !(Green_S || Red_S);
                Blue_L = !(Green_L || Red_L);

                if (Green_S && Green_L)
                {
                    output[i] = 1.0;
                    conGreen = true;
                }
                if (Green_S && Red_L)
                {
                    output[i] = 4.0;
                    conPurple = true;
                }
                if (Red_S && Red_L)
                {
                    output[i] = 2.0;
                    conRed = true;
                }
                if (Red_S && Green_L)
                {
                    output[i] = 3.0;
                    conBlue = true;
                }
                if (Green_S && Blue_L)
                {
                    output[i] = 3.0;
                    conBlue = true;
                }
                if (Blue_S && Green_L)
                {
                    output[i] = 3.0;
                    conBlue = true;
                }
                if (Blue_S && Red_L)
                {
                    output[i] = 4.0;
                    conPurple = true;
                }
                if (Red_S && Blue_L)
                {
                    output[i] = 4.0;
                    conPurple = true;
                }
                if (Blue_S && Blue_L)
                {
                    output[i] = 5.0;
                    conBlack = true;
                }
            }

            return output;
        }
    }

}