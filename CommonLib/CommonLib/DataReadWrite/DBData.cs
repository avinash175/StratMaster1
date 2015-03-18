//using System;
//using System.Collections.Generic;
//using System.Text;
//using Oracle.DataAccess.Client;
//using System.Collections;
//using System.Globalization;
//using System.Data;
//using System.IO;

//namespace CommonLib
//{
//    public class DBData
//    {
//        private static string passwrd = "BLOOMDATA1";
//        private static string connstr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.250.2.28)(PORT=1525)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=analytic)));Persist Security Info=True;" +
//                   "User ID=BLOOMDATA;Password=";
//        public static string[] ReturnStringArray(string SQLquery)
//        {
//            string connectionString = connstr+passwrd;
//            string[] output;
//            using (OracleConnection connection = new OracleConnection())
//            {
//                ArrayList TablesArr = new ArrayList();
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
                
//                command.CommandText = SQLquery;

//                OracleDataReader reader = command.ExecuteReader();
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    TablesArr.Add((string)reader[0]);                    
//                }
//                connection.Close();
//                output = new string[TablesArr.Count];
//                TablesArr.CopyTo(output);                 
//            }
//            return output;
//        }

//        public static DateTime[] ReturnDateTimeArray(string SQLquery)
//        {
//            string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.13.1.25)(PORT=1525)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=analytic)));Persist Security Info=True;" +
//                   "User ID=testing;Password=testing1";
//            DateTime[] output;
//            using (OracleConnection connection = new OracleConnection())
//            {
//                ArrayList TablesArr = new ArrayList();
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();

//                command.CommandText = SQLquery;

//                OracleDataReader reader = command.ExecuteReader();
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    TablesArr.Add((DateTime)reader[0]);
//                }
//                connection.Close();
//                output = new DateTime[TablesArr.Count];
//                TablesArr.CopyTo(output);
//            }
//            return output;
//        }

//        public static DBDataFormat GetDataFromDB(string SQLquery)
//        {
//            //       "User ID=testing;Password=testing;Unicode=True";//;Service_name=analytic";           
//            DBDataFormat output;
//            string connectionString = connstr+passwrd+";Unicode=True";
//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;

//                connection.Open();

//                OracleCommand command = connection.CreateCommand();
//                //string sql = "SELECT * FROM FNO1MIN_2009N WHERE TRADE_SYM = 'NIFTY' AND OPTIONTYPE='FF' AND EXPIRY_TYPE = 'Current'";
//                command.CommandText = SQLquery;

//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList TimeStamp = new ArrayList();
//                ArrayList Trade_Sym = new ArrayList();
//                ArrayList Instrument = new ArrayList();
//                ArrayList Expiry = new ArrayList();
//                ArrayList OptionType = new ArrayList();
//                ArrayList Strike = new ArrayList();
//                ArrayList LTP = new ArrayList();
//                ArrayList Volume = new ArrayList();
//                ArrayList VWAP = new ArrayList();

//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    DateTime date = DateTime.ParseExact(((decimal)reader["TIME_GROUP"]).ToString(), "yyyyMMdd", provider);
//                    DateTime Time = DateTime.ParseExact((string)reader["CLOSE_TIME"], "HH:mm", provider);
//                    TimeStamp.Add(date.Add(Time.TimeOfDay));
//                    Trade_Sym.Add((string)reader["TRADE_SYM"]);
//                    Instrument.Add((string)reader["INSTRUMENT"]);
//                    Expiry.Add((DateTime)reader["SETTDATE"]);
//                    OptionType.Add((string)reader["OPTIONTYPE"]);
//                    Strike.Add((decimal)reader["STRIKE"]);
//                    LTP.Add((decimal)reader["LTP"]);
//                    Volume.Add((decimal)reader["TOTAL_QTY"]);
//                    //VWAP.Add((decimal)reader["VWAP"]);
//                }
//                connection.Close();

//                output = new DBDataFormat(TimeStamp.Count);

//                TimeStamp.CopyTo(output.TimeStamp);
//                Trade_Sym.CopyTo(output.Trade_Sym);
//                Instrument.CopyTo(output.Instrument);
//                Expiry.CopyTo(output.Expiry);
//                OptionType.CopyTo(output.OptionType);
//                Strike.CopyTo(output.Strike);
//                LTP.CopyTo(output.LTP);
//                Volume.CopyTo(output.Volume);
//                //VWAP.CopyTo(output.VWAP);         

//            }

//            return output;
//        }

//        public static Level1Entry GetBasketStkDataFromDB(string SQLqueryF, string SQLqueryC)
//        {
//            Level1Entry output;
//            string connectionString = connstr + passwrd;
            
//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();                
//                command.CommandText = SQLqueryF;
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList TimeArr = new ArrayList();
//                ArrayList TimeIdx = new ArrayList();
//                ArrayList LTP = new ArrayList();
//                ArrayList LTV = new ArrayList();
//                ArrayList BidV = new ArrayList();
//                ArrayList AskV = new ArrayList();
//                ArrayList BidPx = new ArrayList();
//                ArrayList AskPx = new ArrayList();
//                ArrayList TimeIdxCash = new ArrayList();
//                ArrayList LTPCash = new ArrayList();

//                decimal kk = 0;
//                CultureInfo provider = CultureInfo.InvariantCulture;
                
//                while (reader.Read())
//                {
//                    double ask = (double)((decimal)reader["ASK"]);
//                    double bid = (double)((decimal)reader["BID"]);
//                    int TimeTemp1 = (int)((decimal)reader["TIME_CODE"]);

//                    if (ask-bid<0.05)
//                        continue; // bad entry in the DB

//                    //if (!TimeIdx.Contains(TimeTemp1)) // repeated entry in the DB
//                    {
//                        DateTime date = DateTime.ParseExact(((decimal)reader["TIME_GROUP"]).ToString(), "yyyyMMdd", provider);
//                        DateTime Time = DateTime.ParseExact((string)reader["Time_Text"], "HH:mm:ss", provider);
//                        int len = TimeArr.Count;
//                        if (len > 0)
//                        {
//                            if ((DateTime)TimeArr[len - 1] != date.Add(Time.TimeOfDay))
//                            {
//                                TimeArr.Add(date.Add(Time.TimeOfDay));
//                                TimeIdx.Add(TimeTemp1);
//                                LTP.Add((double)((decimal)reader["LTP"]));
//                                LTV.Add((double)((decimal)reader["LTPVOL"]));
//                                BidV.Add((double)((decimal)reader["BIDVOL"]));
//                                AskV.Add((double)((decimal)reader["ASKVOL"]));
//                                BidPx.Add(bid);
//                                AskPx.Add(ask);
//                                kk = kk + 1;
//                            }
//                        }
//                        else
//                        {
//                            TimeArr.Add(date.Add(Time.TimeOfDay));
//                            TimeIdx.Add(TimeTemp1);
//                            LTP.Add((double)((decimal)reader["LTP"]));
//                            LTV.Add((double)((decimal)reader["LTPVOL"]));
//                            BidV.Add((double)((decimal)reader["BIDVOL"]));
//                            AskV.Add((double)((decimal)reader["ASKVOL"]));
//                            BidPx.Add(bid);
//                            AskPx.Add(ask);
//                            kk = kk + 1;
//                        }
//                        //VWAP.Add((decimal)reader["VWAP"]);
//                    }
//                }
//                connection.Close();

//                connection.Open();
                
//                command = connection.CreateCommand();
//                command.CommandText = SQLqueryC;
//                reader = command.ExecuteReader();

//                while (reader.Read())
//                {
//                    int TimeTemp = (int)((decimal)reader["TIME_CODE"]);
//                    if (!TimeIdxCash.Contains(TimeTemp))
//                    {
//                        TimeIdxCash.Add(TimeTemp);
//                        LTPCash.Add((double)((decimal)reader["LTP"]));
//                    }
//                }
//                connection.Close();

//                output = new Level1Entry(TimeArr.Count);

//                output.LTPCash = new double[LTPCash.Count];
//                output.TimeIdxCash = new int[TimeIdxCash.Count];

//                TimeArr.CopyTo(output.TimeArr);
//                TimeIdx.CopyTo(output.TimeIdx);                
//                LTP.CopyTo(output.LTP);
//                LTV.CopyTo(output.LTV);
//                BidPx.CopyTo(output.bidPx);
//                AskPx.CopyTo(output.askPx);
//                AskV.CopyTo(output.askVol);
//                BidV.CopyTo(output.bidVol);
//                LTPCash.CopyTo(output.LTPCash);
//                TimeIdxCash.CopyTo(output.TimeIdxCash);
//                //VWAP.CopyTo(output.VWAP);
//            }            

//            return output;
//        }

//        public static Level1EntryCF GetBasketStkDataFromDBCF(string SQLqueryF, string SQLqueryC)
//        {
//            Level1EntryCF output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLqueryF;
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList TimeArrFut = new ArrayList();
//                ArrayList TimeIdxFut = new ArrayList();
//                ArrayList LTPFut = new ArrayList();  
//                ArrayList BidFut = new ArrayList();
//                ArrayList AskFut = new ArrayList();
//                ArrayList TimeArrCash = new ArrayList();
//                ArrayList TimeIdxCash = new ArrayList();
//                ArrayList LTPCash = new ArrayList();
//                ArrayList BidCash = new ArrayList();
//                ArrayList AskCash = new ArrayList();                

//                decimal kk = 0;
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    double ask = (double)((decimal)reader["ASK"]);
//                    double bid = (double)((decimal)reader["BID"]);
//                    int TimeTemp1 = (int)((decimal)reader["TIME_CODE"]);

//                    if (ask - bid < 0.045)
//                        continue; // bad entry in the DB

//                    //if (!TimeIdx.Contains(TimeTemp1)) // repeated entry in the DB
//                    {
//                        DateTime date = DateTime.ParseExact(((decimal)reader["TIME_GROUP"]).ToString(), "yyyyMMdd", provider);
//                        DateTime Time = DateTime.ParseExact((string)reader["Time_Text"], "HH:mm:ss", provider);
//                        int len = TimeArrFut.Count;
//                        if (len > 0)
//                        {
//                            if ((DateTime)TimeArrFut[len - 1] != date.Add(Time.TimeOfDay))
//                            {
//                                TimeArrFut.Add(date.Add(Time.TimeOfDay));
//                                TimeIdxFut.Add(TimeTemp1);
//                                LTPFut.Add((double)((decimal)reader["LTP"]));                                
//                                BidFut.Add(bid);
//                                AskFut.Add(ask);
//                                kk = kk + 1;
//                            }
//                        }
//                        else
//                        {
//                            TimeArrFut.Add(date.Add(Time.TimeOfDay));
//                            TimeIdxFut.Add(TimeTemp1);
//                            LTPFut.Add((double)((decimal)reader["LTP"]));                           
//                            BidFut.Add(bid);
//                            AskFut.Add(ask);
//                            kk = kk + 1;
//                        }                        
//                    }
//                }
//                connection.Close();

//                connection.Open();

//                command = connection.CreateCommand();
//                command.CommandText = SQLqueryC;
//                reader = command.ExecuteReader();

//                while (reader.Read())
//                {
//                    double ask = (double)((decimal)reader["ASK"]);
//                    double bid = (double)((decimal)reader["BID"]);
//                    int TimeTemp1 = (int)((decimal)reader["TIME_CODE"]);

//                    if (ask - bid < 0.045)
//                        continue; // bad entry in the DB

//                    //if (!TimeIdx.Contains(TimeTemp1)) // repeated entry in the DB
//                    {
//                        DateTime date = DateTime.ParseExact(((decimal)reader["TIME_GROUP"]).ToString(), "yyyyMMdd", provider);
//                        DateTime Time = DateTime.ParseExact((string)reader["Time_Text"], "HH:mm:ss", provider);
//                        int len = TimeArrCash.Count;
//                        if (len > 0)
//                        {
//                            if ((DateTime)TimeArrCash[len - 1] != date.Add(Time.TimeOfDay))
//                            {
//                                TimeArrCash.Add(date.Add(Time.TimeOfDay));
//                                TimeIdxCash.Add(TimeTemp1);
//                                LTPCash.Add((double)((decimal)reader["LTP"]));
//                                BidCash.Add(bid);
//                                AskCash.Add(ask);
//                                kk = kk + 1;
//                            }
//                        }
//                        else
//                        {
//                            TimeArrCash.Add(date.Add(Time.TimeOfDay));
//                            TimeIdxCash.Add(TimeTemp1);
//                            LTPCash.Add((double)((decimal)reader["LTP"]));
//                            BidCash.Add(bid);
//                            AskCash.Add(ask);
//                            kk = kk + 1;
//                        }
//                    }
//                }
//                connection.Close();

//                output = new Level1EntryCF(TimeArrFut.Count, TimeArrCash.Count); 

//                TimeArrFut.CopyTo(output.TimeArrFut);
//                TimeIdxFut.CopyTo(output.TimeIdxFut);
//                LTPFut.CopyTo(output.LTPFut);                
//                BidFut.CopyTo(output.bidFut);
//                AskFut.CopyTo(output.askFut);

//                TimeArrCash.CopyTo(output.TimeArrCash);
//                TimeIdxCash.CopyTo(output.TimeIdxCash);
//                LTPCash.CopyTo(output.LTPCash);
//                BidCash.CopyTo(output.bidCash);
//                AskCash.CopyTo(output.askCash);
//                //VWAP.CopyTo(output.VWAP);
//            }

//            return output;
//        }

//        public static Level1Entry GetBasketStkDataFromDBOnlyCash(string SQLqueryC)
//        {
//            Level1Entry output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLqueryC;
//                OracleDataReader reader = command.ExecuteReader();
                                
//                ArrayList TimeIdxCash = new ArrayList();
//                ArrayList LTPCash = new ArrayList();
//                ArrayList LTVCash = new ArrayList();

//                decimal kk = 0;
//                CultureInfo provider = CultureInfo.InvariantCulture;
                           

//                while (reader.Read())
//                {
//                    int TimeTemp = (int)((decimal)reader["TIME_CODE"]);
//                    if (!TimeIdxCash.Contains(TimeTemp))
//                    {
//                        TimeIdxCash.Add(TimeTemp);
//                        LTPCash.Add((double)((decimal)reader["LTP"]));
//                        LTVCash.Add((double)((decimal)reader["LTPVOL"]));
//                    }
//                }
//                connection.Close();

//                output = new Level1Entry(TimeIdxCash.Count);

//                output.LTPCash = new double[LTPCash.Count];
//                output.TimeIdxCash = new int[TimeIdxCash.Count];

//                LTPCash.CopyTo(output.LTPCash);
//                LTVCash.CopyTo(output.LTVCash);
//                TimeIdxCash.CopyTo(output.TimeIdxCash);
//                //VWAP.CopyTo(output.VWAP);
//            }

//            return output;
//        }

//        public static Level1Entry GetL1EDataFromDB(string SQLquery)
//        {
//            Level1Entry output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLquery;
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList TimeArr = new ArrayList();
//                ArrayList TimeIdx = new ArrayList();
//                ArrayList LTP = new ArrayList();
//                ArrayList LTV = new ArrayList();
//                ArrayList BidV = new ArrayList();
//                ArrayList AskV = new ArrayList();
//                ArrayList BidPx = new ArrayList();
//                ArrayList AskPx = new ArrayList();                

//                decimal kk = 0;
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    double ask = (double)((decimal)reader["ASK"]);
//                    double bid = (double)((decimal)reader["BID"]);
//                    int TimeTemp1 = (int)((decimal)reader["TIME_CODE"]);

//                    if (ask - bid < 0.05)
//                        continue; // bad entry in the DB

//                    //if (!TimeIdx.Contains(TimeTemp1)) // repeated entry in the DB
//                    {
//                        DateTime date = DateTime.ParseExact(((decimal)reader["TIME_GROUP"]).ToString(), "yyyyMMdd", provider);
//                        DateTime Time = DateTime.ParseExact((string)reader["Time_Text"], "HH:mm:ss", provider);

//                        TimeArr.Add(date.Add(Time.TimeOfDay));
//                        TimeIdx.Add(TimeTemp1);
//                        LTP.Add((double)((decimal)reader["LTP"]));
//                        LTV.Add((double)((decimal)reader["LTPVOL"]));
//                        BidV.Add((double)((decimal)reader["BIDVOL"]));
//                        AskV.Add((double)((decimal)reader["ASKVOL"]));
//                        BidPx.Add(bid);
//                        AskPx.Add(ask);
//                        kk = kk + 1;
//                        //VWAP.Add((decimal)reader["VWAP"]);
//                    }
//                }
//                connection.Close();               

//                output = new Level1Entry(TimeArr.Count);                

//                TimeArr.CopyTo(output.TimeArr);
//                TimeIdx.CopyTo(output.TimeIdx);
//                LTP.CopyTo(output.LTP);
//                LTV.CopyTo(output.LTV);
//                BidPx.CopyTo(output.bidPx);
//                AskPx.CopyTo(output.askPx);
//                AskV.CopyTo(output.askVol);
//                BidV.CopyTo(output.bidVol);              
                
//            }

//            return output;
//        }

//        public static DB1MinData Get1MinDataFromDB(string SQLquery)
//        {
//            DB1MinData output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLquery;
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList TimeArr = new ArrayList();                
//                ArrayList LTP = new ArrayList();
//                ArrayList Vol = new ArrayList();               
//                ArrayList BidPx = new ArrayList();
//                ArrayList AskPx = new ArrayList();
                                
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    double ask = (double)((decimal)reader["ASK"]);
//                    double bid = (double)((decimal)reader["BID"]);                    

//                    //if (ask - bid < 0.05)
//                        //continue; // bad entry in the DB                                       
                   
//                    DateTime date = ((DateTime)reader["DATE_VAL"]);
//                    string timeStr =  ((string)reader["TIME_VAL"]);                    
//                    if (timeStr.Split(':')[0].Length<2)
//                    {
//                        timeStr = "0" + timeStr;
//                    }
//                    DateTime Time = DateTime.ParseExact(timeStr, "HH:mm:ss", provider);

//                    TimeArr.Add(date.Add(Time.TimeOfDay));                   
//                    LTP.Add((double)((decimal)reader["LTP"]));
//                    Vol.Add((double)((decimal)reader["VOLUME"]));
                   
//                    BidPx.Add(bid);
//                    AskPx.Add(ask);                    
//                }
//                connection.Close();

//                output = new DB1MinData(TimeArr.Count);

//                TimeArr.CopyTo(output.TimeArr);                
//                LTP.CopyTo(output.LTPArr);
//                Vol.CopyTo(output.VolArr);
//                BidPx.CopyTo(output.BidArr);
//                AskPx.CopyTo(output.AskArr);
//            }

//            return output;
//        }

//        public static string[] GetDistinctNSE_SYMFromDB(string SQLquery)
//        {
//            string[] output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLquery;
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList NSESYM = new ArrayList();
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    NSESYM.Add((string)reader["NSE_SYM"]);                             
//                }
//                connection.Close();
//                output = new string[NSESYM.Count];
//                NSESYM.CopyTo(output);
//            }            
//            return output;
//        }

//        public static T[] GetDistinctFromDB<T>( string tableName, string colName, string whereCondition)
//        {
//            T[] output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                if (whereCondition != "")
//                {
//                    command.CommandText = "SELECT DISTINCT " + colName + " FROM " + tableName 
//                        + " WHERE " + whereCondition + " ORDER BY " + colName + " ASC";
//                }
//                else
//                {
//                    command.CommandText = "SELECT DISTINCT " + colName + " FROM " + tableName 
//                        + " ORDER BY " + colName + " ASC";
//                }
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList distAL = new ArrayList();
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    distAL.Add((T)reader[colName]);
//                }
//                connection.Close();
//                output = new T[distAL.Count];
//                distAL.CopyTo(output);
//            }
//            return output;
//        }

//        public static T[] GetColDataFromDB<T>(string tableName, string colName, string whereCondition)
//        {
//            T[] output;
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                if (whereCondition != "")
//                {
//                    command.CommandText = "SELECT " + colName + " FROM " + tableName
//                        + " WHERE " + whereCondition;
//                }
//                else
//                {
//                    command.CommandText = "SELECT " + colName + " FROM " + tableName;
//                }
//                OracleDataReader reader = command.ExecuteReader();

//                ArrayList distAL = new ArrayList();
//                CultureInfo provider = CultureInfo.InvariantCulture;

//                while (reader.Read())
//                {
//                    if(typeof(T) == typeof(double))
//                    {
//                        distAL.Add((double)((decimal)reader[colName]));
//                    }
//                    else
//                    {
//                        distAL.Add((T)(reader[colName]));
//                    }
//                }
//                connection.Close();
//                output = new T[distAL.Count];
//                distAL.CopyTo(output);
//            }
//            return output;
//        }

//        public static DataTable GetSQLqueryDataFromDB(string SQLquery)
//        {
//            DataTable output = new DataTable();
//            string connectionString = connstr + passwrd;

//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                command.CommandText = SQLquery;
//                OracleDataReader reader = command.ExecuteReader();
//                OracleDataAdapter OracAdapter = new OracleDataAdapter(SQLquery, connection);
//                OracAdapter.Fill(output);                
//                connection.Close();                
//            }
//            return output;
//        }

//        public static void LoadData2CashFNO1MinTable(string fileName, string tableName)
//        {
//            string connectionString = connstr + passwrd;

//            StreamReader sr = new StreamReader(fileName);
//            sr.ReadLine();
//            string values = "";
//            string sqlCommand = "INSERT INTO "+tableName+" VALUES (";
//            StringBuilder repstr = new StringBuilder();
//            int cnt = 0;
//            using (OracleConnection connection = new OracleConnection())
//            {
//                connection.ConnectionString = connectionString;
//                connection.Open();
//                OracleCommand command = connection.CreateCommand();
//                while (sr.Peek() >= 0)
//                {
//                    string line = sr.ReadLine();
//                    cnt++;
//                    string[] fields = line.Split(',');
//                    values = "'" + fields[0] + "'";
//                    for (int i = 1; i < fields.Length; i++)
//                    {
//                        string str;
//                        if (i == 1 || i == 2 || i == 3 || i == 4 || i == 6)
//                        {
//                            str = "'" + fields[i] + "'";
//                        }
//                        else
//                        {
//                            str = fields[i];
//                        }
//                        values = values + "," + str ;
//                    }

//                    command.CommandText = sqlCommand + values + ")";
//                    command.ExecuteNonQuery();                    
//                }                
//                sr.Close();
//                connection.Close();
//            }
//        }    
//    }

//    public class DBDataFormat
//    {
//        public DateTime[] TimeStamp;
//        public string[] Trade_Sym;
//        public string[] Instrument;
//        public DateTime[] Expiry;
//        public string[] OptionType;
//        public decimal[] Strike;
//        public decimal[] LTP;
//        public decimal[] Volume;
//        //public double[] VWAP;

//        public DBDataFormat()
//        {
//        }

//        public DBDataFormat(int n)
//        {
//            TimeStamp = new DateTime[n];
//            Trade_Sym = new string[n];
//            Instrument = new string[n];
//            Expiry = new DateTime[n];
//            OptionType = new string[n];
//            Strike = new decimal[n];
//            LTP = new decimal[n];
//            Volume = new decimal[n];
//           // VWAP = new double[n];
//        }
//    }

//    public class DB1MinData
//    {
//        public DateTime[] Date { get; set; }
//        public string InstName { get; set; }
//        public string InstType { get; set; }
//        public DateTime[] Expiry { get; set; }
//        public double Strike { get; set; }
//        public string CallPut { get; set; }
//        public DateTime[] TimeArr { get; set; }
//        public double[] BidArr { get; set; }
//        public double[] AskArr { get; set; }
//        public double[] LTPArr { get; set; }
//        public double[] VolArr { get; set; }

//        public DB1MinData()
//        {

//        }

//        public DB1MinData(int n)
//        {
//            this.TimeArr = new DateTime[n];            
//            this.BidArr = new double[n];
//            this.AskArr = new double[n];
//            this.LTPArr = new double[n];
//            this.VolArr = new double[n];           
//        }       

//        public void FillData(DateTime[] date, string instName, string instType, 
//            DateTime[] expiry, double strike, string callPut)
//        {
//            if (date.Length > 0)
//            {
//                this.Date = date;
//                this.Expiry = expiry;
//                this.InstName = instName;
//                this.InstType = instType;
//                this.Strike = strike;
//                this.CallPut = callPut;

//                for (int i = 0; i < date.Length; i++)
//                {
//                    string tableName = "CASH_FNO_1MIN_" + date[i].ToString("MMMyy").ToUpper();
//                    string sqlQuery = "SELECT DATE_VAL, TIME_VAL, BID, ASK, LTP, VOLUME FROM " + tableName
//                        + " WHERE DATE_VAL = '" + date[i].ToString("dd-MMM-yyyy") + "' AND INSTRNAME = '" + instName
//                        + "' AND INSTRTYPE = '" + instType + "'";
//                    if (expiry[i].Year != 01)
//                    {
//                        sqlQuery = sqlQuery + " AND EXPIRY = '" + expiry[i].ToString("dd-MMM-yyyy") + "'";
//                    }
//                    if (strike > 0.0)
//                    {
//                        sqlQuery = sqlQuery + " AND STRIKE = " + strike.ToString();
//                    }
//                    if (callPut != "")
//                    {
//                        sqlQuery = sqlQuery + " AND CALLPUT = '" + callPut + "'";
//                    }

//                    //sqlQuery = sqlQuery; //+ " ORDER BY TIME_VAL ASC";

//                    DB1MinData temp = DBData.Get1MinDataFromDB(sqlQuery);

//                    SortByTimeArr(temp);

//                    this.TimeArr = UF.AppendArray<DateTime>(this.TimeArr,temp.TimeArr);
//                    this.BidArr = UF.AppendArray<double>(this.BidArr,temp.BidArr);
//                    this.AskArr = UF.AppendArray<double>(this.AskArr, temp.AskArr);
//                    this.LTPArr = UF.AppendArray<double>(this.LTPArr, temp.LTPArr);
//                    this.VolArr = UF.AppendArray<double>(this.VolArr,temp.VolArr);
                    
//                }                
//            }
//        }

//        public void FillDataOptions(DateTime[] date, string instName, string instType,
//           DateTime[] expiry, double[] strike, string callPut)
//        {
//            if (date.Length > 0)
//            {
//                this.Date = date;
//                this.Expiry = expiry;
//                this.InstName = instName;
//                this.InstType = instType;
//                this.Strike = strike[0];
//                this.CallPut = callPut;

//                for (int i = 0; i < date.Length; i++)
//                {
//                    string tableName = "CASH_FNO_1MIN_" + date[i].ToString("MMMyy").ToUpper();
//                    string sqlQuery = "SELECT DATE_VAL, TIME_VAL, BID, ASK, LTP, VOLUME FROM " + tableName
//                        + " WHERE DATE_VAL = '" + date[i].ToString("dd-MMM-yyyy") + "' AND INSTRNAME = '" + instName
//                        + "' AND INSTRTYPE = '" + instType + "'";
//                    if (expiry[i].Year != 01)
//                    {
//                        sqlQuery = sqlQuery + " AND EXPIRY = '" + expiry[i].ToString("dd-MMM-yyyy") + "'";
//                    }
//                    if (strike[i] > 0.0)
//                    {
//                        sqlQuery = sqlQuery + " AND STRIKE = " + strike[i].ToString();
//                    }
//                    if (callPut != "")
//                    {
//                        sqlQuery = sqlQuery + " AND CALLPUT = '" + callPut + "'";
//                    }

//                    //sqlQuery = sqlQuery; //+ " ORDER BY TIME_VAL ASC";

//                    DB1MinData temp = DBData.Get1MinDataFromDB(sqlQuery);

//                    SortByTimeArr(temp);

//                    this.TimeArr = UF.AppendArray<DateTime>(this.TimeArr, temp.TimeArr);
//                    this.BidArr = UF.AppendArray<double>(this.BidArr, temp.BidArr);
//                    this.AskArr = UF.AppendArray<double>(this.AskArr, temp.AskArr);
//                    this.LTPArr = UF.AppendArray<double>(this.LTPArr, temp.LTPArr);
//                    this.VolArr = UF.AppendArray<double>(this.VolArr, temp.VolArr);
//                }
//            }
//        }

//        private void SortByTimeArr(DB1MinData data)
//        {
//            int[] idxSort = UF.BubbleSortIdx(data.TimeArr, true);
//            TimeSpan ts = new TimeSpan(15,30,0);
//            int i;
//            for (i = 0; i < idxSort.Length; i++)
//            {
//                if (data.TimeArr[i].TimeOfDay > ts)
//                {
//                    break;
//                }
//            }
//            idxSort = UF.GetRange(idxSort, 0, i - 1);
//            data.TimeArr = UF.GetIndexVals(data.TimeArr, idxSort);
//            data.BidArr = UF.GetIndexVals(data.BidArr, idxSort);
//            data.AskArr = UF.GetIndexVals(data.AskArr, idxSort);
//            data.LTPArr = UF.GetIndexVals(data.LTPArr, idxSort);
//            data.VolArr = UF.GetIndexVals(data.VolArr, idxSort);
//        }

//        public  SpreadData CalculateSpread(DB1MinData data)
//        {
//            DateTime[] CommonDT = NF.IntersectSortedDateTimes(this.TimeArr, data.TimeArr);
//            SpreadData res = new SpreadData(CommonDT.Length);

//            for (int i = 0; i < CommonDT.Length; i++)
//            {
//                int i1 = Array.BinarySearch<DateTime>(this.TimeArr, CommonDT[i]);
//                int i2 = Array.BinarySearch<DateTime>(data.TimeArr, CommonDT[i]);

//                res.dateTime[i] = CommonDT[i];
//                res.ShortRoll[i] = 10000 * (data.BidArr[i2] - this.AskArr[i1]);              
//                res.LongRoll[i] = 10000 * (this.BidArr[i1] - data.AskArr[i2]);                
//            }
//            return res;
//        }
//    }    
//}
