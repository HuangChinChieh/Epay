using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Google.Authenticator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BackendFunction {
    public  JArray GetWithdrawBankSettingData(string CurrencyType)
    {
        JArray RetValue;
        string path = "";
        //初始化設定檔資料
        if (CurrencyType == "TWD")
        {
             path = Pay.ProviderSettingPath + "\\" + "withdrawBankTW.json";
        }
        else if (CurrencyType == "JPY")
        {
            path = Pay.ProviderSettingPath + "\\" + "withdrawBankJPY.json";
        }
        else {
             path = Pay.ProviderSettingPath + "\\" + "withdrawBankCN.json";
        }

        string jsonContent;
        string jsonArrayContent;
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                jsonContent = sr.ReadToEnd();
            }
        }
        jsonArrayContent = JsonConvert.DeserializeObject<JObject>(jsonContent)["BankCodeSettings"].ToString();
        RetValue = JsonConvert.DeserializeObject<JArray>(jsonArrayContent);
        return RetValue;
    }

    public DBModel.Admin TestCheckLogin(FromBody.Login login)
        {

            BackendDB backendDB = new BackendDB();
            DBModel.Admin admin;
            string MD5Input;

            admin = backendDB.GetAdminByLoginAccount(login.LoginAccount);

            if (admin != null)
            {
                    return admin;
            }

            return null;
        }

        public DBModel.AdminWithLoginPassword CheckLogin(FromBody.Login login) {

            BackendDB backendDB = new BackendDB();
            DBModel.AdminWithLoginPassword admin;
            string MD5Input;

            admin = backendDB.GetAdminByLoginAccountWithLoginPassword(login.LoginAccount);

            if (admin != null) {
                //MD5Input = CodingControl.GetMD5(login.Password);
                if (login.Password.ToUpper() == CodingControl.GetSHA256(admin.LoginPassword+ login.UserKey,false).ToUpper())
                {
                    return admin;
                }
            }

            return null;
        }

        public bool CheckPassword(string Password,int AdminID)
        {

            BackendDB backendDB = new BackendDB();
            DBModel.AdminWithLoginPassword admin;
            string MD5Input;

            admin = backendDB.GetAdminByLoginAdminIDWithLoginPassword(AdminID);

            if (admin != null)
            {
                MD5Input = CodingControl.GetMD5(Password);
                if (MD5Input == admin.LoginPassword)
                {
                    return true;
                }
            }

            return false;
        }

        public DBModel.GoogleQrCode GetGoogleQrCode(string CompanyName) {
            //建立一把用戶的私鑰
            var retValue = new DBModel.GoogleQrCode();
            string AccountSecretKey = Guid.NewGuid().ToString("N").Substring(0, 10);
            retValue.GoogleKey = AccountSecretKey;
  
            //產生QR Code.
            var tfa = new TwoFactorAuthenticator();
            //第一個參數為 Title
            //第二個參數為 User 代碼
            //第三個參數為 SecreKey，這裡很簡單的只使用 Key，真實的環境請自行設置夠安全的 SecreKey
            //第四和第五個參數為 QRCode 的寬和高
            var setupInfo = tfa.GenerateSetupCode(Pay.SiteTitle, CompanyName, AccountSecretKey, false, 6);
            //用內建的API 產生
            retValue.ImageUrl =setupInfo.QrCodeSetupImageUrl;
            retValue.ManualEntryKey = setupInfo.ManualEntryKey;
            return retValue;
        }

        public bool CheckGoogleKey(string GoogleKey,string UserKey)
        {
            bool retValue = false;

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            //第一個參數是你當初產生QRcode 所產生的Secret code 
            //第二個參數是用戶輸入的純數字Code
            retValue = tfa.ValidateTwoFactorPIN(GoogleKey, UserKey);
            return retValue;
        }

        //隨機擇一专属供應商群组
        public static int SelectProxyProviderGroup(string ProxyProviderCode,decimal OrderAmount)
        {
            //回傳值
            int returnValue = 1;
            //權重隨機結果
            int randomWeight;
            //總權種數
            int totalWeight = 0;
            List<DBModel.ProxyProviderGroup> ProxyProviderGroupModel = null;
            BackendDB backendDB = new BackendDB();
            //0=启用/1=停用
            ProxyProviderGroupModel= backendDB.GetProxyProviderGroupByState(ProxyProviderCode,0);

            if (ProxyProviderGroupModel != null) {
                ProxyProviderGroupModel= ProxyProviderGroupModel.Where(x => {
                    //檢查上下限制
                    if (OrderAmount > x.MaxAmount || OrderAmount < x.MinAmount)
                    {
                        return false;
                    }

                    //if (x.WithdrawingCount >= x.CanWithdrawingCount )
                    //{
                    //    return false;
                    //}

                    return true;
                }).ToList();

                if (ProxyProviderGroupModel.Count == 0) {
                    ProxyProviderGroupModel = backendDB.GetProxyProviderGroupByState(ProxyProviderCode, 0);
                }
        
                foreach (var SelectModel in ProxyProviderGroupModel)
                {
                    totalWeight += SelectModel.Weight;
                }
                //產生隨機數，方式可能需要再調整，故此處帶入整個陣列

                System.Random ran = new System.Random(GetRandomSeed());
                randomWeight = (ran.Next(totalWeight)) + 1;

                int calWeight = 0;
                for (int i = 0; i < ProxyProviderGroupModel.Count; i++)
                {
                    calWeight += ProxyProviderGroupModel[i].Weight;
                    if (calWeight >= randomWeight)
                    {
                        returnValue = ProxyProviderGroupModel[i].GroupID;
                        break;
                    }
                }
            }
            return returnValue;
        }

        //隨機擇一专属供應商群组(商户已绑定出款群组)
        public static int SelectProxyProviderGroupByCompanySelected(string ProxyProviderCode, decimal OrderAmount,string ProviderGroups)
        {
            //回傳值
            int returnValue = 1;
            //權重隨機結果
            int randomWeight;
            //總權種數
            int totalWeight = 0;
            List<DBModel.ProxyProviderGroup> ProxyProviderGroupModel = null;
            BackendDB backendDB = new BackendDB();

            var LstProviderGroups = ProviderGroups.Split(',').ToList();
            //0=启用/1=停用
            ProxyProviderGroupModel = backendDB.GetProxyProviderGroupByState(ProxyProviderCode, 0);
            ProxyProviderGroupModel = ProxyProviderGroupModel.Where(w => LstProviderGroups.Contains(w.GroupID.ToString())).ToList();
            if (ProxyProviderGroupModel != null)
            {
                ProxyProviderGroupModel = ProxyProviderGroupModel.Where(x => {
                    //檢查上下限制
                    if (OrderAmount > x.MaxAmount || OrderAmount < x.MinAmount)
                    {
                        return false;
                    }

                    //if (x.WithdrawingCount >= x.CanWithdrawingCount)
                    //{
                    //    return false;
                    //}

                    return true;
                }).ToList();

                if (ProxyProviderGroupModel.Count == 0)
                {
                    ProxyProviderGroupModel = backendDB.GetProxyProviderGroupByState(ProxyProviderCode, 0);
                    foreach (var SelectModel in ProxyProviderGroupModel)
                    {
                        totalWeight += SelectModel.Weight;
                    }
                    //產生隨機數，方式可能需要再調整，故此處帶入整個陣列

                    System.Random ran = new System.Random(GetRandomSeed());
                    randomWeight = (ran.Next(totalWeight)) + 1;

                    int calWeight = 0;
                    for (int i = 0; i < ProxyProviderGroupModel.Count; i++)
                    {
                        calWeight += ProxyProviderGroupModel[i].Weight;
                        if (calWeight >= randomWeight)
                        {
                            returnValue = ProxyProviderGroupModel[i].GroupID;
                            break;
                        }
                    }
                }
                else {
               
                    foreach (var SelectModel in ProxyProviderGroupModel)
                    {
                        totalWeight += 1;
                    }
                    //產生隨機數，方式可能需要再調整，故此處帶入整個陣列

                    System.Random ran = new System.Random(GetRandomSeed());
                    randomWeight = (ran.Next(totalWeight)) + 1;

                    int calWeight = 0;
                    for (int i = 0; i < ProxyProviderGroupModel.Count; i++)
                    {
                        calWeight += 1;
                        if (calWeight >= randomWeight)
                        {
                            returnValue = ProxyProviderGroupModel[i].GroupID;
                            break;
                        }
                    }
                }

                
            }
            return returnValue;
        }

    #region  Geo
    public string CheckIPInTW(string IP)
    {
        return IP;
        //try
        //{
        //    var GeoCode = GetGeoCode(IP);
        //    if (GeoCode.GeoCountry == "TW")
        //    {
        //        var secret = aesEncryptBase64(IP);
        //        return secret;
        //    }
        //    else
        //    {
        //        return IP;
        //    }
        //}
        //catch (Exception)
        //{
        //    return IP;
        //    throw;
        //}
    }
     
        public class GeoClass
        {
            public string GeoCode { get; set; }
            public string GeoCountry { get; set; }
            public string GeoName { get; set; }
            public string GeoNameCN { get; set; }
            public string GPS { get; set; }
            public string Timezone { get; set; }
            public string ISPName { get; set; }
            public string ASN { get; set; }
        }

        public  GeoClass GetGeoCode(string IP)
        {
            GeoClass GC = null;
            Dictionary<string, object> GeoRet = null;
            MaxMind.Db.Reader GeoDB = null;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    GeoDB = new MaxMind.Db.Reader(Pay.GeoIPDatabase);
                    GeoRet = GeoDB.Find<Dictionary<string, object>>(System.Net.IPAddress.Parse(IP));
                    break;
                }
                catch (Exception ex)
                {
                    if (GeoDB != null)
                        GeoDB.Dispose();

                    GeoDB = null;
                }
                finally
                {
                    if (GeoDB != null)
                        GeoDB.Dispose();

                    GeoDB = null;
                }
            }

            if (GeoRet != null)
            {
                string continent = string.Empty;
                string country = string.Empty;
                string city = string.Empty;
                MaxMind.Db.Reader AsnDB = null;
                Dictionary<string, object> AsnRet = null;

                if (string.IsNullOrEmpty(Pay.AsnDatabase) == false)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            AsnDB = new MaxMind.Db.Reader(Pay.AsnDatabase);
                            AsnRet = AsnDB.Find<Dictionary<string, object>>(System.Net.IPAddress.Parse(IP));
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (AsnDB != null)
                                AsnDB.Dispose();

                            AsnDB = null;
                        }
                        finally
                        {
                            if (AsnDB != null)
                                AsnDB.Dispose();

                            AsnDB = null;
                        }
                    }
                }

                GC = new GeoClass();

                try { continent = (string)MaxMindFindValue(GeoRet, "continent.code"); }
                catch (Exception ex) { }

                if (string.IsNullOrEmpty(continent) == false)
                {
                    GC.GeoCode = continent;
                    GC.GeoName = (string)MaxMindFindValue(GeoRet, "continent.names.en");
                    GC.GeoNameCN = (string)MaxMindFindValue(GeoRet, "continent.names.zh-CN");

                    country = (string)MaxMindFindValue(GeoRet, "country.iso_code");
                    if (string.IsNullOrEmpty(country) == false)
                    {
                        Dictionary<string, object> LangDict;

                        GC.GeoCode += "." + country;
                        GC.GeoCountry = country;

                        LangDict = (Dictionary<string, object>)MaxMindFindValue(GeoRet, "country.names");
                        if (LangDict != null)
                        {
                            if (LangDict.ContainsKey("en"))
                            {
                                GC.GeoName = (string)LangDict["en"];
                            }

                            if (LangDict.ContainsKey("zh-CN"))
                            {
                                try { GC.GeoNameCN = (string)LangDict["zh-CN"]; }
                                catch (Exception ex) { }
                            }
                        }

                        city = (string)MaxMindFindValue(GeoRet, "subdivisions[0].iso_code");
                        if (city != null)
                        {
                            Dictionary<string, object> LangDict2;

                            GC.GeoCode += "." + city;
                            LangDict2 = (Dictionary<string, object>)MaxMindFindValue(GeoRet, "subdivisions[0].names");
                            if (LangDict2 != null)
                            {
                                if (LangDict2.ContainsKey("en"))
                                {
                                    GC.GeoName += "." + (string)LangDict2["en"];
                                }

                                if (LangDict2.ContainsKey("zh-CN"))
                                {
                                    try { GC.GeoNameCN += "." + (string)LangDict2["zh-CN"]; }
                                    catch (Exception ex) { }
                                }
                            }
                        }
                    }
                }

                GC.GPS = ((double)MaxMindFindValue(GeoRet, "location.latitude")).ToString() + "," + ((double)MaxMindFindValue(GeoRet, "location.longitude")).ToString();
                GC.Timezone = (string)MaxMindFindValue(GeoRet, "location.time_zone");

                if (AsnRet != null)
                {
                    GC.ASN = MaxMindFindValue(AsnRet, "autonomous_system_number").ToString();
                    GC.ISPName = (string)MaxMindFindValue(AsnRet, "autonomous_system_organization");
                }
            }

            return GC;
        }

        private  object MaxMindFindValue(System.Collections.Generic.Dictionary<string, object> o, string path)
        {
            int tmpIndex;
            string findKey;
            bool hasIndex = false;
            int keyIndex = 0;

            tmpIndex = path.IndexOf(".");
            if (tmpIndex != -1)
            {
                findKey = path.Substring(0, tmpIndex);
                path = path.Substring(tmpIndex + 1);
            }
            else
            {
                findKey = path;
                path = string.Empty;
            }

            tmpIndex = findKey.IndexOf("[");
            if (tmpIndex != -1)
            {
                int tmpIndex2 = findKey.IndexOf("]", tmpIndex + 1);
                if (tmpIndex2 != -1)
                {
                    hasIndex = true;
                    keyIndex = Convert.ToInt32(findKey.Substring(tmpIndex + 1, tmpIndex2 - tmpIndex - 1));
                    findKey = findKey.Substring(0, tmpIndex);
                }
            }

            if (string.IsNullOrEmpty(findKey) == false)
            {
                if (o.ContainsKey(findKey))
                {
                    object v = o[findKey];
                    object tmpValue = null;

                    if (hasIndex)
                    {
                        System.Collections.Generic.List<object> tmpList = null;

                        try { tmpList = (List<object>)v; }
                        catch (Exception ex) { }

                        if (tmpList != null)
                        {
                            if ((tmpList.Count - 1) >= keyIndex)
                            {
                                tmpValue = tmpList[keyIndex];
                            }
                        }
                    }
                    else
                    {
                        tmpValue = v;
                    }

                    if (tmpValue != null)
                    {
                        if (string.IsNullOrEmpty(path))
                        {
                            return tmpValue;
                        }
                        else
                        {
                            object ret = null;

                            ret = MaxMindFindValue((Dictionary<string, object>)tmpValue, path);
                            if (ret == null)
                            {
                                return tmpValue;
                            }
                            else
                            {
                                return ret;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

     
        /// <summary>
        /// 字串加密(非對稱式)
        /// </summary>
        /// <param name="Source">加密前字串</param>
        /// <param name="CryptoKey">加密金鑰</param>
        /// <returns>加密後字串</returns>
        public  string aesEncryptBase64(string SourceStr)
        {

            string CryptoKey = "b09d69911ca34e28bd99292ae16a5dd4";
            string encrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Encoding.UTF8.GetBytes(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception e)
            {
                encrypt = e.Message;
            }
            return encrypt;
        }

        /// <summary>
        /// 字串解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <param name="CryptoKey">解密金鑰</param>
        /// <returns>解密後字串</returns>
        public  string aesDecryptBase64(string SourceStr)
        {
            string CryptoKey = "b09d69911ca34e28bd99292ae16a5dd4";
            string decrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Convert.FromBase64String(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                decrypt = e.Message;
            }
            return decrypt;
        }

       
    }

