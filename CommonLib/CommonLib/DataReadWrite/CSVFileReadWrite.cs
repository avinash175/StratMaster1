using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace CommonLib
{
    public class CSVFileReadWrite
    {
        public CSVFileDetails FileType { get; set; }
        public CSVData Data;

        public CSVFileReadWrite()
        {

        }

        public CSVFileReadWrite(CSVFileDetails _FileType)
        {
            FileType = _FileType;
        }

        public CSVFileReadWrite (CSVFileDetails _FileType, CSVData _Data)
	    {
            FileType = _FileType;
            Data = _Data;
    	}

        public void Read()
        {
            try
            {
                StreamReader sr = new StreamReader(FileType.FileName);

                string line;
                string[] fields;
                bool isFirstTime = true;

                if (FileType.CSVOperationType == TypeOfCSVOperation.ALL_COLS
                    || FileType.CSVOperationType == TypeOfCSVOperation.SOME_COLS)
                {
                    for (int i = 0; i < FileType.NumSkip; i++)
                    {
                        sr.ReadLine();
                    }

                    if (FileType.Is1stArrayHeader)
                    {
                        line = sr.ReadLine();
                        fields = line.Split(',');
                        if (FileType.CSVOperationType == TypeOfCSVOperation.ALL_COLS)
                        {
                            FileType.HeaderParams = new CSVHeaderParams(fields.Length);
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FileType.HeaderParams.HeaderNames[i] = fields[i];
                                FileType.HeaderParams.RowColIdx[i] = i;
                            }
                        }
                        else if (FileType.CSVOperationType == TypeOfCSVOperation.SOME_COLS)
                        {
                            int numCol = FileType.HeaderParams.RowColIdx.Length;
                            FileType.HeaderParams.HeaderNames = new string[numCol];
                            for (int i = 0; i < numCol; i++)
                            {
                                int idx = FileType.HeaderParams.RowColIdx[i];
                                FileType.HeaderParams.HeaderNames[i] = fields[idx];
                            }
                        }
                    }
                    
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        fields = line.Split(',');
                        if (FileType.CSVOperationType == TypeOfCSVOperation.ALL_COLS)
                        {
                            if (isFirstTime)
                            {
                                FileType.CSVDataObj = new CSVData(fields.Length);
                                isFirstTime = false;
                            }
                            for (int i = 0; i < fields.Length; i++)
                            {
                                
                            }
                        }
                        else if (FileType.CSVOperationType == TypeOfCSVOperation.SOME_COLS)
                        {
                            int numCol = FileType.HeaderParams.RowColIdx.Length;
                            if (isFirstTime)
                            {
                                FileType.CSVDataObj = new CSVData(numCol);
                                isFirstTime = false;
                            }
                            for (int i = 0; i < numCol; i++)
                            {
                                int idx = FileType.HeaderParams.RowColIdx[i];
                                
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;// new Exception(ex.Message);
            }
        }

        public void Write()
        {
        }

     
    }

    public class CSVParser
    {
        public static List<string[]> parseCSV(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return parsedData;
        }

        public static T[] GetCol<T>(List<T[]> data, int col, int skip)//, IgnoreType iT)
        {
            T[] res = null;

            if (skip>=data.Count)
            {
                return res;
            }

            res = new T[data.Count - skip];

            for (int i = skip; i < data.Count; i++)
            {
                res[i - skip] =  data[i][col];
            }

            return res;
        }
    }

    public enum IgnoreType
    {
        REPLACE_BY_ZERO,
        STOP_AT_1ST_ERROR,
        REPLACE_BY_MIN,
        REPLACE_BY_MAX
    }
 
}
