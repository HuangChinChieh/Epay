using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// EWin 的摘要描述
/// </summary>
public static class Pay
{
    public static string SharedFolder = System.Configuration.ConfigurationManager.AppSettings["SharedFolder"];
    public static string DBConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnStr"].ConnectionString;
    //public static string SessionDBConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["SessionDBConnStr"].ConnectionString;
    //public static string RiskControlDBConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["RiskControlDBConnStr"].ConnectionString;
    public static DateTime DateTimeNull = Convert.ToDateTime("1900/1/1");
    public static string DirSplit = "\\";
    public static bool IsTestSite = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsTestSite"]);
    public static string WebRedisConnStr = System.Configuration.ConfigurationManager.AppSettings["WebRedisConnStr"];
    public static string ProxyServerUrl = System.Configuration.ConfigurationManager.AppSettings["ProxyServerUrl"];
    public static string GeoIPDatabase = System.Configuration.ConfigurationManager.AppSettings["GeoIPDatabase"];
    public static string AsnDatabase = System.Configuration.ConfigurationManager.AppSettings["AsnDatabase"];
    public static string ProviderSettingPath = System.Configuration.ConfigurationManager.AppSettings["ProviderSettingFolder"];
    public static string TimeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
    public static string MaintainUrl = System.Configuration.ConfigurationManager.AppSettings["MaintainUrl"];
    public static string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
    public static string SiteTitle = System.Configuration.ConfigurationManager.AppSettings["SiteTitle"];
    
    /// <summary>
    /// 重建 SortKey 與 InsideLevel
    /// </summary>
    /// <param name="CompanyID"></param>
    /// <remarks></remarks>


    public static void CompanyReSortkey(int CompanyID) {
        string SS;
        System.Data.DataTable DT;
        System.Data.DataTable ParentDT;
        System.Data.DataTable ChildDT;
        System.Data.SqlClient.SqlCommand DBCmd;
        string SortKey;
        int InsideLevel;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);
        foreach (System.Data.DataRow EachDR in DT.Rows) {
            if (int.Parse(EachDR["ParentCompanyID"].ToString()) != 0) {
                // 取得上層帳戶編號
                SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@ParentCompanyID";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = System.Data.CommandType.Text;
                DBCmd.Parameters.Add("@ParentCompanyID", System.Data.SqlDbType.Int).Value = EachDR["ParentCompanyID"];
                ParentDT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);
                if (ParentDT.Rows.Count > 0) {
                    // 有上層用戶
                    InsideLevel = int.Parse(ParentDT.Rows[0]["InsideLevel"].ToString()) + 1;
                    SortKey = ParentDT.Rows[0]["SortKey"] + CompanyID.ToString().PadLeft(6, '0');
                } else {
                    // 沒有上層用戶(直接屬於 root)
                    InsideLevel = 0;
                    SortKey = CompanyID.ToString().PadLeft(6, '0');
                }
            } else {
                // 沒有上層用戶(直接屬於 root)
                InsideLevel = 0;
                SortKey = CompanyID.ToString().PadLeft(6, '0');
            }
            
            SS = "UPDATE CompanyTable SET InsideLevel=@InsideLevel, SortKey=@SortKey WHERE CompanyID=@CompanyID";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@InsideLevel", System.Data.SqlDbType.Int).Value = InsideLevel;
            DBCmd.Parameters.Add("@SortKey", System.Data.SqlDbType.VarChar).Value = SortKey;
            DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = EachDR["CompanyID"];
            DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            //更新Redis
            RedisCache.Company.UpdateCompanyByID(int.Parse(EachDR["CompanyID"].ToString()));
      
            //檢查是否有下一層資料需要更新
            SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE ParentCompanyID=@CompanyID";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = EachDR["CompanyID"];
            ChildDT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);
            if (ChildDT.Rows.Count > 0) {
                foreach (System.Data.DataRow EachChildDR in ChildDT.Rows)
                    CompanyReSortkey(int.Parse(EachChildDR["CompanyID"].ToString()));
            }
        }
    }

    private static StackExchange.Redis.ConnectionMultiplexer RedisClient = null;

    public static StackExchange.Redis.IDatabase GetRedisClient(int db = -1) {
        StackExchange.Redis.IDatabase RetValue;

        RedisPrepare();

        if (db == -1) {
            RetValue = RedisClient.GetDatabase();
        } else {
            RetValue = RedisClient.GetDatabase(db);
        }

        return RetValue;
    }

    public static string GetJValue(Newtonsoft.Json.Linq.JObject o, string FieldName, string DefaultValue = null) {
        string RetValue = DefaultValue;

        if (o != null) {
            Newtonsoft.Json.Linq.JToken T;

            T = o[FieldName];
            if (T != null) {
                RetValue = T.ToString();
            }
        }

        return RetValue;
    }

    private static void RedisPrepare() {
        if (RedisClient == null) {
            RedisClient = StackExchange.Redis.ConnectionMultiplexer.Connect(WebRedisConnStr);
        }
    }

    public static bool SaveFileWithForderName(string Filename, byte[] Content, string SaveFolderName)
    {
        string FolderName;
        bool RetValue = false;
        string DirSplit = "\\";
        FolderName = SharedFolder + DirSplit + SaveFolderName;
        if (!System.IO.Directory.Exists(FolderName))
        {
            System.IO.Directory.CreateDirectory(FolderName);
        }
        System.IO.FileStream fs = null;
        bool Success = false;

        if (System.IO.File.Exists(FolderName + DirSplit + Filename))
        {
            try
            {
                fs = System.IO.File.Open(FolderName + DirSplit + Filename, System.IO.FileMode.Truncate, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                Success = true;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
        }
        else
        {
            try
            {
                fs = System.IO.File.Open(FolderName + DirSplit + Filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                Success = true;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
        }

        if (Success)
        {
            if (fs != null)
            {
                fs.Write(Content, 0, Content.Length);
                fs.Close();
                fs.Dispose();
                fs = null;

                RetValue = true;
            }
        }

        return RetValue;
    }
}