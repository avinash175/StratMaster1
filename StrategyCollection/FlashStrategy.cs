using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using DotFuzzy;

namespace StrategyCollection
{
    public class FlashStrategy : BasicStrategy
    {
        public object MAFastPeriod = 3;
        public object MASlowPeriod = 20;
        public object Band = 0.1;
        public object LSThresh = 0.0;
                           
        public FlashStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;

            int maSlowP = Convert.ToInt32(MASlowPeriod);
            int maFastP = Convert.ToInt32(MAFastPeriod);
            double band = Convert.ToDouble(Band);
            double lsThresh = Convert.ToDouble(LSThresh);
           
            LinguisticVariable crossOld = new LinguisticVariable("CrossOld");
            crossOld.MembershipFunctionCollection.Add(new MembershipFunction("Negative", -100.0, -100.0, -band, 0));
            crossOld.MembershipFunctionCollection.Add(new MembershipFunction("Zero", -band, 0, 0, band));
            crossOld.MembershipFunctionCollection.Add(new MembershipFunction("Positive", 0, band, 100.0, 100.0));

            LinguisticVariable crossNew = new LinguisticVariable("CrossNew");
            crossNew.MembershipFunctionCollection.Add(new MembershipFunction("Negative", -100.0, -100.0, -band, 0));
            crossNew.MembershipFunctionCollection.Add(new MembershipFunction("Zero", -band, 0, 0, band));
            crossNew.MembershipFunctionCollection.Add(new MembershipFunction("Positive", 0, band, 100.0, 100.0));

            LinguisticVariable patternH = new LinguisticVariable("PatternH");
            patternH.MembershipFunctionCollection.Add(new MembershipFunction("Negative", -100.0, -100.0, -band, 0));
            patternH.MembershipFunctionCollection.Add(new MembershipFunction("Zero", -band, 0, 0, band));
            patternH.MembershipFunctionCollection.Add(new MembershipFunction("Positive", 0, band, 100.0, 100.0));

            LinguisticVariable patternL = new LinguisticVariable("PatternL");
            patternL.MembershipFunctionCollection.Add(new MembershipFunction("Negative", -100.0, -100.0, -band, 0));
            patternL.MembershipFunctionCollection.Add(new MembershipFunction("Zero", -band, 0, 0, band));
            patternL.MembershipFunctionCollection.Add(new MembershipFunction("Positive", 0, band, 100.0, 100.0));

            LinguisticVariable trade = new LinguisticVariable("Trade");
            trade.MembershipFunctionCollection.Add(new MembershipFunction("Short", -1.0, -1.0, -0.5, 0));
            trade.MembershipFunctionCollection.Add(new MembershipFunction("NoTrade", -0.5, 0, 0, 0.5));
            trade.MembershipFunctionCollection.Add(new MembershipFunction("Long", 0, 0.5, 1.0, 1.0));

            FuzzyEngine fuzzyEngine = new FuzzyEngine();
            fuzzyEngine.LinguisticVariableCollection.Add(crossOld);
            fuzzyEngine.LinguisticVariableCollection.Add(crossNew);
            fuzzyEngine.LinguisticVariableCollection.Add(patternH);
            fuzzyEngine.LinguisticVariableCollection.Add(patternL);
            fuzzyEngine.LinguisticVariableCollection.Add(trade);
            fuzzyEngine.Consequent = "Trade";

            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Negative) AND (CrossNew IS Positive)"+
                " AND (PatternH IS Positive) THEN Trade IS Long"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Positive) AND (CrossNew IS Negative)" +
                " AND (PatternL IS Negative) THEN Trade IS Short"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Zero) OR (CrossNew IS Zero)" +
                " THEN Trade IS NoTrade"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Positive) OR (CrossNew IS Positive)" +
                " THEN Trade IS NoTrade"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Negative) OR (CrossNew IS Negative)" +
                " THEN Trade IS NoTrade"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Negative) AND (CrossNew IS Positive)" +
                " AND (PatternH IS Negative) THEN Trade IS NoTrade"));
            fuzzyEngine.FuzzyRuleCollection.Add(new FuzzyRule("IF (CrossOld IS Positive) AND (CrossNew IS Negative)" +
                " AND (PatternL IS Positive) THEN Trade IS NoTrade"));
                        
            if (data.SeriesType == TypeOfSeries.OHLC ||
                data.SeriesType == TypeOfSeries.OHLCV)
            {
                // Assuming lower time frame data
                for (int i = 0; i < numSec; i++)
                {
                    double[] maSlow = Technicals.MovAvg(data.InputData[i].Prices, maSlowP);
                    double[] maFast = Technicals.MovAvg(data.InputData[i].Prices, maFastP);

                    double[] diff = maSlow.Select((x, j) => (maFast[j] - x)/x).ToArray();
                         
                    double[] sig = new double[data.InputData[i].Dates.Length];

                    for (int j = 2; j < sig.Length; j++)
                    {
                        crossOld.InputValue = diff[j - 2];
                        crossNew.InputValue = diff[j - 1];
                        patternH.InputValue = data.InputData[i].OHLC.high[j] - data.InputData[i].OHLC.high[j - 1];
                        patternL.InputValue = data.InputData[i].OHLC.low[j] - data.InputData[i].OHLC.low[j - 1];
                        double signal = fuzzyEngine.Defuzzify();
                        sig[j] = Double.IsNaN(signal)  ? 0 : signal;
                    }               

                    base.CalculateNetPosition(data, sig, i,lsThresh, -lsThresh);                    
                }
            }
            else
            {
                throw new Exception("Use OHLC data for this strategy");
            }

            base.RunStrategyBase(data);
        }
    }
}

