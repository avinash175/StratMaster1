using System;
using System.Collections.Generic;
using System.Text;

namespace Math2
{
    public class Math_
    {
        // Machine constants
        private const double SQTPI = 2.50662827463100050242E0;
        private const double SQRTH = 7.07106781186547524401E-1;
        private const double LOGPI = 1.14472988584940017414;
        private const double MAXLOG = 7.09782712893383996732E2;
        private const double MINLOG = -7.451332191019412076235E2;
        private const double PI = Math.PI;

        public Math_()
        {
        }
        // normal function
        public static double normal(double z)
        {
            return 1.0 / Math.Sqrt(2.0 * PI) * Math.Exp(-z * z / 2.0);

        }


        // cumulative normal function, short code
        public static double cumulativeNormal(double z)
        {
            double c1, c2, c3, c4, c5, c6, w;
            double y;
            c1 = 2.506628;
            c2 = 0.3193815;
            c3 = -0.3565638;
            c4 = 1.7814779;
            c5 = -1.821256;
            c6 = 1.3302744;
            if (z >= 0)
                w = 1;
            else
                w = -1;
            y = 1 / (1 + 0.2316419 * w * z);

            return 0.5 + w * (0.5 - (Math.Exp(-z * z / 2) / c1) *
                (y * (c2 + y * (c3 + y * (c4 + y * (c5 + y * c6))))));
        }
        // cumulative normal function, bigger version
        public double cumNormal(double a)
        {
            double x, y, z;

            x = a * SQRTH;
            z = Math.Abs(x);

            if (z < SQRTH) y = 0.5 + 0.5 * erf(x);
            else
            {
                y = 0.5 * erfc(z);
                if (x > 0) y = 1.0 - y;
            }

            return y;
        }
        // complimentary error function
        public double erfc(double a)
        {
            double x, y, z, p, q;

            double[] P = {
						 2.46196981473530512524E-10,
						 5.64189564831068821977E-1,
						 7.46321056442269912687E0,
						 4.86371970985681366614E1,
						 1.96520832956077098242E2,
						 5.26445194995477358631E2,
						 9.34528527171957607540E2,
						 1.02755188689515710272E3,
						 5.57535335369399327526E2
					 };
            double[] Q = {
						 //1.0
						 1.32281951154744992508E1,
						 8.67072140885989742329E1,
						 3.54937778887819891062E2,
						 9.75708501743205489753E2,
						 1.82390916687909736289E3,
						 2.24633760818710981792E3,
						 1.65666309194161350182E3,
						 5.57535340817727675546E2
					 };

            double[] R = {
						 5.64189583547755073984E-1,
						 1.27536670759978104416E0,
						 5.01905042251180477414E0,
						 6.16021097993053585195E0,
						 7.40974269950448939160E0,
						 2.97886665372100240670E0
					 };
            double[] S = {
						 //1.00000000000000000000E0, 
						 2.26052863220117276590E0,
						 9.39603524938001434673E0,
						 1.20489539808096656605E1,
						 1.70814450747565897222E1,
						 9.60896809063285878198E0,
						 3.36907645100081516050E0
					 };

            if (a < 0.0) x = -a;
            else x = a;

            if (x < 1.0) return 1.0 - erf(a);

            z = -a * a;

            if (z < -MAXLOG)
            {
                if (a < 0) return (2.0);
                else return (0.0);
            }

            z = Math.Exp(z);

            if (x < 8.0)
            {
                p = polevl(x, P, 8);
                q = p1evl(x, Q, 8);
            }
            else
            {
                p = polevl(x, R, 5);
                q = p1evl(x, S, 6);
            }

            y = (z * p) / q;

            if (a < 0) y = 2.0 - y;

            if (y == 0.0)
            {
                if (a < 0) return 2.0;
                else return (0.0);
            }


            return y;
        }
        // error function
        public double erf(double x)
        {
            double y, z;
            double[] T = {
						 9.60497373987051638749E0,
						 9.00260197203842689217E1,
						 2.23200534594684319226E3,
						 7.00332514112805075473E3,
						 5.55923013010394962768E4
					 };
            double[] U = {
						 //1.00000000000000000000E0,
						 3.35617141647503099647E1,
						 5.21357949780152679795E2,
						 4.59432382970980127987E3,
						 2.26290000613890934246E4,
						 4.92673942608635921086E4
					 };

            if (Math.Abs(x) > 1.0) return (1.0 - erfc(x));
            z = x * x;
            y = x * polevl(z, T, 4) / p1evl(z, U, 5);
            return y;
        }

        // returns a polynomial of degree N, given N coefficients
        public double polevl(double x, double[] coef, int N)
        {
            double ans;

            ans = coef[0];

            for (int i = 1; i <= N; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }

        private double p1evl(double x, double[] coef, int N)
        {
            double ans;

            ans = x + coef[0];

            for (int i = 1; i < N; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }
       
        // -1 to 1
        public static double sigmoid(double x)
        {
            double scale = 1.0;
            if (x > 100) x = 100;
            if (x < -100) x = -100;
            return (1.0 - Math.Exp(-scale * x)) / (1.0 + Math.Exp(-scale * x));
        }

        

    }
}
