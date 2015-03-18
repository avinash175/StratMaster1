using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommonLib
{
    public class Technicals
    {
        public static double[] MovAvg(double[] series, int nwindow )
        {
            int i,n;
            n = series.Length;
            double[] output = new double[n];
            List<double> window = new List<double>();

            for (i = 0; i < nwindow; i++)
            {
                output[i] = UF.SumArray(UF.GetRange(series, 0, i)) / (i + 1);
                window.Add(series[i]);
            }
            
            for (i = nwindow; i < n; i++)
            {                
                double sum = output[i - 1] * nwindow + series[i] - window[0];
                output[i] = sum/nwindow;
                window.Add(series[i]);
                window.RemoveAt(0);
            }
            return output;
        }

        public static double[] MovAvgInefficient(double[] series, int nwindow)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];

            for (i = 0; i < nwindow - 1; i++)
            {
                output[i] = UF.SumArray(UF.GetRange(series, 0, i)) / (i + 1);
            }

            for (i = nwindow - 1; i < n; i++)
            {
                output[i] = UF.SumArray(UF.GetRange(series, i - nwindow + 1, i)) / nwindow;
            }

            return output;
        }

        public static double[] MovAvg(double[] series, int nwindow, int startIdx)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];

            for (i = startIdx+nwindow - 1; i < n; i++)
            {
                output[i] = UF.SumArray(UF.GetRange(series, i - nwindow + 1, i)) / nwindow;
            }

            return output;
        }

        public static double[] MoneyFlowIndex(TimeSeries ts, int nBars = 14)
        {
            if(ts.OHLC == null)
            {
                throw new Exception("Use OHLCV data");
            }
            else if (ts.OHLC.volume == null)
            {
                throw new Exception("Use OHLCV data");
            }
            int len = ts.Dates.Length;
            double[] ret = new double[len];
            double[] moneyflow = ts.OHLC.close.Select((x, i) => ts.OHLC.volume[i] *
                (x + ts.OHLC.high[i] + ts.OHLC.low[i]) / 3.0).ToArray();
            double[] roc = ROC(ts.Prices, 1);

            for (int i = 0; i < nBars; i++)
            {
                ret[i] = 50;
            }

            for (int i = nBars; i < len; i++)
            {
                double[] lmf = UF.GetRange(moneyflow, i - nBars, i);
                double[] lroc = UF.GetRange(roc, i - nBars, i);
                double pmf = lmf.Select((x, j) => lroc[j] > 0 ? x : 0).Sum();
                double nmf = lmf.Select((x, j) => lroc[j] < 0 ? x : 0).Sum();
                ret[i] = 100.0 * pmf / (pmf + nmf);
            }

            return ret;
        }

        /// <summary>
        /// 1st-> MA
        /// 2nd-> MA + std
        /// 3rd-> MA - std
        /// </summary>
        /// <param name="px">price series</param>
        /// <param name="maPeriod">moving average period</param>
        /// <param name="std">standard deviation multiple</param>
        /// <returns></returns>
        public static List<double[]> BollingerBand(double[] px, int maPeriod, double std)
        {
            List<double[]> ret = new List<double[]>();
            double[] ma = MovAvg(px, maPeriod);
            ret.Add(ma);

            double[] stdVal = new double[px.Length];

            for (int i = maPeriod; i < px.Length; i++)
            {
                stdVal[i] = UF.StandardDeviation(UF.GetRange(px, i - maPeriod, i));
            }
                        
            double[] stdP = stdVal.Select((x, i) => ma[i] + std * x).ToArray();
            double[] stdN = stdVal.Select((x, i) => ma[i] - std * x).ToArray();
            ret.Add(stdP);
            ret.Add(stdN);

            return ret;
        }

        public static double[] BreathPerUp(List<double[]> securities, int lbwin)
        {            
            int[] idxs = securities.Select((x, i) => (x == null || x.Length==0) ? -1 : i)
                .Where(x => x > -1).ToArray();

            if (idxs.Length == 0)
                return null;

            if (!securities.Where((x, i) => idxs.Contains(i)).Select(x => x.Length).All(x => x == securities[idxs[0]].Length))
                return null;

            double[] ret = new double[securities[idxs[0]].Length];
            
            for (int j = 0; j < lbwin; j++)
			{
			    ret[j] = 0.5;
			}

            for (int j =lbwin; j < ret.Length; j++)
            {
                for (int i = 0; i < idxs.Length; i++)
                {
                    int idx = idxs[i];
                    if (securities[idx][j] - securities[idx][j - lbwin] > 0)
                    {
                        ret[j] += 1;
                    }
                }
                ret[j] /= idxs.Length;
            }

            return ret;
        }

        public static double[] ExpMovAvg(double[] series, int nday)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];
            double lambda, ewmaYest, ewmaToday;

            lambda = 2.0 / (1+nday);

            for (i = 0; i < nday; i++)
            {
                output[i] = UF.SumArray(UF.GetRange(series, 0,i)) / (i+1);
            }
            ewmaYest = output[nday-1];// = series[0];
            for (i = nday; i < n; i++)
            {
                ewmaToday = ewmaYest + lambda * (series[i] - ewmaYest);
                output[i] = ewmaToday;
                ewmaYest = ewmaToday;
            }

            return output;
        }

        public static double[] ExpMovAvg(double[] series, double lambda)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];
            double ewmaYest, ewmaToday;

            output[0] = series[0];
            ewmaYest = output[0];
            for (i = 0; i < n; i++)
            {
                ewmaToday = ewmaYest + lambda * (series[i] - ewmaYest);
                output[i] = ewmaToday;
                ewmaYest = ewmaToday;
            }

            return output;

        }
        // pass returns
        public static double[] EWMAVol(double[] series, double lambda)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];
            double ewmaYest, ewmaToday;

            output[0] = Math.Abs(series[0]) * Math.Sqrt(252.0);
            ewmaYest = output[0] * output[0];
            for (i = 0; i < n; i++)
            {
                ewmaToday = ewmaYest + lambda * (series[i] * series[i] - ewmaYest);
                output[i] = Math.Sqrt(ewmaToday) * Math.Sqrt(252.0);
                ewmaYest = ewmaToday;
            }
            return output;
        }

        public static double[] EMAVol(double[] series, double lambda)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];
            double ewmaYest, ewmaToday;

            output[0] = Math.Abs(series[0]);
            ewmaYest = output[0] * output[0];
            for (i = 0; i < n; i++)
            {
                ewmaToday = ewmaYest + lambda * (series[i] * series[i] - ewmaYest);
                output[i] = Math.Sqrt(ewmaToday);
                ewmaYest = ewmaToday;
            }
            return output;
        }

        public static double[] ModExpMovAvg(double[] series, int nday)
        {
            int i, n;
            n = series.Length;
            double[] output = new double[n];
            double lambda, ewmaYest, ewmaToday;
            
            lambda = 1.0 / (nday);
            
            for (i = 0; i < nday - 1; i++)
            {
                output[i] = Double.NaN;
            }
            
            output[nday - 1] = UF.SumArray(UF.GetRange(series, 0, nday - 1)) / nday;
            ewmaYest = output[nday - 1];
            for (i = nday; i < n; i++)
            {
                ewmaToday = ewmaYest + lambda * (series[i] - ewmaYest);
                output[i] = ewmaToday;
                ewmaYest = ewmaToday;
            }

            return output;

        }

        public static double[] ATRSeries(OHLCDataSet series, int ndays)
        {
            int i, n;
            n = series.numOfElements;
            double[] ATRday = new double[n];
            ATRday[0] = series.high[0] - series.low[0];
            for (i = 1; i < n; i++)
            {
                ATRday[i] = Math.Max(series.high[i] - series.low[i],
                    Math.Max(Math.Abs(series.high[i] - series.close[i-1]), 
                    Math.Abs(series.low[i] - series.close[i-1])));
            }

            double[] ATR = MovAvg(ATRday, ndays);

            return ATR;
        }

        public static double[] ATR(OHLCDataSet series, int ndays)
        {
            int i, n;
            n = series.numOfElements;
            double[] ATRday = new double[n];
            ATRday[0] = series.high[0] - series.low[0];
            for (i = 1; i < n; i++)
            {
                ATRday[i] = Math.Max(series.high[i] - series.low[i],
                    Math.Max(Math.Abs(series.high[i] - series.close[i - 1]),
                    Math.Abs(series.low[i] - series.close[i - 1])));
            }

            double[] ATR = ExpMovAvg(ATRday, ndays);

            return ATR;
        }

        public static int[] DojiIndicator(OHLCDataSet series, int ndaysFwd = 1, double eps = 1.0/10000.0)
        {
            int i, n;
            n = series.numOfElements;
            int[] doji = new int[n];
            
            for (i = 0; i < n; i++)
            {
                if (Math.Abs(series.open[i] / series.close[i]-1) <= eps)
                {
                    for (int j = i+1; j <= i+ndaysFwd && j<n; j++)
                    {
                        doji[j] = 1;
                    }
                }
            }
            return doji;
        }

        public static ArrayCollection<double> UpsNDowns(double[] series)
        {
            ArrayCollection<double> ret = new ArrayCollection<double>();

            double[] ups = new double[series.Length];
            double[] downs = new double[series.Length];

            for (int i = 1; i < series.Length; i++)
            {
                if (series[i] > series[i - 1])
                {
                    ups[i] = series[i] - series[i - 1];
                }
                else
                {
                    downs[i] = series[i - 1] - series[i];
                }
            }

            ret.Coll.Add(ups);
            ret.Coll.Add(downs);

            return ret;
        }

        public static double[] RSI(double[] series, int ndays = 14)
        {
            double[] ret = new double[series.Length];

            ArrayCollection<double> upsNdowns = UpsNDowns(series);

            double[] ups = upsNdowns.Coll[0];
            double[] downs = upsNdowns.Coll[1];

            double[] RS = UF.ArrayDiv(ExpMovAvg(ups, ndays), ExpMovAvg(downs, ndays));

            for (int i = 0; i < series.Length; i++)
            {
                ret[i] = 100 - 100 / (1 + RS[i]);
            }

            return ret;
        }

        public static double[] MACD(double[] series, int shortWin = 12, int longWin = 26)
        {
            double[] ret = new double[series.Length];

            ret = UF.ArraySub(ExpMovAvg(series, shortWin), ExpMovAvg(series, longWin));

            return ret;
        }

        public static double[] MACDHist(double[] series, int sigWin = 9, int shortWin = 12, int longWin = 26)
        {
            double[] ret = new double[series.Length];

            double[] macd = UF.ArraySub(ExpMovAvg(series, shortWin), ExpMovAvg(series, longWin));
            ret = UF.ArraySub(macd, ExpMovAvg(series, sigWin));

            return ret;
        }

        public static double[] StochasticK(double[] series, int ndays = 14)
        {
            double[] Hhigh = Extrema(series, true, ndays);
            double[] Llow = Extrema(series, false, ndays);

            double[] ret = new double[series.Length];

            for (int i = 0; i < series.Length; i++)
            {
                if (Hhigh[i] - Llow[i] > 0)
                {
                    ret[i] = 100 * (series[i] - Llow[i]) / (Hhigh[i] - Llow[i]);
                }
                else
                {
                    ret[i] = 0;
                }
            }

            return ret;
        }

        public static int[] BreakOutIndicator(double[] series, int lbWin = 10)
        {
            int[] ret = new int[series.Length];
            for (int i = lbWin; i < series.Length; i++)
            {
                double[] vals = UF.GetRange(series, i - lbWin, i - 1);
                double max = UF.MaxArray(vals);
                double min = UF.MinArray(vals);
                if (series[i] > max)
                    ret[i] = 1;
                else if (series[i] < min)
                    ret[i] = -1;
            }
            return ret;
        }

        public static double[] WilliamR(double[] series, int ndays = 14)
        {
            double[] Hhigh = Extrema(series, true, ndays);
            double[] Llow = Extrema(series, false, ndays);

            double[] ret = new double[series.Length];

            for (int i = 0; i < series.Length; i++)
            {
                ret[i] = -100 * (Hhigh[i] - series[i]) / (Hhigh[i] - Llow[i]);
            }
            return ret;
        }

        public static double[] ADO(OHLCDataSet series)
        {
            double[] ret = new double[series.close.Length];

            for (int i = 0; i < series.close.Length; i++)
            {
                if (series.high[i] - series.low[i] > 0)
                    ret[i] = 100 * (series.high[i] + series.close[i] - series.open[i] - series.low[i]) /
                        (2 * (series.high[i] - series.low[i]));
                else
                    ret[i] = 0;

            }
            return ret;
        }

        public static double[] Extrema(double[] series, bool max = true, int ndays = 14)
        {
            double[] ret = new double[series.Length];                       
            
            for (int i = 0; i < series.Length; i++)
            {
                ret[i] = max ? UF.MaxArray(UF.GetRange(series, Math.Max(i - ndays,0), i)) :
                    UF.MinArray(UF.GetRange(series, Math.Max(i - ndays, 0), i));
            }
            return ret;
        }

        public static double[] DIPlus(OHLCDataSet series, int ndays)
        {
            int i, n;
            n = series.numOfElements;
            double[] ATR = ATRSeries(series, ndays);

            double[] upMove = new double[n];
            double[] downMove = new double[n];
            double[] DMplus = new double[n];
            //double[] DMminus = new double[n];

            for (i = 1; i < n; i++)
            {
                upMove[i] = series.high[i] - series.high[i - 1];
                downMove[i] = series.low[i - 1] - series.low[i];
                if (upMove[i] > downMove[i] && upMove[i] > 0)
                {
                    DMplus[i] = upMove[i];
                }
                else
                {
                    DMplus[i] = 0;
                }

            }

            double[] DMplusExp = ExpMovAvg(DMplus, ndays);

            double[] DIp = new double[n];

            for (i = 0; i < n; i++)
            {
                if (Double.IsNaN(DMplusExp[i])|| Double.IsNaN(ATR[i])||ATR[i]==0)
                {
                    DIp[i] = 0;
                }
                else
                {
                    DIp[i] = Math.Round(100 * DMplusExp[i] / ATR[i]);
                }
            }
            return DIp;            
        }

        public static double[] DIMinus(OHLCDataSet series, int ndays)
        {
            int i, n;
            n = series.numOfElements;
            double[] ATR = ATRSeries(series, ndays);

            double[] upMove = new double[n];
            double[] downMove = new double[n];
            double[] DMminus = new double[n];
            //double[] DMminus = new double[n];

            for (i = 1; i < n; i++)
            {
                upMove[i] = series.high[i] - series.high[i - 1];
                downMove[i] = series.low[i - 1] - series.low[i];
                if (downMove[i] > upMove[i] && downMove[i] > 0)
                {
                    DMminus[i] = downMove[i];
                }
                else
                {
                    DMminus[i] = 0;
                }

            }

            double[] DMminusExp = ExpMovAvg(DMminus, ndays);

            double[] DIm = new double[n];

            for (i = 0; i < n; i++)
            {
                if (Double.IsNaN(DMminusExp[i]) || Double.IsNaN(ATR[i]) || ATR[i] == 0)
                {
                    DIm[i] = 0.0;
                }
                else
                {
                    DIm[i] = Math.Round(100 * DMminusExp[i] / ATR[i]);
                }
            }

            return DIm;
        }

        public static double[] ADX(OHLCDataSet series, int nBars = 14)
        {
            double[] adx = new double[series.dates.Length];

            double[] DIP = DIPlus(series, nBars);
            double[] DIM = DIMinus(series, nBars);
            double[] DX = DIP.Select((x, i) => 100.0 * Math.Abs(x - DIM[i]) / (x + DIM[i])).ToArray();
            adx = MovAvg(DX, nBars);
            return adx;
        }

        public static double[] DrawDown(double[] X)
        {
            int n = X.Length;
            double[] DD = new double[n];
            double peak = X[0];
            for (int i = 0; i < n; i++)
            {
                if (X[i] > peak) peak = X[i];
                DD[i] = 100 * (peak - X[i]) / peak;
            }
            return DD;
        }

        public static double[] DrawDownAbs(double[] X)
        {
            int n = X.Length;
            double[] DD = new double[n];
            double peak = X[0];
            for (int i = 0; i < n; i++)
            {
                if (X[i] > peak) peak = X[i];
                DD[i] = (peak - X[i]);
            }
            return DD;
        }

        public static double[] OBV(double[] series, double[] volume)
        {
            double[] ret = new double[series.Length];

            ret[0] = volume[0];
            for (int i = 1; i < series.Length; i++)
            {
                ret[i] = Math.Sign(series[i] - series[i - 1]) * volume[i] + ret[i - 1];
            }

            return ret;
        }

        public static double[] ROC(double[] series, int nDays)
        {
            double[] ret = new double[series.Length];

            for (int i = 0; i < nDays; i++)
            {
                ret[i] = 0;
            }

            for (int i = nDays; i < series.Length; i++)
            {
                ret[i] = 100 * (series[i] / series[i - nDays] - 1.0);
            }
            return ret;
        }

        /// <summary>
        /// Don't use! FORWARD LOOKING
        /// USE IT ONLY FOR TRAINING "AI"
        /// </summary>
        /// <param name="series"></param>
        /// <param name="nDays"></param>
        /// <returns></returns>
        public static double[] ROCForward(double[] series, int nDays)
        {
            double[] ret = new double[series.Length];
            
            for (int i = 0; i < series.Length - nDays; i++)
            {
                ret[i] = 100 * (series[i + nDays] / series[i] - 1.0);
            }

            return ret;
        }

        public static double[] Tresholding(double[] series, double upperLim, double lowerLim)
        {
            if (upperLim >= lowerLim)
            {
                IEnumerable<double> thresh = series.Select(x => x > upperLim ? upperLim :
                    x < lowerLim ? lowerLim : x);
                return thresh.ToArray();
            }
            else
            {
                IEnumerable<double> thresh = series.Select(x => Double.NaN);
                return thresh.ToArray();
            }            
        }

        public static double[] Change(double[] A, int n)
        {
            double[] B = new double[A.Length];

            for (int i = n; i < A.Length; i++)
            {
                B[i] = A[i] - A[i - n];
            }

            return B;
        }

        public static double[] ChangeFwd(double[] A, int n)
        {
            double[] B = new double[A.Length];

            for (int i = 0; i < A.Length - n; i++)
            {
                B[i] = A[i + n] - A[i];
            }

            return B;
        }

        public static double[] Change(DateTime[] A, int n)
        {
            double[] B = new double[A.Length];

            for (int i = n; i < A.Length; i++)
            {
                B[i] = A[i].ToOADate() - A[i - n].ToOADate();
            }

            return B;
        }

        public static double[] AbsChange(double[] A, int n)
        {
            double[] B = new double[A.Length];

            for (int i = n; i < A.Length; i++)
            {
                B[i] = Math.Abs(A[i] - A[i - n]);
            }

            return B;
        }

        public static TrendLines[] TDLine(TimeSeries series)
        {
            Dictionary<int, double> TDsp = new Dictionary<int, double>();
            Dictionary<int, double> TDdp = new Dictionary<int, double>();

            // Find TD points
            for (int i = 2; i < series.OHLC.dates.Length - 1; i++)
            {
                if (series.OHLC.high[i] >= series.OHLC.high[i + 1]
                    && series.OHLC.high[i] >= series.OHLC.high[i - 1]
                    && series.OHLC.high[i] >= series.OHLC.close[i-2])
                {
                    TDsp.Add(i, series.OHLC.high[i]);
                }
                if(series.OHLC.low[i] <= series.OHLC.low[i + 1]
                    && series.OHLC.low[i] <= series.OHLC.low[i - 1]
                    && series.OHLC.low[i] <= series.OHLC.close[i - 2])
                {
                    TDdp.Add(i, series.OHLC.low[i]);
                }
            }

            TrendLines[] ret = new TrendLines[series.OHLC.dates.Length];

            if (TDdp.Count < 2 || TDsp.Count < 2)
            {
                return ret;
            }            

            int startIdx = Math.Max(TDsp.ElementAt(1).Key, TDdp.ElementAt(1).Key);

            int[] keys = TDsp.Keys.Select(x => x).ToArray();
            int[] keyd = TDdp.Keys.Select(x => x).ToArray();

            for (int i = startIdx; i < series.OHLC.dates.Length; i++)
            {
                bool supplyPresent = false;
                bool demandPresent = false;

                double sX1 = 0, sY1 = 0, sX2 = 0, sY2 = 0, dX1 = 0, dY1 = 0, dX2 = 0, dY2 = 0;

                int idxs = Array.BinarySearch(keys, i);
                if (idxs < 0)
                {
                    idxs = ~idxs;
                    idxs--;
                }
                else
                {
                    idxs--;
                }
               
                if (idxs > 0)
                {
                    sX2 = TDsp.ElementAt(idxs).Key;
                    sY2 = TDsp.ElementAt(idxs).Value;
                    while (idxs > 0)
                    {
                        idxs--;
                        sX1 = TDsp.ElementAt(idxs).Key;
                        sY1 = TDsp.ElementAt(idxs).Value;
                        if (sY1 > sY2)
                        {
                            supplyPresent = true;
                            break;
                        }
                    }
                }

                int idxd = Array.BinarySearch(keyd, i);
                if (idxd < 0)
                {
                    idxd = ~idxd;
                    idxd--;
                }
                else
                {
                    idxd--;
                }

                if (idxd > 0)
                {
                    dX2 = TDdp.ElementAt(idxd).Key;
                    dY2 = TDdp.ElementAt(idxd).Value;
                    while (idxd > 0)
                    {
                        idxd--;
                        dX1 = TDdp.ElementAt(idxd).Key;
                        dY1 = TDdp.ElementAt(idxd).Value;
                        if (dY1 < dY2)
                        {
                            demandPresent = true;
                            break;
                        }
                    }
                }
                
                if (supplyPresent || demandPresent)
                {                    
                    Line supplyLine = null, demandLine = null;

                    if (supplyPresent)
                    {
                        supplyLine = new Line(new Point(sX1, sY1), new Point(sX2, sY2));                        
                    }
                    if (demandPresent)
                    {
                        demandLine = new Line(new Point(dX1, dY1), new Point(dX2, dY2)); 
                    }

                    ret[i] = new TrendLines(supplyLine, demandLine);
                }                
            }
            return ret;
        }

        public static double[] FVE(TimeSeries series, int window, double fac = 0.003)
        {
            double typtoday = 0.0, typyest = 0.0;
            double midtoday = 0.0;

            double[] movavg = MovAvg(series.OHLC.volume, window);

            double[] ret = new double[series.OHLC.dates.Length];
            double[] vol = series.OHLC.volume.Select(x => x).ToArray();

            for (int i = 1; i < series.OHLC.dates.Length; i++)
            {
                typtoday = (series.OHLC.high[i] + series.OHLC.low[i] + series.OHLC.close[i]) / 3;
                typyest = (series.OHLC.high[i - 1] + series.OHLC.low[i - 1] + series.OHLC.close[i - 1]) / 3;

                midtoday = (series.OHLC.high[i] + series.OHLC.low[i]) / 2;

                if (series.OHLC.close[i] - midtoday + typtoday - typyest > fac * series.OHLC.close[i])
                {
                    vol[i] = series.OHLC.volume[i];
                }
                else if (series.OHLC.close[i] - midtoday + typtoday - typyest < -fac * series.OHLC.close[i])
                {
                    vol[i] = -(series.OHLC.volume[i]);
                }
                else
                {
                    vol[i] = 0;
                }
            }

            double[] vol1 = MovAvg(vol, window);

            ret = vol1.Select((x, i) => 100 * x / movavg[i]).ToArray();

            return ret;
        }

        public static double[] EntropyInd(TimeSeries series, int window)
        {
            double[] ret = new double[series.Dates.Length];
            double[] logchange = new double[series.Dates.Length];

            for (int i = 1; i < series.Dates.Length; i++)
            {
                logchange[i] = Math.Log(series.Prices[i] / series.Prices[i - 1]);
            }

            double[] avg = MovAvg(logchange, window);
            double[] rms = MovAvg(logchange.Select(x => x * x).ToArray(), window);
            rms = rms.Select(x => Math.Pow(x, 0.5)).ToArray();

            ret = avg.Select((x, i) => x == 0 && rms[i] == 0 ? 0.5 : ((x / rms[i]) + 1) / 2).ToArray();

            return ret;
        }

        public static double[] ADL(TimeSeries series)
        {
            double[] ret = new double[series.OHLC.dates.Length];

            for (int i = 1; i < series.OHLC.dates.Length; i++)
            {
                double mfm = 0.0;

                mfm = ((series.OHLC.close[i] - series.OHLC.low[i]) - (series.OHLC.high[i] - series.OHLC.close[i])) 
                       / (series.OHLC.high[i] - series.OHLC.low[i]);

                ret[i] = ret[i - 1] + mfm * series.OHLC.volume[i];
            }

            return ret;
        }

        public static double[] CCI(TimeSeries series, int winLen)
        {
            double[] ret = new double[series.OHLC.dates.Length];

            double[] typ = series.OHLC.high.Select((x, i) => (x + series.OHLC.low[i] + series.OHLC.close[i]) / 3).ToArray();
            double[] typavg = MovAvg(typ, winLen);

            double[] md = new double[series.OHLC.dates.Length];

            for (int i = winLen; i < series.OHLC.dates.Length; i++)
            {
                md[i] = typ.Where((x,j)=>j<=i).Select(x => Math.Abs(x - typavg[i])).Sum()/winLen;
                ret[i] = (typ[i] - typavg[i]) / (0.015 * md[i]);
            }

            return ret;
        }

        public static double[] ForceIndex(TimeSeries series, int window)
        {
            double[] fi = new double[series.OHLC.dates.Length];

            for (int i = 1; i < series.OHLC.dates.Length; i++)
            {
                fi[i] = (series.OHLC.close[i] - series.OHLC.close[i - 1]) * series.OHLC.volume[i];
            }

            double[] ret = ExpMovAvg(fi, window);

            return ret;
        }

        public static double[] ChaikinInd(TimeSeries series, int smallwindow, int largewindow)
        {
            double[] ret = new double[series.OHLC.dates.Length];

            double[] m1 = ExpMovAvg(ADL(series), smallwindow);
            double[] m2 = ExpMovAvg(ADL(series), largewindow);

            ret = m1.Select((x, j) => x - m2[j]).ToArray();

            return ret;
            
         }

        public static double[] REI(TimeSeries series, int period, bool useOHLC)
        {

            double[] high =null, low = null, close = null;
            if (useOHLC)
            {
                high = series.OHLC.high;
                low = series.OHLC.low;
                close = series.OHLC.close;
            }
            else
            {
                high = low = close = series.OHLC.close;
            }

            double[] ret = new double[series.Dates.Length];
            double[] s1 = new double[series.Dates.Length];
            double[] s2 = new double[series.Dates.Length];

            for (int i = 2*period; i < series.Dates.Length; i++)
            {
                double[] s = new double[period];
                double[] ss = new double[period];
                int m = 0, n = 0, o = 0;

                for (int j = i; j > i - period; j--)
                {
                    if(high[j - 2] < close[j - 7] && high[j - 2] < close[j - 8]
                       && high[j] < high[j - 5] && high[j] < high[j - 6])
                        n = 0;
                    else
                        n = 1;

                    if (low[j - 2] > close[j - 7] && low[j - 2] > close[j - 8]
                       && low[j] > low[j - 5] && low[j] > low[j - 6])
                        m = 0;
                    else
                        m = 1;

                    s[o] = m*n*(high[j] - high[j - 2] + low[j] - low[j - 2]);
                    ss[o] = Math.Abs(high[j] - high[j - 2]) + Math.Abs(low[j] - low[j - 2]);
                    o++;
                }

                s1[i]=s.Sum();
                s2[i] = ss.Sum();

                ret[i] = s1[i] / s2[i] * 100;
            }

            return ret;
        }

        public static double[][] MassIndex(TimeSeries series, int emaperiod, int miperiod)
        {
            double[] ema1 = ExpMovAvg(series.OHLC.high.Select((x, j) => x - series.OHLC.low[j]).ToArray(), emaperiod);
            double[] ema2 = ExpMovAvg(ema1, emaperiod);
            double[] ema = ema1.Select((x, j) => (x == 0 && ema2[j] == 0) ? 0 : x / ema2[j]).ToArray();

            double[] ma = MovAvg(series.OHLC.close, emaperiod);

            double[] a = new double[series.OHLC.dates.Length];
            double[] b = new double[series.OHLC.dates.Length];
    
            double[][] ret = new double[2][];

            for (int i = miperiod-1; i < series.OHLC.dates.Length; i++)
            {
                for (int j = i; j > i-miperiod; j--)
                {
                    b[i] += ema[j];
                }

                if (series.OHLC.close[i] >= ma[i])
                    a[i] = 1;
                else if (series.OHLC.close[i] < ma[i])
                    a[i] = -1;
            }

            ret[0]=a;
            ret[1]=b;

            return ret;
        }

        public static double[] Trix(TimeSeries series, int window)
        {
            double[] ema1 = ExpMovAvg(series.OHLC.close, window);
            double[] ema2 = ExpMovAvg(ema1, window);
            double[] ema3 = ExpMovAvg(ema2, window);

            double[] ret = new double[series.OHLC.dates.Length];

            for (int i = 1; i < series.OHLC.dates.Length; i++)
            {
                ret[i] = (ema3[i] - ema3[i - 1]) / ema3[i - 1];
            }

            return ret;
        }

        public static List<double[]> MamaFama(TimeSeries series, double slowlimit=0.05,double fastlimit=0.5)
        {
            double[] smooth = new double[series.Dates.Length];
            double[] detrender = new double[series.Dates.Length];
            double[] I1 = new double[series.Dates.Length];
            double[] I2 = new double[series.Dates.Length];
            double[] Q1 = new double[series.Dates.Length];
            double[] Q2 = new double[series.Dates.Length];
            double[] jI = new double[series.Dates.Length];
            double[] jQ = new double[series.Dates.Length];

            double[] Re = new double[series.Dates.Length];
            double[] Im = new double[series.Dates.Length];

            double[] period = new double[series.Dates.Length];
            double[] smoothperiod = new double[series.Dates.Length];
            double[] phase = new double[series.Dates.Length];
            double[] deltaphase = new double[series.Dates.Length];
            double[] alpha = new double[series.Dates.Length];

            double[] MAMA = new double[series.Dates.Length];
            double[] FAMA = new double[series.Dates.Length];
           
            double[] price=series.Prices;

            List<double[]> ret=new List<double[]>();

            for (int i = 6; i < series.Dates.Length; i++)
            {
                smooth[i] = (4 * series.Prices[i] + 3 * series.Prices[i - 1] + 2 * series.Prices[i - 2] + series.Prices[i - 3]) / 10;
                detrender[i] = (.0962 * smooth[i] + .5769 * smooth[i - 2] - .5769 * smooth[i - 4] - .0962 * smooth[i - 6]) * (.075 * period[i - 1] + .54);

                Q1[i] = (.0962 * detrender[i] + .5769 * detrender[i-2] - .5769 * detrender[i-4] - .0962 * detrender[i-6]) * (.075 * period[i-1] + .54);
                I1[i] = detrender[i-3];

                jI[i] = (.0962 * I1[i] + .5769 * I1[i-2] - .5769 * I1[i-4] -.0962 * I1[i-6]) * (.075 * period[i-1] + .54);
                jQ[i] = (.0962 * Q1[i] + .5769 * Q1[i-2] - .5769 * Q1[i-4] -.0962 * Q1[i-6]) * (.075 * period[i-1] + .54);

                I2[i] = I1[i] - jQ[i];
                Q2[i] = Q1[i] + jI[i];

                I2[i] = .2 * I2[i] + .8 * I2[i-1];
                Q2[i] = .2 * Q2[i] + .8 * Q2[i-1];

                Re[i] = I2[i] * I2[i-1] + Q2[i] * Q2[i-1];
                Im[i] = I2[i] * Q2[i-1] - Q2[i] * I2[i-1];
                Re[i] = .2 * Re[i] + .8 * Re[i-1];
                Im[i] = .2 * Im[i] + .8 * Im[i-1];

                if(Im[i]!=0 && Re[i]!=0) period[i] = 360/Math.Atan(Im[i]/Re[i]);
                if(period[i] > 1.5*period[i-1]) period[i] = 1.5*period[i-1];
                if(period[i] < .67*period[i-1]) period[i] = .67*period[i-1];
                if(period[i] < 6) period[i] = 6;
                if(period[i] > 50) period[i] = 50;
                period[i] = .2*period[i] + .8*period[i-1];
                smoothperiod[i] = .33*period[i] + .67*smoothperiod[i-1];
    
                if(I1[i]!=0) phase[i] = Math.Atan(Q1[i] / I1[i]);
                deltaphase[i] = phase[i-1] - phase[i];
                if(deltaphase[i] < 1) deltaphase[i] = 1;
                alpha[i] = fastlimit / deltaphase[i];
                if(alpha[i] < slowlimit) alpha[i] = slowlimit;

                MAMA[i] = alpha[i]*price[i] + (1 - alpha[i])*MAMA[i-1];
                FAMA[i] = .5*alpha[i]*MAMA[i] + (1 - .5*alpha[i])*FAMA[i-1];
            }

            ret.Add(MAMA);
            ret.Add(FAMA);

            return ret;
        }

        public static double[] FRAMA(TimeSeries series, int Length)
        {

            double[] N1 = new double[series.Dates.Length];
            double[] N2 = new double[series.Dates.Length];
            double[] N3 = new double[series.Dates.Length];

            double[] Dimen = new double[series.Dates.Length];
            double[] alpha = new double[series.Dates.Length];
            double[] Filt = new double[series.Dates.Length];

            double[] price = series.OHLC.close;

            int count = 0;
            double HH, LL;

            for (int i = 0; i < series.Dates.Length; i++)
            {
                count = 0;

                if (i < Length + 1)
                {
                    Filt[i] = price[i];
                    continue;
                }

                double high = UF.GetRange(series.OHLC.high, i - Length + 1, i).Max();
                double low = UF.GetRange(series.OHLC.low, i - Length + 1, i).Min();

                N3[i] = (high - low) / Length;
                HH = series.OHLC.high[i];
                LL = series.OHLC.low[i];

                while (count < Length / 2)
                {
                    if (series.OHLC.high[i - count] > HH)
                        HH = series.OHLC.high[i - count];
                    if (series.OHLC.low[i - count] < LL)
                        LL = series.OHLC.low[i - count];
                    count++;
                }

                N1[i] = (HH - LL) / (Length / 2.0);

                HH = series.OHLC.high[Length / 2];
                LL = series.OHLC.low[Length / 2];

                while (count < Length)
                {
                    if (series.OHLC.high[i - count] > HH)
                        HH = series.OHLC.high[i - count];
                    if (series.OHLC.low[i - count] < LL)
                        LL = series.OHLC.low[i - count];
                    count++;
                }

                N2[i] = (HH - LL) / (Length / 2.0);

                if (N1[i] > 0 && N2[i] > 0 && N3[i] > 0)
                    Dimen[i] = (Math.Log(N1[i] + N2[i]) - Math.Log(N3[i])) / Math.Log(2);

                alpha[i] = Math.Exp(-4.6 * (Dimen[i] - 1));
                if (alpha[i] < .01)
                    alpha[i] = .01;
                if (alpha[i] > 1)
                    alpha[i] = 1;

                Filt[i] = alpha[i] * price[i] + (1 - alpha[i]) * Filt[i - 1];
            }

            return Filt;
        }

        public static double[] TSI(TimeSeries series, int shortperiod, int longperiod)
        {
            double[] prices = series.Prices;

            double[] ema1 = ExpMovAvg(ExpMovAvg(prices.Select((x, j) => j == 0 ? 0 : x - series.Prices[j - 1]).ToArray(), shortperiod), longperiod);
            double[] ema2 = ExpMovAvg(ExpMovAvg(prices.Select((x, j) => j == 0 ? 0 : Math.Abs(x - series.Prices[j - 1])).ToArray(), shortperiod), longperiod);
            double[] ret = ema1.Select((x, j) => j == 0 ? 0 : x / ema2[j]).ToArray();

            return ret;
        }

        public static List<double[]> ErgodicInd(TimeSeries series, int shortperiod, int longperiod, int period)
        {
            List<double[]> ret = new List<double[]>();

            double[] erg = Technicals.TSI(series, shortperiod, longperiod);
            double[] ergsig = ExpMovAvg(erg, period);

            ret.Add(erg);
            ret.Add(ergsig);

            return ret;

        }

        public static double[] RollingDD(double[] prices, int LBperiod)
        {
            double[] ret = new double[prices.Length];

            for (int i = LBperiod-1; i < prices.Length; i++)
            {
                ret[i] = DrawDown(UF.GetRange(prices, i - LBperiod + 1, i)).Max();
            }

            return ret;
        }

    }
}
 