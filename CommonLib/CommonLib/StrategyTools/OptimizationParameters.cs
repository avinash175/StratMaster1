using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class OptimizationParameters
    {
        public string StrategyName { get; set; }
        public List<OptimizationParamAttributes> Parameters { get; set; }
        public List<List<ParamValue>> AllCombinations { get; set; }

        public OptimizationParameters(string strategyName)
        {
            StrategyName = strategyName;
            Parameters = new List<OptimizationParamAttributes>();
        }

        public void GenerateAllCombinations()
        {            
            ParamValue[][] OneColl = new ParamValue[Parameters.Count][];
            
            for (int i = 0; i < Parameters.Count; i++)
            {
                double range = Parameters[i].Max - Parameters[i].Min;
                int cnt = (int)Math.Ceiling(range / Parameters[i].StepSize) + 1;
                
                ParamValue[] paramValues = new ParamValue[cnt];
                paramValues[0] = new ParamValue(Parameters[i].ParamName, Parameters[i].Min);        
                
                for (int j = 1; j < cnt - 1; j++)
                {
                    paramValues[j] = new ParamValue(Parameters[i].ParamName, Parameters[i].Min + j*Parameters[i].StepSize);
                }

                paramValues[cnt - 1] = new ParamValue(Parameters[i].ParamName,Parameters[i].Max);
                OneColl[i] = paramValues;
            }
            AllCombinations = Prod(OneColl);            
        }
        
        private List<List<ParamValue>> Prod(ParamValue[][] oneColl)
        {
            List<List<ParamValue>> ret = new List<List<ParamValue>>();
            for (int i = 0; i < oneColl[0].Length; i++)
            {
                List<ParamValue> temp = new List<ParamValue>();
                temp.Add(oneColl[0][i]);
                ret.Add(temp);
            }
            
            for (int k = 1; k < oneColl.Length; k++)
            {
                int prevCount = ret[0].Count;
                int cnt = ret.Count;
                for (int i = 0; i < cnt; i++)
                {
                    for (int j = 0; j < oneColl[k].Length; j++)
                    {
                        List<ParamValue> temp = new List<ParamValue>(ret[i]);
                        temp.Add(oneColl[k][j]);
                        ret.Add(temp);
                    }
                }
                ret = ret.Where(x => x.Count > prevCount).ToList();
            }
            return ret;           
        }

        public List<List<ParamValue>> FindStableRegion(List<List<ParamValue>> bestParams)
        {
            double[] range = Parameters.Select(x => x.Max - x.Min).ToArray();
            double[][] pars = bestParams.Select(x => UF.ArrayDiv(x.Select(y => y.Value)
                .ToArray(), range)).ToArray();
            int[] clusters = Kmeans.Classify(pars, 4, DistanceType.Eucledian);
            int largestClass = clusters.GroupBy(x => x).Select(x => new { Key = x.Key, Count = x.Count() })
                .OrderBy(x => x.Count).Last().Key;
            int[] stableIdx = clusters.Select((x, i) => new { Idx = i, Val = x }).
                Where(x => x.Val == largestClass).Select(x => x.Idx).ToArray();
            return bestParams.Where((x, i) => stableIdx.Contains(i)).ToList();
        }
    }

    public class OptimizationParamAttributes
    {
        public string ParamName { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double StepSize { get; set; }
    }

    public class ParamValue
    {
        public string ParamName { get; set; }
        public double Value { get; set; }

        public ParamValue(string paramName, double value)
        {
            ParamName = paramName;
            Value = value;
        }

        public override string ToString()
        {
            return ParamName +"="+Value.ToString();
        }
    }
}
