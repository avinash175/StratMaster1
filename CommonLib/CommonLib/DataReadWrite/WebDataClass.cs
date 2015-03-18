using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Net.Security;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

namespace CommonLib
{
    public class WebDataClass
    {
        private string uname;
        private string upass;
        private string server;
        private int port;

        public WebDataClass()
        {
            uname = "";
            server = "";
            port = 0;
            upass = "";
        }

        public WebDataClass(string server_,int port_, string uname_, string upass_)
        {
            uname = uname_;
            server = server_;
            port = port_;
            upass = upass_;
        }

        public void PrependHTTP(ref string strURL)
        {

            if (strURL.Length < 7 || strURL.Substring(0, 7).ToUpper() != "HTTP://")
            {
                if (strURL.Substring(0, 8).ToUpper() != "HTTPS://")
                    strURL = "http://" + strURL;
            }

        }        

        public void DownloadCSVFromURL(string url_,string path_fileName, int field)
        {
            IWebProxy proxyObj = new WebProxy(server, port);
            ICredentials icred = new NetworkCredential(uname, upass);
            
            WebClient wc = new WebClient();

            proxyObj.Credentials = icred;
            wc.Proxy = proxyObj;
            string url = url_;
            PrependHTTP(ref url);
           
            byte[] data = wc.DownloadData(url);
                       
            string strData = Encoding.ASCII.GetString(data);
           
            FileWrite fw = new FileWrite(path_fileName);
            fw.DataWrite(strData);            
        }

        public string DownloadFromURL3(string url_, string fileNameInZip)
        {           
            IWebProxy proxyObj = new WebProxy(server, port);//10.15.1.2: 8080
            ICredentials icred = new NetworkCredential(uname, upass);//shivaram // edel
            
            WebClient wc = new WebClient();
            proxyObj.Credentials = icred;
            wc.Proxy = proxyObj;
            wc.Credentials = new NetworkCredential(uname, upass);
            //wc.Proxy = 
            string url = url_;
            PrependHTTP(ref url);
            string fileNameStr = "";
            //InitiateSSLTrust();
                        
            wc.DownloadFile(url, "tempBhav.zip");
            FastZip fz = new FastZip();
            fz.ExtractZip("tempBhav.zip", Directory.GetCurrentDirectory(),"");
            File.Delete("tempBhav.zip");
            fileNameStr = fileNameInZip;       

            FileRead fr = new FileRead(Directory.GetCurrentDirectory() + "\\" + fileNameStr);
            string strData = fr.ReadRaw();

            return strData;
        }

        // Used in MSTRATS
        public string DownloadFileFromURL(string url_, string path_fileName, bool useProxy)
        {
            WebClient wc = new WebClient();
            string url = url_;
            PrependHTTP(ref url);


            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            IWebProxy proxyObj = myWebRequest.Proxy;

            //IWebProxy proxyObj = new WebProxy(server, port);            
            ICredentials icred = new NetworkCredential(uname, upass);
            proxyObj.Credentials = icred;
            

            if (useProxy)
            {
                InitiateSSLTrust();
                wc.Proxy = proxyObj;
            }
                       
            byte[] data = wc.DownloadData(url);
            char[] hdata = new char[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                hdata[i] = (char)(255 - data[i]);
                //hdata[i] = (char)(data[i]);
            }
            StreamWriter sw = new StreamWriter(path_fileName);
            sw.Write(hdata);
            sw.Close();

            FileRead fr = new FileRead(path_fileName);
            string temp1 = fr.ReadRaw();
            char[] hdata_file = temp1.ToCharArray();

            char[] data_file = new char[hdata_file.Length];
            for (int i = 0; i < hdata_file.Length; i++)
                data_file[i] = (char)(255 - (byte)hdata_file[i]);

            string strr = new string(data_file);
            return strr;
                       
        }

        public void DownloadFileFromURL(string url_, string path_fileName, 
            string account = "Avinash", string pass = "20111203", bool login = false)
        {
            WebClient wc = new WebClient();
            string url = url_;
            PrependHTTP(ref url);

            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            IWebProxy proxyObj = myWebRequest.Proxy;

            ICredentials icred = new NetworkCredential(uname, upass);
            proxyObj.Credentials = icred;

            InitiateSSLTrust();
            wc.Proxy = proxyObj;

            if (login)
            {
                NameValueCollection postvals = new NameValueCollection();
                postvals.Add("login", account);
                postvals.Add("password", pass);
                //postvals.Add("uselandingpage", "1"); 

                Byte[] responseArray = wc.UploadValues(url, "POST", postvals);

                StreamReader strRdr = new StreamReader(new MemoryStream(responseArray));

                string cookiestr = wc.ResponseHeaders.Get("Set-Cookie");

                wc.Headers.Add("Cookie", cookiestr);
            }


            wc.DownloadFile(url, path_fileName);
        }

        public double[,] DownloadMultiCSVFromURL(string url_, string path_fileName, int numOfFields)
        {
            IWebProxy proxyObj = new WebProxy(server, port);
            ICredentials icred = new NetworkCredential(uname, upass);
            WebClient wc = new WebClient();

            proxyObj.Credentials = icred;
            wc.Proxy = proxyObj;
            string url = url_;
            PrependHTTP(ref url);

            byte[] data = wc.DownloadData(url);
            string strData = Encoding.ASCII.GetString(data);

            FileWrite fw = new FileWrite(path_fileName);
            fw.DataWrite(strData);
            FileRead fr = new FileRead(path_fileName);
            return fr.CSVDataExtractMultiVar(numOfFields,0);

        }

        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(delegate { return true; });
            }
            catch (Exception ex)
            {
               
            }
        }

    }
}
