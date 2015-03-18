using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Globalization;
//using Microsoft.Office.Interop.Excel;
//using Excel;

namespace CommonLib
{
    public class FileRead
    {
        private StreamReader sr;
        public string fileName;

        public FileRead()
        {
           
        }

        public FileRead(string fileRead)
        {
            try
            {
                fileName = fileRead;
                //sr = new StreamReader(fileName);
            }
            catch
            {
                throw new Exception("Try running the program closing the open files");
            }
        }

        public void SetFileName(string file)
        {
            fileName = file;
        }

        // This function reads a CSV file and takes the 'fieldNum' field into a double array
        public double[] CSVDataExtractOneVar(int fieldNum)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            //ArrayList data = new ArrayList();
            double[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }            
            sr.Close();
            ret=new double[data.Count];
            data.CopyTo(ret);            
            return ret;
        }       

        // This function reads a CSV file and takes the 'fieldNum' field into a double array
        // and skips first 'N' lines
        public double[] CSVDataExtractOneVar(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            double[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    if (fields[fieldNum - 1].Trim() != "")
                    {
                        data.Add(Double.Parse(fields[fieldNum - 1]));
                    }
                    else
                    {
                        data.Add(0.0);
                    }
                }
                catch
                {
                    continue;
                    //break;
                }
            }

            sr.Close();
            ret = new double[data.Count];
            data.CopyTo(ret); 

            return ret;
        }

        public double[] CSVDataExtractOneVarEndSoon(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            double[] ret;
                        
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                try
                {
                    if (fields[fieldNum - 1].Trim() != "")
                    {
                        data.Add(Double.Parse(fields[fieldNum - 1]));
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    
                    break;
                }
            }

            sr.Close();
            ret = new double[data.Count];
            data.CopyTo(ret);           

            return ret;
        }

        public int[] CSVDataExtractOneVarInt(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<int> data = new List<int>();
            int[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    if (fields[fieldNum - 1].Trim() != "")
                    {
                        data.Add(Int32.Parse(fields[fieldNum - 1]));
                    }
                    else
                    {
                        data.Add(0);
                    }
                }
                catch
                {
                    continue;
                    //break;
                }
            }

            sr.Close();
            ret = new int[data.Count];
            data.CopyTo(ret);

            return ret;
        }

        public DateTime[] CSVDataExtractOneVarDate(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<DateTime> data = new List<DateTime>();
            DateTime[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    data.Add(DateTime.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            ret = new DateTime[data.Count];
            data.CopyTo(ret);

            return ret;
        }

        public DateTime[] CSVDataExtractFastOneVarDate(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            DateTime[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            ret = new DateTime[data.Count];
            for (i = 0; i < data.Count; i++)
            {
                ret[i] = DateTime.FromOADate(data[i]);
            }

            return ret;
        }


        public DateTime[] CSVDataExtractFastOneVarDate2(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            DateTime[] ret;

            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                try
                {
                    data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    break;
                }
            }

            sr.Close();
            ret = new DateTime[data.Count];
            for (i = 0; i < data.Count; i++)
            {
                ret[i] = DateTime.FromOADate(data[i]);
            }

            return ret;
        }

        // Copy of the above function with 'break' in 'catch'
        public double[] CSVDataExtractOneVar2(int fieldNum, int N, string Datastr)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            double[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[250];
            //sr = new StreamReader(fileName);
            Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(Datastr));
            sr = new StreamReader(s);

            for (i = 0; i < N; i++)
                line = sr.ReadLine();

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 250);
                try
                {
                    if (fields[fieldNum - 1].ToUpper() == "NAN")
                        break;
                    data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    //continue;
                    break;
                }
            }

            sr.Close();
            ret = new double[data.Count];
            data.CopyTo(ret);
            //for (i = 0; i < data.Count; i++)
            //{
            //    ret[i] = (double)data[i];
            //}
            return ret;
        }
                
        // doesn't use the file but instead uses Datastr as the file.
        public double[] CSVDataExtractOneVar(int fieldNum, string Datastr)
        {
            int i;
            string line;
            string[] fields;
            List<double> data = new List<double>();
            double[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[300];
            Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(Datastr));
            sr = new StreamReader(s);
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 300);
                try
                {
                    data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            ret = new double[data.Count];
            data.CopyTo(ret);

            return ret;
        }

        // This function reads a CSV file and takes 'numOfFields' field into a double matrix
        public double[,] CSVDataExtractMultiVar(int numOfFields, int NumSkip)
        {
            int i,j,fieldNum;
            string line;
            string[] fields;
            ArrayList data = new ArrayList();
            double[,] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[300];
            sr = new StreamReader(fileName);
            for (i = 0; i < NumSkip; i++)
            {
                line = sr.ReadLine();
            }

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                try
                {
                    for (fieldNum = 1; fieldNum <= numOfFields; fieldNum++)
                        data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            int rows = (int)data.Count / numOfFields;
            ret = new double[rows, numOfFields];
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j < numOfFields; j++)
                {
                    ret[i, j] = (double)data[i * numOfFields + j];
                }
            }

            return ret;
        }

        public double[,] CSVDataExtractMultiVar(int numOfFields, int NumSkipRow, int NumSkipCol)
        {
            int i, j, fieldNum;
            string line;
            string[] fields;
            ArrayList data = new ArrayList();
            double[,] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[300];
            sr = new StreamReader(fileName);
            for (i = 0; i < NumSkipRow; i++)
            {
                line = sr.ReadLine();
            }

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                try
                {
                    for (fieldNum = NumSkipCol+1; fieldNum < numOfFields+NumSkipCol+1; fieldNum++)
                        data.Add(Double.Parse(fields[fieldNum - 1]));
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            int rows = (int)data.Count / numOfFields;
            ret = new double[rows, numOfFields];
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j < numOfFields; j++)
                {
                    ret[i, j] = (double)data[i * numOfFields + j];
                }
            }

            return ret;
        }

        // This function reads a CSV file and takes 'numOfFields' field into a double matrix
        public string[,] CSVDataExtractMultiVarSting(int numOfFields, int NumSkip)
        {
            int i, j, fieldNum;
            string line;
            string[] fields;
            List<string> data = new List<string>();
            string[,] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[300];
            sr = new StreamReader(fileName);
            for (i = 0; i < NumSkip; i++)
            {
                line = sr.ReadLine();
            }

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(',');
                try
                {
                    for (fieldNum = 1; fieldNum <= numOfFields; fieldNum++)
                        data.Add(fields[fieldNum - 1]);
                }
                catch
                {
                    continue;
                }
            }

            sr.Close();
            int rows = (int)data.Count / numOfFields;
            ret = new string[rows, numOfFields];
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j < numOfFields; j++)
                {
                    ret[i, j] = data[i * numOfFields + j];
                }
            }

            return ret;
        }

        //This function reads a CSV file and converts the 'fieldNum' field into string array
        public string[] CSVDataExtractStringArray(int fieldNum)
        {
            int i;
            string line;
            string[] fields;
            List<string> data = new List<string>();
            string[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                data.Add(fields[fieldNum - 1]);
            }

            sr.Close();
            ret = new string[data.Count];
            data.CopyTo(ret);

            return ret;
        }

        //This function reads a CSV file and converts the 'fieldNum' field into string array
        // it ignores first 'N' lines
        public string[] CSVDataExtractStringArray(int fieldNum, int N)
        {
            int i;
            string line;
            string[] fields;
            List<string> data = new List<string>();
            string[] ret;

            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';
            fields = new string[100];
            sr = new StreamReader(fileName);

            for (i = 0; i < N; i++)
            {
                line = sr.ReadLine();
            }

            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 100);
                try
                {
                    data.Add(fields[fieldNum - 1]);
                }
                catch
                {
                    break;
                }
            }

            sr.Close();
            ret = new string[data.Count];
            data.CopyTo(ret);

            return ret;
        }

        //Takes in the entire row as the string array

        public string[] CSVDataExtractStringArray()
        {
            int i;
            
            ArrayList data = new ArrayList();
            string[] ret;          
            
            sr = new StreamReader(fileName);
            
            
            while (sr.Peek() >= 0)
            {

                 data.Add(sr.ReadLine());
            }

            sr.Close();
            ret = new string[data.Count];
            for (i = 0; i < data.Count; i++)
            {
                ret[i] = (string)data[i];
            }

            return ret;
        }

        public static int GetNumOfCol(string FileName)
        {
            int n = 0;            
            string line;
            string[] fields;
            
            char[] delimiter = new char[] {','};            
            
            StreamReader sr1 = new StreamReader(FileName);
            line = sr1.ReadLine();
            fields = line.Split(delimiter, 1000);

            n = fields.Length;
            sr1.Close();
            return n;
        }

        public string ReadRaw()
        {
            sr = new StreamReader(fileName);
            string output = sr.ReadToEnd();
            sr.Close();
            return output;
        }

        public string[] ReadFirstLine()
        {
            sr = new StreamReader(fileName);
            string[] output = sr.ReadLine().Split(',');
            sr.Close();
            return output;
        }

        public static List<string[]> Read(string fileName, int numSkip = 0, char delimiter = ',')
        {            
            string line;
            string[] fields;
            List<string[]> data = new List<string[]>();            
            
            StreamReader sr1 = new StreamReader(fileName);

            for (int i = 0; i < numSkip; i++)
            {
                line = sr1.ReadLine();
            }

            while (sr1.Peek() >= 0)
            {
                line = sr1.ReadLine();
                fields = line.Split(delimiter);               
                data.Add(fields);                
            }

            sr1.Close();
            return data;
        }



        // special purpose...
        public TimeStampData[] ReadTradeFile()
        {
            int i=0,j;
            ArrayList dataArray = new ArrayList();
            TimeStampData[] ret;
            
            string line;
            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = '|';
            string[] fields = new string[10];
            sr = new StreamReader(fileName);
            string datestr = fileName.Substring(fileName.Length - 12, 8);
            datestr = datestr.Substring(4, 2) + "/" + datestr.Substring(6, 2) + "/" + datestr.Substring(0, 4);
            DateTime day = DateTime.Parse(datestr);

            while (sr.Peek() >= 0.0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter, 10);
                TimeStampData temp = new TimeStampData();
                temp.indexName = fields[1];
                temp.instrumentType = fields[2];
                temp.timeStamp = DateTime.Parse(day.Date.ToShortDateString() + " " + fields[7]);
                string dstr = fields[3].Substring(4, 2) + "/" + fields[3].Substring(6, 2) + "/" + fields[3].Substring(0, 4);
                DateTime expDate1 = DateTime.Parse(dstr);
                temp.expDate = (int)(expDate1.ToOADate() - day.ToOADate());
                temp.callPut = fields[4];
                temp.strike = Double.Parse(fields[6]);
                temp.price = Double.Parse(fields[8]);
                temp.volume = Int32.Parse(fields[9]);
                
                if (fields[1].ToUpper() == "NIFTY")
                {
                    dataArray.Add(temp);
                    
                }               
            }
            sr.Close();

            ret = new TimeStampData[dataArray.Count];
            dataArray.Sort();
            dataArray.CopyTo(ret);           
           
            return ret;
        }
    }

    public class FileWrite
    {
        public StreamWriter sw;
        public string fileName;

        public FileWrite(string fileWrite)
        {
            fileName = fileWrite;
            if (fileName == "")
                fileName = "log.csv";
            //sw = new StreamWriter(fileName);
        }

        public void DataWrite(string raw)
        {
            sw = new StreamWriter(fileName);
            sw.Write(raw);
            sw.Close();
        }

        public void WriteLineAppend(string Line)
        {
            sw = new StreamWriter(fileName,true);
            sw.WriteLine(Line);
            
            sw.Close();
        }

        public void WriteLine(string Line)
        {
            sw = new StreamWriter(fileName);
            sw.WriteLine(Line);
            sw.Close();
        }

        public static void Write<T>(T[] Var, string fileName, bool append = false)
        {
            int i;
            StreamWriter sw1 = new StreamWriter(fileName, append);

            for (i = 0; i < Var.Length; i++)
            {
                sw1.WriteLine(Var[i]);
            }
            sw1.Close();
        }

        public static void Write<T>(T Var, string fileName, bool append = false)
        {
            StreamWriter sw1 = new StreamWriter(fileName, append);            
            sw1.WriteLine(Var);            
            sw1.Close();
        }

        public static void Write<T>(T[,] Var, string fileName, bool append = false)
        {
            int i, j;
            string line;
            StreamWriter sw1 = new StreamWriter(fileName, append);

            for (i = 0; i <= Var.GetUpperBound(0); i++)
            {
                line = Var[i, 0].ToString();
                for (j = 1; j <= Var.GetUpperBound(1); j++)
                    line = line + ", " + Var[i, j].ToString();
                sw1.WriteLine(line);
                line = "";
            }
            sw1.Close();            
        }

        public static void Write<T>(List<T> Var, string fileName, bool append = false)
        {
            int i;
           
            StreamWriter sw1 = new StreamWriter(fileName,append);

            for (i = 0; i < Var.Count; i++)
            {
                sw1.WriteLine(Var[i].ToString());                
            }
            sw1.Close(); 
        }

        public static void Write<T>(List<T[]> Var, string fileName, bool append = false, bool rows = true)
        {
            int i;

            StreamWriter sw1 = new StreamWriter(fileName, append);

            List<T[]> Var1;
            if (rows)
            {
                Var1 = Var;
            }
            else
            {
                Var1 = UF.Col2Rows<T>(Var);
            }

            List<T> row = new List<T>();
           
            for (i = 0; i < Var1.Count; i++)
            {
                sw1.WriteLine(UF.ToCSVString<T>(Var1[i]));
            }
            sw1.Close();
        }

        public void DataWriteSelected(string raw) // To get the Bhavcopy files...
        {
            StreamWriter sw1 = new StreamWriter("dummy.csv");

            sw1.Write(raw);
            sw1.Close();

            sw = new StreamWriter(fileName,true);
            StreamReader sr = new StreamReader("dummy.csv");

            int i;
            string line;
            string[] fields;
                      
            char[] delimiter;

            delimiter = new char[1];
            delimiter[0] = ',';

            fields = new string[25];
            
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                fields = line.Split(delimiter,25);
                if (fields[1].ToUpper() == "NIFTY" && Int32.Parse(fields[10])>10)
                    sw.WriteLine(line);               
            }

            sr.Close();
            sw.Close();
        }

        public void DataWriteOneVar(double[] entries)
        {
            int i;
            sw = new StreamWriter(fileName);
            
            for (i = 0; i < entries.Length; i++)
            {
                
                sw.WriteLine(entries[i]);
            }

            sw.Close();
         }
        public void DataWriteOneVar(int[] entries)
        {
            int i;
            sw = new StreamWriter(fileName);

            for (i = 0; i < entries.Length; i++)
            {

                sw.WriteLine(entries[i]);
            }

            sw.Close();
        }

        public void DataWriteOneVar(string[] entries)
        {
            int i;
            sw = new StreamWriter(fileName);

            for (i = 0; i < entries.Length; i++)
            {

                sw.WriteLine(entries[i]);
            }

            sw.Close();
        }

        public void DataWriteOneVar<T>(T[] entries, bool Append)
        {
            int i;
            if (Append)
            {
                sw = new StreamWriter(fileName, true);
            }
            else
            {
                sw = new StreamWriter(fileName);
            }

            for (i = 0; i < entries.Length; i++)
            {               
                sw.WriteLine(entries[i].ToString());
            }

            sw.Close();
        }

        public void DataWriteOneVar(DateTime[] entries)
        {
            int i;
            sw = new StreamWriter(fileName);

            for (i = 0; i < entries.Length; i++)
            {
                sw.WriteLine(entries[i].ToString("dd-MMM-yyyy HH:mm:ss"));
            }

            sw.Close();
        }


        public void DataSaveWriteRaw(string str)
        {                 
            sw = new StreamWriter(fileName,true);
            sw.WriteLine(str);
            sw.Close();
        }

        public void DataSaveWriteOneVar(double[] entries)
        {
            int i;
            string[] lines;
            FileRead fr = new FileRead(fileName);
            if (!File.Exists(fileName))
            {
                DataWriteOneVar(entries);
                return;
            }
            lines = fr.CSVDataExtractStringArray();
            int len = lines.Length;
            if (len == 0)
            {
                DataWriteOneVar(entries);
                return;
            }
            int entLen = entries.Length;
            string[] fields = lines[0].Split(new char[1] {','},1000);
            string dummyStr = " ";
            for (i = 0; i < fields.Length - 1; i++)
            {
                dummyStr = dummyStr + ", ";
            }
            sw = new StreamWriter(fileName);
            if (len >= entLen)
            {
                for (i = 0; i < entLen; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = entLen; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", ");
                }
            }
            else
            {
                for (i = 0; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = len; i < entLen; i++)
                {
                    sw.WriteLine(dummyStr + ", " + entries[i].ToString());
                }
            }

            sw.Close();
        }
        

        public void DataSaveWriteOneVar(string[] entries)
        {
            int i;
            string[] lines;
            FileRead fr = new FileRead(fileName);
            if (!File.Exists(fileName))
            {
                DataWriteOneVar(entries);
                return;
            }
            lines = fr.CSVDataExtractStringArray();
            int len = lines.Length;
            if (len == 0)
            {
                DataWriteOneVar(entries);
                return;
            }
            int entLen = entries.Length;
            string[] fields = lines[0].Split(new char[1] {',' }, 1000);
            string dummyStr = " ";
            for (i = 0; i < fields.Length - 1; i++)
            {
                dummyStr = dummyStr + ", ";
            }
            sw = new StreamWriter(fileName);
            if (len >= entLen)
            {
                for (i = 0; i < entLen; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i]);
                }
                for (i = entLen; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", ");
                }
            }
            else
            {
                for (i = 0; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i]);
                }
                for (i = len; i < entLen; i++)
                {
                    sw.WriteLine(dummyStr + ", " + entries[i]);
                }
            }

            sw.Close();
        }

        public void DataSaveWriteOneVar(int[] entries)
        {
            int i;
            string[] lines;
            FileRead fr = new FileRead(fileName);
            if (!File.Exists(fileName))
            {
                DataWriteOneVar(entries);
                return;
            }
            lines = fr.CSVDataExtractStringArray();
            int len = lines.Length;
            if (len == 0)
            {
                DataWriteOneVar(entries);
                return;
            }
            int entLen = entries.Length;
            string[] fields = lines[0].Split(new char[1] {',' }, 1000);
            string dummyStr = " ";
            for (i = 0; i < fields.Length - 1; i++)
            {
                dummyStr = dummyStr + ", ";
            }
            sw = new StreamWriter(fileName);
            if (len >= entLen)
            {
                for (i = 0; i < entLen; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = entLen; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", ");
                }
            }
            else
            {
                for (i = 0; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = len; i < entLen; i++)
                {
                    sw.WriteLine(dummyStr + ", " + entries[i].ToString());
                }
            }

            sw.Close();
        }

        public void DataSaveWriteOneVar(DateTime[] entries)
        {
            int i;
            string[] lines;
            FileRead fr = new FileRead(fileName);
            if (!File.Exists(fileName))
            {
                DataWriteOneVar(entries);
                return;
            }
            lines = fr.CSVDataExtractStringArray();
            int len = lines.Length;
            if (len == 0)
            {
                DataWriteOneVar(entries);
                return;
            }
            int entLen = entries.Length;
            string[] fields = lines[0].Split(new char[1] { ',' }, 1000);
            string dummyStr = " ";
            for (i = 0; i < fields.Length - 1; i++)
            {
                dummyStr = dummyStr + ", ";
            }
            sw = new StreamWriter(fileName);
            if (len >= entLen)
            {
                for (i = 0; i < entLen; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = entLen; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", ");
                }
            }
            else
            {
                for (i = 0; i < len; i++)
                {
                    sw.WriteLine(lines[i] + ", " + entries[i].ToString());
                }
                for (i = len; i < entLen; i++)
                {
                    sw.WriteLine(dummyStr + ", " + entries[i].ToString());
                }
            }

            sw.Close();
        }

        public void DataWriteMat(double[,] entries)
        {
            int i,j;
            string line;
            sw = new StreamWriter(fileName);
            for (i = 0; i <= entries.GetUpperBound(0); i++)
            {
                line = entries[i, 0].ToString();
                for (j = 1; j <= entries.GetUpperBound(1); j++)
                    line = line + ", " + entries[i, j].ToString();
                sw.WriteLine(line);
                line = "";
            }

            sw.Close();
        }

        public void DataWriteMat(int[,] entries)
        {
            int i, j;
            string line;
            sw = new StreamWriter(fileName);
            for (i = 0; i <= entries.GetUpperBound(0); i++)
            {
                line = entries[i, 0].ToString();
                for (j = 1; j <= entries.GetUpperBound(1); j++)
                    line = line + ", " + entries[i, j].ToString();
                sw.WriteLine(line);
                line = "";
            }

            sw.Close();
        }

        public void WriteTradeData(TimeStampData[] inptmp)
        {
            int i, j;
            string line;
            sw = new StreamWriter(fileName);
            for (i = 0; i <inptmp.Length; i++)
            {
                try
                {
                    line = inptmp[i].indexName + ", " + inptmp[i].instrumentType + ", " + inptmp[i].expDate.ToString() + ", " + inptmp[i].callPut + ", " + inptmp[i].strike.ToString() +
                        ", " + inptmp[i].timeStamp.ToString() + ", " + inptmp[i].price.ToString() + ", " + inptmp[i].volume.ToString();
                    sw.WriteLine(line);
                }
                catch
                {
                    break;
                }
                               
            }

            sw.Close();
        }

    }

   

    public class TimeStampData: IComparable
    {
        public string indexName;
        public string instrumentType;
        public int expDate;
        public string callPut;
        public double strike;
        public DateTime timeStamp;
        public double price;
        public int volume;

        int IComparable.CompareTo(object x)
        {
            TimeStampData catx = (TimeStampData)x;
                        
            if (this.timeStamp > catx.timeStamp)
                return 1;
            else
                return -1;
        }



    }
}
