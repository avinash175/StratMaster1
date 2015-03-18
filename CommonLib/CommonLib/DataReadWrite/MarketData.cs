using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;

namespace CommonLib
{
    public class MarketData
    {
        public static double[] GetClosingFutPx(string[] stkNames, DateTime date, DateTime MatDate)
        {
            string matStr = "26-Aug-10";
            string url = "http://www.nseindia.com/content/historical/DERIVATIVES/" + date.ToString("yyyy") +
                "/" + date.ToString("MMM").ToUpper() + "/fo" + date.ToString("ddMMMyyyy").ToUpper() + "bhav.csv.zip";

            string fileName = "fo" + date.ToString("ddMMMyyyy") + "bhav.csv";
            WebDataClass wc = new WebDataClass("10.15.1.2", 8080, "shivaram", "edelcap@2");
            wc.DownloadFromURL3(url, fileName);
            FileRead fr = new FileRead(fileName);
            double[] ClosingPx = fr.CSVDataExtractOneVar(9, 1);
            DateTime[] Mat = fr.CSVDataExtractOneVarDate(3, 1);
            string[] Sec = fr.CSVDataExtractStringArray(2, 1);
            
            double[] px = new double[stkNames.Length];
            int cnt = 0;
            foreach (string s in stkNames)
            {
                for (int i = 0; i < Mat.Length; i++)
                {
                    if (Mat[i].ToString("dd-MMM-yy") == matStr && Sec[i] == s)
                    {
                        px[cnt] = ClosingPx[i];
                        cnt++;
                        break;
                    }
                }
            }

            File.Delete(fileName);
            return px;
        }

        public static NIFTYCashEODInfo GetNIFTYCashEODInfo(DateTime day, double NIFTYClose)
        {
            int NumStks = 50;
            NIFTYCashEODInfo res = new NIFTYCashEODInfo();

            string fileName = "ffix" + day.ToString("ddMMyy") + ".csv";
            string url = "http://www.nseindia.com/archives/ix/Ix" + day.ToString("ddMMyy") + ".zip";
            WebDataClass wc = new WebDataClass("10.15.1.2", 8080, "shivaram", "edelcap@2");
            wc.DownloadFromURL3(url, fileName);

            FileRead fr = new FileRead(fileName);
            res.StkNames = fr.CSVDataExtractStringArray(2, 3);
            res.StkNames = UF.GetRange(res.StkNames, 0, NumStks - 1);

            res.FreeFltMarketCap = fr.CSVDataExtractOneVarEndSoon(8, 3);
            res.ClosingPx = fr.CSVDataExtractOneVarEndSoon(7, 3);
            double InitialMarCap = UF.SumArray(res.FreeFltMarketCap) / NIFTYClose;

            res.FreeFltSharesPerNifty = UF.ArrayDiv(res.FreeFltMarketCap, UF.MulArrayByConst(res.ClosingPx, InitialMarCap));
                       
            File.Delete(fileName);
            string otherFile = "Ix" + day.ToString("ddMMyy") + ".csv";
            File.Delete(otherFile);
            
            return res;
        }

        public static NIFTYCashEODInfo GetNIFTYCashEODInfoNew(DateTime day, double NIFTYClose)
        {
            int NumStks = 50;
            NIFTYCashEODInfo res = new NIFTYCashEODInfo();

            string destDir = Directory.GetCurrentDirectory() + "\\Data\\MarketCap\\";
            string fileName = "ffix" + day.ToString("ddMMyy") + ".csv";                  

            FileRead fr = new FileRead(destDir+fileName);
            res.StkNames = fr.CSVDataExtractStringArray(2, 3);
            res.StkNames = UF.GetRange(res.StkNames, 0, NumStks - 1);

            res.FreeFltMarketCap = fr.CSVDataExtractOneVarEndSoon(8, 3);
            res.ClosingPx = fr.CSVDataExtractOneVarEndSoon(7, 3);
            double InitialMarCap = UF.SumArray(res.FreeFltMarketCap) / NIFTYClose;

            res.FreeFltSharesPerNifty = UF.ArrayDiv(res.FreeFltMarketCap, UF.MulArrayByConst(res.ClosingPx, InitialMarCap));
            return res;
        }

        public static NIFTYCashEODInfo GetNIFTYCashEODInfoNewBN(DateTime day, double NIFTYClose)
        {            
            NIFTYCashEODInfo res = new NIFTYCashEODInfo();

            string destDir = Directory.GetCurrentDirectory() + "\\Data\\MarketCap\\";
            string fileName = "ffix" + day.ToString("ddMMyy") + ".csv";

            StreamReader sr = new StreamReader(destDir + fileName);

            string line = sr.ReadLine();
            string[] fields = line.Split(',');

            while (fields[3].Trim() != "BANK Nifty")
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                if (sr.Peek() < 0)
                    break;
            }

            sr.ReadLine();
            List<string> stkNames = new List<string>();
            List<double> clpxs = new List<double>();
            List<double> freefltMcap = new List<double>();

            line = sr.ReadLine();
            fields = line.Split(',');

            while (fields[0].Trim() != "")
            {                
                stkNames.Add(fields[1]);
                clpxs.Add(Double.Parse(fields[6]));
                freefltMcap.Add(Double.Parse(fields[7]));
                line = sr.ReadLine();
                fields = line.Split(',');
                if (sr.Peek() < 0)
                    break;
            }

            sr.Close();

            FileRead fr = new FileRead(destDir + fileName);

            res.StkNames = new string[stkNames.Count];
            stkNames.CopyTo(res.StkNames);

            res.FreeFltMarketCap = new double[freefltMcap.Count];
            freefltMcap.CopyTo(res.FreeFltMarketCap); 

            res.ClosingPx = new double[clpxs.Count];
            clpxs.CopyTo(res.ClosingPx);
            
            double InitialMarCap = UF.SumArray(res.FreeFltMarketCap) / NIFTYClose;

            res.FreeFltSharesPerNifty = UF.ArrayDiv(res.FreeFltMarketCap, UF.MulArrayByConst(res.ClosingPx, InitialMarCap));
            return res;
        }

        public static void DownloadDataFromNSE(DateTime FromDate, DateTime ToDate)
        {
            DateTime[] AnalysisDates;
            FileRead fr = new FileRead("NIFTYCash.csv");
            DateTime[] tempDates = fr.CSVDataExtractFastOneVarDate(1, 1);
            AnalysisDates = UF.GetAllDates(FromDate, ToDate);
            AnalysisDates = NF.IntersectSortedDateTimes(AnalysisDates, tempDates);
            int NumDates = AnalysisDates.Length;

            for (int i = 0; i < NumDates; i++)
            {
                DateTime day = AnalysisDates[i];
                string zipFile = "Ix" + day.ToString("ddMMyy") + ".zip";
                string fileName = "ffix" + day.ToString("ddMMyy") + ".csv";
                string otherFile = "Ix" + day.ToString("ddMMyy") + ".csv";
                string url = "http://www.nseindia.com/archives/ix/Ix" + day.ToString("ddMMyy") + ".zip";

                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                ProcessStartInfo si = new ProcessStartInfo("iexplore", url);
                si.WindowStyle = ProcessWindowStyle.Hidden;
                si.CreateNoWindow = true;
                Process browser1 = Process.Start(si);

                browser1.WaitForExit();                

                FastZip fz = new FastZip();
                string destDir =  Directory.GetCurrentDirectory()+"\\Data\\MarketCap\\";
                fz.ExtractZip(zipFile,destDir, "");

                File.Delete(zipFile);
                File.Delete(destDir + otherFile);
            }
        }

        public static void DownloadDataFromNSE(DateTime Date)
        {
            DateTime[] AnalysisDates = new DateTime[1];
            AnalysisDates[0] = Date;            
            
            int NumDates = AnalysisDates.Length;

            for (int i = 0; i < NumDates; i++)
            {
                DateTime day = AnalysisDates[i];
                string zipFile = "Ix" + day.ToString("ddMMyy") + ".zip";
                string fileName = "ffix" + day.ToString("ddMMyy") + ".csv";
                string otherFile = "Ix" + day.ToString("ddMMyy") + ".csv";
                string url = "http://www.nseindia.com/archives/ix/Ix" + day.ToString("ddMMyy") + ".zip";

                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                ProcessStartInfo si = new ProcessStartInfo("iexplore", url);
                si.WindowStyle = ProcessWindowStyle.Hidden;
                si.CreateNoWindow = true;
                Process browser1 = Process.Start(si);

                browser1.WaitForExit();

                FastZip fz = new FastZip();
                string destDir = Directory.GetCurrentDirectory();
                fz.ExtractZip(zipFile, destDir, "");

                File.Delete(zipFile);
                File.Delete(otherFile);
            }
        }

        public static void AdjustForCorporateAction(DateTime day, DateTime exp, ref double[] Px, string[] AllStks)
        {
            FileRead fr = new FileRead("CorporateAction.csv");
            DateTime[] dates = fr.CSVDataExtractOneVarDate(1, 1);
            string[] StkNames = fr.CSVDataExtractStringArray(2, 1);
            double[] AdjFac = fr.CSVDataExtractOneVar(3, 1);

            for (int i = 0; i < dates.Length; i++)
            {
                DateTime temp1 = dates[i];
                if (day.Date < dates[i].Date && dates[i].Date <= exp.Date)
                {
                    int idx = Array.BinarySearch<string>(AllStks, StkNames[i]);
                    if (idx > 0)
                        Px[idx] = Px[idx] * AdjFac[i];
                }
            }
        }

        public static NIFTYCashEODInfo GetNIFTYCashEODInfoFromFile(string fileName)
        {
            //int NumStks = 50;
            NIFTYCashEODInfo res = new NIFTYCashEODInfo();            

            FileRead fr = new FileRead(fileName);

            string[] stockNames = fr.CSVDataExtractStringArray(2, 2);
            double[] issueCap = fr.CSVDataExtractOneVar(4, 2);
            double[] freefltfac = fr.CSVDataExtractOneVar(5, 2);
            double[] cashClose = fr.CSVDataExtractOneVar(6, 2);
            double[] LotSizes = fr.CSVDataExtractOneVar(7, 2);
            double NIFTYClose = fr.CSVDataExtractOneVar(6, 1)[0];
            double[] freefltMakCap = UF.ArrayProduct(UF.ArrayProduct(issueCap, freefltfac), cashClose);
            
            for (int i = 0; i < stockNames.Length; i++)
            {
                stockNames[i] = stockNames[i].Replace("-EQ", "").Trim();
            }

            int[] idx = UF.BubbleSortIdx(stockNames, true);
            res.StkNames = UF.GetIndexVals(stockNames, idx);
            res.FreeFltMarketCap = UF.GetIndexVals(freefltMakCap, idx);
            res.ClosingPx = UF.GetIndexVals(cashClose, idx);
            res.NIFTYClose = NIFTYClose;
            res.LotSizes = UF.GetIndexVals(LotSizes, idx);

            double InitialMarCap = UF.SumArray(res.FreeFltMarketCap) / NIFTYClose;
            res.FreeFltSharesPerNifty = UF.ArrayDiv(res.FreeFltMarketCap, UF.MulArrayByConst(res.ClosingPx, InitialMarCap));

            return res;
        }

    }

    public class NIFTYCashEODInfo
    {
        public string[] StkNames;
        public double[] ClosingPx;
        public double[] FreeFltMarketCap;
        public double[] FreeFltSharesPerNifty;
        public double[] LotSizes;
        public double NIFTYClose;
    }
}
