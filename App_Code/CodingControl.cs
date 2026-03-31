using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class CodingControl
{
    public static string JSEncodeString(string Content)
    {
        if (Content != null)
        {
            return System.Web.HttpUtility.JavaScriptStringEncode(Content);
        }
        else
        {
            return null;
        }
    }

    public static void WriteXFowardForIP(int AdminOPID)
    {
        string Path = Pay.SharedFolder + "\\AdminOPLogIP\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");
        string OutputContent = "";
        if (System.IO.Directory.Exists(Path) == false)
        {
            try { System.IO.Directory.CreateDirectory(Path); }
            catch (Exception ex) { }
        }

        if (System.IO.Directory.Exists(Path))
        {
            JsonConvert.SerializeObject(HttpContext.Current.Request.Headers);

                OutputContent = "-------------------------------------------------\r\n" + System.DateTime.Now.ToString() + "\r\n" + "AdminOP：" + AdminOPID.ToString() + "," + GetXForwardedFor() + "\r\n";
  
            try {
                    System.IO.File.AppendAllText(Path + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log", OutputContent);
            }
            catch (Exception ex) { }
        }
    }

    public static void WriteXFowardForIP2(int AdminOPID, int type = 0)
    {
        string Path = Pay.SharedFolder + "\\IPERROR\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");
        string OutputContent = "";
        if (System.IO.Directory.Exists(Path) == false)
        {
            try { System.IO.Directory.CreateDirectory(Path); }
            catch (Exception ex) { }
        }

        if (System.IO.Directory.Exists(Path))
        {
            JsonConvert.SerializeObject(HttpContext.Current.Request.Headers);
    
                OutputContent = "-------------------------------------------------\r\n" + System.DateTime.Now.ToString() + "\r\n" + "AdminOP1：" + AdminOPID.ToString() + "," + "X-Forwarded-For:" + HttpContext.Current.Request.Headers["X-Forwarded-For"]+ ";UserHostAddress:" + HttpContext.Current.Request.UserHostAddress + "\r\n" + "Headers:" + HttpContext.Current.Request.Headers + "\r\n";
           
            OutputContent = HttpUtility.UrlDecode(OutputContent);
    
            try
            {
                System.IO.File.AppendAllText(Path + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log", OutputContent);
            }
            catch (Exception ex) { }
        }
    }

    public static void WriteBlackList(string IP)
    {
        string Path = Pay.SharedFolder + "\\BlackList\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");
        string AllPath = System.Web.HttpContext.Current.Server.MapPath("~\\RiskControl");
        if (System.IO.Directory.Exists(Path) == false)
        {
            try { System.IO.Directory.CreateDirectory(Path); }
            catch (Exception ex) { }
        }

        if (System.IO.Directory.Exists(AllPath) == false)
        {
            try { System.IO.Directory.CreateDirectory(AllPath); }
            catch (Exception ex) { }
        }

        if (System.IO.Directory.Exists(Path))
        {
            string OutputContent = "-------------------------------------------------\r\n" + System.DateTime.Now.ToString() + "\r\n" + "IP：" + IP + "," + GetXForwardedFor() + "\r\n";

            try
            {
                System.IO.File.AppendAllText(Path + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log", OutputContent);
                if (File.Exists(AllPath + "\\AllBlackList.log"))
                {
                    string AllText = File.ReadAllText(AllPath + "\\AllBlackList.log");
                    if (!AllText.Contains(IP))
                        AllText += "\r\n" + IP;
                    File.WriteAllText(AllPath + "\\AllBlackList.log", AllText);
                }
                else
                {
                    File.WriteAllText(AllPath + "\\AllBlackList.log", IP);
                }
            }


            catch (Exception ex) { }
        }
    }

    public static void LoginRecord()
    {
        string Path = Pay.SharedFolder + "\\LoginRecord\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");

        if (System.IO.Directory.Exists(Path) == false)
        {
            try { System.IO.Directory.CreateDirectory(Path); }
            catch (Exception ex) { }
        }

        if (System.IO.Directory.Exists(Path))
        {
            string OutputContent = "-------------------------------------------------\r\n" + System.DateTime.Now.ToString() + "\r\n" + "IP：" + GetUserIP() + "," + GetXForwardedFor() + "\r\n";

            try { System.IO.File.AppendAllText(Path + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log", OutputContent); }
            catch (Exception ex) { }
        }
    }

    public static string Base64URLEncode(string SourceString, System.Text.Encoding TextEncoding = null)
    {
        System.Text.Encoding TxtEnc;

        if (TextEncoding == null)
            TxtEnc = System.Text.Encoding.UTF8;
        else
            TxtEnc = TextEncoding;

        return Convert.ToBase64String(TxtEnc.GetBytes(SourceString)).Replace('+', '-').Replace('/', '_');
    }

    public static string Base64URLDecode(string b64String, System.Text.Encoding TextEncoding = null)
    {
        string tmp = b64String.Replace('-', '+').Replace('_', '/');
        string tmp2;
        System.Text.Encoding TxtEnc;

        // 轉換表: '-' -> '+'
        //         '_' -> '/'
        //         c -> c

        if (TextEncoding == null)
            TxtEnc = System.Text.Encoding.UTF8;
        else
            TxtEnc = TextEncoding;

        if ((tmp.Length % 4) == 0)
        {
            tmp2 = tmp;
        }
        else
        {
            tmp2 = tmp + new string('=', 4 - (tmp.Length % 4));
        }

        return TxtEnc.GetString(Convert.FromBase64String(tmp2));
    }

    public static string GetGUID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public static bool GetIsHttps()
    {
        bool RetValue = false;

        if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-Proto"]) == false)
        {
            if (System.Convert.ToString(HttpContext.Current.Request.Headers["X-Forwarded-Proto"]).ToUpper() == "HTTPS")
                RetValue = true;
        }
        else
            RetValue = HttpContext.Current.Request.IsSecureConnection;

        return RetValue;
    }

    public static string GetUserIP()
    {
        string RetValue = string.Empty;

        if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]) == false)
        {
            RetValue = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(RetValue) == false)
            {
                int tmpInt;

                tmpInt = RetValue.IndexOf(",");
                if (tmpInt != -1)
                {
                    RetValue = RetValue.Substring(0, tmpInt);
                }
            }
        }
        else
        {
            RetValue = HttpContext.Current.Request.UserHostAddress;
        }

        IPAddress address;
        if (IPAddress.TryParse(RetValue, out address))
        {
            switch (address.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    if (string.IsNullOrEmpty(RetValue) == false)
                    {
                        int tmpIndex;

                        tmpIndex = RetValue.IndexOf(":");
                        if (tmpIndex != -1)
                        {
                            RetValue = RetValue.Substring(0, tmpIndex);
                        }
                    }
                    return RetValue;
               
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    return RetValue;
                  
                default:
                    // umm... yeah... I'm going to need to take your red packet and...
                    break;
            }
        }

        return RetValue;
    }

    public static string GetXForwardedFor()
    {
        string RetValue = string.Empty;

        if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]) == false)
        {
            RetValue = "GetUserIP:" + HttpContext.Current.Request.Headers["X-Forwarded-For"] + ";UserHostAddress:" + HttpContext.Current.Request.UserHostAddress;
        }
        else
        {
            RetValue = "X-Forwarded-For is empty" + ";UserHostAddress:" + HttpContext.Current.Request.UserHostAddress;
        }


        return RetValue;
    }

    public static bool CheckXForwardedFor()
    {
        bool RetValue = true;
        return RetValue;
        //var gpayIP = new List<string>() { "47.57.7.146", "47.90.122.210", "47.116.48.202", "47.103.41.137", "13.94.39.139", "52.184.37.95", "52.229.204.114", "47.242.46.206", "207.46.156.9"
        //                                 ,"169.56.70.83","10.178.32.23","10.111.65.152","161.202.44.131", "47.104.203.18", "27.102.132.54","172.31.38.85","172.19.254.222","47.242.108.78"};

        var gpayIP = PayDB.GetProxyIPList();

        if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]) == false)
        {
            string XForwarder = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            var XForwarderLst = XForwarder.Split(',');
            if (XForwarderLst.Length > 1)
            {
                for (int j = 1; j < XForwarderLst.Count(); j++)
                {
                    if (!CheckIPInCDNList(XForwarderLst[j].Trim()))
                    {
                        WriteBlackList(XForwarderLst[j].Trim());
                        return RetValue;
                    }

                }
            }

            if (!gpayIP.Contains(HttpContext.Current.Request.UserHostAddress))
            {
                WriteBlackList(HttpContext.Current.Request.UserHostAddress);
                return RetValue;
            }
            RetValue = true;
            return RetValue;

        }
        else
        {
            RetValue = true;
            return RetValue;
        }
    }

    public static bool CheckXForwardedForTest()
    {
        bool RetValue = false;

        //var gpayIP = new List<string>() { "47.57.7.146", "47.90.122.210", "47.116.48.202", "47.103.41.137", "13.94.39.139", "52.184.37.95", "52.229.204.114", "47.242.46.206", "207.46.156.9"
        //                                 ,"169.56.70.83","10.178.32.23","10.111.65.152","161.202.44.131", "47.104.203.18", "27.102.132.54","172.31.38.85","172.19.254.222","47.242.108.78"};

        var gpayIP = PayDB.GetProxyIPList();

        if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]) == false)
        {
            string XForwarder = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            var XForwarderLst = XForwarder.Split(',');
            if (XForwarderLst.Length > 1)
            {
                for (int j = 1; j < XForwarderLst.Count(); j++)
                {
                    if (!gpayIP.Contains(XForwarderLst[j].Trim()) && !CheckIPInCDNList(XForwarderLst[j].Trim()))
                    {
                        WriteBlackList(XForwarderLst[j].Trim());
                        return RetValue;
                    }

                }
            }

            if (!gpayIP.Contains(HttpContext.Current.Request.UserHostAddress) && !CheckIPInCDNList(HttpContext.Current.Request.UserHostAddress))
            {
                WriteBlackList(HttpContext.Current.Request.UserHostAddress);
                return RetValue;
            }
            RetValue = true;
            return RetValue;

        }
        else
        {
            return RetValue;
        }
    }

    public static void UpdateCDNList()
    {
        string json = GetWebTextContent("https://api.cloudflare.com/client/v4/ips");
        JObject IPListJO = JObject.Parse(json);
        
        JObject IP_LIST = JObject.Parse(IPListJO["result"].ToString());
        string[] ipv4_cidrs = IP_LIST["ipv4_cidrs"].ToString().Replace("[", "").Replace("]", "").Replace("\r\n", "").Replace("\"", "").Replace(" ", "").Split(',');

        foreach (string ipv4 in ipv4_cidrs)
        {
            string[] IP = ipv4.Split('/');
            ClaculateIPRange(IP[0].Trim(), Convert.ToInt32(IP[1].Trim()));
        }
    }

    public static void ClaculateIPRange(string gate, int maskint)
    {
        string BinSubmask = "";
        BinSubmask = BinSubmask.PadLeft(maskint, '1').PadRight(32, '0');
        string SubMaskIP = BinToIP(BinSubmask);

        string[] SplitGate = gate.Split('.');
        string[] BinSplitGate = new string[4];
        BinSplitGate[0] = Convert.ToString(Convert.ToInt32(SplitGate[0]), 2).PadLeft(8, '0');
        BinSplitGate[1] = Convert.ToString(Convert.ToInt32(SplitGate[1]), 2).PadLeft(8, '0');
        BinSplitGate[2] = Convert.ToString(Convert.ToInt32(SplitGate[2]), 2).PadLeft(8, '0');
        BinSplitGate[3] = Convert.ToString(Convert.ToInt32(SplitGate[3]), 2).PadLeft(8, '0');

        string BinBroadcastAddress = (BinSplitGate[0] + BinSplitGate[1] + BinSplitGate[2] + BinSplitGate[3]).Substring(0, maskint).PadRight(32, '1');
        string BroadcastAddress = BinToIP(BinBroadcastAddress);

        long IntIP_to = (long)(uint)IPAddress.NetworkToHostOrder(IPAddress.Parse(BroadcastAddress).GetHashCode());

        RedisCache.CDNList.AddCDNList(gate + "/" + SubMaskIP, IntIP_to);

    }

    public static string BinToIP(string BinStr)
    {
        string[] IP = new string[4];
        IP[0] = Convert.ToInt32(BinStr.Substring(0, 8), 2).ToString();
        IP[1] = Convert.ToInt32(BinStr.Substring(8, 8), 2).ToString();
        IP[2] = Convert.ToInt32(BinStr.Substring(16, 8), 2).ToString();
        IP[3] = Convert.ToInt32(BinStr.Substring(24, 8), 2).ToString();

        return IP[0] + "." + IP[1] + "." + IP[2] + "." + IP[3];
    }

    public static bool CheckIPInCDNList(string IP)
    {
        long IPToInt = (long)(uint)IPAddress.NetworkToHostOrder(IPAddress.Parse(IP).GetHashCode());

        StackExchange.Redis.RedisValue[] GetCDNIP = RedisCache.CDNList.GetCDNList(IPToInt);

        if (GetCDNIP.Length > 0)
        {
            string[] value = GetCDNIP[0].ToString().Split('/');
            string gate = value[0];
            string mask = value[1];
            return CheckIPMaskGateway(mask, gate, IP);
        }
        else
            return false;
    }

    private static bool CheckIPMaskGateway(string mask, string gateway, string ip)
    {
        string[] maskList = mask.Split('.');
        string[] gatewayList = gateway.Split('.');
        string[] ipList = ip.Split('.');
        for (int j = 0; j < maskList.Length; j++)
        {
            if ((int.Parse(gatewayList[j]) & int.Parse(maskList[j])) != (int.Parse(ipList[j]) & int.Parse(maskList[j])))
            {
                return false;
            }
        }

        return true;
    }



    public static string RandomPassword(int MaxPasswordChars)
    {
        Random R2 = new Random();

        return RandomPassword(R2, MaxPasswordChars);
    }

    public static string RandomPassword(Random R, int MaxPasswordChars)
    {
        string PasswordString;

        PasswordString = "1234567890ABCDEFGHJKLMNPQRSTUVWXYZ";

        return RandomPassword(R, MaxPasswordChars, PasswordString);
    }

    public static string RandomPassword(Random R, int MaxPasswordChars, string AvailableCharList)
    {
        int I;
        int CharIndex;
        string PasswordString;
        string RetValue;

        RetValue = string.Empty;
        PasswordString = AvailableCharList;
        for (I = 1; I <= MaxPasswordChars; I++)
        {
            CharIndex = R.Next(0, PasswordString.Length - 1);

            RetValue = RetValue + PasswordString.Substring(CharIndex, 1);
        }

        return RetValue;
    }

    public static string GetQueryString()
    {
        string QueryString;
        int QueryStringIndex;

        QueryStringIndex = HttpContext.Current.Request.RawUrl.IndexOf("?");
        QueryString = string.Empty;
        if (QueryStringIndex > 0)
            QueryString = HttpContext.Current.Request.RawUrl.Substring(QueryStringIndex + 1);

        return QueryString;
    }

    public static bool FormSubmit()
    {
        if (HttpContext.Current.Request.HttpMethod.Trim().ToUpper() == "POST")
            return true;
        else
            return false;
    }
    public static void CheckingLanguage(string Lang)
    {
        try
        {
            if (HttpContext.Current.Request["BackendLang"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Lang);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(GetDefaultLanguage());
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GetDefaultLanguage());
            }
        }
        catch (Exception ex)
        {

        }
    }
    public static string GetDefaultLanguage()
    {
        // 取得使用者的語言
        // 傳回: 字串, 代表使用者預設的語言集
        string[] LangArr;
        string Temp;
        string[] TempArr;
        string RetValue;

        Temp = HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
        TempArr = Temp.Split(';');

        LangArr = TempArr[0].Split(',');

        if (LangArr[0].Trim() == string.Empty)
            RetValue = "en-us";
        else
            RetValue = LangArr[0];

        return RetValue;
    }

    public static byte[] GetWebBinaryContent(string URL)
    {
        byte[] HttpContent;
        System.Net.WebClient HttpClient;

        HttpClient = new System.Net.WebClient();
        HttpContent = HttpClient.DownloadData(URL);

        return HttpContent;
    }

    public static string GetWebTextContent(string URL, string Method = "GET", string SendData = "", string CustomHeader = null, string ContentType = null)
    {
        System.Net.HttpWebRequest HttpClient;
        System.Net.HttpWebResponse HttpResponse;
        System.IO.Stream Stm;
        System.IO.StreamReader SR;
        string RetValue;
        byte[] SendBytes;

        System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

        HttpClient = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
        HttpClient.Method = Method;
        HttpClient.Accept = "*/*";
        HttpClient.UserAgent = "Sender";
        HttpClient.KeepAlive = false;

        if (CustomHeader != null)
        {
            foreach (string EachHead in CustomHeader.Split('\r', '\n'))
            {
                if (string.IsNullOrEmpty(EachHead) == false)
                {
                    string TmpString = EachHead.Replace("\r", "").Replace("\n", "");
                    int tmpIndex = -1;
                    string cmd = null;
                    string value = null;

                    tmpIndex = TmpString.IndexOf(":");
                    if (tmpIndex != -1)
                    {
                        cmd = TmpString.Substring(0, tmpIndex).Trim();
                        value = TmpString.Substring(tmpIndex + 1).Trim();

                        if (string.IsNullOrEmpty(cmd) == false)
                        {
                            HttpClient.Headers.Set(cmd, value);
                        }
                    }
                }
            }
        }

        switch (Method.ToUpper())
        {
            case "POST":
                {
                    SendBytes = System.Text.Encoding.Default.GetBytes(SendData);

                    if (ContentType == null)
                        HttpClient.ContentType = "application/x-www-form-urlencoded";
                    else
                        HttpClient.ContentType = ContentType;

                    HttpClient.ContentLength = SendBytes.Length;
                    HttpClient.GetRequestStream().Write(SendBytes, 0, SendBytes.Length);
                    break;
                }
        }

        try
        {
            HttpResponse = (System.Net.HttpWebResponse)HttpClient.GetResponse();
        }
        catch (System.Net.WebException ex)
        {
            HttpResponse = (System.Net.HttpWebResponse)ex.Response;
        }

        Stm = HttpResponse.GetResponseStream();
        SR = new System.IO.StreamReader(Stm);
        RetValue = SR.ReadToEnd();

        Stm.Close();

        try
        {
            HttpResponse.Close();
        }
        catch (System.Net.WebException ex)
        {
        }

        HttpClient = null;

        return RetValue;
    }

    public static int GetStringLength(string S)
    {
        return System.Text.Encoding.Default.GetByteCount(S);
    }

    public static string XMLSerial(object obj)
    {
        System.Xml.Serialization.XmlSerializer XMLSer;
        System.IO.MemoryStream Stm;
        byte[] XMLArray;
        string RetValue;

        XMLSer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
        Stm = new System.IO.MemoryStream();
        XMLSer.Serialize(Stm, obj);

        Stm.Position = 0;

        XMLArray = new byte[Stm.Length - 1 + 1];
        Stm.Read(XMLArray, 0, XMLArray.Length);
        Stm.Dispose();
        Stm = null;

        RetValue = System.Text.Encoding.UTF8.GetString(XMLArray);

        return RetValue;
    }

    public static object XMLDeserial(string xmlContent, Type objType)
    {
        System.Xml.Serialization.XmlSerializer XMLSer;
        System.IO.MemoryStream Stm;
        byte[] XMLArray;
        object RetValue = null;

        if (xmlContent != string.Empty)
        {
            XMLArray = System.Text.Encoding.UTF8.GetBytes(xmlContent);

            Stm = new System.IO.MemoryStream();
            Stm.Write(XMLArray, 0, XMLArray.Length);
            Stm.Position = 0;
            XMLSer = new System.Xml.Serialization.XmlSerializer(objType);

            RetValue = XMLSer.Deserialize(Stm);

            Stm.Dispose();
            Stm = null;
        }

        return RetValue;
    }

    public static string GetMD5(string DataString, bool Base64Encoding = true)
    {
        return GetMD5(System.Text.Encoding.UTF8.GetBytes(DataString), Base64Encoding);
    }

    public static string GetMD5(byte[] Data, bool Base64Encoding = true)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider MD5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hash;
        System.Text.StringBuilder RetValue = new System.Text.StringBuilder();

        hash = MD5Provider.ComputeHash(Data);
        MD5Provider = null;

        if (Base64Encoding)
        {
            RetValue.Append(System.Convert.ToBase64String(hash));
        }
        else
        {
            foreach (byte EachByte in hash)
            {
                string ByteStr = EachByte.ToString("x");

                ByteStr = new string('0', 2 - ByteStr.Length) + ByteStr;
                RetValue.Append(ByteStr);
            }
        }


        return RetValue.ToString();
    }

    public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
    {
        public bool CheckValidationResult(System.Net.ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Net.WebRequest request, int certificateProblem)
        {
            return true;
        }
    }

    public static decimal FormatDecimal(decimal s)
    {
        //decimal iValue;
        //decimal LeftValue;
        //int i = 1;
        //decimal s2;
        //bool IsNegative = false;

        //if (s < 0)
        //    IsNegative = true;

        //s2 = Math.Abs(s);

        //iValue = Math.Floor(s2)/1;

        //LeftValue = s2 % 1;
        //ExtControl.AlertMsg("", "LeftValue="+LeftValue.ToString()+",s2="+ s2.ToString()+ ",iValue=" + iValue.ToString());
        //do
        //{
        //    decimal tmpValue ;
        //    decimal powerNumber = Convert.ToDecimal(Math.Pow(10, i));

        //    tmpValue = (LeftValue * powerNumber % 1);
        //    if (tmpValue == 0)
        //    {
        //        iValue += (LeftValue * powerNumber) * Convert.ToDecimal(Math.Pow(10, -i));

        //        break;
        //    }
        //    else
        //        i += 1;
        //} while (true);

        //if (IsNegative)
        //    return 0 - iValue;
        //else
        //    return iValue;
        return s / 1.000000000000000000000000000000000m;
    }

    public static string RequestJsonAPI(string Url, string JsonString)
    {
        string result = string.Empty;
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            if (!Pay.IsTestSite)
            {
                if (!string.IsNullOrEmpty(Pay.ProxyServerUrl)) {
                    handler.Proxy = new WebProxy(Pay.ProxyServerUrl);
                }
            }

            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);
                    HttpResponseMessage response = null;

                    #region  設定相關網址內容

                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Content-Type 用於宣告遞送給對方的文件型態
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    // 將 data 轉為 json
                    string json = JsonString;
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = client.SendAsync(request).GetAwaiter().GetResult();

                    #endregion
                    #endregion

                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        }
                        else
                        {
                            PayDB.InsertPaymentTransferLog("状态码有误:" + response.StatusCode + ", RequestJsonAPI 回传结果:" + response.Content, 2,"" , "");
                        }

                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    PayDB.InsertPaymentTransferLog("ex:" + ex.Message , 2, "", "");
                }
            }
        }

        return result;
    }

    public static string RequestJsonAPI(string Url, string JsonString, string PaymentSerial, string ProviderCode)
    {
        string result = null;
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            if (!Pay.IsTestSite)
            {
                if (!string.IsNullOrEmpty(Pay.ProxyServerUrl)) {
                    handler.Proxy = new WebProxy(Pay.ProxyServerUrl);
                }
            }

            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);
                    HttpResponseMessage response = null;

                    #region  設定相關網址內容

                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Content-Type 用於宣告遞送給對方的文件型態
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                    // 將 data 轉為 json
                    string json = JsonString;
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = client.SendAsync(request).GetAwaiter().GetResult();

                    #endregion
                    #endregion

                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            //PayDB.InsertPaymentTransferLog("状态码:" + response.StatusCode + ", ReSendWithdraw回传结果:" + result, 2, PaymentSerial, ProviderCode);
                        }
                        else
                        {
                            PayDB.InsertPaymentTransferLog("状态码有误:" + response.StatusCode + ", ReSendWithdraw回传结果:" + response.Content, 2, PaymentSerial, ProviderCode);
                        }
                    }
                    else
                    {
                        PayDB.InsertPaymentTransferLog("ReSendWithdraw:回传为空值", 2, PaymentSerial, ProviderCode);
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    PayDB.InsertPaymentTransferLog("ReSendWithdraw系统错误:" + ex.Message, 2, PaymentSerial, ProviderCode);
                }
            }
        }

        return result;
    }

    public static string RequestJsonAPIByGet(string Url)
    {
        string result = string.Empty;
        using (HttpClientHandler handler = new HttpClientHandler())
        {
            if (!Pay.IsTestSite)
            {
                if (!string.IsNullOrEmpty(Pay.ProxyServerUrl)) {
                    handler.Proxy = new WebProxy(Pay.ProxyServerUrl);
                }
            }
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    #region 呼叫遠端 Web API
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
                    HttpResponseMessage response = null;

                    #region  設定相關網址內容
                    // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Content-Type 用於宣告遞送給對方的文件型態
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    response = client.SendAsync(request).GetAwaiter().GetResult();

                    #endregion
                    #endregion

                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        }

                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    //return ex.Message;
                }
            }
        }

        return result;
    }

    public static string GetSHA256(string DataString, bool Base64Encoding = true)
    {
        return GetSHA256(System.Text.Encoding.UTF8.GetBytes(DataString), Base64Encoding);
    }

    public static string GetSHA256(byte[] Data, bool Base64Encoding = true)
    {
        System.Security.Cryptography.SHA256 SHA256Provider = new System.Security.Cryptography.SHA256CryptoServiceProvider();
        byte[] hash;
        System.Text.StringBuilder RetValue = new System.Text.StringBuilder();

        hash = SHA256Provider.ComputeHash(Data);
        SHA256Provider = null;

        if (Base64Encoding)
        {
            RetValue.Append(System.Convert.ToBase64String(hash));
        }
        else
        {
            foreach (byte EachByte in hash)
            {
                // => .ToString("x2")
                string ByteStr = EachByte.ToString("x");

                ByteStr = new string('0', 2 - ByteStr.Length) + ByteStr;
                RetValue.Append(ByteStr);
            }
        }


        return RetValue.ToString();
    }


    public static string GetGPaySign(string OrderID, decimal OrderAmount, DateTime OrderDateTime, string ServiceType, string CurrencyType, string CompanyCode, string CompanyKey)
    {
        string sign;
        string signStr = "CompanyCode=" + CompanyCode;
        signStr += "&CurrencyType=" + CurrencyType;
        signStr += "&ServiceType=" + ServiceType;
        signStr += "&OrderID=" + OrderID;
        signStr += "&OrderAmount=" + OrderAmount.ToString("#.##");
        signStr += "&OrderDate=" + OrderDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        signStr += "&CompanyKey=" + CompanyKey;

        sign = GetSHA256(signStr, false).ToUpper();

        return sign;
    }

}

