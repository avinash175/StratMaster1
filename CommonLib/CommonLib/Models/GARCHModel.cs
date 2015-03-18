using System;
using System.Collections.Generic;
using System.Text;


namespace CommonLib
{
    // GARCH model, doesn't capture leverage effect
    public class GARCHpq
    {
        private int p, q;
        public double omega;
        public double[] alpha, beta;
        public double[] retdata;

        // p is for vol and q is for ret sqr
        // alpha - coefficients of vol
        // beta - coefficients for ret^2
        
        public GARCHpq(int p_, int q_)
        {
            p = p_;
            q = q_;
            alpha = new double[p];
            beta = new double[q];            
        }

        public void SetParam(double omega_, double[] alpla_, double[] beta_)
        {
            omega = omega_;
            alpha = alpla_;
            beta = beta_;
        }

        public double[] GenerateVols()
        {
            int i;
            double[] volArray;
            int size = retdata.Length;
            volArray = new double[size];

            //Small approximation in GARCH(1,2)
            for (i = 0; i < Math.Max(p, q); i++)
            {
                volArray[i] = Math.Pow(retdata[i], 2);

            }

            if (p == 1 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i],2.0);
                }
            }
            else if (p == 1 && q == 2)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i],2.0) + beta[1] * Math.Pow(retdata[i - 1],2.0);
                }
            }
            else if (p == 2 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i],2.0);
                }
            }
            else
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i],2.0) + beta[1] * Math.Pow(retdata[i - 1],2.0);
                }
            }

            return volArray;

        }

        public OutputFunction Log_liklihood(double[] parameters, InputStructure S_In)
        {
            int i;
            OutputFunction result = new OutputFunction();
            result.I_nc = 0;
            result.FVr_ca = new double[1] { 0 };
            result.I_no = 1;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            double[] volArray = this.GenerateVols();
            double logLH=0;
            

            for (i = 0; i < volArray.Length-1; i++)
            {
                logLH = logLH + (-Math.Log(volArray[i]) - (Math.Pow(retdata[i + 1], 2) / volArray[i])); 
            }

            result.FVr_oa = new double[1] { -logLH };
            return result;
        }

        public double[,] GenerateUV_Mat(double[] parameters)
        {
            double[,] output = new double[retdata.Length,2];
            omega = parameters[0];
            int i;
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            double[] volArray = this.GenerateVols();
            output[0, 0] = retdata[0];
            for (i = 1; i < retdata.Length; i++)
            {
                output[i, 0] = retdata[i];
                output[i, 1] = volArray[i - 1];
            }

            return output;
        }

        public double ForcastRealizedVol(double[] parameters, int numOfDays)
        {
            double output;
            double[] volPred = new double[numOfDays];
            double[] retlast = new double[q];
            double[] vollast = new double[p];
            omega = parameters[0];
            int i;
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            double[] volArray = this.GenerateVols();

            for (i = 0; i < q; i++)
            {
                retlast[q - i - 1] = retdata[retdata.Length - i - 1];
            }

            for (i = 0; i < p; i++)
            {
                vollast[p - i - 1] = volArray[volArray.Length - i - 1];
            }
                     

            if (p == 1 && q == 1)
            {
                
                volPred[0] = omega + alpha[0] * vollast[0] + beta[0] * Math.Pow(retlast[0], 2.0);
                
                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + alpha[0] * volPred[i - 1] + beta[0] * volPred[i - 1];
                }
            }
            else if (p == 1 && q == 2)
            {

                volPred[0] = omega + alpha[0] * vollast[0] + beta[0] * Math.Pow(retlast[1], 2.0) + beta[1] * Math.Pow(retlast[0], 2.0);
                volPred[1] = omega + alpha[0] * volPred[0] + beta[0] * volPred[0] + beta[1] * Math.Pow(retlast[1], 2.0);

                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + alpha[0] * volPred[i - 1] + beta[0] * volPred[i - 1] + beta[1] * volPred[i - 2];
                }
            }
            else if (p == 2 && q == 1)
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * Math.Pow(retlast[0], 2.0);
                volPred[1] = omega + alpha[0] * volPred[0] + alpha[1] * vollast[1] + beta[0] * volPred[0];
                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + alpha[0] * volPred[i - 1] + alpha[1] * volPred[i - 2] + beta[0] * volPred[i - 1];
                }
            }
            else
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * Math.Pow(retlast[1], 2.0) + beta[1] * Math.Pow(retlast[0], 2.0);
                volPred[1] = omega + alpha[0] * volPred[0] + alpha[1] * vollast[1] + beta[0] * volPred[0] + beta[1] * Math.Pow(retlast[1], 2.0);
                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + alpha[0] * volPred[i - 1] + alpha[1] * volPred[i - 2] + beta[0] * volPred[i - 1] + beta[1] * volPred[i - 2];
                }
            }

            output = Math.Sqrt(252)*Math.Sqrt(UF.ExpectedValue(volPred));

            return output;

        }

    }

    // NGARCH model captures leverage effect but only when |rho| is very high 
    public class NGARCH
    {
        private int p, q;
        public double theta;
        public double omega;
        public double[] alpha, beta;
        public double[] retdata;

        // p is for vol and q is for ret sqr
        // alpha - coefficients of vol
        // beta - coefficients for (ret-theta*Sqrt(v(t))^2
        
        // NGARCH(1,1): v(t+1) = omega + alpha*v(t) + beta*(u(t)-theta*Sqrt(v(t)))^2;

        public NGARCH(int p_, int q_)
        {
            p = p_;
            q = q_;
            alpha = new double[p];
            beta = new double[q];
        }

        public void SetParam(double omega_, double[] alpla_, double[] beta_, double theta_)
        {
            omega = omega_;
            alpha = alpla_;
            beta = beta_;
            theta = theta_;
        }

        public double[] GenerateVols()
        {
            int i;
            double[] volArray;
            int size = retdata.Length;
            volArray = new double[size];

            //Small approximation in GARCH(1,2)
            for (i = 0; i < Math.Max(p, q); i++)
            {
                volArray[i] = Math.Pow(retdata[i], 2);

            }

            if (p == 1 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i] - theta * Math.Sqrt(volArray[i - 1]), 2.0);
                }
            }
            else if (p == 1 && q == 2)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i] - theta * Math.Sqrt(volArray[i - 1]), 2.0) + beta[1] * Math.Pow(retdata[i - 1] - theta * Math.Sqrt(volArray[i - 2]), 2.0);
                }
            }
            else if (p == 2 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i] - theta * Math.Sqrt(volArray[i - 1]), 2.0);
                }
            }
            else
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i] - theta * Math.Sqrt(volArray[i - 1]), 2.0) + beta[1] * Math.Pow(retdata[i - 1] - theta * Math.Sqrt(volArray[i - 2]), 2.0);
                }
            }

            return volArray;

        }

        public OutputFunction Log_liklihood(double[] parameters, InputStructure S_In)
        {
            int i;
            OutputFunction result = new OutputFunction();
            result.I_nc = 0;
            result.FVr_ca = new double[1] { 0 };
            result.I_no = 1;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];

            double[] volArray = this.GenerateVols();
            double logLH = 0;


            for (i = 0; i < volArray.Length-1; i++)
            {
                logLH = logLH + (-Math.Log(volArray[i]) - (Math.Pow(retdata[i + 1], 2) / volArray[i]));
            }

            result.FVr_oa = new double[1] { -logLH };
            return result;
        }

        public double[,] GenerateUV_Mat(double[] parameters)
        {
            double[,] output = new double[retdata.Length, 2];
            int i;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];

            double[] volArray = this.GenerateVols();
            output[0, 0] = retdata[0];
            for (i = 1; i < retdata.Length; i++)
            {
                output[i, 0] = retdata[i];
                output[i, 1] = volArray[i - 1];
            }

            return output;
        }

        public double ForcastRealizedVol(double[] parameters, int numOfDays)
        {
            double output;
            int maxpq = Math.Max(p, q);
            double[] volPred = new double[numOfDays];
            double[] retlast = new double[maxpq];
            double[] vollast = new double[maxpq];

            int i;

            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];

            double[] volArray = this.GenerateVols();

            double Vl = UF.ExpectedValue(volArray);
            double Vlast;
            double ga = omega / Vl;
            double a = alpha[0];
            double b = beta[0];

            if ((a + b + b * theta * theta) >= 1)
            {
                alpha[0] = a / (a + b + b * theta * theta + ga);
                beta[0] = b / (a + b + b * theta * theta + ga);
            }


            for (i = 0; i < maxpq; i++)
            {
                retlast[maxpq - i - 1] = retdata[retdata.Length - i - 1];
            }

            for (i = 0; i < maxpq; i++)
            {
                vollast[maxpq - i - 1] = volArray[volArray.Length - i - 1];
            }

            Vlast = volArray[volArray.Length - 1];

            if (p == 1 && q == 1)
            {

                //volPred[0] = omega + alpha[0] * vollast[0] + beta[0] * (retlast[0] - theta * Math.Sqrt(vollast[0]));

                //for (i = maxpq; i < numOfDays; i++)
                //{
                //    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1];

                //}
                //volPred[0] = Vl + Math.Pow(alpha[0] + beta[0] + beta[0] * theta * theta, 1) * (vollast[0] - Vl);
                for (i = 0; i < numOfDays; i++)
                {
                    //volPred[i] = Vl + Math.Pow(alpha[0] + beta[0] + beta[0] * theta * theta, i+1) * (Vlast - Vl);
                    volPred[i] = Vl + Math.Pow(alpha[0] + beta[0], i + 1) * (Vlast - Vl);
                }

            }
            else if (p == 1 && q == 2)
            {

                volPred[0] = omega + alpha[0] * vollast[1] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1])) + beta[1] * (retlast[0] - theta * Math.Sqrt(vollast[0]));
                volPred[1] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[0] + beta[1] * (retlast[1] - theta * Math.Sqrt(vollast[1]));

                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1] + (beta[1] + beta[1] * theta * theta) * volPred[i - 2];
                }
            }
            else if (p == 2 && q == 1)
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1]));
                volPred[1] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[0] + alpha[1] * vollast[1];

                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1] + alpha[1] * volPred[i - 2];
                }
            }
            else
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1])) + beta[1] * (retlast[0] - theta * Math.Sqrt(vollast[0]));
                volPred[1] = omega + alpha[0] * volPred[0] + alpha[1] * vollast[1] + beta[0] * (1 + theta * theta) * volPred[0] + beta[1] * (retlast[1] - theta * Math.Sqrt(vollast[1]));
                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] * (1 + theta * theta)) * volPred[i - 1] + (alpha[1] + beta[1] * (1 + theta * theta)) * volPred[i - 2];
                }
            }

            output = Math.Sqrt(252) * Math.Sqrt(UF.ExpectedValue(volPred));

            return output;

        }

    }

    class NGARCH_Mod1
    {
        private int p, q;
        public double[] theta;
        public double omega;
        public double[] alpha, beta, delta;
        public double[] retdata;

        // p is for vol and q is for ret sqr
        // alpha - coefficients of vol
        // beta - coefficients for (ret-theta*Sqrt(v(t))^2

        // NGARCH(1,1): v(t+1) = omega + alpha*v(t) + beta*(u(t)-theta*Sqrt(v(t)))^2;

        public NGARCH_Mod1(int p_, int q_)
        {
            p = p_;
            q = q_;
            alpha = new double[p];
            beta = new double[q];
            delta = new double[q];
            theta = new double[q];
        }

        public void SetParam(double omega_, double[] alpla_, double[] beta_, double[] delta_, double[] theta_)
        {
            omega = omega_;
            alpha = alpla_;
            beta = beta_;
            delta = delta_;
            theta = theta_;
        }

        public double[] GenerateVols()
        {
            int i;
            double[] volArray;
            int size = retdata.Length;
            volArray = new double[size];

            //Small approximation in GARCH(1,2)
            for (i = 0; i < Math.Max(p, q); i++)
            {
                volArray[i] = Math.Pow(retdata[i], 2);

            }

            if (p == 1 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i] - theta[0] * Math.Sqrt(volArray[i - 1]), 2.0) + delta[0] * Math.Pow(retdata[i], 2);
                }
            }
            else if (p == 1 && q == 2)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + beta[0] * Math.Pow(retdata[i] - theta[0] * Math.Sqrt(volArray[i - 1]), 2.0) + beta[1] * Math.Pow(retdata[i - 1] - theta[1] * Math.Sqrt(volArray[i - 2]), 2.0) + delta[0] * Math.Pow(retdata[i], 2) + delta[1] * Math.Pow(retdata[i-1], 2);
                }
            }
            else if (p == 2 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i] - theta[0] * Math.Sqrt(volArray[i - 1]), 2.0) + delta[0] * Math.Pow(retdata[i], 2);
                }
            }
            else
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    volArray[i] = omega + alpha[0] * volArray[i - 1] + alpha[1] * volArray[i - 2] + beta[0] * Math.Pow(retdata[i] - theta[0] * Math.Sqrt(volArray[i - 1]), 2.0) + beta[1] * Math.Pow(retdata[i - 1] - theta[1] * Math.Sqrt(volArray[i - 2]), 2.0) + delta[0] * Math.Pow(retdata[i], 2) + delta[1] * Math.Pow(retdata[i - 1], 2);
                }
            }

            return volArray;

        }

        public OutputFunction Log_liklihood(double[] parameters, InputStructure S_In)
        {
            int i;
            OutputFunction result = new OutputFunction();
            result.I_nc = 0;
            result.FVr_ca = new double[1] { 0 };
            result.I_no = 1;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }
            for (i = 0; i < q; i++)
            {
                delta[i] = parameters[i + p + q+ 1];
            }
            for (i = 0; i < q; i++)
            {
                theta[i] = parameters[i + p + 2*q +1];
            }

            
            double[] volArray = this.GenerateVols();
            double logLH = 0;


            for (i = 0; i < volArray.Length-1; i++)
            {
                logLH = logLH + (-Math.Log(volArray[i]) - (Math.Pow(retdata[i + 1], 2) / volArray[i]));
            }

            result.FVr_oa = new double[1] { -logLH };
            return result;
        }

    }

    public class EGARCH
    {
        private int p, q;
        public double theta;
        public double lamda;
        public double omega;
        public double[] alpha, beta;
        public double[] retdata;

        // p is for vol and q is for ret sqr
        // alpha - coefficients of vol
        // beta - coefficients for (ret-theta*Sqrt(v(t))^2
        
        // EGARCH(1,1): v(t+1) = Exp(omega + alpha*log(v(t)) + beta*g(z(t));
        // g(z(t)) = theta*z(t)+lamda*(|z(t)|-E(|z(t)|))
        // z(t) = u(t)/Sqrt(v(t))

        public EGARCH(int p_, int q_)
        {
            p = p_;
            q = q_;
            alpha = new double[p];
            beta = new double[q];
        }

        public void SetParam(double omega_, double[] alpla_, double[] beta_, double theta_, double lamda_)
        {
            omega = omega_;
            alpha = alpla_;
            beta = beta_;
            theta = theta_;
            lamda = lamda_;
        }

        public double[] GenerateVols()
        {
            int i;
            double[] volArray;
            double gz0, gz1, z0, z1;
                        
            int size = retdata.Length;
            volArray = new double[size];

            //Small approximation in GARCH(1,2)
            for (i = 0; i < Math.Max(p, q); i++)
            {
                volArray[i] = Math.Pow(retdata[i], 2);

            }

            if (p == 1 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    z0 = retdata[i]/Math.Sqrt(volArray[i-1]);
                    gz0 = theta * z0 + lamda * (Math.Abs(z0) - Math.Sqrt(2.0 / Math.PI));
                    volArray[i] = Math.Exp( omega + alpha[0] * Math.Log(volArray[i - 1]) + beta[0] * gz0);
                }
            }
            else if (p == 1 && q == 2)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    z0 = retdata[i] / Math.Sqrt(volArray[i - 1]);
                    gz0 = theta * z0 + lamda * (Math.Abs(z0) - Math.Sqrt(2.0 / Math.PI));
                    z1 = retdata[i - 1] / Math.Sqrt(volArray[i - 2]);
                    gz1 = theta * z1 + lamda * (Math.Abs(z1) - Math.Sqrt(2.0 / Math.PI));
                    volArray[i] = Math.Exp(omega + alpha[0] * Math.Log(volArray[i - 1]) + beta[0] * gz0 + beta[1] * gz1);
                }
            }
            else if (p == 2 && q == 1)
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    z0 = retdata[i] / Math.Sqrt(volArray[i - 1]);
                    gz0 = theta * z0 + lamda * (Math.Abs(z0) - Math.Sqrt(2.0 / Math.PI));
                    volArray[i] = Math.Exp(omega + alpha[0] * Math.Log(volArray[i - 1]) + alpha[1] * Math.Log(volArray[i - 2]) + beta[0] * gz0);
                }
            }
            else
            {
                for (i = Math.Max(p, q); i < size; i++)
                {
                    z0 = retdata[i] / Math.Sqrt(volArray[i - 1]);
                    gz0 = theta * z0 + lamda * (Math.Abs(z0) - Math.Sqrt(2.0 / Math.PI));
                    z1 = retdata[i - 1] / Math.Sqrt(volArray[i - 2]);
                    gz1 = theta * z1 + lamda * (Math.Abs(z1) - Math.Sqrt(2.0 / Math.PI));
                    volArray[i] = Math.Exp(omega + alpha[0] * Math.Log(volArray[i - 1]) + alpha[1] * Math.Log(volArray[i - 2]) + beta[0] * gz0 + beta[1] * gz1);
                }
            }

            return volArray;

        }

        public OutputFunction Log_liklihood(double[] parameters, InputStructure S_In)
        {
            int i;
            OutputFunction result = new OutputFunction();
            result.I_nc = 0;
            result.FVr_ca = new double[1] { 0 };
            result.I_no = 1;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];
            lamda = parameters[2 + p + q];

            double[] volArray = this.GenerateVols();
            double logLH = 0;


            for (i = 0; i < volArray.Length-1; i++)
            {
                logLH = logLH + (-Math.Log(volArray[i]) - (Math.Pow(retdata[i + 1], 2) / volArray[i]));
                
            }

            result.FVr_oa = new double[1] { -logLH };
            return result;
        }

        //public OutputFunction Log_liklihoodReduced(double[] parameters, InputStructure S_In)
        //{
        //    int i;
        //    OutputFunction result = new OutputFunction();
        //    result.I_nc = 0;
        //    result.FVr_ca = new double[1] { 0 };
        //    result.I_no = 1;
        //    omega = 0;
        //    for (i = 0; i < p; i++)
        //    {
        //        alpha[i] = parameters[i];
        //    }
        //    for (i = 0; i < q; i++)
        //    {
        //        beta[i] = parameters[i + p];
        //    }

        //    theta = parameters[p + q];
        //    lamda = 0;

        //    double[] volArray = this.GenerateVols();
        //    double logLH = 0;


        //    for (i = 0; i < volArray.Length; i++)
        //    {
        //        logLH = logLH + (-Math.Log(volArray[i]) - (Math.Pow(retdata[i + 1], 2) / volArray[i]));
        //    }

        //    result.FVr_oa = new double[1] { -logLH };
        //    return result;
        //}

        public double[,] GenerateUV_Mat(double[] parameters)
        {
            double[,] output = new double[retdata.Length, 2];
            int i;
            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];
            lamda = parameters[2 + p + q];

            double[] volArray = this.GenerateVols();
            output[0, 0] = retdata[0];
            for (i = 1; i < retdata.Length; i++)
            {
                output[i, 0] = retdata[i];
                output[i, 1] = volArray[i - 1];
            }

            return output;
        }

        public double ForcastRealizedVol(double[] parameters, int numOfDays)
        {
            double output;
            int maxpq = Math.Max(p, q);
            double[] volPred = new double[numOfDays];
            double[] retlast = new double[maxpq];
            double[] vollast = new double[maxpq];

            int i;

            omega = parameters[0];
            for (i = 0; i < p; i++)
            {
                alpha[i] = parameters[i + 1];
            }
            for (i = 0; i < q; i++)
            {
                beta[i] = parameters[i + p + 1];
            }

            theta = parameters[1 + p + q];
            lamda = parameters[2 + p + q];
            
            double[] volArray = this.GenerateVols();
            
            for (i = 0; i < maxpq; i++)
            {
                retlast[maxpq - i - 1] = retdata[retdata.Length - i - 1];
            }

            for (i = 0; i < maxpq; i++)
            {
                vollast[maxpq - i - 1] = volArray[volArray.Length - i - 1];
            }


            if (p == 1 && q == 1)
            {

                volPred[0] = omega + alpha[0] * vollast[0] + beta[0] * (retlast[0] - theta * Math.Sqrt(vollast[0]));

                for (i = maxpq; i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1];
                }
            }
            else if (p == 1 && q == 2)
            {

                volPred[0] = omega + alpha[0] * vollast[1] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1])) + beta[1] * (retlast[0] - theta * Math.Sqrt(vollast[0]));
                volPred[1] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[0] + beta[1] * (retlast[1] - theta * Math.Sqrt(vollast[1]));

                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1] + (beta[1] + beta[1] * theta * theta) * volPred[i - 2];
                }
            }
            else if (p == 2 && q == 1)
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1]));
                volPred[1] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[0] + alpha[1] * vollast[1];

                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] + beta[0] * theta * theta) * volPred[i - 1] + alpha[1] * volPred[i - 2];
                }
            }
            else
            {
                volPred[0] = omega + alpha[0] * vollast[1] + alpha[1] * vollast[0] + beta[0] * (retlast[1] - theta * Math.Sqrt(vollast[1])) + beta[1] * (retlast[0] - theta * Math.Sqrt(vollast[0]));
                volPred[1] = omega + alpha[0] * volPred[0] + alpha[1] * vollast[1] + beta[0] * (1 + theta * theta) * volPred[0] + beta[1] * (retlast[1] - theta * Math.Sqrt(vollast[1]));
                for (i = Math.Max(p, q); i < numOfDays; i++)
                {
                    volPred[i] = omega + (alpha[0] + beta[0] * (1 + theta * theta)) * volPred[i - 1] + (alpha[1] + beta[1] * (1 + theta * theta)) * volPred[i - 2];
                }
            }

            output = Math.Sqrt(252) * Math.Sqrt(UF.ExpectedValue(volPred));

            return output;

        }

        

    } 

}
