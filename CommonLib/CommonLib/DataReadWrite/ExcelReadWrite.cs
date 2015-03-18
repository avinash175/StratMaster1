using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Data.OleDb;
using System.Reflection;
using System.Data;
using Microsoft.CSharp;
using Microsoft.Office.Interop.Excel;
//using Excel;

namespace CommonLib
{

    public class ExcelRead
    {
        public void Read(string fileName, string SheetName)
        {
            string connstr = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=" + fileName +
                ";Extended Properties=Excel 8.0;HDR=Yes;IMEX=1";
            OleDbConnection conn = new OleDbConnection(connstr);
            string strSQL = "SELECT * FROM " + SheetName;
            OleDbCommand cmd = new OleDbCommand(strSQL, conn);
            DataSet ds = new DataSet();
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(ds);
            //GridView1.DataSource = ds;
            //GridView1.DataBind();
        }
    }

    public class XlReadWrite
    {
        public Application excelApp;
        public Workbook Myworkbook;
        public Worksheet MyWorkSheet;
        public string FileName;
        public string SheetName;

        public XlReadWrite() // open new Excel WB
        {
            excelApp = new Application();

            Myworkbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            MyWorkSheet = Myworkbook.Sheets["Sheet1"] as Worksheet;

        }
        public XlReadWrite(string fileName) // open existing WB
        {
            FileName = fileName;
            excelApp = new Application();
            Myworkbook = excelApp.Workbooks.Open(FileName, 0, true, 5, "", "", false,
                XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            MyWorkSheet = Myworkbook.Sheets["Sheet1"] as Worksheet;

        }
        public XlReadWrite(string fileName, string _SheetName) // open existing WB
        {
            FileName = fileName;
            SheetName = _SheetName;
            excelApp = new Application();
            Myworkbook = excelApp.Workbooks.Open(FileName, 0, true, 5, "", "", false,
                XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            MyWorkSheet = Myworkbook.Sheets["Sheet1"] as Worksheet;

        }


        public void ShowExcelWB()
        {
            excelApp.Visible = true;
        }

        public void SaveExcelWB()
        {
            Myworkbook.Save();

        }
        public void CloseExcelWB()
        {
            excelApp.Workbooks.Close();
            excelApp.Quit();
        }

        #region OverloadedFunctions

        public void WriteDate(double data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }

        public void WriteDate(double[] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            for (int i = 0; i < data.Length; i++)
                excelRange.Cells[i + 1, 1] = data[i];
        }

        public void WriteDate(double[,] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }
        public void WriteDate(string data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }

        public void WriteDate(string[] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            for (int i = 0; i < data.Length; i++)
                excelRange.Cells[i + 1, 1] = data[i];
        }

        public void WriteDate(string[,] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }
        public void WriteDate(int data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }

        public void WriteDate(int[] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            for (int i = 0; i < data.Length; i++)
                excelRange.Cells[i + 1, 1] = data[i];
        }

        public void WriteDate(int[,] data, string range1, string range2)
        {
            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            excelRange.Value2 = data;
        }

        public double[] ReadData(string range1, string range2)
        {
            excelApp = new Application();
            Myworkbook = excelApp.Workbooks.Open(FileName, 0, true, 5, "", "", false,
                XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            MyWorkSheet = Myworkbook.Sheets["Sheet1"] as Worksheet;

            Range excelRange = MyWorkSheet.get_Range(range1, range2);
            List<string> data = new List<string>();           

            dynamic valueArray = excelRange.get_Value
                (XlRangeValueDataType.xlRangeValueDefault);

            object[,] valueArr = valueArray;
                        
            double[] dataOut = new double[data.Count];
            //data.CopyTo(dataOut);

            excelApp.Workbooks.Close();

            return dataOut;

        }

        public string[] ReadDataString(string range1, string range2)
        {
            excelApp = new Application();
            Myworkbook = excelApp.Workbooks.Open(FileName, 0, true, 5, "", "", false,
                XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            MyWorkSheet = Myworkbook.Sheets["Sheet1"] as Worksheet;

            Range excelRange = (Range)MyWorkSheet.get_Range(range1, range2);
            ArrayList data = new ArrayList();
            string d;

            for (int i = 0; i < excelRange.Count; i++)
            {
                try
                {
                    d = (string)((Range)excelRange.Cells[i + 1, 1]).Value2;
                    data.Add(d);
                }
                catch
                {
                    break;
                }

            }
            string[] dataOut = new string[data.Count];
            data.CopyTo(dataOut);

            excelApp.Workbooks.Close();

            return dataOut;

        }

        ~XlReadWrite()
        {
            excelApp.Workbooks.Close();
        }

        #endregion

    }

}
