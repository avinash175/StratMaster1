using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class CSVFileDetails
    {
        public string FileName { get; set; }
        public CSVHeaderParams HeaderParams { get; set; }             
        public bool Is1stArrayHeader { get; set; } 
        public int NumSkip { get; set; }       
        public TypeOfCSVOperation CSVOperationType { get; set; }
        public CSVData CSVDataObj { get; set; }
        
        public CSVFileDetails()
        {

        }

        public CSVFileDetails(string _FileName, TypeOfCSVOperation _CSVOperationType, 
            CSVHeaderParams _HeaderParams, int _NumSkip, bool _Is1stArrayHeader)
        {
            FileName = _FileName;
            CSVOperationType = _CSVOperationType;
            HeaderParams =_HeaderParams;
            NumSkip = _NumSkip;
            Is1stArrayHeader = _Is1stArrayHeader;
        }
    }

    public class CSVHeaderParams
    {
        public Type[] TypeOfHeader { get; set; }
        public int[] RowColIdx { get; set; }
        public string[] HeaderNames { get; set; }

        public CSVHeaderParams()
        {

        }

        public CSVHeaderParams(int n)
        {
            TypeOfHeader = new Type[n];
            RowColIdx = new int[n];
            HeaderNames = new string[n];
        }

        public CSVHeaderParams(Type[] _TypeOfHeader, int[] _RowColIdx, string[] _HeaderNames)
        {
            TypeOfHeader = _TypeOfHeader;
            RowColIdx = _RowColIdx;
            HeaderNames = _HeaderNames;
        }
    }

    public class CSVData
    {
        public string[] HeaderNames { get; set; }
        public List<Array> Data { get; set; }

        public CSVData()
        {

        }

        public CSVData(int n)
        {
            HeaderNames = new string[n];
            Data = new List<Array>();
        }
    }

    public enum TypeOfCSVOperation
    {        
        ALL_COLS,
        ALL_ROWS,
        SOME_ROWS,
        SOME_COLS,
        XY_VALS
    }

    
}
