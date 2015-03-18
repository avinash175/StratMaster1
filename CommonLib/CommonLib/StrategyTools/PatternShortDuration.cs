using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class PatternShortDuration
    {
        /// <summary>
        /// Engulfing Pattern
        /// </summary>
        /// <param name="series">Time series input</param>
        /// <param name="NoOfBars">Number of bars in history to check for updown trend</param>
        /// <param name="PerBarsTrend"></param>
        /// <param name="RatioTrend"></param>
        /// <param name="DontUseUpDownTrend"></param>
        /// <returns></returns>
        public static int[] EngulfingLine(TimeSeries series, int NoOfBars = 5, double PerBarsTrend = 75, double RatioTrend = 2, bool DontUseUpDownTrend = true)
        {
            PerBarsTrend /= 100;
            int[] ret = new int[series.OHLC.dates.Length];

            double sum1 = 0.0, sum2 = 0.0;
            int n = 0, white = 0, black = 0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                sum1 = 0;
                sum2 = 0;
                white = 0;
                black = 0;
                n = NoOfBars;

                if (series.OHLC.close[i] > series.OHLC.open[i]
                    && series.OHLC.close[i - 1] < series.OHLC.open[i - 1]
                    && series.OHLC.close[i] > series.OHLC.open[i - 1]
                    && series.OHLC.open[i] < series.OHLC.close[i - 1])
                {
                    int j = i;

                    if (DontUseUpDownTrend)
                    {
                        ret[i] = 1;
                        continue;
                    }

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }
                    else if ((black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend))
                    {
                        ret[i] = 1;
                    }
                }
                else if (series.OHLC.close[i] < series.OHLC.open[i]
                   && series.OHLC.close[i - 1] > series.OHLC.open[i - 1]
                   && series.OHLC.close[i] < series.OHLC.open[i - 1]
                   && series.OHLC.open[i] > series.OHLC.close[i - 1])
                {
                    int j = i;


                    if (DontUseUpDownTrend)
                    {
                        ret[i] = -1;
                        continue;
                    }

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        n--;
                    }

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }
                    else if ((white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend))
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] ExhaustionBar(TimeSeries series, int NoOfBars = 5, double GapPerc = 0.5, double MagThresh = 2.5, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            GapPerc /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;
                sum3 = 0.0;

                if (Math.Min(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) / series.OHLC.open[i] - 1 > GapPerc
                    && (series.OHLC.open[i] - series.OHLC.low[i]) <= (0.25) * (series.OHLC.high[i] - series.OHLC.low[i])
                    && (series.OHLC.close[i] - series.OHLC.low[i]) >= (0.75) * (series.OHLC.high[i] - series.OHLC.low[i]))
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[j] - series.OHLC.low[j];

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && black / NoOfBars > PerBarsTrend
                        && sum2 / sum1 > RatioTrend)
                    {
                        ret[i] = 1;
                    }

                }

                else if (series.OHLC.open[i] / Math.Max(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) - 1 > GapPerc
                        && (series.OHLC.open[i] - series.OHLC.low[i]) >= (0.75) * (series.OHLC.high[i] - series.OHLC.low[i])
                        && (series.OHLC.close[i] - series.OHLC.low[i] <= (0.25) * (series.OHLC.high[i] - series.OHLC.low[i])))
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[j] - series.OHLC.low[j];

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && white / NoOfBars > PerBarsTrend
                        && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }

                }
            }

            return ret;
        }

        public static int[] Gravestone(TimeSeries series, int NoOfBars = 5, double ValuePerc = 95, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            ValuePerc /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if ((series.OHLC.high[i] - Math.Max(series.OHLC.open[i], series.OHLC.close[i])) / (series.OHLC.high[i] - series.OHLC.low[i]) > ValuePerc)
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    else if ((black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend))
                    {
                        ret[i] = 1;
                    }
                    else if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    else if ((white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend))
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] Hammer(TimeSeries series, int NoOfBars = 5, double UpperShadowRatio = 2.5, double BodyRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            BodyRatio /= 100;
            UpperShadowRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if (series.OHLC.open[i] - Math.Max(series.OHLC.close[i - 1], series.OHLC.open[i - 1]) > 0
                   && (series.OHLC.high[i - 1] - Math.Max(series.OHLC.close[i - 1], series.OHLC.open[i - 1])) / (series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) < UpperShadowRatio
                   && Math.Abs(series.OHLC.open[i - 1] - series.OHLC.close[i - 1]) / (series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) < BodyRatio)
                {
                    int j = i - 1;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    if ((black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend))
                    {
                        ret[i] = 1;
                    }
                }
            }

            return ret;
        }

        public static int[] InsideBar(TimeSeries series, int NoOfBars = 5, double SizeRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            SizeRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if ((series.OHLC.high[i] - series.OHLC.low[i]) / (series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) < SizeRatio
                    && series.OHLC.high[i] < series.OHLC.high[i - 1] && series.OHLC.low[i] > series.OHLC.low[i - 1])
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    else if ((black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend))
                    {
                        ret[i] = 1;
                    }

                    else if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    else if ((white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend))
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] InvertedHammer(TimeSeries series, int NoOfBars = 5, double LowerShadowRatio = 2.5, double BodyRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            LowerShadowRatio /= 100;
            BodyRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if (Math.Max(series.OHLC.open[i], series.OHLC.close[i]) - Math.Min(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) < 0
                   && (Math.Min(series.OHLC.open[i], series.OHLC.close[i]) - series.OHLC.low[i]) / (series.OHLC.high[i] - series.OHLC.low[i]) < LowerShadowRatio
                   && Math.Abs(series.OHLC.open[i] - series.OHLC.close[i]) / (series.OHLC.high[i] - series.OHLC.low[i]) < BodyRatio)
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    else if (black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend)
                    {
                        ret[i] = 1;
                    }
                }
            }

            return ret;
        }

        public static int[] KeyReversalBar(TimeSeries series, int NoOfBars = 5, double GapPerc = 0.5, double CloseDiffPer = 1, double MagThresh = 2.5, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            GapPerc /= 100;
            CloseDiffPer /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;
                sum3 = 0.0;

                if (Math.Min(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) / series.OHLC.open[i] - 1 > GapPerc
                    && (Math.Abs(series.OHLC.close[i] / series.OHLC.close[i - 1] - 1) < CloseDiffPer))
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[i] - series.OHLC.low[i];

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && black / NoOfBars > PerBarsTrend
                        && sum2 / sum1 > RatioTrend)
                    {
                        ret[i] = 1;
                    }

                }

                else if (series.OHLC.open[i] / Math.Max(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) - 1 > GapPerc
                    && (Math.Abs(series.OHLC.close[i] / series.OHLC.close[i - 1] - 1) < CloseDiffPer))
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[i] - series.OHLC.low[i];

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && white / NoOfBars > PerBarsTrend
                        && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] OutsideBar(TimeSeries series, int NoOfBars = 5, double SizeRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            SizeRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if ((series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) / (series.OHLC.high[i] - series.OHLC.low[i]) < SizeRatio
                    && series.OHLC.high[i] > series.OHLC.high[i - 1] && series.OHLC.low[i] < series.OHLC.low[i - 1])
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    else if (black / NoOfBars > PerBarsTrend && sum2 / sum1 > RatioTrend)
                    {
                        ret[i] = 1;
                    }

                    else if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    else if (white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] HangingMan(TimeSeries series, int NoOfBars = 5, double UpperShadowRatio = 2.5, double BodyRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            UpperShadowRatio /= 100;
            BodyRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if (series.OHLC.open[i] - Math.Min(series.OHLC.close[i - 1], series.OHLC.open[i - 1]) < 0
                   && (series.OHLC.high[i - 1] - Math.Max(series.OHLC.close[i - 1], series.OHLC.open[i - 1])) / (series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) < UpperShadowRatio
                   && Math.Abs(series.OHLC.open[i - 1] - series.OHLC.close[i - 1]) / (series.OHLC.high[i - 1] - series.OHLC.low[i - 1]) < BodyRatio)
                {
                    int j = i - 1;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    if (white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] ShootingStar(TimeSeries series, int NoOfBars = 5, double LowerShadowRatio = 2.5, double BodyRatio = 30, double PerBarsTrend = 75, double RatioTrend = 2)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            LowerShadowRatio /= 100;
            BodyRatio /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;

                if (Math.Min(series.OHLC.open[i], series.OHLC.close[i]) - Math.Max(series.OHLC.open[i - 1], series.OHLC.close[i - 1]) > 0
                   && (Math.Min(series.OHLC.open[i], series.OHLC.close[i]) - series.OHLC.low[i]) / (series.OHLC.high[i] - series.OHLC.low[i]) < LowerShadowRatio
                   && Math.Abs(series.OHLC.open[i] - series.OHLC.close[i]) / (series.OHLC.high[i] - series.OHLC.low[i]) < BodyRatio)
                {
                    int j = i;

                    while (n > 0)
                    {
                        j--;

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    else if (white / NoOfBars > PerBarsTrend && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

        public static int[] TwoBarReversal(TimeSeries series, int NoOfBars = 5, double MagThresh = 2.5, double GapPerc=1, double PerBarsTrend = 75, double RatioTrend = 2, bool UseUpDownTrend = true)
        {
            int[] ret = new int[series.OHLC.dates.Length];
            PerBarsTrend /= 100;
            MagThresh /= 100;
            GapPerc /= 100;

            int n, white = 0, black = 0;
            double sum1 = 0.0, sum2 = 0.0, sum3=0.0;

            for (int i = NoOfBars + 2; i < ret.Length; i++)
            {
                n = NoOfBars;
                white = 0;
                black = 0;
                sum1 = 0.0;
                sum2 = 0.0;
                sum3 = 0.0;

                double range = series.OHLC.high[i] - series.OHLC.low[i];
                double range1 = series.OHLC.high[i-1] - series.OHLC.low[i-1];

                if ((series.OHLC.close[i - 1] - series.OHLC.low[i - 1]) <= 0.25 * (series.OHLC.high[i - 1] - series.OHLC.low[i - 1])
                    && Math.Abs(series.OHLC.open[i] - series.OHLC.close[i - 1])/range < GapPerc
                    && Math.Abs(series.OHLC.close[i] - series.OHLC.open[i - 1])/range < GapPerc
                    && Math.Abs(range1/range -1) < 0.20)
                {
                    int j = i - 1;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[j] - series.OHLC.low[j];

                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum1 == 0)
                    {
                        ret[i] = 1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && black / NoOfBars > PerBarsTrend 
                        && sum2 / sum1 > RatioTrend)
                    {
                        ret[i] = 1;
                    }
                }

                else if ((series.OHLC.close[i - 1] - series.OHLC.low[i - 1]) >= 0.75 * (series.OHLC.high[i - 1] - series.OHLC.low[i - 1])
                    && Math.Abs(series.OHLC.open[i] - series.OHLC.close[i - 1])/range < GapPerc
                    && Math.Abs(series.OHLC.close[i] - series.OHLC.open[i - 1])/range < GapPerc
                    && Math.Abs(range1 / range - 1) < 0.20)
                {
                    int j = i - 1;

                    while (n > 0)
                    {
                        j--;

                        sum3 += series.OHLC.high[j] - series.OHLC.low[j];
                       
                        if (series.OHLC.close[j] >= series.OHLC.open[j])
                        {
                            white++;
                            sum1 += series.OHLC.close[j] - series.OHLC.open[j];
                        }

                        if (series.OHLC.close[j] < series.OHLC.open[j])
                        {
                            black++;
                            sum2 += series.OHLC.open[j] - series.OHLC.close[j];
                        }

                        n--;
                    }

                    sum3 /= NoOfBars;

                    if (sum2 == 0)
                    {
                        ret[i] = -1;
                    }

                    if ((series.OHLC.high[i] - series.OHLC.low[i]) / sum3 > MagThresh
                        && white / NoOfBars > PerBarsTrend 
                        && sum1 / sum2 > RatioTrend)
                    {
                        ret[i] = -1;
                    }
                }
            }

            return ret;
        }

    }
}
