using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using System.IO;

namespace CommonLib
{
    public class StrategyData
    {
        public List<TimeSeries> InputData { get; set; }
        public TypeOfData DataType { get; set; }
        public List<string> SecName { get; set; }
        public int SelectedIndex { get; set; }
        public TypeOfSeries SeriesType { get; set; }
        public int[][] SecGrouping { get; set; }
        
        public StrategyData()
        {

        }

        public StrategyData(string fileName, TypeOfData dataType, TypeOfSeries ts, string grpfileName = null)
        {
            DataType = dataType;
            SeriesType = ts;

            if (DataType == TypeOfData.MULTISEC_SINGLEDATE)
            {
                FileRead fr = new FileRead(fileName);
                string[] colNames = fr.ReadFirstLine();
                double[,] data = fr.CSVDataExtractMultiVar(colNames.Length - 1, 1, 1);
                DateTime[] Dates = fr.CSVDataExtractFastOneVarDate(1, 1);
                InputData = new List<TimeSeries>();

                if (SeriesType == TypeOfSeries.LTP)
                {
                    for (int i = 0; i < colNames.Length - 1; i++)
                    {
                        TimeSeries temp = new TimeSeries(colNames[i + 1]);
                        temp.Dates = Dates;
                        temp.Prices = UF.Get_ith_col(data, i);
                        InputData.Add(temp);
                    }                    
                }
                else if (SeriesType == TypeOfSeries.BID_ASK_LTP)
                {
                    for (int i = 0; i < colNames.Length - 1; i+=3)
                    {
                        TimeSeries temp = new TimeSeries(colNames[i + 1]);
                        temp.Dates = Dates;
                        temp.Bid = UF.Get_ith_col(data, i);
                        temp.Ask = UF.Get_ith_col(data, i+1);
                        temp.Prices = UF.Get_ith_col(data, i+2);
                        InputData.Add(temp);
                    }                    
                }
                else if (SeriesType == TypeOfSeries.BID_ASK_LTP_VOL)
                {
                    for (int i = 0; i < colNames.Length - 1; i += 4)
                    {
                        TimeSeries temp = new TimeSeries(colNames[i + 1]);
                        temp.Dates = Dates;
                        temp.Bid = UF.Get_ith_col(data, i);
                        temp.Ask = UF.Get_ith_col(data, i + 1);
                        temp.Prices = UF.Get_ith_col(data, i + 2);
                        temp.Extra1 = UF.Get_ith_col(data, i + 3);
                        InputData.Add(temp);
                    }
                }
                else if (SeriesType == TypeOfSeries.OHLC)
                {
                    for (int i = 0; i < colNames.Length - 1; i += 4)
                    {
                        TimeSeries temp = new TimeSeries(colNames[i+1]);
                        temp.OHLC = new OHLCDataSet(Dates.Length);
                        temp.Dates = Dates;
                        temp.OHLC.dates = Dates;
                        temp.OHLC.open = UF.Get_ith_col(data, i);
                        temp.OHLC.high = UF.Get_ith_col(data, i + 1);
                        temp.OHLC.low = UF.Get_ith_col(data, i + 2);
                        temp.OHLC.close = UF.Get_ith_col(data, i + 3);
                        temp.Prices = temp.OHLC.close;
                        InputData.Add(temp);
                    }
                }
                else if (SeriesType == TypeOfSeries.OHLCV)
                {
                    for (int i = 0; i < colNames.Length - 1; i += 5)
                    {
                        TimeSeries temp = new TimeSeries(colNames[i+1]);
                        temp.OHLC = new OHLCDataSet(Dates.Length);
                        temp.Dates = Dates;
                        temp.OHLC.dates = Dates;
                        temp.OHLC.open = UF.Get_ith_col(data, i);
                        temp.OHLC.high = UF.Get_ith_col(data, i + 1);
                        temp.OHLC.low = UF.Get_ith_col(data, i + 2);
                        temp.OHLC.close = UF.Get_ith_col(data, i + 3);
                        temp.OHLC.volume = UF.Get_ith_col(data, i + 4);
                        temp.Prices = temp.OHLC.close;
                        InputData.Add(temp);
                    }
                }               
            }
            else if (DataType == TypeOfData.FILE_LIST)
            {
                FileRead fr = new FileRead(fileName);
                string[] fileNames = fr.CSVDataExtractStringArray();
                InputData = new List<TimeSeries>();

                for (int j = 0; j < fileNames.Length; j++)
                {
                    fr = new FileRead(fileNames[j]);
                    string[] colNames = fr.ReadFirstLine();
                    double[,] data = fr.CSVDataExtractMultiVar(colNames.Length - 1, 1, 1);
                    DateTime[] Dates = fr.CSVDataExtractFastOneVarDate(1, 1);

                    if (SeriesType == TypeOfSeries.LTP)
                    {
                        for (int i = 0; i < colNames.Length - 1; i++)
                        {
                            TimeSeries temp = new TimeSeries(colNames[i + 1]);
                            temp.Dates = Dates;
                            temp.Prices = UF.Get_ith_col(data, i);
                            InputData.Add(temp);
                        }
                    }
                    else if (SeriesType == TypeOfSeries.BID_ASK_LTP)
                    {
                        for (int i = 0; i < colNames.Length - 1; i += 3)
                        {
                            TimeSeries temp = new TimeSeries(colNames[i + 1]);
                            temp.Dates = Dates;
                            temp.Bid = UF.Get_ith_col(data, i);
                            temp.Ask = UF.Get_ith_col(data, i + 1);
                            temp.Prices = UF.Get_ith_col(data, i + 2);
                            InputData.Add(temp);
                        }
                    }
                    else if (SeriesType == TypeOfSeries.BID_ASK_LTP_VOL)
                    {
                        for (int i = 0; i < colNames.Length - 1; i += 4)
                        {
                            TimeSeries temp = new TimeSeries(colNames[i + 1]);
                            temp.Dates = Dates;
                            temp.Bid = UF.Get_ith_col(data, i);
                            temp.Ask = UF.Get_ith_col(data, i + 1);
                            temp.Prices = UF.Get_ith_col(data, i + 2);
                            temp.Extra1 = UF.Get_ith_col(data, i + 3);
                            InputData.Add(temp);
                        }
                    }
                    else if (SeriesType == TypeOfSeries.OHLC)
                    {
                        for (int i = 0; i < colNames.Length - 1; i += 4)
                        {
                            TimeSeries temp = new TimeSeries(colNames[i + 1]);
                            temp.OHLC = new OHLCDataSet(Dates.Length);
                            temp.Dates = Dates;
                            temp.OHLC.dates = Dates;
                            temp.OHLC.open = UF.Get_ith_col(data, i);
                            temp.OHLC.high = UF.Get_ith_col(data, i + 1);
                            temp.OHLC.low = UF.Get_ith_col(data, i + 2);
                            temp.OHLC.close = UF.Get_ith_col(data, i + 3);
                            temp.Prices = temp.OHLC.close;
                            InputData.Add(temp);
                        }
                    }
                    else if (SeriesType == TypeOfSeries.OHLCV)
                    {
                        for (int i = 0; i < colNames.Length - 1; i += 5)
                        {
                            TimeSeries temp = new TimeSeries(colNames[i + 1]);
                            temp.OHLC = new OHLCDataSet(Dates.Length);
                            temp.Dates = Dates;
                            temp.OHLC.dates = Dates;
                            temp.OHLC.open = UF.Get_ith_col(data, i);
                            temp.OHLC.high = UF.Get_ith_col(data, i + 1);
                            temp.OHLC.low = UF.Get_ith_col(data, i + 2);
                            temp.OHLC.close = UF.Get_ith_col(data, i + 3);
                            temp.OHLC.volume = UF.Get_ith_col(data, i + 4);
                            temp.Prices = temp.OHLC.close;
                            InputData.Add(temp);
                        }
                    }
                }
            }
            else if (DataType == TypeOfData.CUSTOM_OPTION_DATA)
            {
                StreamReader sr = new StreamReader(fileName);

                sr.ReadLine();
                List<DFMultiScrip> allSecData = new List<DFMultiScrip>();
                while(sr.Peek()>=0)
                {
                    DFMultiScrip df = new DFMultiScrip();
                    df.ParseLine(sr.ReadLine());
                    allSecData.Add(df);
                }
                sr.Close();

                string[] sec = allSecData.Select(x => x.Scrip).Distinct().OrderBy(x=>x).ToArray();
                InputData = new List<TimeSeries>();

                for (int i = 0; i < sec.Length; i++)
                {
                    TimeSeries timeSeries = new TimeSeries();
                    timeSeries.Name = sec[i];
                    List<DFMultiScrip> thisSec = allSecData.Where(x => x.Scrip == sec[i] 
                        && x.Date.TimeOfDay < TimeSpan.Parse("15:20:00") 
                        && x.Date.TimeOfDay > TimeSpan.Parse("09:50:00")).ToList();
                    timeSeries.Dates = thisSec.Select(x => x.Date).ToArray();
                    timeSeries.Bid = thisSec.Select(x => x.Values[0]).ToArray();
                    timeSeries.Ask = thisSec.Select(x => x.Values[1]).ToArray();
                    timeSeries.Prices = thisSec.Select(x => x.Values[2]).ToArray();
                    timeSeries.Extra1 = thisSec.Select(x => x.Values[3]).ToArray();
                    timeSeries.Extra2 = thisSec.Select(x => x.Values[4]).ToArray();
                    string Security = new string(timeSeries.Name.TakeWhile(x=>!Char.IsDigit(x)).ToArray());
                    string rem = timeSeries.Name.Replace(Security, "");
                    if(rem.Length > 0)
                    {
                        string datestr = rem.Substring(0,5);
                        DateTime expiry = DateTime.ParseExact(datestr + "01", "yyMMMdd", null);
                        rem = rem.Replace(datestr, "");
                        TypeOfOption to;
                        double strike = 0;
                        if (rem.Trim() == "FUT")
                        {
                            strike = 0;
                            to = TypeOfOption.FUT;
                        }
                        else
                        {
                            string stk = new string(rem.TakeWhile(x => Char.IsDigit(x)).ToArray());
                            strike = Convert.ToDouble(stk);
                            rem = rem.Replace(stk, "").ToUpper();
                            to = rem.Contains("C") ? TypeOfOption.CALL : rem.Contains("P") ?
                                TypeOfOption.PUT : TypeOfOption.FUT;
                        }

                        timeSeries.OptionDetails = new Option(Security,strike,expiry,to);
                    }
                    else
                    {
                        timeSeries.OptionDetails = new Option(Security,0.0,new DateTime(),TypeOfOption.FUT);
                    }

                    //timeSeries.OptionDetails = new Option(
                    InputData.Add(timeSeries);
                }
            }

            SecName = InputData.Select(x => x.Name).ToList();
            SelectedIndex = 0;

            if (grpfileName == null)
            {
                SecGrouping = new int[1][];
                SecGrouping[0] = Enumerable.Range(0, InputData.Count).ToArray();
            }
            else
            {
                //FileRead fr = new FileRead(grpfileName);
                //int[] col1 = fr.CSVDataExtractOneVarInt(1, 0);
                //int[] col2 = fr.CSVDataExtractOneVarInt(2, 0);
                //SecGrouping = new int[col1.Length][];                
            }
        }

        public StrategyData GetSelectedElements(int[] idx)
        {
            StrategyData st = new StrategyData();
            st.InputData = new List<TimeSeries>();
            st.SecName = new List<string>();

            st.DataType = this.DataType;

            for (int i = 0; i < idx.Length; i++)
            {
                st.InputData.Add(this.InputData[idx[i]]);
                st.SecName.Add(this.SecName[idx[i]]);
            }
            st.SelectedIndex = idx[0];

            st.SecGrouping = new int[1][];
            st.SecGrouping[0] = Enumerable.Range(0, st.InputData.Count).ToArray();       

            return st;
        }
    }

    public enum TypeOfData
    {
        MULTISEC_SINGLEDATE,
        MULTISEC_MULTIDATE,
        BLOOMBERG_FORMAT,
        CUSTOM_OPTION_DATA,
        FILE_LIST
    }

}
