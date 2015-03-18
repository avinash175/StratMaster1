using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace StrategyCollection
{
    public class ExampleStrategy : BasicStrategy
    {
        public object EMAPeriod = 20;
        public object LongAbove = 0.0;
        public object ShortBelow = 1.0;

        public ExampleStrategy(string stratName, double alloc, double cost, double timeStep)
            : base(stratName, alloc, cost, timeStep)
        {

        }

        public override void RunStrategy(StrategyData data)
        {
            int numSec = data.InputData.Count;
            double la = Convert.ToDouble(LongAbove);
            double sb = Convert.ToDouble(ShortBelow);
            int emaP = Convert.ToInt32(EMAPeriod);

            int idx = data.SecName.Select((x, i) => x == "NIFTY11AUGFUT" ? i : -1).Where(x => x >= 0).ToArray()[0];

            DateTime[] dates = data.InputData[idx].Dates;
            double[] prices = data.InputData[idx].Prices;

            FileRead fr = new FileRead("expiries.csv");

            DateTime[] expiry = fr.CSVDataExtractFastOneVarDate(1,0);

            double strike = Math.Round(prices[0] / 100, 2) * 100;
            int idx1 = 0;
            DateTime exp = expiry[0];

            for (int i = 0; i < data.InputData.Count; i++)
			{
			    NetPositions.Add(new int[data.InputData[i].Dates.Length]);
			}

            List<int> idxTrade = new List<int>();
            int idxCall = -1;
            int idxPut = -1;

            for (int i = 1; i < dates.Length; i++)
            {
                

                if (dates[i].Date != dates[i - 1].Date || i==1)
                {
                    idx1 = Array.BinarySearch(expiry, dates[i]);

                    if (idx1 < 0)
                    {
                        idx1 = ~idx1;
                    }

                    exp = expiry[idx1];
                    exp = exp.AddDays(-exp.Day + 1);

                    strike = Math.Round(prices[i] / 100, 2) * 100;

                    exp = new DateTime(2011, 8, 1);
                    strike = 4700.0;

                    TimeSeries call = data.InputData.Where(x => x.OptionDetails.Strike == strike && x.OptionDetails.Expiry == exp
                    && x.OptionDetails.CallPut == TypeOfOption.CALL).First();
                    
                    for (int j = 0; j < data.InputData.Count; j++)
                    {
                        if (call == data.InputData[j])
                        {
                            idxCall = j;
                            break;
                        }
                    }                    

                    TimeSeries put = data.InputData.Where(x => x.OptionDetails.Strike == strike && x.OptionDetails.Expiry == exp
                        && x.OptionDetails.CallPut == TypeOfOption.PUT).First();

                    for (int j = 0; j < data.InputData.Count; j++)
                    {
                        if (put == data.InputData[j])
                        {
                            idxPut = j;
                            break;
                        }
                    }
                }

                if(true)
                {
                    idxTrade.Add(idxCall);
                    idxTrade.Add(idxPut);
                }               

            }           

            base.RunStrategyBase(data);
        }
    }
}
