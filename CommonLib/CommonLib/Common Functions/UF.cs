using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace CommonLib
{
    #region Comments
    // Use this file as a basic lib to perform common operations
    // Make sure that there is no dependencies and this file can operate independently.
    // Once modified update in all the projects.

    // // // // List of all the functions // // // //
    /*
     * double[] Convert2Returns(double[] stock, int m) : outputs 'm' day rolling returns, takes in stock price 
     * double[] Convert2ReturnsMod(double[] stock, int m) : outputs 'm' day rolling returns (wrt to S(0)), takes in stock price
     * double ExpectedValue(double[] values) : returns the mean of an array
     * double Variance(double[] values) : returns the variance of an array
     * static double StandardDeviation(double[] values) : returns the SD of an array
     * void Ones(ref double[] vec)
     * void Ones(ref double[,] Mat) : sets the content of 1D or 2D array to 1.
     *     
    */
    #endregion
    
    public class UF
    {
        public delegate void ProgressBarReporter(int value);
        public delegate void ListBoxUpdate(string str);

        public delegate double Fun_ArrR_ArrR_R(double[] X, double[] Y);
        public static Func<double[], double[]> SqrArrDouble = x => x.Select(y => y * y).ToArray();
        public static Func<int[], int[]> SqrArrInt = x => x.Select(y => y * y).ToArray();
        public static Func<double[], double[], double[]> ProdArrDouble = ArrayProduct;

        // converts stock prices to returns {ret(t)=log(S(t)/S(t-1))}
        public static double[] Convert2Returns(double[] stock, int m)
        {
            int size, i;
            double[] Ret;

            size = stock.Length;
            Ret = new double[size - m];

            for (i = 0; i < Ret.Length; i++)
            {
                if (stock[i] > 0 && stock[i + m] > 0)
                    Ret[i] = Math.Log(stock[i + m] / stock[i]);
            }

            return Ret;
        }

        public static double[,] Convert2Returns(double[,] stock, int m)
        {
            int size, i;
            double[,] Ret;

            size = stock.GetUpperBound(0)+1;
            int cols = stock.GetUpperBound(1)+1;
            Ret = new double[size - m,cols];

            for (int j = 0; j < cols; j++)
                for (i = 0; i < size - m; i++)
                {
                    if(stock[i, j] > 0 && stock[i + m, j] >0)
                        Ret[i, j] = Math.Log(stock[i + m, j] / stock[i, j]);
                }

            return Ret;
        }

        public static double[] Convert2ReturnsRegular(double[] stock, int m)
        {
            int size, i;
            double[] Ret;

            size = stock.Length;
            Ret = new double[size - m];

            for (i = 0; i < Ret.Length; i++)
            {
                if (stock[i] != 0)
                {
                    Ret[i] = (stock[i + m] - stock[i]) / stock[i];
                }
                else
                {
                    Ret[i] = 0.0;
                }
            }

            return Ret;
        }

        public static double[,] Convert2ReturnsRegular(double[,] stock, int m)
        {           
            int rows, cols;
            rows = stock.GetUpperBound(0)+1;
            cols = stock.GetUpperBound(1)+1;
            double[,] Ret = new double[rows-m,cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j <= Ret.GetUpperBound(0); j++)
                {
                    if (stock[j,i] != 0)
                    {
                        Ret[j,i] = (stock[j + m,i] - stock[j,i]) / stock[j,i];
                    }
                    else
                    {
                        Ret[j,i] = 0.0;
                    }
                }
            }
            return Ret;
        }

        public static double[] SeriesEquiSpaced(double start, double inc, double end)
        {
            List<double> res = new List<double>();
            
            if (inc > 0 && end >= start)
            {
                while (end > start)
                {
                    res.Add(start);
                    start += inc;
                }
                return res.ToArray();
            }
            else if (inc < 0 && end <= start)
            {
                while (end < start)
                {
                    res.Add(start);
                    start += inc;
                }
                return res.ToArray();
            }
            else
            {
                return null;
            }
        }

        public static int[] SeriesEquiSpaced(int start, int inc, int end)
        {
            List<int> res = new List<int>();

            if (inc > 0 && end >= start)
            {
                while (end > start)
                {
                    res.Add(start);
                    start += inc;
                }
                return res.ToArray();
            }
            else if (inc < 0 && end <= start)
            {
                while (end < start)
                {
                    res.Add(start);
                    start += inc;
                }
                return res.ToArray();
            }
            else
            {
                return null;
            }
        }

        public static double[] Convert2SeriesRegular(double[] ret, double startVal)
        { 
            double[] stk = new double[ret.Length + 1];
            stk[0] = startVal;
            for (int i = 1; i < stk.Length; i++)
            {
                stk[i] = stk[i-1]*(1+ret[i-1]);
            }
            return stk;
        }

        public static double[] Convert2SeriesCummMTM(double[] ret, double alloc)
        {
            double[] stk = new double[ret.Length + 1];

            stk[0] = alloc;
            for (int i = 1; i < stk.Length; i++)
            {
                stk[i] = alloc * (ret[i-1]);
            }
            return CummSum(stk);
        }

        public static double[,] Convert2SeriesRegular(double[,] ret, double startVal)
        {
            int rows = ret.GetUpperBound(0) + 1;
            int cols = ret.GetUpperBound(1) + 1;
            double[,] stk = new double[rows, cols];

            for (int j = 0; j < cols; j++)
            {
                stk[0, j] = startVal;
                for (int i = 1; i < rows; i++)
                {
                    stk[i, j] = stk[i - 1, j] *(1+ret[i - 1, j]);
                }
            }
            return stk;
        }

        public static double[] Convert2Series(double[] ret, double startVal)
        {
            double[] stk = new double[ret.Length + 1];
            stk[0] = startVal;
            for (int i = 1; i < stk.Length; i++)
            {
                stk[i] = stk[i - 1] * Math.Exp(ret[i - 1]);
            }
            return stk;
        }

        public static double[,] Convert2Series(double[,] ret, double startVal)
        {
            int rows = ret.GetUpperBound(0) + 1;
            int cols = ret.GetUpperBound(1) + 1;
            double[,] stk = new double[rows,cols];

            for (int j = 0; j < cols; j++)
            {
                stk[0, j] = startVal;
                for (int i = 1; i < rows; i++)
                {
                    stk[i,j] = stk[i - 1,j] * Math.Exp(ret[i - 1,j]);
                }
            }
            return stk;
        }

        public static DateTime[] ConvertToDateTimeArr(double[] arr)
        {
            DateTime[] dates = new DateTime[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                dates[i] = DateTime.FromOADate(arr[i]);
            }

            return dates;
        }

        public static double[] ReplaceNAN(double[] inArr, double val)
        {
            double[] arr = new double[inArr.Length];
            for (int i = 0; i < inArr.Length; i++)
            {
                if (Double.IsNaN(inArr[i]))
                {
                    arr[i] = val;
                }
                else
                {
                    arr[i] = inArr[i];
                }
            }
            return arr;
        }

        public static double[] ReplaceZeroByPrev(double[] inArr)
        {
            double[] arr = new double[inArr.Length];
            for (int i = 1; i < inArr.Length; i++)
            {
                if (inArr[i]==0)
                {
                    arr[i] = arr[i-1];
                }
                else
                {
                    arr[i] = inArr[i];
                }
            }
            return arr;
        }

        public static T[] Flatten<T>(List<T[]> list)
        {
            List<T> outPut = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Length; j++)
                {
                    outPut.Add(list[i][j]);
                }
            }

            return outPut.ToArray();
        }

        public static T[] Flatten<T>(T[,] mat)
        {
            List<T> outPut = new List<T>();

            for (int i = 0; i <= mat.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= mat.GetUpperBound(1); j++)
                {
                    outPut.Add(mat[i,j]);
                }
            }

            return outPut.ToArray();
        }

        // converts stock prices to returns {ret(t)=log(S(t)/S(t-1))}
        public static double[] Convert2NonOverlappingReturns(double[] stock, int m, int skip)
        {
            int size, i;
            double[] Ret;

            size = (int)(stock.Length / m)-1;
            Ret = new double[size];
            int cnt = 0;
            for (i = skip; i < stock.Length - m; i = i + m)
            {

                Ret[cnt] = Math.Log(stock[i + m] / stock[i]);
                cnt = cnt + 1;
            }

            return Ret;
        }

        public static double[] Convert2NormalReturns(double[] stock, int m)
        {
            int size, i;
            double[] Ret;

            size = stock.Length;
            Ret = new double[size - m];

            for (i = 0; i < Ret.Length; i++)
            {
                Ret[i] = stock[i + m] - stock[i];
            }

            return Ret;
        }

        public static double[] Convert2StandardNormalReturns(double[] ret)
        {
            int size=ret.Length, i;
            double[] RetOut = new double[size];
            double Exp = ExpectedValue(ret);
            double Std = StandardDeviation(ret);
            for (i = 0; i < size; i++)
            {
                if (Std > 0)
                    RetOut[i] = (ret[i] - Exp) / Std;
            }
            return RetOut;
        }

        public static double[,] Convert2StandardNormalReturns(double[,] ret)
        {
            int i;
            int row = ret.GetUpperBound(0) + 1;
            int col = ret.GetUpperBound(1) + 1;
            double[,] RetOut = new double[row,col];
            for (int j = 0; j < col; j++)
            {
                double Exp = ExpectedValue(Get_ith_col(ret,j));
                double Std = StandardDeviation(Get_ith_col(ret, j));
                for (i = 0; i < row; i++)
                {
                    if(Std >0)
                        RetOut[i,j] = (ret[i,j] - Exp) / Std;
                }
            }
            return RetOut;
        }

        public static double[] StartIndexFromVal(double[] px, double val)
        {            
            double[] ret = Convert2ReturnsRegular(px,1);
            double[] pxOut = Convert2SeriesRegular(ret, val);

            return pxOut;
        }        

        public static int NumTrue(bool[] Arr)
        {
            int cnt = 0;
            for (int i = 0; i < Arr.Length; i++)
            {
                if (Arr[i] == true)
                    cnt++;
            }
            return cnt;
        }


        // converts stock prices to returns {ret(t)=log(S(t)/S(0))}
        public static double[] Convert2ReturnsMod(double[] stock, int m)
        {
            int size, i;
            double[] Ret;

            size = stock.Length;
            Ret = new double[size - m];

            for (i = 0; i < Ret.Length; i++)
            {
                Ret[i] = Math.Log(stock[i + m] / stock[0]);
            }

            return Ret;
        }

        // returns the time interval between dates
        public static double[] Convert2Dt(DateTime[] dates)
        {
            int i, size;
            double[] dt;
            size = dates.Length;
            dt = new double[size - 1];
            for (i = 0; i < size - 1; i++)
            {
                dt[i] = (dates[i + 1].ToOADate() - dates[i].ToOADate()) / 365.0;
                // actual dates (not business dates)                                      
            }
            return dt;
        }

        public static double ExpectedValue(double[] values)
        {
            int i;
            double sum = 0;
            for (i = 0; i < values.Length; i++)
            {
                sum=sum+values[i];

            }

            return (sum / ((double)values.Length));
        }

        public static double Median(double[] values)
        {
            double[] val = BubbleSort(values, true);
            int mid = 0;
            if (values.Length % 2 == 1) // odd
            {
                mid = (int)Math.Floor(values.Length / 2.0);
                return val[mid];
            }
            else
            {
                mid = values.Length / 2;
                return (val[mid] + val[mid - 1]) / 2.0;
            }           
        }

        public static double[] ExpectedValue(double[,] values)
        {
            int i;
            
            int rows = values.GetUpperBound(0) + 1;
            int cols = values.GetUpperBound(1) + 1;
            double[] means = new double[cols];

            for (int j = 0; j < cols; j++)
            {                
                means[j] = 0;
                for (i = 0; i < rows; i++)
                {
                    means[j]+= values[i,j];
                }
                means[j] /= cols;
            }

            return means;
        }

        public static double ExpectedValueX2(double[] values)
        {
            int i;
            double sum = 0;
            for (i = 0; i < values.Length; i++)
            {
                sum = sum + (values[i] * values[i]);
            }

            return (sum / ((double)values.Length));
        }

        public static double Variance(double[] values)
        {
            int i;
            double sum = 0;
            double mu = ExpectedValue(values); 
            for (i = 0; i < values.Length; i++)
            {
                sum = sum + Math.Pow(values[i]-mu,2);

            }

            return (sum / ((double)values.Length));
        }

        public static double[] Variance(double[,] values)
        {
            int i;            
            double[] mu = ExpectedValue(values);
            int cols = values.GetUpperBound(1) + 1;
            int rows = values.GetUpperBound(0) + 1;
            double[] var1 = new double[cols];

            for (int j = 0; j < cols; j++)
            {
                for (i = 0; i < rows; i++)
                {
                    var1[j] += Math.Pow(values[i,j] - mu[j], 2);

                }
                var1[j] /= cols;
            }

            return var1;
        }

        public static double Covariance(double[] valuesA, double[] valuesB)
        {
            int i;
            double sum = 0;
            double mu1 = ExpectedValue(valuesA);
            double mu2 = ExpectedValue(valuesB);
            for (i = 0; i < valuesA.Length; i++)
            {
                sum = sum + (valuesA[i] - mu1) * (valuesB[i] - mu2);

            }

            return (sum / ((double)valuesA.Length));
        }

        public static double Correlation(double[] valuesA, double[] valuesB)
        {
            int i;
            double covar = Covariance(valuesA, valuesB);
            double sigA, sigB;
            sigA = StandardDeviation(valuesA);
            sigB = StandardDeviation(valuesB);
            
            return covar / sigA / sigB;
        }

        public static double[] Correlation(double[] valuesA, double[] valuesB, int winLen)
        {
            double[] ret = new double[valuesA.Length];
            for (int i = winLen; i < valuesA.Length; i++)
            {
                double[] va = UF.GetRange(valuesA, i - winLen, i);
                double[] vb = UF.GetRange(valuesB, i - winLen, i);
                ret[i] = Correlation(va, vb);
            }
            return ret;
        }


        public static double[] FindATP(double[] px, double[] vol)
        {
            if (px.Length != vol.Length)
                throw new Exception("Array size mismatch");

            double prvol = 0;
            double totalvol = 0;
            double[] ATP = new double[px.Length];
            for (int i = 0; i < px.Length; i++)
            {
                prvol += px[i] * vol[i];
                totalvol += vol[i];
                if (totalvol>0)
                {
                    ATP[i] = prvol / totalvol;
                }
            }

            return ATP;
        }

        public static double Percentile(double[] data, double percentile, int numbins)
        {
            double[] edges;// = new double[numbins];
            double[] hist = Histogram(data, numbins,out edges);

            double total = SumArray(hist);
            double sum=0;
            int i;
            for (i = 0; i < edges.Length; i++)
            {
                sum+=hist[i];
                if (sum>=total*percentile)
                {
                    break;
                }
            }

            if (i < edges.Length)
                return edges[i];
            else
                return Double.NaN;
        }

        public static List<double> PercentileUpDown(double[] Data, double p)
        {
            double[] sortedData = new double[Data.Length];
            Data.CopyTo(sortedData, 0);
            Array.Sort(sortedData);
            List<double> ret = new List<double>();
            ret.Add(PercentileSorted(sortedData, p));
            ret.Add(PercentileSorted(sortedData, 1-p));

            return ret;
        }

        public static double Percentile(double[] Data, double p)
        {
            double[] sortedData = new double[Data.Length];
            Data.CopyTo(sortedData,0);
            Array.Sort(sortedData);
            return PercentileSorted(sortedData, p);
        }

        public static double PercentileSorted(double[] sortedData, double p)
        {
            if (p >= 1.0)
                return sortedData[sortedData.Length - 1];

            double position = (double)(sortedData.Length + 1) * p;
            double leftNumber = 0.0, rightNumber = 0.0;

            double n = p * (sortedData.Length - 1) + 1.0;

            if (position >= 1)
            {
                leftNumber = sortedData[(int)System.Math.Floor(n) - 1];
                rightNumber = sortedData[(int)System.Math.Floor(n)];
            }
            else
            {
                leftNumber = sortedData[0];
                rightNumber = sortedData[1];
            }

            if (leftNumber == rightNumber)
                return leftNumber;
            else
            {
                double part = n - System.Math.Floor(n);
                return leftNumber + part * (rightNumber - leftNumber);
            }
        }
        
        public static double StandardDeviation(double[] values)
        {
            return Math.Sqrt(Variance(values));
        }

        public static double[] StandardDeviation(double[,] values)
        {
            return UF.SqrtArray(Variance(values));
        }

        public static void Ones(ref double[] vec)
        {
            for (int i = 0; i < vec.Length; i++)
                vec[i] = 1.0;
        }

        public static double RSquare(double[] rStk, double[] rIdx, double[] rMkt)
        {
            double EIdx = UF.ExpectedValue(rIdx);
            double EMkt = UF.ExpectedValue(rMkt);
            double EStk = UF.ExpectedValue(rStk);
            //
            double SigIdx = UF.StandardDeviation(rIdx);
            double SigMkt = UF.StandardDeviation(rMkt);
            double SigStk = UF.StandardDeviation(rStk);
            //
            double rhoMkt = UF.Correlation(rStk, rMkt);
            double betaMkt;
            double betaIdx;
            //
            double rhoIdx = Correlation(rStk, rIdx);
            double rhoIdxMkt = Correlation(rIdx, rMkt);
            //
            double A = EMkt * EMkt + SigMkt * SigMkt;
            double B = EIdx * EIdx + SigIdx * SigIdx;
            double C = rhoMkt * SigMkt * SigStk + EStk * EMkt;
            double D = rhoIdx * SigIdx * SigStk + EStk * EIdx;
            double E = rhoIdxMkt * SigIdx * SigMkt + EIdx * EMkt;
            //
            betaMkt = (D * E - C * B) / (E * E - A * B);
            betaIdx = (C * E - D * A) / (E * E - A * B);
            //
            int n = rIdx.Length;
            //
            double sumtotal = 0, sumreg = 0, sumerr = 0, y, f;
            //
            for (int i = 0; i < n; i++)
            {
                sumtotal = sumtotal + Math.Pow(rStk[i] - EStk, 2.0); 
                sumreg = sumreg + Math.Pow(betaIdx * (rIdx[i] - EIdx) + betaMkt * (rMkt[i] - EMkt), 2.0);
                y = rStk[i];
                f = betaIdx * rIdx[i] + betaMkt * rMkt[i];
                sumerr = sumerr + Math.Pow(y - f, 2.0);
            }
            double rsq = 1.0 - (sumerr / sumtotal);
            return rsq;
        }

        public static double EucledianDist(double[] X, double[] Y)
        {
            double d = 0;
            for (int i = 0; i < X.Length; i++)
            {
                d += Math.Pow(X[i] - Y[i],2);
            }
            d = Math.Sqrt(d);
            return d;
        }

        public static void Ones(ref double[,] Mat)
        {
            for (int i = 0; i <= Mat.GetUpperBound(0); i++)
                for (int j = 0; j <= Mat.GetUpperBound(1);j++ )
                    Mat[i,j] = 1.0;
        }

        public static double[] Ones(int n)
        {
            double[] vec = new double[n];
            for (int i = 0; i < n; i++)
                vec[i] = 1.0;
            return vec;
        }
        
        public static int[] Double2Int(double[] input)
        {
            int[] vec = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                vec[i] = (int)input[i];
            return vec;
        }

        public static double[] Int2Double(int[] input)
        {
            double[] vec = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                vec[i] = (double)input[i];
            return vec;
        }

        

        public static double[,] Ones(int n, int m)
        {
            double[,] Mat = new double[n,m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    Mat[i, j] = 1.0;
            return Mat;
        }

        public static T[,] ListArray2Mat<T>(List<T[]> Arr)
        {
            int cols = Arr.Count;
            int rows = Arr[0].Length;

            T[,] mat = new T[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                UF.Set_ith_col<T>(ref mat, Arr[i], i);
            }

            return mat;
        }

        public static double[,] CorrMatrix(double[,] data)
        {
            // gives a nCols x nCols matrix
            int nCols = data.GetUpperBound(1) + 1;
            int nRows = data.GetUpperBound(0) + 1;

            double[,] res = new double[nCols,nCols];

            for (int i = 0; i < nCols; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                    {
                        res[i, j] = 1.0;
                    }
                    else
                    {
                        res[i, j] = res[j, i] = Correlation(UF.Get_ith_col(data, i), UF.Get_ith_col(data, j));
                    }
                }
            }
            return res;
        }

        public static double[,] CrossCorrMatrix(double[,] X, double[,] Y)
        {
            // gives a nCols x nCols matrix
            int nColX = X.GetUpperBound(1) + 1;
            int nColY = Y.GetUpperBound(1) + 1;
            int nRowX = X.GetUpperBound(0) + 1;
            int nRowY = Y.GetUpperBound(0) + 1;

            if (nRowX != nRowY)
            {
                throw new Exception("Matrix dimentions don't match");
            }
            
            double[,] res = new double[nColX, nColY];

            for (int i = 0; i < nColX; i++)
            {
                for (int j = 0; j < nColY; j++)
                {                   
                    res[i, j] = Correlation(UF.Get_ith_col(X, i), UF.Get_ith_col(Y, j));                    
                }
            }
            return res;
        }

        public static double[] GetDiagonal(double[,] X)
        {
            // gives a nCols x nCols matrix
            int nColX = X.GetUpperBound(1) + 1;            
            int nRowX = X.GetUpperBound(0) + 1;

            double[] res = new double[Math.Min(nRowX, nColX)];

            for (int i = 0; i < Math.Min(nRowX,nColX); i++)
            {
                res[i] = X[i,i];
            }
            return res;
        }

        //copy contents of one array to another
        public static void Copy1DArrayL2R(double[] left, ref double[] right)
        {
            if (left == null)
                return;

            right = new double[left.Length];
            
            for (int i = 0; i < left.Length; i++)
            {
                right[i] = left[i];
            }
            
        }

        public static void Copy1DArrayL2R(int[] left, ref int[] right)
        {
            if (left == null)
                return;
            right = new int[left.Length];
            
            for (int i = 0; i < left.Length; i++)
            {
                right[i] = left[i];
            }
            
        }

        public static double[] Copy(double[] source)
        {
            double[] dest = new double[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = source[i];
            }
            return dest;
        }

        public static int[] Copy(int[] source)
        {
            int[] dest = new int[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = source[i];
            }
            return dest;
        }

        public static string[] Copy(string[] source)
        {
            string[] dest = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = source[i];
            }
            return dest;
        }

        public static T[] Copy<T>(T[] source)// where T: ICloneable
        {
            T[] dest = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = source[i];
            }
            return dest;
        }

        public static T[,] Copy<T>(T[,] source)// where T: ICloneable
        {
            T[,] dest = new T[source.GetUpperBound(0) + 1, source.GetUpperBound(1) + 1];
            for (int i = 0; i < source.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < source.GetUpperBound(1) + 1; j++)
                {
                    dest[i, j] = source[i, j];
                }
            }
            return dest;
        }

        //copy contents of one array to another
        public static void Copy1DArrayL2RDates(DateTime[] left, ref DateTime[] right)
        {
            if (left == null)
                return;
            right = new DateTime[left.Length];
            
            for (int i = 0; i < left.Length; i++)
            {
                right[i] = left[i];
            }
            
        }

        //copy contents of one array to another
        public static void Copy1DArrayL2R(string[] left, ref string[] right)
        {
            if (left == null)
                return;
            right = new string[left.Length];
            
            for (int i = 0; i < left.Length; i++)
            {
                right[i] = left[i];
            }
            
        }
               

        // copy contents of matrix to another
        public static void Copy2DArrayL2R(double[,] left, ref double[,] right)
        {
            if (left == null)
                return;

            right = new double[left.GetUpperBound(0) + 1, left.GetUpperBound(1) + 1];
            
            for (int i = 0; i <= left.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= left.GetUpperBound(1); j++)
                {
                    right[i, j] = left[i, j];
                }
            }
            
        }
        
        // Get ith row from the matrix
        public static double[] Get_ith_row(double[,] Mat, int i)
        {
            double[] tmp = new double[Mat.GetUpperBound(1) + 1];

            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                tmp[j] = Mat[i, j];
            }

            return tmp;
        }

        public static T[] Get_ith_row<T>(T[,] Mat, int i)
        {
            T[] tmp = new T[Mat.GetUpperBound(1) + 1];

            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                tmp[j] = Mat[i, j];
            }

            return tmp;
        }

        // Get ith row from the matrix
        public static int[] Get_ith_row(int[,] Mat, int i)
        {
            int[] tmp = new int[Mat.GetUpperBound(1) + 1];

            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                tmp[j] = Mat[i, j];
            }

            return tmp;
        }

        // Get ith column from a matrix
        public static double[] Get_ith_col(double[,] Mat, int i)
        {
            double[] tmp = new double[Mat.GetUpperBound(0) + 1];

            for (int j = 0; j <= Mat.GetUpperBound(0); j++)
            {
                tmp[j] = Mat[j, i];
            }

            return tmp;
        }

        // Get ith column from a matrix
        public static T[] Get_ith_col<T>(T[,] Mat, int i)
        {
            T[] tmp = new T[Mat.GetUpperBound(0) + 1];

            for (int j = 0; j <= Mat.GetUpperBound(0); j++)
            {
                tmp[j] = Mat[j, i];
            }

            return tmp;
        }

        // Set ith row of the matrix
        public static void Set_ith_row(ref double[,] Mat, double[] setRow, int i)
        {
            if ((Mat.GetUpperBound(1) + 1) != setRow.Length)
                throw new Exception("Matrix dimensions don't match");
            
            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                Mat[i, j]=setRow[j];
            }
        }
        // Set ith row of the matrix
        public static void Set_ith_row(ref int[,] Mat, int[] setRow, int i)
        {
            if ((Mat.GetUpperBound(1) + 1) != setRow.Length)
                throw new Exception("Matrix dimensions don't match");

            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                Mat[i, j] = setRow[j];
            }
        }

        public static void Set_ith_row<T>(ref T[,] Mat, T[] setRow, int i)
        {
            if ((Mat.GetUpperBound(1) + 1) != setRow.Length)
                throw new Exception("Matrix dimensions don't match");

            for (int j = 0; j <= Mat.GetUpperBound(1); j++)
            {
                Mat[i, j] = setRow[j];
            }
        }

        // Set ith row of the matrix
        public static void Set_ith_col(ref double[,] Mat, double[] setCol, int i)
        {
            if ((Mat.GetUpperBound(0) + 1) != setCol.Length)
                throw new Exception("Matrix dimensions don't match");

            for (int j = 0; j <= Mat.GetUpperBound(0); j++)
            {
                Mat[j, i] = setCol[j];
            }
        }

        // Set ith row of the matrix
        public static void Set_ith_col(ref int[,] Mat, int[] setCol, int i)
        {
            if ((Mat.GetUpperBound(0) + 1) != setCol.Length)
                throw new Exception("Matrix dimensions don't match");

            for (int j = 0; j <= Mat.GetUpperBound(0); j++)
            {
                Mat[j, i] = setCol[j];
            }
        }

        public static void Set_ith_col<T>(ref T[,] Mat, T[] setCol, int i)
        {
            if ((Mat.GetUpperBound(0) + 1) != setCol.Length)
                throw new Exception("Matrix dimensions don't match");

            for (int j = 0; j <= Mat.GetUpperBound(0); j++)
            {
                Mat[j, i] = setCol[j];
            }
        }

        // Generate 'm' random numbers Eg: rand(6) = [2, 4, 3, 5, 1, 0]
        public static int[] randperm(int m)
        {
            int[] index = new int[m];
            int tempi;
            double[] randArray = new double[m];
            double tempd;
            Random rand = new Random();


            for (int i = 0; i < m; i++)
            {
                randArray[i] = rand.NextDouble();
                index[i] = i;
            }

            // sort the numbers in randarray, also modify index
            for (int i = 0; i < m - 1; i++)
                for (int j = 0; j < m - i - 1; j++)
                    if (randArray[j + 1] < randArray[j])
                    {
                        tempd = randArray[j];
                        randArray[j] = randArray[j + 1];
                        randArray[j + 1] = tempd;
                        tempi = index[j];
                        index[j] = index[j + 1];
                        index[j + 1] = tempi;
                    }
            return index;
        }

       
        // returns transpose of a matrix
        public static double[,] Transpose(double[,] Mat)
        {
            double[,] result = new double[Mat.GetUpperBound(1) + 1, Mat.GetUpperBound(0) + 1];
            for (int i = 0; i <= Mat.GetUpperBound(0); i++)
                for (int j = 0; j <= Mat.GetUpperBound(1); j++)
                    result[j, i] = Mat[i, j];

            return result;
        }

        // perform BubbleSort on an array
        public static double[] BubbleSort(double[] Unsorted, bool assending)
        {
            double[] Sorted = new double[Unsorted.Length];
            Copy1DArrayL2R(Unsorted, ref Sorted);
            double temp;
            if (assending == true)
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] < Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                        }
                    }
                }
            }
            else
            {

                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] > Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                        }
                    }
                }
            }

            return Sorted;
        }

        // perform BubbleSort on an array
        public static int[] BubbleSortIdx(double[] Unsorted, bool assending)
        {
            int[] idx = new int[Unsorted.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;
            double[] Sorted = new double[Unsorted.Length];
            Copy1DArrayL2R(Unsorted, ref Sorted);
            double temp;
            int temp1;
            if (assending == true)
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] < Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;

                        }
                    }
                }
            }
            else
            {

                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] > Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;
                        }
                    }
                }
            }

            return idx;
        }

        public static double[] RemoveZeros(double[] A)
        {
            List<double> B = new List<double>();
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != 0)
                    B.Add(A[i]);
            }

            return B.ToArray();
        }

        public static int[] BubbleSortIdx(string[] Unsorted, bool assending)
        {
            int[] idx = new int[Unsorted.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;
            string[] Sorted = new string[Unsorted.Length];
            Copy1DArrayL2R(Unsorted, ref Sorted);
            string temp;
            int temp1;
            if (assending == true)
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if ( string.Compare(Sorted[j],Sorted[j + 1])>0)
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;

                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (string.Compare(Sorted[j], Sorted[j + 1]) < 0)
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;
                        }
                    }
                }
            }

            return idx;
        }

        public static int[] BubbleSortIdx(DateTime[] Unsorted, bool assending)
        {
            int[] idx = new int[Unsorted.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;
            DateTime[] Sorted = new DateTime[Unsorted.Length];
            Copy1DArrayL2RDates(Unsorted, ref Sorted);
            DateTime temp;
            int temp1;
            if (assending == true)
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] < Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;

                        }
                    }
                }
            }
            else
            {

                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] > Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;
                        }
                    }
                }
            }

            return idx;
        }

        public static int[] BubbleSortIdx(int[] Unsorted, bool assending)
        {
            int[] idx = new int[Unsorted.Length];
            for (int i = 0; i < idx.Length; i++)
                idx[i] = i;
            int[] Sorted = new int[Unsorted.Length];
            Copy1DArrayL2R(Unsorted, ref Sorted);
            int temp;
            int temp1;
            if (assending == true)
            {
                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] < Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;

                        }
                    }
                }
            }
            else
            {

                for (int i = 0; i < Sorted.Length - 1; i++)
                {
                    for (int j = 0; j < Sorted.Length - 1 - i; j++)
                    {
                        if (Sorted[j + 1] > Sorted[j])
                        {
                            temp = Sorted[j];
                            Sorted[j] = Sorted[j + 1];
                            Sorted[j + 1] = temp;
                            temp1 = idx[j];
                            idx[j] = idx[j + 1];
                            idx[j + 1] = temp1;
                        }
                    }
                }
            }

            return idx;
        }

        public static double SharpeRatio(double[] pxs, double marketReturn)
        {
            double[] ret = Convert2Returns(pxs, 1);
            double E = ExpectedValue(ret) * 250.0;
            double SD = StandardDeviation(ret) * Math.Sqrt(250.0);
            return ((E - (marketReturn)) / SD);
        }

        public static double StrikeRate(Trade[] trds)
        {
            double stkRate = 0;
            if (trds != null)
            {
                for (int i = 0; i < trds.Length; i++)
                {
                    if (trds[i].Return >= 0)
                        stkRate += 1;
                }
                return stkRate / trds.Length;
            }

            return stkRate;
        }

        public static int[] RankValues(int[] indexes)
        {
            int[] Ranks = new int[indexes.Length];
            for (int j = 0; j < Ranks.Length; j++)
                Ranks[indexes[j]] = j;

            return Ranks;
        }

        // Sort each column using Bubble Sort
        public static double[,] BubbleSort(double[,] Unsorted, bool assending)
        {
            double[,] Sorted = new double[Unsorted.GetUpperBound(0) + 1, Unsorted.GetUpperBound(1) + 1];
            Copy2DArrayL2R(Unsorted, ref Sorted);
            double temp;

            if (assending == true)
            {
                for (int k = 0; k <= Sorted.GetUpperBound(1); k++)
                {
                    for (int i = 0; i < Sorted.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < Sorted.GetUpperBound(0) - i; j++)
                        {
                            if (Sorted[j + 1, k] < Sorted[j, k])
                            {
                                temp = Sorted[j, k];
                                Sorted[j, k] = Sorted[j + 1, k];
                                Sorted[j + 1, k] = temp;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int k = 0; k <= Sorted.GetUpperBound(1); k++)
                {
                    for (int i = 0; i < Sorted.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < Sorted.GetUpperBound(0) - i; j++)
                        {
                            if (Sorted[j + 1, k] > Sorted[j, k])
                            {
                                temp = Sorted[j, k];
                                Sorted[j, k] = Sorted[j + 1, k];
                                Sorted[j + 1, k] = temp;
                            }
                        }
                    }
                }
            }

            return Sorted;
        }

        // Convert a double array into a CSV string
        public static string DoubleArray2String(double[] arrayIn)
        {
            string output = arrayIn[0].ToString();
            for (int i = 1; i < arrayIn.Length; i++)
            {
                output = output + "," + arrayIn[i].ToString();
            }

            return output;

        }

        public static string ToCSVString<T>(T[] arrayIn)
        {
            string output = arrayIn[0].ToString().Replace(",", "");
            for (int i = 1; i < arrayIn.Length; i++)
            {
                output = output + "," + arrayIn[i].ToString().Replace(",","");
            }
            return output;
        }

        public static List<T[]> Col2Rows<T>(List<T[]> Cols)
        {
            List<T[]> output = new List<T[]>();
            
            int[] cnt = Cols.Select(x => x.Length).ToArray();

            if (cnt.Min() == 0)
                throw new Exception("One of the array has zero elements");
            
            for (int j = 0; j < cnt.Max(); j++)
            {
                T[] row = Cols.Select((x, i) => j < cnt[i] ? x[j] : x[cnt[i]-1]).ToArray();
                output.Add(row);
            }
         
            return output;
        }

        public static DateTime[] DoubleArray2DateTimeArr(double[] arrayIn)
        {
            DateTime[] output = new DateTime[arrayIn.Length];
            for (int i = 0; i < arrayIn.Length; i++)
            {
                output[i] = DateTime.FromOADate(arrayIn[i]);
            }
            return output;
        }

        public static string NumArray2String<T>(T[] arrayIn)
        {
            string output = arrayIn[0].ToString();
            for (int i = 1; i < arrayIn.Length; i++)
            {
                output = output + "," + arrayIn[i].ToString();
            }

            return output;

        }

        public static string IntArray2String(int[] arrayIn)
        {
            string output = arrayIn[0].ToString();
            for (int i = 1; i < arrayIn.Length; i++)
            {
                output = output + ", " + arrayIn[i].ToString();
            }

            return output;

        }

        // Convert a double array to string array
        public static string[] DoubleArray2StringArray(double[] arrayIn)
        {
            string[] output = new string[arrayIn.Length];
            for (int i = 0; i < arrayIn.Length; i++)
            {
                output[i] = arrayIn[i].ToString();
            }

            return output;
        }
        
        public static string StringArray2CSVString(string[] arrayIn)
        {
            string output = "";
            output = arrayIn[0];
            if (arrayIn.Length > 1)
            {
                for (int i = 1; i < arrayIn.Length; i++)
                {
                    output += ","+ arrayIn[i];
                }
            }

            return output;
        }
        // Convert a int array to string array
        public static string[] IntArray2StringArray(int[] arrayIn)
        {
            string[] output = new string[arrayIn.Length];
            for (int i = 0; i < arrayIn.Length; i++)
            {
                output[i] = arrayIn[i].ToString();
            }
            return output;
        }
        
        public static DateTime[] StringArray2DateTimeArray(string[] arrayIn)
        {
            DateTime[] output = new DateTime[arrayIn.Length];
            for (int i = 0; i < arrayIn.Length; i++)
            {
                output[i] = DateTime.Parse(arrayIn[i]);
            }

            return output;
        }

        public static int[,] IdentityI(int n)
        {
            int[,] output = new int[n,n];

            for (int i = 0; i < n; i++)
            {
                output[i, i] = 1;
            }

            return output;
        }

        public static double[,] IdentityD(int n)
        {
            double[,] output = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                output[i, i] = 1.0;
            }

            return output;
        }

        public static string[] Unique(string[] Input)
        {
            List<string> strArr = new List<string>();

            int n = Input.Length;

            for (int i = 0; i < n; i++)
            {
                if (!strArr.Contains(Input[i]))
                {
                    strArr.Add(Input[i]);
                }
            }
            string[] outPut = new string[strArr.Count];
            strArr.CopyTo(outPut);

            return outPut;
        }

        public static double[] Unique(double[] Input)
        {
            List<double> strArr = new List<double>();

            int n = Input.Length;

            for (int i = 0; i < n; i++)
            {
                if (!strArr.Contains(Input[i]))
                {
                    strArr.Add(Input[i]);
                }
            }
            double[] outPut = new double[strArr.Count];
            strArr.CopyTo(outPut);

            return outPut;
        }

        public static int[] Unique(int[] Input)
        {
            List<int> strArr = new List<int>();

            int n = Input.Length;

            for (int i = 0; i < n; i++)
            {
                if (!strArr.Contains(Input[i]))
                {
                    strArr.Add(Input[i]);
                }
            }
            int[] outPut = new int[strArr.Count];
            strArr.CopyTo(outPut);

            return outPut;
        }
        
        public static DateTime[] Unique(DateTime[] Input)
        {
            List<DateTime> strArr = new List<DateTime>();

            int n = Input.Length;

            for (int i = 0; i < n; i++)
            {
                if (!strArr.Contains(Input[i]))
                {
                    strArr.Add(Input[i]);
                }
            }
            DateTime[] outPut = new DateTime[strArr.Count];
            strArr.CopyTo(outPut);

            return outPut;
        }

        // Perform matrix multiplication
        public static double[,] MatrixMul(double[,] A, double[,] B)
        {
            if (A.GetUpperBound(1) != B.GetUpperBound(0))
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] C = new double[A.GetUpperBound(0) + 1, B.GetUpperBound(1) + 1];
            for (int i = 0; i <= C.GetUpperBound(0); i++)
                for (int j = 0; j <= C.GetUpperBound(1); j++)
                    for (int k = 0; k <= A.GetUpperBound(1); k++)
                        C[i, j] = C[i, j] + A[i, k] * B[k, j];

            return C;
        }

        public static double[,] MatrixMulByConst(double[,] A, double B)
        {            
            double[,] C = new double[A.GetUpperBound(0) + 1, A.GetUpperBound(1) + 1];
            for (int i = 0; i <= C.GetUpperBound(0); i++)
                for (int j = 0; j <= C.GetUpperBound(1); j++)
                    C[i, j] = A[i, j] * B;

            return C;
        }

        // Performs matrix addition
        public static double[,] MatrixAdd(double[,] A, double[,] B,bool add)
        {
            if (A.GetUpperBound(0) != B.GetUpperBound(0) || A.GetUpperBound(1) != B.GetUpperBound(1))
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] C = new double[A.GetUpperBound(0) + 1, A.GetUpperBound(1) + 1];
            if (add == true)
            {
                for (int i = 0; i <= C.GetUpperBound(0); i++)
                    for (int j = 0; j <= C.GetUpperBound(1); j++)
                        C[i, j] = A[i, j] + B[i, j];
            }
            else
            {
                for (int i = 0; i <= C.GetUpperBound(0); i++)
                    for (int j = 0; j <= C.GetUpperBound(1); j++)
                        C[i, j] = A[i, j] - B[i, j];
            }
            return C;
        }

        public static double[] MatrixSum1D(double[,] A, bool row)
        {
            double[] C;
            if (row)
                C = new double[A.GetUpperBound(0) + 1];
            else
                C = new double[A.GetUpperBound(1) + 1];

            if (row)
            {
                for (int i = 0; i < C.Length; i++)
                    C[i] = UF.SumArray(UF.Get_ith_row(A, i));
            }
            else
            {
                for (int i = 0; i < C.Length; i++)
                    C[i] = UF.SumArray(UF.Get_ith_col(A, i));
            }
            return C;
        }
        
              
        // Performs dot product
        public static double[] ArrayProduct(double[] A, double[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            double[] C = new double[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] * B[i];
            }

            return C;

        }
        

        public static double[] ArrayDiv(double[] A, double[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            double[] C = new double[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] / B[i];
            }

            return C;

        }

        public static double[] ArrayReciprocal(double[] A)
        {
            double[] C = new double[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                if (A[i] != 0.0)
                    C[i] = 1.0 / A[i];
                else C[i] = 1.0;// throw new InvalidOperationException("Error: array element is zero");
            }           

            return C;

        }

        public static double DotProduct(double[] A, double[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            double C = 0;

            for (int i = 0; i < A.Length; i++)
            {
                C = C + A[i] * B[i];
            }

            return C;

        }
        public static double SumArray(double[] A)
        {
            
            double C = 0;

            for (int i = 0; i < A.Length; i++)
            {
                C = C + A[i];
            }

            return C;

        }

        public static int SumArray(int[] A)
        {

            int C = 0;

            for (int i = 0; i < A.Length; i++)
            {
                C = C + A[i];
            }

            return C;

        }

        public static double[] MulArrayByConst(double[] A,double b)
        {

            double[] C = new double[A.Length];

            for (int i = 0; i < A.Length; i++)
            {
                C[i] = A[i]*b;
            }

            return C;

        }

        public static double[]LogArray(double[] A)
        {
            double[] C = new double[A.Length];

            for (int i = 0; i < A.Length; i++)
            {
                C[i] = Math.Log(A[i]);
            }

            return C;
        }

        public static double[] SqrtArray(double[] A)
        {
            double[] C = new double[A.Length];

            for (int i = 0; i < A.Length; i++)
            {
                C[i] = Math.Sqrt(A[i]);
            }

            return C;
        }

        public static double[] LogArrayNormalized(double[] A)
        {
            double[] C = new double[A.Length];

            for (int i = 0; i < A.Length; i++)
            {
                C[i] = Math.Log(A[i]/A[0]);
            }

            return C;
        }

        public static double[,] LogArrayNormalized(double[,] A)
        {
            int rows = A.GetUpperBound(0) + 1;
            int cols = A.GetUpperBound(1) + 1;
            double[,] C = new double[rows,cols];

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    if(A[0,j] > 0)
                        C[i,j] = Math.Log(A[i,j] / A[0,j]);
                }
            }

            return C;
        }


        public static double[] ArrayAdd(double[] A, double[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            double[] C = new double[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] + B[i];
            }

            return C;
        }
        public static int[] ArrayAdd(int[] A, int[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            int[] C = new int[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] + B[i];
            }

            return C;

        }

        public static double[] ArrayPower(double[] A, double pow)
        {
            return A.Select(x => Math.Pow(x, pow)).ToArray();            
        }

        public static double[] Append(double[] A, double b, bool end)
        {
            double[] C = new double[A.Length + 1];
            int i;
            if (end == false)
            {
                C[0] = b;
                for (i = 0; i < A.Length; i++)
                    C[i + 1] = A[i];

            }
            else
            {
                for (i = 0; i < A.Length; i++)
                    C[i] = A[i];
                C[A.Length] = b;
            }

            return C;
        }

        public static string[] Append(string[] A, string b, bool end)
        {
            string[] C = new string[A.Length + 1];
            int i;
            if (end == false)
            {
                C[0] = b;
                for (i = 0; i < A.Length; i++)
                    C[i + 1] = A[i];

            }
            else
            {
                for (i = 0; i < A.Length; i++)
                    C[i] = A[i];
                C[A.Length] = b;
            }

            return C;
        }

        public static double[] AppendArray(double[] A, double[] B)
        {
            int i, n = A.Length + B.Length;
            double[] C = new double[n];
                                    
            for (i = 0; i < A.Length; i++)
                C[i] = A[i];

            for (i = A.Length; i < n; i++)
                C[i] = B[i - A.Length];

            return C;
        }

        public static T[] AppendArray<T>(T[] A, T[] B)
        {
            if (A == null)
                return B;
            else if (B == null)
                return A;

            int i, n = A.Length + B.Length;
            T[] C = new T[n];

            for (i = 0; i < A.Length; i++)
                C[i] = A[i];

            for (i = A.Length; i < n; i++)
                C[i] = B[i - A.Length];

            return C;
        }

        public static int[] AppendArray(int[] A, int[] B)
        {
            int i, n = A.Length + B.Length; ;
            int[] C = new int[n];
            

            for (i = 0; i < A.Length; i++)
                C[i] = A[i];

            for (i = A.Length; i < n; i++)
                C[i] = B[i - A.Length];

            return C;
        }

        public static double[,] AppendMat(double[,] A, double[,] B, bool rowsApp)
        {            
            int rA = A.GetUpperBound(0) + 1;
            int rB = B.GetUpperBound(0) + 1;
            int cA = A.GetUpperBound(1) + 1;
            int cB = B.GetUpperBound(1) + 1;

            int row = 0, col = 0;

            if (rowsApp)
            {
                row = rA + rB;
                col = cA;
                if (cA != cB)
                {
                    throw new Exception("Size doesn't match");
                }
            }
            else
            {
                row = rA;
                col = cA + cB;
                if (rA != rB)
                {
                    throw new Exception("Size doesn't match");
                }
            }

            double[,] C = new double[row, col];

            if (rowsApp)
            {
                for (int i = 0; i < rA; i++)
                    for (int j = 0; j < cA; j++)
                        C[i, j] = A[i, j];

                for (int i = rA; i < row; i++)
                    for (int j = 0; j < col; j++)
                        C[i, j] = B[i-rA, j];

            }
            else
            {
                for (int i = 0; i < rA; i++)
                    for (int j = 0; j < cA; j++)
                        C[i, j] = A[i, j];

                for (int i = 0; i < row; i++)
                    for (int j = cA; j < col; j++)
                        C[i, j] = B[i, j-cA];
            }

            return C;
        }

        public static double MaxArray(double[] A)
        {
            double max = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] > max) 
                    max = A[i];
            }
            return max;
        }

        public static int MaxArrayIdx(double[] A)
        {
            int max = 0;
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] > A[max]) 
                    max = i;
            }
            return max;
        }

        public static int MaxArray(int[] A)
        {
            int max = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] > max) max = A[i];
            }
            return max;
        }


        public static DateTime MaxArray(DateTime[] A)
        {
            DateTime max = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] > max) max = A[i];
            }
            return max;
        }

        public static double MinArray(double[] A)
        {
            double min = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] < min) min = A[i];
            }
            return min;
        }

        public static int MinArray(int[] A)
        {
            int min = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] < min) min = A[i];
            }
            return min;
        }

        public static double MeasureOfVariation(double[] X, double[] Y, Fun_ArrR_ArrR_R fun, int numSplits)
        {
            double[] resloop = new double[numSplits];

            if (X.Length != Y.Length || X.Length < numSplits)
            {
                throw new Exception("Array size doesn't match");
            }

            int numSize = (int)Math.Round( (double)X.Length / numSplits);
            

            for (int i = 0; i < numSplits; i++)
            {
                if (i < numSplits - 1)
                {
                    double[] x = UF.GetRange<double>(X, i * numSize, (i + 1) * numSize);
                    double[] y = UF.GetRange<double>(Y, i * numSize, (i + 1) * numSize);
                    resloop[i] = fun(x, y);
                }
                else
                {
                    double[] x = UF.GetRange<double>(X, i * numSize, X.Length-1);
                    double[] y = UF.GetRange<double>(Y, i * numSize, Y.Length-1);
                    resloop[i] = fun(x, y);
                }
            }

            return StandardDeviation(resloop);
        }

        public static double[,] MeasureOfVarOfCorrMat(double[,] X, int numSplits)
        {
            int rows = X.GetUpperBound(0)+1;
            int cols = X.GetUpperBound(1)+1;
            double[,] res = new double[cols, cols];
            for (int i = 0; i < cols; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    res[i, j] = res[j,i]= MeasureOfVariation(UF.Get_ith_col(X, i), UF.Get_ith_col(X, j)
                        , UF.Correlation, numSplits);
                }
            }

            return res;
        }

        public static DateTime MinArray(DateTime[] A)
        {
            DateTime min = A[0];
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] < min) min = A[i];
            }
            return min;
        }

        public static int MinArrayIdx(double[] A)
        {
            int min = 0;
            int i, n = A.Length;
            for (i = 0; i < n; i++)
            {
                if (A[i] < A[min]) min = i;
            }
            return min;
        }

        public static double[] CummSum(double[] A)
        {
            double[] C = new double[A.Length];
            C[0] = A[0];
            for (int i = 1; i < A.Length; i++)
            {
                C[i] = C[i - 1] + A[i];
            }
            return C;
        }

        public static double[] CummAvg(double[] A)
        {
            double[] C = new double[A.Length];
            C[0] = A[0];
            double sum = A[0];
            for (int i = 1; i < A.Length; i++)
            {
                sum += A[i];
                C[i] = sum/(i+1);
            }
            return C;
        }

        public static double[] MTM2NAV(double[] MTM, double alloc)
        {
            double[] retArr = MulArrayByConst(MTM, 1.0 / alloc);
            return Convert2SeriesRegular(retArr, alloc);
        }

        public static double[] AddConst2Array(double[] A, double b)
        {
            double[] C = new double[A.Length];

            for (int i = 0; i < A.Length; i++)
                C[i] = A[i] + b;

            return C;
        }

        
        //
        public static double[] Histc(double[] X, double[] Edges,ref int[] Bins)
        {
            int L=Edges.Length;
            double[] NumC = new double[L];
            if (X.Length != Bins.Length)
            {
                throw new InvalidOperationException("Error: array dimensions don't match");
            }
            for (int i = 0; i < X.Length; i++)
            {
                for (int j = 0; j < L - 1; j++)
                {
                    if (X[i] >= Edges[j] && X[i] < Edges[j + 1])
                    {
                        NumC[j] = NumC[j] + 1;
                        Bins[i] = j;
                        break;
                    }
                    if (X[i] == Edges[L - 1])
                    {
                        NumC[L-1] = NumC[L-1] + 1;
                        Bins[i] = L-1;
                        break;
                    }
                }
            }

            return NumC;
        }

        public static double[] HistExcel(double[] X, double[] Edges)
        {
            int L = Edges.Length;
            double[] NumC = new double[L];
            
            for (int i = 0; i < X.Length; i++)
            {
                for (int j = 1; j < L; j++)
                {
                    if (X[i] <= Edges[j] && X[i] > Edges[j - 1])
                    {
                        NumC[j] = NumC[j] + 1;                       
                        break;
                    }
                    if (X[i] <= Edges[0])
                    {
                        NumC[0] = NumC[0] + 1;                       
                        break;
                    }
                }
            }
            return NumC;
        }

        public static double[] Histogram(double[] X, int numBins, out double[] Edges)
        {
            double min = UF.MinArray(X);
            double max = UF.MaxArray(X);

            double range = max - min;
            if (numBins < 2)
                throw new Exception("Number of bins should be greater than 2");
            double dx = range / (numBins-1);

            Edges = new double[numBins];
            Edges[0] = min;
            for (int i = 1; i < numBins; i++)
            {
                Edges[i] = Edges[i - 1] + dx;
            }

            return HistExcel(X, Edges); 
        }

        public static int[] Sign(double[] source)
        {
            int n = source.Length;
            int[] C = new int[n];

            for (int i = 0; i < n; i++)
            {
                if (source[i] < 0)
                    C[i] = -1;
                else
                    C[i] = 1;
            }

            return C;
        }

        // does A-B
        public static double[] ArraySub(double[] A, double[] B)
        {
            if (A.Length != B.Length)
            {
                throw new InvalidOperationException("Error: array dimentions don't match");
            }
            double[] C = new double[A.Length];

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] - B[i];
            }

            return C;
        }

        public static List<object[]> Mingle<T, G>(List<T[]> first, List<G[]> second
            )
        {
            List<object[]> ret = new List<object[]>();
            for (int i = 0; i < first.Count; i++)
            {
                ret.Add(first[i].Select(x => (object)x).ToArray());
                ret.Add(second[i].Select(x => (object)x).ToArray());                
            }
            return ret;
        }

        public static List<object[]> Mingle<T,G,H>(List<T[]> first, List<G[]> second,
            List<H[]> third)             
        {
            List<object[]> ret = new List<object[]>();
            for (int i = 0; i < first.Count; i++)
            {
                ret.Add(first[i].Select(x => (object)x).ToArray());
                ret.Add(second[i].Select(x => (object)x).ToArray());
                ret.Add(third[i].Select(x => (object)x).ToArray());                
            }
            return ret;
        }

        public static List<object[]> Mingle<T,G,H,I>(List<T[]> first, List<G[]> second,
            List<H[]> third, List<I[]> fourth)             
        {
            List<object[]> ret = new List<object[]>();
            for (int i = 0; i < first.Count; i++)
            {
                ret.Add(first[i].Select(x => (object)x).ToArray());
                ret.Add(second[i].Select(x => (object)x).ToArray());
                ret.Add(third[i].Select(x => (object)x).ToArray());
                ret.Add(fourth[i].Select(x => (object)x).ToArray());                
            }
            return ret;
        }

        public static List<object[]> Mingle<T,G,H,I,J>(List<T[]> first, List<G[]> second,
            List<H[]> third, List<I[]> fourth, List<J[]> fifth)             
        {
            List<object[]> ret = new List<object[]>();
            for (int i = 0; i < first.Count; i++)
            {
                ret.Add(first[i].Select(x => (object)x).ToArray());
                ret.Add(second[i].Select(x => (object)x).ToArray());
                ret.Add(third[i].Select(x => (object)x).ToArray());
                ret.Add(fourth[i].Select(x => (object)x).ToArray());
                ret.Add(fifth[i].Select(x => (object)x).ToArray());
            }
            return ret;
        }

        // converts array to Mat
        public static double[,] Convert1DArray2Mat(double[] S, bool Row)
        {
            int size, i;

            size = S.Length;
            double[,] Mat;

            if (Row)
            {
                Mat = new double[1, size];
                for (i = 0; i < size; i++)
                {
                    Mat[0, i] = S[i];
                }
            }
            else
            {
                Mat = new double[size, 1];
                for (i = 0; i < size; i++)
                {
                    Mat[i, 0] = S[i];
                }

            }
            return Mat;
        }

        public static double[] GetIndexVals(double[] Source, int[] Index)
        {
            double[] C = new double[Index.Length];

            for (int i = 0; i < Index.Length; i++)
                C[i] = Source[Index[i]];

            return C;
        }

        public static T[] GetIndexVals<T>(T[] Source, int[] Index)
        {
            T[] C = new T[Index.Length];

            for (int i = 0; i < Index.Length; i++)
                C[i] = Source[Index[i]];

            return C;
        }

        public static T[] GetIndexVals<T>(T[] Source, ArrayList Index)
        {
            T[] C = new T[Index.Count];

            for (int i = 0; i < Index.Count; i++)
                C[i] = Source[(int)Index[i]];

            return C;
        }

        public static string[] GetIndexVals(string[] Source, int[] Index)
        {
            string[] C = new string[Index.Length];

            for (int i = 0; i < Index.Length; i++)
                C[i] = Source[Index[i]];

            return C;
        }

        public static DateTime[] GetIndexVals(DateTime[] Source, int[] Index)
        {
            DateTime[] C = new DateTime[Index.Length];

            for (int i = 0; i < Index.Length; i++)
                C[i] = Source[Index[i]];

            return C;
        }

        public static double[] GetIndexVals(double[] Source, ArrayList Index)
        {
            double[] C = new double[Index.Count];

            for (int i = 0; i < Index.Count; i++)
                C[i] = Source[(int)Index[i]];

            return C;
        }

        public static string[] GetIndexVals(string[] Source, ArrayList Index)
        {
            string[] C = new string[Index.Count];

            for (int i = 0; i < Index.Count; i++)
                C[i] = Source[(int)Index[i]];

            return C;
        }

        public static T[,] GetRows<T>(T[,] Source, int[] Index)
        {
            if(Index.Length > 0)
            {
                T[,] C = new T[Index.Length, Source.GetUpperBound(1)+1];

                for (int i = 0; i < Index.Length; i++)
                    UF.Set_ith_row<T>(ref C, UF.Get_ith_row<T>(Source,Index[i]), i);

                return C;
            }
            else
            {
                return null;
            }
        }

        public static T[,] GetCols<T>(T[,] Source, int[] Index)
        {
            if (Index.Length > 0)
            {
                T[,] C = new T[Source.GetUpperBound(0) + 1, Index.Length];

                for (int i = 0; i < Index.Length; i++)
                    UF.Set_ith_col<T>(ref C, UF.Get_ith_col<T>(Source, Index[i]), i);

                return C;
            }
            else
            {
                return null;
            }
        }

        public static double[] GetRange(double[] Source, int indexStart, int indexEnd)
        {
            double[] C = new double[indexEnd-indexStart+1];

            for (int i = indexStart; i <= indexEnd; i++)
                C[i-indexStart] = Source[i];

            return C;
        }

        public static int[] GetRange(int[] Source, int indexStart, int indexEnd)
        {
            int[] C = new int[indexEnd - indexStart + 1];

            for (int i = indexStart; i <= indexEnd; i++)
                C[i - indexStart] = Source[i];

            return C;
        }

        public static TimeSeries GetRange(TimeSeries Source, int indexStart, int indexEnd)
        {
            TimeSeries ret = new TimeSeries(UF.GetRange(Source.Dates, indexStart, indexEnd),
                UF.GetRange(Source.OHLC.open, indexStart, indexEnd),
                UF.GetRange(Source.OHLC.high, indexStart, indexEnd),
                UF.GetRange(Source.OHLC.low, indexStart, indexEnd),
                UF.GetRange(Source.OHLC.close, indexStart, indexEnd));
            
            return ret;
        }

        public static double ExpectedValue(double[] values, int indexStart, int indexEnd)
        {
            int i;
            double sum = 0.0;
            for (i = indexStart; i < indexEnd; i++)
            {
                sum = sum + values[i];
            }
            return sum / ((double)(indexEnd - indexStart + 1));
        }

        public static double[] GeneratePxSeriesFromReturns(double[] ret, double initvalue)
        {
            int i, n=ret.Length;
            double[] px = new double[n + 1];
            px[0] = initvalue;
            for (i = 1; i < n+1; i++)
            {
                px[i] = px[i - 1] * Math.Exp(ret[i - 1]);
            }
            return px;
        }


        public static string[] GetRange(string[] Source, int indexStart, int indexEnd)
        {
            string[] C = new string[indexEnd - indexStart + 1];

            for (int i = indexStart; i <= indexEnd; i++)
                C[i - indexStart] = Source[i];

            return C;
        }

        public static T[] GetRange<T>(T[] Source, int indexStart, int indexEnd)
        {
            if (indexEnd >= indexStart && Source!=null)
            {
                T[] C = new T[indexEnd - indexStart + 1];

                for (int i = indexStart; i <= indexEnd; i++)
                    C[i - indexStart] = Source[i];

                return C;
            }

            return null;
        }

        public static T[,] GetRange<T>(T[,] Source, int idxStartRow, int idxEndRow,
            int idxStartCol, int idxEndCol)
        {
            T[,] C = new T[idxEndRow - idxStartRow + 1, idxEndCol - idxStartCol + 1];

            for (int i = idxStartRow; i <= idxEndRow; i++)
                for (int j = idxStartCol; j <= idxEndCol; j++)
                    C[i - idxStartRow, j - idxStartCol] = Source[i,j];

            return C;
        }

        public static DateTime[] GetRange(DateTime[] Source, int indexStart, int indexEnd)
        {
            DateTime[] C = new DateTime[indexEnd - indexStart + 1];

            for (int i = indexStart; i <= indexEnd; i++)
                C[i - indexStart] = Source[i];

            return C;
        }

        public static DateTime[] GetRangeFromSortedValues(DateTime[] Source, DateTime Start, DateTime End)
        {
            int iStart, iEnd;

            DateTime tempStart = new DateTime(Start.Year, Start.Month, Start.Day, 1, 0, 0);
            DateTime tempEnd = new DateTime(End.Year, End.Month, End.Day, 23, 0, 0);

            iStart = Array.BinarySearch(Source, tempStart);
            if (iStart < 0)
            {
                iStart = ~iStart;
            }

            iEnd = Array.BinarySearch(Source, tempEnd);
            if (iEnd < 0)
            {
                iEnd = ~iEnd;
                iEnd = iEnd - 1;
            } 

            DateTime[] Output = GetRange(Source, iStart, iEnd);

            return Output;

        }

        public static int[] GetRangeIdxFromSortedValues<T>(T[] Source, T Start, T End)
        {
            int iStart, iEnd;

            iStart = Array.BinarySearch(Source, Start);
            if (iStart < 0)
            {
                iStart = ~iStart;
            }

            iEnd = Array.BinarySearch(Source, End);
            if (iEnd < 0)
            {
                iEnd = ~iEnd;
                iEnd = iEnd - 1;
            }

            int[] output = new int[iEnd - iStart + 1];
            for (int i = iStart; i <= iEnd; i++)
            {
                output[i - iStart] = i;
            }
                        
            return output;
        }

        public static DateTime[] GetAllDates(DateTime FromDate, DateTime ToDate)
        {
            if (FromDate > ToDate)
                throw new Exception("From date smaller than ToDate");
            int ndays = (int)Math.Floor(ToDate.ToOADate() - FromDate.ToOADate()) + 1;
            DateTime[] Output = new DateTime[ndays];
            DateTime Today = FromDate.Date;
            int cnt = 0;
            while (Today <= ToDate.Date)
            {
                Output[cnt] = Today;
                Today = Today.AddDays(1.0);
                cnt = cnt + 1;
            }

            return Output;
        }

        public static DateTime[] GetAllWeekDayDates(DateTime FromDate, DateTime ToDate)
        {
            if (FromDate > ToDate)
                throw new Exception("From date smaller than ToDate");
            int ndays = (int)Math.Floor(ToDate.ToOADate() - FromDate.ToOADate()) + 1;
            List<DateTime> Output = new List<DateTime>(ndays);
            DateTime Today = FromDate.Date;
            
            while (Today <= ToDate.Date)
            {
                if(Today.DayOfWeek != DayOfWeek.Saturday 
                    || Today.DayOfWeek != DayOfWeek.Sunday)
                    Output.Add(Today);
                Today = Today.AddDays(1.0);                
            }

            return Output.ToArray();
        }

        public static DateTime[] GetUniqueMonths(DateTime[] dates)
        {
            int ndays = dates.Length;
            List<DateTime> collection = new List<DateTime>();
            List<string> monthL = new List<string>();
            DateTime[] Output;

            for (int i = 0; i < ndays; i++)
            {
                string monthyr = dates[i].ToString("MMM-yy");
                if (!monthL.Contains(monthyr))
                {
                    monthL.Add(monthyr);
                    collection.Add(dates[i]);
                }
            }
            Output = new DateTime[collection.Count];
            collection.CopyTo(Output);
            return Output;
        }

        public static double[] ReverseArray(double[] source)
        {
            int n = source.Length;
            double[] C = new double[n];
            
            for (int i = 0; i < n; i++)
                C[n-i-1] = source[i];

            return C;
        }

        // true if there is any zero
        public static bool FindAnyZero(double[] Input)
        {
            int n = Input.Length;
            bool flag = false;
            for (int i = 0; i < n; i++)
            {
                if (Input[i] == 0.0)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public static bool FindZeroRun(double[] Input, int RunLen)
        {
            int n = Input.Length;
            bool flag = false;
            int cnt = 0;
            for (int i = 0; i < n; i++)
            {
                if (Input[i] == 0.0)
                {
                    cnt++;
                    if (cnt >= RunLen)
                    {
                        flag = true;
                        break;
                    }
                }
                else
                {
                    cnt = 0;
                }
            }
            return flag;
        }        

    }
    
}
