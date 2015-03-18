using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class GravityStrategy: BasicStrategy
    {
        public object ShortPeriod = 5;
        public object LongPeriod = 20;
        public object VelocityMul = 0.5;

        public GravityStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            int lp = Convert.ToInt32(LongPeriod);
            int sp = Convert.ToInt32(ShortPeriod);
            double vMul = Convert.ToDouble(VelocityMul);

            for (int i = 0; i < numSec; i++)
            {
                double[] velShort = Technicals.ROC(data.InputData[i].Prices, sp).
                    Select(x => x / (100 * sp * TimeInterval)).ToArray();
                double[] velLong1 = Technicals.ROC(data.InputData[i].Prices, lp).
                    Select(x => x / (100 * lp * TimeInterval)).ToArray();

                double[] accShort = Technicals.MovAvg(Technicals.Change(velShort, 1),sp)
                    .Select(x => x / TimeInterval).ToArray();
                double[] accLong1 = Technicals.MovAvg(Technicals.Change(velLong1, 1), lp)
                    .Select(x => x / TimeInterval).ToArray();

                double[] velLong = new double[velLong1.Length];
                double[] accLong = new double[accLong1.Length];

                for (int j = 0; j < data.InputData[i].Dates.Length; j++)
                {
                    if (j >= sp)
                    {
                        velLong[j] = velLong1[j - sp];
                        accLong[j] = accLong1[j - sp];
                    }
                }

                double[] sig = velShort.Select((x, j) => (j > sp && x < 0 && sp * Math.Abs(x) < lp * vMul * Math.Abs(velLong[j])
                    && accShort[j] < 0 && accLong[j] > 0) ? -1.0 : (j > sp && x > 0 && sp * Math.Abs(x) < lp * vMul * Math.Abs(velLong[j])
                    && accShort[j] > 0 && accLong[j] < 0) ? 1.0 : 0).ToArray();

                base.CalculateNetPosition(data, sig, i);                
            }

            base.RunStrategyBase(data);
        }
    }
}
