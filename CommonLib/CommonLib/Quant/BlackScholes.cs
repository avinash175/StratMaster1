using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class BlackScholes
    {
        public Option BSOption { get; set; }

        public BlackScholes()
        {

        }

        public BlackScholes(Option bs)
        {
            BSOption = new Option(bs);
        }

        private static double[] nsia = { 2.50662823884, -18.61500062529, 41.39119773534, -25.44106049637 };
        private static double[] nsib = { -8.4735109309, 23.08336743743, -21.06224101826, 3.13082909833 };
        private static double[] nsic = { 0.3374754822726147, 0.9761690190917186, 0.1607979714918209, 0.0276438810333863, 0.0038405729373609, 0.0003951896511919, 0.0000321767881768, 0.0000002888167364, 0.0000003960315187 };

        //cumulative normal distribution function
        private double CND(double X)
        {
            double L = 0.0;
            double K = 0.0;
            double dCND = 0.0;
            const double a1 = 0.31938153;
            const double a2 = -0.356563782;
            const double a3 = 1.781477937;
            const double a4 = -1.821255978;
            const double a5 = 1.330274429;
            L = Math.Abs(X);
            K = 1.0 / (1.0 + 0.2316419 * L);
            dCND = 1.0 - 1.0 / Math.Sqrt(2 * Convert.ToDouble(Math.PI.ToString())) *
                Math.Exp(-L * L / 2.0) * (a1 * K + a2 * K * K + a3 * Math.Pow(K, 3.0) +
                a4 * Math.Pow(K, 4.0) + a5 * Math.Pow(K, 5.0));

            if (X < 0)
            {
                return 1.0 - dCND;
            }
            else
            {
                return dCND;
            }
        }

        //function phi
        private double phi(double x)
        {
            double phi = 0.0;

            phi = Math.Exp(-x * x / 2) / Math.Sqrt(2 * Math.PI);
            return phi;
        }


        public static double NORMSINV(double probability)
        {
            double r = 0;
            double x = 0;
            x = probability - 0.5;
            if (Math.Abs(x) < 0.42)
            {
                r = x * x;
                r = x * (((nsia[3] * r + nsia[2]) * r + nsia[1]) * r + nsia[0]) / ((((nsib[3] * r + nsib[2]) * r + nsib[1]) * r + nsib[0]) * r + 1);
                return r;
            }
            r = probability;
            if (x > 0)
                r = 1 - probability;

            r = Math.Log(-Math.Log(r));
            r = nsic[0] + r * (nsic[1] + r * (nsic[2] + r * (nsic[3] + r * (nsic[4] + r * (nsic[5] + r * (nsic[6] + r * (nsic[7] + r * nsic[7])))))));
            return Math.Abs(r);
        }

        public static double NORMINV(double probability, double mean, double standard_deviation)
        {
            return (NORMSINV(probability) * standard_deviation + mean);
        }

        public static double NORMINV(double probability, double[] values)
        {
            return NORMINV(probability, Mean(values), StandardDeviation(values));
        }

        public static double Mean(double[] values)
        {
            double tot = 0;
            foreach (double val in values)
                tot += val;

            return (tot / values.Length);
        }

        public static double StandardDeviation(double[] values)
        {
            return Math.Sqrt(Variance(values));
        }

        public static double Variance(double[] values)
        {
            double m = Mean(values);
            double result = 0;
            foreach (double d in values)
                result += Math.Pow((d - m), 2);

            return (result / values.Length);
        }

        public double blsimpv()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            const double ACCURACY = 1.0e-6;
            double ComputedValue;
            double ComputedVolatility = Math.Pow(Math.Abs(Math.Log(Price / Strike) + Rate * Time) * 2 / Time, 0.5); // initial value of volatility
            BSOption.IV = ComputedVolatility;

            if (check)
                ComputedValue = blsCall();
            else
                ComputedValue = blsPut();
            
            double Vega = blsvega();            

            while (Math.Abs(Value - ComputedValue) > ACCURACY)
            {
                ComputedVolatility = ComputedVolatility - ((ComputedValue - Value) / Vega);
                if (ComputedVolatility < 0)
                {
                    return 0;
                }
                BSOption.IV = ComputedVolatility;
                if (check == true)
                    ComputedValue = blsCall();
                else
                    ComputedValue = blsPut();
                Vega = blsvega();
            }

            return ComputedVolatility;
        }

        public double blsFutImpv()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            const double ACCURACY = 1.0e-6;
            double ComputedValue;
            double ComputedVolatility = Math.Pow(Math.Abs(Math.Log(Price / Strike) + Rate * Time) * 2 / Time, 0.5); // initial value of volatility
            BSOption.IV = ComputedVolatility;

            if (check)
                ComputedValue = blsFutureCall();
            else
                ComputedValue = blsFuturePut();
            
            double Vega = blsvega();

            while (Math.Abs(Value - ComputedValue) > ACCURACY)
            {
                ComputedVolatility = ComputedVolatility - ((ComputedValue - Value) / Vega);
                if (ComputedVolatility<0)
                {
                    return 0;
                }
                BSOption.IV = ComputedVolatility;
                if (check == true)
                    ComputedValue = blsFutureCall();
                else
                    ComputedValue = blsFuturePut();

                Vega = blsvega();
            }

            return ComputedVolatility;
        }


        //Call pricer
        public double blsCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double Call = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            Call = Price * Math.Exp(-Yield * Time) * CND(d1) - Strike * Math.Exp(-Rate * Time) * CND(d2);
            
            return Call;
        }

        //Put pricer
        public double blsPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double Put = 0.0;


            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            Put = Strike * Math.Exp(-Rate * Time) * CND(-d2) - Price * Math.Exp(-Yield * Time) * CND(-d1);
            return Put;
        }

        //delta for Call
        public double blsdeltaCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));

            return Math.Exp(-Yield * Time) * CND(d1);
        }

        //delta for Put
        public double blsdeltaPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;
            double d1 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));

            return Math.Exp(-Yield * Time) * CND(d1) - 1;
        }

        //gamma is the same for Put and Call
        public double blsgamma()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));

            return Math.Exp(-Yield * Time) * phi(d1) / (Price * Volatility * Math.Sqrt(Time));
        }

        //vega is the same for Put and Call
        public double blsvega()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            return Price * Math.Exp(-Yield * Time) * phi(d1) * Math.Sqrt(Time);
        }

        //theta for Call
        public double blsthetaCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            return -Math.Exp(-Yield * Time) * (Price * phi(d1) * Volatility / (2 * Math.Sqrt(Time))) - Rate * Strike * Math.Exp(-Rate * Time) * CND(d2) + Yield * Price * Math.Exp(-Yield * Time) * CND(d1);
        }

        //theta for Put
        public double blsthetaPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            return -Math.Exp(-Yield * Time) * (Price * phi(d1) * Volatility / (2 * Math.Sqrt(Time))) + Rate * Strike * Math.Exp(-Rate * Time) * CND(-d2) - Yield * Price * Math.Exp(-Yield * Time) * CND(-d1);
        }

        //rho for Call
        public double blsrhoCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            return Strike * Time * Math.Exp(-Rate * Time) * CND(d2);
        }

        //rho for Put
        public double blsrhoPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            return -Strike * Time * Math.Exp(-Rate * Time) * CND(-d2);
        }

        //volga is the same for Call and Put
        public double blsvolga()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            return Price * Math.Exp(-Yield * Time) * phi(d1) * Math.Sqrt(Time) * d1 * d2 / Volatility;

        }

        //vanna is the same for Call and Put
        public double blsvanna()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double vanna = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            vanna = -Math.Exp(-Yield * Time) * phi(d1) * d2 / Volatility;

            return vanna;
        }

        //charm for Call
        public double blscharmCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double charmC = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            charmC = -Yield * Math.Exp(-Yield * Time) * CND(d1) + Math.Exp(-Yield * Time) * phi(d1) * (2 * (Rate - Yield) * Time - d2 * Volatility * Math.Sqrt(Time)) / (2 * Time * Volatility * Math.Sqrt(Time));
            return charmC;
        }

        //charm for Put
        public double blscharmPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double charmP = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            charmP = Yield * Math.Exp(-Yield * Time) * CND(-d1) - Math.Exp(-Yield * Time) * phi(d1) * (2 * (Rate - Yield) * Time - d2 * Volatility * Math.Sqrt(Time)) / (2 * Time * Volatility * Math.Sqrt(Time));
            return charmP;
        }

        //color is the same for Call and Put
        public double blscolor()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double color = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            color = -Math.Exp(-Yield * Time) * (phi(d1) / (2 * Price * Time * Volatility * Math.Sqrt(Time))) * (2 * Yield * Time + 1 + (2 * (Rate - Yield) * Time - d2 * Volatility * Math.Sqrt(Time)) * d1 / (2 * Time * Volatility * Math.Sqrt(Time)));
            return color;
        }

        //dual delta for Call
        public double blsdualdeltaCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double ddelta = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            ddelta = -Math.Exp(-Rate * Time) * CND(d2);
            return ddelta;
        }

        //dual delta for Put
        public double blsdualdeltaPut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;
            double d1 = 0.0;
            double d2 = 0.0;
            double ddelta = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);
            ddelta = Math.Exp(-Rate * Time) * CND(-d2);
            return ddelta;
        }

        //dual gamma is the same for Call and Put
        public double blsdualgamma()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double dgamma = 0.0;

            d1 = (Math.Log(Price / Strike) + (Rate - Yield + Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            dgamma = Math.Exp(-Rate * Time) * phi(d2) / (Strike * Volatility * Math.Sqrt(Time));
            return dgamma;
        }

        
        public double blsFutureCall()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double Call = 0.0;

            d1 = (Math.Log(Price / Strike) + (Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            Call = Price * Math.Exp(-Rate * Time) * CND(d1) - Strike * Math.Exp(-Rate * Time) * CND(d2);
            
            return Call;
        }

        //Put pricer
        public double blsFuturePut()
        {
            double Price = BSOption.UndPrice;
            double Strike = BSOption.Strike;
            double Rate = BSOption.Rate;
            double Time = (BSOption.Expiry.ToOADate() - BSOption.Now.ToOADate()) / (365.0);
            double Value = BSOption.ValLTP;
            double Volatility = BSOption.IV;
            double Yield = 0;
            Boolean check = BSOption.CallPut == TypeOfOption.CALL ? true : false;

            double d1 = 0.0;
            double d2 = 0.0;
            double Put = 0.0;

            d1 = (Math.Log(Price / Strike) + (Volatility * Volatility / 2.0) * Time) / (Volatility * Math.Sqrt(Time));
            d2 = d1 - Volatility * Math.Sqrt(Time);

            Put = Strike * Math.Exp(-Rate * Time) * CND(-d2) - Price * Math.Exp(-Rate * Time) * CND(-d1);
            return Put;
        }
               
    }
}
