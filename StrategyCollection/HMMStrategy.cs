using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;


namespace StrategyCollection
{
    public class HMMStrategy: BasicStrategy
    {
        public object TrainPeriod = 100;
        public object TestPeriod = 50;
        public object LBPeriod = 5;
        public object NumSysStates = 5;
        public object NumLevelsInp = 4;
        public object NumLevelsOP = 2;
        public object LevelMul = 0.01;
        
         
        public HMMStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int trainP = Convert.ToInt32(TrainPeriod);
            int testP = Convert.ToInt32(TestPeriod);
            int lbP = Convert.ToInt32(LBPeriod);
            int numSS = Convert.ToInt32(NumSysStates);
            int numLevels = Convert.ToInt32(NumLevelsInp);
            int numLevelsOP = Convert.ToInt32(NumLevelsOP);
            double levelMul = Convert.ToDouble(LevelMul);

            double[] edges = new double[numLevels];
            edges = edges.Select((x, i) => (0.5 + (double)i - (numLevels / 2.0)) * levelMul).ToArray();

            double[] edgesT;
            edgesT = numLevelsOP == 2 ? new double[] { 0.0 } : new double[] { -0.5 * levelMul, 0.5 * levelMul };
            
            for (int i = 0; i < numSec; i++)
            {
                int numPoints = data.InputData[i].Prices.Length;

                double[] stkROC = Technicals.ROC(data.InputData[i].Prices, 1);
                double[] stkFowROC = Technicals.ROCForward(data.InputData[i].Prices, 1);

                int[] stkHistSign = new int[numPoints];
                int[] stkPredictSign = new int[numPoints];

                for (int j = 0; j < stkROC.Length; j++)
                {
                    int idx1 = Array.BinarySearch(edges, Math.Round(stkROC[j], 4));
                    int idx2 = Array.BinarySearch(edgesT, Math.Round(stkFowROC[j], 4));
                    if (idx1 < 0)
                    {
                        idx1 = ~idx1;
                    }
                    if (idx2 < 0)
                    {
                        idx2 = ~idx2;
                    }
                    stkHistSign[j] = idx1;
                    stkPredictSign[j] = idx2;
                }

                int[] uni = stkHistSign.Distinct().ToArray();
                int[] uniC = stkPredictSign.Distinct().ToArray();                

                int symbols = uni.Length;
                int classes = uniC.Length;

                int[] states = UF.Double2Int(UF.MulArrayByConst(UF.Ones(uniC.Length), numSS));

                List<int[]> inputsList = new List<int[]>();
                List<int> outputList = new List<int>();

                for (int j = 0; j < lbP - 1; j++)
                {
                    inputsList.Add(new int[lbP]);
                    outputList.Add(0);
                }

                for (int j = lbP - 1; j < stkROC.Length; j++)
                {
                    int[] singleInput = UF.GetRange<int>(stkHistSign, j-lbP+1, j);

                    singleInput = singleInput.Select(x => x == -1 ? 0 : x).ToArray();
                    inputsList.Add(singleInput);

                    int singleOp = stkPredictSign[i] == -1 ? 0 : stkPredictSign[j];
                    outputList.Add(singleOp);
                }

                int[][] inputs = inputsList.ToArray();
                int[] outputs = outputList.ToArray();

                int iterations = 100;
                double limit = 0;

                SequenceClassifier hmmsc = new SequenceClassifier(classes, states, symbols);

                var teacher = new SequenceClassifierLearning(hmmsc, j =>
                    new BaumWelchLearning(hmmsc.Models[j])
                    {
                        Iterations = iterations,
                        Tolerance = limit
                    }
                );

                int[] predictedTest = new int[outputs.Length];
                
                for (int j = trainP; j < numPoints; j+=testP)
                {
                    int[] outTemp = UF.GetRange(outputs, j - trainP, j - 1);
                    int[][] inputTemp = UF.GetRange(inputs, j - trainP, j - 1);
                    teacher.Run(inputTemp, outTemp);

                    for (int k = j; k <= Math.Min(j + testP - 1, stkROC.Length - 1); k++)
                    {
                        double llh;
                        predictedTest[k] = hmmsc.Compute(inputs[k], out llh);                        
                    }
                }

                int max = predictedTest.Max();
                int min = predictedTest.Min();
                double val = (max + min) / 2.0;

                double[] sig = predictedTest.Select(x=>(double)x).ToArray();

                base.CalculateNetPosition(data, sig, i, val, val);                
            }
            base.RunStrategyBase(data);
        }
    }
}
