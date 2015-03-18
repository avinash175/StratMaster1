using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class dfpminClass
    {

        #region Constructors
        public dfpminClass(Function targetfunction, FunctionDerivative funcderivative)
        {
            func = targetfunction;
            funcderiv = funcderivative;
        }
        #endregion

        public delegate double Function(double[] paras);
        public static Function func;
        public delegate double[] FunctionDerivative(double[] paras);
        public static FunctionDerivative funcderiv;

        public void dfpmin(ref double[] p, int n, double GTOL, ref int iter, ref double fret)
        {
            // parameters
            int NMAX, ITMAX, STPMX;
            double EPS, TOLX;
            NMAX = 50;
            ITMAX = 500;
            STPMX = 500;
            EPS = 0.000000000003;
            TOLX = 4 * EPS;

            int i, its, j;
            bool check = false;
            double den, fac, fad, fae, sum, sumdg, sumxi, temp, test;
            double fp;
            double stpmax;
            double[] dg = new double[NMAX];
            double[] hdg = new double[NMAX];
            double[,] hessin = new double[NMAX, NMAX];
            double[] xi = new double[NMAX];
            double[] g = new double[NMAX];
            double[] pnew = new double[NMAX];

            fp = func(p);
            g = funcderiv(p);
            sum = 0;
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++) hessin[i, j] = 0;
                hessin[i, i] = 1;
                xi[i] = -g[i];
                sum = sum + (p[i] * p[i]);
            }
            stpmax = STPMX * Math.Max(Math.Sqrt(sum), n);
            for (its = 0; its < ITMAX; its++)
            {
                iter = its;
                lnsrch(n, p, fp, g, xi, ref pnew, ref fret, stpmax, ref check);
                fp = fret;
                for (i = 0; i < n; i++)
                {
                    xi[i] = pnew[i] - p[i];
                    p[i] = pnew[i];
                }
                test = 0;
                for (i = 0; i < n; i++)
                {
                    temp = Math.Abs(xi[i]) / Math.Max(Math.Abs(p[i]), 1.0);
                    if (temp > test) test = temp;
                }
                if (test < TOLX) return;

                for (i = 0; i < n; i++)
                {
                    dg[i] = g[i];
                }
                g = funcderiv(p);
                test = 0;
                den = Math.Max(Math.Abs(fret), 1.0);
                for (i = 0; i < n; i++)
                {
                    temp = Math.Abs(g[i]) * Math.Max(Math.Abs(p[i]), 1.0) / den;
                    if (temp > test) test = temp;
                }
                if (test < GTOL) return;

                for (i = 0; i < n; i++)
                {
                    dg[i] = g[i] - dg[i];
                }
                for (i = 0; i < n; i++)
                {
                    hdg[i] = 0;
                    for (j = 0; j < n; j++)
                    {
                        hdg[i] = hdg[i] + hessin[i, j] * dg[j];
                    }
                }
                fac = 0;
                fae = 0;
                sumdg = 0;
                sumxi = 0;
                for (i = 0; i < n; i++)
                {
                    fac = fac + dg[i] * xi[i];
                    fae = fae + dg[i] * hdg[i];
                    sumdg = sumdg + dg[i] * dg[i];
                    sumxi = sumxi + xi[i] * xi[i];
                }
                if (fac > Math.Sqrt(EPS * sumdg * sumxi))
                {
                    fac = 1 / fac;
                    fad = 1 / fae;
                    for (i = 0; i < n; i++)
                    {
                        dg[i] = fac * xi[i] - fad * hdg[i];
                    }
                    for (i = 0; i < n; i++)
                    {
                        for (j = 0; j < n; j++)
                        {
                            hessin[i, j] = hessin[i, j] + fac * xi[i] * xi[j]
                            - fad * hdg[i] * hdg[j] + fae * dg[i] * dg[j];
                            hessin[j, i] = hessin[i, j];
                        }
                    }
                }

                for (i = 0; i < n; i++)
                {
                    xi[i] = 0;
                    for (j = 0; j < n; j++)
                    {
                        xi[i] = xi[i] - hessin[i, j] * g[j];
                    }
                }
            }
            //throw new Exception("too many iterations in dfpmin");
            //return 0;
        }

        public void lnsrch(int n, double[] xold, double fold, double[] g, double[] p, ref double[] x,
            ref double f, double stpmax, ref bool check)
        {
            double ALF, TOLX;
            ALF = 0.00001;
            TOLX = 0.0000001;
            int i;
            double A, alam, alamin, b, disc, rhs1, rhs2, slope, sum, temp, test;
            double f2 = 0, alam2 = 0, tmplam = 0;
            string msg = "round-off problem in lnsrch";
            check = false;
            sum = 0;
            for (i = 0; i < n; i++) sum = sum + p[i] * p[i];
            sum = Math.Sqrt(sum);
            if (sum > stpmax) for (i = 0; i < n; i++) p[i] = p[i] * stpmax / sum;
            slope = 0.0;

            for (i = 0; i < n; i++) slope = slope + g[i] * p[i];
            //if (slope >= 0) throw new Exception(msg);
            test = 0;
            for (i = 0; i < n; i++)
            {
                temp = Math.Abs(p[i]) / Math.Max(Math.Abs(xold[i]), 1.0);
                if (temp > test) test = temp;
            }
            alamin = TOLX / test;
            alam = 1;
        continu:
            for (i = 0; i < n; i++) x[i] = xold[i] + alam * p[i];
            f = func(x);
            if (alam < alamin)
            {
                for (i = 0; i < n; i++) x[i] = xold[i];
                check = true;
                return;
            }
            else if (f <= fold + ALF * alam * slope) return;
            else
            {
                if (alam == 1) tmplam = -slope / (2 * (f - fold - slope));
                else
                {
                    rhs1 = f - fold - alam * slope;
                    rhs2 = f2 - fold - alam2 * slope;
                    A = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam)) / (alam - alam2);
                    b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam * alam)) / (alam - alam2);
                    if (A == 0) tmplam = -slope / 2 / b;
                    else
                    {
                        disc = b * b - 3 * A * slope;
                        if (disc < 0) tmplam = 0.5 * tmplam;
                        else if (b <= 0) tmplam = (-b + Math.Sqrt(disc)) / (3 * A);
                        else tmplam = -slope / (b + (Math.Sqrt(disc)));
                    }
                    if (tmplam > 0.5 * alam) tmplam = 0.5 * alam;
                }
            }
            alam2 = alam;
            f2 = f;
            alam = Math.Max(tmplam, 0.1 * alam);
            goto continu;
        }
    }

}
