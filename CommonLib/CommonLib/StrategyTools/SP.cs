using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommonLib
{
    // set of static functions
    public class SP // Strategy Parameters
    {
        public static double[] GenerateMTM(double[] series, int[] longShort,
            double alloc, int longInt = 1, int shortInt = -1, double cost = 10/(10000.0))
        {
            int[] qty = new int[series.Length];
            double[] mtm = new double[series.Length];

            if(series[0] > 0)
                qty[0] = (longShort[0] == longInt || longShort[0] == shortInt) ? 
                    (int)(alloc / series[0]) : 0;

            for (int i = 1; i < series.Length; i++)
            {
                int lsOld = longShort[i - 1] == longInt ? 1 : longShort[i - 1] == shortInt ? -1 : 0;
                int lsNew = longShort[i] == longInt ? 1 : longShort[i] == shortInt ? -1 : 0;

                if (longShort[i] != longShort[i - 1])
                {
                    qty[i] = series[i] > 0 ? Math.Abs(lsNew) * (int)(alloc / series[i]) : 0;
                }
                else
                {
                    qty[i] = qty[i - 1];
                }

                mtm[i] = (series[i] - series[i - 1]) * lsOld * qty[i - 1] -
                    series[i] * Math.Abs(lsNew * qty[i] - lsOld * qty[i - 1]) * cost;
            }

            return mtm;
        }

        public static double[] GenerateMTM(double[] Bid, double[] Ask, double[] LTP, int[] longShort,
            double alloc, int longInt = 1, int shortInt = -1, double cost = 10/(10000.0))
        {
            int[] qty = new int[Bid.Length];
            double[] mtm = new double[Bid.Length];

            if (LTP[0] > 0)
                qty[0] = (longShort[0] == longInt || longShort[0] == shortInt) ?
                    (int)(alloc / LTP[0]) : 0;

            for (int i = 1; i < Bid.Length; i++)
            {
                int lsOld = longShort[i - 1] == longInt ? 1 : longShort[i - 1] == shortInt ? -1 : 0;
                int lsNew = longShort[i] == longInt ? 1 : longShort[i] == shortInt ? -1 : 0;

                if (lsOld != lsNew)
                {
                    qty[i] = LTP[i] >0 ? Math.Abs(lsNew)*(int)(alloc / LTP[i]):0;
                }
                else
                {
                    qty[i] = qty[i - 1];
                }

                mtm[i] = (LTP[i] - LTP[i - 1]) * lsOld * qty[i - 1] -
                    LTP[i] * Math.Abs(lsNew * qty[i] - lsOld * qty[i - 1]) * cost;

                if (lsOld != lsNew)
                {
                    double spread = 0;
                    if (lsNew == -1)
                        spread += -1*(LTP[i] - Bid[i]) *  qty[i];
                    else if (lsNew == 1)
                        spread += (LTP[i] - Ask[i]) *  qty[i];

                    if (lsOld == -1)
                        spread += (LTP[i] - Ask[i]) * qty[i-1];
                    else if (lsOld == 1)
                        spread += -1*(LTP[i] - Bid[i]) * qty[i-1];

                    mtm[i] += spread;
                }
            }
            return mtm;
        }

        /// <summary>
        /// Generate MTM from Netposition
        /// </summary>
        /// <param name="Price"></param>
        /// <param name="longShort"></param>
        /// <param name="alloc"></param>
        /// <param name="longInt"></param>
        /// <param name="shortInt"></param>
        /// <param name="cost"></param>
        /// <param name="UseBidAsk"></param>
        /// <param name="UseOpen"></param>
        /// <returns></returns>
        public static List<double[]> GenerateMTM(TimeSeries Price, int[] longShort,
            double alloc, int longInt = 1, int shortInt = -1, double cost = 0.0,
            bool UseBidAsk = false, bool UseNetPosAsQty = false)
        {
            double[] LTP = Price.Prices;
            double[] Bid = Price.Prices;
            double[] Ask = Price.Prices;

            if (UseBidAsk)
            {
                Bid = Price.Bid;
                Ask = Price.Ask;
            }

            int[] qty = new int[LTP.Length];
            double[] mtm = new double[LTP.Length];
            double[] gtv = new double[LTP.Length];
            double[] ge = new double[LTP.Length];

            if (LTP[0] > 0 && !UseNetPosAsQty)
            {
                qty[0] = (longShort[0] == longInt || longShort[0] == shortInt) ?
                    (int)(alloc / LTP[0]) : 0;
                gtv[0] = (longShort[0] == longInt || longShort[0] == shortInt) ?
                    alloc : 0;
            }
            else if (LTP[0] > 0 && UseNetPosAsQty)
            {
                qty[0] = Math.Abs(longShort[0]);
                gtv[0] = qty[0] * LTP[0];
            }

            for (int i = 1; i < LTP.Length; i++)
            {
                int lsOld = longShort[i - 1] == longInt ? 1 : longShort[i - 1] == shortInt ? -1 : 0;
                int lsNew = longShort[i] == longInt ? 1 : longShort[i] == shortInt ? -1 : 0;

                if (UseNetPosAsQty)
                {
                    lsOld = Math.Sign(longShort[i - 1]);
                    lsNew = Math.Sign(longShort[i]);                    
                    qty[i] = LTP[i] > 0 ? Math.Abs(longShort[i]) : 0;
                    
                }
                else
                {
                    if (longShort[i - 1] != longShort[i])
                    {
                        qty[i] = LTP[i] > 0 ? Math.Abs(lsNew)*(int)(alloc / LTP[i]):0;
                    }
                    else
                    {
                        qty[i] = qty[i - 1];
                    }
                }

                mtm[i] = (LTP[i] - LTP[i - 1]) * lsOld * qty[i - 1] -
                    LTP[i] * Math.Abs(lsNew * qty[i] - lsOld * qty[i - 1]) * cost;

                if (UseNetPosAsQty)
                {
                    gtv[i] = LTP[i] * Math.Abs(longShort[i] - longShort[i-1]);
                    ge[i] = LTP[i] * Math.Abs(longShort[i]);
                }
                else
                {
                    gtv[i] = alloc * Math.Abs(lsNew - lsOld);
                    ge[i] = alloc * Math.Abs(lsNew);
                }

                if (longShort[i] != longShort[i-1])
                {
                    double spread = 0;
                    if (lsNew == -1)
                        spread += -1 * (LTP[i] - Bid[i]) * qty[i];
                    else if (lsNew == 1)
                        spread += (LTP[i] - Ask[i]) * qty[i];

                    if (lsOld == -1)
                        spread += (LTP[i] - Ask[i]) * qty[i - 1];
                    else if (lsOld == 1)
                        spread += -1 * (LTP[i] - Bid[i]) * qty[i - 1];

                    mtm[i] += spread;
                }
            }
            List<double[]> ret = new List<double[]>();
            ret.Add(mtm);
            ret.Add(gtv);
            ret.Add(ge);
            
            return ret;
        }

        public static List<Trade> GenerateTrades(TimeSeries series, int[] longShort,
            bool UseBidAsk = false, double cost = 0.0)
        {
            List<Trade> TradesArr = new List<Trade>();
            bool InTrade = false;
            int entIdx = 0;
            int n = series.Dates.Length;
            Trade temp = new Trade();

            double[] AskPx = series.Prices;
            double[] BidPx = series.Prices;

            if (UseBidAsk)
            {
                AskPx = series.Ask;
                BidPx = series.Bid;
            }

            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    if (longShort[i] > 0)
                    {
                        temp.EntryDate = series.Dates[i];
                        temp.EntryIdx = i;
                        temp.PositionType = true;
                        temp.LongShort = LongShortType.LONG;
                        temp.Quantity = Math.Abs(longShort[i]);
                        if (UseBidAsk)
                            temp.EntryPrice = AskPx[i];
                        else
                            temp.EntryPrice = series.Prices[i];
                        entIdx = i;
                        InTrade = true;
                    }
                    else if (longShort[i] < 0)
                    {
                        temp.EntryDate = series.Dates[i];
                        temp.EntryIdx = i;
                        temp.PositionType = false;
                        temp.LongShort = LongShortType.SHORT;
                        temp.Quantity = Math.Abs(longShort[i]);
                        if (UseBidAsk)
                            temp.EntryPrice = BidPx[i];
                        else
                            temp.EntryPrice = series.Prices[i];
                        entIdx = i;
                        InTrade = true;
                    }
                }
                else
                {
                    if (InTrade && longShort[i] > 0)// Long trade
                    {
                        if (longShort[i - 1] < 0)
                        {
                            temp.ExitDate = series.Dates[i];
                            temp.ExitIdx = i;
                            if (UseBidAsk)
                                temp.ExitPrice = AskPx[i];
                            else
                                temp.ExitPrice = series.Prices[i];

                            temp.ScripName = series.Name;
                            TradesArr.Add(new Trade(temp, cost));

                            temp = new Trade();
                            temp.EntryDate = series.Dates[i];
                            temp.EntryIdx = i;
                            temp.PositionType = true;
                            temp.LongShort = LongShortType.LONG;
                            temp.Quantity = Math.Abs(longShort[i]);

                            if (UseBidAsk)
                                temp.EntryPrice = AskPx[i];
                            else
                                temp.EntryPrice = series.Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }

                    }
                    else if (InTrade && longShort[i] < 0)
                    {
                        if (longShort[i - 1] > 0)
                        {
                            temp.ExitDate = series.Dates[i];
                            temp.ExitIdx = i;
                            if (UseBidAsk)
                                temp.ExitPrice = BidPx[i];
                            else
                                temp.ExitPrice = series.Prices[i];
                            temp.ScripName = series.Name;
                            TradesArr.Add(new Trade(temp, cost));

                            temp = new Trade();
                            temp.EntryDate = series.Dates[i];
                            temp.EntryIdx = i;
                            temp.PositionType = false;
                            temp.LongShort = LongShortType.SHORT;
                            temp.Quantity = Math.Abs(longShort[i]);

                            if (UseBidAsk)
                                temp.EntryPrice = BidPx[i];
                            else
                                temp.EntryPrice = series.Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }
                    }
                    else if (InTrade && longShort[i] == 0)
                    {
                        if (longShort[i - 1] > 0) //Long exit
                        {
                            temp.ExitDate = series.Dates[i];
                            temp.ExitIdx = i;
                            if (UseBidAsk)
                                temp.ExitPrice = BidPx[i];
                            else
                                temp.ExitPrice =series.Prices[i];
                            temp.ScripName = series.Name;
                            TradesArr.Add(new Trade(temp, cost));
                            temp = new Trade();
                            InTrade = false;

                        }
                        else if (longShort[i - 1] < 0)// Short exit
                        {
                            temp.ExitDate = series.Dates[i];
                            temp.ExitIdx = i;
                            if (UseBidAsk)
                                temp.ExitPrice = AskPx[i];
                            else
                                temp.ExitPrice = series.Prices[i];
                            temp.ScripName = series.Name;
                            TradesArr.Add(new Trade(temp, cost));
                            temp = new Trade();
                            InTrade = false;
                        }

                    }
                    else if (!InTrade)
                    {
                        if (longShort[i] > 0) // long Entry
                        {
                            temp = new Trade();
                            temp.EntryDate = series.Dates[i];
                            temp.EntryIdx = i;
                            temp.PositionType = true;
                            temp.LongShort = LongShortType.LONG;
                            temp.Quantity = Math.Abs(longShort[i]);

                            if (UseBidAsk)
                                temp.EntryPrice = AskPx[i];
                            else
                                temp.EntryPrice = series.Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }
                        else if (longShort[i] < 0) // short Entry
                        {
                            temp = new Trade();
                            temp.EntryDate = series.Dates[i];
                            temp.EntryIdx = i;
                            temp.PositionType = false;
                            temp.LongShort = LongShortType.SHORT;
                            temp.Quantity = Math.Abs(longShort[i]);

                            if (UseBidAsk)
                                temp.EntryPrice = BidPx[i];
                            else
                                temp.EntryPrice = series.Prices[i];
                            entIdx = i;
                            InTrade = true;
                        }
                    }
                }
            }

            return TradesArr;
        }

        public static int[] CreateBuySellSig(double[] Signal, TimeSeries Price, double LEA, double SEB,
            double LExB = Double.NegativeInfinity, double SExA = Double.PositiveInfinity, 
            bool UseBidAsk = false, bool HoldOverNightPos = true, bool UseTrailingSL = false,
            bool UseSL = false, bool UseTarget = false,
            double Stoploss = Double.PositiveInfinity, double Target = Double.PositiveInfinity,
            double TrailingSL = Double.PositiveInfinity, int skipPeriod = 0, bool UseOpen = false,
            bool IsReverseStrategy = false, int MaxHoldPeriod = Int32.MaxValue, bool AllowSignalFlip = true)
        {
            double[] Sig = Signal.Select(x => IsReverseStrategy ? -x : x).ToArray();
            int n = Sig.Length;
            DateTime[] dates = Price.Dates;
            double[] Prices = UseOpen? Price.OHLC.open : Price.Prices;
                        
            double[] BidPx = Price.Bid;
            double[] AskPx = Price.Ask;

            int[] BuySellSig = new int[n];
            bool InTrade = false;
            int lastTrdSLHitIdx = -1000;
            int lastTrdDirection = 0;
            double EntryPx = 0.0;
            int EntryIdx = 0;
            bool longExit = false;
            bool shortExit = false;

            if (!UseTrailingSL)
                TrailingSL = Double.PositiveInfinity;
            if (!UseSL)
                Stoploss = Double.PositiveInfinity;
            if (!UseTarget)
                Target = Double.PositiveInfinity;

            double trlstopVal= -TrailingSL;            
                       
            // Create Buy Sell signal
            for (int i = 0; i < n; i++)
            {
                longExit = false;
                shortExit = false;
                if (InTrade) // Check for Exit
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

                        trlstopVal = Math.Max(trlstopVal, ret - TrailingSL);

                        if ((Sig[i] <= LExB && AllowSignalFlip) || ret >= Target || ret <= -Stoploss ||
                            ret <= trlstopVal || i - EntryIdx >= MaxHoldPeriod )
                        {
                            BuySellSig[i] = 0;
                            InTrade = false;
                            longExit = true;
                            EntryPx = 0.0;
                            EntryIdx = 0;
                            lastTrdSLHitIdx = Sig[i] <= LExB ? lastTrdSLHitIdx : i;
                            lastTrdDirection = 1;
                        }
                    }
                    else if (BuySellSig[i] == -1)
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

                        trlstopVal = Math.Max(trlstopVal, ret - TrailingSL);

                        if ((Sig[i] >= SExA && AllowSignalFlip) || ret >= Target || ret <= -Stoploss ||
                            ret <= trlstopVal || i - EntryIdx >= MaxHoldPeriod)
                        {
                            BuySellSig[i] = 0;
                            InTrade = false;
                            shortExit = true;
                            EntryPx = 0.0;
                            EntryIdx = 0;
                            lastTrdSLHitIdx = Sig[i] >= SExA ? lastTrdSLHitIdx : i;
                            lastTrdDirection = -1;
                        }
                    }
                }
                if(!InTrade
                    || ((Sig[i] > LEA
                    || Sig[i] < SEB) && AllowSignalFlip)) // Check for Entry or flip
                {
                    if (Sig[i] > LEA
                        &&(lastTrdDirection != 1 || lastTrdSLHitIdx + skipPeriod <= i)
                        && BuySellSig[i] != 1
                        && !longExit)
                    {
                        lastTrdDirection = BuySellSig[i] = 1;
                        InTrade = true;
                        if (UseBidAsk)
                        {
                            EntryPx = AskPx[i];
                        }
                        else
                        {
                            EntryPx = Prices[i];
                        }
                        EntryIdx = i;
                        trlstopVal = -TrailingSL;
                    }
                    else if (Sig[i] < SEB
                        && (lastTrdDirection != -1 || lastTrdSLHitIdx + skipPeriod <= i)
                        && BuySellSig[i] != -1
                        && !shortExit)
                    {
                        lastTrdDirection = BuySellSig[i] = -1;
                        InTrade = true;
                        if (UseBidAsk)
                        {
                            EntryPx = BidPx[i];
                        }
                        else
                        {
                            EntryPx = Prices[i];
                        }
                        EntryIdx = i;
                        trlstopVal = -TrailingSL;
                    }
                }
                if (!HoldOverNightPos && InTrade)
                {
                    if (i < n - 1)
                    {
                        if (dates[i].Date != dates[i + 1].Date)
                        {
                            BuySellSig[i] = 0;
                            InTrade = false;
                            EntryPx = 0.0;
                            EntryIdx = 0;
                        }
                    }
                }
            }// for

            return BuySellSig;
        }
                
    }
}
