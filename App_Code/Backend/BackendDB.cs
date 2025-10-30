using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;

public class BackendDB
{
    private string DBConnStr = Pay.DBConnStr;
    //private string SessionDBConnStr = Pay.SessionDBConnStr;
    //private string RiskControlDBConnStr = Pay.RiskControlDBConnStr;

    #region Company
    public string GetCompanyNameByCompanyID(int CompanyID)
    {
        string returnValue = "";
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT CompanyName FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    public string GetCompanyCodeByCompanyID(int CompanyID)
    {
        string returnValue = "";
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT CompanyCode FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    public string GetCompanyDescriptionByCompanyID(int CompanyID)
    {
        string returnValue = "";
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT Description FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    //public string GetCompanyKeyByCompanyID(int CompanyID)
    //{
    //    string returnValue = "";
    //    string SS;
    //    SqlCommand DBCmd = null;

    //    SS = "SELECT CompanyKey FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

    //    DBCmd = new SqlCommand();
    //    DBCmd.CommandText = SS;
    //    DBCmd.CommandType = System.Data.CommandType.Text;
    //    DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
    //    returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

    //    return returnValue;
    //}

    public List<DBModel.Company> GetCompany(int forCompanyID, int CompanyType)
    {
        List<DBModel.Company> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;
        CompanyData = GetCompanyByID(forCompanyID);
        if (CompanyData != null)
        {
            SortKey = CompanyData.SortKey;
        }


        SS = "SELECT *,(select count(*) from CompanyTable WITH (NOLOCK) where CompanyTable.ParentCompanyID=c.CompanyID ) as ChildCompanyCount FROM CompanyTable c WITH (NOLOCK) WHERE 1=1 And CompanyType<>0 ";
        if (CompanyType != 0)
        {
            if (CompanyType == 1)
            {
                SS += "  AND  c.CompanyID =@CompanyID";
            }
            else
            {
                SS += "  AND  c.SortKey LIKE @SortKey + '%'";
            }
        }
        SS += " order by SortKey ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = SortKey;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = forCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT) as List<DBModel.Company>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Company> GetCompanyByCompanyID(int forCompanyID)
    {
        List<DBModel.Company> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;
        CompanyData = GetCompanyByID(forCompanyID);
        if (CompanyData != null)
        {
            SortKey = CompanyData.SortKey;
        }


        SS = "SELECT * FROM CompanyTable c WITH (NOLOCK) WHERE 1=1 And CompanyType<>0 ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = forCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT) as List<DBModel.Company>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.OffLineCompany> GetOffLineResult(FromBody.GetOffLineResultSet data, int CompanyID)
    {
        List<DBViewModel.OffLineCompany> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;
        CompanyData = GetCompanyByID(CompanyID);
        if (CompanyData != null)
        {
            SortKey = CompanyData.SortKey;
        }


        SS = " SELECT *,pc.CompanyCode as ParentCompanyCode FROM CompanyTable c WITH (NOLOCK) ";
        SS += " LEFT JOIN CompanyTable pc on pc.CompanyID=c.ParentCompanyID";
        SS += " WHERE c.SortKey LIKE @SortKey + '%' And c.CompanyID<>@CompanyID";


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = SortKey;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.OffLineCompany>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Company> GetAgentCompany()
    {
        List<DBModel.Company> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;

        SS = "SELECT * FROM CompanyTable c WITH (NOLOCK) WHERE 1=1 And CompanyType=2 ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT) as List<DBModel.Company>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Company> GetCompanyByCompanyType()
    {
        List<DBModel.Company> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;

        SS = "SELECT * FROM CompanyTable c WITH (NOLOCK) WHERE 1=1 And CompanyType=4 ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT) as List<DBModel.Company>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Company> GetCompany2(int forCompanyID, int CompanyType)
    {
        List<DBModel.Company> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;

        CompanyData = GetCompanyByID(forCompanyID);
        if (CompanyData != null)
        {
            SortKey = CompanyData.SortKey;
        }

     
        SS = "SELECT *,(select count(*) from CompanyTable where CompanyTable.ParentCompanyID=c.CompanyID ) as ChildCompanyCount FROM CompanyTable c WITH (NOLOCK) WHERE 1=1 And CompanyType<>0 ";
        if (CompanyType != 0)
        {
            if (CompanyType == 1)
            {
                SS += "  AND  c.CompanyID =@CompanyID";
            }
            else
            {
                SS += "  AND  c.SortKey LIKE @SortKey + '%' And  c.CompanyID <> @CompanyID";
            }
        }
        SS += " AND CompanyState=0 ";
        SS += " order by SortKey ";


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = SortKey;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = forCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT) as List<DBModel.Company>;
            }
        }

        return returnValue;
    }

    public DBModel.Company GetCompanyByID(int CompanyID)
    {
        DBModel.Company returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.CompanyWithGooleKey GetCompanyByIDWithGooleKey(int CompanyID)
    {
        DBModel.CompanyWithGooleKey returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyWithGooleKey>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public bool CheckCompanyBackendLoginIPTypeByCode(string CompanyCode)
    {
        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;


        SS = "SELECT Count(*) FROM CompanyTable WITH (NOLOCK) WHERE CompanyCode=@CompanyCode And BackendLoginIPType=0 ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;

        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }

        return returnValue;
    }

    public bool CheckCompanyBackendWithdrawIPTypeByCode(string CompanyCode)
    {
        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;


        SS = "SELECT Count(*) FROM CompanyTable WITH (NOLOCK) WHERE CompanyCode=@CompanyCode And BackendWithdrawIPType=0 ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;

        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }

        return returnValue;
    }

    public DBModel.Company GetCompanyByCode(string CompanyCode)
    {
        DBModel.Company returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyCode=@CompanyCode ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Company>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.CompanyWithKey GetCompanyWithKeyByCode(string CompanyCode)
    {
        DBModel.CompanyWithKey returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyCode=@CompanyCode ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyWithKey>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.CompanyWithKey GetCompanyWithKeyByCompanyID(int CompanyID)
    {
        DBModel.CompanyWithKey returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyID=@CompanyID ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyWithKey>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBViewModel.InsertCompanyReturn InsertCompany(DBModel.Company company, int CreateAdminID)
    {

        string CompanyKey = Guid.NewGuid().ToString("N");
        string SS;
        string MerchantCode;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int CompanyID = 0;
        DBViewModel.InsertCompanyReturn returnValue = new DBViewModel.InsertCompanyReturn();

        MerchantCode = GetCompanyMerchantCode();
        DBAccess.ExecuteTransDB(Pay.DBConnStr, T =>
        {
            
            SS = "   INSERT INTO CompanyTable" +
                     "              (CompanyType," +
                     "               CompanyName," +
                     "               CompanyState," +
                     "               CompanyCode," +
                     "               URL," +
                     "               ParentCompanyID," +
                     "               CreateDate," +
                     "               CreateAdminID," +
                     "               CompanyKey," +
                     "               ContacterName," +
                     "               ContacterMobile," +
                     "               ContacterMethod," +
                     "               ContacterMethodAccount," +
                     "               MerchantCode," +
                     "               WithdrawType," +
                     "               AutoWithdrawalServiceType," +
                     "               WithdrawAPIType," +
                     "               BackendLoginIPType," +
                     "               ProviderGroupID," +
                     "               BackendWithdrawType," +
                     "               ContacterEmail," +
                     "               BackendWithdrawIPType," +
                     "               Timezone," +
                     "               CurrencyType)" +
                     "   VALUES" +
                     "              (@CompanyType," +
                     "               @CompanyName," +
                     "               @CompanyState," +
                     "               @CompanyCode," +
                     "               @URL," +
                     "               @ParentCompanyID," +
                     "               Getdate()," +
                     "               @CreateAdminID," +
                     "               @CompanyKey," +
                     "               @ContacterName," +
                     "               @ContacterMobile," +
                     "               @ContacterMethod," +
                     "               @ContacterMethodAccount," +
                     "               @MerchantCode," +
                     "               @WithdrawType," +
                     "               @AutoWithdrawalServiceType," +
                     "               @WithdrawAPIType," +
                     "               @BackendLoginIPType," +
                     "               @ProviderGroupID," +
                     "               @BackendWithdrawType," +
                     "               @ContacterEmail," +
                     "               @BackendWithdrawIPType," +
                     "               @Timezone," +
                     "               @CurrencyType)" +
                     "                      SELECT @@IDENTITY;";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@CompanyType", SqlDbType.Int).Value = company.CompanyType;
            DBCmd.Parameters.Add("@CompanyState", SqlDbType.Int).Value = company.CompanyState;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = company.CompanyCode;
            DBCmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = company.CompanyName;
            DBCmd.Parameters.Add("@URL", SqlDbType.VarChar).Value = String.IsNullOrEmpty(company.URL) ? "" : company.URL;
            DBCmd.Parameters.Add("@ParentCompanyID", SqlDbType.Int).Value = company.ParentCompanyID;
            DBCmd.Parameters.Add("@CreateAdminID", SqlDbType.Int).Value = CreateAdminID;
            DBCmd.Parameters.Add("@WithdrawType", SqlDbType.Int).Value = company.WithdrawType;
            DBCmd.Parameters.Add("@CompanyKey", SqlDbType.VarChar).Value = CompanyKey;
            DBCmd.Parameters.Add("@MerchantCode", SqlDbType.VarChar).Value = MerchantCode;
            DBCmd.Parameters.Add("@ContacterName", SqlDbType.NVarChar).Value = String.IsNullOrEmpty(company.ContacterName) ? "" : company.ContacterName;
            DBCmd.Parameters.Add("@ContacterMobile", SqlDbType.VarChar).Value = String.IsNullOrEmpty(company.ContacterMobile) ? "" : company.ContacterMobile;
            DBCmd.Parameters.Add("@ContacterMethod", SqlDbType.Int).Value = company.ContacterMethod;
            DBCmd.Parameters.Add("@ContacterMethodAccount", SqlDbType.VarChar).Value = String.IsNullOrEmpty(company.ContacterMethodAccount) ? "" : company.ContacterMethodAccount;
            DBCmd.Parameters.Add("@ContacterEmail", SqlDbType.VarChar).Value = String.IsNullOrEmpty(company.ContacterEmail) ? "" : company.ContacterEmail;
            DBCmd.Parameters.Add("@AutoWithdrawalServiceType", SqlDbType.VarChar).Value = company.AutoWithdrawalServiceType;
            DBCmd.Parameters.Add("@WithdrawAPIType", SqlDbType.Int).Value = company.WithdrawAPIType;
            DBCmd.Parameters.Add("@BackendLoginIPType", SqlDbType.Int).Value = company.BackendLoginIPType;
            DBCmd.Parameters.Add("@ProviderGroupID", SqlDbType.Int).Value = company.ProviderGroupID;
            DBCmd.Parameters.Add("@BackendWithdrawType", SqlDbType.Int).Value = company.BackendWithdrawType;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = company.CurrencyType;
            DBCmd.Parameters.Add("@BackendWithdrawIPType", SqlDbType.Int).Value = company.BackendWithdrawIPType;
            DBCmd.Parameters.Add("@Timezone", SqlDbType.Decimal).Value = company.Timezone;
        
            CompanyID = int.Parse(T.GetDBValue(DBCmd).ToString());
        });

        #region 更新Sortkey
        Pay.CompanyReSortkey(CompanyID);
        #endregion
        returnValue.CompanyID = CompanyID;
        returnValue.CompanyKey = CompanyKey;

        return returnValue;
    }

    public int UpdateCompany(DBModel.Company company)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE CompanyTable SET ParentCompanyID=@ParentCompanyID,CompanyName=@CompanyName,CompanyType=@CompanyType, CompanyState=@CompanyState, CompanyCode=@CompanyCode, URL=@URL,ContacterName=@ContacterName,ContacterMobile=@ContacterMobile,ContacterMethod=@ContacterMethod,ContacterMethodAccount=@ContacterMethodAccount,ContacterEmail=@ContacterEmail,WithdrawType=@WithdrawType,AutoWithdrawalServiceType=@AutoWithdrawalServiceType,CheckCompanyWithdrawUrl=@CheckCompanyWithdrawUrl,WithdrawAPIType=@WithdrawAPIType,BackendLoginIPType=@BackendLoginIPType,BackendWithdrawType=@BackendWithdrawType,ProviderGroups=@ProviderGroups,CheckCompanyWithdrawType=@CheckCompanyWithdrawType,Description=@Description,BackendWithdrawIPType=@BackendWithdrawIPType,Timezone=@Timezone " +
             " WHERE CompanyID=@CompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyType", SqlDbType.VarChar).Value = company.CompanyType;
        DBCmd.Parameters.Add("@CompanyState", SqlDbType.VarChar).Value = company.CompanyState;
        DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = company.CompanyCode;
        DBCmd.Parameters.Add("@URL", SqlDbType.VarChar).Value = company.URL;
        DBCmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value = company.CompanyName;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = company.CompanyID;
        DBCmd.Parameters.Add("@WithdrawType", SqlDbType.Int).Value = company.WithdrawType;
        DBCmd.Parameters.Add("@ParentCompanyID", SqlDbType.Int).Value = company.ParentCompanyID;
        DBCmd.Parameters.Add("@ContacterName", SqlDbType.NVarChar).Value = company.ContacterName;
        DBCmd.Parameters.Add("@ContacterMobile", SqlDbType.VarChar).Value = company.ContacterMobile;
        DBCmd.Parameters.Add("@AutoWithdrawalServiceType", SqlDbType.VarChar).Value = company.AutoWithdrawalServiceType;
        DBCmd.Parameters.Add("@ContacterMethod", SqlDbType.Int).Value = company.ContacterMethod;
        DBCmd.Parameters.Add("@ContacterMethodAccount", SqlDbType.VarChar).Value = company.ContacterMethodAccount;
        DBCmd.Parameters.Add("@ContacterEmail", SqlDbType.VarChar).Value = company.ContacterEmail;
        DBCmd.Parameters.Add("@CheckCompanyWithdrawUrl", SqlDbType.VarChar).Value = company.CheckCompanyWithdrawUrl;
        DBCmd.Parameters.Add("@BackendLoginIPType", SqlDbType.Int).Value = company.BackendLoginIPType;
        DBCmd.Parameters.Add("@WithdrawAPIType", SqlDbType.Int).Value = company.WithdrawAPIType;
        DBCmd.Parameters.Add("@BackendWithdrawType", SqlDbType.Int).Value = company.BackendWithdrawType;
        DBCmd.Parameters.Add("@ProviderGroups", SqlDbType.VarChar).Value = company.ProviderGroups;
        DBCmd.Parameters.Add("@CheckCompanyWithdrawType", SqlDbType.Int).Value = company.CheckCompanyWithdrawType;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = company.Description;
        DBCmd.Parameters.Add("@BackendWithdrawIPType", SqlDbType.Int).Value = company.BackendWithdrawIPType;
        DBCmd.Parameters.Add("@Timezone", SqlDbType.Decimal).Value = company.Timezone;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);


        #region 更新Sortkey
        Pay.CompanyReSortkey(company.CompanyID);
        #endregion

        return returnValue;
    }

    public int UpdateCompanyWithdrawlType(DBModel.Company company)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE CompanyTable SET WithdrawType=@WithdrawType,AutoWithdrawalServiceType=@AutoWithdrawalServiceType,WithdrawAPIType=@WithdrawAPIType,BackendLoginIPType=@BackendLoginIPType,BackendWithdrawType=@BackendWithdrawType " +
             " WHERE CompanyID=@CompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = company.CompanyID;
        DBCmd.Parameters.Add("@WithdrawType", SqlDbType.Int).Value = company.WithdrawType;
        DBCmd.Parameters.Add("@AutoWithdrawalServiceType", SqlDbType.VarChar).Value = company.AutoWithdrawalServiceType;
        DBCmd.Parameters.Add("@BackendLoginIPType", SqlDbType.Int).Value = company.BackendLoginIPType;
        DBCmd.Parameters.Add("@WithdrawAPIType", SqlDbType.Int).Value = company.WithdrawAPIType;
        DBCmd.Parameters.Add("@BackendWithdrawType", SqlDbType.Int).Value = company.BackendWithdrawType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);


        #region 更新Sortkey
        Pay.CompanyReSortkey(company.CompanyID);
        #endregion

        return returnValue;
    }

    public int UpdateCompanyGoogleKey(string GoogleKey, int CompanyID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE CompanyTable SET GoogleKey=@GoogleKey " +
             " WHERE CompanyID=@CompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@GoogleKey", SqlDbType.VarChar).Value = GoogleKey;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        #region 更新Sortkey
        Pay.CompanyReSortkey(CompanyID);
        #endregion

        return returnValue;
    }

    public string GetCompanyMerchantCode()
    {
        string returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT MerchantCode FROM CompanyTable WHERE MerchantCode like @MerchantCode Order By MerchantCode desc";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@MerchantCode", System.Data.SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyyMM") + "%";
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = (int.Parse(DT.Rows[0]["MerchantCode"].ToString()) + 1).ToString();
            }
            else
            {
                returnValue = DateTime.Now.ToString("yyyyMM") + "001";
            }
        }

        return returnValue;
    }

    public int DisableCompanyByID(int CompanyID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE CompanyTable SET CompanyState= 1 WHERE CompanyID=@CompanyID"; ;

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        #region 更新Redis
        RedisCache.Company.UpdateCompanyByID(CompanyID);
        #endregion

        return returnValue;
    }

    public int UpdateAllCompanyRedis()
    {
        int CompanyCount = 0;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;



        SS = "SELECT * FROM CompanyTable c WITH (NOLOCK) ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                var CompanyModel = DataTableExtensions.ToList<DBModel.Company>(DT);
                for (int i = 0; i < CompanyModel.Count; i++)
                {
                    var CompanyDT = RedisCache.Company.UpdateCompanyByID(CompanyModel[i].CompanyID);
                    if (CompanyDT != null)
                    {
                        CompanyCount++;
                    }
                }
            }
        }



        return CompanyCount;

    }
    #endregion

    #region Currency
    public List<DBModel.Currency> GetCurrency()
    {
        List<DBModel.Currency> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM CurrencyType WITH (NOLOCK)";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Currency>(DT) as List<DBModel.Currency>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Currency> GetCurrencyByCompanyID(int CompanyID)
    {
        List<DBModel.Currency> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CompanyID == 0)
        {
            SS = "SELECT * FROM CurrencyType WITH (NOLOCK)";
        }
        else
        {
            SS = "SELECT * FROM CompanyPoint WITH (NOLOCK) where forCompanyID=@CompanyID";
        }

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Currency>(DT) as List<DBModel.Currency>;
            }
        }

        return returnValue;
    }

    public int InsertCurrency(DBModel.Currency Currency)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " SELECT COUNT(*) FROM CurrencyType WITH (NOLOCK) " +
                      " WHERE CurrencyType=@CurrencyType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Currency.CurrencyType;
        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }


        SS = "INSERT INTO CurrencyType (CurrencyType) " +
         "                          VALUES (@CurrencyType)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Currency.CurrencyType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    public int UpdateCurrency(DBModel.Currency Currency)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE CurrencyType SET CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Currency.CurrencyType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }
    #endregion

    #region ServiceType
    public string GetServiceTypeNameByServiceType(string ServiceType)
    {
        string returnValue = null;
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT ServiceTypeName FROM ServiceType WITH (NOLOCK) WHERE ServiceType =@ServiceType And CurrencyType='JPY'";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    public string GetServiceTypeNameByServiceType(string ServiceType,string CurrencyType)
    {
        string returnValue = null;
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT ServiceTypeName FROM ServiceType WITH (NOLOCK) WHERE ServiceType =@ServiceType And CurrencyType=@CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    public List<DBViewModel.ServiceTypeVM> GetTestServiceType(int CompanyID, int InsideLevel)
    {
        List<DBViewModel.ServiceTypeVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;


        if (CompanyID == 0)
        {
            SS = " SELECT *,1 as isUpLine FROM ServiceType WITH (NOLOCK)" +
                 //" Join ProviderService WITH (NOLOCK) On ProviderService.ServiceType = ServiceType.ServiceType And ProviderService.CurrencyType = ServiceType.CurrencyType" +
                 " order by ServiceType.CurrencyType";
        }
        else
        {
            //取得SortKey
            CompanyData = GetCompanyByID(CompanyID);
            if (CompanyData != null)
            {
                SortKey = CompanyData.SortKey;
            }

            SS = "SELECT ServiceType.ServiceType,ServiceType.CurrencyType,ServiceType.ServiceTypeName,CollectRate,CollectCharge,MinOnceAmount,MaxOnceAmount,0 as isUpLine," +
                 " ( SELECT CompanyService.MaxDaliyAmount - ISNULL(sum(MaxDaliyAmount),0) from CompanyService cs" +
                 " join CompanyTable ct on cs.forCompanyID = ct.CompanyID" +
                 " where cs.ServiceType = CompanyService.ServiceType" +
                 " and cs.CurrencyType = CompanyService.CurrencyType" +
                 " and SortKey LIKE  @SortKey + '%' and ct.CompanyID<> @CompanyID and InsideLevel=@InsideLevel ) as MaxDaliyAmount" +
                 " FROM CompanyService WITH(NOLOCK)" +
                 " Join ServiceType WITH(NOLOCK) On CompanyService.ServiceType = ServiceType.ServiceType And CompanyService.CurrencyType = ServiceType.CurrencyType" +
                 " Where CompanyService.forCompanyID = @CompanyID" +
                 " Order by CompanyService.CurrencyType";
        }


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@InsideLevel", SqlDbType.Int).Value = InsideLevel;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = SortKey;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ServiceTypeVM>(DT) as List<DBViewModel.ServiceTypeVM>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ServiceTypeVM> GetServiceType(int CompanyID, int InsideLevel)
    {
        List<DBViewModel.ServiceTypeVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        string SortKey = "";
        DBModel.Company CompanyData;


        if (CompanyID == 0)
        {
            SS = " SELECT *,1 as isUpLine FROM ServiceType WITH (NOLOCK)" +
                 //" Join ProviderService WITH (NOLOCK) On ProviderService.ServiceType = ServiceType.ServiceType And ProviderService.CurrencyType = ServiceType.CurrencyType" +
                 " order by ServiceType.CurrencyType";
        }
        else
        {
            //取得SortKey
            CompanyData = GetCompanyByID(CompanyID);
            if (CompanyData != null)
            {
                SortKey = CompanyData.SortKey;
            }

            SS = "SELECT ServiceType.ServiceType,ServiceType.CurrencyType,ServiceType.ServiceTypeName,CollectRate,CollectCharge,MinOnceAmount,MaxOnceAmount,0 as isUpLine," +
                 " ( SELECT CompanyService.MaxDaliyAmount - ISNULL(sum(MaxDaliyAmount),0) from CompanyService cs" +
                 " join CompanyTable ct on cs.forCompanyID = ct.CompanyID" +
                 " where cs.ServiceType = CompanyService.ServiceType" +
                 " and cs.CurrencyType = CompanyService.CurrencyType" +
                 " and SortKey LIKE  @SortKey + '%' and ct.CompanyID<> @CompanyID and InsideLevel=@InsideLevel ) as MaxDaliyAmount" +
                 " FROM CompanyService WITH(NOLOCK)" +
                 " Join ServiceType WITH(NOLOCK) On CompanyService.ServiceType = ServiceType.ServiceType And CompanyService.CurrencyType = ServiceType.CurrencyType" +
                 " Where CompanyService.forCompanyID = @CompanyID" +
                 " Order by CompanyService.CurrencyType";
        }


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@InsideLevel", SqlDbType.Int).Value = InsideLevel;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = SortKey;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ServiceTypeVM>(DT) as List<DBViewModel.ServiceTypeVM>;
            }
        }

        return returnValue;
    }

    public int InsertServiceType(DBModel.ServiceTypeModel Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO ServiceType (ServiceTypeName,ServiceType,CurrencyType,AllowCollect,AllowPay,ServicePaymentType,ServiceSupplyType) " +
         "                          VALUES (@ServiceTypeName,@ServiceType,@CurrencyType,@AllowCollect,@AllowPay,@ServicePaymentType,@ServiceSupplyType)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@ServiceTypeName", SqlDbType.NVarChar).Value = Model.ServiceTypeName;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@AllowCollect", SqlDbType.Int).Value = Model.AllowCollect;
        DBCmd.Parameters.Add("@AllowPay", SqlDbType.Int).Value = Model.AllowPay;
        DBCmd.Parameters.Add("@ServicePaymentType", SqlDbType.Int).Value = Model.ServicePaymentType;
        DBCmd.Parameters.Add("@ServiceSupplyType", SqlDbType.Int).Value = Model.ServiceSupplyType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    public int UpdateServiceType(DBModel.ServiceTypeModel Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ServiceType SET ServiceTypeName=@ServiceTypeName,AllowCollect=@AllowCollect,AllowPay=@AllowPay,ServicePaymentType=@ServicePaymentType,ServiceSupplyType=@ServiceSupplyType WHERE ServiceType=@ServiceType AND CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@ServiceTypeName", SqlDbType.NVarChar).Value = Model.ServiceTypeName;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@AllowCollect", SqlDbType.Int).Value = Model.AllowCollect;
        DBCmd.Parameters.Add("@AllowPay", SqlDbType.Int).Value = Model.AllowPay;
        DBCmd.Parameters.Add("@ServicePaymentType", SqlDbType.Int).Value = Model.ServicePaymentType;
        DBCmd.Parameters.Add("@ServiceSupplyType", SqlDbType.Int).Value = Model.ServiceSupplyType;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }
    #endregion

    #region WithdrawLimit
    public List<DBModel.WithdrawLimit> GetAllWithdrawLimitResultByCompanyID(DBModel.WithdrawLimit data)
    {
        List<DBModel.WithdrawLimit> returnValue = null;
        string SS = "";
        SqlCommand DBCmd = null;
        DataTable DT = null;


        SS = " SELECT WithdrawLimit.* FROM WithdrawLimit " +
             " WHERE forCompanyID =@forCompanyID And WithdrawLimitType=@WithdrawLimitType";

        if (!String.IsNullOrEmpty(SS))
        {
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = data.CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawLimit>(DT) as List<DBModel.WithdrawLimit>;
            }
        }

        return returnValue;
    }

    public List<DBModel.WithdrawLimit> GetWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        List<DBModel.WithdrawLimit> returnValue = null;
        string SS = "";
        SqlCommand DBCmd = null;
        DataTable DT = null;
        // 0=上游後台出款手續費，1=商戶後台出款手續費/2=商戶api 出款手續費/3=上游 api 手續費
        if (data.WithdrawLimitType == 0 || data.WithdrawLimitType == 3)
        { //供應商資料
            SS = "SELECT * FROM WithdrawLimit WHERE ProviderCode =@ProviderCode And WithdrawLimitType=@WithdrawLimitType";
        }
        else if (data.WithdrawLimitType == 1|| data.WithdrawLimitType == 2)
        {   //營運商資料
            SS = " SELECT WithdrawLimit.*,ServiceType.ServiceTypeName FROM WithdrawLimit " +
                 " LEFT JOIN ServiceType ON ServiceType.ServiceType=WithdrawLimit.ServiceType" +
                 " WHERE forCompanyID =@forCompanyID And WithdrawLimitType=@WithdrawLimitType";
        }


        if (!String.IsNullOrEmpty(SS))
        {
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = data.CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawLimit>(DT) as List<DBModel.WithdrawLimit>;
            }
        }

        return returnValue;
    }


    public List<DBModel.WithdrawLimit> GetProviderWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        List<DBModel.WithdrawLimit> returnValue = null;
        string SS = "";
        SqlCommand DBCmd = null;
        DataTable DT = null;


        SS = " SELECT WithdrawLimit.*,ServiceType.ServiceTypeName FROM WithdrawLimit " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=WithdrawLimit.ServiceType" +
             "  WHERE ProviderCode =@ProviderCode And WithdrawLimitType IN (0, 3)";

        if (!String.IsNullOrEmpty(SS))
        {
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawLimit>(DT) as List<DBModel.WithdrawLimit>;
            }
        }

        return returnValue;
    }

    public List<DBModel.ProviderBankCard> GetProviderBankCard(DBModel.WithdrawLimit data)
    {
        List<DBModel.ProviderBankCard> returnValue = null;
        string SS = "";
        SqlCommand DBCmd = null;
        DataTable DT = null;


        SS = " SELECT * , " +
                  "     CASE  " +
                  "         WHEN EXISTS ( " +
                  "               SELECT 1   " +
                  "               FROM MemOpt_ProviderBankCardLock M  " +
                  "               WHERE M.BankCardGUID = ProviderBankCard.BankCardGUID " +
                  "                 AND (M.ExpireDate IS NULL OR M.ExpireDate > GETUTCDATE()) " +
                  "         ) THEN 1    " +
                  "         ELSE 0   " +
                  "     END AS IsAvailable " +
                  " 	FROM ProviderBankCard  " +
                  " WHERE forProviderCode =@ProviderCode AND (BankCardState=0 OR BankCardState=1) ";

        if (!String.IsNullOrEmpty(SS))
        {
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProviderBankCard>(DT) as List<DBModel.ProviderBankCard>;
            }
        }

        return returnValue;
    }

    public DBModel.WithdrawLimit GetWithdrawLimitResultByCurrencyType(string CurrencyType, int WithdrawLimitType, string ProviderCode, int CompanyID)
    {
        DBModel.WithdrawLimit returnValue = null;
        string SS = "";
        SqlCommand DBCmd = null;
        DataTable DT = null;
        if (WithdrawLimitType == 0|| WithdrawLimitType == 3)
        { //供應商資料
            SS = "SELECT * FROM WithdrawLimit WHERE ProviderCode =@ProviderCode And WithdrawLimitType=@WithdrawLimitType And CurrencyType=@CurrencyType ";
        }
        else if (WithdrawLimitType == 1|| WithdrawLimitType == 2)
        {   //營運商資料
            SS = "SELECT * FROM WithdrawLimit WHERE forCompanyID =@forCompanyID And WithdrawLimitType=@WithdrawLimitType And CurrencyType=@CurrencyType ";
        }


        if (!String.IsNullOrEmpty(SS))
        {
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = WithdrawLimitType;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawLimit>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }


    public bool CheckWithdrawLimitData(DBModel.WithdrawLimit data)
    {
        bool returnValue = false;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "Select Count(*) from  WithdrawLimit where CurrencyType=@CurrencyType And WithdrawLimitType=@WithdrawLimitType ";
        if (data.WithdrawLimitType == 0|| data.WithdrawLimitType == 3)
        {
            //供應商資料
            SS += " And ProviderCode=@ProviderCode";
        }
        else
        {
            //營運商資料
            SS += " And forCompanyID=@forCompanyID And ServiceType=@ServiceType ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = string.IsNullOrEmpty(data.ServiceType) ? "" : data.ServiceType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = data.CompanyID;
        var dataCount = int.Parse(DBAccess.GetDBValue(Pay.DBConnStr, DBCmd).ToString());

        if (dataCount == 0)
        {
            returnValue = true;
        }

        return returnValue;
    }

    public int InsertProviderWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO WithdrawLimit (CurrencyType,WithdrawLimitType,ProviderCode,MaxLimit,MinLimit,Charge,Rate) " +
         "                       VALUES (@CurrencyType,@WithdrawLimitType,@ProviderCode,@MaxLimit,@MinLimit,@Charge,@Rate)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = data.Charge;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = data.Rate;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderWithdrawLimit.UpdateProviderAPIWithdrawLimit(data.ProviderCode, data.CurrencyType);
        return returnValue;
    }

    public int UpdateProviderWithdrawLimitBlockChainRateResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE WithdrawLimit SET MaxLimit=@MaxLimit,MinLimit=@MinLimit,BlockChainRate=@BlockChainRate " +
             " WHERE ProviderCode=@ProviderCode And WithdrawLimitType=@WithdrawLimitType And CurrencyType=@CurrencyType And ServiceType=@ServiceType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@BlockChainRate", SqlDbType.Decimal).Value = data.BlockChainRate;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = data.ServiceType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        RedisCache.ProviderWithdrawLimit.UpdateProviderWithdrawLimitBlockChainRate(data.ProviderCode, data.CurrencyType);

        return returnValue;
    }

    public int InsertProviderWithdrawLimitBlockChainRateResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO WithdrawLimit (CurrencyType,WithdrawLimitType,ProviderCode,MaxLimit,MinLimit,Charge,BlockChainRate,ServiceType) " +
         "                       VALUES (@CurrencyType,@WithdrawLimitType,@ProviderCode,@MaxLimit,@MinLimit,@Charge,@BlockChainRate,@ServiceType)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = data.ServiceType;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = 0;
        DBCmd.Parameters.Add("@BlockChainRate", SqlDbType.Decimal).Value = data.BlockChainRate;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderWithdrawLimit.UpdateProviderWithdrawLimitBlockChainRate(data.ProviderCode, data.CurrencyType);
        return returnValue;
    }

    public int UpdateProviderWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE WithdrawLimit SET MaxLimit=@MaxLimit,MinLimit=@MinLimit,Charge=@Charge,Rate=@Rate " +
             " WHERE ProviderCode=@ProviderCode And WithdrawLimitType=@WithdrawLimitType And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = data.Charge;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = data.Rate;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        if (data.WithdrawLimitType == 0)
        {
            RedisCache.ProviderWithdrawLimit.UpdateProviderBackendWithdrawLimit(data.ProviderCode, data.CurrencyType);
        } else if (data.WithdrawLimitType == 3) {
            RedisCache.ProviderWithdrawLimit.UpdateProviderAPIWithdrawLimit(data.ProviderCode, data.CurrencyType);
        }



        return returnValue;
    }

    public int InsertCompanyWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO WithdrawLimit (CurrencyType,WithdrawLimitType,forCompanyID,MaxLimit,MinLimit,Charge,ServiceType,Rate) " +
         "                       VALUES (@CurrencyType,@WithdrawLimitType,@forCompanyID,@MaxLimit,@MinLimit,@Charge,@ServiceType,@Rate)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = data.ServiceType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = data.CompanyID;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = data.Charge;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = data.Rate;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        if (data.WithdrawLimitType == 1)
        {
            RedisCache.CompanyWithdrawLimit.UpdateCompanyBackendWithdrawLimit(data.CompanyID, data.CurrencyType);
        }
        else if (data.WithdrawLimitType == 2)
        {
            RedisCache.CompanyWithdrawLimit.UpdateCompanyAPIWithdrawLimit(data.CompanyID, data.CurrencyType);
        }
        return returnValue;
    }

    public int UpdateCompanyWithdrawLimitResult(DBModel.WithdrawLimit data)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE WithdrawLimit SET MaxLimit=@MaxLimit,MinLimit=@MinLimit,Charge=@Charge,Rate=@Rate " +
             " WHERE forCompanyID=@forCompanyID And WithdrawLimitType=@WithdrawLimitType And CurrencyType=@CurrencyType And ServiceType=@ServiceType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = data.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@WithdrawLimitType", SqlDbType.Int).Value = data.WithdrawLimitType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = data.CompanyID;
        DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = data.MaxLimit;
        DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = data.MinLimit;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = data.Charge;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = data.Rate;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        if (data.WithdrawLimitType == 1)
        {
            RedisCache.CompanyWithdrawLimit.UpdateCompanyBackendWithdrawLimit(data.CompanyID, data.CurrencyType);
        }
        else if (data.WithdrawLimitType == 2)
        {
            RedisCache.CompanyWithdrawLimit.UpdateCompanyAPIWithdrawLimit(data.CompanyID, data.CurrencyType);
        }
        return returnValue;
    }
    #endregion

    #region ProxyProvider 

    public int SetProxyProviderData(DBModel.ProxyProvider Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        var ProxyProviderModel = GetProxyProviderResult(Model.forProviderCode);

        if (ProxyProviderModel == null)
        {
            SS = "INSERT INTO ProxyProvider (forProviderCode,Charge,Rate,MaxWithdrawalAmount) " +
        "                          VALUES (@forProviderCode,@Charge,@Rate,@MaxWithdrawalAmount)";
        }
        else
        {
            SS = "UPDATE ProxyProvider SET Charge=@Charge,Rate=@Rate,MaxWithdrawalAmount=@MaxWithdrawalAmount WHERE forProviderCode=@forProviderCode ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = Model.forProviderCode;
        DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = Model.Charge;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = Model.Rate;
        DBCmd.Parameters.Add("@MaxWithdrawalAmount", SqlDbType.Decimal).Value = Model.MaxWithdrawalAmount;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    public DBModel.ProxyProvider GetProxyProviderResult(string ProviderCode)
    {
        DBModel.ProxyProvider returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM ProxyProvider  WITH (NOLOCK)  WHERE forProviderCode =@ProviderCode";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProvider>(DT).First();
            }
        }

        return returnValue;
    }

    public static int InsertProxyProviderOrderWithDeductionProfit(string OrderSerial, int Type, decimal Charge, decimal Rate, int GroupID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "INSERT INTO ProxyProviderOrder (forOrderSerial,Type,PaymentRate,WithdrawalCharge,GroupID,DeductionProfit) " +
         "                          VALUES (@forOrderSerial,@Type,@PaymentRate,@WithdrawalCharge,@GroupID,@DeductionProfit)";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forOrderSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@Type", System.Data.SqlDbType.Int).Value = Type;
        DBCmd.Parameters.Add("@PaymentRate", System.Data.SqlDbType.Decimal).Value = Rate;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@WithdrawalCharge", System.Data.SqlDbType.Decimal).Value = Charge;
        DBCmd.Parameters.Add("@DeductionProfit", System.Data.SqlDbType.Int).Value = 0;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        //当是代付单时
        if (Type == 1)
        {
            var BackendDB = new BackendDB();
            string GroupName = BackendDB.GetProxyProviderGroupNameByGroupID(GroupID);
            new BackendDB().InsertAdminOPLog(1, 5, 0, "设定订单群组(扣除利润):" + OrderSerial + ",群组名:" + GroupName, "");
        }

        return RetValue;
    }

    public static int InsertProxyProviderOrder(string OrderSerial, int Type, decimal Charge, decimal Rate, int GroupID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "INSERT INTO ProxyProviderOrder (forOrderSerial,Type,PaymentRate,WithdrawalCharge,GroupID) " +
         "                          VALUES (@forOrderSerial,@Type,@PaymentRate,@WithdrawalCharge,@GroupID)";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forOrderSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@Type", System.Data.SqlDbType.Int).Value = Type;
        DBCmd.Parameters.Add("@PaymentRate", System.Data.SqlDbType.Decimal).Value = Rate;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@WithdrawalCharge", System.Data.SqlDbType.Decimal).Value = Charge;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        //当是代付单时
        if (Type == 1)
        {
            var BackendDB = new BackendDB();
            string GroupName = BackendDB.GetProxyProviderGroupNameByGroupID(GroupID);
            new BackendDB().InsertAdminOPLog(1, 5, 0, "设定订单群组:" + OrderSerial + ",群组名:" + GroupName, "");
        }

        return RetValue;
    }

    public static int UpdateProxyProviderOrder(string OrderSerial, int Type, decimal Charge, decimal Rate)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;


        SS = "UPDATE ProxyProviderOrder SET PaymentRate=@PaymentRate,WithdrawalCharge=@WithdrawalCharge WHERE  forOrderSerial=@forOrderSerial And Type=@Type";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forOrderSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@Type", System.Data.SqlDbType.Int).Value = Type;
        DBCmd.Parameters.Add("@PaymentRate", System.Data.SqlDbType.Decimal).Value = Rate;
        DBCmd.Parameters.Add("@WithdrawalCharge", System.Data.SqlDbType.Decimal).Value = Charge;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public int UpdateProxyProviderOrderGroup(string OrderSerial, int ChangeGroupID, int AdminGroupID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = " UPDATE ProxyProviderOrder" +
             " SET ProxyProviderOrder.GroupID =@GroupID " +
             " FROM ProxyProviderOrder JOIN Withdrawal" +
             " ON Withdrawal.WithdrawSerial = ProxyProviderOrder.forOrderSerial AND ProxyProviderOrder.Type = 1" +
             " WHERE ProxyProviderOrder.forOrderSerial = @forOrderSerial" +
             " AND Withdrawal.Status = 1" +
             " AND Withdrawal.HandleByAdminID = 0" +
             " And ProxyProviderOrder.GroupID=@AdminGroupID";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forOrderSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = ChangeGroupID;
        DBCmd.Parameters.Add("@AdminGroupID", System.Data.SqlDbType.Int).Value = AdminGroupID;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public int UpdateProxyProviderOrderGroupByAdmin(string OrderSerial, int GroupID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = " UPDATE ProxyProviderOrder" +
             " SET ProxyProviderOrder.GroupID =@GroupID " +
             " FROM ProxyProviderOrder JOIN Withdrawal" +
             " ON Withdrawal.WithdrawSerial = ProxyProviderOrder.forOrderSerial AND ProxyProviderOrder.Type = 1" +
             " WHERE ProxyProviderOrder.forOrderSerial = @forOrderSerial" +
             " AND Withdrawal.Status = 1" +
             " And Withdrawal.HandleByAdminID = 0";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forOrderSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = GroupID;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public int spUpdateProxyProviderOrderGroupByAdmin(string OrderSerial, int GroupID)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -3;
        //--0 = success
        // - 1 = 鎖定失敗
        // - 2 = 訂單處理中
        // - 3 = 其他原因

        SS = "spChangeWithdrawalGroupByAdmin";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;

        DBCmd.Parameters.Add("@WithdrawSerial", System.Data.SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@ChangeGroupID", System.Data.SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;

    }

    public DBModel.ProxyProviderOrder GetProxyProviderOrderByOrderSerial(string OrderSerial, int Type)
    {
        DBModel.ProxyProviderOrder returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM ProxyProviderOrder  WITH (NOLOCK)  WHERE forOrderSerial =@OrderSerial And Type=@Type ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@OrderSerial", SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderOrder>(DT).First();
            }
        }

        return returnValue;
    }

    public int GetProxyProviderPaymentGroupID(string PaymentSerial)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        int returnValue = 0;
        object DBReturn;
        SS = "SELECT PPO.GroupID  " +
             " FROM PaymentTable AS P WITH(NOLOCK) " +
             "  JOIN  ProxyProviderOrder PPO WITH(NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0" +
             " WHERE P.PaymentSerial=@PaymentSerial ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBReturn = DBAccess.GetDBValue(DBConnStr, DBCmd);
        if (DBReturn != null)
        {
            returnValue = int.Parse(DBReturn.ToString());
        }

        return returnValue;
    }

    public DBViewModel.UpdatePatmentResult ConfirmManualProviderPayment(string PaymentSerial, int modifyStatus, string PatchDescription, int ConfirmAdminID)
    {
        DBViewModel.UpdatePatmentResult returnValue = new DBViewModel.UpdatePatmentResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.PaymentTable PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);

        returnValue.Status = -1;
        if (PaymentData == null)
        {
            returnValue.Message = "订单资讯错误";
            return returnValue;
        }

        if (PaymentData.ProcessStatus == 8 && PaymentData.SubmitType == 1)
        {

            //人工充值
            SS = "spSetManualProviderPayment";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.StoredProcedure;
            DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
            DBCmd.Parameters.Add("@ProcessStatus", SqlDbType.Int).Value = modifyStatus;
            DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
            DBAccess.ExecuteDB(DBConnStr, DBCmd);
            DBreturn = (int)DBCmd.Parameters["@Return"].Value;

            switch (DBreturn)
            {
                case 0://成功
                    UpdatePaymentPatchDescriptionAndConfirmAdminID(PaymentSerial, PatchDescription, ConfirmAdminID);
                    returnValue.Message = "审核完成";
                    returnValue.Status = 0;
                    break;
                case -1://交易單不存在
                    returnValue.Message = "交易单不存在";
                    //UpdateWithdrawal(WithdrawSerial, "交易單不存在");
                    break;
                case -2://交易資料有誤 
                    returnValue.Message = "交易资料有误";
                    //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額錯誤");
                    break;

                case -4://鎖定失敗
                    returnValue.Message = "锁定失败";
                    //UpdateWithdrawal(WithdrawSerial, "鎖定失敗");
                    break;
                case -5://加扣點失敗
                    returnValue.Message = "加扣点失败";
                    //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                    break;
                case -9://加扣點失敗
                    returnValue.Message = "订单审核中";
                    //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                    break;
                default://其他錯誤
                    returnValue.Message = "其他错误";
                    //UpdateWithdrawal(WithdrawSerial, "其他錯誤");
                    break;
            }
        }
        else
        {
            returnValue.Message = "目前订单状态无法审核";
        }

        returnValue.PaymentData = GetProxyProviderPaymentReportByPaymentSerial(PaymentSerial);
        return returnValue;
    }

    public static int UpdatePaymentSerialByProxyProviderOrder(string PaymentSerial, int ProcessStatus, string ProviderCode, string ServiceType)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "UPDATE PaymentTable   SET ProcessStatus=@ProcessStatus,ServiceType=@ServiceType,ProviderCode=@ProviderCode WHERE  PaymentSerial=@PaymentSerial";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@ProcessStatus", System.Data.SqlDbType.Int).Value = ProcessStatus;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public List<DBModel.Withdrawal> GetWithdrawalAdminTableResultForProvider(FromBody.WithdrawalSet fromBody, string ProviderCode, int GroupID)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,HandleByAdminID FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK) ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate AND Status<>0 And Status<>8 AND Status <> 90 AND Status <> 91 And Withdrawal.ProviderCode=@ProviderCode" +
             " And (PPO.GroupID=0 or PPO.GroupID=@GroupID) ";

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And WithdrawSerial=@WithdrawSerial";
        }

        //卡号
        if (fromBody.BankDescription != "")
        {
            SS += " And Withdrawal.BankDescription=@BankDescription";
        }

        //營運商過濾
        if (fromBody.CompanyID != 0)
        {
            SS += " And Withdrawal.forCompanyID=@CompanyID";
        }

        //金额过滤
        if (fromBody.MinAmount != 0)
        {
            SS += " And Withdrawal.Amount >= @MinAmount";

            if (fromBody.MaxAmount != 0)
            {
                SS += " And Withdrawal.Amount <= @MaxAmount";
            }
        }

        if (fromBody.MaxAmount != 0)
        {
            SS += " And Withdrawal.Amount <= @MaxAmount";

            if (fromBody.MinAmount != 0)
            {
                SS += " And Withdrawal.Amount >= @MinAmount";
            }
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@MinAmount", SqlDbType.Decimal).Value = fromBody.MinAmount;
        DBCmd.Parameters.Add("@MaxAmount", SqlDbType.Decimal).Value = fromBody.MaxAmount;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = fromBody.BankDescription;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Withdrawal> OnlySearchWithdrawalForProvider(FromBody.WithdrawalSet fromBody, string ProviderCode)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,HandleByAdminID,GroupName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK) ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " LEFT JOIN ProxyProviderGroup PPG WITH (NOLOCK) ON PPO.GroupID=PPG.GroupID" +
             " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate AND Status<>0 And Status<>8 AND Status <> 90 AND Status <> 91 And Withdrawal.ProviderCode=@ProviderCode";

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And WithdrawSerial=@WithdrawSerial";
        }

        //序號過濾
        if (fromBody.GroupID != 0)
        {
            SS += " And GroupID=@GroupID";
        }

        //卡号
        if (fromBody.BankDescription != "")
        {
            SS += " And Withdrawal.BankDescription=@BankDescription";
        }

        //營運商過濾
        if (fromBody.CompanyID != 0)
        {
            SS += " And Withdrawal.forCompanyID=@CompanyID";
        }

        //金额过滤
        if (fromBody.MinAmount != 0)
        {
            SS += " And Withdrawal.Amount >= @MinAmount";

            if (fromBody.MaxAmount != 0)
            {
                SS += " And Withdrawal.Amount <= @MaxAmount";
            }
        }

        if (fromBody.MaxAmount != 0)
        {
            SS += " And Withdrawal.Amount <= @MaxAmount";

            if (fromBody.MinAmount != 0)
            {
                SS += " And Withdrawal.Amount >= @MinAmount";
            }
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@MinAmount", SqlDbType.Decimal).Value = fromBody.MinAmount;
        DBCmd.Parameters.Add("@MaxAmount", SqlDbType.Decimal).Value = fromBody.MaxAmount;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = fromBody.BankDescription;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Withdrawal> OnlySearchProviderWithdrawalTableResultByStatus(string ProviderCode)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;


        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,GroupName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK) ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " LEFT JOIN ProxyProviderGroup PPG WITH (NOLOCK) ON PPO.GroupID=PPG.GroupID" +
             " WHERE Withdrawal.Status=1 AND Withdrawal.ProviderCode=@ProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Withdrawal> GetProviderWithdrawalTableResultByStatus(string ProviderCode, int GroupID)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;


        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1 FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK) ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " WHERE Withdrawal.Status=1 AND Withdrawal.ProviderCode=@ProviderCode" +
             " And (PPO.GroupID=0 or PPO.GroupID=@GroupID) ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region ProviderCode

    public List<DBModel.Provider> GetProviderByServiceType(string ServiceType, string CurrencyType)
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        SS = "SELECT PC.ProviderName,PC.ProviderCode FROM ProviderService PS WITH (NOLOCK) " +
            "  JOIN ProviderCode PC ON PC.ProviderCode=PS.ProviderCode " +
            "WHERE PS.ServiceType =@ServiceType AND PS.CurrencyType=@CurrencyType And PC.ProviderState=0 ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT).ToList();
            }
        }

        return returnValue;
    }

    public string GetProviderNameByListProviderCode(List<string> ProviderCodes)
    {
        string returnValue = "";
        string SS;
        SqlCommand DBCmd = null;
        var parameters = new string[ProviderCodes.Count];
        DBCmd = new SqlCommand();
        DataTable DT;
        SS = "SELECT ProviderName FROM ProviderCode WITH (NOLOCK) ";

        for (int i = 0; i < ProviderCodes.Count; i++)
        {
            parameters[i] = string.Format("@ProviderCode{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], ProviderCodes[i]);

        }

        SS += string.Format(" WHERE  ProviderCode IN ({0})", string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null && DT.Rows.Count > 0)
        {
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                returnValue += DT.Rows[i]["ProviderName"] + ",";
            }

            returnValue = returnValue.Substring(0, returnValue.Length - 1);
        }

        return returnValue;
    }

    public string GetProviderNameByProviderCode(string ProviderCode)
    {
        string returnValue = "";
        string SS;
        SqlCommand DBCmd = null;

        SS = "SELECT ProviderName  FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode =@ProviderCode";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        var DBreturn= DBAccess.GetDBValue(DBConnStr, DBCmd);
        if (DBreturn!=null)
        {
            returnValue = DBreturn.ToString();
        }

        return returnValue;
    }

    public List<DBModel.Provider> GetProviderCodeResult(string ProviderCode = "")
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        if (ProviderCode != "")
        {
    
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode=@ProviderCode";
         
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK)";
    
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT) as List<DBModel.Provider>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Provider> GetProviderCodeResultByCompanyID(int CompanyID, int CompanyType, string ProviderCode = "")
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        if (ProviderCode != "")
        {
            if (CompanyType == 4)
            {
                SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode=@ProviderCode And forCompanyID=@forCompanyID";
            }
            else
            {
                SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode=@ProviderCode";
            }

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            if (CompanyType == 4)
            {
                SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE forCompanyID=@forCompanyID";
            }
            else
            {
                SS = "SELECT * FROM ProviderCode WITH (NOLOCK)";
            }

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT) as List<DBModel.Provider>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Provider> GetProviderCodeResultByCompanyID(int CompanyID,string ProviderCode = "")
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        if (ProviderCode != "")
        {
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode =@ProviderCode AND ProviderState=0";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderState=0";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT) as List<DBModel.Provider>;
            }
        }

        return returnValue;
    }

    public List<DBModel.Provider> GetProviderCodeResultByShowType(string ProviderCode = "")
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        if (ProviderCode != "")
        {
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderCode =@ProviderCode AND ProviderState=0";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = "SELECT * FROM ProviderCode WITH (NOLOCK) WHERE ProviderState=0";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT) as List<DBModel.Provider>;
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProvider> getProviderCodeResultByProxyProvider()
    {
        List<DBModel.ProxyProvider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT ProxyProvider.*,ProviderName,ProviderCode FROM ProviderCode WITH (NOLOCK)" +
             " LEFT JOIN ProxyProvider ON ProviderCode.ProviderCode=ProxyProvider.forProviderCode" +
             " Where CollectType=1 ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProvider>(DT) as List<DBModel.ProxyProvider>;
            }
        }

        return returnValue;
    }

    public DBModel.ProxyProvider GetProxyProviderByProviderCode(string ProviderCode)
    {
        DBModel.ProxyProvider returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT ProxyProvider.* FROM ProxyProvider WITH (NOLOCK)" +
             " Where forProviderCode=@ProviderCode ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProvider>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBModel.Provider> GetProviderCodeResultByProviderAPIType(int ProviderAPIType)
    {
        List<DBModel.Provider> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT * FROM ProviderCode WITH (NOLOCK)";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                var tmpProviders = new List<DBModel.Provider>();
                returnValue = DataTableExtensions.ToList<DBModel.Provider>(DT) as List<DBModel.Provider>;

                //旗標，API支援度，0=無/1=代收/2=代付/4=查詢餘額/8=查詢單/16=補單
                foreach (var data in returnValue)
                {
                    if (data.ProviderAPIType == 0)
                    {
                        continue;
                    }

                    if ((data.ProviderAPIType & ProviderAPIType) == ProviderAPIType)
                    {
                        tmpProviders.Add(data);
                    }
                }
                if (tmpProviders.Count > 0)
                {
                    returnValue = tmpProviders;
                }
                else
                {
                    returnValue = null;
                }
            }
        }

        return returnValue;
    }

    public int InsertProviderCode(DBModel.Provider Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO ProviderCode (ProviderCode,ProviderName,Introducer,ProviderUrl,MerchantCode,MerchantKey,NotifyAsyncUrl,NotifySyncUrl,ProviderAPIType,CollectType,ProviderState,WithdrawRate,forCompanyID,isPrePaid,BankCardTimeoutSecond,WalletTimeoutSecond) " +
         "                          VALUES (@ProviderCode,@ProviderName,@Introducer,@ProviderUrl,@MerchantCode,@MerchantKey,@NotifyAsyncUrl,@NotifySyncUrl,@ProviderAPIType,@CollectType,@ProviderState,@WithdrawRate,@forCompanyID,@isPrePaid,@BankCardTimeoutSecond,@WalletTimeoutSecond)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ProviderName", SqlDbType.NVarChar).Value = Model.ProviderName;
        DBCmd.Parameters.Add("@Introducer", SqlDbType.NVarChar).Value = Model.Introducer;
        DBCmd.Parameters.Add("@ProviderUrl", SqlDbType.VarChar).Value = Model.ProviderUrl;
        DBCmd.Parameters.Add("@MerchantCode", SqlDbType.NVarChar).Value = Model.MerchantCode;
        DBCmd.Parameters.Add("@MerchantKey", SqlDbType.NVarChar).Value = Model.MerchantKey;
        DBCmd.Parameters.Add("@NotifyAsyncUrl", SqlDbType.NVarChar).Value = Model.NotifyAsyncUrl;
        DBCmd.Parameters.Add("@NotifySyncUrl", SqlDbType.NVarChar).Value = Model.NotifySyncUrl;
        DBCmd.Parameters.Add("@ProviderAPIType", SqlDbType.Int).Value = Model.ProviderAPIType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@CollectType", SqlDbType.Int).Value = Model.CollectType;
        DBCmd.Parameters.Add("@ProviderState", SqlDbType.Int).Value = Model.CollectType == 1 ? 1 : 0;
        DBCmd.Parameters.Add("@WithdrawRate", SqlDbType.Decimal).Value = 0;
        DBCmd.Parameters.Add("@isPrePaid", SqlDbType.Int).Value = Model.isPrePaid;
        DBCmd.Parameters.Add("@BankCardTimeoutSecond", SqlDbType.Int).Value = Model.BankCardTimeoutSecond;
        DBCmd.Parameters.Add("@WalletTimeoutSecond", SqlDbType.Int).Value = Model.WalletTimeoutSecond;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderCode.UpdateProviderCode(Model.ProviderCode);
        return returnValue;
    }

    public int UpdateProviderCode(DBModel.Provider Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderCode SET ProviderName=@ProviderName,Introducer=@Introducer,ProviderUrl=@ProviderUrl,MerchantCode=@MerchantCode,MerchantKey=@MerchantKey,NotifyAsyncUrl=@NotifyAsyncUrl,NotifySyncUrl=@NotifySyncUrl,ProviderAPIType=@ProviderAPIType,CollectType=@CollectType,WithdrawRate=@WithdrawRate,forCompanyID=@forCompanyID,isPrePaid=@isPrePaid,BankCardTimeoutSecond=@BankCardTimeoutSecond,WalletTimeoutSecond=@WalletTimeoutSecond WHERE ProviderCode=@ProviderCode ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ProviderName", SqlDbType.NVarChar).Value = Model.ProviderName;
        DBCmd.Parameters.Add("@Introducer", SqlDbType.NVarChar).Value = Model.Introducer;
        DBCmd.Parameters.Add("@ProviderUrl", SqlDbType.VarChar).Value = Model.ProviderUrl;
        DBCmd.Parameters.Add("@MerchantCode", SqlDbType.NVarChar).Value = Model.MerchantCode;
        DBCmd.Parameters.Add("@MerchantKey", SqlDbType.NVarChar).Value = Model.MerchantKey;
        DBCmd.Parameters.Add("@NotifyAsyncUrl", SqlDbType.NVarChar).Value = Model.NotifyAsyncUrl;
        DBCmd.Parameters.Add("@NotifySyncUrl", SqlDbType.NVarChar).Value = Model.NotifySyncUrl;
        DBCmd.Parameters.Add("@ProviderAPIType", SqlDbType.Int).Value = Model.ProviderAPIType;
        DBCmd.Parameters.Add("@CollectType", SqlDbType.Int).Value = Model.CollectType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@WithdrawRate", SqlDbType.Decimal).Value = Model.WithdrawRate;
        DBCmd.Parameters.Add("@isPrePaid", SqlDbType.Int).Value = Model.isPrePaid;
        DBCmd.Parameters.Add("@BankCardTimeoutSecond", SqlDbType.Int).Value = Model.BankCardTimeoutSecond;
        DBCmd.Parameters.Add("@WalletTimeoutSecond", SqlDbType.Int).Value = Model.WalletTimeoutSecond;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderCode.UpdateProviderCode(Model.ProviderCode);
        return returnValue;
    }

    public int InsertProviderBankCard(FromBody.ProviderBankCardData companyBankCard)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " INSERT INTO ProviderBankCard (forProviderCode, BankCardState, CurrencyType, Type, CardType, BankCardName, BankCode, BankName, BranchName, BankNumber, AccountName, BankProvince, BankCity, Description, AmountMax, TotalCreditValueDay, TotalCreditValueWeek, TotalCreditValueMonth, AmountMin, ServiceStartDate, ServiceEndDate) " +
                                  " VALUES (@ProviderCode, @BankCardState, @CurrencyType, @Type, @CardType, @BankCardName, @BankCode, @BankName, @BranchName, @BankNumber, @AccountName, @BankProvince, @BankCity, @Description, @AmountMax, @TotalCreditValueDay, @TotalCreditValueWeek, @TotalCreditValueMonth, @AmountMin, @ServiceStartDate, @ServiceEndDate) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = companyBankCard.forProviderCode;
        DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyBankCard.BankCardState;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = companyBankCard.CurrencyType;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = companyBankCard.Type;
        DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyBankCard.CardType;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyBankCard.BankCardName;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = companyBankCard.BankCode;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = companyBankCard.BankName;
        DBCmd.Parameters.Add("@BranchName", SqlDbType.NVarChar).Value = companyBankCard.BranchName;
        DBCmd.Parameters.Add("@BankNumber", SqlDbType.VarChar).Value = companyBankCard.BankNumber;
        DBCmd.Parameters.Add("@AccountName", SqlDbType.NVarChar).Value = companyBankCard.AccountName;
        DBCmd.Parameters.Add("@BankProvince", SqlDbType.NVarChar).Value = companyBankCard.BankProvince;
        DBCmd.Parameters.Add("@BankCity", SqlDbType.NVarChar).Value = companyBankCard.BankCity;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = companyBankCard.Description;
        DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyBankCard.AmountMax;
        DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueDay;
        DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueWeek;
        DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueMonth;
        DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyBankCard.AmountMin;
        DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyBankCard.ServiceStartDate;
        DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyBankCard.ServiceEndDate;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }

    public int UpdateProviderBankCard(FromBody.ProviderBankCardData companyBankCard)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderBankCard SET BankCardState=@BankCardState " +
             " , CardType=@CardType " +
             " , BankCardName=@BankCardName " +
             " , BankCode=@BankCode " +
             " , BankName=@BankName " +
             " , BranchName=@BranchName " +
             " , BankNumber=@BankNumber " +
             " , AccountName=@AccountName " +
             " , BankProvince=@BankProvince " +
             " , BankCity=@BankCity " +
             " , Description=@Description " +
             " , AmountMax=@AmountMax " +
             " , TotalCreditValueDay=@TotalCreditValueDay " +
             " , TotalCreditValueWeek=@TotalCreditValueWeek " +
             " , TotalCreditValueMonth=@TotalCreditValueMonth " +
             " , AmountMin=@AmountMin " +
             " , ServiceStartDate=@ServiceStartDate " +
             " , ServiceEndDate=@ServiceEndDate " +
             " WHERE BankCardGUID=@BankCardGUID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = companyBankCard.BankCardGUID;
        DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyBankCard.BankCardState;
        DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyBankCard.CardType;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyBankCard.BankCardName;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = companyBankCard.BankCode;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = companyBankCard.BankName;
        DBCmd.Parameters.Add("@BranchName", SqlDbType.NVarChar).Value = companyBankCard.BranchName;
        DBCmd.Parameters.Add("@BankNumber", SqlDbType.VarChar).Value = companyBankCard.BankNumber;
        DBCmd.Parameters.Add("@AccountName", SqlDbType.NVarChar).Value = companyBankCard.AccountName;
        DBCmd.Parameters.Add("@BankProvince", SqlDbType.NVarChar).Value = companyBankCard.BankProvince;
        DBCmd.Parameters.Add("@BankCity", SqlDbType.NVarChar).Value = companyBankCard.BankCity;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = companyBankCard.Description;
        DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyBankCard.AmountMax;
        DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueDay;
        DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueWeek;
        DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyBankCard.TotalCreditValueMonth;
        DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyBankCard.AmountMin;
        DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyBankCard.ServiceStartDate;
        DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyBankCard.ServiceEndDate;


        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }

    public int DeleteProviderBankCard(FromBody.ProviderBankCardData companyBankCard)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderBankCard SET BankCardState=@BankCardState " +
             " WHERE BankCardGUID=@BankCardGUID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = companyBankCard.BankCardGUID;
        DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = 2;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }

    public int GetProviderState(string ProviderCode)
    {

        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "SELECT ProviderState FROM ProviderCode WHERE ProviderCode =@ProviderCode";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());
        return returnValue;
    }

    public List<DBModel.ProviderBalance> GetProviderPoint(string CurrencyType, List<string> ArrayProviderCode)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];
        #region SignCheck
        string strSign;
        string sign;
        List<DBModel.ProviderBalance> objReturnValue = null;
        strSign = string.Format("CurrencyType={0}&GPayBackendKey={1}"
        , CurrencyType
        , GPayBackendKey
        );

        sign = CodingControl.GetSHA256(strSign);

        #endregion
        var _ProviderBalance = new DBModel.ProviderBalanceSet();
        _ProviderBalance.CurrencyType = CurrencyType;
        _ProviderBalance.ArrayProviderCode = ArrayProviderCode;
        _ProviderBalance.Sign = sign;

        var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "QueryProviderBalance", JsonConvert.SerializeObject(_ProviderBalance));

        if (!string.IsNullOrEmpty(returnValue))
        {
            var jobj = JObject.Parse(returnValue);
            var ArrayProviderBalance = jobj["ArrayProviderBalance"];
            if (ArrayProviderBalance != null)
            {
                objReturnValue = JsonConvert.DeserializeObject<List<DBModel.ProviderBalance>>(ArrayProviderBalance.ToString());
            }

            if (objReturnValue.Count == 0)
            {
                objReturnValue = null;
            }
        }
        return objReturnValue;
    }

    public List<DBViewModel.ProviderListResult> GetProviderListResult()
    {
        List<DBViewModel.ProviderListResult> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT PC.ProviderCode, " +
                 "        PC.ProviderName, " +
                 "        PC.ProviderAPIType," +
                 "        PC.ProviderState," +
                 "        WL.MaxLimit, " +
                 "        WL.MinLimit, " +
                 "        WL.Charge, " +
                 "        WL.CurrencyType" +
                 " FROM   ProviderCode PC " +
                 "        LEFT JOIN WithdrawLimit WL " +
                 "               ON WL.ProviderCode = PC.ProviderCode " +
                 "                  AND WL.WithdrawLimitType = 0 " +
                 " ORDER BY PC.ProviderState";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderListResult>(DT) as List<DBViewModel.ProviderListResult>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ServiceData> GetProviderListServiceData(string Currency = "")
    {
        List<DBViewModel.ServiceData> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT ST.ServiceTypeName, " +
                 "        PS.MaxDaliyAmount, " +
                 "        PS.MaxOnceAmount, " +
                 "        PS.MinOnceAmount, " +
                 "        PS.CostCharge, " +
                 "        PS.CostRate, " +
                 "        PS.ServiceType, " +
                 "        PS.ProviderCode, " +
                 "        PS.CurrencyType, " +
                 "        PS.State," +
                 "        PS.CheckoutType" +
                 " FROM   ProviderService PS" +
                 "        LEFT JOIN ServiceType ST" +
                 "               ON ST.ServiceType = PS.ServiceType " +
                 " WHERE  1=1 ";
        if (!string.IsNullOrEmpty(Currency))
        {
            SS += "        AND PS.CurrencyType = @CurrencyType ";
        }


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        if (!string.IsNullOrEmpty(Currency))
        {
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Currency;
        }
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ServiceData>(DT) as List<DBViewModel.ServiceData>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderListPoint> GetProviderListPoint()
    {
        List<DBViewModel.ProviderListPoint> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT PP.SystemPointValue,PP.CurrencyType" +
                  " FROM ProviderPoint PP ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderListPoint>(DT) as List<DBViewModel.ProviderListPoint>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderListPoint> GetProviderListFrozenPoint(string ProviderCode)
    {
        List<DBViewModel.ProviderListPoint> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT CurrencyType,SUM(ProviderFrozenAmount) SystemPointValue FROM FrozenPoint " +
                  " WHERE forProviderCode = @ProviderCode and Status = 0  " +
                  " GROUP BY CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderListPoint>(DT) as List<DBViewModel.ProviderListPoint>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.AllProviderTotal> GetAllProviderTotal()
    {
        List<DBViewModel.AllProviderTotal> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT SUM(PP.SystemPointValue) AS Total," +
            " 		SUM(FP.FrozenTotal) AS FrozenTotal," +
                  "        COUNT(*) as Count" +
                  " FROM ProviderPoint PP " +
                  "  LEFT JOIN ( SELECT forProviderCode,SUM(ProviderFrozenAmount) FrozenTotal FROM FrozenPoint " +
                  "  WHERE Status = 0 GROUP BY forProviderCode) FP ON PP.ProviderCode = FP.forProviderCode ";


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AllProviderTotal>(DT) as List<DBViewModel.AllProviderTotal>;
            }
        }

        return returnValue;
    }

    public int ChangeProviderCodeState(DBModel.Provider Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderCode SET ProviderState=(IIF(ProviderState=0,1,0)) WHERE ProviderCode=@ProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderCode.UpdateProviderCode(Model.ProviderCode);

        return returnValue;
    }

    public int UpdateProxyProviderPoint(string ProviderCode, decimal Amount, int GroupID, string Description)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -3;

        SS = "spAddProxyProviderGroupPoint";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;

        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Pay.CurrencyType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public int UpdateProxyProviderPoint2(string PaymentSerial)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -3;

        SS = "spAddProxyProviderGroupPoint2";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;

        DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public int ChangeProviderAPIType(FromBody.UpdateProviderAPIType Model, int APIType)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderCode SET ProviderAPIType= ProviderAPIType + @ProviderAPIType WHERE ProviderCode=@ProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ProviderAPIType", SqlDbType.Int).Value = APIType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderCode.UpdateProviderCode(Model.ProviderCode);

        return returnValue;
    }

    public bool CheckProxyProviderState(string ProviderCode)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "SELECT COUNT(*) FROM ProviderCode WHERE CollectType = 1 AND ProviderState = 0 AND ProviderCode<>@ProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        int result = (int)DBAccess.GetDBValue(Pay.DBConnStr, DBCmd);
        return result > 0 ? false : true;
    }
    #endregion

    #region Admin
    public bool CheckLoginIP(string UserIP)
    {

        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT IP FROM BackendLoginIP WITH (NOLOCK)" +
            " WHERE IP=@IP ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = UserIP;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT.Rows.Count > 0)
        {
            returnValue = true;
        }
        return returnValue;
    }

    public bool CheckLoginIP(string UserIP, string CompanyCode)
    {

        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CheckCompanyBackendLoginIPTypeByCode(CompanyCode))
        {
            SS = "SELECT IP FROM BackendLoginIP WITH (NOLOCK)" +
          " WHERE IP=@IP And CompanyCode=@CompanyCode ";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = UserIP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
            if (DT.Rows.Count > 0)
            {
                returnValue = true;
            }
            return returnValue;
        }
        else
        {
            return true;
        }
    }

    public bool CheckBackendWithdrawalIP(string UserIP, string CompanyCode)
    {

        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CheckCompanyBackendWithdrawIPTypeByCode(CompanyCode))
        {
            SS = "SELECT IP FROM BackendWithdrawalIP WITH (NOLOCK)" +
          " WHERE IP=@IP And CompanyCode=@CompanyCode ";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = UserIP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
            if (DT.Rows.Count > 0)
            {
                returnValue = true;
            }
            return returnValue;
        }
        else
        {
            return true;
        }
    }

    public int UpdateAdminGoogleKey(string GoogleKey, string LoginAccount)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE AdminTable SET GoogleKey=@GoogleKey " +
             " WHERE LoginAccount=@LoginAccount";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@GoogleKey", SqlDbType.VarChar).Value = GoogleKey;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }

    public DBModel.Admin GetAdminByLoginAccount(string LoginAccount)
    {
        DBModel.Admin returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT *,CompanyType,SortKey,CompanyCode,CompanyName FROM AdminTable WITH (NOLOCK) " +
            " Left Join CompanyTable WITH (NOLOCK) On AdminTable.forCompanyID = CompanyTable.CompanyID" +
            " WHERE LoginAccount=@LoginAccount And AdminState=0";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Admin>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.AdminWithLoginPassword GetAdminByLoginAccountWithLoginPassword(string LoginAccount)
    {
        DBModel.AdminWithLoginPassword returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT *,CompanyType,SortKey,CompanyCode,CompanyName FROM AdminTable WITH (NOLOCK) " +
            " Left Join CompanyTable WITH (NOLOCK) On AdminTable.forCompanyID = CompanyTable.CompanyID" +
            " WHERE LoginAccount=@LoginAccount And AdminState=0 And CompanyState=0 ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AdminWithLoginPassword>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.AdminWithGoogleKey GetAdminByLoginAccountWithGoogleKey(string LoginAccount)
    {
        DBModel.AdminWithGoogleKey returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT * FROM AdminTable WITH (NOLOCK) " +
            " WHERE LoginAccount=@LoginAccount And AdminState=0";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AdminWithGoogleKey>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public string GetGoogleKeyByLoginAccount(string LoginAccount)
    {
        string returnValue = "";
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        object DTreturn = null;
        SS = " SELECT GoogleKey FROM AdminTable WITH (NOLOCK) " +
            " WHERE LoginAccount=@LoginAccount And AdminState=0";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        DTreturn = DBAccess.GetDBValue(DBConnStr, DBCmd);
        if (DTreturn != null)
        {
            returnValue = DTreturn.ToString();
        }

        return returnValue;
    }

    public DBModel.Admin GetAdminByLoginAdminID(int AdminID)
    {
        DBModel.Admin returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT * FROM AdminTable WITH (NOLOCK) " +
            " WHERE AdminID=@AdminID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Admin>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.AdminWithLoginPassword GetAdminByLoginAdminIDWithLoginPassword(int AdminID)
    {
        DBModel.AdminWithLoginPassword returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT * FROM AdminTable WITH (NOLOCK) " +
            " WHERE AdminID=@AdminID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AdminWithLoginPassword>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBViewModel.AdminWithKey GetAdminByLoginAdminIDWithKey(int AdminID)
    {
        DBViewModel.AdminWithKey returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT * FROM AdminTable WITH (NOLOCK) " +
            " WHERE AdminID=@AdminID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminWithKey>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.AdminTableResult> GetAdminTableByCompanyID(int CompanyID)
    {
        List<DBViewModel.AdminTableResult> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CompanyType,AdminTable.*,RoleName,convert(varchar, AdminTable.CreateDate, 120) as CreateDate2 FROM AdminTable WITH (NOLOCK) " +
             " Left Join AdminRole on AdminRole.AdminRoleID=AdminTable.forAdminRoleID" +
             " Left Join CompanyTable on CompanyTable.CompanyID=AdminTable.forCompanyID" +
             " WHERE AdminTable.forCompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminTableResult>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertAdmin(int CompanyID, int AdminroleID, string LoginAccount, string Password, string RealName, string Description, int AdminType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM AdminTable WITH (NOLOCK) " +
             " WHERE LoginAccount=@LoginAccount";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        //登入帳號重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO AdminTable (AdminState,forCompanyID,forAdminRoleID,AdminType,LoginAccount,LoginPassword,RealName,Description,UserIP)" +
             " VALUES(0,@forCompanyID,@forAdminRoleID,@AdminType,@LoginAccount,@LoginPassword,@RealName,@Description,@UserIP)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminroleID;
        DBCmd.Parameters.Add("@AdminType", SqlDbType.Int).Value = AdminType;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;
        DBCmd.Parameters.Add("@LoginPassword", SqlDbType.VarChar).Value = CodingControl.GetMD5(Password);
        DBCmd.Parameters.Add("@RealName", SqlDbType.NVarChar).Value = RealName;
        DBCmd.Parameters.Add("@UserIP", SqlDbType.VarChar).Value = CodingControl.GetUserIP();
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;


        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateAdmin(int AdminID, int CompanyID, int AdminroleID, string Password, string RealName, string Description, int AdminType, int AdminState)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE AdminTable";
        SS += " SET forAdminRoleID=@forAdminRoleID,";
        if (!string.IsNullOrEmpty(Password))
        {
            SS += "LoginPassword=@LoginPassword,";
        }
        SS += " AdminState=@AdminState,AdminType=@AdminType,RealName=@RealName,Description=@Description,UserIP=@UserIP";
        SS += " WHERE AdminID=@AdminID And forCompanyID=@forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminroleID;
        DBCmd.Parameters.Add("@AdminType", SqlDbType.Int).Value = AdminType;
        if (!string.IsNullOrEmpty(Password))
        {
            DBCmd.Parameters.Add("@LoginPassword", SqlDbType.VarChar).Value = CodingControl.GetMD5(Password);
        }
        DBCmd.Parameters.Add("@RealName", SqlDbType.NVarChar).Value = RealName;
        DBCmd.Parameters.Add("@UserIP", SqlDbType.VarChar).Value = CodingControl.GetUserIP();
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
        DBCmd.Parameters.Add("@AdminState", SqlDbType.Int).Value = AdminState;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int DisableAdmin(int AdminID, int CompanyID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE AdminTable";
        SS += " SET AdminState=1";
        SS += " WHERE AdminID=@AdminID And forCompanyID=@forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateLoginPassword(int AdminID, string Password, string Newpassword)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        string oldLoginPassword = "";
        string MD5Input;

        SS = "SELECT LoginPassword FROM AdminTable WITH (NOLOCK) " +
            " WHERE AdminID=@AdminID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        oldLoginPassword = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        MD5Input = CodingControl.GetMD5(Password);
        if (MD5Input == oldLoginPassword)
        {

            SS = " UPDATE AdminTable";
            SS += " SET LoginPassword=@LoginPassword";
            SS += " WHERE AdminID=@AdminID ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@LoginPassword", SqlDbType.VarChar).Value = CodingControl.GetMD5(Newpassword);
            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

            returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        }
        else
        {
            returnValue = -1;
        }

        return returnValue;

    }

    public int CheckAdminExistByLoginAccount(string LoginAccount)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = "SELECT count(*) FROM AdminTable WITH (NOLOCK) " +
            " WHERE LoginAccount=@LoginAccount";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }
    #endregion

    #region ProxyProviderGroup
    public decimal GetProxyProviderGroupFrozenPoint(string ProviderCode, int GroupID)
    {
        decimal returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT  ISNULL(SUM(ProviderFrozenAmount), 0) FROM FrozenPoint FP WITH(NOLOCK)" +
             " WHERE FP.forProviderCode = @ProviderCode AND FP.GroupID = @GroupID AND  FP.Status= 0";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        returnValue = decimal.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroupFrozenPointHistory> GetProxyProviderGroupFrozenPoint(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroupFrozenPointHistory> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;


        SS = " SELECT PPG.GroupName,ISNULL(TmpTable.FrozenAmount, 0) AS FrozenAmount FROM ProxyProviderGroup PPG WITH(NOLOCK) LEFT JOIN" +
             " (SELECT FP.GroupID, SUM(ProviderFrozenAmount) AS FrozenAmount FROM FrozenPoint FP WITH(NOLOCK)" +
             " JOIN ProxyProviderGroup PPG WITH(NOLOCK) ON PPG.GroupID= FP.GroupID" +
             " WHERE FP.forProviderCode=@ProviderCode AND FP.Status= 0 GROUP BY FP.GroupID" +
             " )  AS TmpTable ON PPG.GroupID = TmpTable.GroupID" +
             " WHERE PPG.forProviderCode=@ProviderCode ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroupFrozenPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupOnWithdrawalAmountResultByAdmin(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ProfitAmount,GroupID,GroupName,PPG.State,PPG.Weight,PPG.PaymentRate, " +
             " (SELECT ISNULL(SUM(Amount), 0) FROM Withdrawal W WITH(NOLOCK)" +
             " LEFT JOIN ProxyProviderOrder PPO ON PPO.forOrderSerial = W.WithdrawSerial AND PPO.Type = 1" +
             " WHERE PPG.forProviderCode = W.ProviderCode AND PPG.GroupID = PPO.GroupID AND W.Status = 1) AS CanUsePoint," +
             " (SELECT ISNULL(COUNT(*), 0) FROM Withdrawal W WITH(NOLOCK)" +
             " LEFT JOIN ProxyProviderOrder PPO ON PPO.forOrderSerial = W.WithdrawSerial AND PPO.Type = 1" +
             " WHERE PPG.forProviderCode = W.ProviderCode AND PPG.GroupID = PPO.GroupID AND W.Status = 1) AS WithdrawalCount," +
              " ISNULL(CanUsePoint - (SELECT ISNULL(SUM(ProviderFrozenAmount),0) FROM FrozenPoint FP WITH(NOLOCK)" +
             " WHERE FP.forProviderCode = PPG.forProviderCode AND FP.Status = 0 AND FP.GroupID = PPG.GroupID), 0) AS CanUsePoint2 " +
             " FROM ProxyProviderGroup PPG WITH(NOLOCK)  WHERE PPG.forProviderCode = @forProviderCode ORDER BY GroupID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupTableResultByAdmin(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT GroupID,GroupName,PPG.State,PPG.Weight, " +
             " ISNULL(CanUsePoint - (SELECT ISNULL(SUM(ProviderFrozenAmount),0) FROM FrozenPoint FP WITH(NOLOCK)" +
             " WHERE FP.forProviderCode = PPG.forProviderCode AND FP.Status = 0 AND FP.GroupID = PPG.GroupID), 0) AS CanUsePoint " +
             " FROM ProxyProviderGroup PPG WITH (NOLOCK) " +
             " WHERE PPG.forProviderCode = @forProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetAllProxyProviderGroupTableResultByAdmin()
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT GroupID,GroupName,PPG.State,PPG.Weight, " +
             " ISNULL(CanUsePoint - (SELECT ISNULL(SUM(ProviderFrozenAmount),0) FROM FrozenPoint FP WITH(NOLOCK)" +
             " WHERE FP.forProviderCode = PPG.forProviderCode AND FP.Status = 0 AND FP.GroupID = PPG.GroupID), 0) AS CanUsePoint " +
             " FROM ProxyProviderGroup PPG WITH (NOLOCK) " +
             " WHERE PPG.forProviderCode = (SELECT ProviderCode FROM ProviderCode WHERE CollectType = 1 AND ProviderState = 0)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        //DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupWeightByAdmin(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT PPG.GroupID,PPG.GroupName,PPG.State,PPG.Weight,PPG.MaxAmount,PPG.MinAmount,PPG.PaymentRate " +
             " FROM ProxyProviderGroup PPG WITH (NOLOCK) " +
             " WHERE PPG.forProviderCode = @forProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupTableResult(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT *,convert(varchar, CreateDate, 120) as CreateDate2," +
            " STUFF((" +
             " Select  ',' + RealName" +
             " From AdminTable AT " +
             " where AT.GroupID =ProxyProviderGroup.GroupID " +
             " For Xml Path(''))" +
             " , 1, 1, '') as GroupAccounts " +
            " FROM ProxyProviderGroup WITH (NOLOCK) " +
             " WHERE ProxyProviderGroup.forProviderCode = @forProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupName(string ProviderCode)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT GroupID,GroupName " +
            " FROM ProxyProviderGroup WITH (NOLOCK) " +
             " WHERE ProxyProviderGroup.forProviderCode = @forProviderCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBModel.ProxyProviderGroup GetProxyProviderGroupByGroupID(string ProviderCode, int GroupID)
    {
        DBModel.ProxyProviderGroup returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * " +
            " FROM ProxyProviderGroup " +
             " WHERE ProxyProviderGroup.forProviderCode = @forProviderCode And GroupID=@GroupID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).First();
            }
        }

        return returnValue;
    }

    public string GetProxyProviderGroupNameByGroupID(int GroupID)
    {
        string returnValue = "";
        String SS = String.Empty;
        SqlCommand DBCmd;
        Object DBreturn = null;

        SS = " SELECT GroupName " +
            " FROM ProxyProviderGroup WITH (NOLOCK) " +
             " WHERE GroupID=@GroupID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBreturn = DBAccess.GetDBValue(DBConnStr, DBCmd);

        if (DBreturn != null)
        {
            returnValue = DBreturn.ToString();
        }

        return returnValue;
    }

    public string GetProxyProviderGroupNameByOrderSerial(string OrderSerial, int Type)
    {
        string returnValue = "";
        String SS = String.Empty;
        SqlCommand DBCmd;
        Object DBreturn = null;

        SS = " SELECT GroupName FROM ProxyProviderOrder WITH(NOLOCK) " +
             " JOIN ProxyProviderGroup WITH(NOLOCK) ON ProxyProviderOrder.GroupID = ProxyProviderGroup.GroupID " +
             " WHERE forOrderSerial=@OrderSerial And Type=@Type ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@OrderSerial", SqlDbType.VarChar).Value = OrderSerial;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
        DBreturn = DBAccess.GetDBValue(DBConnStr, DBCmd);

        if (DBreturn != null)
        {
            returnValue = DBreturn.ToString();
        }

        return returnValue;
    }

    public List<DBModel.ProxyProviderGroup> GetProxyProviderGroupByState(string ProviderCode, int State)
    {
        List<DBModel.ProxyProviderGroup> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * " +
            " FROM ProxyProviderGroup WITH (NOLOCK) " +
             " WHERE ProxyProviderGroup.forProviderCode = @forProviderCode And State=@State";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = State;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxyProviderGroup>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertProxyProviderGroup(string ProviderCode, string GroupName, decimal MinAmount, decimal MaxAmount)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM ProxyProviderGroup WITH (NOLOCK) " +
             " WHERE forProviderCode=@forProviderCode And GroupName=@GroupName";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = GroupName;

        //登入帳號重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO ProxyProviderGroup (forProviderCode,GroupName,MinAmount,MaxAmount)" +
             " VALUES (@forProviderCode,@GroupName,@MinAmount,@MaxAmount)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = GroupName;

        DBCmd.Parameters.Add("@MinAmount", SqlDbType.Decimal).Value = MinAmount;
        DBCmd.Parameters.Add("@MaxAmount", SqlDbType.Decimal).Value = MaxAmount;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateProxyProviderGroup(string ProviderCode, string GroupName, int GroupID, int State, decimal MinAmount, decimal MaxAmount)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE ProxyProviderGroup";
        SS += " SET GroupName=@GroupName,State=@State,MinAmount=@MinAmount,MaxAmount=@MaxAmount ";
        SS += " WHERE forProviderCode=@ProviderCode And GroupID=@GroupID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = GroupName;

        DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = State;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@MinAmount", SqlDbType.Decimal).Value = MinAmount;
        DBCmd.Parameters.Add("@MaxAmount", SqlDbType.Decimal).Value = MaxAmount;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateProxyProviderGroupWeight(List<DBModel.ProxyProviderGroup> data)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE ProxyProviderGroup";
        SS += " SET Weight=@Weight,State=@State,MaxAmount=@MaxAmount,MinAmount=@MinAmount,PaymentRate=@PaymentRate";

        SS += " WHERE GroupID=@GroupID ";

        for (int i = 0; i < data.Count; i++)
        {
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = data[i].Weight;
            DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = data[i].State;
            DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = data[i].GroupID;
            DBCmd.Parameters.Add("@MaxAmount", SqlDbType.Decimal).Value = data[i].MaxAmount;
            DBCmd.Parameters.Add("@MinAmount", SqlDbType.Decimal).Value = data[i].MinAmount;
            DBCmd.Parameters.Add("@PaymentRate", SqlDbType.Decimal).Value = data[i].PaymentRate;
            returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);

        }

        return returnValue;

    }

    public int DisableProxyProviderGroup(int GroupID, string ProviderCode)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE ProxyProviderGroup";
        SS += " SET State = (CASE State WHEN 1 THEN 0 ELSE 1 END)";
        SS += " WHERE forProviderCode=@forProviderCode And GroupID=@GroupID ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    #endregion

    #region 专属供应商设定

    public int InsertProxyProviderAcount(int CompanyID, int AdminroleID, string LoginAccount, string Password, string RealName, string Description, int AdminType, int GroupID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM AdminTable WITH (NOLOCK) " +
             " WHERE LoginAccount=@LoginAccount";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

        //登入帳號重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO AdminTable (AdminState,forCompanyID,forAdminRoleID,AdminType,LoginAccount,LoginPassword,RealName,Description,UserIP,GroupID)" +
             " VALUES(0,@forCompanyID,@forAdminRoleID,@AdminType,@LoginAccount,@LoginPassword,@RealName,@Description,@UserIP,@GroupID)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminroleID;
        DBCmd.Parameters.Add("@AdminType", SqlDbType.Int).Value = AdminType;
        DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;
        DBCmd.Parameters.Add("@LoginPassword", SqlDbType.VarChar).Value = CodingControl.GetMD5(Password);
        DBCmd.Parameters.Add("@RealName", SqlDbType.NVarChar).Value = RealName;
        DBCmd.Parameters.Add("@UserIP", SqlDbType.VarChar).Value = CodingControl.GetUserIP();
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateProxyProviderAcount(int AdminID, int CompanyID, int AdminroleID, string Password, string RealName, string Description, int AdminType, int AdminState, int GroupID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE AdminTable";
        SS += " SET forAdminRoleID=@forAdminRoleID,";
        if (!string.IsNullOrEmpty(Password))
        {
            SS += "LoginPassword=@LoginPassword,";
        }
        SS += " AdminState=@AdminState,AdminType=@AdminType,RealName=@RealName,Description=@Description,UserIP=@UserIP,GroupID=@GroupID";
        SS += " WHERE AdminID=@AdminID And forCompanyID=@forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminroleID;
        DBCmd.Parameters.Add("@AdminType", SqlDbType.Int).Value = AdminType;
        if (!string.IsNullOrEmpty(Password))
        {
            DBCmd.Parameters.Add("@LoginPassword", SqlDbType.VarChar).Value = CodingControl.GetMD5(Password);
        }
        DBCmd.Parameters.Add("@RealName", SqlDbType.NVarChar).Value = RealName;
        DBCmd.Parameters.Add("@UserIP", SqlDbType.VarChar).Value = CodingControl.GetUserIP();
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
        DBCmd.Parameters.Add("@AdminState", SqlDbType.Int).Value = AdminState;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int DisableProxyProviderAcount(int AdminID, int CompanyID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE AdminTable";
        SS += " SET AdminState = (CASE AdminState WHEN 1 THEN 0 ELSE 1 END)";
        SS += " WHERE AdminID=@AdminID And forCompanyID=@forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public List<DBViewModel.AdminTableResult> GetProxyProviderAcountResult(int CompanyID)
    {
        List<DBViewModel.AdminTableResult> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ProxyProviderGroup.GroupName,CompanyType,AdminTable.*,RoleName,convert(varchar, AdminTable.CreateDate, 120) as CreateDate2 FROM AdminTable WITH (NOLOCK) " +
             " Left Join AdminRole on AdminRole.AdminRoleID=AdminTable.forAdminRoleID" +
             " Left Join CompanyTable on CompanyTable.CompanyID=AdminTable.forCompanyID" +
             " Left Join ProxyProviderGroup on ProxyProviderGroup.GroupID=AdminTable.GroupID" +
             " WHERE AdminTable.forCompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminTableResult>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region WithdrawalIP
    public List<DBModel.WithdrawalIP> GetWithdrawalIPresult(int CompanyID)
    {
        List<DBModel.WithdrawalIP> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WIP.CreateDate,AT.RealName,WIP.CompanyCode,WIP.WithdrawalIP AS withdrawalIP,CT.CompanyName,CT.CompanyID AS forCompanyID," +
            " STUFF((" +
             " Select  ',' + CONVERT(varchar(100), IT.ImageID)+'_'+ImageName+'_'+convert(varchar, CreateDate, 120) " +
             " From ImageTable IT " +
             " where IT.TransactionID = WIP.WithdrawalIP AND IT.CompanyCode = WIP.CompanyCode And IT.Type=0 " +
             " For Xml Path(''))" +
             " , 1, 1, '') as ImageName" +
            " FROM WithdrawalIP WIP WITH (NOLOCK) " +
            " LEFT JOIN CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " LEFT JOIN AdminTable AT WITH (NOLOCK) on WIP.forAdminID= AT.AdminID " +
            " WHERE CT.CompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertWithdrawalIP(int CompanyID, string IP, int AdminID)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " INSERT INTO WithdrawalIP (WithdrawalIP,CompanyCode,forAdminID) " +
                 "   VALUES (@WithdrawalIP,@CompanyCode,@forAdminID) ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawalIP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public string UpdateImage(int CompanyID, string IP, byte[] ImageData, string _ImageName, string ModifyType, int ImageID, int Type)
    {
        int dbReturnImageID = -1;
        string returnStr = "";
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        string FileExtension = _ImageName;
        string ImageName = CompanyCode + "_" + IP + "." + FileExtension;

        if (ModifyType == "create")
        {
            SS = " INSERT INTO ImageTable (TransactionID,CompanyCode,Type) " +
                 "   VALUES (@TransactionID,@CompanyCode,0) " +
                 " SELECT @@IDENTITY ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            var DBReturn = DBAccess.GetDBValue(Pay.DBConnStr, DBCmd);
            if (DBReturn != null)
            {
                dbReturnImageID = int.Parse(DBReturn.ToString());
                ImageName = dbReturnImageID + "." + FileExtension;

                SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                     "   WHERE ImageID=@ImageID And CompanyCode=@CompanyCode AND TransactionID=@TransactionID AND Type=0 ";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
                DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
                DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
                DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = dbReturnImageID;
                DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
            }
        }
        else if (ModifyType == "update")
        {
            ImageName = ImageID + "." + FileExtension;

            SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                 " WHERE CompanyCode=@CompanyCode AND TransactionID=@TransactionID AND ImageID=@ImageID ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
            var DBReturn = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            if (DBReturn > 0)
            {
                dbReturnImageID = ImageID;
            }
        }

        if (dbReturnImageID > 0)
        {
            returnStr = ImageName;
            Pay.SaveFileWithForderName(ImageName, ImageData, "WithdrawalIP");
        }

        return returnStr;
    }

    public string UpdateFrozenPointImg(int FrozenID, byte[] ImageData, string _ImageName, string ModifyType, int ImageID, int Type)
    {
        int dbReturnImageID = -1;
        string returnStr = "";
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string FileExtension = _ImageName;
        string ImageName = "";

        if (ModifyType == "create")
        {
            SS = " INSERT INTO ImageTable (TransactionID,Type) " +
                 "   VALUES (@TransactionID,@Type) " +
                 " SELECT @@IDENTITY ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = FrozenID.ToString();
            DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
            var DBReturn = DBAccess.GetDBValue(Pay.DBConnStr, DBCmd);
            if (DBReturn != null)
            {
                dbReturnImageID = int.Parse(DBReturn.ToString());
                ImageName = dbReturnImageID + "." + FileExtension;

                SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                     "   WHERE ImageID=@ImageID  AND TransactionID=@TransactionID AND Type=@Type ";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = FrozenID.ToString();

                DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
                DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = dbReturnImageID;
                DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
                DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
            }
        }
        else if (ModifyType == "update")
        {
            ImageName = ImageID + "." + FileExtension;

            SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                 " WHERE  TransactionID=@TransactionID AND ImageID=@ImageID AND Type=@Type ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = FrozenID.ToString();
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
            DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
            var DBReturn = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            if (DBReturn > 0)
            {
                dbReturnImageID = ImageID;
            }
        }

        if (dbReturnImageID > 0)
        {
            returnStr = ImageName;
            Pay.SaveFileWithForderName(ImageName, ImageData, "FrozenPoint");
        }

        return returnStr;
    }

    public DBModel.ImageTable GetImageTableResultByImageID(int ImageID)
    {
        DBModel.ImageTable returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM ImageTable IT WITH (NOLOCK) " +
            " WHERE IT.ImageID = @ImageID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ImageTable>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public int DeleteWithdrawalIP(int CompanyID, string IP)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID); ;
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " DELETE FROM  WithdrawalIP" +
                 " WHERE WithdrawalIP=@WithdrawalIP And CompanyCode=@CompanyCode ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@WithdrawalIP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DeleteImageTableData(0, IP, CompanyCode);
            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public DBModel.WithdrawalIP GetWithdrawalIP(int CompanyID, string IP)
    {
        DBModel.WithdrawalIP returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM WithdrawalIP WIP WITH (NOLOCK) " +
            " Join CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " WHERE CT.CompanyID = @forCompanyID And WIP.WithdrawalIP=@WithdrawalIP";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@WithdrawalIP", SqlDbType.VarChar).Value = IP;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public void DeleteImageTableData(int Type, string TransactionID, string CompanyCode)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " DELETE FROM ImageTable " +
            " WHERE TransactionID = @TransactionID AND CompanyCode = @CompanyCode And Type = @Type";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = TransactionID;
        DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

    }

    public int DeleteImageByImageID(int ImageID)
    {
        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " DELETE FROM ImageTable " +
            " WHERE ImageID = @ImageID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        return returnValue;
    }
    #endregion

    #region 后台白名单
    public List<DBModel.WithdrawalIP> GetBackendIPresult(int CompanyID)
    {
        List<DBModel.WithdrawalIP> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WIP.CompanyCode,WIP.IP AS withdrawalIP,CT.CompanyName,CT.CompanyID AS forCompanyID," +
            " STUFF((" +
             " Select  ',' + CONVERT(varchar(100), IT.ImageID)+'_'+ImageName+'_'+convert(varchar, CreateDate, 120) " +
             " From ImageTable IT " +
             " where IT.TransactionID = WIP.IP AND IT.CompanyCode = WIP.CompanyCode And IT.Type=2 " +
             " For Xml Path(''))" +
             " , 1, 1, '') as ImageName" +
            " FROM BackendLoginIP WIP WITH (NOLOCK) " +
            " Join CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " WHERE CT.CompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertBackendIP(int CompanyID, string IP)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " INSERT INTO BackendLoginIP (IP,CompanyCode) " +
                 "   VALUES (@IP,@CompanyCode) ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public int DeleteBackendIP(int CompanyID, string IP)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " DELETE FROM  BackendLoginIP " +
                 " WHERE IP=@IP And CompanyCode=@CompanyCode ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
            DeleteImageTableData(2, IP, CompanyCode);
        }

        return returnValue;
    }

    public string UpdateBackendIPImage(int CompanyID, string IP, byte[] ImageData, string _ImageName, string ModifyType, int ImageID, int Type)
    {
        int dbReturnImageID = -1;
        string returnStr = "";
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        string FileExtension = _ImageName;
        string ImageName = CompanyCode + "_" + IP + "." + FileExtension;

        if (ModifyType == "create")
        {
            SS = " INSERT INTO ImageTable (TransactionID,CompanyCode,Type) " +
                 "   VALUES (@TransactionID,@CompanyCode,2) " +
                 " SELECT @@IDENTITY ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            var DBReturn = DBAccess.GetDBValue(Pay.DBConnStr, DBCmd);
            if (DBReturn != null)
            {
                dbReturnImageID = int.Parse(DBReturn.ToString());
                ImageName = dbReturnImageID + "." + FileExtension;

                SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                     "   WHERE ImageID=@ImageID And CompanyCode=@CompanyCode AND TransactionID=@TransactionID AND Type=2 ";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
                DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
                DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
                DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = dbReturnImageID;
                DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
            }
        }
        else if (ModifyType == "update")
        {
            ImageName = ImageID + "." + FileExtension;

            SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                 " WHERE CompanyCode=@CompanyCode AND TransactionID=@TransactionID AND ImageID=@ImageID ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
            var DBReturn = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            if (DBReturn > 0)
            {
                dbReturnImageID = ImageID;
            }
        }

        if (dbReturnImageID > 0)
        {
            returnStr = ImageName;
            Pay.SaveFileWithForderName(ImageName, ImageData, "BackendWithdrawalIP");
        }

        return returnStr;
    }

    public DBModel.WithdrawalIP GetBackendIP(int CompanyID, string IP)
    {
        DBModel.WithdrawalIP returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM BackendLoginIP WIP WITH (NOLOCK) " +
            " Join CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " WHERE CT.CompanyID = @forCompanyID And WIP.IP=@IP";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    #endregion

    #region 后台下发白名单
    public List<DBModel.WithdrawalIP> GetBackendWithdrawalIPresult(int CompanyID)
    {
        List<DBModel.WithdrawalIP> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WIP.ImageName,WIP.CompanyCode,WIP.IP AS withdrawalIP,CT.CompanyName,CT.CompanyID AS forCompanyID FROM BackendWithdrawalIP WIP WITH (NOLOCK) " +
            " Join CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " WHERE CT.CompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertBackendWithdrawalIP(int CompanyID, string IP)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " INSERT INTO BackendWithdrawalIP (IP,CompanyCode) " +
                 "   VALUES (@IP,@CompanyCode) ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public string UpdateBackendWithdrawalIPimg(int CompanyID, string IP, byte[] ImageData, string _ImageName)
    {
        int returnValue = -1;
        string returnStr = "";
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        string ImageName = CompanyCode + "_" + IP + "." + _ImageName;
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " UPDATE BackendWithdrawalIP SET ImageName=@ImageName " +
                 "   WHERE CompanyCode=@CompanyCode AND IP=@IP ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            if (returnValue > 0)
            {
                returnStr = ImageName;
                Pay.SaveFileWithForderName(ImageName, ImageData, "BackendWithdrawalIP");
            }
        }

        return returnStr;
    }

    public int DeleteBackendWithdrawalIP(int CompanyID, string IP)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        if (!string.IsNullOrEmpty(CompanyCode))
        {
            SS = " DELETE FROM  BackendWithdrawalIP " +
                 " WHERE IP=@IP And CompanyCode=@CompanyCode ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public DBModel.WithdrawalIP GetBackendWithdrawalIP(int CompanyID, string IP)
    {
        DBModel.WithdrawalIP returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM BackendWithdrawalIP WIP WITH (NOLOCK) " +
            " Join CompanyTable CT WITH (NOLOCK) on WIP.CompanyCode= CT.CompanyCode " +
            " WHERE CT.CompanyID = @forCompanyID And WIP.IP=@IP";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalIP>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    #endregion

    #region AdminRole

    public List<DBModel.AdminRole> GetAdminRoleTableByCompanyID(int CompanyID)
    {
        List<DBModel.AdminRole> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT AdminRoleID,RoleName,CompanyName FROM AdminRole WITH (NOLOCK) " +
            " Left Join CompanyTable WITH (NOLOCK) on AdminRole.forCompanyID= CompanyTable.CompanyID " +
            " WHERE forCompanyID = @forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AdminRole>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertAdminRole(int CompanyID, string RoleName, List<string> AdminPermission, List<string> NormalPermission)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        int AdminRoleID;
        SS = " SELECT count(*) FROM AdminRole WITH (NOLOCK) " +
             " WHERE RoleName=@RoleName And forCompanyID=@forCompanyID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = RoleName;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        //角色名稱重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {

            SS = "   INSERT INTO AdminRole" +
                 "              (RoleName," +
                 "               forCompanyID)" +
                 "   VALUES" +
                 "              (@RoleName," +
                 "               @forCompanyID);" +
                 "   SELECT @@IDENTITY;";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = RoleName;

            AdminRoleID = int.Parse(T.GetDBValue(DBCmd).ToString());
            returnValue = AdminRoleID;
            foreach (var AdminPermissionName in AdminPermission)
            {
                SS = " INSERT INTO AdminRolePermission (forAdminRoleID,forCompanyID,forPermissionName)" +
                     " VALUES (@forAdminRoleID,@forCompanyID,@forPermissionName)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;
                DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = AdminPermissionName;
                T.ExecuteDB(DBCmd);
            }

            foreach (var NormalPermissionName in NormalPermission)
            {
                SS = " INSERT INTO AdminRolePermission (forAdminRoleID,forCompanyID,forPermissionName)" +
                     " VALUES (@forAdminRoleID,@forCompanyID,@forPermissionName)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;
                DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = NormalPermissionName;

                T.ExecuteDB(DBCmd);
            }
        });
        return returnValue;

    }

    public int UpdateAdminRole(int CompanyID, int AdminRoleID, string RoleName, List<string> AdminPermission, List<string> NormalPermission)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT count(*) FROM AdminRole WITH (NOLOCK) " +
             " WHERE RoleName=@RoleName And forCompanyID=@forCompanyID And AdminRoleID=@AdminRoleID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = RoleName;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@AdminRoleID", SqlDbType.Int).Value = AdminRoleID;

        //先檢查名稱是否與之前的相同
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) != 1)
        {
            SS = " SELECT count(*) FROM AdminRole WITH (NOLOCK) " +
                 " WHERE RoleName=@RoleName And forCompanyID=@forCompanyID";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = RoleName;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            //角色名稱重複
            if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
            {
                returnValue = -1;
                return returnValue;
            }
        }

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {

            SS = " UPDATE AdminRole" +
                 " SET RoleName=@RoleName" +
                 " WHERE AdminRoleID=@AdminRoleID ";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = RoleName;
            DBCmd.Parameters.Add("@AdminRoleID", SqlDbType.Int).Value = AdminRoleID;

            T.ExecuteDB(DBCmd);


            SS = " DELETE FROM AdminRolePermission" +
                 " WHERE forAdminRoleID=@forAdminRoleID";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;

            T.ExecuteDB(DBCmd);


            foreach (var AdminPermissionName in AdminPermission)
            {
                SS = " INSERT INTO AdminRolePermission (forAdminRoleID,forCompanyID,forPermissionName)" +
                     " VALUES (@forAdminRoleID,@forCompanyID,@forPermissionName)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;
                DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = AdminPermissionName;
                T.ExecuteDB(DBCmd);
            }

            foreach (var NormalPermissionName in NormalPermission)
            {
                SS = " INSERT INTO AdminRolePermission (forAdminRoleID,forCompanyID,forPermissionName)" +
                     " VALUES (@forAdminRoleID,@forCompanyID,@forPermissionName)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;
                DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = NormalPermissionName;
                T.ExecuteDB(DBCmd);
            }
        });
        return returnValue;

    }

    public List<DBViewModel.AdminRolePermission> GetPermissionByAdminRoleID(int AdminRoleID, int CompanyType)
    {
        List<DBViewModel.AdminRolePermission> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable AdminRoleDT;
        DataTable AllAdminRoleDT;

        //取得目前修改角色擁有權限
        SS = " SELECT forAdminRoleID, Description, PermissionName, AdminPermission FROM AdminRolePermission WITH(NOLOCK)" +
             " Join PermissionTable WITH(NOLOCK) on AdminRolePermission.forPermissionName = PermissionTable.PermissionName" +
             " WHERE forAdminRoleID = @AdminRoleID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminRoleID", SqlDbType.Int).Value = AdminRoleID;

        AdminRoleDT = DBAccess.GetDB(DBConnStr, DBCmd);

        //取得所有權限
        if (CompanyType == 0)
        {
            //管理者權限
            SS = " SELECT PermissionName, AdminPermission,Description FROM PermissionTable WITH(NOLOCK) ";
        }
        else
        {
            //一般角色權限
            SS = " SELECT PermissionName, AdminPermission,Description FROM PermissionTable WITH(NOLOCK) where AdminPermission=0";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        AllAdminRoleDT = DBAccess.GetDB(DBConnStr, DBCmd);



        if (AllAdminRoleDT != null)
        {
            if (AllAdminRoleDT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminRolePermission>(AllAdminRoleDT).ToList();
                foreach (var rowData in returnValue)
                {
                    if (AdminRoleDT.Select("PermissionName='" + rowData.PermissionName + "'").Count() > 0)
                    {
                        rowData.selectedPermission = true;
                    }
                    else
                    {
                        rowData.selectedPermission = false;
                    }
                }
            }
        }

        return returnValue;
    }

    public List<string> GetPermissionByRoleName(string RoleName, int CompanyID)
    {
        List<string> returnValue = new List<string>();
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable AdminRoleDT;


        //取得目前修改角色擁有權限
        SS = " SELECT forPermissionName FROM AdminRole WITH(NOLOCK)" +
             " Join AdminRolePermission WITH(NOLOCK) on AdminRolePermission.forAdminRoleID = AdminRole.AdminRoleID" +
             " WHERE AdminRole.forCompanyID=@CompanyID And RoleName=@RoleName";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@RoleName", SqlDbType.VarChar).Value = RoleName;
        AdminRoleDT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (AdminRoleDT != null && AdminRoleDT.Rows.Count > 0)
        {
            for (int i = 0; i < AdminRoleDT.Rows.Count; i++)
            {
                returnValue.Add(AdminRoleDT.Rows[i]["forPermissionName"].ToString());
            }

        }

        return returnValue;
    }
    #endregion

    #region BackendNotify

    public List<DBModel.BackendNotifyTable> GetBackendNotifyTableResult(int CompanyID)
    {
        List<DBModel.BackendNotifyTable> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CompanyName,convert(varchar, BackendNotify.CreateDate, 120) as CreateDate,BackendNotify.Data,BackendNotify.Type,BackendNotifyID FROM BackendNotify WITH (NOLOCK) " +
             " JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=BackendNotify.forCompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.BackendNotifyTable>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.BackendNotifyTable> GetBackendNotifyTableResult2(int CompanyID, int AdminID)
    {
        List<DBModel.BackendNotifyTable> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT Data,CompanyName,convert(varchar, BackendNotify.CreateDate, 120) as CreateDate,BackendNotify.Type,BackendNotifyID FROM BackendNotify WITH (NOLOCK) " +
             " JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=BackendNotify.forCompanyID " +
              " Where OpenByAdminID is null";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.BackendNotifyTable>(DT).ToList();
                //修改查看通知者為目前登入者
                SS = " UPDATE BackendNotify" +
                     " SET OpenByAdminID=@OpenByAdminID" +
                     " Where OpenByAdminID is null";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@OpenByAdminID", SqlDbType.Int).Value = AdminID;

                DBAccess.ExecuteDB(DBConnStr, DBCmd);

            }
        }

        return returnValue;
    }

    public void CreateBackendNotify(int forCompanyID, int Type, string Data)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " INSERT INTO BackendNotify (forCompanyID,Type,Data)" +
             " VALUES(@forCompanyID,@Type,@Data);";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = forCompanyID;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
        DBCmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = Data;

        DBAccess.ExecuteDB(DBConnStr, DBCmd);
    }

    #endregion

    #region  CompanyService

    #region 商户通道调整
    public List<DBViewModel.CompanyServiceRelation> GetCompanyServiceRelationByEditView(string ServiceType, string ProviderCode)
    {
        List<DBViewModel.CompanyServiceRelation> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT CT.CompanyName,CT.CompanyID," +
             " IIF(((SELECT COUNT(*) FROM  GPayRelation  GR WHERE SC.ServiceType = GR.ServiceType" +
             " AND GR.CurrencyType = SC.CurrencyType" +
             " AND GR.forCompanyID = SC.forCompanyID" +
             " AND GR.ProviderCode = @ProviderCode) = 0), 0, 1) AS isSelected" +
             " FROM CompanyTable CT WITH(NOLOCK)" +
             " JOIN CompanyService SC WITH(NOLOCK)" +
             " ON CT.CompanyID = SC.forCompanyID" +
             " WHERE SC.ServiceType = @ServiceType AND SC.CurrencyType = @CurrencyType" +
             " AND CT.CompanyState = 0";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Pay.CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServiceRelation>(DT).ToList();
            }
        }

        return returnValue;
    }


    public int InsertCompanyServiceByEditView(FromBody.CompanyServiceSet fromBody, int CompanyType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM CompanyService WITH (NOLOCK) " +
             " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType  And CurrencyType=@CurrencyType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;

        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);

        if (CompanyTD == null)
        {
            returnValue = -2;
            return returnValue;
        }

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {
            //新增目前選擇公司的 CompanyService
            SS = " INSERT INTO CompanyService (forCompanyID,ServiceType,CurrencyType,CollectRate,CollectCharge,MaxDaliyAmount,MaxOnceAmount,MinOnceAmount,State,DeviceType,MaxDaliyAmountByUse,CheckoutType,Description)" +
       " VALUES(@forCompanyID,@ServiceType,@CurrencyType,@CollectRate,@CollectCharge,@MaxDaliyAmount,@MaxOnceAmount,@MinOnceAmount,@State,@DeviceType,@MaxDaliyAmountByUse,@CheckoutType,@Description)";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
            DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
            DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = fromBody.CollectRate;
            DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = fromBody.CollectCharge;
            DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
            DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = fromBody.MaxOnceAmount;
            DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = fromBody.MinOnceAmount;
            DBCmd.Parameters.Add("@MaxDaliyAmountByUse", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
            DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = fromBody.CheckoutType;
            DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = fromBody.State;
            DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = "";
            DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = 0;

            returnValue = T.ExecuteDB(DBCmd);
            if (returnValue > 0)
            {
                RedisCache.CompanyService.UpdateCompanyService(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
            }
        });


        return returnValue;
    }

    public int SetCompanyServiceRelationByEditView(FromBody.GPayRelationSet fromBody)
    {
        int returnValue = 0;

        String SS = String.Empty;
        SqlCommand DBCmd;

        var CompanyTD = GetCompanyByID(fromBody.forCompanyID);

        if (CompanyTD == null)
        {
            returnValue = -1;
            return returnValue;
        }

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {
            //如果是删除
            if (!fromBody.isAddRelation)
            {
                //先刪除舊有資料
                SS = "DELETE FROM GPayRelation WHERE forCompanyID =@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType And ProviderCode=@ProviderCode ";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
                returnValue = T.ExecuteDB(DBCmd);

                if (returnValue > 0)
                {
                    SS = " SELECT COUNT(*) FROM GPayRelation WITH (NOLOCK) " +
                                                 " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType  And CurrencyType=@CurrencyType";
                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;

                    if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
                    {
                        RedisCache.GPayRelation.UpdateGPayRelation(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    }
                    else
                    {
                        RedisCache.GPayRelation.DeleteGPayRelation(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    }
                }
            }
            else
            {//新增
                SS = " INSERT INTO GPayRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID,Weight) " +
               " VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID,@Weight)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;
                DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = 1;
                returnValue = T.ExecuteDB(DBCmd);
                if (returnValue > 0)
                {
                    RedisCache.GPayRelation.UpdateGPayRelation(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                }
            }
        });

        return returnValue;

    }

    public int UpdateCompanyServiceByEditView(FromBody.CompanyServiceSet fromBody)
    {
        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);
        //新增 GPayRelation
        if (CompanyTD == null)
        {
            returnValue = -2;
            return returnValue;
        }

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {
            SS = " UPDATE CompanyService";
            SS += " SET CollectRate=@CollectRate,CollectCharge=@CollectCharge,MaxDaliyAmount=@MaxDaliyAmount,MaxOnceAmount=@MaxOnceAmount,MinOnceAmount=@MinOnceAmount,DeviceType=@DeviceType,State=@State,CheckoutType=@CheckoutType,MaxDaliyAmountByUse=MaxDaliyAmountByUse - @modifyMaxDaliyAmountByUse";
            SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
            DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
            DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = fromBody.CollectRate;
            DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = fromBody.CollectCharge;
            DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
            DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = fromBody.MaxOnceAmount;
            DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = fromBody.MinOnceAmount;
            DBCmd.Parameters.Add("@modifyMaxDaliyAmountByUse", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
            DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = fromBody.State;
            DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = fromBody.CheckoutType;
            DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = 0;

            returnValue = T.ExecuteDB(DBCmd);

            if (returnValue > 0)
            {
                RedisCache.CompanyService.UpdateCompanyService(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);

                if (CompanyTD.ParentCompanyID == 0)
                {
                    var childCompanys = GetCompany2(fromBody.CompanyID, 2);
                    if (childCompanys != null)
                    {
                        if (childCompanys.Count > 0)
                        {
                            foreach (var childCompany in childCompanys)
                            {
                                SS = " UPDATE CompanyService";
                                SS += " SET CheckoutType=@CheckoutType";
                                SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                                DBCmd = new System.Data.SqlClient.SqlCommand();
                                DBCmd.CommandText = SS;
                                DBCmd.CommandType = CommandType.Text;
                                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = childCompany.CompanyID;
                                DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                                DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = fromBody.CheckoutType;

                                var updateCount = T.ExecuteDB(DBCmd);
                                if (updateCount > 0)
                                {
                                    RedisCache.CompanyService.UpdateCompanyService(childCompany.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                                }
                            }
                        }
                    }
                }
            }




        });

        return returnValue;
    }

    public int UpdateCompanyServiceWeightByEditView(FromBody.CompanyServiceSet fromBody)
    {

        int returnValue = 0;
        int successCount = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);

        if (CompanyTD == null)
        {
            returnValue = -2;
            return returnValue;
        }

        foreach (var providerdata in fromBody.ProviderCodeAndWeight)
        {
            SS = " Update GPayRelation SET Weight=@Weight " +
                                   " WHERE ProviderCode=@ProviderCode And ServiceType=@ServiceType" +
                                   " And CurrencyType=@CurrencyType And forCompanyID=@forCompanyID";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = providerdata.ProviderCode;
            DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
            DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = providerdata.Weight;
            successCount += DBAccess.ExecuteDB(DBConnStr, DBCmd);
        }
        if (successCount > 0)
        {
            returnValue = successCount;
            RedisCache.GPayRelation.UpdateGPayRelation(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
        }
        return returnValue;
    }
    #endregion

    public List<DBViewModel.CompanyServiceTableResult> GetCompanyServiceTableByCompanyID(int CompanyID)
    {
        List<DBViewModel.CompanyServiceTableResult> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CompanyService.*," +
                 "        CompanyName," +
                 "        ServiceType.ServiceTypeName," +
                 "        (SELECT Count (GPayRelation.CurrencyType)" +
                 "         FROM   GPayRelation WITH (NOLOCK)" +
                 "         WHERE  GPayRelation.CurrencyType = CompanyService.CurrencyType" +
                 "                AND GPayRelation.ServiceType = CompanyService.ServiceType" +
                 "                AND GPayRelation.forCompanyID = CompanyService.forCompanyID) AS GPayRelationCount" +
                 " FROM   CompanyService WITH (NOLOCK)" +
                 "        LEFT JOIN CompanyTable" +
                 "               ON CompanyService.forCompanyID = CompanyTable.CompanyID" +
                 "        LEFT JOIN ServiceType" +
                 "               ON ServiceType.ServiceType = CompanyService.ServiceType" +
                 " WHERE  CompanyService.forCompanyID = @CompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServiceTableResult>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int GetCompanyServiceState(string ServiceType, string CurrencyType, int CompanyID)
    {

        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "SELECT State FROM CompanyService WITH (NOLOCK)  WHERE forCompanyID =@CompanyID AND ServiceType = @ServiceType AND CurrencyType =@CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());
        return returnValue;
    }

    public DBModel.CompanyService GetCompanyService(int CompanyID, string ServiceType, string CurrencyType)
    {
        DBModel.CompanyService returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM CompanyService WITH (NOLOCK)" +
                 " WHERE forCompanyID = @CompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServiceTableResult>(DT).First();
            }
        }

        return returnValue;
    }

    public decimal GetCompanyServiceMaxDaliyAmountByUse(int CompanyID, string ServiceType, string CurrencyType)
    {
        decimal returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT MaxDaliyAmountByUse" +
             " FROM   CompanyService WITH (NOLOCK)" +
             " WHERE  ServiceType = @ServiceType And forCompanyID=@CompanyID And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        decimal.TryParse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString(), out returnValue);

        return returnValue;
    }

    public DBModel.CompanyService GetSelectedCompanyService(int CompanyID, string ServiceType, string CurrencyType)
    {
        DataTable DT;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DBModel.CompanyService returnValue = null;
        SS = " SELECT *" +
             " FROM   CompanyService WITH (NOLOCK)" +
             " WHERE  ServiceType = @ServiceType And forCompanyID=@CompanyID And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyService>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public int InsertCompanyService(FromBody.CompanyServiceSet fromBody, int CompanyType)
    {
        int returnValue = 0;
        int CheckoutType = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        decimal modifyMaxDaliyAmountByUse = 0;
        SS = " SELECT COUNT(*) FROM CompanyService WITH (NOLOCK) " +
             " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType  And CurrencyType=@CurrencyType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;

        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);
        if (CompanyTD != null)
        {
            //當有上線時,先查詢上線額度
            if (CompanyTD.ParentCompanyID != 0)
            {
                //var upLineMaxDaliyAmountByUse = GetCompanyServiceMaxDaliyAmountByUse(CompanyTD.ParentCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                //if (upLineMaxDaliyAmountByUse == -1)
                //{//查詢不到上線渠道時
                //    returnValue = -2;
                //    return returnValue;
                //}
                //else
                //{
                //    //確認上線額度是否足夠     
                //    modifyMaxDaliyAmountByUse = 0 - fromBody.MaxDaliyAmount;
                //    if (upLineMaxDaliyAmountByUse + modifyMaxDaliyAmountByUse < 0)
                //    {
                //        returnValue = -2;
                //        return returnValue;
                //    }
                //    CheckoutType = fromBody.CheckoutType;
                //}
                //取得上線渠道設定
                var parentServiceType = GetSelectedCompanyService(CompanyTD.ParentCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                if (parentServiceType != null)
                {
                    CheckoutType = parentServiceType.CheckoutType;
                }
            }

            DBAccess.ExecuteTransDB(DBConnStr, T =>
            {
                //新增目前選擇公司的 CompanyService
                SS = " INSERT INTO CompanyService (forCompanyID,ServiceType,CurrencyType,CollectRate,CollectCharge,MaxDaliyAmount,MaxOnceAmount,MinOnceAmount,State,DeviceType,MaxDaliyAmountByUse,CheckoutType,Description)" +
           " VALUES(@forCompanyID,@ServiceType,@CurrencyType,@CollectRate,@CollectCharge,@MaxDaliyAmount,@MaxOnceAmount,@MinOnceAmount,@State,@DeviceType,@MaxDaliyAmountByUse,@CheckoutType,@Description)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = fromBody.CollectRate;
                DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = fromBody.CollectCharge;
                DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
                DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = fromBody.MaxOnceAmount;
                DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = fromBody.MinOnceAmount;
                DBCmd.Parameters.Add("@MaxDaliyAmountByUse", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
                DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = CheckoutType;
                DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = fromBody.State;
                DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
                DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = 0;

                returnValue = T.ExecuteDB(DBCmd);
                RedisCache.CompanyService.UpdateCompanyService(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                //如果有上線,修改上線的每日可使用量
                //if (CompanyTD.ParentCompanyID != 0)
                //{
                //    SS = " UPDATE CompanyService";
                //    SS += " SET MaxDaliyAmountByUse=MaxDaliyAmountByUse-@MaxDaliyAmount";
                //    SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                //    DBCmd = new System.Data.SqlClient.SqlCommand();
                //    DBCmd.CommandText = SS;
                //    DBCmd.CommandType = CommandType.Text;
                //    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyTD.ParentCompanyID;
                //    DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                //    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                //    DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;

                //    T.ExecuteDB(DBCmd);
                //    RedisCache.CompanyService.UpdateCompanyService(CompanyTD.ParentCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                //}


                //新增 GPayRelation
                if (returnValue > 0)
                {
                    // if (CompanyType == 0 && CompanyTD.ParentCompanyID == 0) {
                    //先刪除舊有資料
                    SS = "DELETE FROM GPayRelation WHERE forCompanyID =@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                    DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                    T.ExecuteDB(DBCmd);
                    RedisCache.GPayRelation.DeleteGPayRelation(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    //最高權限允許以勾選方式新增供應商
                    //新增資料
                    foreach (var providerdata in fromBody.ProviderCodeAndWeight)
                    {
                        SS = " INSERT INTO GPayRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID,Weight) " +
                             " VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID,@Weight)";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = providerdata.ProviderCode;
                        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
                        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
                        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                        DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = providerdata.Weight;
                        T.ExecuteDB(DBCmd);

                    }

                    RedisCache.GPayRelation.UpdateGPayRelation(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    //}
                }
            });
        }

        return returnValue;

    }

    public int UpdateCompanyService(FromBody.CompanyServiceSet fromBody, int CompanyType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);
        //新增 GPayRelation
        if (CompanyTD != null)
        {

            DBAccess.ExecuteTransDB(DBConnStr, T =>
            {
                SS = " UPDATE CompanyService";
                SS += " SET Description=@Description,CollectRate=@CollectRate,CollectCharge=@CollectCharge,MaxDaliyAmount=@MaxDaliyAmount,MaxOnceAmount=@MaxOnceAmount,MinOnceAmount=@MinOnceAmount,DeviceType=@DeviceType,State=@State,CheckoutType=@CheckoutType,MaxDaliyAmountByUse=MaxDaliyAmountByUse - @modifyMaxDaliyAmountByUse";
                SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = fromBody.CollectRate;
                DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = fromBody.CollectCharge;
                DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
                DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = fromBody.MaxOnceAmount;
                DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = fromBody.MinOnceAmount;
                DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
                DBCmd.Parameters.Add("@modifyMaxDaliyAmountByUse", SqlDbType.Decimal).Value = fromBody.MaxDaliyAmount;
                DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = fromBody.State;
                DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = fromBody.CheckoutType;
                DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = 0;

                returnValue = T.ExecuteDB(DBCmd);
                RedisCache.CompanyService.UpdateCompanyService(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);

                if (CompanyTD.ParentCompanyID == 0)
                {

                    var childCompanys = GetCompany2(fromBody.CompanyID, 2);
                    if (childCompanys != null)
                    {
                        if (childCompanys.Count > 0)
                        {
                            foreach (var childCompany in childCompanys)
                            {
                                SS = " UPDATE CompanyService";
                                SS += " SET CheckoutType=@CheckoutType";
                                SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                                DBCmd = new System.Data.SqlClient.SqlCommand();
                                DBCmd.CommandText = SS;
                                DBCmd.CommandType = CommandType.Text;
                                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = childCompany.CompanyID;
                                DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                                DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = fromBody.CheckoutType;

                                var updateCount = T.ExecuteDB(DBCmd);
                                if (updateCount > 0)
                                {
                                    RedisCache.CompanyService.UpdateCompanyService(childCompany.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                                }
                            }
                        }
                    }


                }

                if (returnValue > 0)
                {
                    //if (CompanyType == 0 && CompanyTD.ParentCompanyID == 0) {
                    //先刪除舊有資料
                    SS = "DELETE FROM GPayRelation WHERE forCompanyID =@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                    DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                    T.ExecuteDB(DBCmd);
                    RedisCache.GPayRelation.DeleteGPayRelation(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    //新增資料
                    foreach (var providerdata in fromBody.ProviderCodeAndWeight)
                    {
                        SS = " INSERT INTO GPayRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID,Weight) " +
                                               " VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID,@Weight)";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = providerdata.ProviderCode;
                        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
                        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
                        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                        DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = providerdata.Weight;
                        T.ExecuteDB(DBCmd);
                        RedisCache.GPayRelation.UpdateGPayRelation(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
                    }
                    //}
                }
            });
        }
        return returnValue;
    }

    public int DeleteProviderService(FromBody.DeleteProviderService fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " DELETE FROM  ProviderService";
        SS += " WHERE ProviderCode=@ProviderCode And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        if (returnValue > 0)
        {
            RedisCache.ProviderService.DeleteProviderService(fromBody.ProviderCode, fromBody.ServiceType, fromBody.CurrencyType);
        }

        return returnValue;

    }

    public int DisableCompanyService(FromBody.CompanyServiceSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE CompanyService";
        SS += " SET State = (CASE State WHEN 1 THEN 0 ELSE 1 END)";
        SS += " WHERE forCompanyID=@forCompanyID And ServiceType=@ServiceType And CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.CompanyService.UpdateCompanyService(fromBody.CompanyID, fromBody.ServiceType, fromBody.CurrencyType);
        return returnValue;

    }

    public int UpdateBlockChainRate(string CurrencyType,decimal Rate,decimal DownFloatRate,decimal UpFloatRate)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE BlockChainRate";
        SS += " SET Rate = @Rate,DownFloatRate=@DownFloatRate,UpFloatRate=@UpFloatRate";
        SS += " WHERE CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = Rate;
        DBCmd.Parameters.Add("@UpFloatRate", SqlDbType.Decimal).Value = UpFloatRate;
        DBCmd.Parameters.Add("@DownFloatRate", SqlDbType.Decimal).Value = DownFloatRate;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.BlockChainRate.UpdateBlockChainRate(CurrencyType);
        RedisCache.BlockChainRate.UpdateAllBlockChainRate();
        return returnValue;

    }

    public int CreateBlockChainRate(string CurrencyType, decimal Rate, decimal DownFloatRate, decimal UpFloatRate)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM BlockChainRate WITH(NOLOCK) WHERE CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        if (returnValue>0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO BlockChainRate (Rate,CurrencyType,DownFloatRate,UpFloatRate)" +
             " VALUES(@Rate,@CurrencyType,@DownFloatRate,@UpFloatRate);" +
             " Select @@IDENTITY";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = Rate;
        DBCmd.Parameters.Add("@UpFloatRate", SqlDbType.Decimal).Value = UpFloatRate;
        DBCmd.Parameters.Add("@DownFloatRate", SqlDbType.Decimal).Value = DownFloatRate;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.BlockChainRate.UpdateBlockChainRate(CurrencyType);
        RedisCache.BlockChainRate.UpdateAllBlockChainRate();
        return returnValue;

    }

    public int UpdateWithdrawOption(string WithdrawOption)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE WebSetting";
        SS += " SET SettingValue = @SettingValue";
        SS += " WHERE SettingKey=@SettingKey";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = WithdrawOption;
        DBCmd.Parameters.Add("@SettingKey", SqlDbType.VarChar).Value = "WithdrawOption";

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.WebSetting.UpdateSettingKey("WithdrawOption");

        return returnValue;

    }

    public int UpdateWebsiteOption(string WithdrawOption)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        string MaintainUrl;

        SS = " UPDATE WebSetting";
        SS += " SET SettingValue = @SettingValue";
        SS += " WHERE SettingKey=@SettingKey";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = WithdrawOption;
        DBCmd.Parameters.Add("@SettingKey", SqlDbType.VarChar).Value = "WebsiteOption";

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.WebSetting.UpdateSettingKey("WebsiteOption");
        string secret = "PaymentMaintAPI";

        // 1. 取得當前 Unix timestamp（秒）
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 2. 組合 secret + timestamp
        string raw = secret + timestamp;

        // 3. SHA256 雜湊
        string hash = CodingControl.GetSHA256(raw,false);

        // 4. 組合成 auth token
        string auth = timestamp+"_"+hash;

        JObject jobj = new JObject();
        jobj["auth"] = auth;
 
        if (WithdrawOption == "0")
        {
            MaintainUrl = Pay.MaintainUrl + "/clear-maint";
        } else {
            MaintainUrl = Pay.MaintainUrl + "/set-maint";
        }
        CodingControl.RequestJsonAPI(MaintainUrl, jobj.ToString());

        return returnValue;

    }

    public int UpdateConfigSetting(string SettingKey, string SettingValue)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE WebSetting";
        SS += " SET SettingValue = @SettingValue";
        SS += " WHERE SettingKey=@SettingKey";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = SettingValue;
        DBCmd.Parameters.Add("@SettingKey", SqlDbType.VarChar).Value = SettingKey;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.WebSetting.UpdateSettingKey(SettingKey);
      
        return returnValue;

    }

    public List<DBModel.ConfigSetting> GetConfigSetting(string SettingKey = "")
    {
        List<DBModel.ConfigSetting> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        if (SettingKey == "")
        {
            SS = " Select * from WebSetting";
        }
        else
        {
            SS = " Select * from WebSetting";
            SS += " WHERE SettingKey=@SettingKey";
        }


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@SettingKey", SqlDbType.VarChar).Value = SettingKey;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ConfigSetting>(DT).ToList();
            }
        }
        return returnValue;

    }

    public string GetConfigSettingFormServer()
    {
        List<DBModel.ConfigSetting> returnValue = null;
        string MaintainUrl;
        string DBreturn = "F";
        MaintainUrl = Pay.MaintainUrl + "/status";

        string secret = "PaymentMaintAPI";

        // 1. 取得當前 Unix timestamp（秒）
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 2. 組合 secret + timestamp
        string raw = secret + timestamp;

        // 3. SHA256 雜湊
        string hash = CodingControl.GetSHA256(raw,false);

        // 4. 組合成 auth token
        string auth = timestamp + "_" + hash;

        MaintainUrl = MaintainUrl + "?auth=" + auth;
        var rtn = CodingControl.RequestJsonAPIByGet(MaintainUrl);

        try
        {
            JObject jsonData = JsonConvert.DeserializeObject<JObject>(rtn);
            if (jsonData["maintenance"].ToString().ToUpper() == "TRUE")
            {
                DBreturn = "Y";
            }
            else if (jsonData["maintenance"].ToString().ToUpper() == "FALSE")
            {
                DBreturn = "N";
            }

            return DBreturn;
        }
        catch (Exception ex)
        {
            return rtn;
        }
    }

    public List<DBModel.ConfigSetting> GetConfigSettingFromRedis(string SettingKey)
    {
        List<DBModel.ConfigSetting> returnValue = null;

        DataTable DT;

        DT = RedisCache.WebSetting.GetWebSetting(SettingKey);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ConfigSetting>(DT).ToList();
            }
        }
        return returnValue;

    }

    public void FastInsertCompanyServiceFromParentCompany(int ParentCompanyID, int CompanyID,string CurrencyType)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        SS = " SELECT * FROM CompanyService WITH (NOLOCK) " +
             " WHERE forCompanyID=@forCompanyID And CurrencyType=@CurrencyType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = ParentCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                var CompanyServiceDT = DataTableExtensions.ToList<DBModel.CompanyService>(DT).ToList();
                foreach (var companyServiceRow in CompanyServiceDT)
                {
                    if (!companyServiceRow.ServiceType.Contains("PROXY"))
                    {
                        SS = " INSERT INTO CompanyService (forCompanyID,ServiceType,CurrencyType,CollectRate,CollectCharge,MaxDaliyAmount,MaxOnceAmount,MinOnceAmount,State,DeviceType,MaxDaliyAmountByUse,CheckoutType)" +
                                       " VALUES(@forCompanyID,@ServiceType,@CurrencyType,@CollectRate,@CollectCharge,@MaxDaliyAmount,@MaxOnceAmount,@MinOnceAmount,@State,@DeviceType,@MaxDaliyAmountByUse,@CheckoutType)";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                        DBCmd.Parameters.Add("@ServiceType", SqlDbType.NVarChar).Value = companyServiceRow.ServiceType;
                        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = companyServiceRow.CurrencyType;
                        DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@MaxDaliyAmountByUse", SqlDbType.Decimal).Value = 0;
                        DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = companyServiceRow.CheckoutType;
                        DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = companyServiceRow.State;
                        DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = 0;
                        DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    }
                }
            }
        }
    }

    #endregion

    #region AgentReceive

    public List<DBModel.AgentReceive> GetAgentReceiveTableResult(FromBody.AgentReceiveSet fromBody)
    {
        List<DBModel.AgentReceive> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = @" SELECT 
                ReceiveStatus,
                -- 根據 OrderType 決定 PaymentSerial 來源
                CASE 
                    WHEN AgentReceive.OrderType = 0 THEN PaymentTable.PaymentSerial
                    WHEN AgentReceive.OrderType = 1 THEN Withdrawal.WithdrawSerial
                    ELSE NULL
                END AS PaymentSerial,

                ServiceTypeName,
                CompanyTable.CompanyName AS ChildCompanyName,
                AgentReceive.*,
                convert(varchar, DATEADD(HOUR, @TimeZone - 8, PaymentFinishDate), 120) as PaymentFinishDate2,

                -- 根據 OrderType 決定 OrderAmount 來源
                CASE 
                    WHEN AgentReceive.OrderType = 0 THEN PaymentTable.PartialOrderAmount
                    WHEN AgentReceive.OrderType = 1 THEN Withdrawal.Amount
                    ELSE NULL
                END AS OrderAmount

            FROM AgentReceive WITH (NOLOCK)
            LEFT JOIN ServiceType ON ServiceType.ServiceType = AgentReceive.ServiceType
            LEFT JOIN CompanyTable ON CompanyTable.CompanyID = AgentReceive.forChildCompanyID
            LEFT JOIN PaymentTable ON PaymentTable.PaymentID = AgentReceive.forPaymentID
            LEFT JOIN Withdrawal ON Withdrawal.WithdrawID = AgentReceive.forPaymentID

            WHERE AgentReceive.forCompanyID = @CompanyID
              AND AgentReceive.Currency = @CurrencyType
              AND DATEADD(HOUR, @TimeZone - 8, PaymentFinishDate) >= @StartDate AND DATEADD(HOUR, @TimeZone - 8, PaymentFinishDate) <= @EndDate
             ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AgentReceive>(DT).ToList();

            }
        }

        return returnValue;
    }

    public List<DBModel.AgentClose> GetAgentCloseTableResult(FromBody.AgentReceiveSet fromBody)
    {
        List<DBModel.AgentClose> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT *,convert(varchar, DATEADD(HOUR, @TimeZone - 8, StartDate), 120) as StartDate2,convert(varchar, DATEADD(HOUR, @TimeZone - 8, EndDate), 120) as EndDate2,convert(varchar, DATEADD(HOUR, @TimeZone - 8, CreateDate), 120) as CreateDate2 FROM AgentClose WITH (NOLOCK) " +
             " WHERE forCompanyID = @CompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.AgentClose>(DT).ToList();

            }
        }

        return returnValue;
    }

    public DBViewModel.AgentReceiveVM GetAgentAmountResult(FromBody.AgentReceiveSet fromBody)
    {
        DBViewModel.AgentReceiveVM returnValue = new DBViewModel.AgentReceiveVM();
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ISNULL(SUM(AgentReceive.ReceiveAmount),0) ReceiveAmount,ISNULL(SUM(PaymentTable.PartialOrderAmount),0) PartialOrderAmount FROM AgentReceive WITH (NOLOCK) " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=AgentReceive.ServiceType" +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID=AgentReceive.forChildCompanyID" +
             " LEFT JOIN PaymentTable ON PaymentTable.PaymentID=AgentReceive.forPaymentID" +
             " WHERE AgentReceive.forCompanyID = @CompanyID AND AgentReceive.Currency = @CurrencyType AND DATEADD(HOUR, @TimeZone, PaymentFinishDate) >= @StartDate AND DATEADD(HOUR, @TimeZone, PaymentFinishDate) <= @EndDate ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue.AgentAmount = (decimal)DT.Rows[0]["ReceiveAmount"];
                returnValue.OrderAmount = (decimal)DT.Rows[0]["PartialOrderAmount"];

            }
        }

        return returnValue;
    }

    public DBViewModel.AgentReceiveVM GetCompanyAgentPointResult(int CompanyID, string CurrencyType = "")
    {
        DBViewModel.AgentReceiveVM returnValue = new DBViewModel.AgentReceiveVM();
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CurrencyType != "")
        {
            SS = " SELECT ISNULL(SUM(CASE ReceiveStatus WHEN 0 THEN ReceiveAmount ELSE 0 END),0) UnCloseAmount,ISNULL(SUM(CASE ReceiveStatus WHEN 0 THEN 0 ELSE ReceiveAmount END),0) CloseAmount FROM AgentReceive " +
                     " WHERE forCompanyID = @CompanyID " +
                     " AND Currency = @CurrencyType ";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = " SELECT ISNULL(SUM(CASE ReceiveStatus WHEN 0 THEN ReceiveAmount ELSE 0 END),0) UnCloseAmount,ISNULL(SUM(CASE ReceiveStatus WHEN 0 THEN 0 ELSE ReceiveAmount END),0) CloseAmount FROM AgentReceive " +
                     " WHERE forCompanyID = @CompanyID " +
                     " AND Currency = @CurrencyType ";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Pay.CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue.CloseAmount = (decimal)DT.Rows[0]["CloseAmount"];
                returnValue.UnCloseAmount = (decimal)DT.Rows[0]["UnCloseAmount"];
            }
        }

        return returnValue;
    }

    public int SetAgentClose(int CompanyID, string CurrencyType)
    {
        int returnValue = -4;
        String SS = String.Empty;
        SqlCommand DBCmd;

        //--0 = success
        //--1 = 處理筆數為0
        //--2 = 鎖定失敗
        //--3 = 加扣點失敗
        //--4 = 其他錯誤

        SS = "spSetAgentClose";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        var Comapny= GetCompanyByCompanyID(CompanyID).First();
        var ServiceType = "";
        ServiceType = "PROXY";

        if (CurrencyType == "CNY") {
            ServiceType = "PROXY";
        } else {
            ServiceType = "PROXYJPY";
        }

        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);
        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    #endregion

    #region ProviderService
    public DBModel.ProviderService GetProviderServiceByProviderCodeAndServiceType(string ProviderCode, string ServiceType, string CurrencyType)
    {
        DBModel.ProviderService returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT * FROM ProviderService WITH (NOLOCK) " +
             " WHERE ProviderCode =@ProviderCode And ServiceType=@ServiceType And CurrencyType=@CurrencyType ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProviderService>(DT).First();
            }
        }

        return returnValue;
    }

    public DBModel.ProviderService GetProxyProviderByGroupID(string ProviderCode, string ServiceType, string CurrencyType, int GroupID)
    {
        DBModel.ProviderService returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT ProxyProviderGroup.PaymentRate as CostRate,ProxyProviderGroup.WithdrawalCharge as CostCharge FROM ProviderService WITH (NOLOCK) " +
             " JOIN ProxyProviderGroup ON ProviderService.ProviderCode = ProxyProviderGroup.forProviderCode " +
             " WHERE ProviderCode =@ProviderCode And ServiceType=@ServiceType And CurrencyType=@CurrencyType AND GroupID = @GroupID";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProviderService>(DT).First();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderServiceVM> GetProviderServiceResultByProviderCode(string ProviderCode)
    {
        List<DBViewModel.ProviderServiceVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT * FROM ProviderService WITH (NOLOCK) " +
             " JOIN ServiceType WITH (NOLOCK) ON ProviderService.ServiceType = ServiceType.ServiceType " +
             " WHERE ProviderCode =@ProviderCode";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderServiceVM>(DT) as List<DBViewModel.ProviderServiceVM>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderServiceVM> GetProviderServiceResult(string ProviderCode = "", string ServiceType = "", string CurrencyType = "")
    {
        List<DBViewModel.ProviderServiceVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (ProviderCode != "")
        {
            SS = "SELECT * FROM ProviderService WHERE ProviderCode =@ProviderCode AND ServiceType = @ServiceType AND CurrencyType =@CurrencyType";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            if (ServiceType != "" && CurrencyType != "")
            {
                SS = "SELECT * FROM ProviderService WHERE ServiceType = @ServiceType AND CurrencyType =@CurrencyType";
                DBCmd = new SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = System.Data.CommandType.Text;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
                DT = DBAccess.GetDB(DBConnStr, DBCmd);
            }
            else
            {
                SS = "SELECT * FROM ProviderService WITH (NOLOCK)";
                DBCmd = new SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = System.Data.CommandType.Text;
                DT = DBAccess.GetDB(DBConnStr, DBCmd);
            }
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderServiceVM>(DT) as List<DBViewModel.ProviderServiceVM>;
            }
        }

        return returnValue;
    }

    public int InsertProviderService(DBModel.ProviderService Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO ProviderService (ProviderCode,ServiceType,CurrencyType,CostRate,CostCharge,MaxOnceAmount,MinOnceAmount,MaxDaliyAmount,CheckoutType,DeviceType,State,Description) " +
         "   VALUES (@ProviderCode,@ServiceType,@CurrencyType,@CostRate,@CostCharge,@MaxOnceAmount,@MinOnceAmount,@MaxDaliyAmount,@CheckoutType,@DeviceType,@State,@Description)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@CostRate", SqlDbType.Decimal).Value = Model.CostRate;
        DBCmd.Parameters.Add("@CostCharge", SqlDbType.Decimal).Value = Model.CostCharge;
        DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = Model.MaxOnceAmount;
        DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = Model.MinOnceAmount;
        DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = Model.MaxDaliyAmount;
        DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = Model.CheckoutType;
        DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = Model.DeviceType;
        DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = Model.State;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderService.UpdateProviderService(Model.ProviderCode, Model.ServiceType, Model.CurrencyType);
        return returnValue;
    }

    public int UpdateProviderService(DBModel.ProviderService Model, string RealName, int CompanyID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderService SET Description=@Description,CostRate=@CostRate,CostCharge=@CostCharge,MaxOnceAmount=@MaxOnceAmount,MinOnceAmount=@MinOnceAmount,MaxDaliyAmount=@MaxDaliyAmount,CheckoutType=@CheckoutType,DeviceType=@DeviceType,State=@State WHERE ProviderCode=@ProviderCode AND ServiceType=@ServiceType AND CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@CostRate", SqlDbType.Decimal).Value = Model.CostRate;
        DBCmd.Parameters.Add("@CostCharge", SqlDbType.Decimal).Value = Model.CostCharge;
        DBCmd.Parameters.Add("@MaxOnceAmount", SqlDbType.Decimal).Value = Model.MaxOnceAmount;
        DBCmd.Parameters.Add("@MinOnceAmount", SqlDbType.Decimal).Value = Model.MinOnceAmount;
        DBCmd.Parameters.Add("@MaxDaliyAmount", SqlDbType.Decimal).Value = Model.MaxDaliyAmount;
        DBCmd.Parameters.Add("@CheckoutType", SqlDbType.Int).Value = Model.CheckoutType;
        DBCmd.Parameters.Add("@DeviceType", SqlDbType.Int).Value = Model.DeviceType;
        DBCmd.Parameters.Add("@State", SqlDbType.Int).Value = Model.State;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderService.UpdateProviderService(Model.ProviderCode, Model.ServiceType, Model.CurrencyType);

        return returnValue;
    }

    public int DisableProvider(string ProviderCode)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderCode " +
             " SET ProviderState = (CASE ProviderState WHEN 1 THEN 0 ELSE 1 END)" +
             " WHERE ProviderCode=@ProviderCode ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderCode.UpdateProviderCode(ProviderCode);

        return returnValue;
    }

    public int DisableProviderService(DBModel.ProviderService Model, string RealName, int CompanyID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderService " +
             " SET State = (CASE State WHEN 1 THEN 0 ELSE 1 END)" +
             " WHERE ProviderCode=@ProviderCode AND ServiceType=@ServiceType AND CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderService.UpdateProviderService(Model.ProviderCode, Model.ServiceType, Model.CurrencyType);

        //CheckGPayRelationByUpdateProviderServiceState(Model.ServiceType, Model.CurrencyType, RealName, Model.ProviderCode, CompanyID);

        return returnValue;
    }

    public int ChangeProviderServiceState(DBModel.ProviderService Model, string RealName, int CompanyID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderService SET State=(IIF(State=0,1,0)) WHERE ProviderCode=@ProviderCode AND ServiceType=@ServiceType AND CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.ProviderService.UpdateProviderService(Model.ProviderCode, Model.ServiceType, Model.CurrencyType);

        //CheckGPayRelationByUpdateProviderServiceState(Model.ServiceType, Model.CurrencyType, RealName, Model.ProviderCode, CompanyID);

        return returnValue;
    }

    public int GetProviderServiceState(string ServiceType, string CurrencyType, string ProviderCode)
    {

        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "SELECT State FROM ProviderService WITH (NOLOCK) WHERE ProviderCode =@ProviderCode AND ServiceType = @ServiceType AND CurrencyType =@CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());
        return returnValue;
    }
    #endregion

    #region PermissionCategory
    public List<DBModel.PermissionCategory> GetPermissionCategoryResult(int PermissionCategoryID = 0)
    {
        List<DBModel.PermissionCategory> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (PermissionCategoryID != 0)
        {
            SS = "SELECT * FROM PermissionCategory WHERE PermissionCategoryID =@PermissionCategoryID";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@PermissionCategoryID", SqlDbType.Int).Value = PermissionCategoryID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = "SELECT * FROM PermissionCategory WITH (NOLOCK) order by SortIndex";
            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PermissionCategory>(DT) as List<DBModel.PermissionCategory>;
            }
        }

        return returnValue;
    }

    public int InsertPermissionCategory(DBModel.PermissionCategory Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO PermissionCategory (PermissionCategoryName, PageType, SortIndex, Description) " +
         "   VALUES (@PermissionCategoryName, @PageType, @SortIndex, @Description)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionCategoryName", SqlDbType.VarChar).Value = Model.PermissionCategoryName;
        DBCmd.Parameters.Add("@PageType", SqlDbType.Int).Value = Model.PageType;
        DBCmd.Parameters.Add("@SortIndex", SqlDbType.Int).Value = Model.SortIndex;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    public int UpdatePermissionCategory(DBModel.PermissionCategory Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE PermissionCategory SET PermissionCategoryName=@PermissionCategoryName,PageType=@PageType,SortIndex=@SortIndex,Description=@Description WHERE PermissionCategoryID=@PermissionCategoryID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionCategoryID", SqlDbType.Int).Value = Model.PermissionCategoryID;
        DBCmd.Parameters.Add("@PageType", SqlDbType.Int).Value = Model.PageType;
        DBCmd.Parameters.Add("@SortIndex", SqlDbType.Int).Value = Model.SortIndex;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@PermissionCategoryName", SqlDbType.NVarChar).Value = Model.PermissionCategoryName;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    public int DeletePermissionCategory(DBModel.PermissionCategory Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "DELETE FROM PermissionCategory WHERE  PermissionCategoryID =@PermissionCategoryID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionCategoryID", SqlDbType.Int).Value = Model.PermissionCategoryID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }
    #endregion

    #region Permission

    public bool CheckLoginPermission(int CompanyID)
    {

        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT CompanyID FROM CompanyTable WITH (NOLOCK)" +
            " WHERE CompanyID =@CompanyID And (CompanyType=0 OR CompanyType=4)";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT.Rows.Count > 0)
        {
            returnValue = true;
        }
        return returnValue;
    }

    public bool CheckPermission(string PermissionName, int AdminID)
    {

        bool returnValue = false;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT Count(*) FROM AdminRolePermission WITH (NOLOCK)" +
            " JOIN AdminRole ON AdminRole.AdminRoleID=AdminRolePermission.forAdminRoleID " +
            " JOIN AdminTable ON AdminTable.forAdminRoleID=AdminRole.AdminRoleID " +
            " WHERE AdminTable.AdminID=@AdminID And forPermissionName=@PermissionName ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@PermissionName", SqlDbType.VarChar).Value = PermissionName;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT.Rows.Count > 0)
        {
            returnValue = true;
        }
        return returnValue;
    }

    public int InsertPermission(FromBody.PermissionSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = "SELECT COUNT(*) FROM PermissionTable WITH (NOLOCK) WHERE PermissionName=@PermissionName";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionName", SqlDbType.VarChar).Value = fromBody.PermissionName;

        //登入帳號重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = "Insert Into PermissionTable(PermissionName, Description, LinkURL, AdminPermission,PermissionCategoryID,SortIndex) " +
                               " Values(@PermissionName, @Description, @LinkURL, @AdminPermission,@PermissionCategoryID,@SortIndex)";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminPermission", SqlDbType.Int).Value = fromBody.AdminPermission;
        DBCmd.Parameters.Add("@PermissionCategoryID", SqlDbType.Int).Value = fromBody.PermissionCategoryID;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
        DBCmd.Parameters.Add("@LinkURL", SqlDbType.VarChar).Value = fromBody.LinkURL;
        DBCmd.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = fromBody.PermissionName;
        DBCmd.Parameters.Add("@SortIndex", SqlDbType.Int).Value = fromBody.SortIndex;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdatePermission(FromBody.PermissionSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE PermissionTable";
        SS += " SET Description=@Description,AdminPermission=@AdminPermission,LinkURL=@LinkURL,PermissionCategoryID=@PermissionCategoryID,SortIndex=@SortIndex";
        SS += " WHERE PermissionName=@PermissionName";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@AdminPermission", SqlDbType.Int).Value = fromBody.AdminPermission;
        DBCmd.Parameters.Add("@PermissionCategoryID", SqlDbType.Int).Value = fromBody.PermissionCategoryID;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
        DBCmd.Parameters.Add("@LinkURL", SqlDbType.VarChar).Value = fromBody.LinkURL;
        DBCmd.Parameters.Add("@PermissionName", SqlDbType.VarChar).Value = fromBody.PermissionName;
        DBCmd.Parameters.Add("@SortIndex", SqlDbType.Int).Value = fromBody.SortIndex;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int DeletePermission(FromBody.PermissionSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = "Delete PermissionTable WHERE PermissionName=@PermissionName";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionName", SqlDbType.VarChar).Value = fromBody.PermissionName;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        SS = "Delete AdminRolePermission Where forPermissionName=@forPermissionName";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = fromBody.PermissionName;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public List<DBViewModel.AdminRolePermissionResult> GetAdminRolePermissionResultByPermissionName(string PermissionName)
    {
        List<DBViewModel.AdminRolePermissionResult> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable selectedAdminRoleDT;
        DataTable allAdminRoleDT;

        SS = " SELECT AdminRole.*,CompanyName FROM AdminRole WITH (NOLOCK) " +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON AdminRole.forCompanyID=CompanyTable.CompanyID ORDER BY forCompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        allAdminRoleDT = DBAccess.GetDB(DBConnStr, DBCmd);


        SS = "SELECT * FROM AdminRolePermission WITH (NOLOCK) " +
             " WHERE forPermissionName=@forPermissionName";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forPermissionName", SqlDbType.NVarChar).Value = PermissionName;
        selectedAdminRoleDT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (allAdminRoleDT != null)
        {
            if (allAdminRoleDT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminRolePermissionResult>(allAdminRoleDT).ToList();
                foreach (var rowData in returnValue)
                {
                    if (selectedAdminRoleDT.Select("forAdminRoleID='" + rowData.AdminRoleID + "'").Count() > 0)
                    {
                        rowData.selectedAdminRole = true;
                    }
                    else
                    {
                        rowData.selectedAdminRole = false;
                    }
                }
            }
        }
        return returnValue;
    }

    public int UpdatePermissionRole(string PermissionName, List<string> PermissionRoles)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        int CompanyID;
        int AdminRoleID;
        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {
            SS = " DELETE FROM AdminRolePermission" +
                 " WHERE forPermissionName=@forPermissionName";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forPermissionName", SqlDbType.NVarChar).Value = PermissionName;

            T.ExecuteDB(DBCmd);
            foreach (var permissionrole in PermissionRoles)
            {

                AdminRoleID = Convert.ToInt32(permissionrole.Split('_')[0]);
                CompanyID = Convert.ToInt32(permissionrole.Split('_')[1]);
                SS = " INSERT INTO AdminRolePermission (forAdminRoleID,forCompanyID,forPermissionName)" +
                               " VALUES (@forAdminRoleID,@forCompanyID,@forPermissionName)";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
                DBCmd.Parameters.Add("@forAdminRoleID", SqlDbType.Int).Value = AdminRoleID;
                DBCmd.Parameters.Add("@forPermissionName", SqlDbType.VarChar).Value = PermissionName;
                T.ExecuteDB(DBCmd);
            }

        });



        return returnValue;

    }

    public List<DBModel.Permission> GetPermissionTable(bool AdminPermission)
    {
        List<DBModel.Permission> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        if (AdminPermission)
        {
            SS = "SELECT * FROM PermissionTable WITH (NOLOCK) ";
        }
        else
        {
            SS = "SELECT * FROM PermissionTable WITH (NOLOCK) WHERE AdminPermission=0";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Permission>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Permission> GetPermissionTableByPermissionCategoryID(int PermissionCategoryID)
    {
        List<DBModel.Permission> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        //PermissionCategoryID==0 取得全部資料
        if (PermissionCategoryID == 0)
        {
            SS = "SELECT PermissionTable.*,PermissionCategory.Description AS PermissionCategoryDescription FROM PermissionTable WITH (NOLOCK) " +
                  " JOIN PermissionCategory WITH (NOLOCK) ON PermissionCategory.PermissionCategoryID=PermissionTable.PermissionCategoryID " +
                  " Order By PermissionCategory.SortIndex,PermissionTable.SortIndex ";
        }
        else
        {
            SS = "SELECT PermissionTable.*,PermissionCategory.Description AS PermissionCategoryDescription FROM PermissionTable WITH (NOLOCK)" +
                " JOIN PermissionCategory WITH (NOLOCK) ON PermissionCategory.PermissionCategoryID=PermissionTable.PermissionCategoryID" +
                " WHERE PermissionTable.PermissionCategoryID=@PermissionCategoryID" +
                " Order By PermissionCategory.SortIndex,PermissionTable.SortIndex ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PermissionCategoryID", System.Data.SqlDbType.Int).Value = PermissionCategoryID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Permission>(DT).ToList();
            }
        }

        return returnValue;
    }

    #endregion  

    #region ProviderPoint
    public List<DBViewModel.ProviderPointVM> GetAllProviderPointByCompanyID(int CompanyID)
    {
        List<DBViewModel.ProviderPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;


        SS = " SELECT PC.ProviderName,PC.ProviderAPIType,PC.ProviderCode, ISNULL(PP.TotalDepositePointValue,0) TotalDepositePointValue,ISNULL(PP.TotalProfitPointValue,0) TotalProfitPointValue, " +
             " (ISNULL(PP.SystemPointValue, 0) - ISNULL(FP.ProviderFrozenAmount, 0)) AS SystemPointValue, ISNULL(FP.ProviderFrozenAmount, 0) ProviderFrozenAmount, ISNULL(FMH.WProfit,0) WithdrawProfit, " +
             " (SELECT  ISNULL(SUM(W.Amount + W.CostCharge), 0) FROM Withdrawal W WITH (NOLOCK) " +
             " WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91 " +
             " AND W.ProviderCode = PP.ProviderCode AND W.CurrencyType = PP.CurrencyType) AS WithdrawPoint " +
             " FROM ProviderCode PC " +
             " LEFT JOIN ProviderPoint PP ON PC.ProviderCode = PP.ProviderCode" +
             " LEFT JOIN(SELECT forProviderCode, SUM(ISNULL(ProviderFrozenAmount, 0)) ProviderFrozenAmount FROM FrozenPoint FP " +
             " WHERE  FP.Status = 0 " +
             " GROUP BY forProviderCode) FP ON FP.forProviderCode = PC.ProviderCode " +
             " LEFT JOIN (SELECT ProviderCode,SUM(ISNULL(Amount, 0)) WProfit FROM ProviderManualHistory FMH " +
             " WHERE FMH.Type = 2 " +
             " GROUP BY ProviderCode) FMH ON FMH.ProviderCode = PC.ProviderCode "+
             " WHERE PC.ProviderState = 0 And PC.forCompanyID=@CompanyID ";
   
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderPointVM> GetAllProviderPoint(bool SearchAllProvider = false)
    {
        List<DBViewModel.ProviderPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;


        SS = " SELECT PP.PrePaidPoint,PC.ProviderName,PC.ProviderAPIType,PC.ProviderCode, ISNULL(PP.TotalDepositePointValue,0) TotalDepositePointValue,ISNULL(PP.TotalProfitPointValue,0) TotalProfitPointValue, " +
             " (ISNULL(PP.SystemPointValue, 0) - ISNULL(FP.ProviderFrozenAmount, 0)) AS SystemPointValue, ISNULL(FP.ProviderFrozenAmount, 0) ProviderFrozenAmount, ISNULL(FMH.WProfit,0) WithdrawProfit, " +
             " (SELECT  ISNULL(SUM(W.Amount + W.CostCharge), 0) FROM Withdrawal W WITH (NOLOCK) " +
             " WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91 " +
             " AND W.ProviderCode = PP.ProviderCode AND W.CurrencyType = PP.CurrencyType) AS WithdrawPoint " +
             " FROM ProviderCode PC " +
             " LEFT JOIN ProviderPoint PP ON PC.ProviderCode = PP.ProviderCode" +
             " LEFT JOIN(SELECT forProviderCode, SUM(ISNULL(ProviderFrozenAmount, 0)) ProviderFrozenAmount FROM FrozenPoint FP " +
             " WHERE  FP.Status = 0 " +
             " GROUP BY forProviderCode) FP ON FP.forProviderCode = PC.ProviderCode " +
             " LEFT JOIN (SELECT ProviderCode,SUM(ISNULL(Amount, 0)) WProfit FROM ProviderManualHistory FMH " +
             " WHERE FMH.Type = 2 " +
             " GROUP BY ProviderCode) FMH ON FMH.ProviderCode = PC.ProviderCode ";

        if (!SearchAllProvider)
        {
            SS += " WHERE PC.ProviderState = 0";
        }

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderPointVM> GetAllProviderPoint2(bool SearchAllProvider = false)
    {
        List<DBViewModel.ProviderPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;


        SS = " SELECT PC.ProviderName,PC.ProviderAPIType,PC.ProviderCode, ISNULL(PP.TotalDepositePointValue,0) TotalDepositePointValue,ISNULL(PP.TotalProfitPointValue,0) TotalProfitPointValue, " +
             " (ISNULL(PP.SystemPointValue, 0) - ISNULL(FP.ProviderFrozenAmount, 0)) AS SystemPointValue, ISNULL(FP.ProviderFrozenAmount, 0) ProviderFrozenAmount, ISNULL(FMH.WProfit,0) WithdrawProfit, " +
             " (SELECT  ISNULL(SUM(W.Amount + W.CostCharge), 0) FROM Withdrawal W WITH (NOLOCK) " +
             " WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91 " +
             " AND W.ProviderCode = PP.ProviderCode AND W.CurrencyType = PP.CurrencyType) AS WithdrawPoint " +
             " FROM ProviderCode PC " +
             " LEFT JOIN ProviderPoint PP WITH (NOLOCK) ON PC.ProviderCode = PP.ProviderCode" +
             " LEFT JOIN(SELECT forProviderCode, SUM(ISNULL(ProviderFrozenAmount, 0)) ProviderFrozenAmount FROM FrozenPoint FP WITH (NOLOCK) " +
             " WHERE  FP.Status = 0 " +
             " GROUP BY forProviderCode) FP ON FP.forProviderCode = PC.ProviderCode " +
             " LEFT JOIN (SELECT ProviderCode,SUM(ISNULL(Amount, 0)) WProfit FROM ProviderManualHistory FMH WITH (NOLOCK) " +
             " WHERE FMH.Type = 2 " +
             " GROUP BY ProviderCode) FMH ON FMH.ProviderCode = PC.ProviderCode WHERE 1=1 And PC.forCompanyID=0";

        if (!SearchAllProvider)
        {
            SS += " And PC.ProviderState = 0";
        }


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointVM>(DT).ToList();
            }
        }

        return returnValue;
    }


    public List<DBViewModel.ProviderPointVM> GetAllProviderPointBC(string CurrencyType,string ServiceType, bool SearchAllProvider = false)
    {
        List<DBViewModel.ProviderPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;


        SS = " SELECT WL.BlockChainRate UpBlockChainRate,ISNULL(BCR.Rate,0) AS OKEXRate,PC.ProviderName,PC.ProviderAPIType,PC.ProviderCode, ISNULL(PP.TotalDepositePointValue,0) TotalDepositePointValue,ISNULL(PP.TotalProfitPointValue,0) TotalProfitPointValue, " +
             " (ISNULL(PP.SystemPointValue, 0) - ISNULL(FP.ProviderFrozenAmount, 0)) AS SystemPointValue, ISNULL(FP.ProviderFrozenAmount, 0) ProviderFrozenAmount, ISNULL(FMH.WProfit,0) WithdrawProfit, " +
             " (SELECT  ISNULL(SUM(W.Amount + W.CostCharge), 0) FROM Withdrawal W WITH (NOLOCK) " +
             " WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91 " +
             " AND W.ProviderCode = PP.ProviderCode AND W.CurrencyType = PP.CurrencyType) AS WithdrawPoint " +
             " FROM ProviderCode PC " +
             " JOIN WithdrawLimit WL ON WL.ProviderCode=PC.ProviderCode AND WL.ServiceType=@ServiceType AND WL.CurrencyType=@CurrencyType AND WL.WithdrawLimitType=3" +
             " LEFT JOIN ProviderPoint PP WITH (NOLOCK) ON PC.ProviderCode = PP.ProviderCode" +
             " JOIN BlockChainRate BCR WITH (NOLOCK) ON PP.CurrencyType = BCR.CurrencyType " +
             " LEFT JOIN(SELECT forProviderCode, SUM(ISNULL(ProviderFrozenAmount, 0)) ProviderFrozenAmount FROM FrozenPoint FP " +
             " WHERE  FP.Status = 0 " +
             " GROUP BY forProviderCode) FP ON FP.forProviderCode = PC.ProviderCode " +
             " LEFT JOIN (SELECT ProviderCode,SUM(ISNULL(Amount, 0)) WProfit FROM ProviderManualHistory FMH " +
             " WHERE FMH.Type = 2 " +
             " GROUP BY ProviderCode) FMH ON FMH.ProviderCode = PC.ProviderCode WHERE WL.BlockChainRate IS NOT NULL ";

        if (!SearchAllProvider)
        {
            SS += " And PC.ProviderState = 0";
        }


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBViewModel.ProviderPointVM GetProviderPointByProviderCode(string ProviderCode)
    {
        DBViewModel.ProviderPointVM returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT SystemPointValue " +
             " FROM ProviderPoint " +
             " WHERE ProviderCode=@ProviderCode ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointVM>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProviderManualHistory> GetProviderProfitManualHistoryResult()
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.ProviderManualHistory> returnValue = new List<DBModel.ProviderManualHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT PMH.*, " +
                 "        AT.LoginAccount,PC.ProviderName, " +
                 "        CONVERT(VARCHAR, PMH.CreateDate, 120) AS CreateDate2 " +
                 " FROM   ProviderManualHistory PMH " +
                 "        LEFT JOIN AdminTable AT " +
                 "               ON PMH.forAdminID = AT.AdminID " +
                  "        LEFT JOIN ProviderCode PC " +
                 "               ON PC.ProviderCode = PMH.ProviderCode " +
                 " WHERE  PMH.Type = 2 ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProviderManualHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public int UpdateProviderProfit(DBModel.ProviderManualHistory Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "UPDATE ProviderPoint SET TotalProfitPointValue=TotalProfitPointValue+@PointValue WHERE ProviderCode=@ProviderCode AND CurrencyType=@CurrencyType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@PointValue", SqlDbType.Decimal).Value = Model.Amount;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        return returnValue;
    }

    #endregion

    #region CompanyPoint
    public List<DBViewModel.CompanyServicePointVM> GetCanUseCompanyServicePoint(int CompanyID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;
        List<DBViewModel.CompanyServicePointVM> returnValue = null;

        SS = " SELECT(CSP.SystemPointValue - (SELECT ISNULL(SUM(W.Amount + W.CollectCharge), 0)" +
             " FROM Withdrawal W WITH (NOLOCK)  " +
             " WHERE W.Status <> 2 " +
             " AND W.Status <> 3 " +
             " AND W.Status <> 90 " +
             " AND W.Status <> 91 " +
             " AND W.forCompanyID = CSP.CompanyID " +
             " AND W.ServiceType = CSP.ServiceType " +
             " AND W.CurrencyType = CSP.CurrencyType)) AS CanUsePoint, " +
             " ISNULL((Select SUM(CompanyFrozenAmount) FROM " +
             " FrozenPoint where FrozenPoint.forCompanyID = CSP.CompanyID " +
             " AND FrozenPoint.CurrencyType = CSP.CurrencyType " +
             " AND FrozenPoint.ServiceType = CSP.ServiceType " +
             " AND FrozenPoint.Status = 0),0) AS FrozenPoint," +
             " ServiceTypeName,WithdrawLimit.MaxLimit,WithdrawLimit.MinLimit,WithdrawLimit.Charge,WithdrawLimit.Rate, " +
             " CSP.*" +
             " FROM CompanyServicePoint AS CSP" +
             " LEFT JOIN ServiceType ON CSP.ServiceType=ServiceType.ServiceType And CSP.CurrencyType=ServiceType.CurrencyType " +
             " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CSP.ServiceType And WithdrawLimit.CurrencyType= CSP.CurrencyType And WithdrawLimit.WithdrawLimitType=1 And CSP.CompanyID=WithdrawLimit.forCompanyID" +
             " WHERE CSP.CompanyID = @CompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBViewModel.CompanyServicePointVM GetCanUseCompanyServicePointByService(int CompanyID, string ServiceType,string CurrencyType)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;
        DBViewModel.CompanyServicePointVM returnValue = null;

        SS = " SELECT(CSP.SystemPointValue - (SELECT ISNULL(SUM(W.Amount + W.CollectCharge), 0)" +
             " FROM Withdrawal W WITH (NOLOCK) " +
             " WHERE W.Status <> 2 " +
             " AND W.Status <> 3 " +
             " AND W.Status <> 90 " +
             " AND W.Status <> 91 " +
             " AND W.forCompanyID = CSP.CompanyID " +
             " AND W.ServiceType = CSP.ServiceType " +
             " AND W.CurrencyType = CSP.CurrencyType)) AS CanUsePoint, " +
             " ISNULL((Select SUM(CompanyFrozenAmount) FROM " +
             " FrozenPoint where FrozenPoint.forCompanyID = CSP.CompanyID " +
             " AND FrozenPoint.CurrencyType = CSP.CurrencyType " +
             " AND FrozenPoint.ServiceType = CSP.ServiceType " +
             " AND FrozenPoint.Status = 0),0) AS FrozenPoint," +
             " ServiceTypeName,WithdrawLimit.MaxLimit,WithdrawLimit.MinLimit,WithdrawLimit.Charge,WithdrawLimit.Rate, " +
             " CSP.*" +
             " FROM CompanyServicePoint AS CSP" +
             " LEFT JOIN ServiceType ON CSP.ServiceType=ServiceType.ServiceType" +
             " LEFT JOIN BlockChainRate ON CSP.CurrencyType=BlockChainRate.CurrencyType " +
             " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CSP.ServiceType And WithdrawLimit.CurrencyType= CSP.CurrencyType And WithdrawLimit.WithdrawLimitType=1 And CSP.CompanyID=WithdrawLimit.forCompanyID" +
             " WHERE CSP.CompanyID = @CompanyID " +
             " AND CSP.CurrencyType = @CurrencyType " +
             " AND CSP.ServiceType = @ServiceType ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyServicePointVM> GetCompanyServicePointByServiceType(int CompanyID, string ServiceType, string CurrencyType)
    {
        List<DBViewModel.CompanyServicePointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT *,ServiceTypeName" +
             " FROM  CompanyServicePoint" +
             " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
              " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CompanyServicePoint.ServiceType And WithdrawLimit.CurrencyType= CompanyServicePoint.CurrencyType And WithdrawLimit.WithdrawLimitType=1 And CompanyServicePoint.CompanyID=WithdrawLimit.forCompanyID" +
             " WHERE CompanyServicePoint.CompanyID = @CompanyID" +
             " AND CompanyServicePoint.CurrencyType = @CurrencyType" +
             " AND CompanyServicePoint.ServiceType = @ServiceType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyServicePointVM> GetCompanyServicePointByServiceTypeAPI(int CompanyID, string ServiceType, string CurrencyType)
    {
        List<DBViewModel.CompanyServicePointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT *,ServiceTypeName" +
             " FROM  CompanyServicePoint" +
             " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
              " LEFT JOIN WithdrawLimit ON  WithdrawLimit.CurrencyType= CompanyServicePoint.CurrencyType And WithdrawLimit.WithdrawLimitType=2 And CompanyServicePoint.CompanyID=WithdrawLimit.forCompanyID" +
             " WHERE CompanyServicePoint.CompanyID = @CompanyID" +
             " AND CompanyServicePoint.CurrencyType = @CurrencyType" +
             " AND CompanyServicePoint.ServiceType = @ServiceType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderPointHistory> GetProviderPointHistory(FromBody.GetProviderPointHistorySet fromBody)
    {
        List<DBViewModel.ProviderPointHistory> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " select ProviderName,CH.*,convert(varchar,CreateDate,120) as CreateDate2, " +
             " Case when CH.OperatorType = 0" +
             " then (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
             " when CH.OperatorType = 1" +
             " then (select Withdrawal.WithdrawSerial from Withdrawal WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID )" +
             " when CH.OperatorType = 3" +
             " then (select ProviderManualHistory.TransactionSerial from ProviderManualHistory where ProviderManualHistory.ProviderManualID = CH.TransactionID )" +
             " else (select Withdrawal.WithdrawSerial from Withdrawal  WITH (NOLOCK) where Withdrawal.WithdrawID = CH.TransactionID )" +
             " End as TransactionOrder" +
             " from ProviderPointHistory CH" +
             " left join ProviderCode  on ProviderCode.ProviderCode = CH.ProviderCode" +
             " WHERE CH.CreateDate Between @StartDate And @EndDate ";

        if (fromBody.OperatorType != 99)
        {
            SS += " And CH.OperatorType=@OperatorType";
        }

        if (fromBody.ProviderCode != "99")
        {
            SS += " And CH.ProviderCode=@ProviderCode";
        }

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = fromBody.OperatorType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderPointHistory> GetProviderPrePaidHistory(FromBody.GetProviderPointHistorySet fromBody)
    {
        List<DBViewModel.ProviderPointHistory> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " select ProviderName,CH.*,convert(varchar,CreateDate,120) as CreateDate2, " +
             " Case when (CH.OperatorType = 0 OR CH.OperatorType = 2) " +
             " then (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
             " else (select '0')" +
             " End as TransactionOrder" +
             " from ProviderPrePaidHistory CH" +
             " left join ProviderCode  on ProviderCode.ProviderCode = CH.ProviderCode" +
             " WHERE CH.CreateDate Between @StartDate And @EndDate ";

        if (fromBody.OperatorType != 99)
        {
            SS += " And CH.OperatorType=@OperatorType";
        }

        if (fromBody.ProviderCode != "99")
        {
            SS += " And CH.ProviderCode=@ProviderCode";
        }

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = fromBody.OperatorType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProxyProviderPointHistory> GetProxyProviderPointHistory(FromBody.GetProviderPointHistorySet fromBody)
    {
        List<DBViewModel.ProxyProviderPointHistory> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " select GroupName,CH.*,convert(varchar,CH.CreateDate,120) as CreateDate2, " +
             " Case when CH.OperatorType = 0" +
             " then (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
             " when CH.OperatorType = 1" +
             " then (select Withdrawal.WithdrawSerial from Withdrawal WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID )" +
             " when CH.OperatorType = 2" +
             " then (select Withdrawal.WithdrawSerial from Withdrawal WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID ) " +
             " else (select Withdrawal.WithdrawSerial from Withdrawal WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID )" +
             " End as TransactionOrder" +
             " from ProxyProviderPointHistory CH" +
             " LEFT JOIN ProxyProviderGroup PPG WITH (NOLOCK) ON PPG.GroupID=CH.GroupID " +
             " WHERE CH.CreateDate Between @StartDate And @EndDate And ProviderCode=@ProviderCode ";

        if (fromBody.OperatorType != 99)
        {
            SS += " And CH.OperatorType=@OperatorType";
        }

        if (fromBody.GroupID != 0)
        {
            SS += " And CH.GroupID=@GroupID";
        }



        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = fromBody.OperatorType;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProxyProviderPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyServicePointVM> GetAllCompanyServicePoint()
    {
        List<DBViewModel.CompanyServicePointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT *,ServiceTypeName,CompanyName, " +
             " (SELECT ISNULL(SUM(W.Amount + W.CollectCharge), 0)" +
             " FROM Withdrawal W  WITH (NOLOCK) " +
             " WHERE W.Status <> 2" +
             " AND W.Status <> 3" +
             " AND W.Status <> 8" +
             " AND W.Status <> 90" +
             " AND W.Status <> 91" +
             " AND W.forCompanyID = CompanyServicePoint.CompanyID" +
             " AND W.ServiceType = CompanyServicePoint.ServiceType" +
             " AND W.CurrencyType = CompanyServicePoint.CurrencyType) AS WithdrawalPoint," +
             " ISNULL((Select SUM(CompanyFrozenAmount) FROM" +
             " FrozenPoint where FrozenPoint.forCompanyID = CompanyServicePoint.CompanyID" +
             " AND FrozenPoint.CurrencyType = CompanyServicePoint.CurrencyType" +
             " AND FrozenPoint.ServiceType = CompanyServicePoint.ServiceType" +
             " AND FrozenPoint.Status = 0),0) AS FrozenPoint" +
             " FROM  CompanyServicePoint" +
             " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID=CompanyServicePoint.CompanyID" +
             " WHERE CompanyServicePoint.CurrencyType = @CurrencyType AND CompanyTable.CompanyState=0 " +
             " Order By CompanyServicePoint.CompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Pay.CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyServicePointVM> GetCompanyServicePointDetail(int CompanyID, string CurrencyType = "")
    {
        List<DBViewModel.CompanyServicePointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        if (string.IsNullOrEmpty(CurrencyType))
        {
            SS = " SELECT CompanyServicePoint.*,CompanyService.State,ServiceTypeName,WithdrawLimit.MaxLimit,WithdrawLimit.MinLimit,WithdrawLimit.Charge" +
            " FROM  CompanyServicePoint" +
            " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
            " LEFT JOIN CompanyService ON CompanyService.ServiceType=CompanyServicePoint.ServiceType And CompanyService.CurrencyType=CompanyServicePoint.CurrencyType And CompanyService.forCompanyID=CompanyServicePoint.CompanyID " +
            " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CompanyServicePoint.ServiceType And WithdrawLimit.CurrencyType=@CurrencyType And WithdrawLimit.WithdrawLimitType=1 And WithdrawLimit.forCompanyID=@CompanyID" +
            " WHERE CompanyServicePoint.CompanyID = @CompanyID";
        }
        else {
            SS = " SELECT CompanyServicePoint.*,CompanyService.State,ServiceTypeName,WithdrawLimit.MaxLimit,WithdrawLimit.MinLimit,WithdrawLimit.Charge" +
                " FROM  CompanyServicePoint" +
                " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
                " LEFT JOIN CompanyService ON CompanyService.ServiceType=CompanyServicePoint.ServiceType And CompanyService.CurrencyType=CompanyServicePoint.CurrencyType And CompanyService.forCompanyID=CompanyServicePoint.CompanyID " +
                " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CompanyServicePoint.ServiceType And WithdrawLimit.CurrencyType=@CurrencyType And WithdrawLimit.WithdrawLimitType=1 And WithdrawLimit.forCompanyID=@CompanyID" +
                " WHERE CompanyServicePoint.CompanyID = @CompanyID" +
                " AND CompanyServicePoint.CurrencyType = @CurrencyType";
        }
       

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyServicePointVM> GetCompanyServicePointDetail2(int CompanyID, string CurrencyType = "")
    {
        List<DBViewModel.CompanyServicePointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;    
        DataTable DT;

        SS = " SELECT " +
             " (SELECT COUNT(FP.FrozenID) FROM FrozenPoint FP WHERE FP.ServiceType = CompanyServicePoint.ServiceType And FP.forCompanyID = @CompanyID AND FP.CurrencyType =@CurrencyType And FP.Status=0) FrozenServiceCount," +
             " ISNULL((SELECT SUM(FP.CompanyFrozenAmount) FROM FrozenPoint FP WHERE FP.ServiceType = CompanyServicePoint.ServiceType And FP.forCompanyID = @CompanyID AND FP.CurrencyType =@CurrencyType  And FP.Status=0),0) FrozenServicePoint" +
             ",CompanyServicePoint.*,ServiceTypeName,WithdrawLimit.MaxLimit,WithdrawLimit.MinLimit,WithdrawLimit.Charge" +
             " FROM  CompanyServicePoint" +
             " LEFT JOIN ServiceType ON CompanyServicePoint.ServiceType=ServiceType.ServiceType" +
             " LEFT JOIN WithdrawLimit ON WithdrawLimit.ServiceType=CompanyServicePoint.ServiceType And WithdrawLimit.CurrencyType=@CurrencyType And WithdrawLimit.WithdrawLimitType=1 And WithdrawLimit.forCompanyID=@CompanyID" +
             " WHERE CompanyServicePoint.CompanyID = @CompanyID" +
             " AND CompanyServicePoint.CurrencyType = @CurrencyType";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);


        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyServicePointVM>(DT).ToList();
            }
        }

        return returnValue;
    }
    public List<DBViewModel.CompanyPointVM> GetCompanyPointTableResult(int CompanyID, string CurrencyType = "")
    {
        List<DBViewModel.CompanyPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CurrencyType != "")
        {
            SS = " SELECT CompanyTable.CompanyName,CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
                 " FROM   CompanyPoint" +
                 " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
                 " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyPoint.forCompanyID = CompanyTable.CompanyID" +
                 " WHERE CompanyPoint.forCompanyID = @CompanyID" +
                 " AND CompanyPoint.CurrencyType = @CurrencyType";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {
            SS = " SELECT CompanyTable.CompanyName,CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
                 " FROM   CompanyPoint" +
                 " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
                 " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyPoint.forCompanyID = CompanyTable.CompanyID" +
                 " WHERE CompanyPoint.forCompanyID = @CompanyID";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyPointVM>(DT) as List<DBViewModel.CompanyPointVM>;
                foreach (var data in returnValue)
                {
                    //檢查錢包金額是否足夠
                    var CanUseCompanyPointDT = GetCanUseCompanyPoint(CompanyID, data.CurrencyType);

                    ////錢包檢查
                    if (CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0)
                    {
                        var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).First();
                        data.LockPointValue = CompanyPointModel.PointValue - CompanyPointModel.CanUsePoint;
                        data.FrozenPoint = CompanyPointModel.FrozenPoint;
                    }
                }
            }
        }

        return returnValue;
    }

    //public List<DBViewModel.CompanyPointVM> GetCompanyPointTableResultByFilter(int CompanyID, string ProviderCode)
    //{
    //    List<DBViewModel.CompanyPointVM> returnValue = null;
    //    string SS;
    //    SqlCommand DBCmd = null;
    //    DataTable DT;

    //    if (CurrencyType != "")
    //    {
    //        SS = " SELECT CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
    //             " FROM   CompanyPoint" +
    //             " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
    //             " WHERE CompanyPoint.forCompanyID = @CompanyID" +
    //             " AND CompanyPoint.CurrencyType = @CurrencyType";

    //        DBCmd = new SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = System.Data.CommandType.Text;
    //        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
    //        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
    //        DT = DBAccess.GetDB(DBConnStr, DBCmd);
    //    }
    //    else
    //    {
    //        SS = " SELECT CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
    //             " FROM   CompanyPoint" +
    //             " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
    //             " WHERE CompanyPoint.forCompanyID = @CompanyID";

    //        DBCmd = new SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = System.Data.CommandType.Text;
    //        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
    //        DT = DBAccess.GetDB(DBConnStr, DBCmd);
    //    }

    //    if (DT != null)
    //    {
    //        if (DT.Rows.Count > 0)
    //        {
    //            returnValue = DataTableExtensions.ToList<DBViewModel.CompanyPointVM>(DT) as List<DBViewModel.CompanyPointVM>;
    //            foreach (var data in returnValue)
    //            {
    //                //檢查錢包金額是否足夠
    //                var CanUseCompanyPointDT = GetCanUseCompanyPoint(CompanyID, data.CurrencyType);

    //                ////錢包檢查
    //                if (CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0)
    //                {
    //                    var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).First();
    //                    data.LockPointValue = CompanyPointModel.PointValue - CompanyPointModel.CanUsePoint;
    //                    data.FrozenPoint = CompanyPointModel.FrozenPoint;
    //                }
    //            }
    //        }
    //    }

    //    return returnValue;
    //}

    public List<DBViewModel.CompanyPointVM> GetCompanyPointDetailTableResult(int CompanyID, string CurrencyType = "")
    {
        List<DBViewModel.CompanyPointVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        if (CurrencyType != "")
        {//OB003 内充渠道
            SS = " SELECT CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
                 " FROM   CompanyPoint" +
                 " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
                 " WHERE CompanyPoint.forCompanyID = @CompanyID" +
                 " AND CompanyPoint.CurrencyType = @CurrencyType";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }
        else
        {   //OB003 内充渠道
            SS = " SELECT CompanyPoint.*,CompanyServicePoint.SystemPointValue AS AutoWithdrawAmount" +
                 " FROM   CompanyPoint" +
                 " LEFT JOIN CompanyServicePoint ON CompanyServicePoint.CompanyID=CompanyPoint.forCompanyID And CompanyServicePoint.CurrencyType=CompanyPoint.CurrencyType And CompanyServicePoint.ServiceType='OOB02'" +
                 " WHERE CompanyPoint.forCompanyID = @CompanyID";

            DBCmd = new SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.CompanyPointVM>(DT) as List<DBViewModel.CompanyPointVM>;
                foreach (var data in returnValue)
                {
                    //檢查錢包金額是否足夠
                    var CanUseCompanyPointDT = GetCanUseCompanyPointDetail(CompanyID, data.CurrencyType);

                    ////錢包檢查
                    if (CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0)
                    {
                        var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).First();
                        data.LockPointValue = CompanyPointModel.PointValue - CompanyPointModel.CanUsePoint;
                        data.FrozenPoint = CompanyPointModel.FrozenPoint;
                        data.InWithdrawProcessPoint = CompanyPointModel.InWithdrawProcessPoint;

                    }
                }
            }
        }

        return returnValue;
    }

    public List<DBViewModel.CompanyPointVM> GetAllCompanyPointTableResult()
    {
        List<DBViewModel.CompanyPointVM> returnValue = new List<DBViewModel.CompanyPointVM>();
        DBViewModel.CompanyPointVM tempdata = new DBViewModel.CompanyPointVM();
        List<DBViewModel.CompanyPointVM> tempResult = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT CompanyTable.CompanyName,CompanyPoint.* " +
             " FROM CompanyPoint WITH (NOLOCK)" +

             " JOIN CompanyTable WITH (NOLOCK) ON CompanyPoint.forCompanyID = CompanyTable.CompanyID" +
             " WHERE CompanyPoint.CurrencyType = 'JPY'";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                tempResult = DataTableExtensions.ToList<DBViewModel.CompanyPointVM>(DT) as List<DBViewModel.CompanyPointVM>;
                tempdata.CompanyName = "ALL";
                tempdata.CurrencyType = Pay.CurrencyType;
                tempdata.CompanyCount = tempResult.Count;
                foreach (var data in tempResult)
                {
                    tempdata.PointValue = Decimal.Add(tempdata.PointValue, data.PointValue);
                    //檢查錢包金額是否足夠
                    var CanUseCompanyPointDT = GetCanUseCompanyPoint(data.forCompanyID, tempdata.CurrencyType);

                    ////錢包檢查
                    if (CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0)
                    {
                        var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).First();
                        tempdata.LockPointValue = Decimal.Add(tempdata.LockPointValue, Decimal.Subtract(CompanyPointModel.PointValue, CompanyPointModel.CanUsePoint));
                        tempdata.FrozenPoint = Decimal.Add(tempdata.FrozenPoint, CompanyPointModel.FrozenPoint);
                    }
                }
                returnValue.Add(tempdata);

                foreach (var data in tempResult)
                {
                    DBViewModel.CompanyPointVM tempdata2 = new DBViewModel.CompanyPointVM();
                    tempdata2.CompanyName = data.CompanyName;
                    tempdata2.CurrencyType = data.CurrencyType;
                    tempdata2.PointValue = data.PointValue;
                    //檢查錢包金額是否足夠
                    var CanUseCompanyPointDT = GetCanUseCompanyPoint(data.forCompanyID, data.CurrencyType);

                    ////錢包檢查
                    if (CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0)
                    {
                        var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).First();
                        tempdata2.LockPointValue = CompanyPointModel.PointValue - CompanyPointModel.CanUsePoint;
                        tempdata2.FrozenPoint = CompanyPointModel.FrozenPoint;
                    }
                    returnValue.Add(tempdata2);
                }

            }
        }

        return returnValue;
    }

    public int InsertCompanyPoint(int CompanyID, string CurrencyType)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " INSERT INTO CompanyPoint (forCompanyID, CurrencyType) " +
             " VALUES (@forCompanyID, @CurrencyType)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);
        return returnValue;
    }

    //public int UpdateCompanyPoint(DBModel.CompanyPoint Model)
    //{
    //    int returnValue;
    //    string SS;
    //    System.Data.SqlClient.SqlCommand DBCmd = null;

    //    SS = "UPDATE CompanyPoint SET PointValue=@PointValue WHERE forCompanyID=@forCompanyID AND CurrencyType=@CurrencyType";

    //    DBCmd = new System.Data.SqlClient.SqlCommand();
    //    DBCmd.CommandText = SS;
    //    DBCmd.CommandType = CommandType.Text;
    //    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
    //    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
    //    DBCmd.Parameters.Add("@PointValue", SqlDbType.Decimal).Value = Model.PointValue;
    //    returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
    //    RedisCache.CompanyPoint.UpdateCompanyPointByID(Model.forCompanyID);
    //    return returnValue;
    //}
    #endregion

    #region  BankCode

    public List<DBModel.BankCodeTable> GetBankCodeTableResult()
    {
        List<DBModel.BankCodeTable> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM BankCode WITH (NOLOCK) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.BankCodeTable>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertBankCode(FromBody.BankCodeSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM BankCode WITH (NOLOCK) " +
             " WHERE BankCode=@BankCode ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;

        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO BankCode (BankCode,BankState,BankName,BankType,ETHContractNumber)" +
             " VALUES(@BankCode,@BankState,@BankName,@BankType,@ETHContractNumber)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankState", SqlDbType.Int).Value = fromBody.BankState;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = fromBody.BankName;
        DBCmd.Parameters.Add("@BankType", SqlDbType.Int).Value = fromBody.BankType;
        DBCmd.Parameters.Add("@ETHContractNumber", SqlDbType.VarChar).Value = fromBody.ETHContractNumber;


        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.BankCode.UpdateBankCode();
        return returnValue;

    }

    public int UpdateBankCode(FromBody.BankCodeSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE BankCode";
        SS += " SET BankState=@BankState,BankName=@BankName,BankType=@BankType,ETHContractNumber=@ETHContractNumber";
        SS += " WHERE BankCode=@BankCode ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankState", SqlDbType.Int).Value = fromBody.BankState;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = fromBody.BankName;
        DBCmd.Parameters.Add("@BankType", SqlDbType.Int).Value = fromBody.BankType;
        DBCmd.Parameters.Add("@ETHContractNumber", SqlDbType.VarChar).Value = fromBody.ETHContractNumber;


        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.BankCode.UpdateBankCode();
        return returnValue;

    }

    public int DisableBankCode(FromBody.BankCodeSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE BankCode";
        SS += " SET BankState=1";
        SS += " WHERE BankCode=@BankCode";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        RedisCache.BankCode.UpdateBankCode();
        return returnValue;

    }

    #endregion

    #region BankCard

    public List<DBViewModel.BankCardVM> GetBankCardTableResult(FromBody.BankCardSet fromBody)
    {
        List<DBViewModel.BankCardVM> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT BankCard.*,BankName FROM BankCard WITH (NOLOCK) " +
             " JOIN BankCode on BankCode.BankCode=BankCard.BankCode" +
             " WHERE forCompanyID=@forCompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.BankCardVM>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertBankCard(FromBody.BankCardSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM BankCard WITH (NOLOCK) " +
             " WHERE BankCard=@BankCard ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = fromBody.BankCard;

        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO BankCard (BankCode,forCompanyID,BankCard,BankCardName,BankBranchName,OwnProvince,OwnCity)" +
             " VALUES(@BankCode,@forCompanyID,@BankCard,@BankCardName,@BankBranchName,@OwnProvince,@OwnCity)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = fromBody.BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = fromBody.BankCardName;
        DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = fromBody.BankBranchName;
        DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = fromBody.OwnProvince;
        DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = fromBody.OwnCity;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateBankCard(FromBody.BankCardSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE BankCard";
        SS += " SET BankCardName=@BankCardName,BankCode=@BankCode,BankBranchName=@BankBranchName,OwnProvince=@OwnProvince,OwnCity=@OwnCity";
        SS += " WHERE BankCard=@BankCard ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = fromBody.BankCode;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = fromBody.BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = fromBody.BankCardName;
        DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = fromBody.BankBranchName;
        DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = fromBody.OwnProvince;
        DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = fromBody.OwnCity;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int DeleteBankCard(FromBody.BankCardSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " DELETE FROM BankCard";
        SS += " WHERE BankCard=@BankCard";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = fromBody.BankCard;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    #endregion

    #region BlockChainAddress

    public List<DBModel.CompanyBlockChainAddress> GetCompanyBlockChainAddressResult(int CompanyID)
    {
        List<DBModel.CompanyBlockChainAddress> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT * FROM CompanyBlockChainAddress WITH (NOLOCK) " +
             " WHERE forCompanyID=@forCompanyID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyBlockChainAddress>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int InsertBlockChainAddress(FromBody.BlockChainAddressSet fromBody,int CompanyID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM CompanyBlockChainAddress WITH (NOLOCK) " +
             " WHERE BlockChainAddress=@BlockChainAddress And forCompanyID=@forCompanyID ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BlockChainAddress", SqlDbType.VarChar).Value = fromBody.BlockChainAddress;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;

        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        SS = " INSERT INTO CompanyBlockChainAddress (BlockChainAddress,forCompanyID)" +
             " VALUES(@BlockChainAddress,@forCompanyID)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BlockChainAddress", SqlDbType.VarChar).Value = fromBody.BlockChainAddress;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
  
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int UpdateBlockChainAddress(FromBody.BlockChainAddressSet fromBody,int CompanyID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE CompanyBlockChainAddress";
        SS += " SET BlockChainAddress=@BlockChainAddress";
        SS += " WHERE forCompanyID=@forCompanyID AND BlockChainAddressID=@BlockChainAddressID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BlockChainAddressID", SqlDbType.Int).Value = fromBody.BlockChainAddressID;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@BlockChainAddress", SqlDbType.VarChar).Value = fromBody.BlockChainAddress;
     
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    public int DeleteBlockChainAddress(FromBody.BlockChainAddressSet fromBody)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " DELETE FROM CompanyBlockChainAddress";
        SS += " WHERE BlockChainAddressID=@BlockChainAddressID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BlockChainAddressID", SqlDbType.Int).Value = fromBody.BlockChainAddressID;

        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;

    }

    #endregion

    #region 提現相關
    public int RemoveWithdrawal(int WithdrawID)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int updateCount = 0;


        SS = " Delete Withdrawal " +
             " WHERE WithdrawID=@WithdrawID And Status=8";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;

        updateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return updateCount;
    }

    public int RemoveAllWithdrawal(List<int> WithdrawIDs)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int updateCount = 0;
        var parameters = new string[WithdrawIDs.Count];

        DBCmd = new System.Data.SqlClient.SqlCommand();

        DBCmd.CommandType = CommandType.Text;
        for (int i = 0; i < WithdrawIDs.Count; i++)
        {
            parameters[i] = string.Format("@WithdrawID{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
        }

        SS = string.Format("Delete Withdrawal WHERE WithdrawID IN ({0}) And Status=8", string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        updateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return updateCount;
    }

    public List<DBModel.Withdrawal> TmpWithdrawalCreate(FromBody.WithdrawalCreate fromBody, int CompnayID)
    {
        var returnValue = new List<DBModel.Withdrawal>();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int WithdrawalID = 0;
        List<string> lst_WithdrawSerial = new List<string>();
        string WithdrawSerial;
        decimal Charge = 0;
        DBModel.WithdrawLimit _WithdrawLimit = new DBModel.WithdrawLimit();
        _WithdrawLimit.CompanyID = CompnayID;
        _WithdrawLimit.WithdrawLimitType = 1;
        _WithdrawLimit.ProviderCode = "";
        var withdrawLimitResult = GetWithdrawLimitResult(_WithdrawLimit);

        DBAccess.ExecuteTransDB(Pay.DBConnStr, T =>
        {

            foreach (var data in fromBody.WithdrawalData)
            {

                WithdrawalID = 0;

                if (withdrawLimitResult != null)
                {
                    var tmpWithdrawLimitResult = withdrawLimitResult.Where(w => w.CurrencyType == data.CurrencyType && w.ServiceType == data.ServiceType).ToList();
                    if (tmpWithdrawLimitResult.Count > 0)
                    {
                        Charge = tmpWithdrawLimitResult.First().Charge;
                    }
                    else
                    {
                        Charge = 0;
                    }

                }

                SS = " INSERT INTO Withdrawal (forCompanyID,CurrencyType,Amount,Status,BankCard,BankName,BankBranchName,BankCardName,OwnProvince,OwnCity,ServiceType,CollectCharge,DownOrderDate)" +
                     " VALUES(@forCompanyID,@CurrencyType,@Amount,@Status,@BankCard,@BankName,@BankBranchName,@BankCardName,@OwnProvince,@OwnCity,@ServiceType,@CollectCharge,@DownOrderDate);" +
                     " Select @@IDENTITY";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompnayID;
                DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = data.BankName;
                DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = data.BankBranchName;
                DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = data.BankCardName;
                DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = data.OwnProvince;
                DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = data.OwnCity;
                DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = data.BankCard;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
                DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = data.Amount;
                DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = Charge;
                DBCmd.Parameters.Add("@DownOrderDate", SqlDbType.DateTime).Value = DateTime.Now;

                DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = data.ServiceType;

                //DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = Charge;

                DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 0;

                int.TryParse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString(), out WithdrawalID);
                if (WithdrawalID != 0)
                {
                    WithdrawSerial = "OP" + System.DateTime.Now.ToString("yyyyMMddHHmm") + (new string('0', 10 - WithdrawalID.ToString().Length) + WithdrawalID.ToString());
                    SS = "UPDATE Withdrawal  SET WithdrawSerial=@WithdrawSerial " +
                    " WHERE WithdrawID=@WithdrawID";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawalID;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    T.ExecuteDB(DBCmd);
                    lst_WithdrawSerial.Add(WithdrawSerial);
                }
            }
        });

        foreach (var withdrawSerial in lst_WithdrawSerial)
        {
            returnValue.Add(GetWithdrawalByWithdrawSerial(withdrawSerial));
        }

        return returnValue;
    }

    public DBModel.Withdrawal TmpWithdrawalUpdate(DBModel.Withdrawal data, int CompnayID)
    {
        DBModel.Withdrawal returnValue = null;
        String SS = String.Empty;
        int UpdateCount = 0;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET CurrencyType=@CurrencyType,Amount= @Amount,BankCard=@BankCard," +
             " BankName=@BankName,BankBranchName=@BankBranchName,BankCardName=@BankCardName,OwnProvince=@OwnProvince,OwnCity=@OwnCity " +
             " WHERE WithdrawID=@WithdrawID And forCompanyID=@forCompanyID And Status=8 " +
             " SELECT @@IDENTITY;";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompnayID;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = data.BankName;
        DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = data.BankBranchName;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = data.BankCardName;
        DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = data.OwnProvince;
        DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = data.OwnCity;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = data.BankCard;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = data.Amount;
        UpdateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        if (UpdateCount > 0)
        {
            returnValue = GetWithdrawalByWithdrawID(data.WithdrawID);
        }

        return returnValue;
    }

    public DBModel.Withdrawal WithdrawalUpdate(DBModel.Withdrawal data, int CompnayID)
    {
        DBModel.Withdrawal returnValue = null;
        String SS = String.Empty;
        int UpdateCount = 0;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET CurrencyType=@CurrencyType,Amount= @Amount,BankCard=@BankCard," +
             " BankName=@BankName,BankBranchName=@BankBranchName,BankCardName=@BankCardName,OwnProvince=@OwnProvince,OwnCity=@OwnCity,Status=13 " +
             " WHERE WithdrawID=@WithdrawID And forCompanyID=@forCompanyID" +
             " SELECT @@IDENTITY;";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompnayID;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = data.BankName;
        DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = data.BankBranchName;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = data.BankCardName;
        DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = data.OwnProvince;
        DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = data.OwnCity;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = data.BankCard;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = data.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = data.Amount;
        UpdateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        if (UpdateCount > 0)
        {
            returnValue = GetWithdrawalByWithdrawID(data.WithdrawID);
        }

        return returnValue;
    }

    //public int AdjustProviderPoint(FromBody.AdjustProviderPointSet data)
    //{

    //    int returnValue = -1;
    //    decimal ProviderCharge;
    //    decimal CostAmount;
    //    String SS = String.Empty;
    //    SqlCommand DBCmd;
    //    DBModel.WithdrawLimit WithdrawLimitModel = new DBModel.WithdrawLimit() { WithdrawLimitType = 0, ProviderCode = data.ProviderCode };
    //    List<DBModel.WithdrawLimit> ProviderWithdrawLimit;
    //    DBModel.Withdrawal WithdrawModel;
    //    #region 取得供應商手續費
    //    ProviderWithdrawLimit = GetWithdrawLimitResult(WithdrawLimitModel);
    //    if (ProviderWithdrawLimit != null)
    //    {
    //        if (ProviderWithdrawLimit.Where(w => w.CurrencyType == "JPY") != null)
    //        {
    //            ProviderCharge = ProviderWithdrawLimit.Where(w => w.CurrencyType == "JPY").First().Charge;
    //        }
    //        else
    //        {
    //            ProviderCharge = 0;
    //        }
    //    }
    //    else
    //    {
    //        ProviderCharge = 0;
    //    }
    //    #endregion

    //    #region 取得提現單
    //    WithdrawModel = GetWithdrawalByWithdrawSerial(data.WithdrawSerial);

    //    if (WithdrawModel == null)
    //    {
    //        returnValue = -1;
    //        return returnValue;
    //    }
    //    #endregion

    //    if (data.Amount > 0)
    //    {
    //        CostAmount = data.Amount + ProviderCharge;
    //    }
    //    else {
    //        CostAmount = data.Amount;
    //    }

    //    #region 確認是否已建立供應商錢包
    //    SS = " SELECT SystemPointValue " +
    //         " FROM ProviderPoint WITH(NOLOCK) " +
    //         " WHERE ProviderCode =@ProviderCode " +
    //         " AND CurrencyType =@CurrencyType ";

    //    DBCmd = new System.Data.SqlClient.SqlCommand();
    //    DBCmd.CommandText = SS;
    //    DBCmd.CommandType = CommandType.Text;

    //    DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
    //    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = "CNY";
    //    var SystemPointValue = DBAccess.GetDBValue(DBConnStr, DBCmd);
    //    #endregion

    //    //尚未建立該供應商錢包
    //    if (SystemPointValue == null) {
    //        #region 建立 ProviderPointHistory
    //        SS = " INSERT INTO ProviderPointHistory(CurrencyType,ProviderCode, OperatorType, Value, BeforeValue, Description, TransactionID) " +
    //            " VALUES(@CurrencyType,@ProviderCode ,2, @Value, @BeforeValue, @Description, @TransactionID)";

    //        DBCmd = new System.Data.SqlClient.SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = CommandType.Text;
    //        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = "CNY";
    //        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
    //        DBCmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = CostAmount * -1;
    //        DBCmd.Parameters.Add("@BeforeValue", SqlDbType.Decimal).Value = 0;
    //        DBCmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = "ReviewWithdrawal:WithdrawSerial=" + data.WithdrawSerial;
    //        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = WithdrawModel.WithdrawID;
    //        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
    //        #endregion

    //        #region ProviderPoint

    //        SS = " INSERT INTO ProviderPoint(CurrencyType, ProviderCode, SystemPointValue) " +
    //             " VALUES(@CurrencyType, @ProviderCode, @CostAmount) ";

    //        DBCmd = new System.Data.SqlClient.SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = CommandType.Text;
    //        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = "CNY";
    //        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
    //        DBCmd.Parameters.Add("@CostAmount", SqlDbType.Decimal).Value = CostAmount * -1;
    //        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
    //        #endregion
    //    }
    //    else {//已建立供應商錢包
    //        #region 建立 ProviderPointHistory
    //        SS = " INSERT INTO ProviderPointHistory(CurrencyType,ProviderCode, OperatorType, Value, BeforeValue, Description, TransactionID) " +
    //        " VALUES(@CurrencyType,@ProviderCode ,2, @Value, @BeforeValue, @Description, @TransactionID)";

    //        DBCmd = new System.Data.SqlClient.SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = CommandType.Text;
    //        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = "CNY";
    //        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
    //        DBCmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = CostAmount * -1;
    //        DBCmd.Parameters.Add("@BeforeValue", SqlDbType.Decimal).Value = decimal.Parse(SystemPointValue.ToString());
    //        DBCmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = "ReviewWithdrawal:WithdrawSerial=" + data.WithdrawSerial;
    //        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = WithdrawModel.WithdrawID;
    //        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
    //        #endregion

    //        #region ProviderPoint

    //        SS = "  UPDATE ProviderPoint " +
    //             "  SET SystemPointValue = SystemPointValue - @CostAmount" +
    //             "  WHERE CurrencyType = @CurrencyType" +
    //             "  AND ProviderCode = @ProviderCode";

    //        DBCmd = new System.Data.SqlClient.SqlCommand();
    //        DBCmd.CommandText = SS;
    //        DBCmd.CommandType = CommandType.Text;
    //        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = "CNY";
    //        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = data.ProviderCode;
    //        DBCmd.Parameters.Add("@CostAmount", SqlDbType.Decimal).Value = CostAmount;
    //        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
    //        #endregion
    //    }

    //    return returnValue;
    //}

    public DBViewModel.ProviderWithdrawalOrderCount GetProviderWithdrawalOrderCount(string ProviderCode, int GroupID)
    {
        DBViewModel.ProviderWithdrawalOrderCount returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT count(*) As TotalCount, " +
             " SUM(CASE WHEN CreateDate < dateadd(minute, -20, getdate()) THEN 1 ELSE 0 END) As TotalCountTimeEnd " +
             " FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK) ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " WHERE Withdrawal.Status=1 AND Withdrawal.ProviderCode=@ProviderCode And (PPO.GroupID=0 or PPO.GroupID=@GroupID) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderWithdrawalOrderCount>(DT).ToList().FirstOrDefault();
            }
        }

        return returnValue;
    }

    public int GetProviderPaymentOrderCount(string ProviderCode, int GroupID)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = "SELECT Count(*)  " +
             "FROM PaymentTable AS P WITH(NOLOCK) " +
             " LEFT JOIN  ProxyProviderOrder PPO WITH(NOLOCK) ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0" +
             "WHERE  SubmitType=1 And P.ProcessStatus=8 And P.ProviderCode=@ProviderCode And (PPO.GroupID=0 or PPO.GroupID=@GroupID) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public string GetWithdrawalCount()
    {
        string returnValue = "";
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT Count(*)  " +
             " FROM Withdrawal WITH (NOLOCK) " +
             " WHERE Status=0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();

        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();

        return returnValue;
    }

    //建立提現訂單
    public int WithdrawalCreate(FromBody.WithdrawalCreate fromBody, int CompnayID, string ProviderGroups, int BackendWithdrawType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        string AutoWithdrawalProviderCode = "";
        decimal CostCharge = 0;
        string CompanyCode = "";
        string CompanyDescription = "";

        int GroupID = 0;
        DBModel.ProxyProvider ProxyProviderModel = null;
        AutoWithdrawalProviderCode = GetCompanyAutoWithdrawalProviderCode(CompnayID);

        CompanyCode = GetCompanyCodeByCompanyID(CompnayID);
        CompanyDescription = GetCompanyDescriptionByCompanyID(CompnayID);
      

        var DistinctWithdrawalData = fromBody.WithdrawalData
    .GroupBy(m => new { m.BankCard, m.BankCardName, m.BankName })
    .Select(group => group.First())  // instead of First you can also apply your logic here what you want to take, for example an OrderBy
    .ToList();


        foreach (var data in fromBody.WithdrawalData)
        {

            if (CheckWitdrawal(data.WithdrawID, data.BankCard, data.BankCardName, data.BankName) == 0)
            {

                if (DistinctWithdrawalData.Contains(data))
                {
                    if (data.ServiceType == "OOB02" && AutoWithdrawalProviderCode == "LPay2" && BackendWithdrawType == 1)
                    {
                        if (ProxyProviderModel != null && data.Amount <= ProxyProviderModel.MaxWithdrawalAmount)
                        {

                            if (GetProxyProviderOrderByOrderSerial(data.WithdrawSerial, 1) == null)
                            {
                                if (ProviderGroups == "0")
                                {
                                    GroupID = BackendFunction.SelectProxyProviderGroup(AutoWithdrawalProviderCode, data.Amount);
                                }
                                else
                                {
                                    GroupID = BackendFunction.SelectProxyProviderGroupByCompanySelected(AutoWithdrawalProviderCode, data.Amount, ProviderGroups);
                                }

                                InsertProxyProviderOrder(data.WithdrawSerial, 1, 0, 0, GroupID);
                            }

                            SS = " UPDATE Withdrawal  SET Status=1,CreateDate=@CreateDate,ProviderCode=@ProviderCode,CostCharge=@CostCharge,CompanyDescription=@CompanyDescription " +
                           " WHERE WithdrawID=@WithdrawID And Status=8 And FinishDate is Null";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                            DBCmd.Parameters.Add("@CostCharge", SqlDbType.Decimal).Value = CostCharge;
                            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = AutoWithdrawalProviderCode;
                            DBCmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                            DBCmd.Parameters.Add("@CompanyDescription", SqlDbType.NVarChar).Value = CompanyDescription;
                            returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);
                        }
                        else
                        {
                            SS = " UPDATE Withdrawal  SET Status=0,CreateDate=@CreateDate,CompanyDescription=@CompanyDescription " +
                         " WHERE WithdrawID=@WithdrawID  And Status=8 And FinishDate is Null";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                            DBCmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                            DBCmd.Parameters.Add("@CompanyDescription", SqlDbType.NVarChar).Value = CompanyDescription;

                            returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);
                        }
                    }
                    else if (data.ServiceType == "OOB02" && AutoWithdrawalProviderCode == "YE888" && BackendWithdrawType == 1)
                    {
                        if (ProxyProviderModel != null && data.Amount <= ProxyProviderModel.MaxWithdrawalAmount)
                        {

                            if (GetProxyProviderOrderByOrderSerial(data.WithdrawSerial, 1) == null)
                            {
                                if (ProviderGroups == "0")
                                {
                                    GroupID = BackendFunction.SelectProxyProviderGroup(AutoWithdrawalProviderCode, data.Amount);
                                }
                                else
                                {
                                    GroupID = BackendFunction.SelectProxyProviderGroupByCompanySelected(AutoWithdrawalProviderCode, data.Amount, ProviderGroups);
                                }

                                InsertProxyProviderOrder(data.WithdrawSerial, 1, 0, 0, GroupID);
                            }

                            SS = " UPDATE Withdrawal  SET Status=1,CreateDate=@CreateDate,ProviderCode=@ProviderCode,CostCharge=@CostCharge,CompanyDescription=@CompanyDescription " +
                           " WHERE WithdrawID=@WithdrawID And Status=8 And FinishDate is Null";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                            DBCmd.Parameters.Add("@CostCharge", SqlDbType.Decimal).Value = CostCharge;
                            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = AutoWithdrawalProviderCode;
                            DBCmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                            DBCmd.Parameters.Add("@CompanyDescription", SqlDbType.NVarChar).Value = CompanyDescription;
                            returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);
                        }
                        else
                        {
                            SS = " UPDATE Withdrawal  SET Status=0,CreateDate=@CreateDate,CompanyDescription=@CompanyDescription " +
                         " WHERE WithdrawID=@WithdrawID  And Status=8 And FinishDate is Null";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                            DBCmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                            DBCmd.Parameters.Add("@CompanyDescription", SqlDbType.NVarChar).Value = CompanyDescription;

                            returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);
                        }
                    }
                    else
                    {
                        SS = " UPDATE Withdrawal  SET Status=0,CreateDate=@CreateDate,CompanyDescription=@CompanyDescription " +
                             " WHERE WithdrawID=@WithdrawID  And Status=8 And FinishDate is Null";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                        DBCmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                        DBCmd.Parameters.Add("@CompanyDescription", SqlDbType.NVarChar).Value = CompanyDescription;

                        returnValue += DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    }
                }
                else
                {
                    SS = " UPDATE Withdrawal SET RejectDescription=@RejectDescription" +
                         " WHERE WithdrawID=@WithdrawID ";

                    InsertRiskControlWithdrawal(CompanyCode, data.BankCard, data.BankCardName, data.BankName);
                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                    DBCmd.Parameters.Add("@RejectDescription", SqlDbType.NVarChar).Value = "5分钟内只能提交一张相同银行卡资讯订单";
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                }




            }
            else
            {
                SS = " UPDATE Withdrawal SET RejectDescription=@RejectDescription" +
              " WHERE WithdrawID=@WithdrawID ";

                InsertRiskControlWithdrawal(CompanyCode, data.BankCard, data.BankCardName, data.BankName);
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = data.WithdrawID;
                DBCmd.Parameters.Add("@RejectDescription", SqlDbType.NVarChar).Value = "5分钟内只能提交一张相同银行卡资讯订单";
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
            }
        }

        return returnValue;
    }

     

    public int CheckWitdrawal(int WithdrawID, string BankCard, string BankCardName, string BankName)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -1;

        SS = " SELECT COUNT(*) as Count " +
             " FROM Withdrawal W " +
             " Where CreateDate" +
             " between DATEADD(minute, -5, GETDATE())" +
             " And GETDATE()" +
             " AND W.BankCard = @BankCard" +
             " AND W.BankCardName = @BankCardName" +
             " AND W.BankName = @BankName  " +
             " AND W.WithdrawID<>@WithdrawID" +
             " And Status <> 8";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = BankCardName;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = BankName;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;
        var dbreturn = DBAccess.GetDBValue(DBConnStr, DBCmd);
        if (dbreturn != null)
        {
            returnValue = int.Parse(dbreturn.ToString());
        }

        return returnValue;
    }

    public string GetCompanyAutoWithdrawalProviderCode(int CompnayID)
    {
        string returnValue = "";
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT TOP(1) GWR.ProviderCode FROM CompanyTable CT WITH(NOLOCK)" +
             " JOIN GPayWithdrawRelation GWR WITH(NOLOCK)  ON GWR.forCompanyID = CT.CompanyID" +
             " AND GWR.CurrencyType = 'JPY'" +
             " JOIN ProviderCode PC WITH(NOLOCK)  ON PC.ProviderCode = GWR.ProviderCode AND PC.CollectType = 1" +
             " WHERE CT.CompanyID = @CompanyID AND CT.AutoWithdrawalServiceType = 'OOB02'";


        DBCmd = new System.Data.SqlClient.SqlCommand();

        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompnayID;
        if (DBAccess.GetDBValue(DBConnStr, DBCmd) != null)
        {
            returnValue = DBAccess.GetDBValue(DBConnStr, DBCmd).ToString();
        }

        return returnValue;
    }

    //每日提現額度與筆數查詢       
    public List<DBModel.WithdrawalTotalAmountsByDate> GetWithdrawalTotalAmountsByDate()
    {
        List<DBModel.WithdrawalTotalAmountsByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT count(*) as TotalCount,sum(Amount) as Amount,BankCardName,BankCard,CurrencyType from Withdrawal WITH (NOLOCK) " +
             " Where convert(varchar,CreateDate, 111) =@CreateDate And Status<>8 AND Status <> 90 AND Status <> 91 And FloatType=0 " +
             " Group by BankCardName,BankCard,CurrencyType  ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@CreateDate", SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyy/MM/dd");

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalTotalAmountsByDate>(DT).ToList();
            }
        }

        return returnValue;
    }
    //代付查詢
    public List<DBModel.Withdrawal> GetWithdrawalReport(FromBody.WithdrawalReportSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        var parameters = new string[fromBody.Status.Count];

        DBCmd = new System.Data.SqlClient.SqlCommand();

        SS = " SELECT convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC), 120) as CreateDate2,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC), 120) as FinishDate2,Withdrawal.* FROM Withdrawal WITH (NOLOCK) " +
             " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) <= @EndDate And forCompanyID=@CompanyID And Status<>8 AND Status <> 90 AND Status <> 91  And (FloatType=1 OR FloatType=2) ";

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And WithdrawSerial=@WithdrawSerial";
        }

        if (fromBody.BankCardName != "")
        {
            SS += " And BankCardName=@BankCardName";
        }

        if (fromBody.OrderID != "")
        {
            SS += " And DownOrderID=@OrderID";
        }

        if (fromBody.Status.FirstOrDefault() != 99)
        {

            for (int i = 0; i < fromBody.Status.Count; i++)
            {
                parameters[i] = string.Format("@Status{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.Status[i]);
            }
            SS += " And Status IN ({0}) ";
            SS = string.Format(SS, string.Join(", ", parameters));
        }


        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = fromBody.BankCardName;
        DBCmd.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = fromBody.OrderID;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    //提現查詢
    public List<DBModel.Withdrawal> GetWithdrawalTableResult(FromBody.WithdrawalSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC), 120) as CreateDate2,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC), 120) as FinishDate2,Withdrawal.* FROM Withdrawal WITH (NOLOCK) " +
             " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) <= @EndDate And forCompanyID=@CompanyID And Status<>8 AND Status <> 90 AND Status <> 91 And FloatType=0  ";

        //過濾資料
        if (fromBody.Status != 99)
        { //99代表取得所有資料
            SS += " And Status=@Status";
        }

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And WithdrawSerial=@WithdrawSerial";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    //提現申請
    public List<DBModel.Withdrawal> GetWithdrawalTableResultByCompanyID(int CompanyID)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT Withdrawal.*,ServiceType.ServiceTypeName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=Withdrawal.ServiceType And ServiceType.CurrencyType=Withdrawal.CurrencyType" +
             " WHERE  forCompanyID=@CompanyID And Status=8";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;


        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBModel.Withdrawal GetWithdrawalResultByWithdrawSerial(string WithdrawSerial)
    {
        DBModel.Withdrawal returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ProviderName,Withdrawal.*,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,BankName,BankCardName,OwnProvince,OwnCity,BankBranchName,CompanyName,RealName as RealName1 FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable WITH (NOLOCK) ON AdminTable.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " WHERE WithdrawSerial=@WithdrawSerial ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;

        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).First();
            }
        }

        return returnValue;
    }

    public int UpdateWithdrawalBankSequence(string WithdrawSerial, string BankDescription, int Status)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int updateCount = 0;


        SS = " UPDATE Withdrawal  SET BankDescription=@BankDescription,Status=@Status" +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = BankDescription;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
        updateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return updateCount;
    }

    public int UpdateWithdrawalRejectDescription(string WithdrawSerial, string RejectDescription, int Status)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int updateCount = 0;


        SS += " UPDATE Withdrawal  SET Status=@Status";
        switch (Status)
        {
            case 9:
                SS += " ,RejectDescription=@RejectDescription";
                break;
            case 10:
                SS += " ,ManagerRejectDescription=@RejectDescription";
                break;
            case 11:
                SS += " ,CashierRejectDescription=@RejectDescription";
                break;
            default:
                break;
        }

        SS += " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@RejectDescription", SqlDbType.NVarChar).Value = RejectDescription;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
        updateCount = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return updateCount;
    }

    public DBModel.UpdateWithdrawalResultsByAdminCheck UpdateWithdrawalResultsByAdminCheck(int Status, List<int> WithdrawIDs, int AdminID)
    {
        DBModel.UpdateWithdrawalResultsByAdminCheck returnValue = new DBModel.UpdateWithdrawalResultsByAdminCheck();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var parameters = new string[WithdrawIDs.Count];
        List<DBModel.Withdrawal> WithdrawalModel = null;
        DataTable DT;
        List<DBModel.Provider> ProviderModel;
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];
        //var WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);

        //失敗單
        if (Status == 3)
        {
            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,FinishDate=@FinishDate" +
                " WHERE WithdrawID IN ({0}) And Status=4;";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
            DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;

            for (int i = 0; i < WithdrawIDs.Count; i++)
            {
                parameters[i] = string.Format("@WithdrawID{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

            if (DBreturn > 0)
            {

                SS = " Select * From Withdrawal  WITH (NOLOCK)  " +
                     " WHERE WithdrawID IN ({0});";

                DBCmd = new System.Data.SqlClient.SqlCommand();

                for (int i = 0; i < WithdrawIDs.Count; i++)
                {
                    parameters[i] = string.Format("@WithdrawID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
                }

                SS = string.Format(SS, string.Join(", ", parameters));
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;

                DT = DBAccess.GetDB(DBConnStr, DBCmd);

                if (DT != null)
                {
                    if (DT.Rows.Count > 0)
                    {
                        WithdrawalModel = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
                        returnValue.WithdrawalModel = WithdrawalModel;
                        returnValue.State = 0;
                    }
                    else
                    {
                        returnValue.Message = "Error";
                        returnValue.State = -1;
                    }
                }
                else
                {
                    returnValue.Message = "Error";
                    returnValue.State = -1;
                }
            }
            else
            {
                returnValue.Message = "Error";
                returnValue.State = -1;
            }
        }
        else
        {
            SS = " Select * From Withdrawal  WITH (NOLOCK)  " +
                 " WHERE WithdrawID IN ({0});";

            DBCmd = new System.Data.SqlClient.SqlCommand();

            for (int i = 0; i < WithdrawIDs.Count; i++)
            {
                parameters[i] = string.Format("@WithdrawID{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;

            DT = DBAccess.GetDB(DBConnStr, DBCmd);

            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {
                    WithdrawalModel = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
                }
                else
                {
                    returnValue.Message = "Error";
                    returnValue.State = -1;
                }
            }
            else
            {
                returnValue.Message = "Error";
                returnValue.State = -1;
            }

            if (WithdrawalModel != null)
            {
                ProviderModel = GetProviderCodeResult();

                foreach (var withdrawaldata in WithdrawalModel)
                {
                    //找不到訂單資訊
                    if (withdrawaldata.Status == 4)
                    {
                        if (withdrawaldata.WithdrawType == 0 && Status == 5)
                        {
                            //水池撥款
                            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                                 " WHERE WithdrawID=@WithdrawID";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 12;
                            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;

                            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                            if (DBreturn == 0)
                            {
                                returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            }

                        }
                        else if (withdrawaldata.WithdrawType == 1 && Status == 5)
                        {
                            //代付
                            if (!string.IsNullOrEmpty(withdrawaldata.ProviderCode))
                            {

                                bool autoPay = false;

                                if (ProviderModel != null)
                                {
                                    var ProviderData = ProviderModel.Where(w => (w.ProviderCode == withdrawaldata.ProviderCode && (w.ProviderAPIType & 2) == 2));

                                    if (ProviderData.Count() > 0)
                                    {
                                        autoPay = true;
                                    }

                                    if (autoPay)
                                    {   //api自動代付
                                        SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                                             " WHERE WithdrawID=@WithdrawID";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 5;
                                        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                                        #region SignCheck
                                        string strSign;
                                        string sign;
                                        DBModel.API_WithdrawResult returnRequireWithdrawal = null;

                                        strSign = string.Format("WithdrawID={0}&GPayBackendKey={1}"
                                        , withdrawaldata.WithdrawID
                                        , GPayBackendKey
                                        );

                                        sign = CodingControl.GetSHA256(strSign);

                                        #endregion
                                        var _RequireWithdrawalSet = new DBModel.RequireWithdrawalSet();
                                        _RequireWithdrawalSet.WithdrawSerial = withdrawaldata.WithdrawSerial;
                                        _RequireWithdrawalSet.Sign = sign;

                                        var strRequireWithdrawal = CodingControl.RequestJsonAPI(GPayApiUrl + "RequireWithdrawal", JsonConvert.SerializeObject(_RequireWithdrawalSet));

                                        if (!string.IsNullOrEmpty(strRequireWithdrawal))
                                        {
                                            returnRequireWithdrawal = JsonConvert.DeserializeObject<DBModel.API_WithdrawResult>(strRequireWithdrawal);
                                            //OK = 0,ERR = 1,SignErr = 2,Invalidate = 3
                                            switch (returnRequireWithdrawal.Status)
                                            {
                                                case 0:

                                                case 1:
                                                    SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                                         " WHERE WithdrawID=@WithdrawID";

                                                    DBCmd = new System.Data.SqlClient.SqlCommand();
                                                    DBCmd.CommandText = SS;
                                                    DBCmd.CommandType = CommandType.Text;
                                                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                                    DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                                    returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                                    break;
                                                case 2:
                                                    SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                                         " WHERE WithdrawID=@WithdrawID";

                                                    DBCmd = new System.Data.SqlClient.SqlCommand();
                                                    DBCmd.CommandText = SS;
                                                    DBCmd.CommandType = CommandType.Text;
                                                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                                    DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                                    returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                                    break;
                                                case 3:
                                                    SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                                          " WHERE WithdrawID=@WithdrawID";

                                                    DBCmd = new System.Data.SqlClient.SqlCommand();
                                                    DBCmd.CommandText = SS;
                                                    DBCmd.CommandType = CommandType.Text;
                                                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                                    DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                                    returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                                    break;
                                                default:

                                                    SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                                         " WHERE WithdrawID=@WithdrawID";

                                                    DBCmd = new System.Data.SqlClient.SqlCommand();
                                                    DBCmd.CommandText = SS;
                                                    DBCmd.CommandType = CommandType.Text;
                                                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                                    DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = "其他錯誤";

                                                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                                    returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                                  " WHERE WithdrawID=@WithdrawID";

                                            DBCmd = new System.Data.SqlClient.SqlCommand();
                                            DBCmd.CommandText = SS;
                                            DBCmd.CommandType = CommandType.Text;
                                            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                            DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                                            DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = "API代付失敗";
                                            DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                        }
                                    }
                                    else
                                    {
                                        //手動供應商後台撥款
                                        SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                                             " WHERE WithdrawID=@WithdrawID";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 12;
                                        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;

                                        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                    }
                                }
                                else
                                {
                                    returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                                }
                            }
                            else
                            {
                                returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            }
                        }
                        else
                        {
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                        }
                    }
                    else
                    {
                        returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                    }
                }

                #region 取得所有單最新狀態
                SS = " Select * From Withdrawal  WITH (NOLOCK)  " +
                          " WHERE WithdrawID IN ({0});";

                DBCmd = new System.Data.SqlClient.SqlCommand();

                for (int i = 0; i < WithdrawIDs.Count; i++)
                {
                    parameters[i] = string.Format("@WithdrawID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
                }

                SS = string.Format(SS, string.Join(", ", parameters));
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;

                DT = DBAccess.GetDB(DBConnStr, DBCmd);

                if (DT != null)
                {
                    if (DT.Rows.Count > 0)
                    {
                        WithdrawalModel = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
                        returnValue.WithdrawalModel = WithdrawalModel;
                        returnValue.State = 0;
                    }
                }
                #endregion
            }
            else
            {
                returnValue.Message = "Error";
                returnValue.State = -1;
            }
        }
        return returnValue;
    }

    public DBModel.UpdateWithdrawalResultsByAdminCheck UpdateWithdrawalResultsByAdminDoubleCheck(List<int> WithdrawIDs)
    {
        DBModel.UpdateWithdrawalResultsByAdminCheck returnValue = new DBModel.UpdateWithdrawalResultsByAdminCheck();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var parameters = new string[WithdrawIDs.Count];
        List<DBModel.Withdrawal> WithdrawalModel = null;
        DataTable DT;

        #region 確認訂單是否存在

        SS = " Select * From Withdrawal  WITH (NOLOCK)  " +
             " WHERE WithdrawID IN ({0});";

        DBCmd = new System.Data.SqlClient.SqlCommand();

        for (int i = 0; i < WithdrawIDs.Count; i++)
        {
            parameters[i] = string.Format("@WithdrawID{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
        }

        SS = string.Format(SS, string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                WithdrawalModel = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        #endregion

        if (WithdrawalModel != null)
        {
            foreach (var withdrawaldata in WithdrawalModel)
            {
                if (withdrawaldata.Status == 6)
                {
                    //扣除公司額度
                    SS = "spReviewWithdrawal";
                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.StoredProcedure;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = withdrawaldata.WithdrawSerial;
                    DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    DBreturn = (int)DBCmd.Parameters["@Return"].Value;
                    switch (DBreturn)
                    {
                        case 0://成功

                            break;
                        case -1://交易單不存在
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            //UpdateWithdrawal(withdrawaldata.WithdrawSerial, "交易單不存在");
                            break;
                        case -2://營運商額度錯誤 
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            //UpdateWithdrawal(withdrawaldata.WithdrawSerial, "營運商錢包金額錯誤");
                            break;
                        case -3://營運商額度不足
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            //UpdateWithdrawal(withdrawaldata.WithdrawSerial, "營運商錢包金額不足");
                            break;
                        case -4://鎖定失敗
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            //UpdateWithdrawal(withdrawaldata.WithdrawSerial, "鎖定失敗");
                            break;
                        case -5://加扣點失敗
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            //UpdateWithdrawal(withdrawaldata.WithdrawSerial, "加扣點失敗");
                            break;
                        default://其他錯誤
                            returnValue.ErrorWithdrawal.Add(withdrawaldata.WithdrawSerial);
                            // UpdateWithdrawal(withdrawaldata.WithdrawSerial, "其他錯誤");
                            break;
                    }
                }
                else if (withdrawaldata.Status == 7)
                {
                    SS = " UPDATE Withdrawal  SET Status=@Status,FinishDate=@FinishDate" +
                         " WHERE WithdrawID=@WithdrawID";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 3;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = withdrawaldata.WithdrawID;
                    DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;
                    DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                    if (DBreturn == 0)
                    {
                        UpdateWithdrawal(withdrawaldata.WithdrawSerial, "更新訂單狀態失敗");
                    }
                }
                else
                {
                    UpdateWithdrawal(withdrawaldata.WithdrawSerial, "訂單狀態錯誤");
                }
            }
            #region 取得訂單更新後狀態

            SS = " Select * From Withdrawal  WITH (NOLOCK)  " +
                 " WHERE WithdrawID IN ({0});";

            DBCmd = new System.Data.SqlClient.SqlCommand();

            for (int i = 0; i < WithdrawIDs.Count; i++)
            {
                parameters[i] = string.Format("@WithdrawID{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], WithdrawIDs[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;

            DT = DBAccess.GetDB(DBConnStr, DBCmd);

            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {
                    WithdrawalModel = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
                    returnValue.WithdrawalModel = WithdrawalModel;
                    returnValue.State = 0;
                }
                else
                {
                    returnValue.Message = "Error";
                    returnValue.State = -1;
                }
            }
            else
            {
                returnValue.Message = "Error";
                returnValue.State = -1;
            }

            #endregion

        }
        else
        {
            returnValue.Message = "Error";
            returnValue.State = -1;
        }

        return returnValue;
    }

    public DBViewModel.UpdateWithdrawalResult UpdateWithdrawalResultByWithdrawSerial(int Status, string WithdrawSerial, int AdminID, string ProviderCode, int WithdrawType, string ServiceType)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.Withdrawal WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        decimal Charge=0;
        decimal Rate = 0;
        decimal ProviderBlockChainRate=0;
        decimal OKEXRate=0;

        int CompanyType;

        CompanyType = GetCompanyTypeByWithdrawSerial(WithdrawSerial);
        if (CompanyType == -1)
        {
            returnValue.Message = "商户状态有误";
            return returnValue;
        }

        if (WithdrawData == null)
        {
            returnValue.WithdrawalData = WithdrawData;
            returnValue.Message = "订单不存在";
            returnValue.Status = -1;
            return returnValue;
        }

        if (WithdrawData.Status == 1)
        {
            returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
            returnValue.Message = "订单审核中";
            returnValue.Status = -2;
            return returnValue;
        }

        if (WithdrawData.Status == 2|| WithdrawData.Status == 3)
        {
            returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
            returnValue.Message = "订单已完成";
            returnValue.Status = -2;
            return returnValue;
        }

        if (Status != 3)
        {
            //取得供應商代付手續費
            DBModel.WithdrawLimit _WithdrawLimit = new DBModel.WithdrawLimit()
            {
                CompanyID = WithdrawData.forCompanyID,
                WithdrawLimitType = 0,
                ProviderCode = ProviderCode
            };

            if (WithdrawData.IsBlockChainOrder == 1)
            {
                System.Data.DataTable ProviderWithdrawLimitDT = RedisCache.ProviderWithdrawLimit.GetProviderWithdrawLimitBlockChainRate(ProviderCode, WithdrawData.CurrencyType);
                if (ProviderWithdrawLimitDT==null)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "尚未定供应商浮动马数";
                    returnValue.Status = -1;
                    return returnValue;
                }

                List<DBModel.WithdrawLimit> ProviderWithdrawLimitModels =DataTableExtensions.ToList<DBModel.WithdrawLimit>(ProviderWithdrawLimitDT).ToList();
                DBModel.WithdrawLimit ProviderWithdrawLimitModel = ProviderWithdrawLimitModels.Where(w => w.ServiceType == WithdrawData.ServiceType).FirstOrDefault();
                if (ProviderWithdrawLimitModel==null) {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "尚未定供应商浮动马数";
                    returnValue.Status = -1;
                    return returnValue;
                }

                ProviderBlockChainRate = ProviderWithdrawLimitModel.BlockChainRate;

                if (ProviderWithdrawLimitModel.BlockChainRate > WithdrawData.DownBlockChainRate)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "商户浮动马数 > 上游浮动马数";
                    returnValue.Status = -1;
                    return returnValue;
                }


                System.Data.DataTable BlockChainRateDT= RedisCache.BlockChainRate.GetBlockChainRate(WithdrawData.CurrencyType);

                if (BlockChainRateDT != null && BlockChainRateDT.Rows.Count > 0)
                {
                    OKEXRate = (decimal)BlockChainRateDT.Rows[0]["Rate"];
                }
            }
            else {

                //0=上游手續費，API/1=提現手續費/2=代付手續費(下游)
                DBModel.WithdrawLimit withdrawLimitResult = null;
                if (WithdrawData.FloatType == 0) //後台下發
                {
                    withdrawLimitResult = GetWithdrawLimitResultByCurrencyType(WithdrawData.CurrencyType, 0, ProviderCode, WithdrawData.forCompanyID);
                }
                else { //api 下發 
                    withdrawLimitResult = GetWithdrawLimitResultByCurrencyType(WithdrawData.CurrencyType, 3, ProviderCode, WithdrawData.forCompanyID);
                }

                if (withdrawLimitResult == null)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "尚未定供应商手续费";
                    returnValue.Status = -1;
                    return returnValue;
                }
                Charge = withdrawLimitResult.Charge;
                Rate = withdrawLimitResult.Rate;
            }


            //检查供应商通道额度
            var ProviderPointModel = GetProviderPointByProviderCode(ProviderCode);
            if (ProviderPointModel == null)
            {
                returnValue.WithdrawalData = WithdrawData;
                returnValue.Message = "供应商通道额度错误";
                returnValue.Status = -1;
                return returnValue;
            }

            if (WithdrawData.IsBlockChainOrder == 1)
            {
                if (WithdrawData.Amount > ProviderPointModel.SystemPointValue)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "供应商通道额度不足";
                    returnValue.Status = -1;
                    return returnValue;
                }
            }
            else {
                if (WithdrawData.Amount + Charge + (Rate * 0.01M * WithdrawData.Amount) > ProviderPointModel.SystemPointValue)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "供应商通道额度不足";
                    returnValue.Status = -1;
                    return returnValue;
                }
            }
            

            if (CompanyType != 4)
            {
                //检查商户支付通道额度
                List<DBViewModel.CompanyServicePointVM> ServicePointModel =null;
                if (WithdrawData.FloatType==0)
                {
                    //後台提單
                    ServicePointModel = GetCompanyServicePointByServiceType(WithdrawData.forCompanyID, ServiceType, WithdrawData.CurrencyType);
                }
                else {
                    //API
                    ServicePointModel = GetCompanyServicePointByServiceTypeAPI(WithdrawData.forCompanyID, ServiceType, WithdrawData.CurrencyType);
                }
               
                if (ServicePointModel == null)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "商户支付通道额度错误";
                    returnValue.Status = -1;
                    return returnValue;
                }

                if (ServicePointModel.First().MaxLimit == 0 || ServicePointModel.First().MinLimit == 0)
                {
                    returnValue.WithdrawalData = WithdrawData;
                    returnValue.Message = "尚未设定商户支付通道限额";
                    returnValue.Status = -1;
                    return returnValue;
                }

                WithdrawData.CollectCharge = ServicePointModel.First().Charge;

                if (WithdrawData.IsBlockChainOrder == 1)
                {
                    WithdrawData.CollectCharge = 0;
                    if (WithdrawData.Amount > ServicePointModel.First().SystemPointValue)
                    {
                        returnValue.WithdrawalData = WithdrawData;
                        returnValue.Message = "商户支付通道额度不足";
                        returnValue.Status = -1;
                        return returnValue;
                    }
                }
                else {
                    if (WithdrawData.Amount + ServicePointModel.First().Charge +(ServicePointModel.First().Rate*0.01M* WithdrawData.Amount) > ServicePointModel.First().SystemPointValue)
                    {
                        returnValue.WithdrawalData = WithdrawData;
                        returnValue.Message = "商户支付通道额度不足";
                        returnValue.Status = -1;
                        return returnValue;
                    }
                }
             
            }
            else {
                WithdrawData.CollectCharge = Charge;
            }
       
            //修改訂單狀態為審核中
            SS = " UPDATE Withdrawal  Set ConfirmByAdminID=@AdminID,ProviderCode=@ProviderCode,WithdrawType=@WithdrawType,Status=@Status,CostCharge=@CostCharge,ServiceType=@ServiceType,UpBlockChainRate=@UpBlockChainRate,OKEXRate=@OKEXRate,UpRate=@UpRate " +
                 " WHERE WithdrawSerial=@WithdrawSerial";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
            DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
            DBCmd.Parameters.Add("@WithdrawType", SqlDbType.Int).Value = WithdrawType;
            DBCmd.Parameters.Add("@CostCharge", SqlDbType.Int).Value = Charge;
            DBCmd.Parameters.Add("@OKEXRate", SqlDbType.Decimal).Value = OKEXRate;
            DBCmd.Parameters.Add("@UpRate", SqlDbType.Decimal).Value = Rate;
            DBCmd.Parameters.Add("@UpBlockChainRate", SqlDbType.Decimal).Value = ProviderBlockChainRate;
            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

            if (DBreturn == 0)
            {
                returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                returnValue.Message = "修改订单状态错误";
                returnValue.Status = -3;
                return returnValue;
            }

            WithdrawData.ProviderCode = ProviderCode;
        }

        //更改為失敗單
        if (Status == 3)
        {
            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,FinishDate=@FinishDate,FinishDateUTC=@FinishDateUTC " +
                " WHERE WithdrawSerial=@WithdrawSerial";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
            DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
            DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;
            DBCmd.Parameters.Add("@FinishDateUTC", SqlDbType.DateTime).Value = DateTime.UtcNow;
            returnValue.Message = "审核完成";
            DBAccess.ExecuteDB(DBConnStr, DBCmd);
            returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);

            //如果不是后台申请提现,发送API回调
            if (WithdrawData.FloatType != 0)
            {
                ReSendWithdrawal(WithdrawData.WithdrawSerial, false);
            }

            return returnValue;
        }
        else
        {
            if (WithdrawType == 0)
            {
                returnValue.Message = "审核完成";
                returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                return returnValue;
            }
            else if (WithdrawType == 1)
            { //代付

                bool autoPay = false;
                var ProviderData = GetProviderCodeResult(WithdrawData.ProviderCode);
                if (ProviderData != null)
                {
                    var ProviderAPIType = ProviderData.First().ProviderAPIType;

                    if ((ProviderAPIType & 2) == 2)
                    {
                        autoPay = true;
                    }
                }
                if (autoPay)
                {
                    //api自動代付
                    string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
                    string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];
                    #region SignCheck
                    string strSign;
                    string sign;
                    APIResult returnRequireWithdrawal = null;

                    strSign = string.Format("WithdrawSerial={0}&GPayBackendKey={1}"
                    , WithdrawData.WithdrawSerial
                    , GPayBackendKey
                    );

                    sign = CodingControl.GetSHA256(strSign);

                    #endregion
                    var _RequireWithdrawalSet = new DBModel.RequireWithdrawalSet();
                    _RequireWithdrawalSet.WithdrawSerial = WithdrawData.WithdrawSerial;
                    _RequireWithdrawalSet.Sign = sign;

                    var strRequireWithdrawal = CodingControl.RequestJsonAPI(GPayApiUrl + "SendWithdraw", JsonConvert.SerializeObject(_RequireWithdrawalSet));

                    if (!string.IsNullOrEmpty(strRequireWithdrawal))
                    {
                        returnRequireWithdrawal = JsonConvert.DeserializeObject<APIResult>(strRequireWithdrawal);
                        //OK = 0,ERR = 1,SignErr = 2,Invalidate = 3(查無此單) //Success=4 (交易已完成)
                        returnValue.Message = returnRequireWithdrawal.Message;

                        if ((int)returnRequireWithdrawal.Status == 1 || (int)returnRequireWithdrawal.Status == 2 || (int)returnRequireWithdrawal.Status == 3)
                        {
                            UpdateWithdrawalStatus(WithdrawSerial, 0);
                            returnValue.Message = returnRequireWithdrawal.Message;
                            returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                            return returnValue;
                        }
                        else
                        {
                            returnValue.Message = "审核完成";
                            returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                            return returnValue;
                        }
                    }
                    else
                    {
                        UpdateWithdrawalStatus(WithdrawSerial, 0);
                        returnValue.Message = "API代付失败";
                        returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                        returnValue.Status = -4;
                        return returnValue;
                    }
                }
                else
                {
                    //更改訂單狀態回建立狀態
                    UpdateWithdrawalStatus(WithdrawSerial, 0);
                    returnValue.Message = "供应商尚未开启API代付";
                    returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                    returnValue.Status = -5;
                    return returnValue;
                }
            }
            else
            {
                //更改訂單狀態回建立狀態
                UpdateWithdrawalStatus(WithdrawSerial, 0);
                returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
                returnValue.Message = "订单状态错误";
                returnValue.Status = -3;
                return returnValue;
            }
        }

    }


    public int GetCompanyTypeByWithdrawSerial(string WithdrawSerial)
    {
        int ret = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DataTable DT;
        DBCmd = new System.Data.SqlClient.SqlCommand();
        object DBreturn;
        SS = " SELECT CompanyType From Withdrawal W JOIN CompanyTable CT ON CT.CompanyID=W.forCompanyID  " +
             " Where WithdrawSerial=@WithdrawSerial ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBreturn = DBAccess.GetDBValue(DBConnStr, DBCmd);
        if (DBreturn != null)
        {
            ret = int.Parse(DBreturn.ToString());
        }

        return ret;
    }

    public int ModifyCompanyServicePointByWithdrawal(int WithdrawID, string ServiceType, int CompanyID, string CurrencyType, decimal FinishtAmount)
    {
        int DBreturn = -9;
        String SS = String.Empty;
        SqlCommand DBCmd;
        SS = "spModifyCompanyServicePointByWithdrawal";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@FinishtAmount", SqlDbType.Decimal).Value = FinishtAmount;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);
        DBreturn = (int)DBCmd.Parameters["@Return"].Value;

        return DBreturn;
    }

    public DBViewModel.UpdateWithdrawalResult ConfirmManualWithdrawal(string WithdrawSerial, int modifyStatus, int AdminID)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.Withdrawal WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);

        if (WithdrawData.Status == 1 && WithdrawData.WithdrawType == 0)
        {
            //取消
            if (modifyStatus == 0)
            {
                UpdateWithdrawalStatus(WithdrawSerial, 0);
            }
            else if (modifyStatus == 2)
            {//成功

                //扣除公司額度
                SS = "spReviewWithdrawal";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                if (DBreturn == 0)
                {
                    //手動到後台下發
                    SS = " UPDATE Withdrawal  SET ConfirmByAdminID=@AdminID " +
                         " WHERE WithdrawSerial=@WithdrawSerial";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    returnValue.Message = "審核完成";
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    //如果不是后台申请提现,发送API回调
                    if (WithdrawData.FloatType != 0)
                    {
                        if (!(WithdrawData.DownUrl == "https://www.baidu.com/" || WithdrawData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawData.WithdrawSerial, false);
                        }
                    }
                }
                else
                {
                    UpdateWithdrawalStatus(WithdrawSerial, 0);
                    returnValue.Status = -6;
                    returnValue.Message = "扣除商戶額度失敗 " + DBreturn;
                }
            }
            else if (modifyStatus == 3)
            {//失败
                SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID,FinishDate=@FinishDate" +
                     " WHERE WithdrawSerial=@WithdrawSerial";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 3;
                DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;
                returnValue.Message = "審核完成";
                DBAccess.ExecuteDB(DBConnStr, DBCmd);

                //如果不是后台申请提现,发送API回调
                if (WithdrawData.FloatType != 0)
                {
                    if (!(WithdrawData.DownUrl == "https://www.baidu.com/" || WithdrawData.DownUrl == "http://baidu.com"))
                    {
                        ReSendWithdrawal(WithdrawData.WithdrawSerial, false);
                    }
                }
            }

        }
        else
        {
            returnValue.Message = "目前状态无法审核确认";
        }
        returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
        return returnValue;
    }

    public int spAddWithdrawalByBackendDownData(int WithdrawType, int forCompanyID, string ProviderCode, string CurrencyType, string ServiceType, decimal Amount, decimal CollectCharge, decimal CollectRate, decimal CostCharge, int Status, string BankCard, string BankCardName, string BankName, string BankBranchName, string OwnProvince, string OwnCity, int DownStatus, int FloatType,decimal DownBlockChainRate=0,decimal OKEXRate=0,int IsBlockChainOrder=0)
    {
        int DBreturn = -9;
        String SS = String.Empty;
        SqlCommand DBCmd;
        SS = "spAddWithdrawalByBackendDownData";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@WithdrawType", SqlDbType.Int).Value = WithdrawType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = forCompanyID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode == null ? "" : ProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
        DBCmd.Parameters.Add("@CollectCharge", SqlDbType.Decimal).Value = CollectCharge;
        DBCmd.Parameters.Add("@CollectRate", SqlDbType.Decimal).Value = CollectRate;
        DBCmd.Parameters.Add("@CostCharge", SqlDbType.Decimal).Value = CostCharge;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = BankCardName;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = BankName;
        DBCmd.Parameters.Add("@BankBranchName", SqlDbType.NVarChar).Value = BankBranchName;
        DBCmd.Parameters.Add("@OwnProvince", SqlDbType.NVarChar).Value = OwnProvince;
        DBCmd.Parameters.Add("@OwnCity", SqlDbType.NVarChar).Value = OwnCity;
        DBCmd.Parameters.Add("@DownStatus", SqlDbType.Int).Value = DownStatus;
        DBCmd.Parameters.Add("@FloatType", SqlDbType.Int).Value = FloatType;
        DBCmd.Parameters.Add("@IsBlockChainOrder", SqlDbType.Int).Value = IsBlockChainOrder;
        DBCmd.Parameters.Add("@OKEXRate", SqlDbType.Decimal).Value = OKEXRate;
        DBCmd.Parameters.Add("@DownBlockChainRate", SqlDbType.Decimal).Value = DownBlockChainRate;
        DBCmd.Parameters.Add(new SqlParameter("@WithdrawID", SqlDbType.Int) { Direction = ParameterDirection.Output });

        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);
        if ((int)DBCmd.Parameters["@RETURN"].Value == 0)
        {
            DBreturn = (int)DBCmd.Parameters["@WithdrawID"].Value;
            int WithdrawalID = DBreturn;
            if (WithdrawalID != 0)
                {
                    string WithdrawSerial = "OP" + System.DateTime.Now.ToString("yyyyMMddHHmm") + (new string('0', 10 - WithdrawalID.ToString().Length) + WithdrawalID.ToString());
                    SS = "UPDATE Withdrawal  SET WithdrawSerial=@WithdrawSerial " +
                    " WHERE WithdrawID=@WithdrawID";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawalID;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
            }

            return DBreturn;
        }
        else
        {
            return (int)DBCmd.Parameters["@RETURN"].Value;
        }
    }

    public DBViewModel.UpdateWithdrawalResult ConfirmManualWithdrawalForProxyProivder(int CompanyID, string WithdrawSerial, int modifyStatus, int AdminID, string BankDescription)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.Withdrawal WithdrawData = GetWithdrawalByWithdrawSerial2(WithdrawSerial);
        DBModel.ProxyProvider ProxyProviderModel;
        try
        {
            if (WithdrawData.Status == 1)
            {
                //取消
                if (modifyStatus == 0)
                {
                    UpdateWithdrawalStatusByWithdrawID(WithdrawData.WithdrawID, 0);
                }
                else if (modifyStatus == 2)
                {//成功
                    ProxyProviderModel = GetProxyProviderResult(WithdrawData.ProviderCode);
                    if (ProxyProviderModel == null)
                    {
                        returnValue.Status = -6;
                        returnValue.WithdrawalData = GetProviderWithdrawalByWithdrawID(WithdrawData.WithdrawID);
                        returnValue.Message = "尚未设定专属供应商费率,请联系系统商";
                        return returnValue;
                    }


                    if (GetProxyProviderOrderByOrderSerial(WithdrawSerial, 1) != null)
                    {
                        UpdateProxyProviderOrder(WithdrawSerial, 1, ProxyProviderModel.Charge, 0);
                    }

                    SS = " UPDATE Withdrawal  SET HandleByAdminID=@AdminID,BankDescription=@BankDescription " +
                      " WHERE WithdrawID=@WithdrawID";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawData.WithdrawID;
                    DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = BankDescription;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);

                    //扣除公司額度
                    SS = "spReviewProviderWithdrawal";
                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.StoredProcedure;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                    if (DBreturn == 0)
                    {

                        returnValue.Message = "審核完成";
                        //如果不是后台申请提现,发送API回调
                        if (WithdrawData.FloatType != 0)
                        {
                            if (!(WithdrawData.DownUrl == "https://www.baidu.com/" || WithdrawData.DownUrl == "http://baidu.com"))
                            {
                                ReSendWithdrawal(WithdrawData.WithdrawSerial, false);
                            }
                        }
                    }
                    else
                    {
                        returnValue.Status = -6;
                        InsertAdminOPLog(CompanyID, AdminID, 5, "专属供应商审核失败,单号:" + WithdrawData.WithdrawSerial + ",错误资讯:" + DBreturn, "");
                        returnValue.Message = "系统忙碌中,请稍后再试, " + DBreturn;
                    }
                }
                else if (modifyStatus == 3)
                {//失败
                    ProxyProviderModel = GetProxyProviderResult(WithdrawData.ProviderCode);
                    if (ProxyProviderModel == null)
                    {
                        returnValue.Status = -6;
                        returnValue.WithdrawalData = GetProviderWithdrawalByWithdrawID(WithdrawData.WithdrawID);
                        returnValue.Message = "尚未设定专属供应商费率,请联系系统商";
                        return returnValue;
                    }


                    if (GetProxyProviderOrderByOrderSerial(WithdrawSerial, 1) != null)
                    {
                        UpdateProxyProviderOrder(WithdrawSerial, 1, ProxyProviderModel.Charge, 0);
                    }

                    SS = " UPDATE Withdrawal  SET Status=@Status,HandleByAdminID=@AdminID,FinishDate=@FinishDate,BankDescription=@BankDescription " +
                         " WHERE WithdrawID=@WithdrawID";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 3;
                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                    DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawData.WithdrawID;
                    DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = BankDescription;
                    DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;
                    returnValue.Message = "審核完成";
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);

                    //如果不是后台申请提现,发送API回调
                    if (WithdrawData.FloatType != 0)
                    {
                        if (!(WithdrawData.DownUrl == "https://www.baidu.com/" || WithdrawData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawData.WithdrawSerial, false);
                        }
                    }
                }

            }
            else
            {
                returnValue.Status = -6;
                returnValue.Message = "目前状态无法审核确认";
            }
            returnValue.WithdrawalData = GetProviderWithdrawalByWithdrawID(WithdrawData.WithdrawID);
        }
        catch (Exception ex)
        {
            returnValue.Status = -6;
            returnValue.Message = "审核失败,资讯:" + ex.Message;
            InsertAdminOPLog(CompanyID, AdminID, 5, "专属供应商审核失败,单号:" + WithdrawData.WithdrawSerial + ",错误资讯:" + ex.Message, "");
            throw;
        }

        return returnValue;
    }

    public DBViewModel.UpdateWithdrawalResult ConfirmAutoWithdrawal(string WithdrawSerial)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        string strSign;
        string sign;
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        DBModel.Withdrawal WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);

        if (WithdrawData.Status == 1 && WithdrawData.WithdrawType == 1)
        {

            #region SignCheck

            //APIResult objReturnValue = null;
            APIResult objReturnValue = new APIResult();
            strSign = string.Format("WithdrawSerial={0}&GPayBackendKey={1}"
                                      , WithdrawSerial
                                      , GPayBackendKey
                                      );

            sign = CodingControl.GetSHA256(strSign);

            #endregion
            var _ReSendWithdraw = new DBModel.ReSendWithdrawSet();
            _ReSendWithdraw.WithdrawSerial = WithdrawSerial;
            _ReSendWithdraw.Sign = sign;

            var QueryWithdrawReturn = CodingControl.RequestJsonAPI(GPayApiUrl + "QueryWithdraw", JsonConvert.SerializeObject(_ReSendWithdraw));


            if (!string.IsNullOrEmpty(QueryWithdrawReturn))
            {
                var _APIResult = JsonConvert.DeserializeObject<APIResult>(QueryWithdrawReturn);
                if (_APIResult.Status == ResultStatus.OK)
                {
                    returnValue.Status = 0;
                }
                else
                {
                    returnValue.Status = -1;
                }
                returnValue.Message = _APIResult.Message;
            }
            else
            {
                returnValue.Status = -1;
                returnValue.Message = "查询失败";
            }
        }
        else
        {
            returnValue.Status = -1;
            returnValue.Message = "目前状态无法查询订单";
        }
        returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
        return returnValue;
    }

    public DBViewModel.UpdateWithdrawalResult QueryAPIWithdrawal(string WithdrawSerial)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        string strSign;
        string sign;
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        DBModel.Withdrawal WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);

        if (WithdrawData.Status == 1 && WithdrawData.WithdrawType == 1)
        {

            #region SignCheck

            //APIResult objReturnValue = null;
            APIResult objReturnValue = new APIResult();
            strSign = string.Format("WithdrawSerial={0}&GPayBackendKey={1}"
                                      , WithdrawSerial
                                      , GPayBackendKey
                                      );

            sign = CodingControl.GetSHA256(strSign);

            #endregion
            var _ReSendWithdraw = new DBModel.ReSendWithdrawSet();
            _ReSendWithdraw.WithdrawSerial = WithdrawSerial;
            _ReSendWithdraw.Sign = sign;

            var QueryWithdrawReturn = CodingControl.RequestJsonAPI(GPayApiUrl + "QueryWithdraw", JsonConvert.SerializeObject(_ReSendWithdraw));


            if (!string.IsNullOrEmpty(QueryWithdrawReturn))
            {
                var _APIResult = JsonConvert.DeserializeObject<APIResult>(QueryWithdrawReturn);
                if (_APIResult.Status == ResultStatus.OK)
                {
                    returnValue.Status = 0;
                }
                else
                {
                    returnValue.Status = -1;
                }

                returnValue.Message = _APIResult.Message;

            }
            else
            {
                returnValue.Status = -1;
                returnValue.Message = "查询失败,请联系工程师";
            }
        }
        else
        {
            returnValue.Status = -1;
            returnValue.Message = "目前状态无法查询订单";
        }
        returnValue.WithdrawalData = GetWithdrawalByWithdrawSerialByAdmin(WithdrawSerial);
        return returnValue;
    }

    public DBViewModel.UpdateWithdrawalResult UpdateWithdrawalResultByWithdrawSerialCheck(int Status, string WithdrawSerial, int AdminID)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);

        //失敗單
        if (Status == 3)
        {
            SS = " UPDATE  Withdrawal SET Status=@Status,ConfirmByAdminID=@AdminID,FinishDate=@FinishDate" +
                " WHERE WithdrawSerial=@WithdrawSerial";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
            DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
            DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;

            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

            if (DBreturn > 0)
            {
                SS = "Select Status from Withdrawal  WITH (NOLOCK)   " +
                     " WHERE WithdrawSerial=@WithdrawSerial";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBreturn = (int)DBAccess.GetDBValue(DBConnStr, DBCmd);
                returnValue.Message = "審核完成";
                returnValue.Status = DBreturn;

            }
        }
        else
        {
            //找不到訂單資訊
            if (WithdrawData != null && (WithdrawData.Status == 4))
            {
                if (WithdrawData.WithdrawType == 0 && Status == 5)
                {
                    //水池撥款
                    SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                 " WHERE WithdrawSerial=@WithdrawSerial";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 12;
                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;

                    DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                    if (DBreturn > 0)
                    {
                        SS = "Select Status from Withdrawal  WITH (NOLOCK)  " +
                             " WHERE WithdrawSerial=@WithdrawSerial";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                        DBreturn = (int)DBAccess.GetDBValue(DBConnStr, DBCmd);
                        returnValue.Message = "審核完成";
                        returnValue.Status = DBreturn;

                    }
                }
                else
                {
                    //代付
                    if (!string.IsNullOrEmpty(WithdrawData.ProviderCode) && WithdrawData.WithdrawType == 1)
                    {

                        bool autoPay = false;
                        var ProviderData = GetProviderCodeResult(WithdrawData.ProviderCode);
                        if (ProviderData != null)
                        {
                            var ProviderAPIType = ProviderData.First().ProviderAPIType;

                            if ((ProviderAPIType & 2) == 2)
                            {
                                autoPay = true;
                            }

                        }
                        if (autoPay)
                        {   //api自動代付
                            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                                 " WHERE WithdrawSerial=@WithdrawSerial";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 5;
                            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                            DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                            string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
                            string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];
                            #region SignCheck
                            string strSign;
                            string sign;
                            DBModel.API_WithdrawResult returnRequireWithdrawal = null;

                            strSign = string.Format("WithdrawID={0}&GPayBackendKey={1}"
                            , WithdrawData.WithdrawID
                            , GPayBackendKey
                            );

                            sign = CodingControl.GetSHA256(strSign);

                            #endregion
                            var _RequireWithdrawalSet = new DBModel.RequireWithdrawalSet();
                            _RequireWithdrawalSet.WithdrawSerial = WithdrawData.WithdrawSerial;
                            _RequireWithdrawalSet.Sign = sign;

                            var strRequireWithdrawal = CodingControl.RequestJsonAPI(GPayApiUrl + "RequireWithdrawal", JsonConvert.SerializeObject(_RequireWithdrawalSet));

                            if (!string.IsNullOrEmpty(strRequireWithdrawal))
                            {
                                returnRequireWithdrawal = JsonConvert.DeserializeObject<DBModel.API_WithdrawResult>(strRequireWithdrawal);
                                //OK = 0,ERR = 1,SignErr = 2,Invalidate = 3
                                switch (returnRequireWithdrawal.Status)
                                {
                                    case 0:
                                        SS = "Select Status from Withdrawal  WITH (NOLOCK)   " +
                                       " WHERE WithdrawSerial=@WithdrawSerial";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                        DBreturn = (int)DBAccess.GetDBValue(DBConnStr, DBCmd);
                                        returnValue.Message = "審核完成";
                                        returnValue.Status = DBreturn;
                                        return returnValue;

                                    case 1:
                                        SS = " UPDATE Withdrawal SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                             " WHERE WithdrawSerial=@WithdrawSerial";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                        DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                        returnValue.Message = returnRequireWithdrawal.Message;
                                        returnValue.Status = 7;
                                        return returnValue;
                                    case 2:
                                        SS = " UPDATE Withdrawal SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                             " WHERE WithdrawSerial=@WithdrawSerial";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                        DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                        returnValue.Message = returnRequireWithdrawal.Message;
                                        returnValue.Status = 7;
                                        return returnValue;
                                    case 3:
                                        SS = " UPDATE Withdrawal SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                              " WHERE WithdrawSerial=@WithdrawSerial";

                                        DBCmd = new System.Data.SqlClient.SqlCommand();
                                        DBCmd.CommandText = SS;
                                        DBCmd.CommandType = CommandType.Text;
                                        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                        DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = returnRequireWithdrawal.Message;

                                        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                        returnValue.Message = returnRequireWithdrawal.Message;
                                        returnValue.Status = 7;
                                        return returnValue;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                SS = " UPDATE Withdrawal SET Status=@Status,ConfirmByAdminID=@AdminID,ManagerRejectDescription=@ManagerRejectDescription" +
                                             " WHERE WithdrawSerial=@WithdrawSerial";

                                DBCmd = new System.Data.SqlClient.SqlCommand();
                                DBCmd.CommandText = SS;
                                DBCmd.CommandType = CommandType.Text;
                                DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 7;
                                DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = "API代付失敗";

                                DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
                                returnValue.Message = "API代付失敗";
                                returnValue.Status = 7;
                            }
                        }
                        else
                        {
                            //手動供應商後台撥款
                            SS = " UPDATE Withdrawal  SET Status=@Status,ConfirmByAdminID=@AdminID" +
                                 " WHERE WithdrawSerial=@WithdrawSerial";

                            DBCmd = new System.Data.SqlClient.SqlCommand();
                            DBCmd.CommandText = SS;
                            DBCmd.CommandType = CommandType.Text;
                            DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 12;
                            DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                            DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;

                            DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                            if (DBreturn > 0)
                            {
                                SS = "Select Status from Withdrawal  WITH (NOLOCK)  " +
                                     " WHERE WithdrawSerial=@WithdrawSerial";

                                DBCmd = new System.Data.SqlClient.SqlCommand();
                                DBCmd.CommandText = SS;
                                DBCmd.CommandType = CommandType.Text;
                                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                                DBreturn = (int)DBAccess.GetDBValue(DBConnStr, DBCmd);
                                returnValue.Message = "審核完成";
                                returnValue.Status = DBreturn;
                            }
                        }

                    }
                    else
                    {
                        returnValue.Message = "訂單狀態錯誤";
                        returnValue.Status = 7;
                    }
                }
            }
            else
            {
                returnValue.Message = "訂單狀態錯誤";
                returnValue.Status = 7;
            }
        }
        return returnValue;
    }

    public void UpdateWithdrawal(string WithdrawSerial, string ManagerRejectDescription)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET ManagerRejectDescription=@ManagerRejectDescription" +
                                            " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@ManagerRejectDescription", SqlDbType.NVarChar).Value = ManagerRejectDescription;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

    }

    public int UpdateWithdrawalStatus(string WithdrawSerial, int Status)
    {
        int DBreturn = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET Status=@Status" +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        return DBreturn;
    }

    public int UpdateWithdrawalStatusByWithdrawID(int WithdrawID, int Status)
    {
        int DBreturn = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET Status=@Status" +
             " WHERE WithdrawID=@WithdrawID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
        DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);
        return DBreturn;
    }

    public DBViewModel.UpdateWithdrawalResult UpdateWithdrawalResultByWithdrawSerialDoubleCheck(string WithdrawSerial)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var WithdrawData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        //找不到訂單資訊
        if (WithdrawData != null)
        {
            if (WithdrawData.Status == 6)
            {
                //扣除公司額度
                SS = "spReviewWithdrawal";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;
                returnValue = new DBViewModel.UpdateWithdrawalResult();
                switch (DBreturn)
                {
                    case 0://成功
                        returnValue.Message = "審核完成";
                        returnValue.Status = 2;
                        break;
                    case -1://交易單不存在
                        returnValue.Message = "交易單不存在";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "交易單不存在");
                        break;
                    case -2://營運商額度錯誤 
                        returnValue.Message = "營運商錢包金額錯誤";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額錯誤");
                        break;
                    case -3://營運商額度不足
                        returnValue.Message = "營運商錢包金額不足";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額不足");
                        break;
                    case -4://鎖定失敗
                        returnValue.Message = "鎖定失敗";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "鎖定失敗");
                        break;
                    case -5://加扣點失敗
                        returnValue.Message = "加扣點失敗";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                        break;
                    default://其他錯誤
                        returnValue.Message = "其他錯誤";
                        returnValue.Status = 3;
                        //UpdateWithdrawal(WithdrawSerial, "其他錯誤");
                        break;
                }
            }
            else if (WithdrawData.Status == 7)
            {
                SS = " UPDATE Withdrawal  SET Status=@Status,FinishDate=@FinishDate" +
                 " WHERE WithdrawSerial=@WithdrawSerial";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = 3;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@FinishDate", SqlDbType.DateTime).Value = DateTime.Now;
                DBreturn = DBAccess.ExecuteDB(DBConnStr, DBCmd);

                if (DBreturn > 0)
                {
                    SS = "Select Status from Withdrawal  WITH (NOLOCK)  " +
                         " WHERE WithdrawSerial=@WithdrawSerial";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    DBreturn = (int)DBAccess.GetDBValue(DBConnStr, DBCmd);
                    returnValue.Message = "審核完成";
                    returnValue.Status = DBreturn;
                }
            }
            else
            {
                returnValue.Message = "訂單狀態錯誤";
                returnValue.Status = -1;
                UpdateWithdrawal(WithdrawSerial, "訂單狀態錯誤");
            }

        }
        else
        {
            returnValue.Message = "訂單狀態錯誤";
            returnValue.Status = -1;

        }

        return returnValue;
    }

    //BackendDB層使用
    public DBModel.Withdrawal GetWithdrawalByWithdrawSerial(string WithdrawSerial)
    {
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " Select * from Withdrawal WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType  WITH (NOLOCK)  ON ServiceType.ServiceType=Withdrawal.ServiceType And ServiceType.CurrencyType=Withdrawal.CurrencyType" +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.Withdrawal GetWithdrawalByWithdrawSerial2(string WithdrawSerial)
    {
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " Select * from Withdrawal WITH (NOLOCK)  " +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().FirstOrDefault();
            }
        }

        return returnValue;
    }

    //BackendDB層使用
    private DBModel.Withdrawal GetWithdrawalByWithdrawID(int WithdrawID)
    {
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = "Select convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,* from Withdrawal  WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=Withdrawal.ServiceType And ServiceType.CurrencyType=Withdrawal.CurrencyType" +
             " WHERE WithdrawID=@WithdrawID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().FirstOrDefault();
            }
        }

        return returnValue;
    }

    public DBModel.Withdrawal GetWithdrawalByWithdrawSerialByAdmin(string WithdrawSerial)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        SS = " SELECT GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK) ON PPO.GroupID= PPG.GroupID  " +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBModel.Withdrawal> GetWithdrawalByWithdrawSerialsByAdmin(List<string> WithdrawSerial)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        List<DBModel.Withdrawal> returnValue = null;
        DataTable DT;

        var parameters = new string[WithdrawSerial.Count];

        DBCmd = new System.Data.SqlClient.SqlCommand();

        DBCmd.CommandType = CommandType.Text;
        for (int i = 0; i < WithdrawSerial.Count; i++)
        {
            parameters[i] = string.Format("@WithdrawSerial{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], WithdrawSerial[i]);
        }

        SS = string.Format(" SELECT GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK) ON PPO.GroupID= PPG.GroupID  " +
             " WHERE WithdrawSerial IN ({0})", string.Join(", ", parameters));

        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public int CancelCheckHandleByAdmin(string WithdrawSerial, int AdminID)
    {

        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET HandleByAdminID=0 " +
                " WHERE WithdrawSerial=@WithdrawSerial And Status=1 And HandleByAdminID=@AdminID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;
    }

    public int ConfirmModifyBankCrad(string WithdrawSerial, string BankDescription)
    {

        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE Withdrawal  SET BankDescription=@BankDescription " +
                " WHERE WithdrawSerial=@WithdrawSerial ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@BankDescription", SqlDbType.NVarChar).Value = BankDescription;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;
    }

    public int ConfirmModifyBankCradByPayment(string PaymentSerial, string PatchDescription)
    {

        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " UPDATE PaymentTable SET PatchDescription=@PatchDescription " +
                " WHERE PaymentSerial=@PaymentSerial ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@PatchDescription", SqlDbType.NVarChar).Value = PatchDescription;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;
    }

    public int CheckHandleByChangeGroupByHandleByAdminID(string WithdrawSerial)
    {

        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        //手動到後台下發
        SS = " SELECT COUNT(*) FROM Withdrawal " +
             " JOIN  ProxyProviderOrder  WITH (NOLOCK) ON WithdrawSerial=forOrderSerial And ProxyProviderOrder.Type=1 " +
             " WHERE WithdrawSerial=@WithdrawSerial And Status=1 And HandleByAdminID <> 0  ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public int CheckHandleByChangeGroup(string WithdrawSerial, int GroupID)
    {

        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        //手動到後台下發
        SS = " SELECT COUNT(*) FROM Withdrawal " +
             " JOIN  ProxyProviderOrder ON WithdrawSerial=forOrderSerial And ProxyProviderOrder.Type=1 " +
             " WHERE WithdrawSerial=@WithdrawSerial And Status=1 And GroupID=@GroupID And  HandleByAdminID=0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public int CheckHandleByChangeGroupByAdmin(string WithdrawSerial)
    {

        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        //手動到後台下發
        SS = " SELECT COUNT(*) FROM Withdrawal " +
             " JOIN  ProxyProviderOrder ON WithdrawSerial=forOrderSerial And ProxyProviderOrder.Type=1 " +
             " WHERE WithdrawSerial=@WithdrawSerial And Status=1  And  HandleByAdminID=0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public int CheckHandleByAdminGroup(string WithdrawSerial, int AdminID, int GroupID)
    {

        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM ProxyProviderOrder WITH (NOLOCK) " +
          " WHERE forOrderSerial=@WithdrawSerial And Type=1 And GroupID=@GroupID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.VarChar).Value = GroupID;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public int CheckHandleByAdmin(string WithdrawSerial, int AdminID)
    {

        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM Withdrawal WITH (NOLOCK) " +
           " WHERE WithdrawSerial=@WithdrawSerial And Status=1 And HandleByAdminID=0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public int ChangeWithdrawHandleByAdmin(string WithdrawSerial, int AdminID)
    {
        int returnValue = -1;
        String SS = String.Empty;
        SqlCommand DBCmd;
        //当前尚未有人审核

        SS = " UPDATE Withdrawal SET HandleByAdminID=@AdminID " +
                " WHERE WithdrawSerial=@WithdrawSerial And Status=1 And HandleByAdminID=0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
        returnValue = DBAccess.ExecuteDB(DBConnStr, DBCmd);

        return returnValue;
    }

    public DBModel.GetProviderWithdrawalByGroupAmount GetProviderWithdrawalByGroupAmount(int GroupID)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        DBModel.GetProviderWithdrawalByGroupAmount returnValue = null;
        DataTable DT;
        SS = " SELECT SUM(Amount) AS TotalAmount,COUNT(*) AS TotalCount  FROM Withdrawal " +
             " JOIN ProxyProviderOrder ON Withdrawal.WithdrawSerial = ProxyProviderOrder.forOrderSerial " +
             " AND ProxyProviderOrder.Type = 1" +
             " WHERE ProxyProviderOrder.GroupID = @GroupID" +
             " AND Withdrawal.Status = 1" +
             " AND Withdrawal.HandleByAdminID <> 0 ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.GetProviderWithdrawalByGroupAmount>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public DBModel.Withdrawal GetProviderWithdrawalByWithdrawSerial(string WithdrawSerial)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,HandleByAdminID,PPO.GroupID FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder  PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " WHERE WithdrawSerial=@WithdrawSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public DBModel.Withdrawal GetProviderWithdrawalByWithdrawID(int WithdrawID)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        DBModel.Withdrawal returnValue = null;
        DataTable DT;
        SS = " SELECT WithdrawalCharge,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,HandleByAdminID,PPO.GroupID FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN  ProxyProviderOrder  PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1" +
             " WHERE WithdrawID=@WithdrawID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WithdrawID", SqlDbType.Int).Value = WithdrawID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBModel.WithdrawalV2> GetWithdrawalV2(FromBody.WithdrawalSetV2 fromBody)
    {
        List<DBModel.WithdrawalV2> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        DBCmd = new System.Data.SqlClient.SqlCommand();

        SS = " WITH T";
        SS += " AS(";
        SS += " SELECT ProviderCode.DecimalPlaces,ProviderCode.WithdrawRate,PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC), 120) as CreateDate2,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC), 120) as FinishDate2,";
        SS += " Withdrawal.ProviderCode,Withdrawal.Status,Withdrawal.WithdrawSerial,Withdrawal.DownOrderID,Withdrawal.UpRate,Withdrawal.DownRate,";
        SS += " Withdrawal.WithdrawType,Withdrawal.BankCard,Withdrawal.BankCardName,Withdrawal.CurrencyType,Withdrawal.OKEXRate,Withdrawal.UpBlockChainRate,Withdrawal.DownBlockChainRate,";
        SS += " Withdrawal.BankName,Withdrawal.Amount,Withdrawal.OwnProvince,Withdrawal.BankBranchName,Withdrawal.IsBlockChainOrder,";
        SS += " Withdrawal.CollectCharge,Withdrawal.CreateDate,Withdrawal.FinishDate,Withdrawal.FloatType,";
        SS += " Withdrawal.OwnCity,Withdrawal.FinishAmount,Withdrawal.DownUrl,Withdrawal.DownStatus,Withdrawal.HandleByAdminID,Withdrawal.CompanyDescription";
        SS += " ,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) ";
        SS += " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID";
        SS += " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID";
        SS += " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID";
        SS += " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 ";
        SS += " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  ";
        if (!fromBody.IsSearchWaitReview)
        {
            if (fromBody.TimeType == "F")
            {
                SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";
            }
            else {
                SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";
            }
          
            #region 筛选条件
            //過濾資料
            if (fromBody.Status != 99)
            { //99代表取得所有資料
                SS += " And Status=@Status";
            }
            //供应商过滤
            if (fromBody.ProviderCode != "0")
            { //99代表取得所有資料
                SS += " And Withdrawal.ProviderCode=@ProviderCode";
            }

            //序號過濾
            if (!string.IsNullOrEmpty(fromBody.WithdrawSerial))
            {
                SS += " And (WithdrawSerial=@WithdrawSerial or DownOrderID=@WithdrawSerial) ";
            }

            //營運商過濾
            if (fromBody.CompanyID != 0)
            {
                SS += " And Withdrawal.forCompanyID=@CompanyID";
            }

            //群組選擇
            if (fromBody.GroupID != 0)
            {
                SS += " And PPO.GroupID=@GroupID";
            }
            #endregion
        }
        else
        {
            SS += " WHERE Status IN ({0}) AND (FloatType=0 OR FloatType=1) ";
            fromBody.LstStatus = new List<int>() { 0, 1, 13, 14 };
            var parameters = new string[fromBody.LstStatus.Count];
            for (int i = 0; i < fromBody.LstStatus.Count; i++)
            {
                parameters[i] = string.Format("@Status{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.LstStatus[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
        }

        if (!string.IsNullOrEmpty(fromBody.search.value))
        {
            SS += " And (Withdrawal.WithdrawSerial like '%'+@SearchFilter+'%' OR  Withdrawal.DownOrderID like '%'+@SearchFilter+'%'  OR  ProviderName like '%'+@SearchFilter+'%'  OR  CompanyName like '%'+@SearchFilter+'%'  OR  Withdrawal.BankCard like '%'+@SearchFilter+'%'  OR   Withdrawal.BankCard like '%'+@SearchFilter+'%' OR  Withdrawal.BankCardName like '%'+@SearchFilter+'%' OR  Withdrawal.Amount like '%'+@SearchFilter+'%' OR  Withdrawal.CollectCharge like '%'+@SearchFilter+'%' OR  AT1.RealName like '%'+@SearchFilter+'%'  OR  AT2.RealName like '%'+@SearchFilter+'%'  OR  Withdrawal.OwnCity like '%'+@SearchFilter+'%' OR  Withdrawal.OwnProvince like '%'+@SearchFilter+'%' OR  Withdrawal.BankBranchName like '%'+@SearchFilter+'%'  OR  Withdrawal.FinishAmount like '%'+@SearchFilter+'%'  ) ";
        }


        SS += " )";
        SS += " SELECT TotalCount = COUNT(1) OVER( ";
        SS += " ),T.* ";
        SS += " FROM T";
        #region 排序

        switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        {
            case "Status":
                SS += " Order By T.Status ";
                break;
            case "WithdrawSerial":
                SS += " Order By T.WithdrawSerial ";
                break;
            case "DownOrderID":
                SS += " Order By T.DownOrderID ";
                break;
            case "WithdrawType":
                SS += " Order By T.WithdrawType ";
                break;
            case "ProviderName":
                SS += " Order By T.ProviderName ";
                break;
            case "CompanyName":
                SS += " Order By T.CompanyName ";
                break;
            case "BankCard":
                SS += " Order By T.BankCard ";
                break;
            case "Amount":
                SS += " Order By T.Amount ";
                break;
            case "CollectCharge":
                SS += " Order By T.CollectCharge ";
                break;
            case "CreateDate":
                SS += " Order By T.CreateDate ";
                break;
            case "FinishDate":
                SS += " Order By T.FinishDate ";
                break;
            case "RealName2":
                SS += " Order By T.RealName2 ";
                break;
            case "OwnCity":
                SS += " Order By T.OwnCity ";
                break;
            case "FinishAmount":
                SS += " Order By T.FinishAmount ";
                break;
            case "FloatType":
                SS += " Order By T.FloatType ";
                break;
            default:
                break;
        }

        if (fromBody.order.First().dir == "asc")
        {
            SS += " ASC ";
        }
        else
        {
            SS += " DESC ";
        }
        #endregion

        SS += " OFFSET @pageNo ROWS ";
        SS += " FETCH NEXT @pageSize ROWS ONLY ";

        //SS = " SELECT PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
        //     " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
        //     " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
        //     " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
        //     " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN  ProxyProviderOrder PPO ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
        //     " LEFT JOIN  ProxyProviderGroup PPG ON PPO.GroupID= PPG.GroupID  " +
        //     " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";


        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = string.IsNullOrEmpty(fromBody.WithdrawSerial) ? "" : fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@SearchFilter", System.Data.SqlDbType.NVarChar).Value = string.IsNullOrEmpty(fromBody.search.value) ? "" : fromBody.search.value;
        DBCmd.Parameters.Add("@pageNo", System.Data.SqlDbType.Int).Value = fromBody.start;
        DBCmd.Parameters.Add("@pageSize", System.Data.SqlDbType.Int).Value = fromBody.length;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalV2>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.WithdrawalV2> GetWithdrawalV3(FromBody.WithdrawalSetV2 fromBody)
    {
        List<DBModel.WithdrawalV2> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        DBCmd = new System.Data.SqlClient.SqlCommand();

        SS = " WITH T";
        SS += " AS(";
        SS += " SELECT ProviderCode.DecimalPlaces,ProviderCode.WithdrawRate,PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,";
        SS += " Withdrawal.ProviderCode,Withdrawal.Status,Withdrawal.WithdrawSerial,Withdrawal.DownOrderID,";
        SS += " Withdrawal.WithdrawType,Withdrawal.BankCard,Withdrawal.BankCardName,Withdrawal.CurrencyType,";
        SS += " Withdrawal.BankName,Withdrawal.Amount,Withdrawal.OwnProvince,Withdrawal.BankBranchName,";
        SS += " Withdrawal.CollectCharge,Withdrawal.CreateDate,Withdrawal.FinishDate,Withdrawal.FloatType,";
        SS += " Withdrawal.OwnCity,Withdrawal.FinishAmount,Withdrawal.DownUrl,Withdrawal.DownStatus,Withdrawal.HandleByAdminID,Withdrawal.CompanyDescription";
        SS += " ,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) ";
        SS += " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID";
        SS += " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID";
        SS += " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID";
        SS += " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 ";
        SS += " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  ";
        if (!fromBody.IsSearchWaitReview)
        {
            SS += " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 AND CompanyTable.CompanyType<>4 ";

            #region 筛选条件
            //過濾資料
            if (fromBody.Status != 99)
            { //99代表取得所有資料
                SS += " And Status=@Status";
            }
            //供应商过滤
            if (fromBody.ProviderCode != "0")
            { //99代表取得所有資料
                SS += " And Withdrawal.ProviderCode=@ProviderCode";
            }

            //序號過濾
            if (!string.IsNullOrEmpty(fromBody.WithdrawSerial))
            {
                SS += " And (WithdrawSerial=@WithdrawSerial or DownOrderID=@WithdrawSerial) ";
            }

            //營運商過濾
            if (fromBody.CompanyID != 0)
            {
                SS += " And Withdrawal.forCompanyID=@CompanyID";
            }

            //群組選擇
            if (fromBody.GroupID != 0)
            {
                SS += " And PPO.GroupID=@GroupID";
            }
            #endregion
        }
        else
        {
            SS += " WHERE Status IN ({0}) AND (FloatType=0 OR FloatType=1) And CompanyTable.CompanyType<>4 ";
            fromBody.LstStatus = new List<int>() { 0, 1, 13, 14 };
            var parameters = new string[fromBody.LstStatus.Count];
            for (int i = 0; i < fromBody.LstStatus.Count; i++)
            {
                parameters[i] = string.Format("@Status{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.LstStatus[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
        }

        if (!string.IsNullOrEmpty(fromBody.search.value))
        {
            SS += " And (Withdrawal.WithdrawSerial like '%'+@SearchFilter+'%' OR  Withdrawal.DownOrderID like '%'+@SearchFilter+'%'  OR  ProviderName like '%'+@SearchFilter+'%'  OR  CompanyName like '%'+@SearchFilter+'%'  OR  Withdrawal.BankCard like '%'+@SearchFilter+'%'  OR   Withdrawal.BankCard like '%'+@SearchFilter+'%' OR  Withdrawal.BankCardName like '%'+@SearchFilter+'%' OR  Withdrawal.Amount like '%'+@SearchFilter+'%' OR  Withdrawal.CollectCharge like '%'+@SearchFilter+'%' OR  AT1.RealName like '%'+@SearchFilter+'%'  OR  AT2.RealName like '%'+@SearchFilter+'%'  OR  Withdrawal.OwnCity like '%'+@SearchFilter+'%' OR  Withdrawal.OwnProvince like '%'+@SearchFilter+'%' OR  Withdrawal.BankBranchName like '%'+@SearchFilter+'%'  OR  Withdrawal.FinishAmount like '%'+@SearchFilter+'%'  ) ";
        }


        SS += " )";
        SS += " SELECT TotalCount = COUNT(1) OVER( ";
        SS += " ),T.* ";
        SS += " FROM T";
        #region 排序

        switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        {
            case "Status":
                SS += " Order By T.Status ";
                break;
            case "WithdrawSerial":
                SS += " Order By T.WithdrawSerial ";
                break;
            case "DownOrderID":
                SS += " Order By T.DownOrderID ";
                break;
            case "WithdrawType":
                SS += " Order By T.WithdrawType ";
                break;
            case "ProviderName":
                SS += " Order By T.ProviderName ";
                break;
            case "CompanyName":
                SS += " Order By T.CompanyName ";
                break;
            case "BankCard":
                SS += " Order By T.BankCard ";
                break;
            case "Amount":
                SS += " Order By T.Amount ";
                break;
            case "CollectCharge":
                SS += " Order By T.CollectCharge ";
                break;
            case "CreateDate":
                SS += " Order By T.CreateDate ";
                break;
            case "FinishDate":
                SS += " Order By T.FinishDate ";
                break;
            case "RealName2":
                SS += " Order By T.RealName2 ";
                break;
            case "OwnCity":
                SS += " Order By T.OwnCity ";
                break;
            case "FinishAmount":
                SS += " Order By T.FinishAmount ";
                break;
            case "FloatType":
                SS += " Order By T.FloatType ";
                break;
            default:
                break;
        }

        if (fromBody.order.First().dir == "asc")
        {
            SS += " ASC ";
        }
        else
        {
            SS += " DESC ";
        }
        #endregion

        SS += " OFFSET @pageNo ROWS ";
        SS += " FETCH NEXT @pageSize ROWS ONLY ";

        //SS = " SELECT PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
        //     " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
        //     " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
        //     " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
        //     " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN  ProxyProviderOrder PPO ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
        //     " LEFT JOIN  ProxyProviderGroup PPG ON PPO.GroupID= PPG.GroupID  " +
        //     " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";


        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = string.IsNullOrEmpty(fromBody.WithdrawSerial) ? "" : fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@SearchFilter", System.Data.SqlDbType.NVarChar).Value = string.IsNullOrEmpty(fromBody.search.value) ? "" : fromBody.search.value;

        DBCmd.Parameters.Add("@pageNo", System.Data.SqlDbType.Int).Value = fromBody.start;
        DBCmd.Parameters.Add("@pageSize", System.Data.SqlDbType.Int).Value = fromBody.length;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalV2>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBModel.WithdrawalV2TotalAmount GetWithdrawalBySearchFilter(FromBody.WithdrawalSetV2 fromBody,bool selectAllCompany)
    {
        DBModel.WithdrawalV2TotalAmount returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        DBCmd = new System.Data.SqlClient.SqlCommand();

        //SS = " WITH T";
        //SS += " AS(";
        SS += " SELECT SUM(Amount) TotalAmount,SUM(CASE WHEN Status=2 THEN CASE WHEN ProviderCode.DecimalPlaces=1 THEN floor((Amount*Withdrawal.DownRate*0.01)+Withdrawal.CollectCharge) ELSE (Amount*Withdrawal.DownRate*0.01)+Withdrawal.CollectCharge END ELSE 0 END) TotalHandlingFee FROM Withdrawal WITH (NOLOCK) ";
        //SS += " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID";
        //SS += " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID";
        SS += " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID";
        //SS += " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode";
        SS += " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 ";
        //SS += " LEFT JOIN  ProxyProviderGroup PPG ON PPO.GroupID= PPG.GroupID  ";
        if (!fromBody.IsSearchWaitReview)
        {
            if (fromBody.TimeType == "F")
            {
                SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) >= @StartDate And DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";
            }
            else {
                SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) >= @StartDate And DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";
            }
        

            #region 筛选条件
            //過濾資料
            if (fromBody.Status != 99)
            { //99代表取得所有資料
                SS += " And Status=@Status";
            }
            //供应商过滤
            if (fromBody.ProviderCode != "0")
            { //99代表取得所有資料
                SS += " And Withdrawal.ProviderCode=@ProviderCode";
            }

            //序號過濾
            if (!string.IsNullOrEmpty(fromBody.WithdrawSerial))
            {
                SS += " And (WithdrawSerial=@WithdrawSerial or DownOrderID=@WithdrawSerial) ";
            }

            //營運商過濾
            if (fromBody.CompanyID != 0)
            {
                SS += " And Withdrawal.forCompanyID=@CompanyID";
            }

            //群組選擇
            if (fromBody.GroupID != 0)
            {
                SS += " And PPO.GroupID=@GroupID";
            }
            #endregion
        }
        else
        {
            SS += " WHERE Status IN ({0}) AND (FloatType=0 OR FloatType=1) ";
            fromBody.LstStatus = new List<int>() { 0, 1, 13, 14 };
            var parameters = new string[fromBody.LstStatus.Count];
            for (int i = 0; i < fromBody.LstStatus.Count; i++)
            {
                parameters[i] = string.Format("@Status{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.LstStatus[i]);
            }

            SS = string.Format(SS, string.Join(", ", parameters));
        }

        if (!selectAllCompany)
        {
            SS+= " AND CompanyTable.CompanyType<>4 ";
        }
        //if (!string.IsNullOrEmpty(fromBody.search.value))
        //{
        //    SS += " And (Withdrawal.WithdrawSerial like '%'+@SearchFilter+'%' OR  Withdrawal.DownOrderID like '%'+@SearchFilter+'%'  OR  ProviderName like '%'+@SearchFilter+'%'  OR  CompanyName like '%'+@SearchFilter+'%'  OR  Withdrawal.BankCard like '%'+@SearchFilter+'%'  OR   Withdrawal.BankCard like '%'+@SearchFilter+'%' OR  Withdrawal.BankCardName like '%'+@SearchFilter+'%' OR  Withdrawal.Amount like '%'+@SearchFilter+'%' OR  Withdrawal.CollectCharge like '%'+@SearchFilter+'%' OR  AT1.RealName like '%'+@SearchFilter+'%'  OR  AT2.RealName like '%'+@SearchFilter+'%'  OR  Withdrawal.OwnCity like '%'+@SearchFilter+'%' OR  Withdrawal.OwnProvince like '%'+@SearchFilter+'%' OR  Withdrawal.BankBranchName like '%'+@SearchFilter+'%'  OR  Withdrawal.FinishAmount like '%'+@SearchFilter+'%'  ) ";
        //}


        //SS += " )";
        //SS += " SELECT TotalCount = COUNT(1) OVER( ";
        //SS += " ),T.* ";
        //SS += " FROM T";
        //#region 排序

        //switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        //{
        //    case "Status":
        //        SS += " Order By T.Status ";
        //        break;
        //    case "WithdrawSerial":
        //        SS += " Order By T.WithdrawSerial ";
        //        break;
        //    case "DownOrderID":
        //        SS += " Order By T.DownOrderID ";
        //        break;
        //    case "WithdrawType":
        //        SS += " Order By T.WithdrawType ";
        //        break;
        //    case "ProviderName":
        //        SS += " Order By T.ProviderName ";
        //        break;
        //    case "CompanyName":
        //        SS += " Order By T.CompanyName ";
        //        break;
        //    case "BankCard":
        //        SS += " Order By T.BankCard ";
        //        break;
        //    case "Amount":
        //        SS += " Order By T.Amount ";
        //        break;
        //    case "CollectCharge":
        //        SS += " Order By T.CollectCharge ";
        //        break;
        //    case "CreateDate":
        //        SS += " Order By T.CreateDate ";
        //        break;
        //    case "FinishDate":
        //        SS += " Order By T.FinishDate ";
        //        break;
        //    case "RealName2":
        //        SS += " Order By T.RealName2 ";
        //        break;
        //    case "OwnCity":
        //        SS += " Order By T.OwnCity ";
        //        break;
        //    case "FinishAmount":
        //        SS += " Order By T.FinishAmount ";
        //        break;
        //    case "FloatType":
        //        SS += " Order By T.FloatType ";
        //        break;
        //    default:
        //        break;
        //}

        //if (fromBody.order.First().dir == "asc")
        //{
        //    SS += " ASC ";
        //}
        //else
        //{
        //    SS += " DESC ";
        //}
        //#endregion

        //SS += " OFFSET @pageNo ROWS ";
        //SS += " FETCH NEXT @pageSize ROWS ONLY ";

        //SS = " SELECT PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
        //     " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
        //     " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
        //     " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
        //     " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
        //     " LEFT JOIN  ProxyProviderOrder PPO ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
        //     " LEFT JOIN  ProxyProviderGroup PPG ON PPO.GroupID= PPG.GroupID  " +
        //     " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 ";


        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = string.IsNullOrEmpty(fromBody.WithdrawSerial) ? "" : fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);
        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawalV2TotalAmount>(DT).ToList().First();
            }
        }
       
        return returnValue;
    }



    public List<DBModel.Withdrawal> GetWithdrawalAdminTableResult2(FromBody.WithdrawalSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ProviderCode.WithdrawRate,ProviderCode.DecimalPlaces,PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 AND Status <> 90 AND Status <> 91 And CompanyTable.CompanyType<>4 ";

        //過濾資料
        if (fromBody.Status != 99)
        { //99代表取得所有資料
            SS += " And Status=@Status";
        }
        //供应商过滤
        if (fromBody.ProviderCode != "0")
        { //99代表取得所有資料
            SS += " And Withdrawal.ProviderCode=@ProviderCode";
        }

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And (WithdrawSerial=@WithdrawSerial or DownOrderID=@WithdrawSerial) ";
        }

        //營運商過濾
        if (fromBody.CompanyID != 0)
        {
            SS += " And Withdrawal.forCompanyID=@CompanyID";
        }

        //群組選擇
        if (fromBody.GroupID != 0)
        {
            SS += " And PPO.GroupID=@GroupID";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    //提現審核 提現審核(主管) 出納匯款
    public List<DBModel.Withdrawal> GetWithdrawalAdminTableResult(FromBody.WithdrawalSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ProviderCode.WithdrawRate,ProviderCode.DecimalPlaces,PPG.GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC), 120) as CreateDate2,convert(varchar, DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC), 120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  ";

        if (fromBody.TimeType == "F")
        {
            SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.FinishDateUTC) <= @EndDate  And Status<>8 AND Status <> 90 AND Status <> 91  ";
        }
        else {
            SS += " WHERE DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, Withdrawal.CreateDateUTC) <= @EndDate  And Status<>8 AND Status <> 90 AND Status <> 91  ";
        }

        //過濾資料
        if (fromBody.Status != 99)
        { //99代表取得所有資料
            SS += " And Status=@Status";
        }
        //供应商过滤
        if (fromBody.ProviderCode != "0")
        { //99代表取得所有資料
            SS += " And Withdrawal.ProviderCode=@ProviderCode";
        }

        //序號過濾
        if (fromBody.WithdrawSerial != "")
        {
            SS += " And (WithdrawSerial=@WithdrawSerial or DownOrderID=@WithdrawSerial) ";
        }

        //營運商過濾
        if (fromBody.CompanyID != 0)
        {
            SS += " And Withdrawal.forCompanyID=@CompanyID";
        }

        //群組選擇
        if (fromBody.GroupID != 0)
        {
            SS += " And PPO.GroupID=@GroupID";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    //提現審核 提現審核(主管) 出納匯款
    public List<DBModel.Withdrawal> GetWithdrawalTableResultByLstStatus(FromBody.WithdrawalSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        var parameters = new string[fromBody.LstStatus.Count];

        SS = " SELECT GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             " WHERE Status IN ({0}) AND (FloatType=0 OR FloatType=1) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();

        for (int i = 0; i < fromBody.LstStatus.Count; i++)
        {
            parameters[i] = string.Format("@Status{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], fromBody.LstStatus[i]);
        }

        SS = string.Format(SS, string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.Withdrawal> GetWithdrawalTableResultByLstWithdrawID(FromBody.WithdrawalSet fromBody)
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        var parameters = new string[fromBody.WithdrawIDs.Count];

        SS = " SELECT GroupName,ProxyProvider.forProviderCode,ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
             " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
             " LEFT JOIN ProxyProvider WITH (NOLOCK) ON ProxyProvider.forProviderCode=Withdrawal.ProviderCode" +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= Withdrawal.WithdrawSerial AND PPO.Type=1 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             " WHERE WithdrawID IN ({0}) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();

        for (int i = 0; i < fromBody.WithdrawIDs.Count; i++)
        {
            parameters[i] = string.Format("@WithdrawID{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], fromBody.WithdrawIDs[i]);
        }

        SS = string.Format(SS, string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }
    //未使用
    //public List<DBModel.Withdrawal> GetetWithdrawalAdminTableResultByCashier(FromBody.WithdrawalSet fromBody)
    //{
    //    List<DBModel.Withdrawal> returnValue = null;
    //    String SS = String.Empty;
    //    SqlCommand DBCmd;
    //    DataTable DT;

    //    SS = " SELECT ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,AT1.RealName as RealName1,AT2.RealName as RealName2,CompanyName FROM Withdrawal WITH (NOLOCK) " +
    //         " LEFT JOIN AdminTable AT1 WITH (NOLOCK) ON AT1.AdminID=Withdrawal.HandleByAdminID" +
    //         " LEFT JOIN AdminTable AT2 WITH (NOLOCK) ON AT2.AdminID=Withdrawal.ConfirmByAdminID" +
    //         " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
    //         " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
    //         " WHERE Withdrawal.CreateDate >= @StartDate And Withdrawal.CreateDate <= @EndDate And Status<>8 And Status<>0 ";
    //    //And Status<>1 And Status<>2 And Status<>3 And Status<>4 And Status<>5 And Status<>9 And Status<>10 And Status<>13
    //    //過濾資料
    //    if (fromBody.Status != 99)
    //    { //99代表取得所有資料
    //        SS += " And Status=@Status";
    //    }

    //    //序號過濾
    //    if (fromBody.WithdrawSerial != "")
    //    {
    //        SS += " And WithdrawSerial=@WithdrawSerial";
    //    }

    //    //營運商過濾
    //    if (fromBody.CompanyID != 0)
    //    {
    //        SS += " And forCompanyID=@CompanyID";
    //    }

    //    DBCmd = new System.Data.SqlClient.SqlCommand();
    //    DBCmd.CommandText = SS;
    //    DBCmd.CommandType = CommandType.Text;
    //    DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
    //    DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
    //    DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
    //    DBCmd.Parameters.Add("@Status", SqlDbType.Int).Value = fromBody.Status;
    //    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = fromBody.WithdrawSerial;

    //    DT = DBAccess.GetDB(DBConnStr, DBCmd);

    //    if (DT != null)
    //    {
    //        if (DT.Rows.Count > 0)
    //        {
    //            returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
    //        }
    //    }

    //    return returnValue;
    //}

    #endregion

    #region GPayRelation
    public System.Data.DataTable GetTopParentGPayRelation(int CompanyID, string ServiceType, string CurrencyType, string SortKey)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;


        SS = "SELECT @CompanyID AS forCompanyID, G.ProviderCode, G.ServiceType, G.CurrencyType, G.Weight "
           + " FROM CompanyTable C "
           + " LEFT JOIN GPayRelation G ON C.CompanyID = G.forCompanyID "
           + " WHERE @SortKey LIKE C.SortKey + '%' "
           + " AND C.InsideLevel = 0 "
           + " AND G.ServiceType = @ServiceType "
           + " AND G.CurrencyType = @CurrencyType ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@SortKey", System.Data.SqlDbType.VarChar).Value = SortKey;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        return DT;
    }

    public System.Data.DataTable GetGPayRelation(int CompanyID, string ServiceType, string CurrencyType)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;

        SS = "SELECT * FROM GPayRelation WITH (NOLOCK) WHERE forCompanyID=@CompanyID AND ServiceType=@ServiceType AND CurrencyType=@CurrencyType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        return DT;
    }

    public List<DBViewModel.ProviderServiceVM> GetGPayRelationByCompany(int forCompanyID, string ServiceType, string CurrencyType)
    {
        List<DBViewModel.ProviderServiceVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT ProviderCode.ProviderName,GPayRelation.Weight,ProviderService.* FROM GPayRelation WITH (NOLOCK)" +
             " LEFT JOIN ProviderService WITH (NOLOCK) ON GPayRelation.ProviderCode= ProviderService.ProviderCode And GPayRelation.ServiceType= ProviderService.ServiceType And GPayRelation.CurrencyType= ProviderService.CurrencyType " +
              " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode= ProviderService.ProviderCode " +
             " WHERE GPayRelation.forCompanyID =@forCompanyID And GPayRelation.ServiceType =@ServiceType And GPayRelation.CurrencyType =@CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = forCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderServiceVM>(DT) as List<DBViewModel.ProviderServiceVM>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.GPayRelationResult> GetGPayRelationTableResult(string ProviderCode)
    {
        List<DBViewModel.GPayRelationResult> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT GPayRelation.*,CompanyTable.CompanyName,ServiceTypeName FROM GPayRelation WITH (NOLOCK)" +
             " LEFT JOIN CompanyTable WITH (NOLOCK) ON GPayRelation.forCompanyID= CompanyTable.CompanyID" +
             " LEFT JOIN ServiceType WITH (NOLOCK) ON ServiceType.ServiceType= GPayRelation.ServiceType" +
             " WHERE ProviderCode =@ProviderCode ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.GPayRelationResult>(DT) as List<DBViewModel.GPayRelationResult>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.GPayRelationResult> GetGPayRelationTableResultByServiceType(string ServiceType)
    {
        List<DBViewModel.GPayRelationResult> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = "SELECT GPayRelation.*,CompanyTable.CompanyName,ServiceTypeName,ProviderName FROM GPayRelation WITH (NOLOCK)" +
            " JOIN CompanyTable WITH (NOLOCK) ON GPayRelation.forCompanyID= CompanyTable.CompanyID" +
            " JOIN ServiceType WITH (NOLOCK) ON ServiceType.ServiceType= GPayRelation.ServiceType" +
            " JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode= GPayRelation.ProviderCode" +
            " WHERE GPayRelation.ServiceType =@ServiceType ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.GPayRelationResult>(DT) as List<DBViewModel.GPayRelationResult>;
            }
        }

        return returnValue;
    }

    public List<DBModel.GPayRelation> GetGPayRelationResult(string ServiceType, string CurrencyType, string ProviderCode = "", int forCompanyID = 0)
    {
        List<DBModel.GPayRelation> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        SS = " SELECT GPayRelation.*,ProviderName FROM GPayRelation WITH (NOLOCK) " +
             " JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=GPayRelation.ProviderCode " +
             " WHERE  GPayRelation.ServiceType =@ServiceType AND GPayRelation.CurrencyType =@CurrencyType ";

        if (ProviderCode != "")
        {
            SS += " AND GPayRelation.ProviderCode=@ProviderCode";
        }
        else
        {
            SS += " AND GPayRelation.forCompanyID=@forCompanyID";
        }

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        if (ProviderCode != "")
        {
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        }
        else
        {
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = forCompanyID;
        }
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.GPayRelation>(DT) as List<DBModel.GPayRelation>;
            }
        }

        return returnValue;
    }

    public List<DBModel.GPayRelation> GetGPayRelationResultByCurrencyTypeAndforCompanyID(string CurrencyType, int forCompanyID)
    {
        List<DBModel.GPayRelation> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        SS = " SELECT * FROM GPayRelation WHERE  CurrencyType=@CurrencyType" +
             " AND forCompanyID=@forCompanyID";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = forCompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.GPayRelation>(DT) as List<DBModel.GPayRelation>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ProviderServiceVM> GetProviderServiceGPayRelationByCompany(int CompanyID, string ServiceType, string CurrencyType,int CompanyType)
    {
        List<DBViewModel.ProviderServiceVM> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable ProviderServiceDT;
        DataTable GPayRelationDT;

        SS = " SELECT ProviderService.*,ProviderName FROM ProviderService WITH (NOLOCK) " +
             " JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=ProviderService.ProviderCode " +
             " WHERE ServiceType = @ServiceType AND CurrencyType =@CurrencyType And ProviderCode.ProviderState=0";
        if (CompanyType == 4)
        {
            SS += " And ProviderCode.forCompanyID=@CompanyID";
        }
       
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        ProviderServiceDT = DBAccess.GetDB(DBConnStr, DBCmd);

        SS = "SELECT * FROM GPayRelation WHERE forCompanyID =@CompanyID AND ServiceType = @ServiceType AND CurrencyType =@CurrencyType";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        GPayRelationDT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (ProviderServiceDT != null)
        {
            if (ProviderServiceDT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ProviderServiceVM>(ProviderServiceDT).ToList();
                foreach (var rowData in returnValue)
                {
                    if (GPayRelationDT.Select("ProviderCode='" + rowData.ProviderCode + "'").Count() > 0)
                    {
                        rowData.selectedProviderService = true;
                        rowData.Weight = GPayRelationDT.Select("ProviderCode='" + rowData.ProviderCode + "'").First().Field<int>("Weight");
                    }
                    else
                    {
                        rowData.selectedProviderService = false;
                        rowData.Weight = 1;
                    }
                }
            }
        }

        return returnValue;
    }

    public int InsertGPayRelation(DBModel.GPayRelation Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO GPayRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID) " +
         "                          VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.GPayRelation.UpdateGPayRelation(Model.forCompanyID, Model.ServiceType, Model.CurrencyType);
        return returnValue;
    }

    public int DeleteGPayRelation(DBModel.GPayRelation Model)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "DELETE FROM GPayRelation WHERE  ServiceType =@ServiceType AND CurrencyType =@CurrencyType AND forCompanyID =@forCompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        RedisCache.GPayRelation.DeleteGPayRelation(Model.forCompanyID, Model.ServiceType, Model.CurrencyType);
        return returnValue;
    }
    #endregion

    #region GPayWithdrawRelation
    public List<DBModel.WithdrawLimit> GetCompanyWithdrawRelationResult(int CompanyID)
    {
        List<DBModel.WithdrawLimit> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT WithdrawLimit.* FROM WithdrawLimit WITH (NOLOCK)" +
             " WHERE forCompanyID =@CompanyID And WithdrawLimitType =2 ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.WithdrawLimit>(DT) as List<DBModel.WithdrawLimit>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.GPayWithdrawRelation> GetGPayWithdrawRelationByCompanyID(int CompanyID)
    {
        List<DBViewModel.GPayWithdrawRelation> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT GPayWithdrawRelation.*,ProviderName FROM GPayWithdrawRelation WITH (NOLOCK)" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode= GPayWithdrawRelation.ProviderCode" +
             " WHERE GPayWithdrawRelation.forCompanyID =@CompanyID ";

        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.GPayWithdrawRelation>(DT) as List<DBViewModel.GPayWithdrawRelation>;
            }
        }

        return returnValue;
    }

    public List<DBViewModel.ApiWithdrawLimit> GetApiWithdrawLimitByProviderCompanyID(int CompanyID,int ProviderCompanyID)
    {
        List<DBViewModel.ApiWithdrawLimit> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        List<DBViewModel.GPayWithdrawRelation> CompanyWithdrawRelationModel = null;
        SS = " SELECT WithdrawLimit.*,ProviderName FROM WithdrawLimit WITH (NOLOCK)" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode= WithdrawLimit.ProviderCode" +
             " WHERE WithdrawLimitType =0 And (ProviderCode.ProviderAPIType & 2) = 2 And ProviderCode.ProviderState=0 And ProviderCode.forCompanyID=@ProviderCompanyID";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCompanyID", SqlDbType.Int).Value = ProviderCompanyID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ApiWithdrawLimit>(DT) as List<DBViewModel.ApiWithdrawLimit>;

                if (CompanyID != 0)
                {
                    CompanyWithdrawRelationModel = GetGPayWithdrawRelationByCompanyID(CompanyID);
                    if (CompanyWithdrawRelationModel != null && CompanyWithdrawRelationModel.Count > 0)
                    {
                        for (int i = 0; i < returnValue.Count; i++)
                        {
                            if (CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).Count() > 0)
                            {
                                returnValue[i].selectedWithdrawLimit = true;
                                returnValue[i].Weight = CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).First().Weight;
                                returnValue[i].WithdrawType = CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).First().WithdrawType;
                            }
                            else
                            {
                                returnValue[i].Weight = 1;
                                returnValue[i].selectedWithdrawLimit = false;
                            }
                        }
                    }
                }
            }
        }
        return returnValue;
    }

    public List<DBViewModel.ApiWithdrawLimit> GetApiWithdrawLimit(int CompanyID)
    {
        List<DBViewModel.ApiWithdrawLimit> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;
        List<DBViewModel.GPayWithdrawRelation> CompanyWithdrawRelationModel = null;
        SS = " SELECT WithdrawLimit.*,ProviderName FROM WithdrawLimit WITH (NOLOCK)" +
             " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode= WithdrawLimit.ProviderCode" +
             " WHERE WithdrawLimitType =0 And (ProviderCode.ProviderAPIType & 2) = 2 And ProviderCode.ProviderState=0";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.ApiWithdrawLimit>(DT) as List<DBViewModel.ApiWithdrawLimit>;

                if (CompanyID != 0)
                {
                    CompanyWithdrawRelationModel = GetGPayWithdrawRelationByCompanyID(CompanyID);
                    if (CompanyWithdrawRelationModel != null && CompanyWithdrawRelationModel.Count > 0)
                    {
                        for (int i = 0; i < returnValue.Count; i++)
                        {
                            if (CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).Count() > 0)
                            {
                                returnValue[i].selectedWithdrawLimit = true;
                                returnValue[i].Weight = CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).First().Weight;
                                returnValue[i].WithdrawType = CompanyWithdrawRelationModel.Where(w => w.ProviderCode == returnValue[i].ProviderCode).First().WithdrawType;
                            }
                            else
                            {
                                returnValue[i].Weight = 1;
                                returnValue[i].selectedWithdrawLimit = false;
                            }
                        }
                    }
                }
            }
        }
        return returnValue;
    }

    public int InsertGPayWithdrawRelation(FromBody.GPayWithdrawRelationSet fromBody, int CompanyType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        SS = " SELECT COUNT(*) FROM WithdrawLimit WITH (NOLOCK) " +
             " WHERE WithdrawLimitType=2 And CurrencyType=@CurrencyType  And forCompanyID=@CompanyID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
        //資料重複
        if ((int)DBAccess.GetDBValue(DBConnStr, DBCmd) > 0)
        {
            returnValue = -1;
            return returnValue;
        }

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);

        DBAccess.ExecuteTransDB(DBConnStr, T =>
        {

            SS = " INSERT INTO WithdrawLimit (WithdrawLimitType,CurrencyType,forCompanyID,MaxLimit,MinLimit,Charge,Rate)" +
           " VALUES(2,@CurrencyType,@forCompanyID,@MaxLimit,@MinLimit,@Charge,@Rate)";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
            DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = fromBody.MaxLimit;
            DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = fromBody.MinLimit;
            DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = fromBody.Charge;
            DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = fromBody.Rate;
            returnValue = T.ExecuteDB(DBCmd);
            RedisCache.CompanyWithdrawLimit.UpdateCompanyAPIWithdrawLimit(fromBody.CompanyID, fromBody.CurrencyType);


            //新增 GPayRelation
            if (returnValue > 0)
            {
                //if (CompanyType == 0 && CompanyTD.ParentCompanyID == 0)
                //{
                //先刪除舊有資料
                SS = "DELETE FROM GPayWithdrawRelation WHERE forCompanyID =@forCompanyID  And CurrencyType=@CurrencyType";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                T.ExecuteDB(DBCmd);
                RedisCache.GPayWithdrawRelation.DeleteGPayWithdrawRelation(fromBody.CompanyID, fromBody.CurrencyType);
                //最高權限允許以勾選方式新增供應商
                //新增資料
                foreach (var providerdata in fromBody.ProviderCodeAndWeight)
                {
                    SS = " INSERT INTO GPayWithdrawRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID,Weight) " +
                         " VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID,@Weight)";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = providerdata.ProviderCode;
                    DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = "";
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
                    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                    DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = providerdata.Weight;
                    T.ExecuteDB(DBCmd);
                    RedisCache.GPayWithdrawRelation.UpdateGPayWithdrawRelation(fromBody.CompanyID, fromBody.CurrencyType);

                }
                //}
            }
        });


        return returnValue;

    }

    public int UpdateGPayWithdrawRelation(FromBody.GPayWithdrawRelationSet fromBody, int CompanyType)
    {
        int returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;

        var CompanyTD = GetCompanyByID(fromBody.CompanyID);
        //新增 GPayRelation
        if (CompanyTD != null)
        {
            //(WithdrawLimitType,CurrencyType,forCompanyID,MaxLimit,MinLimit,Charge)
            DBAccess.ExecuteTransDB(DBConnStr, T =>
            {
                SS = " UPDATE WithdrawLimit";
                SS += " SET MaxLimit=@MaxLimit,MinLimit=@MinLimit,Charge=@Charge,Rate=@Rate ";
                SS += " WHERE CurrencyType=@CurrencyType And WithdrawLimitType=2 And forCompanyID=@forCompanyID";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                DBCmd.Parameters.Add("@MaxLimit", SqlDbType.Decimal).Value = fromBody.MaxLimit;
                DBCmd.Parameters.Add("@MinLimit", SqlDbType.Decimal).Value = fromBody.MinLimit;
                DBCmd.Parameters.Add("@Charge", SqlDbType.Decimal).Value = fromBody.Charge;
                DBCmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = fromBody.Rate;

                returnValue = T.ExecuteDB(DBCmd);
                RedisCache.CompanyWithdrawLimit.UpdateCompanyAPIWithdrawLimit(fromBody.CompanyID, fromBody.CurrencyType);

                if (returnValue > 0)
                {
                    //if (CompanyType == 0 && CompanyTD.ParentCompanyID == 0)
                    //{
                    //先刪除舊有資料
                    SS = "DELETE FROM GPayWithdrawRelation WHERE forCompanyID =@forCompanyID  And CurrencyType=@CurrencyType";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.NVarChar).Value = fromBody.CurrencyType;
                    T.ExecuteDB(DBCmd);
                    RedisCache.GPayWithdrawRelation.DeleteGPayWithdrawRelation(fromBody.CompanyID, fromBody.CurrencyType);
                    //最高權限允許以勾選方式新增供應商
                    //新增資料
                    foreach (var providerdata in fromBody.ProviderCodeAndWeight)
                    {
                        SS = " INSERT INTO GPayWithdrawRelation (ProviderCode,ServiceType,CurrencyType,forCompanyID,Weight) " +
                             " VALUES (@ProviderCode,@ServiceType,@CurrencyType,@forCompanyID,@Weight)";

                        DBCmd = new System.Data.SqlClient.SqlCommand();
                        DBCmd.CommandText = SS;
                        DBCmd.CommandType = CommandType.Text;
                        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = providerdata.ProviderCode;
                        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = "";
                        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
                        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.CompanyID;
                        DBCmd.Parameters.Add("@Weight", SqlDbType.Int).Value = 1;

                        T.ExecuteDB(DBCmd);
                        RedisCache.GPayWithdrawRelation.UpdateGPayWithdrawRelation(fromBody.CompanyID, fromBody.CurrencyType);

                    }
                    //}
                }
            });
        }
        return returnValue;
    }
    #endregion

    #region SummaryProviderByDate
    public List<DBModel.SummaryProviderByDate> GetSummaryProviderByDateTableResult(FromBody.SummaryProviderByDate fromBody)
    {
        List<DBModel.SummaryProviderByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ServiceTypeName,ProviderName,SummaryProviderByDate.*,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryProviderByDate WITH (NOLOCK) " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryProviderByDate.ServiceType And ServiceType.CurrencyType=SummaryProviderByDate.CurrencyType" +
             " LEFT JOIN ProviderCode ON ProviderCode.ProviderCode = SummaryProviderByDate.ProviderCode " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And SummaryProviderByDate.CurrencyType=@CurrencyType AND (SummaryAmount <> 0 OR SummaryWithdrawalAmount <> 0) ";

        if (fromBody.ServiceType != "0")
        {
            SS += " And SummaryProviderByDate.ServiceType=@ServiceType ";
        }
        if (fromBody.ProviderCode != "-99")
        {
            SS += " And SummaryProviderByDate.ProviderCode=@ProviderCode ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = fromBody.CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryProviderByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.ProxySummaryProviderByDate> GetProxySummaryProviderByDateTableResult(FromBody.SummaryProviderByDate fromBody)
    {
        List<DBModel.ProxySummaryProviderByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT GroupName,ProxySummaryProviderByDate.*,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM ProxySummaryProviderByDate WITH (NOLOCK) " +
             " LEFT JOIN ProxyProviderGroup PPG WITH (NOLOCK) ON PPG.GroupID=ProxySummaryProviderByDate.GroupID " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And ProxySummaryProviderByDate.CurrencyType='JPY' AND (SummaryAmount <> 0 OR SummaryWithdrawalAmount <> 0) And ProviderCode=@ProviderCode ";

        if (fromBody.GroupID != 0)
        {
            SS += " And ProxySummaryProviderByDate.GroupID=@GroupID ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = fromBody.GroupID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProxySummaryProviderByDate>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region SummaryCompanyByDate
    public List<DBModel.CompanyServicePointHistory> GetCompanyServicePointHistoryResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.CompanyServicePointHistory> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " select ServiceTypeName,CompanyName,CSP.*," +
             " Case when CSP.OperatorType = 0" +
             " then(select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CSP.TransactionID )" +
             " else (select Withdrawal.WithdrawSerial from Withdrawal  WITH (NOLOCK)  where Withdrawal.WithdrawID = CSP.TransactionID ) " +
             " End as TransactionOrder" +
             " from CompanyServicePointHistory CSP" +
             " left join CompanyTable CT on CT.CompanyID = CSP.forCompanyID" +
             " left join ServiceType ST on ST.ServiceType = CSP.ServiceType and ST.CurrencyType = CSP.CurrencyType" +
             " WHERE CSP.CreateDate Between @StartDate And @EndDate ";

        if (SearchData.ServiceType != "0")
        {
            SS += " And CSP.ServiceType=@ServiceType ";
        }

        if (SearchData.CompanyID != 0)
        {
            SS += " And CSP.forCompanyID=@CompanyID ";
        }


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyServicePointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryBlockChainByDate> GetSummaryBlockChainByDateResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.SummaryBlockChainByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CompanyTable.CompanyType,ServiceTypeName,SummaryBlockChainByDate.*,CompanyTable.CompanyName,ProviderCode.ProviderName,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryBlockChainByDate WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryBlockChainByDate.ServiceType And ServiceType.CurrencyType=SummaryBlockChainByDate.CurrencyType " +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID = SummaryBlockChainByDate.forCompanyID " +
             " LEFT JOIN ProviderCode ON ProviderCode.ProviderCode = SummaryBlockChainByDate.forProviderCode " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And SummaryBlockChainByDate.CurrencyType=@CurrencyType ";

        if (SearchData.ServiceType != "0")
        {
            SS += " And SummaryBlockChainByDate.ServiceType=@ServiceType ";
        }

        if (SearchData.CompanyID != -99)
        {
            SS += " And forCompanyID=@CompanyID ";
        }
        if (SearchData.ProviderCode != "-99")
        {
            SS += " And forProviderCode=@forProviderCode ";
        }
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = SearchData.ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryBlockChainByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetSummaryCompanyByDateResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CompanyTable.CompanyType,ServiceTypeName,SummaryCompanyByDate.*,CompanyTable.CompanyName,ProviderCode.ProviderName,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryCompanyByDate WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryCompanyByDate.ServiceType And ServiceType.CurrencyType=SummaryCompanyByDate.CurrencyType " +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID = SummaryCompanyByDate.forCompanyID " +
             " LEFT JOIN ProviderCode ON ProviderCode.ProviderCode = SummaryCompanyByDate.forProviderCode " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And SummaryCompanyByDate.CurrencyType=@CurrencyType ";

        if (SearchData.ServiceType != "0")
        {
            SS += " And SummaryCompanyByDate.ServiceType=@ServiceType ";
        }

        if (SearchData.CompanyID != -99)
        {
            SS += " And SummaryCompanyByDate.forCompanyID=@CompanyID ";
        }
        if (SearchData.ProviderCode != "-99")
        {
            SS += " And SummaryCompanyByDate.forProviderCode=@forProviderCode ";
        }
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = SearchData.ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDateFlot> GetSummaryCompanyByDateResultFlot(FromBody.PaymentTable SearchData)
    {
        List<DBModel.SummaryCompanyByDateFlot> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT convert(varchar, SummaryDate, 23) as SummaryDate,ISNULL(SUM(SummaryWithdrawalAmount),0) TotalWithdrawalAmount,ISNULL(SUM(SummaryNetAmount),0) TotalNetAmount" +
            " FROM SummaryCompanyByDate"+
            " WHERE SummaryDate Between @StartDate And @EndDate"+
            " And SummaryCompanyByDate.CurrencyType = @CurrencyType"+
            " And forCompanyID = @CompanyID"+
            " GROUP BY SummaryDate";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDateFlot>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByHour> GetSummaryCompanyByHourResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.SummaryCompanyByHour> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT SummaryCompanyByHour.*,convert(varchar, SummaryDate, 23) as SummaryTime2 FROM SummaryCompanyByHour WITH (NOLOCK)  " +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID = SummaryCompanyByHour.forCompanyID " +
             " WHERE SummaryTime Between @StartDate And @EndDate " +
             " And SummaryCompanyByHour.CurrencyType=@CurrencyType ";



        if (SearchData.CompanyID != -99)
        {
            SS += " And forCompanyID=@CompanyID ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByHour>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetSummaryCompanyByAgent(FromBody.PaymentTable SearchData)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ServiceTypeName,SummaryCompanyByDate.*,CompanyTable.CompanyName,ProviderCode.ProviderName,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryCompanyByDate WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryCompanyByDate.ServiceType And ServiceType.CurrencyType=SummaryCompanyByDate.CurrencyType " +
             " JOIN CompanyTable ON CompanyTable.CompanyID = SummaryCompanyByDate.forCompanyID " +
             " LEFT JOIN ProviderCode ON ProviderCode.ProviderCode = SummaryCompanyByDate.forProviderCode " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And SummaryCompanyByDate.CurrencyType=@CurrencyType And CompanyTable.CompanyType=2 And SummaryAgentAmount<>0 ";

        if (SearchData.ServiceType != "0")
        {
            SS += " And SummaryCompanyByDate.ServiceType=@ServiceType ";
        }

        if (SearchData.CompanyID != -99)
        {
            SS += " And SummaryCompanyByDate.forCompanyID=@CompanyID ";
        }
        if (SearchData.ProviderCode != "-99")
        {
            SS += " And SummaryCompanyByDate.forProviderCode=@forProviderCode ";
        }
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = SearchData.ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetSummaryCompanyByAgent2(FromBody.PaymentTable SearchData, int CompanyID)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ServiceTypeName,SummaryCompanyByDate.*,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryCompanyByDate WITH (NOLOCK)  " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryCompanyByDate.ServiceType And ServiceType.CurrencyType=SummaryCompanyByDate.CurrencyType " +
             " LEFT JOIN CompanyTable ON CompanyTable.CompanyID = SummaryCompanyByDate.forCompanyID " +
             " WHERE SummaryDate Between @StartDate And @EndDate " +
             " And SummaryCompanyByDate.CurrencyType=@CurrencyType And CompanyTable.CompanyType=2 And SummaryAgentAmount<>0  And forCompanyID=@CompanyID ";

        if (SearchData.ServiceType != "0")
        {
            SS += " And SummaryCompanyByDate.ServiceType=@ServiceType ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetSummaryCompanyByAgentDownCompany(FromBody.PaymentTable SearchData, int CompanyID)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        var CompanyModel = GetCompanyByID(CompanyID);

        SS = @" SELECT 
                convert(varchar, SummaryDate, 23) as SummaryDate2,
                SummaryAmount,
                tmpTable.CurrencyType, -- 改這裡，直接用內層的 CurrencyType
                CompanyName,
                ServiceTypeName
            FROM (
                SELECT 
                    SummaryDate, 
                    ServiceType, 
                    SC.CurrencyType,  -- 改這裡，這才是正確來源
                    forCompanyID, 
                    SUM(SummaryAmount) AS SummaryAmount
                FROM CompanyTable CT 
                JOIN SummaryCompanyByDate SC ON CT.CompanyID = SC.forCompanyID
                WHERE  
                    CT.SortKey LIKE @SortKey + '%' 
                    AND CT.CompanyID <> @CompanyID  
                    AND SummaryDate BETWEEN @StartDate AND @EndDate 
                GROUP BY SC.CurrencyType, forCompanyID, SummaryDate, ServiceType
            ) tmpTable 
            LEFT JOIN CompanyTable CT ON CT.CompanyID = tmpTable.forCompanyID 
            LEFT JOIN ServiceType ST ON ST.ServiceType = tmpTable.ServiceType 
            WHERE SummaryAmount <> 0";
        if (SearchData.ServiceType != "0")
        {
            SS += " And SummaryCompanyByDate.ServiceType=@ServiceType ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@SortKey", SqlDbType.VarChar).Value = CompanyModel.SortKey;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetSummaryCompanyByDateResultByCurrencyType(FromBody.SummaryCompanyByDateResultByCurrencyTypeSet SearchData)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ServiceTypeName,SummaryCompanyByDate.*,convert(varchar, SummaryDate, 23) as SummaryDate2 FROM SummaryCompanyByDate WITH (NOLOCK) " +
             " LEFT JOIN ServiceType ON ServiceType.ServiceType=SummaryCompanyByDate.ServiceType" +
             " WHERE SummaryCompanyByDate.SummaryDate Between @StartDate And @EndDate And SummaryCompanyByDate.forCompanyID=@CompanyID And SummaryCompanyByDate.CurrencyType=@CurrencyType And SummaryCompanyByDate.ServiceType=@ServiceType";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDateChartData> GetSummaryCompanyByDateResultForChart(FromBody.SummaryCompanyByDateSet SearchData)
    {
        List<DBModel.SummaryCompanyByDateChartData> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        SS = " select ServiceType.ServiceTypeName,b.* from(SELECT sum(SummaryNetAmount) as SummaryNetAmounts, ServiceType FROM SummaryCompanyByDate" +
             " WHERE SummaryDate Between @StartDate And @EndDate And forCompanyID = @CompanyID And SummaryCompanyByDate.CurrencyType = @CurrencyType group by SummaryCompanyByDate.ServiceType) as b" +
             " LEFT JOIN ServiceType WITH(NOLOCK)" +
             " ON ServiceType.ServiceType = b.ServiceType order by SummaryNetAmounts desc";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SearchData.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SearchData.EndDate;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDateChartData>(DT).ToList();
                decimal amounts = 0;
                foreach (var rowData in returnValue)
                {
                    amounts += rowData.SummaryNetAmounts;
                }

                if (amounts == 0)
                {
                    returnValue = null;
                }
                else
                {
                    foreach (var rowData in returnValue)
                    {
                        if (amounts != 0)
                        {
                            rowData.SummaryNetAmountPercent = Convert.ToInt32((rowData.SummaryNetAmounts / amounts) * 100);
                        }
                        else
                        {
                            rowData.SummaryNetAmountPercent = 0;
                        }
                    }
                }
            }
        }

        return returnValue;
    }

    public List<DBModel.SummaryCompanyByDate> GetCompanySummary(DateTime SummaryDate, int CompanyID, string CurrencyType, string ServiceType)
    {
        List<DBModel.SummaryCompanyByDate> returnValue = null;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;
        String SS = String.Empty;


        SS = "SELECT * FROM SummaryCompanyByDate WITH (NOLOCK) WHERE SummaryDate=@SummaryDate AND forCompanyID=@CompanyID AND CurrencyType=@CurrencyType AND ServiceType=@ServiceType";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@SummaryDate", System.Data.SqlDbType.DateTime).Value = SummaryDate;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = ServiceType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.SummaryCompanyByDate>(DT).ToList();
            }
        }

        return returnValue;
    }

    public decimal GetSummaryCompanyByDateLockAmount(int CompanyID, string CurrencyType)
    {
        decimal returnValue = 0;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable TodayDT;
        DataTable BeforedayDT = null;
        DayOfWeek Today = DateTime.Now.DayOfWeek;

        SS = " SELECT SummaryAgentAmount,SummaryNetAmount,CheckoutType,SummaryCompanyByDate.ServiceType FROM SummaryCompanyByDate" +
            " LEFT JOIN CompanyService ON SummaryCompanyByDate.ServiceType = CompanyService.ServiceType" +
            " And SummaryCompanyByDate.CurrencyType = CompanyService.CurrencyType" +
            " And SummaryCompanyByDate.forCompanyID = CompanyService.forCompanyID" +
            " Where SummaryCompanyByDate.forCompanyID = @CompanyID" +
            " And SummaryCompanyByDate.CurrencyType = @CurrencyType" +
            " And (CheckoutType = 1 or CheckoutType = 2)" +
            " And SummaryCompanyByDate.SummaryDate = @SummaryDate";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@SummaryDate", SqlDbType.DateTime).Value = DateTime.Now.ToShortDateString();
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;
        var a = DateTime.Now.ToShortDateString();
        TodayDT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (Today == DayOfWeek.Sunday)
        {
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@SummaryDate", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-1).ToShortDateString();
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
            DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

            BeforedayDT = DBAccess.GetDB(DBConnStr, DBCmd);
        }

        if (TodayDT != null)
        {
            if (TodayDT.Rows.Count > 0)
            {
                var today_datas = DataTableExtensions.ToList<DBViewModel.SummaryCompanyByDateVM>(TodayDT).ToList();
                foreach (var today_data in today_datas)
                {
                    returnValue += today_data.SummaryNetAmount;
                }
            }
        }

        if (BeforedayDT != null)
        {
            if (BeforedayDT.Rows.Count > 0)
            {
                var Beforeday_datas = DataTableExtensions.ToList<DBViewModel.SummaryCompanyByDateVM>(BeforedayDT).ToList();
                foreach (var beforeday_data in Beforeday_datas)
                {
                    if (beforeday_data.CheckoutType == 2)
                    {
                        returnValue += beforeday_data.SummaryNetAmount;
                    }
                }
            }
        }

        return returnValue;
    }

    #endregion

    #region 公司錢包歷程
    public List<DBModel.CompanyPointHistory> GetCompanyPointHistoryTableResult(FromBody.CompanyPointHistoryeSet SearchData)
    {
        List<DBModel.CompanyPointHistory> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        if (SearchData.SearchDays == "1")
        {
            SS = " Select CONVERT(varchar(10), DATEPART(hh,CreateDate)) as CreateDate2,sum(CompanyPointHistory.Value) as Value FROM CompanyPointHistory WITH (NOLOCK)  " +
                 " Where convert(varchar, CompanyPointHistory.CreateDate, 111) =@CreateDate" +
                 " And CurrencyType=@CurrencyType And forCompanyID=@CompanyID" +
                 " Group by DATEPART(hh,CreateDate)";
        }
        else
        {
            SS = " SELECT convert(varchar, CompanyPointHistory.CreateDate, 111) as CreateDate2,sum(CompanyPointHistory.Value) as Value" +
                 " FROM CompanyPointHistory WITH (NOLOCK) WHERE  forCompanyID=@CompanyID  And CurrencyType=@CurrencyType " +
                 " And CreateDate Between @StartDate And @EndDate" +
                 " Group by convert(varchar, CompanyPointHistory.CreateDate, 111)";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = SearchData.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = SearchData.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@CreateDate", SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyy/MM/dd");

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.CompanyServicePointLog> GetCompanyServicePointLogResultByCompany(FromBody.PaymentTable SearchData)
    {
        List<DBModel.CompanyServicePointLog> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT    CT.CompanyName, " +
                 "           CH.ServiceType, " +
                 "           Isnull(ST.ServiceTypeName,'') ServiceTypeName, " +
                 "           CH.OperatorType, " +
                 "           CH.Value, " +
                 "           CH.BeforeValue, " +
                 "           CH.ValueByService , " +
                 "           CH.BeforeValueByService, " +
                 "           TransactionID, " +
                 "           Dateadd(HOUR, @TimeZone - 8, CH.CreateDate) AS CreateDate, " +
                 "           (SELECT paymenttable.paymentserial FROM   paymenttable WITH (nolock) WHERE　paymenttable.paymentid = CH.transactionid) AS PaymentSerial, " +
                 "           CASE " +
                 "                     WHEN CH.OperatorType = 1 THEN ( SELECT PaymentTable.PaymentSerial FROM   PaymentTable WITH (NOLOCK) WHERE  PaymentTable.PaymentID = CH.TransactionID ) " +
                 "                     WHEN CH.OperatorType = 0 THEN ( SELECT Withdrawal.WithdrawSerial FROM   Withdrawal WITH (NOLOCK) WHERE  Withdrawal.WithdrawID = CH.TransactionID ) " +
                 "                     WHEN CH.OperatorType = 3 THEN ( SELECT CompanyManualHistory.TransactionSerial FROM   CompanyManualHistory WITH (NOLOCK) WHERE  CompanyManualHistory.CompanyManualID = CH.TransactionID ) " +
                 "                     ELSE ( SELECT PaymentTable.PaymentSerial FROM   PaymentTable WITH (NOLOCK) WHERE  PaymentTable.PaymentID = CH.TransactionID ) " +
                 "           END AS TransactionOrder, " +
                 "           CASE " +
                 "                     WHEN CH.OperatorType = 1 THEN ( SELECT PaymentTable.OrderID FROM   PaymentTable WITH (NOLOCK) WHERE  PaymentTable.PaymentID = CH.TransactionID ) " +
                 "                     WHEN CH.OperatorType = 0 THEN ( SELECT Withdrawal.DownOrderID FROM   Withdrawal WITH (NOLOCK) WHERE  Withdrawal.WithdrawID = CH.TransactionID ) " +
                 "                     WHEN CH.OperatorType = 3 THEN ( SELECT PaymentTable.OrderID FROM   PaymentTable WITH (nolock) WHERE  PaymentTable.PaymentSerial = ( SELECT CompanyManualHistory.TransactionSerial FROM                    CompanyManualHistory WITH (nolock) WHERE　CompanyManualHistory.CompanyManualID = CH.transactionid) ) " +
                 "                     ELSE ( SELECT PaymentTable.OrderID FROM   PaymentTable WITH (NOLOCK) WHERE  PaymentTable.PaymentID = CH.TransactionID ) " +
                 "           END AS DownOrderID " +
                 " FROM      CompanyPointHistory CH WITH (NOLOCK) " +
                 " LEFT JOIN ServiceType ST WITH (NOLOCK) ON CH.ServiceType = ST.ServiceType " +
                 " LEFT JOIN CompanyTable CT WITH (NOLOCK) ON CH.forCompanyID = CT.CompanyID " +
                 " WHERE     DATEADD(HOUR, @TimeZone - 8, CH.CreateDate) BETWEEN @StartDate AND @EndDate AND CH.forCompanyID = @CompanyID ";

        if (SearchData.OperatorType != 99)
        {
            SS += " AND CH.OperatorType = @OperatorType ";
        }

        SS += " ORDER BY CH.CreateDate DESC";



        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = SearchData.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = SearchData.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = SearchData.OperatorType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Pay.CurrencyType;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyServicePointLog>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.CompanyServicePointLog> GetCompanyPointHistoryLogResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.CompanyServicePointLog> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT CT.CompanyName,CH.ServiceType,ISNULL(ST.ServiceTypeName,'') ServiceTypeName,CH.OperatorType,CH.Value,CH.BeforeValue,CH.ValueByService " +
             " ,CH.BeforeValueByService,TransactionID,DATEADD(HOUR, @TimeZone - 8, CH.CreateDate) AS CreateDate, " +
             " Case when CH.OperatorType = 1" +
             " then (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
             " when CH.OperatorType = 0" +
             " then (select Withdrawal.WithdrawSerial from Withdrawal  WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID )" +
             " when CH.OperatorType = 3" +
             " then (select CompanyManualHistory.TransactionSerial from CompanyManualHistory where CompanyManualHistory.CompanyManualID = CH.TransactionID )" +
             " else (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
             " End as TransactionOrder" +
             " FROM CompanyPointHistory CH " +
             " LEFT JOIN ServiceType ST ON CH.ServiceType = ST.ServiceType " +
             " LEFT JOIN CompanyTable CT ON CH.forCompanyID = CT.CompanyID " +
             " WHERE DATEADD(HOUR, @TimeZone - 8, CH.CreateDate) BETWEEN @StartDate AND @EndDate ";

        if (SearchData.ServiceType.ToString() != "0")
        {
            SS += " AND CH.ServiceType = @ServiceType ";
        }
        if (SearchData.CompanyID != -99)
        {
            SS += "  AND CH.forCompanyID = @CompanyID ";
        }

        if (SearchData.OperatorType != 99)
        {
            SS += " AND CH.OperatorType = @OperatorType ";
        }

        SS += " ORDER BY CH.CreateDate DESC";



        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = SearchData.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = SearchData.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = SearchData.OperatorType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = SearchData.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = SearchData.CurrencyType;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyServicePointLog>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.CompanyServiceAndProviderPointLog> GetManualPointLogResult(FromBody.PaymentTable SearchData)
    {
        List<DBModel.CompanyServiceAndProviderPointLog> returnValue = new List<DBModel.CompanyServiceAndProviderPointLog>();
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        SS = " SELECT tmpTable.*," +
             " COALESCE(PT.CostRate, WW.CostCharge, 0) as UpValue," +
             " COALESCE(PT.CollectRate, WW.CollectCharge, 0) as DownValue," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select ProviderCode.ProviderName from ProviderCode where ProviderCode.ProviderCode = PT.ProviderCode)" +
             " WHEN WW.WithdrawSerial is not null" +
             " then(select ProviderCode.ProviderName from ProviderCode where ProviderCode.ProviderCode = WW.ProviderCode)" +
             " else (SELECT ProviderName FROM ProviderManualHistory LEFT JOIN ProviderCode" +
             " ON ProviderManualHistory.ProviderCode = ProviderCode.ProviderCode WHERE ProviderManualHistory.ProviderManualID = tmpTable.PHTransactionID)" +
             " end as ProviderName2," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select PT.ProviderCode)" +
             " WHEN WW.WithdrawSerial is not null" +
             " then(select WW.ProviderCode)" +
             " else (SELECT ProviderCode FROM ProviderManualHistory WHERE ProviderManualHistory.ProviderManualID = tmpTable.PHTransactionID)" +
             " end as ProviderCode2," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select ServiceType.ServiceTypeName from ServiceType where ServiceType.ServiceType = PT.ServiceType)" +
             " WHEN WW.WithdrawSerial is not null" +
             " THEN(select ServiceType.ServiceTypeName from ServiceType where ServiceType.ServiceType = WW.ServiceType)" +
             " WHEN tmpTable.OperatorType2=4 THEN(select ServiceType.ServiceTypeName from ServiceType where ServiceType.ServiceType = tmpTable.ServiceType) " +
             " else (SELECT ServiceTypeName FROM CompanyManualHistory LEFT JOIN ServiceType" +
             " ON CompanyManualHistory.ServiceType = ServiceType.ServiceType WHERE CompanyManualHistory.CompanyManualID = tmpTable.TransactionID)" +
             " end as ServiceTypeName2," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select CompanyTable.CompanyName from CompanyTable where CompanyTable.CompanyID = PT.forCompanyID)" +
             " WHEN WW.WithdrawSerial is not null" +
             " THEN(select CompanyTable.CompanyName from CompanyTable where CompanyTable.CompanyID = WW.forCompanyID)" +
             " WHEN tmpTable.OperatorType2 = 4 THEN(select CompanyTable.CompanyName from CompanyTable where CompanyTable.CompanyID = tmpTable.forCompanyID)" +
             " else (select CompanyTable.CompanyName FROM CompanyManualHistory LEFT JOIN CompanyTable" +
             " ON CompanyManualHistory.forCompanyID = CompanyTable.CompanyID WHERE CompanyManualHistory.CompanyManualID = tmpTable.TransactionID)" +
             " end as CompanyName2," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select PT.forCompanyID)" +
             " WHEN WW.WithdrawSerial is not null" +
             " THEN(select WW.forCompanyID)" +
             " WHEN tmpTable.OperatorType2=4 THEN(select tmpTable.forCompanyID) " +
             " else (select CompanyTable.CompanyID FROM CompanyManualHistory LEFT JOIN CompanyTable" +
             " ON CompanyManualHistory.forCompanyID = CompanyTable.CompanyID WHERE CompanyManualHistory.CompanyManualID = tmpTable.TransactionID)" +
             " end as CompanyID2," +
             " CASE WHEN PT.PaymentSerial is not null" +
             " then(select PT.ServiceType)" +
             " WHEN WW.WithdrawSerial is not null" +
             " THEN(select WW.ServiceType)" +
             " WHEN tmpTable.OperatorType2 = 4 THEN(select tmpTable.ServiceType)" +
             " else (SELECT ServiceType FROM CompanyManualHistory WHERE CompanyManualHistory.CompanyManualID = tmpTable.TransactionID)" +
             " end as ServiceType2," +
             " CreateDate2" +
             " FROM(" +
            " SELECT  CH.*, PH.Value as ProviderValue, PH.BeforeValue AS ProviderBeforeValue,PH.TransactionID AS PHTransactionID," +
            " IIF(CH.CreateDate is not null, CH.CreateDate, PH.CreateDate) As CreateDate2," +
            " IIF(CH.OperatorType is not null," +
            " CASE WHEN CH.OperatorType = 1 THEN 0" +
            " WHEN CH.OperatorType = 0 THEN 1" +
            " WHEN CH.OperatorType = 2 THEN 4" +
            " ELSE CH.OperatorType END," +
            " CASE WHEN PH.OperatorType = 0 THEN 0" +
            " WHEN PH.OperatorType = 2 THEN 1" +
            " ELSE PH.OperatorType END) As OperatorType2," +
            " IIF(CH.TransactionID is not null," +
            " Case when CH.OperatorType = 1 then(select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
            " when CH.OperatorType = 0 then(select Withdrawal.WithdrawSerial from Withdrawal  WITH (NOLOCK)  where Withdrawal.WithdrawID = CH.TransactionID )" +
            " when CH.OperatorType = 2 then(select CONVERT(varchar(50), AgentClose.AgentCloseID) from AgentClose where AgentClose.AgentCloseID = CH.TransactionID )" +
            " when CH.OperatorType = 3" +
            " then(select CompanyManualHistory.TransactionSerial" +
            " from CompanyManualHistory where CompanyManualHistory.CompanyManualID = CH.TransactionID )" +
            " else (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = CH.TransactionID )" +
            " End," +
            " Case when PH.OperatorType = 0" +
            " then(select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = PH.TransactionID )" +
            " when PH.OperatorType = 2" +
            " then(select Withdrawal.WithdrawSerial from Withdrawal  WITH (NOLOCK)  where Withdrawal.WithdrawID = PH.TransactionID )" +
            " when PH.OperatorType = 3" +
            " then(select ProviderManualHistory.TransactionSerial" +
            " from ProviderManualHistory where ProviderManualHistory.ProviderManualID = PH.TransactionID )" +
            " else (select PaymentTable.PaymentSerial from PaymentTable where PaymentTable.PaymentID = PH.TransactionID ) End" +
            " ) as TransactionOrder" +
            " FROM CompanyPointHistory CH" +
            " FULL OUTER JOIN ProviderPointHistory PH" +
            " ON CASE" +
            " WHEN CH.TransactionID = PH.TransactionID  AND CH.OperatorType = 0 AND PH.OperatorType = 2 THEN 1" +
            " WHEN CH.TransactionID = PH.TransactionID  AND CH.OperatorType = 1 AND PH.OperatorType = 0 THEN 1" +
            //" WHEN CH.TransactionID = PH.TransactionID  AND CH.OperatorType = 3 AND PH.OperatorType = 3 THEN 1"+
            " ELSE 0 END = 1 " +
            " WHERE (CH.CreateDate between @StartDate and @EndDate OR PH.CreateDate between @StartDate and @EndDate) " +
            " ) AS tmpTable" +
            " LEFT JOIN PaymentTable PT ON PT.PaymentSerial = TransactionOrder" +
            " LEFT JOIN Withdrawal WW ON WW.WithdrawSerial = TransactionOrder" +
            " WHERE CreateDate2 between @StartDate and @EndDate";


        if (SearchData.OperatorType != 99)
        {
            SS += " AND tmpTable.OperatorType2 = @OperatorType ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = SearchData.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = SearchData.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = SearchData.CompanyID;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.Int).Value = SearchData.OperatorType;


        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyServiceAndProviderPointLog>(DT).ToList();

                if (SearchData.ServiceType.ToString() != "0")
                {
                    returnValue = returnValue.Where(w => w.ServiceType2 == SearchData.ServiceType).ToList();
                }

                if (SearchData.ProviderCode.ToString() != "99")
                {
                    returnValue = returnValue.Where(w => w.ProviderCode2 == SearchData.ProviderCode).ToList();
                }


                if (SearchData.CompanyID.ToString() != "-99")
                {
                    returnValue = returnValue.Where(w => w.CompanyID2 == SearchData.CompanyID).ToList();
                }
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    #endregion

    #region PaymentTable
    public List<DBModel.PaymentTransferLog> GetPaymentTransferLogResult(FromBody.PaymentTransferLogSet fromBody)
    {
        List<DBModel.PaymentTransferLog> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT PaymentTransferLog.*,ProviderCode.ProviderName FROM PaymentTransferLog WITH (NOLOCK) " +
             " LEFT JOIN ProviderCode ON ProviderCode.ProviderCode=PaymentTransferLog.forProviderCode " +
             " WHERE CreateDate >= @StartDate And CreateDate <= @EndDate ";

        if (fromBody.ProcessStatus != 99)
        {
            SS += " And PaymentTransferLog.Type=@Type ";
        }
        if (!string.IsNullOrEmpty(fromBody.PaymentSerial))
        {
            SS += " And PaymentTransferLog.forPaymentSerial=@PaymentSerial ";
        }
        if (!string.IsNullOrEmpty(fromBody.ProviderCode))
        {
            SS += " And PaymentTransferLog.forProviderCode=@ProviderCode ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = fromBody.PaymentSerial;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = fromBody.ProcessStatus;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentTransferLog>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentTable> GetPatchPaymentTableResult()
    {
        List<DBModel.PaymentTable> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode " +
             " FROM PaymentTable AS P " +
             " LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             " LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             " LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode " +
             " WHERE P.ProcessStatus = 7";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;


        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentTable>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentTable> GetPatchPaymentTableResultByDate(FromBody.PaymentTable fromBody)
    {
        List<DBModel.PaymentTable> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode " +
             " FROM PaymentTable AS P " +
             " LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             " LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             " LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode " +
             " WHERE (P.ProcessStatus = 7 or P.forPaymentSerial <>'')" +
             " And P.CreateDate >= @StartDate And P.CreateDate <= @EndDate ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentTable>(DT).ToList();
            }
        }

        return returnValue;
    }

    public DBViewModel.PaymentRow_NunberID GetAbnormalPaymentRow_NunberID(string OrderID, string ProviderID, int CompanyID, string ProviderCode, int SearchType, string IP)
    {
        DBViewModel.PaymentRow_NunberID returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT ROWID,CreateDate ";
        SS += " FROM(SELECT PaymentSerial, ROW_NUMBER() OVER(ORDER BY CreateDate) AS ROWID,CreateDate ";
        SS += " FROM PaymentTable";
        SS += " Where 1=1 ";
        SS += " And ProviderCode = @ProviderCode";


        if (CompanyID != 0)
        {
            SS += " And forCompanyID = @CompanyID";
        }

        if (!string.IsNullOrEmpty(IP))
        {
            SS += " And (UserIP = @UserIP OR ClientIP=@UserIP) ";
        }

        SS += " ) as tmpTable";

        SS += " WHERE tmpTable.PaymentSerial = @OrderID";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;

        DBCmd.Parameters.Add("@OrderID", System.Data.SqlDbType.VarChar).Value = OrderID;
        DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = IP;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.PaymentRow_NunberID>(DT).FirstOrDefault();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetAbnormalPaymentTableResult(FromBody.GetAbnormalPaymentSet fromBody)
    {

        string SS = "";
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBViewModel.PaymentRow_NunberID Row_NunberData = GetAbnormalPaymentRow_NunberID(fromBody.OrderID, fromBody.ProviderOrderID, fromBody.CompanyID, fromBody.Providercode, fromBody.SearchType, fromBody.IP);
        if (Row_NunberData != null)
        {

            SS = " SELECT * FROM(";
            SS += " SELECT ROW_NUMBER() OVER(ORDER BY P.CreateDate) AS ROWID,";
            SS += " P.* ,  ";
            SS += "       S.ServiceTypeName, ";
            SS += "       PC.ProviderName, ";
            SS += "       B.BankName, ";
            SS += "       B.BankType, ";
            SS += "       C.CompanyName, ";
            SS += "       C.CompanyCode, ";
            SS += "       C.MerchantCode, ";
            SS += "       convert(varchar, P.CreateDate, 120) as CreateDate2, ";
            SS += "       convert(varchar, P.OrderDate, 120) as OrderDate2, ";
            SS += "       convert(varchar, P.FinishDate, 120) as FinishDate2 ";
            SS += "FROM PaymentTable AS P ";
            SS += "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType ";
            SS += "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode ";
            SS += "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID ";
            SS += "LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode ";
            SS += " Where 1=1 ";
            SS += " And P.ProviderCode = @ProviderCode";

            if (!string.IsNullOrEmpty(fromBody.IP))
            {
                SS += " And (P.UserIP = @UserIP OR P.ClientIP=@UserIP) ";
            }

            if (fromBody.CompanyID != 0)
            {
                SS += " And P.forCompanyID = @CompanyID ";
            }
            SS += " ) as tmpTable";
            if (fromBody.CheckType == 0)
            {
                SS += " WHERE @ROWID-10<=tmpTable.ROWID And tmpTable.ROWID<=@ROWID+10";
            }
            else
            {
                SS += " WHERE dateadd(mi,-10,@SearchDate) <= tmpTable.CreateDate And tmpTable.CreateDate<= dateadd(mi,+10,@SearchDate)";
            }

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;

            DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
            DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.Providercode;
            DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = fromBody.IP;

            DBCmd.Parameters.Add("@SearchDate", SqlDbType.DateTime).Value = Row_NunberData.CreateDate;
            DBCmd.Parameters.Add("@ROWID", SqlDbType.Int).Value = Row_NunberData.ROWID;
            DT = DBAccess.GetDB(DBConnStr, DBCmd);

            if (DT != null)
            {
                if (DT.Rows.Count > 0)
                {
                    returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
                }
            }

        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentProviderReportReviewResult(FromBody.GetPaymentForAdmin fromBody, string CompanyCode, int GroupID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = " SELECT P.* ,PaymentRate,AT.RealName,  " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             " FROM PaymentTable AS P WITH (NOLOCK) " +
             " LEFT JOIN AdminTable AT WITH (NOLOCK) ON AT.AdminID=P.ConfirmAdminID " +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0" +
             " WHERE P.CreateDate >= @StartDate And P.CreateDate <= @EndDate And SubmitType=1 And P.ProviderCode=@ProviderCode" +
             " And(PPO.GroupID = 0 or PPO.GroupID = @GroupID) ";

        //商户订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            SS += " And P.PaymentSerial = @OrderID ";
        }

        //卡号
        if (fromBody.PatchDescription != "")
        {
            SS += " And P.PatchDescription=@PatchDescription";
        }

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@PatchDescription", System.Data.SqlDbType.NVarChar).Value = fromBody.PatchDescription;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@OrderID", System.Data.SqlDbType.VarChar).Value = fromBody.OrderID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        //if (!fromBody.ProcessStatus.Contains(99))
        //{
        //    returnValue = returnValue.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        //}

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentReportReviewResult(FromBody.GetPaymentForAdmin fromBody)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             " PPG.GroupName, " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P WITH (NOLOCK)  " +
             "LEFT JOIN ServiceType AS S WITH (NOLOCK)  ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC WITH (NOLOCK)  ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C WITH (NOLOCK)   ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             "WHERE P.CreateDate >= @StartDate And P.CreateDate <= @EndDate And SubmitType=1";

        if (fromBody.CompanyID != -99)
        {//-99代表所有營運商(有選營運商)

            SS += " And P.forCompanyID = @CompanyID ";
        }

        if (fromBody.ServiceType != "0")
        {//-99代表所有營運商(有選營運商)

            SS += " And P.ServiceType = @ServiceType ";
        }

        if (fromBody.ProviderCode != "0")
        {//-99代表所有營運商(有選營運商)

            SS += " And P.ProviderCode = @ProviderCode ";
        }

        //商户订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            SS += " And P.PaymentSerial = @PaymentSerial ";
        }

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@SubmitType", System.Data.SqlDbType.Int).Value = fromBody.SubmitType;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = fromBody.OrderID;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            returnValue = returnValue.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentTableResultByWaitReview(List<int> ProcessStatus)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             " PPG.GroupName," +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P WITH (NOLOCK)  " +
             "LEFT JOIN ServiceType AS S WITH (NOLOCK)  ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC WITH (NOLOCK)  ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C WITH (NOLOCK)   ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN  ProxyProviderOrder PPO WITH (NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0 " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             "WHERE  SubmitType=1";


        if (!ProcessStatus.Contains(99))
        {
            SS += " And P.ProcessStatus=@ProcessStatus";
        }

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProcessStatus", System.Data.SqlDbType.Int).Value = ProcessStatus.First();

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetProviderPaymentTableResultByWaitReview(List<int> ProcessStatus, string ProviderCode, int GroupID)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P  WITH (NOLOCK)  " +
             "LEFT JOIN ServiceType AS S  WITH (NOLOCK)  ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC  WITH (NOLOCK)  ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  WITH (NOLOCK)   ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN  ProxyProviderOrder PPO  WITH (NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0 " +
             "WHERE  SubmitType=1 And P.ProviderCode=@ProviderCode And(PPO.GroupID = 0 or PPO.GroupID = @GroupID) ";


        if (!ProcessStatus.Contains(99))
        {
            SS += " And P.ProcessStatus=@ProcessStatus";
        }

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProcessStatus", System.Data.SqlDbType.Int).Value = ProcessStatus.First();
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = GroupID;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentTableResultByAdmin(FromBody.GetPaymentForAdmin fromBody)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       PC.DecimalPlaces, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       PBC.BankCardName, " +
             "       convert(varchar, DATEADD(HOUR, @TimeZone, P.CreateDateUTC), 120) as CreateDate2, " +
             "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
             "       convert(varchar, DATEADD(HOUR, @TimeZone, P.FinishDateUTC), 120) as FinishDate2 " +
             "FROM PaymentTable AS P " +
             "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             "LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode " +
             " LEFT JOIN PaymentDetailBankCard AS PDB ON PDB.forPaymentID = P.PaymentID "+
             " LEFT JOIN ProviderBankCard AS PBC ON PDB.forBankCardGUID = PBC.BankCardGUID ";

        if (fromBody.TimeType == "F")
        {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.FinishDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.FinishDateUTC) <= @EndDate ";
        }
        else {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.CreateDateUTC) <= @EndDate ";
        }
        

        if (fromBody.CompanyID != -99)
        {//-99代表所有營運商(有選營運商)

            SS += " And P.forCompanyID = @CompanyID ";
        }

        if (fromBody.ProviderCode != "0")
        {//0代表所有供應商)(有選供應商)
            SS += " And P.ProviderCode = @ProviderCode ";
        }

        //商户订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstOrderID = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstOrderID.Count > 0)
            {
                var parameters = new string[LstOrderID.Count];
                for (int i = 0; i < LstOrderID.Count; i++)
                {
                    parameters[i] = string.Format("@OrderID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstOrderID[i]);
                }

                SS += string.Format(" And P.OrderID IN ({0})", string.Join(", ", parameters));
            }
        }

        //系统订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstPaymentSerial = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstPaymentSerial.Count > 0)
            {
                var parameters = new string[LstPaymentSerial.Count];
                for (int i = 0; i < LstPaymentSerial.Count; i++)
                {
                    parameters[i] = string.Format("@PaymentSerial{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstPaymentSerial[i]);
                }

                SS += string.Format(" Or P.PaymentSerial IN ({0})", string.Join(", ", parameters));
            }
        }

        if (!string.IsNullOrEmpty(fromBody.CompanyName))
        {
            SS += " And C.CompanyName=@CompanyName ";
        }

        if (!string.IsNullOrEmpty(fromBody.ProviderName))
        {
            SS += " And PC.ProviderName=@ProviderName ";
        }

        if (fromBody.ServiceType != "99")
        {
            SS += " And P.ServiceType=@ServiceType ";
        }

        if (fromBody.StartAmount != 0)
        {
            SS += " And P.OrderAmount >= @StartAmount";

            if (fromBody.EndAmount != 0)
            {
                SS += " And P.OrderAmount <= @EndAmount";
            }
        }

        if (fromBody.EndAmount != 0)
        {
            SS += " And P.OrderAmount <= @EndAmount";

            if (fromBody.StartAmount != 0)
            {
                SS += " And P.OrderAmount >= @StartAmount";
            }
        }

        if (fromBody.SubmitType != 99)
        {
            SS += " And P.SubmitType >= @SubmitType";
        }

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@CompanyName", System.Data.SqlDbType.NVarChar).Value = fromBody.CompanyName;
        DBCmd.Parameters.Add("@ProviderName", System.Data.SqlDbType.NVarChar).Value = fromBody.ProviderName;
        DBCmd.Parameters.Add("@StartAmount", System.Data.SqlDbType.Decimal).Value = fromBody.StartAmount;
        DBCmd.Parameters.Add("@EndAmount", System.Data.SqlDbType.Decimal).Value = fromBody.EndAmount;
        DBCmd.Parameters.Add("@SubmitType", System.Data.SqlDbType.Int).Value = fromBody.SubmitType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            returnValue = returnValue.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public DBModel.StatisticsPaymentAmount GetPaymentPointBySearchFilter(FromBody.GetPaymentForAdmin fromBody)
    {
        List<DBModel.PointBySearchFilter> PointBySearchFilterModel = null;
        DBModel.StatisticsPaymentAmount _StatisticsPaymentAmount = new DBModel.StatisticsPaymentAmount();
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = " SELECT ProcessStatus," +
            " SUM(PartialOrderAmount) AS SumOrderAmount," +
            " (Select SUM(P.PartialOrderAmount) WHERE(P.ProcessStatus = 4 OR P.ProcessStatus = 2)) AS SumSuccessOrderAmount," +
            " COUNT(*) AS OrderCount," +
            " (Select " +
            " SUM(CASE WHEN PC.DecimalPlaces=1 THEN ROUND(PartialOrderAmount * CollectRate * 0.01,0) " +
            " ELSE PartialOrderAmount * CollectRate * 0.01 END)" +
            " WHERE(P.ProcessStatus = 4 OR P.ProcessStatus = 2)) AS SumCharge," +
            " (Select SUM(CASE WHEN PC.DecimalPlaces=1 THEN ROUND(PartialOrderAmount * CollectRate * 0.01,0) " +
            " ELSE PartialOrderAmount * CollectRate * 0.01 END) + SUM(PaymentAmount) WHERE(P.ProcessStatus = 4 OR P.ProcessStatus = 2)) AS SumChargeAndSuccessOrderAmount," +
            " (Select SUM(CASE WHEN PC.DecimalPlaces=1 THEN ROUND(PartialOrderAmount * (CollectRate- CostRate) * 0.01,0) " +
            " ELSE PartialOrderAmount * (CollectRate- CostRate) * 0.01 END) WHERE(P.ProcessStatus = 4 OR P.ProcessStatus = 2)) AS SumProviderCharge" +
            " FROM PaymentTable AS P " +
            " LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode ";

        if (fromBody.TimeType == "F")
        {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.FinishDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.FinishDateUTC) <= @EndDate ";
        }
        else
        {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.CreateDateUTC) <= @EndDate ";
        }
        if (fromBody.CompanyID != -99)
        {//-99代表所有營運商(有選營運商)

            SS += " And P.forCompanyID = @CompanyID ";
        }

        if (fromBody.ProviderCode != "0")
        {//0代表所有供應商)(有選供應商)
            SS += " And P.ProviderCode = @ProviderCode ";
        }

        //商户订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstOrderID = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstOrderID.Count > 0)
            {
                var parameters = new string[LstOrderID.Count];
                for (int i = 0; i < LstOrderID.Count; i++)
                {
                    parameters[i] = string.Format("@OrderID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstOrderID[i]);
                }

                SS += string.Format(" And P.OrderID IN ({0})", string.Join(", ", parameters));
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            var parameters = new string[fromBody.ProcessStatus.Count];

            for (int i = 0; i < fromBody.ProcessStatus.Count; i++)
            {

                parameters[i] = string.Format("@ProcessStatus{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.ProcessStatus[i]);
            }

            SS += string.Format(" And P.ProcessStatus IN ({0})", string.Join(", ", parameters));
        }

        //系统订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstPaymentSerial = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstPaymentSerial.Count > 0)
            {
                var parameters = new string[LstPaymentSerial.Count];
                for (int i = 0; i < LstPaymentSerial.Count; i++)
                {
                    parameters[i] = string.Format("@PaymentSerial{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstPaymentSerial[i]);
                }

                SS += string.Format(" Or P.PaymentSerial IN ({0})", string.Join(", ", parameters));
            }
        }
        if (fromBody.ServiceType != "99")
        {
            SS += " And P.ServiceType=@ServiceType ";
        }

        if (fromBody.StartAmount != 0)
        {
            SS += " And P.OrderAmount >= @StartAmount";

            if (fromBody.EndAmount != 0)
            {
                SS += " And P.OrderAmount <= @EndAmount";
            }
        }

        if (fromBody.EndAmount != 0)
        {
            SS += " And P.OrderAmount <= @EndAmount";

            if (fromBody.StartAmount != 0)
            {
                SS += " And P.OrderAmount >= @StartAmount";
            }
        }

        if (fromBody.SubmitType != 99)
        {
            SS += " And P.SubmitType >= @SubmitType";
        }

        SS += " GROUP BY ProcessStatus ";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@StartAmount", System.Data.SqlDbType.Decimal).Value = fromBody.StartAmount;
        DBCmd.Parameters.Add("@EndAmount", System.Data.SqlDbType.Decimal).Value = fromBody.EndAmount;
        DBCmd.Parameters.Add("@SubmitType", System.Data.SqlDbType.Int).Value = fromBody.SubmitType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                PointBySearchFilterModel = DataTableExtensions.ToList<DBModel.PointBySearchFilter>(DT).ToList();
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            PointBySearchFilterModel = PointBySearchFilterModel.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        }

        if (PointBySearchFilterModel.Count > 0)
        {
            _StatisticsPaymentAmount.OrderCount = PointBySearchFilterModel.Sum(s => s.OrderCount);
            _StatisticsPaymentAmount.SuccessOrderCount = PointBySearchFilterModel.Where(w => w.ProcessStatus == 2 || w.ProcessStatus == 4).Sum(s => s.OrderCount);



            _StatisticsPaymentAmount.ProfitAmount = PointBySearchFilterModel.Where(w => w.ProcessStatus == 2 || w.ProcessStatus == 4).Sum(s => s.SumProviderCharge);

            _StatisticsPaymentAmount.SumCharge = PointBySearchFilterModel.Where(w => w.ProcessStatus == 2 || w.ProcessStatus == 4).Sum(s => s.SumCharge);

            _StatisticsPaymentAmount.SumChargeAndSuccessOrderAmount = PointBySearchFilterModel.Where(w => w.ProcessStatus == 2 || w.ProcessStatus == 4).Sum(s => s.SumChargeAndSuccessOrderAmount);

            _StatisticsPaymentAmount.SumSuccessAmount = PointBySearchFilterModel.Where(w => w.ProcessStatus == 2 || w.ProcessStatus == 4).Sum(s => s.SumSuccessOrderAmount);

            _StatisticsPaymentAmount.SumOrderAmount = PointBySearchFilterModel.Sum(s => s.SumOrderAmount);
        }




        return _StatisticsPaymentAmount;
    }

    public List<DBModel.PaymentReportV2> GetPaymentResultV2(FromBody.GetPaymentForAdminV2 fromBody)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReportV2> returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();

        SS = " WITH T";
        SS += " AS(";
        SS += " SELECT P.* ,  ";
        SS += "       PDB.TransferRemark, ";
        SS += "       PBC.BankName , ";
        SS += "       PBC.BranchName, ";
        SS += "       PBC.BankNumber, ";
        SS += "       PBC.AccountName, ";
        SS += "       PBC.BankCardName, ";
        SS += "       S.ServiceTypeName, ";
        SS += "       PC.ProviderName, ";
        SS += "       PC.DecimalPlaces, ";
        SS += "       C.CompanyName, ";
        SS += "       C.CompanyCode, ";
        SS += "       C.MerchantCode, ";
        SS += "       convert(varchar, DATEADD(HOUR, @TimeZone, P.CreateDateUTC), 120) as CreateDate2, ";
        SS += "       convert(varchar, DATEADD(HOUR, @TimeZone, P.FinishDateUTC), 120) as FinishDate2 ";
        SS += " FROM PaymentTable AS P ";
        SS += " LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType ";
        SS += " LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode ";
        SS += " LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID ";
        SS += " LEFT JOIN PaymentDetailBankCard AS PDB ON PDB.forPaymentID = P.PaymentID ";
        SS += " LEFT JOIN ProviderBankCard AS PBC ON PDB.forBankCardGUID = PBC.BankCardGUID ";

        if (fromBody.TimeType == "F")
        {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.FinishDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.FinishDateUTC) <= @EndDate ";
        }
        else {
            SS += " WHERE DATEADD(HOUR, @TimeZone, P.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.CreateDateUTC) <= @EndDate ";
        }
        #region 筛选条件
        if (fromBody.CompanyID != -99)
        {//-99代表所有營運商(有選營運商)

            SS += " And P.forCompanyID = @CompanyID ";
        }

        if (fromBody.ProviderCode != "0")
        {//0代表所有供應商)(有選供應商)
            SS += " And P.ProviderCode = @ProviderCode ";
        }

        //商户订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstOrderID = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstOrderID.Count > 0)
            {
                var parameters = new string[LstOrderID.Count];
                for (int i = 0; i < LstOrderID.Count; i++)
                {
                    parameters[i] = string.Format("@OrderID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstOrderID[i]);
                }

                SS += string.Format(" And P.OrderID IN ({0})", string.Join(", ", parameters));
            }
        }

        //系统订单号查询
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstPaymentSerial = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstPaymentSerial.Count > 0)
            {
                var parameters = new string[LstPaymentSerial.Count];
                for (int i = 0; i < LstPaymentSerial.Count; i++)
                {
                    parameters[i] = string.Format("@PaymentSerial{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstPaymentSerial[i]);
                }

                SS += string.Format(" Or P.PaymentSerial IN ({0})", string.Join(", ", parameters));
            }
        }

        if (!string.IsNullOrEmpty(fromBody.CompanyName))
        {
            SS += " And C.CompanyName=@CompanyName ";
        }

        if (!string.IsNullOrEmpty(fromBody.ProviderName))
        {
            SS += " And PC.ProviderName=@ProviderName ";
        }

        if (fromBody.ServiceType != "99")
        {
            SS += " And P.ServiceType=@ServiceType ";
        }

        if (fromBody.StartAmount != 0)
        {
            SS += " And P.OrderAmount >= @StartAmount";

            if (fromBody.EndAmount != 0)
            {
                SS += " And P.OrderAmount <= @EndAmount";
            }
        }

        if (fromBody.EndAmount != 0)
        {
            SS += " And P.OrderAmount <= @EndAmount";

            if (fromBody.StartAmount != 0)
            {
                SS += " And P.OrderAmount >= @StartAmount";
            }
        }

        if (fromBody.SubmitType != 99)
        {
            SS += " And P.SubmitType >= @SubmitType";
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            var parameters = new string[fromBody.ProcessStatus.Count];

            for (int i = 0; i < fromBody.ProcessStatus.Count; i++)
            {

                parameters[i] = string.Format("@ProcessStatus{0}", i);
                DBCmd.Parameters.AddWithValue(parameters[i], fromBody.ProcessStatus[i]);
            }

            SS += string.Format(" And P.ProcessStatus IN ({0})", string.Join(", ", parameters));
        }

        if (!string.IsNullOrEmpty(fromBody.search.value))
        {
            SS += " And (P.PaymentSerial like '%'+@SearchFilter+'%' OR  P.OrderID like '%'+@SearchFilter+'%'  OR  C.CompanyName like '%'+@SearchFilter+'%'  OR  PC.ProviderName like '%'+@SearchFilter+'%'  OR  S.ServiceTypeName like '%'+@SearchFilter+'%' OR    P.UserIP like '%'+@SearchFilter+'%'  OR    P.ClientIP like '%'+@SearchFilter+'%' ) ";
        }
        #endregion



        SS += " )";
        SS += " SELECT TotalCount = COUNT(1) OVER( ";
        //#region 排序

        //switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        //{
        //    case "PaymentSerial":
        //        SS += " Order By T.PaymentSerial ";
        //        break;
        //    case "OrderID":
        //        SS += " Order By T.OrderID ";
        //        break;
        //    case "CompanyName":
        //        SS += " Order By T.CompanyName ";
        //        break;
        //    case "ProviderName":
        //        SS += " Order By T.ProviderName ";
        //        break;
        //    case "ServiceTypeName":
        //        SS += " Order By T.ServiceTypeName ";
        //        break;
        //    case "ProcessStatus":
        //        SS += " Order By T.ProcessStatus ";
        //        break;
        //    case "OrderAmount":
        //        SS += " Order By T.OrderAmount ";
        //        break;
        //    case "CollectRate":
        //        SS += " Order By T.CollectRate ";
        //        break;
        //    case "PartialOrderAmount":
        //        SS += " Order By T.PartialOrderAmount ";
        //        break;
        //    case "PaymentAmount":
        //        SS += " Order By T.PaymentAmount ";
        //        break;
        //    case "SubmitType":
        //        SS += " Order By T.SubmitType ";
        //        break;
        //    case "CreateDate2":
        //        SS += " Order By T.CreateDate2 ";
        //        break;
        //    case "FinishDate2":
        //        SS += " Order By T.FinishDate2 ";
        //        break;
        //    case "ClientIP":
        //        SS += " Order By T.ClientIP ";
        //        break;

        //    default:
        //        break;
        //}

        //if (fromBody.order.First().dir == "asc")
        //{
        //    SS += " ASC ";
        //}
        //else
        //{
        //    SS += " DESC ";
        //}
        //#endregion
        SS += " ),T.* ";
        SS += " FROM T";
        #region 排序

        switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        {
            case "TransferRemark":
                SS += " Order By T.TransferRemark ";
                break;
            case "PaymentSerial":
                SS += " Order By T.PaymentSerial ";
                break;
            case "OrderID":
                SS += " Order By T.OrderID ";
                break;
            case "CompanyName":
                SS += " Order By T.CompanyName ";
                break;
            case "ProviderName":
                SS += " Order By T.ProviderName ";
                break;
            case "UserName":
                SS += " Order By T.UserName ";
                break;
            case "ServiceTypeName":
                SS += " Order By T.ServiceTypeName ";
                break;
            case "ProcessStatus":
                SS += " Order By T.ProcessStatus ";
                break;
            case "OrderAmount":
                SS += " Order By T.OrderAmount ";
                break;
            case "CollectRate":
                SS += " Order By T.CollectRate ";
                break;
            case "PartialOrderAmount":
                SS += " Order By T.PartialOrderAmount ";
                break;
            case "PaymentAmount":
                SS += " Order By T.PaymentAmount ";
                break;
            case "SubmitType":
                SS += " Order By T.SubmitType ";
                break;
            case "CreateDate2":
                SS += " Order By T.CreateDate2 ";
                break;
            case "FinishDate2":
                SS += " Order By T.FinishDate2 ";
                break;
            case "ClientIP":
                SS += " Order By T.ClientIP ";
                break;

            default:
                break;
        }

        if (fromBody.order.First().dir == "asc")
        {
            SS += " ASC ";
        }
        else
        {
            SS += " DESC ";
        }
        #endregion

        SS += " OFFSET @pageNo ROWS ";
        SS += " FETCH NEXT @pageSize ROWS ONLY ";



        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@StartAmount", System.Data.SqlDbType.Decimal).Value = fromBody.StartAmount;
        DBCmd.Parameters.Add("@EndAmount", System.Data.SqlDbType.Decimal).Value = fromBody.EndAmount;
        DBCmd.Parameters.Add("@SubmitType", System.Data.SqlDbType.Int).Value = fromBody.SubmitType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = fromBody.ServiceType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@SearchFilter", System.Data.SqlDbType.NVarChar).Value = string.IsNullOrEmpty(fromBody.search.value) ? "" : fromBody.search.value;
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DBCmd.Parameters.Add("@pageNo", System.Data.SqlDbType.Int).Value = fromBody.start;
        DBCmd.Parameters.Add("@pageSize", System.Data.SqlDbType.Int).Value = fromBody.length;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReportV2>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentTableResultByLstPaymentID(FromBody.GetPaymentForAdmin fromBody)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        var parameters = new string[fromBody.PaymentIDs.Count];

        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();

        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P " +
             "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             "LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode ";

        for (int i = 0; i < fromBody.PaymentIDs.Count; i++)
        {
            parameters[i] = string.Format("@PaymentID{0}", i);
            DBCmd.Parameters.AddWithValue(parameters[i], fromBody.PaymentIDs[i]);

        }
        SS += string.Format(" WHERE  P.PaymentID IN ({0})", string.Join(", ", parameters));
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentTableResult(FromBody.GetPaymentForAdmin fromBody, int CompanyType, int LoginCompanyID)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P " +
             "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             "LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode " +
             "WHERE P.CreateDate >= @StartDate And P.CreateDate <= @EndDate ";

        if (fromBody.CompanyID != -99)
        {//-99代表所有營運商(有選營運商)

            SS += " And P.forCompanyID = @CompanyID ";
        }

        if (fromBody.ProviderCode != "0")
        {//0代表所有供應商)(有選供應商)
            SS += " And P.ProviderCode = @ProviderCode ";
        }

        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            LstOrderID = fromBody.OrderID.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstOrderID.Count > 0)
            {
                var parameters = new string[LstOrderID.Count];
                for (int i = 0; i < LstOrderID.Count; i++)
                {
                    parameters[i] = string.Format("@OrderID{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstOrderID[i]);
                }

                SS += string.Format(" And P.OrderID IN ({0})", string.Join(", ", parameters));
            }
        }

        if (!string.IsNullOrEmpty(fromBody.PaymentSerial))
        {
            LstPaymentSerial = fromBody.PaymentSerial.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\r\n", "").Split(',').ToList();
            if (LstPaymentSerial.Count > 0)
            {
                var parameters = new string[LstPaymentSerial.Count];
                for (int i = 0; i < LstPaymentSerial.Count; i++)
                {
                    parameters[i] = string.Format("@PaymentSerial{0}", i);
                    DBCmd.Parameters.AddWithValue(parameters[i], LstPaymentSerial[i]);
                }

                SS += string.Format(" And P.PaymentSerial IN ({0})", string.Join(", ", parameters));
            }
        }

        if (!string.IsNullOrEmpty(fromBody.UserIP))
        {
            SS += " And P.UserIP=@UserIP ";
        }

        if (!string.IsNullOrEmpty(fromBody.ClientIP))
        {
            SS += " And P.ClientIP=@ClientIP ";
        }


        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = fromBody.UserIP;
        DBCmd.Parameters.Add("@ClientIP", System.Data.SqlDbType.VarChar).Value = fromBody.ClientIP;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            returnValue = returnValue.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public List<DBModel.PaymentReport> GetPaymentTableResult2(FromBody.GetPayment fromBody, int LoginCompanyID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = new List<DBModel.PaymentReport>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();

        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       convert(varchar, DATEADD(HOUR, @TimeZone, P.CreateDateUTC), 120) as CreateDate2, " +
             "       convert(varchar, DATEADD(HOUR, @TimeZone, P.FinishDateUTC), 120) as FinishDate2 " +
             "FROM PaymentTable AS P WITH(NOLOCK) " +
             "LEFT JOIN ServiceType AS S WITH(NOLOCK) ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC WITH(NOLOCK) ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C WITH(NOLOCK)  ON C.CompanyID = P.forCompanyID " +
             "WHERE DATEADD(HOUR, @TimeZone, P.CreateDateUTC) >= @StartDate AND DATEADD(HOUR, @TimeZone, P.CreateDateUTC) <= @EndDate " +
             "And P.forCompanyID = @CompanyID";


        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            SS += " And P.OrderID=@OrderID ";
        }

        if (!string.IsNullOrEmpty(fromBody.PaymentSerial))
        {
            SS += " And P.PaymentSerial=@PaymentSerial ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = LoginCompanyID;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = fromBody.PaymentSerial;
        DBCmd.Parameters.Add("@OrderID", System.Data.SqlDbType.VarChar).Value = fromBody.OrderID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        if (!fromBody.ProcessStatus.Contains(99))
        {
            returnValue = returnValue.Where(w => fromBody.ProcessStatus.Contains(w.ProcessStatus)).ToList();
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public DBModel.CreatePatchPayment CreatePayment(int CompanyID, decimal Amount, string Description)
    {
        String SS = String.Empty;
        DBModel.CreatePatchPayment returnValue = new DBModel.CreatePatchPayment();

        string PaymentSerial;
        int PaymentID = 0;

        #region 建立交易單

        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO PaymentTable (forCompanyID, CurrencyType, UserIP, OrderDate,OrderAmount,SubmitType,Description)" +
             "                  VALUES (@CompanyID, @CurrencyType, @UserIP, @OrderDate, @OrderAmount,@SubmitType,@Description);" +
             " SELECT @@IDENTITY";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = Pay.CurrencyType;
        DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = CodingControl.GetUserIP();
        DBCmd.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = Description;

        DBCmd.Parameters.Add("@OrderDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
        DBCmd.Parameters.Add("@OrderAmount", System.Data.SqlDbType.Decimal).Value = Amount;
        DBCmd.Parameters.Add("@SubmitType", System.Data.SqlDbType.Int).Value = 1;


        PaymentID = Convert.ToInt32(DBAccess.GetDBValue(Pay.DBConnStr, DBCmd));

        if (PaymentID == 0)
        {
            //新增交易單失敗
            returnValue.PatchPaymentState = -2;
            return returnValue;
        }

        #region 新建單 => 尚未提交
        PaymentSerial = "IP" + System.DateTime.Now.ToString("yyyyMMddHHmm") + (new string('0', 10 - PaymentID.ToString().Length) + PaymentID.ToString());

        if (UpdatePaymentSerial2(PaymentSerial, PaymentID) == 0)
        {
            //建立單號失敗
            returnValue.PatchPaymentState = -2;
            return returnValue;
        }

        #endregion
        #endregion

        returnValue.PatchPaymentState = 0;
        returnValue.PaymentSerial = PaymentSerial;

        return returnValue;

    }

    public DBModel.CreatePatchPayment CreatePatchPayment(string oldPaymentSerial, decimal Amount, string PatchDescription)
    {
        String SS = String.Empty;
        DBModel.CreatePatchPayment returnValue = new DBModel.CreatePatchPayment();
        DBModel.PaymentTable PaymentModel;
        string PaymentSerial;
        int PaymentID = 0;
        int oldPaymentID;
        PaymentModel = GetPaymentResultByPaymentSerial(oldPaymentSerial);

        #region 查詢舊訂單狀態

        if (PaymentModel == null)
        {
            //找不到舊交易單
            returnValue.PatchPaymentState = -1;
            return returnValue;
        }

        //if (PaymentModel.ProcessStatus == 2 || PaymentModel.ProcessStatus == 4 || PaymentModel.ProcessStatus == 7)
        //{
        //    returnValue.PatchPaymentState = -3;
        //    return returnValue;
        //}

        oldPaymentID = PaymentModel.PaymentID;
        #endregion



        #region 建立交易單

        //產生交易單model
        PaymentModel.ProcessStatus = 7;
        PaymentModel.PatchDescription = PatchDescription;
        PaymentModel.OrderAmount = Amount;
        PaymentModel.forPaymentSerial = PaymentModel.PaymentSerial;
        PaymentModel.OrderDate = DateTime.Now;

        PaymentID = InsertPayment(PaymentModel);

        if (PaymentID == 0)
        {
            //新增交易單失敗
            returnValue.PatchPaymentState = -2;
            return returnValue;
        }

        PaymentModel.PaymentID = PaymentID;

        #region 新建單 => 尚未提交
        PaymentSerial = "IP" + System.DateTime.Now.ToString("yyyyMMddHHmm") + (new string('0', 10 - PaymentModel.PaymentID.ToString().Length) + PaymentModel.PaymentID.ToString());

        if (UpdatePaymentSerial(PaymentSerial, PaymentModel.PaymentID) == 0)
        {
            //建立單號失敗
            returnValue.PatchPaymentState = -2;
            return returnValue;
        }

        PaymentModel.PaymentSerial = PaymentSerial;
        #endregion
        #endregion

        if (!(PaymentModel.ProcessStatus == 2 || PaymentModel.ProcessStatus == 4))
        {
            #region 將舊單改為失敗單
            UpdatePaymentSerialToFail(oldPaymentID);
            #endregion
        }



        returnValue.PatchPaymentState = 0;
        returnValue.PaymentSerial = PaymentSerial;

        return returnValue;

    }

    public static int UpdatePaymentCostRate(decimal CostRate, decimal CollectRate, string PaymentSerial)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue = -1;

        SS = "UPDATE PaymentTable SET CostRate=@CostRate,CollectRate=@CollectRate WHERE PaymentSerial=@PaymentSerial";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@CostRate", System.Data.SqlDbType.Decimal).Value = CostRate;
        DBCmd.Parameters.Add("@CollectRate", System.Data.SqlDbType.Decimal).Value = CollectRate;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public static int UpdatePaymentSerialToFail(int PaymentID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "UPDATE PaymentTable SET ProcessStatus=3 WHERE PaymentID=@PaymentID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentID", System.Data.SqlDbType.Int).Value = PaymentID;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public static int UpdatePaymentSerial(string PaymentSerial, int PaymentID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "UPDATE PaymentTable SET PaymentSerial=@PaymentSerial WHERE ProcessStatus=7 AND PaymentID=@PaymentID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@PaymentID", System.Data.SqlDbType.Int).Value = PaymentID;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public static int UpdatePaymentSerial2(string PaymentSerial, int PaymentID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "UPDATE PaymentTable  SET PaymentSerial=@PaymentSerial,ProcessStatus=1 WHERE  PaymentID=@PaymentID";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@PaymentID", System.Data.SqlDbType.Int).Value = PaymentID;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public static int UpdatePaymentPatchDescriptionAndConfirmAdminID(string PaymentSerial, string PatchDescription, int ConfirmAdminID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int RetValue;

        SS = "UPDATE PaymentTable  SET PatchDescription=@PatchDescription,ConfirmAdminID=@ConfirmAdminID WHERE PaymentSerial=@PaymentSerial ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DBCmd.Parameters.Add("@ConfirmAdminID", System.Data.SqlDbType.Int).Value = ConfirmAdminID;
        DBCmd.Parameters.Add("@PatchDescription", System.Data.SqlDbType.NVarChar).Value = PatchDescription;
        RetValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return RetValue;
    }

    public static int InsertPayment(DBModel.PaymentTable payment)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        int PaymentID;

        SS = "INSERT INTO PaymentTable (forCompanyID, CurrencyType, ServiceType, BankCode, ProviderCode," +
             "                          ProcessStatus, ReturnURL, State, ClientIP, OrderID," +
             "                          OrderDate, OrderAmount, CostRate, CostCharge, CollectRate, CollectCharge, Accounting,PatchDescription,forPaymentSerial)" +
             "                  VALUES (@CompanyID, @CurrencyType, @ServiceType, @BankCode, @ProviderCode, @ProcessStatus, @ReturnURL," +
             "                          @State, @ClientIP, @OrderID, @OrderDate, @OrderAmount," +
             "                          @CostRate, @CostCharge, @CollectRate, @CollectCharge, @Accounting,@PatchDescription,@forPaymentSerial);" +
             " SELECT @@IDENTITY";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = payment.forCompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = payment.CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", System.Data.SqlDbType.VarChar).Value = payment.ServiceType;
        DBCmd.Parameters.Add("@BankCode", System.Data.SqlDbType.VarChar).Value = payment.BankCode;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = payment.ProviderCode;
        DBCmd.Parameters.Add("@ProcessStatus", System.Data.SqlDbType.Int).Value = payment.ProcessStatus;
        DBCmd.Parameters.Add("@ReturnURL", System.Data.SqlDbType.VarChar).Value = payment.ReturnURL;
        DBCmd.Parameters.Add("@State", System.Data.SqlDbType.VarChar).Value = payment.State;
        DBCmd.Parameters.Add("@ClientIP", System.Data.SqlDbType.VarChar).Value = payment.ClientIP;
        DBCmd.Parameters.Add("@OrderID", System.Data.SqlDbType.VarChar).Value = payment.OrderID;
        DBCmd.Parameters.Add("@OrderDate", System.Data.SqlDbType.DateTime).Value = payment.OrderDate;
        DBCmd.Parameters.Add("@OrderAmount", System.Data.SqlDbType.Decimal).Value = payment.OrderAmount;
        DBCmd.Parameters.Add("@CostRate", System.Data.SqlDbType.Decimal).Value = payment.CostRate;
        DBCmd.Parameters.Add("@CostCharge", System.Data.SqlDbType.Decimal).Value = payment.CostCharge;
        DBCmd.Parameters.Add("@CollectRate", System.Data.SqlDbType.Decimal).Value = payment.CollectRate;
        DBCmd.Parameters.Add("@CollectCharge", System.Data.SqlDbType.Decimal).Value = payment.CollectCharge;
        DBCmd.Parameters.Add("@Accounting", System.Data.SqlDbType.Int).Value = 0;
        DBCmd.Parameters.Add("@PatchDescription", System.Data.SqlDbType.NVarChar).Value = payment.PatchDescription;
        DBCmd.Parameters.Add("@forPaymentSerial", System.Data.SqlDbType.VarChar).Value = payment.forPaymentSerial;

        PaymentID = Convert.ToInt32(DBAccess.GetDBValue(Pay.DBConnStr, DBCmd));
        //int.TryParse(DBAccess.GetDBValue(Pay.DBConnStr, DBCmd).ToString(), out PaymentID);

        return PaymentID;
    }

    public DBModel.PaymentTable GetPaymentResultByPaymentSerial(string PaymentSerial)
    {
        DBModel.PaymentTable returnValue = null;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;
        String SS = String.Empty;


        SS = "SELECT * FROM PaymentTable WITH (NOLOCK) WHERE PaymentSerial=@PaymentSerial ";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentTable>(DT).First();
            }
        }

        return returnValue;
    }

    public DBModel.PaymentReport GetPaymentReportByPaymentSerial(string PaymentSerial)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.PaymentReport returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       PPG.GroupName, " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P  WITH (NOLOCK)  " +
             "LEFT JOIN ServiceType AS S  WITH (NOLOCK)  ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC  WITH (NOLOCK)  ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  WITH (NOLOCK)   ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN  ProxyProviderOrder PPO  WITH (NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0 " +
             " LEFT JOIN  ProxyProviderGroup PPG  WITH (NOLOCK)  ON PPO.GroupID= PPG.GroupID  " +
             "WHERE P.PaymentSerial=@PaymentSerial";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public DBModel.PaymentReport GetProxyProviderPaymentReportByPaymentSerial(string PaymentSerial)
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.PaymentReport returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       AT.RealName, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM PaymentTable AS P " +
             "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             " LEFT JOIN AdminTable AT WITH (NOLOCK) ON AT.AdminID=P.ConfirmAdminID " +
             "WHERE P.PaymentSerial=@PaymentSerial";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public DBViewModel.UpdatePatmentResult ConfirmManualPayment(string PaymentSerial, int modifyStatus, int AdminID, string ProviderCode, string ServiceType, int GroupID)
    {
        DBViewModel.UpdatePatmentResult returnValue = new DBViewModel.UpdatePatmentResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.PaymentTable PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
        DBModel.CompanyService CompanyServiceModel = null;
        DBModel.ProviderService ProviderServiceModel = null;
        List<DBModel.Provider> ProviderModel = null;
        //DBModel.ProxyProvider ProxyProviderModel = null;
        DBModel.ProxyProviderGroup ProxyProviderGroupModel = null;
        returnValue.Status = -1;
        if (PaymentData == null)
        {
            returnValue.Message = "订单资讯错误";
            return returnValue;
        }

        if (PaymentData.ProcessStatus == 1 && PaymentData.SubmitType == 1)
        {
            ProviderModel = GetProviderCodeResult(ProviderCode);

            if (ProviderModel == null)
            {
                returnValue.Message = "尚未设定对应供应商";
                return returnValue;
            }

            CompanyServiceModel = GetCompanyService(PaymentData.forCompanyID, ServiceType, PaymentData.CurrencyType);
            if (CompanyServiceModel == null)
            {
                returnValue.Message = "商户尚未设定该支付通道";
                return returnValue;
            }

            if (GroupID > 0)
                ProviderServiceModel = GetProxyProviderByGroupID(ProviderCode, ServiceType, PaymentData.CurrencyType, GroupID);
            else
                ProviderServiceModel = GetProviderServiceByProviderCodeAndServiceType(ProviderCode, ServiceType, PaymentData.CurrencyType);

            if (ProviderServiceModel == null)
            {
                returnValue.Message = "供应商尚未设定该支付通道";
                return returnValue;
            }
            if (UpdatePaymentCostRate(ProviderServiceModel.CostRate, CompanyServiceModel.CollectRate, PaymentSerial) <= 0)
            {
                returnValue.Message = "更改订单费率失败";
                return returnValue;
            }

            if (ProviderModel.First().CollectType == 1 && modifyStatus != 3)
            {
                //ProxyProviderModel = GetProxyProviderResult(ProviderCode);
                //if (ProxyProviderModel == null) {
                //    returnValue.Message = "尚未设定专属供应商费率";
                //    return returnValue;
                //}

                ProxyProviderGroupModel = GetProxyProviderGroupByGroupID(ProviderCode, GroupID);

                if (ProxyProviderGroupModel == null)
                {
                    returnValue.Message = "找不到专属供应商资料";
                    return returnValue;
                }

                if (ProxyProviderGroupModel.PaymentRate == 0)
                {
                    returnValue.Message = "尚未设定专属供应商费率";
                    return returnValue;
                }

                if (UpdatePaymentSerialByProxyProviderOrder(PaymentSerial, 8, ProviderCode, ServiceType) > 0)
                {


                    if (GetProxyProviderOrderByOrderSerial(PaymentSerial, 0) == null)
                    {

                        InsertProxyProviderOrder(PaymentSerial, 0, 0, ProxyProviderGroupModel.PaymentRate, GroupID);
                    }


                    returnValue.Message = "上游审核中";
                    returnValue.Status = 0;
                }
                else
                {
                    returnValue.Message = "修改订单状态失败";
                }
            }
            else
            {

                //人工充值
                SS = "spSetManualPayment";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
                DBCmd.Parameters.Add("@ProcessStatus", SqlDbType.Int).Value = modifyStatus;

                DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
                DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                switch (DBreturn)
                {
                    case 0://成功
                        returnValue.Message = "审核完成";
                        returnValue.Status = 0;
                        break;
                    case -1://交易單不存在
                        returnValue.Message = "交易单不存在";
                        //UpdateWithdrawal(WithdrawSerial, "交易單不存在");
                        break;
                    case -2://交易資料有誤 
                        returnValue.Message = "交易资料有误";
                        //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額錯誤");
                        break;

                    case -4://鎖定失敗
                        returnValue.Message = "锁定失败";
                        //UpdateWithdrawal(WithdrawSerial, "鎖定失敗");
                        break;
                    case -5://加扣點失敗
                        returnValue.Message = "加扣点失败";
                        //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                        break;
                    case -9://加扣點失敗
                        returnValue.Message = "订单审核中";
                        //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                        break;
                    default://其他錯誤
                        returnValue.Message = "其他错误";
                        //UpdateWithdrawal(WithdrawSerial, "其他錯誤");
                        break;
                }

            }
        }
        else
        {
            returnValue.Message = "目前订单状态无法审核";
        }

        returnValue.PaymentData = GetPaymentReportByPaymentSerial(PaymentSerial);
        return returnValue;
    }

    public DBViewModel.UpdateWithdrawalResult SavePaymentConfirm(string PaymentSerial, decimal PatchAmount, string ProviderOrderID)
    {
        DBViewModel.UpdateWithdrawalResult returnValue = new DBViewModel.UpdateWithdrawalResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
        //找不到訂單資訊
        if (PaymentData != null && PaymentData.PaymentSerial != "")
        {
            if (PaymentData.ProcessStatus == 7)
            {
                //扣除公司額度
                SS = "spSetSupPayment";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@PaymentID", SqlDbType.Int).Value = PaymentData.PaymentID;
                DBCmd.Parameters.Add("@ProviderOrderAmount", SqlDbType.Decimal).Value = PatchAmount;
                DBCmd.Parameters.Add("@ProviderOrderID", SqlDbType.VarChar).Value = ProviderOrderID;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;
                returnValue = new DBViewModel.UpdateWithdrawalResult();
                switch (DBreturn)
                {
                    case 0://成功
                        returnValue.Message = "審核完成";
                        break;
                    case -1://交易單不存在
                        returnValue.Message = "交易單不存在";
                        //UpdateWithdrawal(WithdrawSerial, "交易單不存在");
                        break;
                    case -2://交易資料有誤 
                        returnValue.Message = "交易資料有誤";
                        //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額錯誤");
                        break;

                    case -4://鎖定失敗
                        returnValue.Message = "鎖定失敗";
                        //UpdateWithdrawal(WithdrawSerial, "鎖定失敗");
                        break;
                    case -5://加扣點失敗
                        returnValue.Message = "加扣點失敗";
                        //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                        break;
                    default://其他錯誤
                        returnValue.Message = "其他錯誤";
                        //UpdateWithdrawal(WithdrawSerial, "其他錯誤");
                        break;
                }

                var newPaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
                returnValue.Status = newPaymentData.ProcessStatus;
                returnValue.PaymentAmount = newPaymentData.PaymentAmount;
            }
            else
            {
                returnValue.Message = "訂單狀態錯誤";
                returnValue.Status = -1;
            }

        }
        else
        {
            returnValue.Message = "訂單狀態錯誤";
            returnValue.Status = -1;

        }

        return returnValue;
    }

    /// <summary>
    /// 充值单未处理转成功
    /// </summary>
    /// <param name="PaymentSerial"></param>
    /// <returns></returns>
    public string ChangePaymentProcessStatus(string PaymentSerial, int AdminID,Decimal RealAmount)
    {
        string returnValue = "其他错误";
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
        //找不到訂單資訊
        if (PaymentData != null && PaymentData.PaymentSerial != "")
        {
            if (PaymentData.ProcessStatus == 1)
            {
                //扣除公司額度
                SS = "spSetPaymentToSuccess";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
                DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
                DBCmd.Parameters.Add("@RealAmount", SqlDbType.Decimal).Value = RealAmount;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                switch (DBreturn)
                {
                    case 0://成功
                        ReSendPaymentByManualPayment(PaymentSerial);
                        returnValue = "审核完成";
                        break;
                    case -1://交易單不存在
                        returnValue = "交易单不存在";
                        break;
                    case -2://交易資料有誤 
                        returnValue = "交易资料有误";
                        break;
                    case -3://更新商戶錢包失敗 
                        returnValue = "更新商户钱包失败";
                        break;
                    case -4://鎖定失敗
                        returnValue = "锁定失败";
                        break;
                    case -5://更新供應商錢包失敗
                        returnValue = "更新供应商钱包失败";
                        break;
                    default://其他錯誤
                        returnValue = "其他错误";
                        break;
                }

            }
            else
            {
                returnValue = "订单状态错误";
            }

        }
        else
        {
            returnValue = "订单不存在";
        }

        return returnValue;
    }

    /// <summary>
    /// 充值单成功转失败
    /// </summary>
    /// <param name="PaymentSerial"></param>
    /// <returns></returns>
    public string ChangePaymentProcessStatusSuccessToFail(string PaymentSerial, int AdminID)
    {
        string returnValue = "其他错误";
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
        //找不到訂單資訊
        if (PaymentData != null && PaymentData.PaymentSerial != "")
        {
            if (PaymentData.ProcessStatus == 2 || PaymentData.ProcessStatus == 4)
            {
                //扣除公司額度
                SS = "spSetPaymentToFail";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
                DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                switch (DBreturn)
                {
                    case 0://成功
                           //ReSendPaymentByManualPayment(PaymentSerial);
                        returnValue = "审核完成";
                        break;
                    case -1://交易單不存在
                        returnValue = "交易单不存在";
                        break;
                    case -2://交易資料有誤 
                        returnValue = "交易资料有误";
                        break;
                    case -3://更新商戶錢包失敗 
                        returnValue = "更新商户钱包失败";
                        break;
                    case -4://鎖定失敗
                        returnValue = "锁定失败";
                        break;
                    case -5://更新供應商錢包失敗
                        returnValue = "更新供应商钱包失败";
                        break;
                    default://其他錯誤
                        returnValue = "其他错误";
                        break;
                }

            }
            else
            {
                returnValue = "订单状态错误";
            }

        }
        else
        {
            returnValue = "订单不存在";
        }

        return returnValue;
    }


    /// <summary>
    /// 提现单成功转失败
    /// </summary>
    /// <param name="WithdrawSerial"></param>
    /// <param name="AdminID"></param>
    /// <returns></returns>
    public string ChangeWithdrawalProcessStatus(string WithdrawSerial, int AdminID)
    {
        string returnValue = "其他错误";
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var WithdrawalData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        //找不到訂單資訊
        if (WithdrawalData != null && WithdrawalData.WithdrawSerial != "")
        {
            if (WithdrawalData.Status == 2)
            {
                SS = "spReviewWithdrawaltoFail";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;

                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                switch (DBreturn)
                {
                    case 0:
                        returnValue = "审核完成";
                        if (!(WithdrawalData.DownUrl == "https://www.baidu.com/" || WithdrawalData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawalData.WithdrawSerial, false);
                        }
                        break;
                    case -1:
                        returnValue = "交易单不存在";
                        break;
                    case -2:
                        returnValue = "交易單狀態有誤";
                        break;
                    case -3:
                        returnValue = "鎖定失敗";
                        break;
                    case -4:
                        returnValue = "修改供应商钱包失败";
                        break;
                    case -5:
                        returnValue = "修改商户钱包失败";
                        break;
                    default:
                        returnValue = "其他错误";
                        break;
                }

            }
            else
            {
                returnValue = "訂單狀態錯誤";
            }

        }
        else
        {
            returnValue = "訂單狀態錯誤";
        }

        return returnValue;
    }

    /// <summary>
    /// 提现单失败转成功
    /// </summary>
    /// <param name="WithdrawSerial"></param>
    /// <param name="AdminID"></param>
    /// <returns></returns>
    public string ChangeWithdrawalProcessStatusFailToSuccess(string WithdrawSerial, int AdminID)
    {
        string returnValue = "其他错误";
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var WithdrawalData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        //找不到訂單資訊
        if (WithdrawalData != null && WithdrawalData.WithdrawSerial != "")
        {
            if (WithdrawalData.Status == 3)
            {
                SS = "spReviewWithdrawalFailToSuccess";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;

                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                switch (DBreturn)
                {
                    case 0:
                        returnValue = "审核完成";

                        if (!(WithdrawalData.DownUrl == "https://www.baidu.com/" || WithdrawalData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawalData.WithdrawSerial, true);
                        }
                        break;
                    case -1:
                        returnValue = "交易单不存在";
                        break;
                    case -2:
                        returnValue = "交易單狀態有誤";
                        break;
                    case -3:
                        returnValue = "鎖定失敗";
                        break;
                    case -4:
                        returnValue = "修改供应商钱包失败";
                        break;
                    case -5:
                        returnValue = "修改商户钱包失败";
                        break;
                    default:
                        returnValue = "其他错误";
                        break;
                }

            }
            else
            {
                returnValue = "訂單狀態錯誤";
            }

        }
        else
        {
            returnValue = "訂單狀態錯誤";
        }

        return returnValue;
    }

    /// <summary>
    /// 提现单進行中转成功
    /// </summary>
    /// <param name="WithdrawSerial"></param>
    /// <param name="AdminID"></param>
    /// <returns></returns>
    public string CancelWithdrawalReviewToSuccess(string WithdrawSerial, int AdminID)
    {
        string returnValue = "其他错误";
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        var WithdrawalData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        //找不到訂單資訊
        if (WithdrawalData != null && WithdrawalData.WithdrawSerial != "")
        {
            if (WithdrawalData.Status == 1)
            {
                //扣除公司額度
                SS = "spCancelWithdrawalReviewToSuccess";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);
                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                if (DBreturn == 0)
                {
                    returnValue = "审核完成";
                    //手動到後台下發
                    SS = " UPDATE Withdrawal  SET ConfirmByAdminID=@AdminID " +
                         " WHERE WithdrawSerial=@WithdrawSerial";

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = AdminID;
                    DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    //如果不是后台申请提现,发送API回调
                    if (WithdrawalData.FloatType != 0)
                    {
                        if (!(WithdrawalData.DownUrl == "https://www.baidu.com/" || WithdrawalData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawalData.WithdrawSerial, false);
                        }
                    }
                }
                else {
                    switch (DBreturn)
                    {
                        case -1:
                            returnValue = "订单不存在";
                            break;
                        case -2:
                            returnValue = "订单状态有误";
                            break;
                        case -3:
                            returnValue = "锁定失败";
                            break;
                        case -4:
                            returnValue = "商户钱包有误";
                            break;
                        case -5:
                            returnValue = "商户余额不足";
                            break;
                        case -6:
                            returnValue = "家扣点失败";
                            break;
                        case -7:
                            returnValue = "订单非在可充值状态";
                            break;
                        default:
                            returnValue = "其他错误";
                            break;
                    }
                }
            }
            else
            {
                returnValue = "訂單狀態錯誤";
            }
        }
        else
        {
            returnValue = "訂單狀態錯誤";
        }

        return returnValue;
    }

    /// <summary>
    /// 提现单進行中转失败
    /// </summary>
    /// <param name="WithdrawSerial"></param>
    /// <param name="AdminID"></param>
    /// <returns></returns>
    public string CancelWithdrawalReviewToFail(string WithdrawSerial,int AdminID)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        string returnValue;
        int DBreturn = -6;
        var WithdrawalData = GetWithdrawalByWithdrawSerial(WithdrawSerial);
        //找不到訂單資訊
        if (WithdrawalData != null && WithdrawalData.WithdrawSerial != "")
        {
            if (WithdrawalData.Status == 1)
            {
                //扣除公司額度
                SS = "spCancelWithdrawalReviewToFail";
                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.StoredProcedure;
                DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
                DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
                DBAccess.ExecuteDB(DBConnStr, DBCmd);

                DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                if (DBreturn == 0)
                {
                    returnValue = "审核完成";
                    //如果不是后台申请提现,发送API回调
                    if (WithdrawalData.FloatType != 0)
                    {
                        if (!(WithdrawalData.DownUrl == "https://www.baidu.com/" || WithdrawalData.DownUrl == "http://baidu.com"))
                        {
                            ReSendWithdrawal(WithdrawalData.WithdrawSerial, false);
                        }
                    }
                }
                else {
                    switch (DBreturn)
                    {
                        case -1:
                            returnValue = "订单不存在";
                            break;
                        case -2:
                            returnValue = "订单状态有误";
                            break;
                        case -3:
                            returnValue = "锁定失败";
                            break;
                        case -4:
                            returnValue = "此订单非专属供应商订单";
                            break;
                        default:
                            returnValue = "其他错误";
                            break;
                    }
                }
            }
            else
            {
                returnValue = "訂單狀態錯誤";
            }
        }
        else
        {
            returnValue = "訂單狀態錯誤";
        }

        return returnValue;
    }


    public int CancelWithdrawalProviderReview(string WithdrawSerial)
    {

        String SS = String.Empty;
        SqlCommand DBCmd;
        int ReturnValue = 0;


        SS = "spCancelWithdrawalReview";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBCmd.Parameters.Add("@WithdrawSerial", SqlDbType.VarChar).Value = WithdrawSerial;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        ReturnValue = (int)DBCmd.Parameters["@Return"].Value;

        return ReturnValue;
    }

    public List<DBModel.Withdrawal> GetQueryWithdrawal()
    {
        List<DBModel.Withdrawal> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT WithdrawID ";
        SS += " FROM Withdrawal  WITH (NOLOCK) ";
        SS += " Where UpStatus=1 And Status=1 ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.Withdrawal>(DT).ToList();
            }
        }

        return returnValue;
    }

    public APIResult ReSendPayment(string PaymentSerial, int CompanyID)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];
        APIResult objReturnValue = new APIResult();
        var PaymentModel = GetPaymentReportByPaymentSerial(PaymentSerial);

        if (PaymentModel == null)
        {
            objReturnValue.Status = ResultStatus.ERR;
            objReturnValue.Message = "查询不到此订单";
            return objReturnValue;
        }

        if (CompanyID != 1)
        {
            if (PaymentModel.forCompanyID != CompanyID)
            {
                objReturnValue.Status = ResultStatus.ERR;
                objReturnValue.Message = "查询不到此订单";
                return objReturnValue;
            }
        }

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        strSign = string.Format("PaymentSerial={0}&GPayBackendKey={1}"
                                  , PaymentSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);
        objReturnValue.Message = "sign";
        objReturnValue.Status = ResultStatus.ERR;
        #endregion

        var _ReSendPayment = new DBModel.ReSendPaymentSet();

        _ReSendPayment.PaymentSerial = PaymentSerial;
        _ReSendPayment.Sign = sign;

        System.Threading.Tasks.Task.Run(() =>
        {
            var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "ReSendPayment", JsonConvert.SerializeObject(_ReSendPayment));
        });

        objReturnValue.Status = ResultStatus.OK;

        return objReturnValue;
    }

    public List<DBModel.PaymentTransferLog> PaymentRecord(string PaymentSerial)
    {
        List<DBModel.PaymentTransferLog> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT *,convert(varchar, CreateDate, 121) as CreateDate2 From PaymentTransferLog WITH (NOLOCK) ";
        SS += " Where forPaymentSerial=@PaymentSerial";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentTransferLog>(DT).ToList();
            }
        }

        return returnValue;
    }

    public APIResult ReSendPaymentByManualPayment(string PaymentSerial)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        APIResult objReturnValue = new APIResult();
        strSign = string.Format("PaymentSerial={0}&GPayBackendKey={1}"
                                  , PaymentSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);
        objReturnValue.Message = "sign";
        objReturnValue.Status = ResultStatus.ERR;
        #endregion

        var _ReSendPayment = new DBModel.ReSendPaymentSet();

        _ReSendPayment.PaymentSerial = PaymentSerial;
        _ReSendPayment.Sign = sign;

        var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "ReSendPaymentByManualPayment", JsonConvert.SerializeObject(_ReSendPayment));

        if (!string.IsNullOrEmpty(returnValue))
        {
            objReturnValue = JsonConvert.DeserializeObject<APIResult>(returnValue);
        }

        return objReturnValue;
    }

    public void ReSendWithdrawal(string WithdrawSerial, bool isReSendWithdraw)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        APIResult objReturnValue = new APIResult();
        strSign = string.Format("WithdrawSerial={0}&GPayBackendKey={1}"
                                  , WithdrawSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);

        #endregion

        var _ReSendWithdraw = new DBModel.ReSendWithdrawSet();
        _ReSendWithdraw.isReSendWithdraw = isReSendWithdraw;
        _ReSendWithdraw.WithdrawSerial = WithdrawSerial;
        _ReSendWithdraw.Sign = sign;

        System.Threading.Tasks.Task.Run(() =>
        {
            CodingControl.RequestJsonAPI(GPayApiUrl + "ReSendWithdraw", JsonConvert.SerializeObject(_ReSendWithdraw), WithdrawSerial, "");
        });
        //var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "ReSendWithdraw", JsonConvert.SerializeObject(_ReSendWithdraw), WithdrawSerial, "");


        //if (!string.IsNullOrEmpty(returnValue))
        //{
        //    objReturnValue = JsonConvert.DeserializeObject<APIResult>(returnValue);
        //}

        //return objReturnValue;
    }

    public APIResult ReSendWithdrawal2(string WithdrawSerial, bool isReSendWithdraw)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        APIResult objReturnValue = new APIResult();
        strSign = string.Format("WithdrawSerial={0}&GPayBackendKey={1}"
                                  , WithdrawSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);

        #endregion
        var _ReSendWithdraw = new DBModel.ReSendWithdrawSet();
        _ReSendWithdraw.isReSendWithdraw = isReSendWithdraw;
        _ReSendWithdraw.WithdrawSerial = WithdrawSerial;
        _ReSendWithdraw.Sign = sign;

        var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "ReSendWithdraw", JsonConvert.SerializeObject(_ReSendWithdraw), WithdrawSerial, "");


        if (!string.IsNullOrEmpty(returnValue))
        {
            objReturnValue = JsonConvert.DeserializeObject<APIResult>(returnValue);
        }

        return objReturnValue;
    }

    public List<DBModel.CompanyPointHistory> WithdrawalRecord(string WithdrawalSerial)
    {
        List<DBModel.CompanyPointHistory> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        DBModel.Withdrawal WithdrawalModel = GetWithdrawalByWithdrawSerial(WithdrawalSerial);
        if (WithdrawalModel == null)
        {
            return returnValue;
        }

        SS = " SELECT ServiceTypeName,CompanyPointHistory.*,convert(varchar, CreateDate, 121) as CreateDate2 From CompanyPointHistory WITH (NOLOCK) ";
        SS += " LEFT JOIN ServiceType ON ServiceType.ServiceType=CompanyPointHistory.ServiceType AND ServiceType.CurrencyType=CompanyPointHistory.CurrencyType";
        SS += " Where TransactionID=@TransactionID And OperatorType=4";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = WithdrawalModel.WithdrawID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyPointHistory>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region TestPage
    public List<DBViewModel.TestPageCompanyService> GetTestPageCompanyService(int CompanyID)
    {
        List<DBViewModel.TestPageCompanyService> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " Select ServiceTypeName,CP.*, STUFF((Select  distinct(',' + ProviderName)  From GPayRelation GP" +
             " LEFT JOIN ProviderCode PC ON PC.ProviderCode = GP.ProviderCode where CP.forCompanyID = @CompanyID" +
             " AND GP.ServiceType = CP.ServiceType  For Xml Path('')) , 1, 1, '') as ProviderName,PS.State ProviderState" +
             " From CompanyService CP LEFT JOIN ServiceType ST ON ST.ServiceType = CP.ServiceType" +
             " LEFT JOIN GPayRelation GR ON GR.forCompanyID = CP.forCompanyID AND GR.ServiceType = CP.ServiceType AND CP.CurrencyType = GR.CurrencyType" +
             " LEFT JOIN ProviderService PS ON GR.ServiceType = PS.ServiceType AND GR.ProviderCode = PS.ProviderCode   where CP.forCompanyID = @CompanyID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.TestPageCompanyService>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBViewModel.TestPageCompanyService> GetTestPageCompanyService2(int CompanyID)
    {
        List<DBViewModel.TestPageCompanyService> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " select ServiceTypeName,CP.*," +
             " STUFF((" +
             " Select  ',' + ProviderName" +
             " From GPayRelation GP LEFT JOIN" +
             " ProviderCode PC ON PC.ProviderCode = GP.ProviderCode" +
             " where forCompanyID = @CompanyID AND GP.ServiceType = CP.ServiceType AND GP.CurrencyType = 'JPY'" +
             " For Xml Path(''))" +
             " , 1, 1, '') as ProviderName,PS.State ProviderState " +
             " From CompanyService CP" +
             " LEFT JOIN ServiceType ST ON ST.ServiceType=CP.ServiceType" +
             "  LEFT JOIN GPayRelation GR ON GR.forCompanyID = CP.forCompanyID AND GR.ServiceType = CP.ServiceType " +
             " LEFT JOIN ProviderService PS ON GR.ServiceType = PS.ServiceType AND GR.ProviderCode = PS.ProviderCode " +
             " where  CP.forCompanyID = @CompanyID And CP.CurrencyType = 'JPY' And CP.State=0";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CompanyID;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.TestPageCompanyService>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region 左邊列表
    public List<DBViewModel.LayoutLeftSideBarResult> GetPermissionTableResultbyAdminID(int AdminID)
    {
        List<DBViewModel.LayoutLeftSideBarResult> returnValue = null;
        string SS;
        SqlCommand DBCmd = null;
        DataTable DT;

        SS = " SELECT PermissionTable.*," +
             "        PermissionCategory.Description AS CategoryDescription," +
             "        PermissionCategory.PermissionCategoryName" +
             " FROM   PermissionTable" +
             "        INNER JOIN PermissionCategory" +
             "                ON PermissionCategory.PermissionCategoryID = PermissionTable.PermissionCategoryID" +
             " WHERE  PermissionName IN(SELECT AdminRolePermission.forPermissionName" +
             "                          FROM   AdminRolePermission" +
             "                          WHERE  forAdminRoleID IN (SELECT AdminTable.forAdminRoleID" +
             "                                                    FROM   AdminTable" +
             "                                                    WHERE  AdminID = @AdminID)) " +
             " ORDER  BY PermissionCategory.SortIndex,PermissionTable.SortIndex ";


        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@AdminID", System.Data.SqlDbType.Int).Value = AdminID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.LayoutLeftSideBarResult>(DT) as List<DBViewModel.LayoutLeftSideBarResult>;
            }
        }

        return returnValue;
    }
    #endregion

    #region 操作Log
    public int InsertAdminOPLog(int CompanyID, int AdminID, int Type, string Description, string IP)
    {
        string SS;
        int AdminOPID;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = "INSERT INTO AdminOPLog (forCompanyID,forAdminID,Type,Description,IP) " +
         "                    VALUES (@forCompanyID,@forAdminID,@Type,@Description,@IP) SELECT @@IDENTITY;";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;
        DBCmd.Parameters.Add("@forAdminID", SqlDbType.NVarChar).Value = AdminID;
        DBCmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
        //DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        AdminOPID = int.Parse(DBAccess.GetDBValue(Pay.DBConnStr, DBCmd).ToString());
        return AdminOPID;
    }

    public List<DBViewModel.AdminOPLogVM> GetAdminOPLogResult(FromBody.GetAdminOPLogResult fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBViewModel.AdminOPLogVM> returnValue = new List<DBViewModel.AdminOPLogVM>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT AOP.*, " +
             "        AT.LoginAccount, " +
             "        CT.CompanyName, " +
             "        CONVERT(VARCHAR, DATEADD(HOUR, @TimeZone - 8, AOP.CreateDate), 120) AS CreateDate2 " +
             " FROM   AdminOPLog AOP " +
             "        LEFT JOIN AdminTable AT ON AOP.forAdminID = AT.AdminID " +
             "        LEFT JOIN CompanyTable CT ON AOP.forCompanyID = CT.CompanyID " +
             " WHERE  DATEADD(HOUR, @TimeZone - 8, AOP.CreateDate) BETWEEN @StartDate AND @EndDate  ";

        if (fromBody.Type != -1)
        {
            SS += "        AND AOP.Type = @Type ";
        }

        if (fromBody.CompanyID != -99)
        {
            SS += "        AND AOP.forCompanyID = @CompanyID ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DBCmd.Parameters.Add("@Type", System.Data.SqlDbType.Int).Value = fromBody.Type;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@TimeZone", System.Data.SqlDbType.Int).Value = Pay.TimeZone;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBViewModel.AdminOPLogVM>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }
    #endregion

    #region ManualHistory
    public List<DBModel.ProviderManualHistory> GetProviderManualHistoryResult(FromBody.GetProviderManualHistory fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.ProviderManualHistory> returnValue = new List<DBModel.ProviderManualHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT PMH.*, " +
                 "        AT.LoginAccount,PC.ProviderName, " +
                 "        CONVERT(VARCHAR, PMH.CreateDate, 120) AS CreateDate2 " +
                 " FROM   ProviderManualHistory PMH " +
                 "        LEFT JOIN AdminTable AT " +
                 "               ON PMH.forAdminID = AT.AdminID " +
                  "        LEFT JOIN ProviderCode PC " +
                 "               ON PC.ProviderCode = PMH.ProviderCode " +
                 " WHERE  PMH.CreateDate >= @StartDate And PMH.CreateDate <= @EndDate ";

        if (fromBody.ProviderCode != "-99")
        {
            SS += "        AND PMH.ProviderCode = @ProviderCode ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.ProviderManualHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public DBModel.PaymentReport GetOrderByCompanyManualHistoryByFrozenPoint(string TransactionSerial)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.PaymentReport returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        if (TransactionSerial.Contains("IP") || TransactionSerial.Contains("PD"))
        {
            SS = "SELECT P.* ,  " +
                 "       PPG.GroupName, " +
                 "       S.ServiceTypeName, " +
                 "       PC.ProviderName, " +
                 "       C.CompanyName, " +
                 "       C.CompanyCode, " +
                 "       C.MerchantCode, " +
                 "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
                 "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
                 "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
                 " FROM PaymentTable AS P WITH(NOLOCK) " +
                 " LEFT JOIN ServiceType AS S WITH(NOLOCK)  ON P.ServiceType = S.ServiceType " +
                 " LEFT JOIN ProviderCode AS PC WITH(NOLOCK)  ON PC.ProviderCode = P.ProviderCode " +
                 " LEFT JOIN CompanyTable AS C WITH(NOLOCK)   ON C.CompanyID = P.forCompanyID " +
                 " LEFT JOIN  ProxyProviderOrder PPO WITH(NOLOCK)  ON PPO.forOrderSerial= P.PaymentSerial AND PPO.Type=0" +
                 " LEFT JOIN  ProxyProviderGroup PPG WITH(NOLOCK)  ON PPO.GroupID= PPG.GroupID " +
                 " WHERE PaymentSerial=@TransactionSerial ";
        }
        else
        {
            SS = " SELECT ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,Amount AS OrderAmount,CompanyName,Status AS ProcessStatus,Withdrawal.WithdrawType as Accounting FROM Withdrawal WITH (NOLOCK) " +
                 " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
                 " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
                 " WHERE WithdrawSerial=@TransactionSerial ";
            //SS = " SELECT CollectCharge,Amount AS OrderAmount FROM Withdrawal WITH (NOLOCK) WHERE WithdrawSerial=@TransactionSerial ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@TransactionSerial", System.Data.SqlDbType.VarChar).Value = TransactionSerial;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public DBModel.PaymentReport GetOrderByCompanyManualHistory(string TransactionSerial)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.PaymentReport returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        if (TransactionSerial.Contains("IP")|| TransactionSerial.Contains("PD"))
        {
            SS = "SELECT P.* ,  " +
                 "       PDB.TransferRemark, " +
                 "       S.ServiceTypeName, " +
                 "       PC.ProviderName, " +
                 "       C.CompanyName, " +
                 "       C.CompanyCode, " +
                 "       C.MerchantCode, " +
                 "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
                 "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
                 "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
                 " FROM PaymentTable AS P " +
                 " LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
                 " LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
                 " LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
                 " LEFT JOIN PaymentDetailBankCard AS PDB ON PDB.forPaymentID = P.PaymentID "+
                 " WHERE PaymentSerial=@TransactionSerial ";
        }
        else
        {
            SS = " SELECT ProviderName,convert(varchar,Withdrawal.CreateDate,120) as CreateDate2,convert(varchar,Withdrawal.FinishDate,120) as FinishDate2,Withdrawal.*,Amount AS OrderAmount,CompanyName,Status AS ProcessStatus,Withdrawal.WithdrawType as Accounting FROM Withdrawal WITH (NOLOCK) " +
                 " LEFT JOIN ProviderCode WITH (NOLOCK) ON ProviderCode.ProviderCode=Withdrawal.ProviderCode" +
                 " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyID=Withdrawal.forCompanyID" +
                 " WHERE WithdrawSerial=@TransactionSerial ";
            //SS = " SELECT CollectCharge,Amount AS OrderAmount FROM Withdrawal WITH (NOLOCK) WHERE WithdrawSerial=@TransactionSerial ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@TransactionSerial", System.Data.SqlDbType.VarChar).Value = TransactionSerial;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList().First();
            }
        }

        return returnValue;
    }

    public List<DBModel.CompanyManualHistory> GetCompanyManualHistory(FromBody.GetCompanyManualHistory fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.CompanyManualHistory> returnValue = new List<DBModel.CompanyManualHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT CMH.*, " +
                  "        AT.LoginAccount, " +
                  "        CONVERT(VARCHAR, CMH.CreateDate, 120) AS CreateDate2, " +
                  "        ST.ServiceTypeName, " +
                  "        CT.CompanyName " +
                  " FROM   CompanyManualHistory CMH " +
                  "        LEFT JOIN AdminTable AT " +
                  "               ON CMH.forAdminID = AT.AdminID " +
                  "        LEFT JOIN ServiceType ST " +
                  "               ON ST.ServiceType = CMH.ServiceType " +
                  "                  AND ST.CurrencyType = CMH.CurrencyType " +
                  "        LEFT JOIN CompanyTable CT " +
                  "               ON CT.CompanyID = CMH.forCompanyID " +
                  " WHERE  CMH.CreateDate >= @StartDate And CMH.CreateDate <= @EndDate ";


        if (fromBody.forCompanyID != -99)
        {
            SS += " AND CMH.forCompanyID = @forCompanyID ";
        }


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", System.Data.SqlDbType.Int).Value = fromBody.forCompanyID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyManualHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public int InsertProviderManualHistory(DBModel.ProviderManualHistory Model, int AdminID)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -4;

        CreateProviderPoint(Model.CurrencyType,Model.ProviderCode);

        SS = "spAddProviderManual";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Model.Type;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Model.Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@TransactionSerial", SqlDbType.VarChar).Value = Model.TransactionSerial;
        DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public int UpdatePrePaidPoint(string ProviderCode,string CurrencyType,decimal Amount)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -99;

        SS = "spAddProviderPrePaidPoint";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = "调整预付额度";
        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = 0;
        DBCmd.Parameters.Add("@OperatorType", SqlDbType.VarChar).Value = 3;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public void CreateProviderPoint(string CurrencyType,string ProviderCode) {
        string SS;
        SqlCommand DBCmd = null;


        SS = @"INSERT INTO ProviderPoint (ProviderCode, CurrencyType)
            SELECT @ProviderCode, @CurrencyType
            WHERE NOT EXISTS (
                SELECT 1 FROM ProviderPoint WITH (NOLOCK)
                WHERE ProviderCode = @ProviderCode AND CurrencyType = @CurrencyType
            ) ";
        DBCmd = new SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;

        DBAccess.ExecuteDB(DBConnStr, DBCmd);
    }

    public int InsertProviderManualHistoryByProfitAmount(DBModel.ProviderManualHistory Model, int AdminID)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -4;

        SS = "spAddProviderManual";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Model.Type;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Model.Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@TransactionSerial", SqlDbType.VarChar).Value = Model.TransactionSerial;
        DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public int InsertCompanyManualHistory(DBModel.CompanyManualHistory Model, int AdminID)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -4;

        SS = "spAddCompanyManual";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Model.Type;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Model.Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@TransactionSerial", SqlDbType.VarChar).Value = Model.TransactionSerial;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }

    public int InsertManualHistory(DBModel.CompanyManualHistory Model, int AdminID)
    {
        String SS = String.Empty;
        SqlCommand DBCmd;
        int returnValue = -4;

        SS = "spAddManual";
        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.StoredProcedure;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = Model.Type;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Model.Amount;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@TransactionSerial", SqlDbType.VarChar).Value = Model.TransactionSerial;
        DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = Model.ProviderCode;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@forAdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

        returnValue = (int)DBCmd.Parameters["@Return"].Value;

        return returnValue;
    }
    #endregion

    #region 案件冻结
    public int InsertFrozenPoint(DBModel.FrozenPoint Model,bool BoolActualProviderFrozenAmount , int AdminID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        decimal ActualProviderFrozenAmount = Model.ProviderFrozenAmount;
        if (ActualProviderFrozenAmount>0) {
            if (BoolActualProviderFrozenAmount)
            {
                var ProviderModel = GetProviderCodeResult(Model.forProviderCode);
                if (ProviderModel != null)
                {
                    if (ProviderModel.First().CollectType == 1)
                    {
                        var ProxyGroupModel = GetProxyProviderGroupByGroupID(Model.forProviderCode, Model.GroupID);
                        var CompanyServiceModel = GetSelectedCompanyService(Model.forCompanyID, Model.ServiceType, Model.CurrencyType);
                        if (ProxyGroupModel != null) {
                            ActualProviderFrozenAmount = (decimal)((double)Model.ProviderFrozenAmount - ((double)Model.ProviderFrozenAmount * 0.01 *((double)CompanyServiceModel.CollectRate-(double)ProxyGroupModel.PaymentRate)));
                        }
                    }
                }
            }
        }
     

        SS = " INSERT INTO FrozenPoint (forPaymentSerial, forCompanyID, forProviderCode, CompanyFrozenAmount, ProviderFrozenAmount, Description,CurrencyType,FrozenAdminID,forPaymentID,ServiceType,GroupID,BankCard,BankCardName,BankName,ActualProviderFrozenAmount) " +
             " VALUES (@forPaymentSerial, @forCompanyID, @forProviderCode, @CompanyFrozenAmount, @ProviderFrozenAmount, @Description,@CurrencyType,@FrozenAdminID,@forPaymentID,@ServiceType,@GroupID,@BankCard,@BankCardName,@BankName,@ActualProviderFrozenAmount)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forPaymentID", SqlDbType.Int).Value = Model.forPaymentID;
        DBCmd.Parameters.Add("@forPaymentSerial", SqlDbType.VarChar).Value = Model.forPaymentSerial;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@forProviderCode", SqlDbType.VarChar).Value = Model.forProviderCode;
        DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = Model.CurrencyType;
        DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = Model.ServiceType;
        DBCmd.Parameters.Add("@CompanyFrozenAmount", SqlDbType.Decimal).Value = Model.CompanyFrozenAmount;
        DBCmd.Parameters.Add("@ProviderFrozenAmount", SqlDbType.Decimal).Value = ActualProviderFrozenAmount;
        DBCmd.Parameters.Add("@FrozenAdminID", SqlDbType.Int).Value = AdminID;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Model.Description;
        DBCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = Model.GroupID;
        DBCmd.Parameters.Add("@ActualProviderFrozenAmount", SqlDbType.Decimal).Value = Model.ProviderFrozenAmount;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = Model.BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = Model.BankCardName;
        DBCmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = Model.BankName;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        //RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);

        return returnValue;
    }

    public int ThawPoint(int FrozenID, int AdminID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE FrozenPoint SET Status = 1,UnFrozenAdminID=@UnFrozenAdminID,UnFrozenDate=getdate() " +
             " WHERE FrozenID = @FrozenID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@FrozenID", SqlDbType.Int).Value = FrozenID;
        DBCmd.Parameters.Add("@UnFrozenAdminID", SqlDbType.Int).Value = AdminID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        //RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);

        return returnValue;
    }

    public List<DBModel.FrozenPointHistory> GetFrozenPointHistoryResult(FromBody.GetFrozenPointHistory fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.FrozenPointHistory> returnValue = new List<DBModel.FrozenPointHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT GroupName,FP.*,CT.CompanyName,PC.ProviderName,convert(varchar,FP.CreateDate,120) as CreateDate2, " +
             " (SELECT RealName From AdminTable WHERE AdminTable.AdminID=FrozenAdminID) as FrozenAdminName," +
             " (SELECT RealName From AdminTable WHERE AdminTable.AdminID=UnFrozenAdminID) as UnFrozenAdminName," +
             " STUFF((" +
             " Select  ',' + CONVERT(varchar(100), IT.ImageID)+'_'+ImageName+'_'+convert(varchar, CreateDate, 120) " +
             " From ImageTable IT " +
             " where IT.TransactionID =CONVERT(varchar(100), FP.FrozenID) And IT.Type=1 " +
             " For Xml Path(''))" +
             " , 1, 1, '') as ImageName" +
             " FROM   FrozenPoint FP WITH(NOLOCK) " +
             " LEFT JOIN  CompanyTable CT WITH(NOLOCK) ON FP.forCompanyID = CT.CompanyID  " +
             " LEFT JOIN  ProviderCode PC WITH(NOLOCK) ON FP.forProviderCode = PC.ProviderCode " +
             " LEFT JOIN  ProxyProviderGroup PPG WITH(NOLOCK) ON PPG.GroupID = FP.GroupID ";
        if (fromBody.Status == 1)
        {
            SS += " WHERE  FP.UnFrozenDate >= @StartDate And FP.UnFrozenDate <= @EndDate ";
        }
        else
        {
            SS += " WHERE  FP.CreateDate >= @StartDate And FP.CreateDate <= @EndDate ";
        }

        if (fromBody.PaymentSerial != "")
        {
            SS += "        AND FP.forPaymentSerial = @PaymentSerial ";
        }


        if (fromBody.GroupID != 0)
        {
            SS += "        AND FP.GroupID = @GroupID ";
        }

        if (fromBody.CompanyID != 0)
        {
            SS += "        AND FP.forCompanyID = @CompanyID ";
        }

        if (fromBody.ProviderCode != "-99")
        {
            SS += "        AND FP.forProviderCode = @ProviderCode ";
        }

        if (fromBody.Status != 99)
        {
            SS += "        AND FP.Status = @Status ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = fromBody.PaymentSerial;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", System.Data.SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.FrozenPointHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public List<DBModel.FrozenPointHistory> GetCompanyFrozenPointHistoryResult(FromBody.GetFrozenPointHistory fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.FrozenPointHistory> returnValue = new List<DBModel.FrozenPointHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT FP.forPaymentSerial,FP.CompanyFrozenAmount,FP.Status,FP.BankCard,FP.BankCard,FP.BankName,FP.BankCardName " +
             " FROM   FrozenPoint FP WITH(NOLOCK) WHERE FP.CompanyFrozenAmount>0 ";
     
        if (fromBody.PaymentSerial != "")
        {
            SS += "        AND FP.forPaymentSerial = @PaymentSerial ";
        }


        if (fromBody.CompanyID != 0)
        {
            SS += "        AND FP.forCompanyID = @CompanyID ";
        }


        if (fromBody.Status != 99)
        {
            SS += "        AND FP.Status = @Status ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = fromBody.PaymentSerial;
        DBCmd.Parameters.Add("@Status", System.Data.SqlDbType.Int).Value = fromBody.Status;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.FrozenPointHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public DBModel.FrozenPointHistory GetSumFrozenPoint(string ProviderCode)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.FrozenPointHistory returnValue = new DBModel.FrozenPointHistory();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT SUM(CompanyFrozenAmount) AS CompanyFrozenAmount,SUM(ProviderFrozenAmount) AS ProviderFrozenAmount " +
             " FROM FrozenPoint WITH(NOLOCK) WHERE Status = 0 And forProviderCode=@ProviderCode ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.FrozenPointHistory>(DT).ToList().FirstOrDefault();
            }
        }

        return returnValue;
    }

    public List<DBModel.FrozenPointHistory> GetFrozenPointHistoryByProxyProvider(FromBody.GetFrozenPointHistory fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.FrozenPointHistory> returnValue = new List<DBModel.FrozenPointHistory>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT GroupName,FP.*,convert(varchar,FP.CreateDate,120) as CreateDate2 " +
             " FROM   FrozenPoint FP WITH(NOLOCK) " +

             " LEFT JOIN  ProxyProviderGroup PPG ON PPG.GroupID = FP.GroupID " +
             " WHERE  FP.CreateDate >= @StartDate And FP.CreateDate <= @EndDate AND FP.forProviderCode = @ProviderCode  ";

        if (fromBody.PaymentSerial != "")
        {
            SS += "        AND FP.forPaymentSerial = @PaymentSerial ";
        }

        if (fromBody.Status != 99)
        {
            SS += "        AND FP.Status = @Status ";
        }

        if (fromBody.GroupID != 0)
        {
            SS += "        AND PPG.GroupID = @GroupID ";
        }


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = fromBody.PaymentSerial;
        DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = fromBody.ProviderCode;
        DBCmd.Parameters.Add("@Status", System.Data.SqlDbType.Int).Value = fromBody.Status;
        DBCmd.Parameters.Add("@GroupID", System.Data.SqlDbType.Int).Value = fromBody.GroupID;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.FrozenPointHistory>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }
    #endregion

    #region 供應商專卡
    public List<DBModel.CompanyWhiteList> GetCompanyWhiteListProviderBankCard(FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.CompanyWhiteList> returnValue = new List<DBModel.CompanyWhiteList>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = @" SELECT CWL.*,CT.CompanyName FROM CompanyWhiteList CWL 
                JOIN CompanyWhiteListProviderBankCard CWLP ON CWL.WhiteListID = CWLP.WhiteListID 
                JOIN CompanyTable CT ON CT.CompanyID = CWL.forCompanyID 
                WHERE CWLP.BankCardGUID=@BankCardGUID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@BankCardGUID", System.Data.SqlDbType.VarChar).Value = fromBody.BankCardGUID;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.CompanyWhiteList>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public int InsertCompanyWhiteListProviderBankCard(FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        string WhiteListID;
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
   

        SS = " INSERT INTO CompanyWhiteList (forCompanyID, UserIdentity, Description) " +
             " OUTPUT INSERTED.WhiteListID "+
             " VALUES (@forCompanyID, @UserIdentity, @Description)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;
        DBCmd.Parameters.Add("@UserIdentity", SqlDbType.NVarChar).Value = fromBody.UserIdentity;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
        WhiteListID = DBAccess.GetDBValue(Pay.DBConnStr, DBCmd).ToString();
        if (!string.IsNullOrEmpty(WhiteListID))
        {
            SS = " INSERT INTO CompanyWhiteListProviderBankCard (WhiteListID, BankCardGUID) " +
                  " VALUES (@WhiteListID, @BankCardGUID)";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = fromBody.BankCardGUID;
            DBCmd.Parameters.Add("@WhiteListID", SqlDbType.VarChar).Value = WhiteListID;
            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }
       
        return returnValue;
    }


    public int UpdateCompanyWhiteListProviderBankCard(FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE CompanyWhiteList SET forCompanyID = @forCompanyID,UserIdentity=@UserIdentity,Description=@Description " +
             " WHERE WhiteListID = @WhiteListID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WhiteListID", SqlDbType.VarChar).Value = fromBody.WhiteListID;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = fromBody.forCompanyID;
        DBCmd.Parameters.Add("@UserIdentity", SqlDbType.NVarChar).Value = fromBody.UserIdentity;
        DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = fromBody.Description;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        //RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);

        return returnValue;
    }

    public int DeleteCompanyWhiteListProviderBankCard(FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        SS = " DELETE FROM  CompanyWhiteList" +
                   " WHERE WhiteListID = @WhiteListID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WhiteListID", SqlDbType.VarChar).Value = fromBody.WhiteListID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        SS = " DELETE FROM  CompanyWhiteListProviderBankCard" +
            " WHERE WhiteListID = @WhiteListID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@WhiteListID", SqlDbType.VarChar).Value = fromBody.WhiteListID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }
    #endregion

    #region DownOrderTransferLog
    public List<DBModel.DownOrderTransferLog> GetDownOrderTransferLogResult(FromBody.DownOrderTransferLogSet fromBody)
    {
        List<DBModel.DownOrderTransferLog> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;

        SS = " SELECT *,CompanyName FROM DownOrderTransferLog WITH (NOLOCK) " +
             " LEFT JOIN CompanyTable  WITH (NOLOCK)  ON CompanyTable.CompanyCode=DownOrderTransferLog.CompanyCode" +
             " WHERE DownOrderTransferLog.CompanyCode <> '5408' AND DownOrderTransferLog.CreateDate >= @StartDate And DownOrderTransferLog.CreateDate <= @EndDate";


        if (fromBody.isErrorOrder != 99)
        {
            SS += " AND isErrorOrder=@isErrorOrder ";
        }

        if (fromBody.ProcessStatus != 99)
        {
            SS += " And DownOrderTransferLog.Type=@Type ";
        }
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            SS += " And DownOrderTransferLog.DownOrderID=@OrderID ";
        }
        if (!string.IsNullOrEmpty(fromBody.CompanyCode))
        {
            SS += " And DownOrderTransferLog.CompanyCode=@CompanyCode ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = fromBody.StartDate.ToString("yyyy/MM/dd") + " 00:00:00.000";
        DBCmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = fromBody.EndDate.ToString("yyyy/MM/dd") + " 23:59:59.999";
        DBCmd.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = fromBody.OrderID;
        DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = fromBody.CompanyCode;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = fromBody.ProcessStatus;
        DBCmd.Parameters.Add("@isErrorOrder", SqlDbType.Int).Value = fromBody.isErrorOrder;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.DownOrderTransferLog>(DT).ToList();
            }
        }

        return returnValue;
    }

    public List<DBModel.DownOrderTransferLogV2> GetDownOrderTransferLogResultV2(FromBody.DownOrderTransferLogSetV2 fromBody)
    {
        List<DBModel.DownOrderTransferLogV2> returnValue = null;
        String SS = String.Empty;
        SqlCommand DBCmd;
        DataTable DT;
        SS = " WITH T";
        SS += " AS(";
        SS += " SELECT DownOrderTransferLog.*,CompanyName FROM DownOrderTransferLog WITH (NOLOCK) ";
        SS += " LEFT JOIN CompanyTable WITH (NOLOCK) ON CompanyTable.CompanyCode=DownOrderTransferLog.CompanyCode";
        SS += " WHERE  DownOrderTransferLog.CreateDate >= @StartDate And DownOrderTransferLog.CreateDate <= @EndDate";


        if (fromBody.isErrorOrder != 99)
        {
            SS += " AND isErrorOrder=@isErrorOrder ";
        }

        if (fromBody.ProcessStatus != 99)
        {
            SS += " And DownOrderTransferLog.Type=@Type ";
        }
        if (!string.IsNullOrEmpty(fromBody.OrderID))
        {
            SS += " And DownOrderTransferLog.DownOrderID=@OrderID ";
        }
        if (!string.IsNullOrEmpty(fromBody.CompanyCode))
        {
            SS += " And DownOrderTransferLog.CompanyCode=@CompanyCode ";
        }

        if (!string.IsNullOrEmpty(fromBody.search.value))
        {
            SS += " And (DownOrderTransferLog.DownOrderID like '%'+@SearchFilter+'%' OR  DownOrderTransferLog.Message like '%'+@SearchFilter+'%'  OR  CompanyTable.CompanyName like '%'+@SearchFilter+'%' ) ";
        }

        SS += " )";
        SS += " SELECT TotalCount = COUNT(1) OVER( ";
        SS += " ),T.* ";
        SS += " FROM T";
        #region 排序

        switch (fromBody.columns[fromBody.order.First().column].data.ToString())
        {
            case "Message":
                SS += " Order By T.Message ";
                break;
            case "CreateDate":
                SS += " Order By T.CreateDate ";
                break;
            case "Type":
                SS += " Order By T.Type ";
                break;
            case "DownOrderID":
                SS += " Order By T.DownOrderID ";
                break;
            case "CompanyName":
                SS += " Order By T.CompanyName ";
                break;
            default:
                SS += " Order By T.CreateDate ";
                break;
        }

        if (fromBody.order.First().dir == "asc")
        {
            SS += " ASC ";
        }
        else
        {
            SS += " DESC ";
        }
        #endregion

        SS += " OFFSET @pageNo ROWS ";
        SS += " FETCH NEXT @pageSize ROWS ONLY ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = fromBody.StartDate;
        DBCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = fromBody.EndDate;
        DBCmd.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = string.IsNullOrEmpty(fromBody.OrderID) ? "" : fromBody.OrderID;
        DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = string.IsNullOrEmpty(fromBody.CompanyCode) ? "" : fromBody.CompanyCode;
        DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = fromBody.ProcessStatus;
        DBCmd.Parameters.Add("@isErrorOrder", SqlDbType.Int).Value = fromBody.isErrorOrder;
        DBCmd.Parameters.Add("@SearchFilter", System.Data.SqlDbType.NVarChar).Value = string.IsNullOrEmpty(fromBody.search.value) ? "" : fromBody.search.value.Trim();
        DBCmd.Parameters.Add("@pageNo", System.Data.SqlDbType.Int).Value = fromBody.start;
        DBCmd.Parameters.Add("@pageSize", System.Data.SqlDbType.Int).Value = fromBody.length;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.DownOrderTransferLogV2>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region 黑名單

    public int InsertBlackList(DBModel.BlackList Model, int AdminID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " INSERT INTO BlackList (forCompanyID, UserIP, BankCard, BankCardName,BlackAdminID) " +
             " VALUES (@forCompanyID, @UserIP, @BankCard, @BankCardName,@BlackAdminID)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", SqlDbType.Int).Value = Model.forCompanyID;
        DBCmd.Parameters.Add("@UserIP", SqlDbType.VarChar).Value = Model.UserIP;
        DBCmd.Parameters.Add("@BankCard", SqlDbType.VarChar).Value = Model.BankCard;
        DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = Model.BankCardName;
        DBCmd.Parameters.Add("@BlackAdminID", SqlDbType.Int).Value = AdminID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        //RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);

        return returnValue;
    }

    public List<DBModel.BlackList> GetBlackListResult(FromBody.GetBlackList fromBody)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.BlackList> returnValue = new List<DBModel.BlackList>();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT *,convert(varchar,CreateDate,120) as CreateDate2 " +
             " FROM   BlackList  " +
             " WHERE  1=1 ";

        if (fromBody.CompanyID != -99)
        {
            SS += "        AND forCompanyID = @forCompanyID ";
        }


        if (fromBody.UserIP != "")
        {
            SS += "        AND UserIP = @UserIP ";
        }

        if (fromBody.BankCard != "")
        {
            SS += "        AND BankCard = @BankCard ";
        }

        if (fromBody.BankCardName != "")
        {
            SS += "        AND BankCardName = @BankCardName ";
        }

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyID", System.Data.SqlDbType.Int).Value = fromBody.CompanyID;
        DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = fromBody.UserIP;
        DBCmd.Parameters.Add("@BankCard", System.Data.SqlDbType.VarChar).Value = fromBody.BankCard;
        DBCmd.Parameters.Add("@BankCardName", System.Data.SqlDbType.NVarChar).Value = fromBody.BankCardName;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.BlackList>(DT).ToList();
            }
        }

        if (returnValue.Count == 0)
        {
            returnValue = null;
        }

        return returnValue;
    }

    public int CancelBlackList(int FrozenID, int AdminID)
    {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE BlackList SET Status = 1,UnBlackAdminID=@UnBlackAdminID " +
             " WHERE BlackListID = @BlackListID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@BlackListID", SqlDbType.Int).Value = FrozenID;
        DBCmd.Parameters.Add("@UnBlackAdminID", SqlDbType.Int).Value = AdminID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        //RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyID);

        return returnValue;
    }

    public DBModel.BlackList GetBlackListResult(int BlackListID)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        DBModel.BlackList returnValue = new DBModel.BlackList();
        string SummaryContent = string.Empty;
        DataTable DT;
        string SummaryDateString = string.Empty;

        SS = " SELECT * " +
             " FROM   BlackList  " +
             " WHERE  BlackListID=@BlackListID ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@BlackListID", System.Data.SqlDbType.Int).Value = BlackListID;


        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.BlackList>(DT).ToList().First();
            }
        }

        return returnValue;
    }
    #endregion

    #region 風控
    public int RiskControlWithdrawalCount()
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        int returnValue = 0;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT Count(*) "
             + " FROM RiskControlWithdrawal WITH(NOLOCK)  "
             + " WHERE CreateDate between DATEADD(minute,-1,GETDATE()) And GETDATE()";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        returnValue = int.Parse(DBAccess.GetDBValue(DBConnStr, DBCmd).ToString());

        return returnValue;
    }

    public List<DBModel.RiskControlByPaymentSuccessCount> RiskControlByPaymentSuccessCount()
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.RiskControlByPaymentSuccessCount> returnValue = null;
        DataTable DT;

        SS = "SELECT tmpTable.* FROM ( SELECT ClientIP,count(*) as Count,convert(varchar, MAX(FinishDate), 120) as EndDate,convert(varchar, min(FinishDate), 120) as StartDate FROM PaymentTable WITH(NOLOCK) " +
             " Where FinishDate between DATEADD(minute,-10,GETDATE()) And GETDATE()" +
             " And(ProcessStatus = 4 or ProcessStatus = 2) And SubmitType=0 " +
             " Group by ClientIP)  tmpTable WHERE Count>= 3";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.RiskControlByPaymentSuccessCount>(DT).ToList();
                InsertRiskControlPayment(returnValue);
            }
        }

        return returnValue;
    }

    public void InsertRiskControlPayment(List<DBModel.RiskControlByPaymentSuccessCount> data)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;

        for (int i = 0; i < data.Count; i++)
        {
            SS = " INSERT INTO RiskControlPaymentTable(forPaymentID)" +
          " SELECT PaymentID FROM PaymentTable PT" +
          " WHERE PT.PaymentID NOT IN(SELECT forPaymentID" +
          " FROM RiskControlPaymentTable)" +
          " AND PT.FinishDate between @StartDate And @EndDate" +
          " AND PT.ClientIP = @UserIP";

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = DateTime.Parse(data[i].EndDate).AddSeconds(1);
            DBCmd.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = DateTime.Parse(data[i].StartDate);
            DBCmd.Parameters.Add("@UserIP", System.Data.SqlDbType.VarChar).Value = data[i].UserIP;
            DBAccess.ExecuteDB(DBConnStr, DBCmd);
        }

    }

    public List<DBModel.PaymentReport> GetRiskControlPayment()
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.PaymentReport> returnValue = null;
        string SummaryContent = string.Empty;
        DataTable DT;
        List<string> LstPaymentSerial = new List<string>();
        List<string> LstOrderID = new List<string>();
        string SummaryDateString = string.Empty;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = "SELECT P.* ,  " +
             "       S.ServiceTypeName, " +
             "       PC.ProviderName, " +
             "       B.BankName, " +
             "       B.BankType, " +
             "       C.CompanyName, " +
             "       C.CompanyCode, " +
             "       C.MerchantCode, " +
             "       convert(varchar, P.CreateDate, 120) as CreateDate2, " +
             "       convert(varchar, P.OrderDate, 120) as OrderDate2, " +
             "       convert(varchar, P.FinishDate, 120) as FinishDate2 " +
             "FROM RiskControlPaymentTable AS RCP " +
             "LEFT JOIN PaymentTable AS P ON RCP.forPaymentID = P.PaymentID " +
             "LEFT JOIN ServiceType AS S ON P.ServiceType = S.ServiceType " +
             "LEFT JOIN ProviderCode AS PC ON PC.ProviderCode = P.ProviderCode " +
             "LEFT JOIN CompanyTable AS C  ON C.CompanyID = P.forCompanyID " +
             "LEFT JOIN BankCode AS B ON B.BankCode = P.BankCode " +
             "WHERE P.SubmitType=0 ";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.PaymentReport>(DT).ToList();
            }
        }

        return returnValue;
    }

    public void InsertRiskControlWithdrawal(string CompanyCode, string BankCard, string BankCardName, string BankName)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;

        SS = "  INSERT INTO RiskControlWithdrawal (forCompanyCode,BankCard,BankCardName,BankName) VALUES(@CompanyCode,@BankCard,@BankCardName,@BankName)";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
        DBCmd.Parameters.Add("@BankCard", System.Data.SqlDbType.VarChar).Value = BankCard;
        DBCmd.Parameters.Add("@BankCardName", System.Data.SqlDbType.NVarChar).Value = BankCardName;
        DBCmd.Parameters.Add("@BankName", System.Data.SqlDbType.NVarChar).Value = BankName;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);
    }

    public List<DBModel.RiskControlWithdrawalTable> GetRiskControlWithdrawal()
    {

        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;
        List<DBModel.RiskControlWithdrawalTable> returnValue = null;
        DataTable DT;
        //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
        DBCmd = new System.Data.SqlClient.SqlCommand();
        SS = " SELECT CompanyName,BankCard,BankCardName,BankName,RiskControlWithdrawal.CreateDate " +
             " FROM RiskControlWithdrawal WITH(NOLOCK) " +
             " LEFT JOIN CompanyTable WITH(NOLOCK) " +
             " ON RiskControlWithdrawal.forCompanyCode = CompanyTable.CompanyCode ";
        //+ " WHERE CreateDate between DATEADD(minute,-10,GETDATE()) And GETDATE()";

        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;

        DT = DBAccess.GetDB(DBConnStr, DBCmd);

        if (DT != null)
        {
            if (DT.Rows.Count > 0)
            {
                returnValue = DataTableExtensions.ToList<DBModel.RiskControlWithdrawalTable>(DT).ToList();
            }
        }

        return returnValue;
    }
    #endregion

    #region 系統警示通知
    public void InsertBotSendLog(string CompanyCode, string MsgContent)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd;

        SS = " INSERT INTO BotSendLog(forCompanyCode,MsgContent)" +
      " VALUES (@forCompanyCode,@MsgContent) ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@forCompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
        DBCmd.Parameters.Add("@MsgContent", System.Data.SqlDbType.NVarChar).Value = MsgContent;
        DBAccess.ExecuteDB(DBConnStr, DBCmd);

    }

    #endregion

    #region 支付寶
    public string UpdateAlipayImage(int CompanyID, byte[] ImageData, string _ImageName, string ModifyType, int ImageID, int Type) {
        int dbReturnImageID = -1;
        string returnStr = "";
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        string CompanyCode = GetCompanyCodeByCompanyID(CompanyID);
        string FileExtension = _ImageName;
        string ImageName = string.Empty;

        if (ModifyType == "create") {
            SS = " INSERT INTO ImageTable (CompanyCode,Type) " +
                 "   VALUES (@CompanyCode,3) " +
                 " SELECT @@IDENTITY ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            var DBReturn = DBAccess.GetDBValue(Pay.DBConnStr, DBCmd);
            if (DBReturn != null) {
                dbReturnImageID = int.Parse(DBReturn.ToString());
                ImageName = dbReturnImageID + "." + FileExtension;

                SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                     "   WHERE ImageID=@ImageID And CompanyCode=@CompanyCode AND Type=3 ";

                DBCmd = new System.Data.SqlClient.SqlCommand();
                DBCmd.CommandText = SS;
                DBCmd.CommandType = CommandType.Text;
                DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
                DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
                DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = dbReturnImageID;
                DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
            }
        } else if (ModifyType == "update") {
            ImageName = ImageID + "." + FileExtension;

            SS = " UPDATE ImageTable SET ImageName=@ImageName " +
                 " WHERE CompanyCode=@CompanyCode AND ImageID=@ImageID ";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
            DBCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar).Value = ImageName;
            DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
            var DBReturn = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

            if (DBReturn > 0) {
                dbReturnImageID = ImageID;
            }
        }

        if (dbReturnImageID > 0) {
            returnStr = ImageName;
            Pay.SaveFileWithForderName(ImageName, ImageData, "ProviderAlipay");
        }

        return returnStr;
    }

    public int UpdateImageTableTransactionIDByImageID(int ImageID, string TransactionID) {
        int returnValue;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ImageTable SET TransactionID=@TransactionID " +
             " WHERE ImageID = @ImageID";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;
        DBCmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = ImageID;
        DBCmd.Parameters.Add("@TransactionID", SqlDbType.VarChar).Value = TransactionID;
        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }

    public string InsertProviderAlipay(FromBody.ProviderBankCardData companyAlipay) {
        string returnValue = string.Empty;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " INSERT INTO ProviderBankCard (BankCardGUID, forProviderCode, BankCardState, CurrencyType, Type, CardType, BankCardName, Description, AmountMax, TotalCreditValueDay, TotalCreditValueWeek, TotalCreditValueMonth, AmountMin, ServiceStartDate, ServiceEndDate) " +
                                  " VALUES (@BankCardGUID ,@ProviderCode, @BankCardState, @CurrencyType, @Type, @CardType, @BankCardName, @Description, @AmountMax, @TotalCreditValueDay, @TotalCreditValueWeek, @TotalCreditValueMonth, @AmountMin, @ServiceStartDate, @ServiceEndDate) ";

        if (companyAlipay.AlipayArray.Length > 0) {
            int index = 0;

            DBAccess.ExecuteTransDB(DBConnStr, T => {

                foreach (var item in companyAlipay.AlipayArray) {
                    returnValue = System.Guid.NewGuid().ToString();

                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.Text;
                    DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = returnValue;
                    DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = companyAlipay.forProviderCode;
                    DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyAlipay.BankCardState;
                    DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = companyAlipay.CurrencyType;
                    DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = companyAlipay.Type;
                    DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyAlipay.CardType;
                    DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyAlipay.BankCardName + "_" + index;
                    DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = item;
                    DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyAlipay.AmountMax;
                    DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueDay;
                    DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueWeek;
                    DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueMonth;
                    DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyAlipay.AmountMin;
                    DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyAlipay.ServiceStartDate;
                    DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyAlipay.ServiceEndDate;

                    T.ExecuteDB(DBCmd);
                    index++;
                }
            });
        } else {
            returnValue = System.Guid.NewGuid().ToString();

            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = returnValue;
            DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = companyAlipay.forProviderCode;
            DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyAlipay.BankCardState;
            DBCmd.Parameters.Add("@CurrencyType", SqlDbType.VarChar).Value = companyAlipay.CurrencyType;
            DBCmd.Parameters.Add("@Type", SqlDbType.Int).Value = companyAlipay.Type;
            DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyAlipay.CardType;
            DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyAlipay.BankCardName;
            DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = companyAlipay.Description;
            DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyAlipay.AmountMax;
            DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueDay;
            DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueWeek;
            DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueMonth;
            DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyAlipay.AmountMin;
            DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyAlipay.ServiceStartDate;
            DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyAlipay.ServiceEndDate;

            DBAccess.ExecuteDB(DBConnStr, DBCmd);
        }

        return returnValue;
    }

    public int UpdateProviderAlipay(FromBody.ProviderBankCardData companyAlipay) {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderBankCard SET BankCardState=@BankCardState " +
             " , CardType=@CardType " +
             " , BankCardName=@BankCardName " +
             " , Description=@Description " +
             " , AmountMax=@AmountMax " +
             " , TotalCreditValueDay=@TotalCreditValueDay " +
             " , TotalCreditValueWeek=@TotalCreditValueWeek " +
             " , TotalCreditValueMonth=@TotalCreditValueMonth " +
             " , AmountMin=@AmountMin " +
             " , ServiceStartDate=@ServiceStartDate " +
             " , ServiceEndDate=@ServiceEndDate " +
             " WHERE BankCardGUID=@BankCardGUID ";

        if (companyAlipay.AlipayArray.Length > 0) {
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;

            DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = companyAlipay.BankCardGUID;
            DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyAlipay.BankCardState;
            DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyAlipay.CardType;
            DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyAlipay.BankCardName;
            DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = companyAlipay.AlipayArray[0];
            DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyAlipay.AmountMax;
            DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueDay;
            DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueWeek;
            DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueMonth;
            DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyAlipay.AmountMin;
            DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyAlipay.ServiceStartDate;
            DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyAlipay.ServiceEndDate;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        } else {
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;

            DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = companyAlipay.BankCardGUID;
            DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = companyAlipay.BankCardState;
            DBCmd.Parameters.Add("@CardType", SqlDbType.Int).Value = companyAlipay.CardType;
            DBCmd.Parameters.Add("@BankCardName", SqlDbType.NVarChar).Value = companyAlipay.BankCardName;
            DBCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = companyAlipay.Description;
            DBCmd.Parameters.Add("@AmountMax", SqlDbType.Decimal).Value = companyAlipay.AmountMax;
            DBCmd.Parameters.Add("@TotalCreditValueDay", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueDay;
            DBCmd.Parameters.Add("@TotalCreditValueWeek", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueWeek;
            DBCmd.Parameters.Add("@TotalCreditValueMonth", SqlDbType.Decimal).Value = companyAlipay.TotalCreditValueMonth;
            DBCmd.Parameters.Add("@AmountMin", SqlDbType.Decimal).Value = companyAlipay.AmountMin;
            DBCmd.Parameters.Add("@ServiceStartDate", SqlDbType.DateTime).Value = companyAlipay.ServiceStartDate;
            DBCmd.Parameters.Add("@ServiceEndDate", SqlDbType.DateTime).Value = companyAlipay.ServiceEndDate;

            returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);
        }
        return returnValue;
    }

    public int DeleteProviderAlipay(FromBody.ProviderBankCardData companyAlipay) {
        int returnValue = -1;
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        SS = " UPDATE ProviderBankCard SET BankCardState=@BankCardState " +
             " WHERE BankCardGUID=@BankCardGUID ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = CommandType.Text;

        DBCmd.Parameters.Add("@BankCardGUID", SqlDbType.VarChar).Value = companyAlipay.BankCardGUID;
        DBCmd.Parameters.Add("@BankCardState", SqlDbType.Int).Value = 2;

        returnValue = DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        return returnValue;
    }
    #endregion

    public string GetUserIP2()
    {
        return CodingControl.GetUserIP();
    }

    /// <summary>
    /// 取得商户钱包额度
    /// </summary>
    /// <param name="CompanyID"></param>
    /// <param name="CurrencyType"></param>
    /// <returns></returns>
    public DataTable GetCanUseCompanyPoint(int CompanyID, string CurrencyType)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;
        System.Data.DataTable DT;

        SS =@"SELECT(CP.PointValue - (SELECT ISNULL(SUM(W.Amount + W.CollectCharge), 0)
             FROM Withdrawal W     WHERE W.Status<> 2 AND W.Status<> 3 AND W.Status<> 8
             AND W.Status<> 90 AND W.Status<> 91 AND W.forCompanyID = CP.forCompanyID AND W.CurrencyType = CP.CurrencyType)
             - (SELECT ISNULL(SUM(SC.SummaryNetAmount), 0)       FROM CompanyService CS
             INNER JOIN SummaryCompanyByDate SC ON CS.forCompanyID = SC.forCompanyID
             AND CS.CurrencyType = SC.CurrencyType AND SC.ServiceType = CS.ServiceType
             AND((CS.CheckoutType = 0 AND 1 = 0)
             OR(CS.CheckoutType = 1 AND SC.SummaryDate = dbo.GetReportDate(GETDATE(), CT.Timezone))
             OR(CS.CheckoutType = 2 AND DATEPART(WEEKDAY, GETDATE() - 1) = 7 AND
             (SC.SummaryDate = dbo.GetReportDate(GETDATE(), CT.Timezone) OR SC.SummaryDate = dbo.GetReportDate(DATEADD(day, -1, getdate()), CT.Timezone)))
             OR(CS.CheckoutType = 2 AND DATEPART(WEEKDAY, GETDATE() - 1) = 6 AND  SC.SummaryDate = dbo.GetReportDate(GETDATE(), CT.Timezone)))	
             WHERE CS.forCompanyID = CP.forCompanyID AND CS.CurrencyType = CP.CurrencyType)) AS CanUsePoint,
             ISNULL((Select SUM(CompanyFrozenAmount) FROM FrozenPoint where FrozenPoint.forCompanyID = CP.forCompanyID
             AND FrozenPoint.CurrencyType = CP.CurrencyType AND FrozenPoint.Status = 0),0) AS FrozenPoint,
             CP.*FROM CompanyPoint AS CP
             LEFT JOIN CompanyTable CT ON CT.CompanyID = CP.forCompanyID
             WHERE CP.forCompanyID = @CompanyID  AND CP.CurrencyType = @CurrencyType ";


        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        return DT;
    }

    #region  RunPay
    public APIResult RunPayTest(string PaymentSerial)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        APIResult objReturnValue = new APIResult();
        strSign = string.Format("PaymentSerial={0}&GPayBackendKey={1}"
                                  , PaymentSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);
        objReturnValue.Message = "sign";
        objReturnValue.Status = ResultStatus.ERR;
        #endregion
        var _ReSendPayment = new DBModel.ReSendPaymentSet();

        _ReSendPayment.PaymentSerial = PaymentSerial;
        _ReSendPayment.Sign = sign;

        var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "RunPayTest", JsonConvert.SerializeObject(_ReSendPayment));


        if (!string.IsNullOrEmpty(returnValue))
        {
            objReturnValue = JsonConvert.DeserializeObject<APIResult>(returnValue);
        }

        return objReturnValue;
    }

    public APIResult RunPayGetDepositByDownOrderID(string PaymentSerial)
    {
        string GPayApiUrl = System.Configuration.ConfigurationManager.AppSettings["GPayApiUrl"];
        string GPayBackendKey = System.Configuration.ConfigurationManager.AppSettings["GPayBackendKey"];

        #region SignCheck
        string strSign;
        string sign;
        //APIResult objReturnValue = null;
        APIResult objReturnValue = new APIResult();
        strSign = string.Format("PaymentSerial={0}&GPayBackendKey={1}"
                                  , PaymentSerial
                                  , GPayBackendKey
                                  );

        sign = CodingControl.GetSHA256(strSign);
        objReturnValue.Message = "sign";
        objReturnValue.Status = ResultStatus.ERR;
        #endregion
        var _ReSendPayment = new DBModel.ReSendPaymentSet();

        _ReSendPayment.PaymentSerial = PaymentSerial;
        _ReSendPayment.Sign = sign;

        var returnValue = CodingControl.RequestJsonAPI(GPayApiUrl + "RunPayGetDepositByDownOrderID", JsonConvert.SerializeObject(_ReSendPayment));


        if (!string.IsNullOrEmpty(returnValue))
        {
            objReturnValue = JsonConvert.DeserializeObject<APIResult>(returnValue);
        }

        return objReturnValue;
    }

    public DBViewModel.UpdatePatmentResult ConfirmManualPaymentForRunPay(string PaymentSerial, int modifyStatus, int AdminID, string ProviderCode, string ServiceType, int GroupID)
    {
        DBViewModel.UpdatePatmentResult returnValue = new DBViewModel.UpdatePatmentResult();
        String SS = String.Empty;
        SqlCommand DBCmd;
        int DBreturn = -6;//其他錯誤
        DBModel.PaymentTable PaymentData = GetPaymentResultByPaymentSerial(PaymentSerial);
        DBModel.CompanyService CompanyServiceModel = null;
        DBModel.ProviderService ProviderServiceModel = null;
        List<DBModel.Provider> ProviderModel = null;
        DBModel.ProxyProvider ProxyProviderModel = null;
        returnValue.Status = -1;
        if (PaymentData == null)
        {
            returnValue.Message = "订单资讯错误";
            return returnValue;
        }

        if (PaymentData.ProcessStatus == 1 && PaymentData.SubmitType == 1)
        {
            ProviderModel = GetProviderCodeResult(ProviderCode);

            if (ProviderModel == null)
            {
                returnValue.Message = "尚未设定对应供应商";
                return returnValue;
            }

            CompanyServiceModel = GetCompanyService(PaymentData.forCompanyID, ServiceType, PaymentData.CurrencyType);
            if (CompanyServiceModel == null)
            {
                returnValue.Message = "商户尚未设定该支付通道";
                return returnValue;
            }

            if (GroupID > 0)
                ProviderServiceModel = GetProxyProviderByGroupID(ProviderCode, ServiceType, PaymentData.CurrencyType, GroupID);
            else
                ProviderServiceModel = GetProviderServiceByProviderCodeAndServiceType(ProviderCode, ServiceType, PaymentData.CurrencyType);

            if (ProviderServiceModel == null)
            {
                returnValue.Message = "供应商尚未设定该支付通道";
                return returnValue;
            }
            if (UpdatePaymentCostRate(ProviderServiceModel.CostRate, CompanyServiceModel.CollectRate, PaymentSerial) <= 0)
            {
                returnValue.Message = "更改订单费率失败";
                return returnValue;
            }

            if (ProviderCode == "RunPay")
            {
                var APIReturn = RunPayTest(PaymentSerial);
                if (APIReturn.Status == ResultStatus.OK)
                {
                    UpdatePaymentSerialByProxyProviderOrder(PaymentSerial, 8, ProviderCode, ServiceType);
                    returnValue.Message = "审核完成";
                    returnValue.Status = 0;
                }
                else
                {
                    returnValue.Message = JsonConvert.SerializeObject(APIReturn.Message);
                }
            }
            else
            {
                if (ProviderModel.First().CollectType == 1 && modifyStatus != 3)
                {
                    ProxyProviderModel = GetProxyProviderResult(ProviderCode);
                    if (ProxyProviderModel == null)
                    {
                        returnValue.Message = "尚未设定专属供应商费率";
                        return returnValue;
                    }

                    if (UpdatePaymentSerialByProxyProviderOrder(PaymentSerial, 8, ProviderCode, ServiceType) > 0)
                    {


                        if (GetProxyProviderOrderByOrderSerial(PaymentSerial, 0) == null)
                        {

                            InsertProxyProviderOrder(PaymentSerial, 0, 0, ProxyProviderModel.Rate, GroupID);
                        }
                        //else
                        //{
                        //    UpdateProxyProviderOrder(PaymentSerial, 0, 0, ProxyProviderModel.Rate);
                        //}

                        returnValue.Message = "上游审核中";
                        returnValue.Status = 0;
                    }
                    else
                    {
                        returnValue.Message = "修改订单状态失败";
                    }
                }
                else
                {

                    //人工充值
                    SS = "spSetManualPayment";
                    DBCmd = new System.Data.SqlClient.SqlCommand();
                    DBCmd.CommandText = SS;
                    DBCmd.CommandType = CommandType.StoredProcedure;
                    DBCmd.Parameters.Add("@PaymentSerial", SqlDbType.VarChar).Value = PaymentSerial;
                    DBCmd.Parameters.Add("@ProcessStatus", SqlDbType.Int).Value = modifyStatus;

                    DBCmd.Parameters.Add("@ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
                    DBCmd.Parameters.Add("@ServiceType", SqlDbType.VarChar).Value = ServiceType;
                    DBCmd.Parameters.Add("@Return", SqlDbType.VarChar).Direction = System.Data.ParameterDirection.ReturnValue;
                    DBAccess.ExecuteDB(DBConnStr, DBCmd);
                    DBreturn = (int)DBCmd.Parameters["@Return"].Value;

                    switch (DBreturn)
                    {
                        case 0://成功
                            returnValue.Message = "审核完成";
                            returnValue.Status = 0;
                            break;
                        case -1://交易單不存在
                            returnValue.Message = "交易单不存在";
                            //UpdateWithdrawal(WithdrawSerial, "交易單不存在");
                            break;
                        case -2://交易資料有誤 
                            returnValue.Message = "交易资料有误";
                            //UpdateWithdrawal(WithdrawSerial, "營運商錢包金額錯誤");
                            break;

                        case -4://鎖定失敗
                            returnValue.Message = "锁定失败";
                            //UpdateWithdrawal(WithdrawSerial, "鎖定失敗");
                            break;
                        case -5://加扣點失敗
                            returnValue.Message = "加扣点失败";
                            //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                            break;
                        case -9://加扣點失敗
                            returnValue.Message = "订单审核中";
                            //UpdateWithdrawal(WithdrawSerial, "加扣點失敗");
                            break;
                        default://其他錯誤
                            returnValue.Message = "其他错误";
                            //UpdateWithdrawal(WithdrawSerial, "其他錯誤");
                            break;
                    }

                }
            }


        }
        else
        {
            returnValue.Message = "目前订单状态无法审核";
        }

        returnValue.PaymentData = GetPaymentReportByPaymentSerial(PaymentSerial);
        return returnValue;
    }

    #endregion

    /// <summary>
    /// 取得商户钱包额度，以及占时扣除额资讯
    /// </summary>
    /// <param name="CompanyID"></param>
    /// <param name="CurrencyType"></param>
    /// <returns></returns>.

    public DataTable GetCanUseCompanyPointDetail(int CompanyID, string CurrencyType)
    {
        string SS;
        System.Data.SqlClient.SqlCommand DBCmd = null;

        System.Data.DataTable DT;
        SS= @" SELECT (CP.PointValue  
             	-(SELECT ISNULL(SUM(W.Amount + W.CollectCharge),0)  
                	   FROM Withdrawal W  WITH (NOLOCK)   
             	   WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91 AND W.forCompanyID = CP.forCompanyID AND W.CurrencyType = CP.CurrencyType) 
             	-(SELECT ISNULL(SUM(SC.SummaryNetAmount),0)  
             	   FROM CompanyService CS  
             	   INNER JOIN SummaryCompanyByDate SC ON CS.forCompanyID = SC.forCompanyID AND CS.CurrencyType = SC.CurrencyType AND SC.ServiceType = CS.ServiceType  
             			AND   ((CS.CheckoutType = 0 AND 1 = 0 )  
             			      OR (CS.CheckoutType = 1 AND SC.SummaryDate = dbo.GetReportDate(GETDATE(),CT.Timezone)) 
             			      OR (CS.CheckoutType = 2 AND DATEPART(WEEKDAY, GETDATE()-1) = 7 AND  
             	                  (SC.SummaryDate = dbo.GetReportDate(GETDATE(),CT.Timezone) OR SC.SummaryDate =  dbo.GetReportDate(DATEADD(day, -1, getdate()),CT.Timezone))) 
             				  OR (CS.CheckoutType = 2 AND DATEPART(WEEKDAY, GETDATE()-1) = 6 AND  SC.SummaryDate = dbo.GetReportDate(GETDATE(),CT.Timezone))) 
             	   WHERE CS.forCompanyID = CP.forCompanyID AND CS.CurrencyType = CP.CurrencyType)) AS CanUsePoint,  
              (SELECT ISNULL(SUM(W.Amount + W.CollectCharge), 0)  
              FROM Withdrawal W  WITH (NOLOCK)   
              WHERE W.Status <> 2 AND W.Status <> 3 AND W.Status <> 8  AND W.Status <> 90 AND W.Status <> 91  
              AND W.forCompanyID = CP.forCompanyID AND W.CurrencyType = CP.CurrencyType) AS InWithdrawProcessPoint,  
              	 ISNULL((Select SUM(CompanyFrozenAmount) FROM FrozenPoint where FrozenPoint.forCompanyID=CP.forCompanyID AND FrozenPoint.CurrencyType=CP.CurrencyType AND FrozenPoint.Status=0),0) AS FrozenPoint, 
             	   CP.*  
              FROM CompanyPoint AS CP  
			  LEFT JOIN CompanyTable CT WITH(NOLOCK) ON CT.CompanyID = CP.forCompanyID
              WHERE CP.forCompanyID = @CompanyID  
              AND CP.CurrencyType = @CurrencyType ";

        DBCmd = new System.Data.SqlClient.SqlCommand();
        DBCmd.CommandText = SS;
        DBCmd.CommandType = System.Data.CommandType.Text;
        DBCmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.Int).Value = CompanyID;
        DBCmd.Parameters.Add("@CurrencyType", System.Data.SqlDbType.VarChar).Value = CurrencyType;
        DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

        return DT;
    }

    public class APIResult
    {
        public ResultStatus Status;
        public string Message;
    }

    public enum ResultStatus
    {
        OK = 0,
        ERR = 1,
        SignErr = 2,
        Invalidate = 3,
        Success = 4
    }
}
