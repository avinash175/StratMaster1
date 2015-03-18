using System;
using System.Collections.Generic;
using System.Text;


namespace CommonLib
{
    public class Quant
    {
        // Create public static methods for Quant functions

        public static int[] RunsArray(double[] StockPr, bool SignSen)
        {
            double[] ret = UF.Convert2Returns(StockPr, 1);
            int[] sign = UF.Sign(ret);

            int[] runs = new int[sign.Length];
            runs[0] = Math.Abs(sign[0]);
            for (int i = 1; i < sign.Length; i++)
            {
                if (SignSen)
                {
                    if (sign[i] == sign[i - 1])
                    {
                        runs[i] = sign[i] + runs[i - 1];
                    }
                    else
                    {
                        runs[i] = sign[i];
                    }
                }
                else
                {
                    if (sign[i] == sign[i - 1])
                    {
                        runs[i] = Math.Abs(sign[i]) + runs[i - 1];
                    }
                    else
                    {
                        runs[i] = Math.Abs(sign[i]);
                    }

                }
            }
            return runs;
        }

        public static double SharpeRatio(double[] StockPr, double IntRate)
        {
            double[] ret = UF.Convert2Returns(StockPr, 1);

            double sd = UF.StandardDeviation(ret);
            double mean = UF.ExpectedValue(ret);

            return ((mean - IntRate) / sd);
        }

        public static double Regression(double[] Y, double[] X)
        {
            // assuming the Series have zero mean
            // Y = beta*X + res
            // beta = Corr(X,Y) * SD(Y) / SD(X)

            double Corr = UF.Correlation(X, Y);
            double SD_X = UF.StandardDeviation(X);
            double SD_Y = UF.StandardDeviation(Y);

            double beta = Corr * SD_Y / SD_X;

            return beta;
        }

        public static double[] RegressionRes(double[] Y, double[] X)
        {
            // assuming the Series have zero mean
            // Y = beta*X + res
            // beta = Corr(X,Y) * SD(Y) / SD(X)

            double Corr = UF.Correlation(X, Y);
            double SD_X = UF.StandardDeviation(X);
            double SD_Y = UF.StandardDeviation(Y);

            double beta = Corr * SD_Y / SD_X;

            double[] res = UF.ArraySub(Y, UF.MulArrayByConst(X, beta));

            return res;
        }

        public static double[] MultiRegression(double[] Y, double[,] X)
        {
            if (Y.Length != X.GetUpperBound(0) + 1)
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] XT = Matrix.Transpose(X);

            double[,] YMat = UF.Convert1DArray2Mat(Y, false);//COL

            double[,] XTX = UF.MatrixMul(XT, X);
            double[,] XTY = UF.MatrixMul(XT, YMat);

            inv.rmatrixinverse(ref XTX, XTX.GetUpperBound(0) + 1);

            double[,] Beta = Matrix.MatrixMul(XTX, XTY);

            double[] beta = UF.Get_ith_col(Beta,0);

            return beta;
        }

        public static double[] PolynomialCurveFitParas(double[] Y, double[] X, int polyOrder)
        {

            double[,] XMat = new double[X.Length,polyOrder+1];

            UF.Set_ith_col(ref XMat, UF.Ones(X.Length), 0);

            for (int i = 0; i < polyOrder; i++)
            {
                UF.Set_ith_col(ref XMat, UF.ArrayPower(X, i+1), i+1);
            }

            return MultiRegression(Y, XMat);            
        }

        public static double[] PolynomialCurveFitValues(double[] Y, double[] X, int polyOrder)
        {
            double[] paras = PolynomialCurveFitParas(Y, X, polyOrder);

            double[] yhat = new double[X.Length];

            for (int i = 0; i < X.Length; i++)
            {
                yhat[i] = paras[0];
                for (int j = 1; j < paras.Length; j++)
                {
                    yhat[i] += paras[j] * Math.Pow(X[i], j);
                }
            }

            return yhat;
        }

        public static long Factorial(int k)
        {
            if (k< 0)
                throw new Exception("choose k>0");
            if (k == 1 || k == 0)
                return 1;
            else
            {
                return k * Factorial(k - 1);
            }
        }

        public static long Comb(int n, int r)
        {
            if (n < r || n < 0 || r < 0)
                throw new Exception("choose n>r, n>0 and r>0");
            if (n == r)
                return 1;
            long result = Perm(n, r) / Factorial(r);
            return result;
        }

        public static long Perm(int n, int r)
        {
            if (n < r || n < 0 || r < 0)
                throw new Exception("choose n>r, n>0 and r>0");
            long result=1;
            for (int i = 0; i < r; i++)
                result = result * (n - i);
            return result;
        }
        /// <summary>
        /// Returns Confusion Matrix with actuals along rows and 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicted"></param>
        /// <returns></returns>
        public static int[,] ConfusionMatrix(int[] actual, int[] predicted, out int[] classes)
        {
            int[] classesActual = UF.Unique(actual);
            int[] classesPredicted = UF.Unique(predicted);            

            classes = UF.AppendArray(classesActual, classesPredicted);
            classes = UF.Unique(classes);
            Array.Sort(classes);

            int[,] CM = new int[classes.Length, classes.Length];

            for (int i = 0; i < actual.Length; i++)
            {
                int row = Array.BinarySearch<int>(classes, actual[i]);
                int col = Array.BinarySearch<int>(classes, predicted[i]);

                CM[row, col]++;
            }

            return CM;
        }

        /// <summary>
        /// Finds AUC for binary classification
        /// </summary>
        /// <param name="predicted"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static double AUCBinaryClassification(int[] predicted, int[] actual, int posLabel, int negLabel)
        {
            int[] idxp = NF.FindMatchIndex<int>(actual, posLabel);
            int[] idxn = NF.FindMatchIndex<int>(actual, negLabel);

            double sum = 0;

            for (int i = 0; i < idxp.Length; i++)
            {
                for (int j = 0; j < idxn.Length; j++)
                {
                    if (predicted[idxp[i]] > predicted[idxn[j]])
                    {
                        sum += 1;
                    }
                }
            }
            return sum / idxp.Length / idxn.Length;
        }

        public static Point[] ROC(int[] predicted, int[] actual, int numPoints,
            int posIdx, int negIdx)
        {
            Point[] roc = new Point[numPoints];
            Random rnd = new Random();
            
            for (int i = 0; i < numPoints; i++)
            {
                roc[i] = new Point();
                int n = Math.Max(rnd.Next(predicted.Length - 1), predicted.Length/4);
                int[] idx = UF.GetRange(UF.randperm(predicted.Length),0,n-1);
                
                int[] act = UF.GetIndexVals<int>(actual, idx);
                int[] pre = UF.GetIndexVals<int>(predicted, idx);

                int[] pos = NF.FindMatchIndex<int>(act, posIdx);
                int numPos = pos.Length;

                int[] neg = NF.FindMatchIndex<int>(act, negIdx);
                int numNeg = neg.Length;

                int[] Ppos = NF.FindMatchIndex<int>(UF.GetIndexVals(pre, pos), posIdx);
                int[] Npos = NF.FindMatchIndex<int>(UF.GetIndexVals(pre, neg), posIdx);

                if (pos.Length > 0)
                {
                    roc[i].Y = (double)Ppos.Length / pos.Length;
                }                

                if (neg.Length > 0)
                {
                    roc[i].X = (double)Npos.Length / neg.Length;
                }
                               
            }
            return roc;
        }

        public static double Accuracy(double[,] CM)
        {
            double sum = 0;
            double diag = 0;

            for (int i = 0; i <= CM.GetUpperBound(0); i++)
            {
                diag += CM[i, i];
                for (int j = 0; j <= CM.GetUpperBound(1); j++)
                {
                    sum += CM[i, j];
                }
            }

            return diag / sum;

        }

        public static double Accuracy(int[,] CM)
        {
            double sum = 0;
            double diag = 0;

            for (int i = 0; i <= CM.GetUpperBound(0); i++)
            {
                diag += CM[i, i];
                for (int j = 0; j <= CM.GetUpperBound(1); j++)
                {
                    sum += CM[i, j];
                }
            }

            return diag / sum;
        }


        public static double[] TrendSignStrength(double[] signal, int longWin, int numSW)
        {
            double[] ret = new double[signal.Length];
            int shortWin = (int)(longWin / (double)numSW);

            for (int i = longWin; i < signal.Length; i++)
            {
                double cnt = 0;
                for (int j = i - longWin + shortWin; j <= i; j+=shortWin)
                {
                    if (signal[j] - signal[j-shortWin] > 0)
	                {
		                cnt+=1;
	                }
                }
                ret[i] = (cnt / numSW) - 0.5;
            }
            return ret;
        }

        public static double[] NoiseMagnitude(double[] signal, int longWin, int numSW)
        {
            double[] ret = new double[signal.Length];
            int shortWin = (int)(longWin / (double)numSW);
            
            for (int i = longWin; i < signal.Length; i++)
            {
                double longRet = Math.Abs(signal[i] / signal[i - longWin] - 1);
                double sum = 0.0;
                for (int j = i - longWin + shortWin; j <= i; j += shortWin)
                {
                    sum += Math.Abs(signal[j] / signal[j - shortWin] - 1);                    
                }
                ret[i] = (sum / numSW) / longRet;
            }
            return ret;
        }

        /// <summary>
        /// Measures the affirmity of trend
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="longWin"></param>
        /// <param name="numSW"></param>
        /// <param name="deltaC">Band</param>
        /// <param name="UseRelativeDelta">Relative to the return</param>
        /// <param name="IsSmoothEdge">Fuzzy edge</param>
        /// <returns></returns>
        public static double[] TimeInRange(double[] signal, int longWin, int numSW, double deltaC,
            bool UseRelativeDelta, bool IsSmoothEdge = true)
        {
            double[] ret = new double[signal.Length];
            int shortWin = (int)(longWin / (double)numSW);

            for (int i = longWin; i < signal.Length; i++)
            {
                double y2 = signal[i];
                double y1 = signal[i - longWin];
                double x2 = i;
                double x1 = i - longWin;

                double m = (y2 - y1) / (x2 - x1);
                double c = y1 - m * x1;
                double delC = deltaC;
                
                if (UseRelativeDelta)
                {
                    delC = y2 * deltaC;
                }

                double sum = 0.0;

                for (int j = i - longWin + shortWin; j <= i; j += shortWin)
                {
                    if (signal[j] <= (m * j + c + delC) && signal[j] >= (m * j + c - delC))
                    {
                        sum += 1;
                    }
                    else if (IsSmoothEdge && signal[j] >= (m * j + c + delC) 
                        && signal[j] <= (m * j + c + 2.0 * delC))
                    {
                        double xd = signal[j] - (m * j + c + delC);
                        sum += -(1 / delC) * xd + 1;
                    }
                    else if (IsSmoothEdge && signal[j] <= (m * j + c - delC) 
                        && signal[j] >= (m * j + c - 2.0 * delC))
                    {
                        double xd = (m * j + c - delC) - signal[j];
                        sum += -(1 / delC) * xd + 1;
                    }
                }

                ret[i] = (sum / numSW);
            }
            return ret;
        }
        

        /// <summary>
        /// Use it with NoiseMagnitude, Abs Ret Magnitude and TimeInRange
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="longWin"></param>
        /// <param name="numSW"></param>
        /// <returns></returns>
        public static double[] ReturnDisparity(double[] signal, int longWin, int numSW)
        {
            double[] ret = new double[signal.Length];
            int shortWin = (int)(longWin / (double)numSW);

            for (int i = longWin; i < signal.Length; i++)
            {
                double longRet = Math.Abs(signal[i] / signal[i - longWin] - 1);
                double sumP = 0.0;
                double sumN = 0.0;
                int cntP=0, cntN=0;
                for (int j = i - longWin + shortWin; j <= i; j += shortWin)
                {
                    double r = signal[j] / signal[j - shortWin] - 1;
                    if (r > 0)
                    {
                        sumP += r;
                        cntP++;
                    }
                    else if(r < 0)
                    {
                        sumN += -r;
                        cntN++;
                    }
                }
                ret[i] = ((sumP / cntP) - (sumN / cntN)) / longRet;
            }
            return ret;
        }

        public static double[] SWAK(double[] price, int period, FilterType ft, double delta = 0.25)
        {
            double[] ret = null;
            double alpha = 0, beta = 0, gamma = 0;
                        
            switch (ft)
            {
                case FilterType.DEFAULT:
                    ret = SWAKTF(price, 1, 0, 0, 1, 0, 0, 0, 0);
                    break;
                case FilterType.EMA:
                    alpha = 2.0 / (1 + period);
                    ret = SWAKTF(price, 1, 0, 0, alpha, 0, 0, (1 - alpha), 0);
                    break;
                case FilterType.SMA:
                    ret = SWAKTF(price, 1, 1.0 / period, period, 1.0 / period, 0, 0, 1, 0);
                    break;
                case FilterType.GAUSS:
                    beta = 2.415 * (1 - Math.Cos(Math.PI * 2 / period));
                    alpha = -beta + Math.Sqrt(beta * beta + 2 * beta);
                    ret = SWAKTF(price, alpha, 0, 0, 1, 0, 0, 2 * (1 - alpha), -(1 - alpha) * (1 - alpha));
                    break;
                case FilterType.BUTTER:
                    beta = 2.415 * (1 - Math.Cos(Math.PI * 2 / period));
                    alpha = -beta + Math.Sqrt(beta * beta + 2 * beta);
                    ret = SWAKTF(price, alpha*alpha/4, 0, 0, 1, 2, 1, 2 * (1 - alpha), -(1 - alpha) * (1 - alpha));
                    break;
                case FilterType.SMOOTH:
                    ret = SWAKTF(price, 1.0 / 4, 0, 0, 1, 2, 1, 0, 0);
                    break;
                case FilterType.HP:
                    alpha = (Math.Cos(2 * Math.PI / period) + Math.Sin(2 * Math.PI / period) - 1) 
                        / (Math.Cos(2 * Math.PI / period));
                    ret = SWAKTF(price, 1 - alpha/2 , 0, 0, 1, -1, 0, 1-alpha, 0);
                    break;
                case FilterType.TwoPHP:
                    beta = 2.415 * (1 - Math.Cos(Math.PI * 2 / period));
                    alpha = -beta + Math.Sqrt(beta * beta + 2 * beta);
                    ret = SWAKTF(price,1- alpha/2, 0, 0, 1, -2, 1, 2 * (1 - alpha), -(1 - alpha) * (1 - alpha));
                    break;
                case FilterType.BP:
                    beta = Math.Cos(2 * Math.PI / period);
                    gamma = Math.Cos(4 * Math.PI * delta / period);
                    alpha = (1 / gamma) - Math.Sqrt(1/(gamma*gamma)-1);
                    ret = SWAKTF(price, (1 - alpha) / 2, 0, 0, 1, 0, -1, beta * (1 + alpha), -alpha);
                    break;
                case FilterType.BS:
                    beta = Math.Cos(2 * Math.PI / period);
                    gamma = Math.Cos(4 * Math.PI * delta / period);
                    alpha = (1 / gamma) - Math.Sqrt(1/(gamma*gamma)-1);
                    ret = SWAKTF(price, (1 + alpha) / 2, 0, 0, 1, -2*beta, 1, beta * (1 + alpha), -alpha);
                    break;
            }

            return ret;
        }

        public static double[] SWAKTF(double[] px, double c0, double c1, int N, double b0, double b1,
            double b2, double a1, double a2)
        {
            double[] ret = new double[px.Length];

            if (N > 0)
            {
                for (int i = 0; i < N; i++)
                {
                    ret[i] = px[i];
                }

                for (int i = N; i < px.Length; i++)
                {
                    ret[i] = c0 * (b0 * px[i] + b1 * px[i - 1] + b2 * px[i - 2])
                        + a1 * ret[i - 1] + a2 * ret[i - 2] - c1 * px[i - N];
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    ret[i] = px[i];
                }

                for (int i = 2; i < px.Length; i++)
                {
                    ret[i] = c0 * (b0 * px[i] + b1 * px[i - 1] + b2 * px[i - 2])
                        + a1 * ret[i - 1] + a2 * ret[i - 2] - c1 * px[i - N];
                }
            }

            return ret;
        }

    }

    public enum FilterType
    {
        DEFAULT,
        EMA,
        SMA,
        GAUSS,
        BUTTER,
        SMOOTH,
        HP,
        TwoPHP,
        BP,
        BS,        
    }
}
