using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    /// <summary>
    /// OU process to model mean reverting processes
    /// dX(t) = -Kappa*(X(t)-Mean)*dt + Sig*dW(t)
    /// Kappa is the speed of mean reversion
    /// Mean is the expected value of X(t)
    /// Sig is the volatility
    /// W(t) is the brownian motion
    /// </summary>

    public class OUProcess
    {
        public double Kappa { get; set; }
        public double Mean { get; set; }
        public double Sig { get; set; }
        public double dt { get; set; }
        public double[] X { get; set; }
        public bool IsFullTime { get; set; }

        private double x0;
        private double[] dX;

        public OUProcess()
        {

        }

        public OUProcess(double[] path, double _dt)
        {
            X = path;
            dt = _dt;
        }

        public OUProcess(double _kappa, double _mean, double _sig, double _dt, double _x0)
        {
            Kappa = _kappa;
            Mean = _mean;
            Sig = _sig;
            x0 = _x0;
            dt = _dt;
        }

        /// <summary>
        /// Generate path of OU process
        /// X is updated; Kappa, Mean, dt and x0 are assumed to be updated  
        /// </summary>
        /// <param name="len">len - number of data points in the path</param>
        public void GeneratePath(int len)
        {
            if (len > 0)
            {
                dX = new double[len - 1];
                X = new double[len];
                X[0] = x0;
                Random rand = new Random();
                for (int i = 0; i < len - 1; i++)
                {
                    double dWt = Math.Sqrt(dt) * SpecialFunction.inverseCummNormal(rand.NextDouble());
                    dX[i] = -Kappa * (X[i] - Mean) * dt + Sig * dWt;
                }
            }
        }

        /// <summary>
        /// Pass X and dt before hand      
        /// Kappa, Mean and Sig are updated from their initial values
        /// It uses MLE to calibrate
        /// </summary>
        public void CalibrateMLE(bool _IsFullTime)
        {
            IsFullTime = _IsFullTime;
            lbfgsb.settargetfn(new lbfgsb.ObjectiveFunction(OptimizationFun));
            
            double[] paras = new double[4];
            paras[1] = Kappa > 0 ? Kappa : 5.0;
            paras[2] = Mean;
            paras[3] = Sig > 0 ? Sig : 0.2;

            int[] nbd = new int[] { 0, 2, 2, 2 };
            double[] L = new double[] { 0, 0.01, -4, 0.001 };
            double[] U = new double[] { 0, 300.0, 4, 5.0 };
            int info = 0;

            lbfgsb.lbfgsbminimize(3, 3, ref paras, 1e-8, 1e-8, 1e-8, 1000, ref nbd, ref L, ref U, ref info);

            Kappa = paras[1];
            Mean = paras[2];
            Sig = paras[3];

        }

        /// <summary>
        /// Uses MSE for calibration
        /// </summary>
        public void CalibrateMSE()
        {
            int len = X.Length;
            dX = new double[len];

            for (int i = 0; i < len-1; i++)
            {
                dX[i] = X[i + 1] - X[i];
            }

            Regression reg = new Regression(UF.GetRange(dX, 0, len - 1), UF.GetRange(X, 0, len - 1));
            reg.Regress(true);

            Kappa = Math.Max(-reg.Beta / dt,0.1);
            Mean = reg.Alpha / Kappa / dt;
            Sig = UF.StandardDeviation(reg.Res) / Math.Sqrt(dt);
        }

        private double LLH(double[] paras,int step)
        {
            double llh=0;
            double t = step * dt;

            double K = paras[1]; // paras[0] is not used
            double M = paras[2];
            double S = paras[3];

            if (t > 0)
            {
                llh = -0.5 * Math.Log(S * S / (2 * K)) - 0.5 * Math.Log(1 - Math.Exp(-2 * K * t))
                    - (K / (S * S)) * Math.Pow((X[step] - M - (x0 - M) * Math.Exp(-K * t))
                    / (1 - Math.Exp(-2 * K * t)), 2.0);
            }
            else
            {
                llh = 0;
            }
            return -llh;
        }

        private double TotalLLH(double[] paras)
        {
            double TLLH = 0;
            if (X.Length > 0)
            {
                x0 = X[0];
                for (int i = 0; i < X.Length; i++)
                {
                    if (IsFullTime)
                        TLLH += LLH(paras, i);
                    else
                        TLLH += LLH(paras, 1);
                }
            }
            return TLLH;
        }

        public double[] DiffTotalLLH(double[] paras)
        {
            int n = paras.Length;
            double[] dparas = new double[n];
            double eps = 1e-4, temp, h, f1, f0;

            f0 = TotalLLH(paras);

            for (int i = 1; i < n; i++)
            {
                temp = paras[i];
                h = eps * Math.Abs(temp);

                if (h == 0) 
                    h = 1e-4;

                paras[i] = temp + h;
                f1 = TotalLLH(paras);                
                
                paras[i] = temp;
                
                dparas[i] = (f1 - f0) / h;
            }
            return dparas;
        } 

        private void OptimizationFun(ref double[] paras, ref double f, ref double[] g)
        {
            f = TotalLLH(paras);
            g = DiffTotalLLH(paras);
        }
    }
}
