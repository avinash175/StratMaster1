using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommonLib
{
    public class NF
    {
        public static double[] FindExpiryArray(DateTime[] Expiry, DateTime startDate)
        {
            double[] exp = new double[Expiry.Length];

            for (int i = 0; i < Expiry.Length; i++)
            {
                exp[i] = (Expiry[i].ToOADate() - startDate.ToOADate()) / 365.0;
            }

            return exp;
        }

        //Function finds the index where there is a match for dates
        public static int[] FindMatchIndex(DateTime[] ManyDates, DateTime OneDate)
        {
            int i, n = ManyDates.Length;
            List<int> Col = new List<int>();
            for (i = 0; i < n; i++)
            {
                if (ManyDates[i] == OneDate)
                {
                    Col.Add(i);
                }
            }
            int m = Col.Count;

            int[] IndexArray = new int[m];
            Col.CopyTo(IndexArray);
            return IndexArray;
        }

        //Function finds the index where there is a match for dates
        public static int[] FindMatchIndex<T>(T[] ManyDates, T OneDate)
        {
            int i, n = ManyDates.Length;
            List<int> Col = new List<int>();
            for (i = 0; i < n; i++)
            {
                if (ManyDates[i].Equals(OneDate))
                {
                    Col.Add(i);
                }
            }                        
            return Col.ToArray();
        }

        // overloaded for double
        public static int[] FindMatchIndex(double[] ManyVal, double OneVal)
        {
            int i, n = ManyVal.Length;
            List<int> Col = new List<int>();
            for (i = 0; i < n; i++)
            {
                if (ManyVal[i] == OneVal)
                {
                    Col.Add(i);
                }
            }
            int m = Col.Count;

            int[] IndexArray = new int[m];
            Col.CopyTo(IndexArray);
            return IndexArray;
        }

        // overloaded for string
        public static int[] FindMatchIndex(string[] ManyVal, string OneVal)
        {
            int i, n = ManyVal.Length;
            List<int> Col = new List<int>();
            for (i = 0; i < n; i++)
            {
                if (ManyVal[i] == OneVal)
                {
                    Col.Add(i);
                }
            }
            int m = Col.Count;

            int[] IndexArray = new int[m];
            Col.CopyTo(IndexArray);
            return IndexArray;
        }
        
        // Expiry, Option Type, Out of the Money
        public static int[] FindOptionMatchIndex(double[] ManyExp, string[] ManyStr,
            double[] ManyStk, double OneExp, string OneStr, double OneStk)
        {
            int i, n = ManyExp.Length;
            ArrayList Col = new ArrayList();
            for (i = 0; i < n; i++)
            {
                if (ManyExp[i] == OneExp && ManyStr[i] == OneStr)
                {
                    if (ManyStr[i] == "CE")
                    {
                        if (ManyStk[i] > OneStk)
                            Col.Add(i);
                    }
                    else if (ManyStr[i] == "PE")
                    {
                        if (ManyStk[i] <= OneStk)
                            Col.Add(i);
                    }
                }
            }
            int m = Col.Count;

            int[] IndexArray = new int[m];
            Col.CopyTo(IndexArray);
            return IndexArray;
        }

        // Modified: for Calls (or Puts) only implied vols
        public static int[] FindOptionMatchIndex(double[] ManyExp, string[] ManyStr,
            double[] ManyStk, double OneExp, string OneStr)
        {
            int i, n = ManyExp.Length;
            ArrayList Col = new ArrayList();
            for (i = 0; i < n; i++)
            {
                if (ManyExp[i] == OneExp && ManyStr[i] == OneStr)
                {
                    Col.Add(i);
                }
            }
            int m = Col.Count;
            int[] IndexArray = new int[m];
            Col.CopyTo(IndexArray);
            return IndexArray;
        }

        public static int LinearSearch<T>(T[] arr, T search)
        {
            int idx = -1;

            for (int i = 0; i < arr.Length; i++)
            {
                if (search.Equals(arr[i]))
                {
                    idx = i;
                    break;
                }
            }

            return idx;
        }

        public static DateTime[] FindUniqueDates(DateTime[] DateTimeArray)
        {
            int i, n = DateTimeArray.Length;
            ArrayList UniqueDates = new ArrayList();

            for (i = 0; i < n; i++)
            {
                if (!UniqueDates.Contains(DateTimeArray[i]))
                {
                    UniqueDates.Add(DateTimeArray[i]);
                }
            }

            DateTime[] output = new DateTime[UniqueDates.Count];
            UniqueDates.CopyTo(output);

            return output;
        }

        public static DateTime[] IntersectSortedDateTimes(DateTime[] A, DateTime[] B)
        {
            int i, idx;
            int n = A.Length;
            ArrayList Dates_Dummy = new ArrayList();
            DateTime[] IntersectAB;

            for (i = 0; i < n; i++)
            {
                idx = Array.BinarySearch(B, A[i]);
                if (idx >= 0)
                {
                    Dates_Dummy.Add(A[i]);
                }
            }
            IntersectAB = new DateTime[Dates_Dummy.Count];
            Dates_Dummy.CopyTo(IntersectAB);
            return IntersectAB;
        }

        public static int[] IntersectSortedInt(int[] A, int[] B)
        {
            int i, idx;
            int n = A.Length;
            ArrayList Ints_Dummy = new ArrayList();
            int[] IntersectAB;

            for (i = 0; i < n; i++)
            {
                idx = Array.BinarySearch(B, A[i]);
                if (idx >= 0)
                {
                    Ints_Dummy.Add(A[i]);
                }
            }
            IntersectAB = new int[Ints_Dummy.Count];
            Ints_Dummy.CopyTo(IntersectAB);
            return IntersectAB;
        }

        public static int FindCommonIndex(DateTime[] A, DateTime B)
        {
            int index = -1;           
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] == B)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        // works for unsorted arrays unlike BinarySearch (Finds 1st instance)
        public static int FindCommonIndex(int[] A, int B)
        {
            int index = -1;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] == B)
                {
                    index = i;
                    break;
                }
            }
            
            return index;
        }
        // works for unsorted arrays unlike BinarySearch (Finds 1st instance)
        public static int FindCommonIndex(string[] A, string B)
        {
            int index = -1;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] == B)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
        // works for unsorted arrays unlike BinarySearch (Finds 1st instance)
        public static int FindCommonIndex(double[] A, double B)
        {
            int index = -1;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] == B)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static int[] FindCommonIndex(DateTime[] OriginalArray, DateTime[] IntersectedArray)
        {
            int[] indexes =  new int[IntersectedArray.Length];
            int i;
            for ( i = 0; i < indexes.Length; i++)
            {                
                indexes[i] = Array.BinarySearch(OriginalArray, IntersectedArray[i]);               
            }
            return indexes;
        }

        public static int[] FindCommonIndex(int[] OriginalArray, int[] IntersectedArray)
        {
            int[] indexes = new int[IntersectedArray.Length];
            int i;
            for (i = 0; i < indexes.Length; i++)
            {
                indexes[i] = Array.BinarySearch(OriginalArray, IntersectedArray[i]);
            }
            return indexes;
        }


        public static DateIndices ReturnCommonIndex(DateTime[] Arr1, DateTime[] Arr2)
        {
            DateTime[] Intersect = IntersectSortedDateTimes(Arr1, Arr2);
            DateIndices idx = new DateIndices();
            idx.index1 = FindCommonIndex(Arr1, Intersect);
            idx.index2 = FindCommonIndex(Arr2, Intersect);
            return idx;
        }

        public static double GetClosingPx(string FileName, DateTime date, int colDate, int colPx, int skip, bool IsDateStr)
        {
            FileRead fr = new FileRead(FileName);
            double[] closePx = fr.CSVDataExtractOneVar(colPx, skip);
            DateTime[] dates;
            if (IsDateStr)
                dates = fr.CSVDataExtractOneVarDate(colDate, skip);
            else
                dates = fr.CSVDataExtractFastOneVarDate(colDate, skip);

            int idx = Array.BinarySearch(dates, date.Date);
            double result = 0;
            if (idx >= 0)
            {
                result = closePx[idx];
            }

            return result;
        }

        public static int GetIdxOfDateFromFile(string FileName, DateTime date, int colDate, int skip, bool IsDateStr)
        {
            FileRead fr = new FileRead(FileName);
            
            DateTime[] dates;
            if (IsDateStr)
                dates = fr.CSVDataExtractOneVarDate(colDate, skip);
            else
                dates = fr.CSVDataExtractFastOneVarDate(colDate, skip);

            int idx = Array.BinarySearch(dates, date);
            
            return idx;
        }
        public static DateTime GetDateUsingIdxFromFile(string FileName, int Idx, int colDate, int skip, bool IsDateStr)
        {
            FileRead fr = new FileRead(FileName);

            DateTime[] dates;
            if (IsDateStr)
                dates = fr.CSVDataExtractOneVarDate(colDate, skip);
            else
                dates = fr.CSVDataExtractFastOneVarDate(colDate, skip);

            DateTime ret = new DateTime();
            if (Idx >= 0 && Idx < dates.Length)
                ret = dates[Idx];
            else
                throw new Exception("Enetered date Idx not proper");
            return ret;
        }

        public static DateTime GetExpiry(string FileName, DateTime day, int colDate, int skip, bool IsDateStr)
        {
            FileRead fr = new FileRead(FileName);

            DateTime[] dates;
            if (IsDateStr)
                dates = fr.CSVDataExtractOneVarDate(colDate, skip);
            else
                dates = fr.CSVDataExtractFastOneVarDate(colDate, skip);

            ArrayList TempDates = new ArrayList();
            
            DateTime[] LastThursdays = GetLastThurdays(dates[0],dates[dates.Length-1]);
            int Idx = Array.BinarySearch(LastThursdays, day);
            if(Idx<0)
                Idx = ~Idx;
            
            DateTime LThur = LastThursdays[Idx];

            int Idx2 = Array.BinarySearch(dates,LThur);
            if(Idx2<0)
            {
                Idx2 = ~Idx2;
                Idx2--;
            }
            
            return dates[Idx2];
        }

        public static DateTime[] GetLastThurdays(DateTime startDate, DateTime endDate)
        {
            DateTime[] ret;
            ArrayList temp = new ArrayList();

            DateTime day = startDate;
            
            while (day <= endDate)
            {
                if (day.DayOfWeek == DayOfWeek.Thursday)
                {
                    DateTime tempday = day.AddDays(7.0);
                    if (tempday.Day < day.Day)
                    {
                        temp.Add(day);
                    }
                }
                day = day.AddDays(1.0);
            }
            ret = new DateTime[temp.Count];
            temp.CopyTo(ret);

            return ret;
        }

        
    }

    public class DateIndices
    {
        public int[] index1, index2;
    }
}
