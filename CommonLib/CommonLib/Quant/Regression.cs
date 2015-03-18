using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public interface IRegression
    {
        void Regress(bool WithRes);
    }

    public class Regression : IRegression
    {
        public double[] Y { get; set; }
        public double[] X { get; set; }
        public double[] Res { get; set; }
        public double Beta { get; set; }
        public double Alpha { get; set; }

        public Regression()
	    {

	    }

        public Regression(double[] _Y, double[] _X)
	    {
            Y = _Y;
            X=_X;
	    }

        public void Regress(bool WithRes)
        {
            int len = X.Length;
            if (X.Length != Y.Length)
            {
                throw new Exception("Dimensions do not match in the regression equation");       
            }

            double sumX = 0;
            double sumY = 0;
            double sumX2 = 0;
            double sumY2 = 0;
            double sumXY = 0;

            for (int i = 0; i < len; i++)
            {
                sumX += X[i];
                sumY += Y[i];
                sumX2 += X[i] * X[i];
                sumY2 += Y[i] * Y[i];
                sumXY += X[i] * Y[i];
            }           
                        
            double meanX = sumX / len;
            double meanY = sumY / len;
            double SD_X = Math.Sqrt((sumX2 / len) - meanX * meanX);
            double SD_Y = Math.Sqrt((sumY2 / len) - meanY * meanY);
            double Corr = ((sumXY / len) - meanX * meanY)/(SD_X*SD_Y);

            Beta = Corr * SD_Y / SD_X;
            Alpha = meanY - Beta * meanX;

            Res = new double[X.Length];

            if (WithRes)
            {
                for (int i = 0; i < X.Length; i++)
                {
                    Res[i] = Y[i] - Beta * X[i] - Alpha;
                }
            }                                  
        }
    }

    public class MultiRegression
    {
        public double[] Y { get; set; }
        public double[,] X { get; set; }
        public double[] Beta { get; set; }
        public double[] Res { get; set; }

        public MultiRegression()
        {

        }

        public MultiRegression(double[] _Y, double[,] _X)
        {
            Y = _Y;
            X = _X;
        }

        public void Regress(bool WithRes, double ridgeDelta)
        {
            if (Y.Length != X.GetUpperBound(0) + 1)
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] XT = Matrix.Transpose(X);

            double[,] YMat = UF.Convert1DArray2Mat(Y, false);//COL

            double[,] XTX = UF.MatrixAdd(UF.MatrixMul(XT, X),
                UF.MatrixMulByConst(UF.IdentityD(X.GetUpperBound(1)+1),ridgeDelta)
                ,true);
            double[,] XTY = UF.MatrixMul(XT, YMat);

            inv.rmatrixinverse(ref XTX, XTX.GetUpperBound(0) + 1);

            double[,] BETA = Matrix.MatrixMul(XTX, XTY);

            Beta = UF.Get_ith_col(BETA, 0);

            Res = new double[Y.Length];

            if (WithRes)
            {  
                for (int i = 0; i < Y.Length; i++)
                {
                    double temp=0;
                    for (int j = 0; j < X.GetUpperBound(1)+1; j++)
			        {
			             temp += Beta[j] * X[i,j];
			        }
                    Res[i] = Y[i] - temp;
                }
            } 
        }
    }

    public class Regression2Var : IRegression
    {
        public double[] Y { get; set; }
        public double[] X1 { get; set; }
        public double[] X2 { get; set; }
        public double Beta1 { get; set; }
        public double Beta2 { get; set; }
        public double[] Res { get; set; }

        public Regression2Var()
        {

        }

        public Regression2Var(double[] _Y, double[] _X1, double[] _X2)
        {
            Y = _Y;
            X1 = _X1;
            X2 = _X2;
        }

        public void Regress(bool WithRes)
        {
            int len = X1.Length;
            if (len != Y.Length || X2.Length != Y.Length)
                throw new Exception("Regression failed: Array dimensions miss match");

            double sumX1 = 0;
            double sumX2 = 0;
            double sumY = 0;
            double sumX1S = 0;
            double sumX2S = 0;
            double sumYS = 0;
            double sumX1Y = 0;
            double sumX2Y = 0;
            double sumX1X2 = 0;

            for (int i = 0; i < len; i++)
            {
                sumX1 += X1[i];
                sumX2 += X2[i];
                sumY += Y[i];
                sumYS += Y[i] * Y[i];
                sumX1S += X1[i] * X1[i];
                sumX2S += X2[i] * X2[i];
                sumX1Y += X1[i] * Y[i];
                sumX2Y += X2[i] * Y[i];
                sumX1X2 += X1[i] * X2[i];
            }

            Beta2 = (sumX1Y * sumX1X2 - sumX1S * sumX2Y) / (sumX1X2 * sumX1X2 - sumX1S * sumX2S);
            Beta1 = (sumX2Y * sumX1X2 - sumX2S * sumX1Y) / (sumX1X2 * sumX1X2 - sumX1S * sumX2S);

            if (WithRes)
            {
                Res = new double[len];
                for (int i = 0; i < len; i++)
                {
                    Res[i] = Y[i] - Beta1 * X1[i] - Beta2 * X2[i];
                }
            }
        }
    }
}
