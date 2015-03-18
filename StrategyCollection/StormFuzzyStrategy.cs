using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using DotFuzzy;

namespace StrategyCollection
{
    public class StormFuzzyStrategy : BasicStrategy
    {
        public object LBWindowSmall = 10;
        public object LBWindowLarge = 100;
        public object LBWindowBrkOut = 5;
        public object CalmThreshMul = 0.6;        

        public StormFuzzyStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int lbs = Convert.ToInt32(LBWindowSmall);
            int lbl = Convert.ToInt32(LBWindowLarge);
            int lbbkt = Convert.ToInt32(LBWindowBrkOut);            
            double ctm = Convert.ToDouble(CalmThreshMul);
            
            if (data.SeriesType == TypeOfSeries.OHLC
                || data.SeriesType == TypeOfSeries.OHLCV)
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] atrS = Technicals.ATRSeries(data.InputData[i].OHLC, lbs);
                    double[] atrL = Technicals.ATRSeries(data.InputData[i].OHLC, lbl);
                    
                    int[] CalmNess = atrS.Select((x, j) => x < ctm * atrL[j] ? 1 : 0).ToArray();
                    int[] BrkOutInd = Technicals.BreakOutIndicator(data.InputData[i].Prices, lbbkt);

                    double[] Sig = CalmNess.Select((x, j) => (double)x * BrkOutInd[j]).ToArray();

                    base.CalculateNetPosition(data, Sig, i); 
                }
                base.RunStrategyBase(data);
            }
            else
            {
                for (int i = 0; i < numSec; i++)
                {
                    double[] ret = UF.Append(UF.Convert2Returns(data.InputData[i].Prices,1),0.0,false);
                    double[] atrS = Technicals.EWMAVol(ret, 2.0/(1+lbs));
                    double[] atrL = Technicals.EWMAVol(ret, 2.0/(1+lbl));

                    int[] CalmNess = atrS.Select((x, j) => x < ctm * atrL[j] ? 1 : 0).ToArray();
                    int[] BrkOutInd = Technicals.BreakOutIndicator(data.InputData[i].Prices, lbbkt);

                    double[] Sig = CalmNess.Select((x, j) => (double)x * BrkOutInd[j]).ToArray();

                    base.CalculateNetPosition(data, Sig, i);
                }
                base.RunStrategyBase(data);
            }            
        }
    }
}
