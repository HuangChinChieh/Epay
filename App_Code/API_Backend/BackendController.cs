using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using ZXing;

// api 類別名稱命名一定要用 <名稱>Controller
// api 存取方式: http://xxx/api/<名稱>

//後台api一律放在這，不另外切，方便統一管理，但一定要用region分開
//記得加上ActionName，路徑可以查看WebAPIConfig
//Post => 需使用 FormBody並定義Class(MVC API才可以直接對)
//Get => 直接對應

//BackednDB => 呼叫DB                
//BackendFunction => 較複雜的邏輯處理，透過      
//BackendModel => FromBody與DBModel 
//DB的DataTable 一律建成Class，方便日後的Redis處理，轉換可以參考DataTableExtensions

public class BackendController : ApiController
{

    [ActionName("HeartBeat")]
    [HttpGet]
    [HttpPost]
    public string HeartBeat(string echo)
    {

        return echo;
    }


    [ActionName("GeoTest")]
    [HttpGet]
    [HttpPost]
    public string GeoTest(string IP)
    {

        BackendFunction backendFunction = new BackendFunction();
        var GeoCode = backendFunction.GetGeoCode(IP);
        if (GeoCode.GeoCountry == "TW")
        {
            var secret = backendFunction.aesEncryptBase64(IP);
            string result = secret;
            return result;
        }
        else
        {
            return IP;
        }

    }

    [ActionName("SessionTest")]
    [HttpGet]
    [HttpPost]
    public string SessionTest()
    {
        if (HttpContext.Current.Session["aaa"] == null)
        {
            HttpContext.Current.Session["aaa"] = 0;
        }
        else
        {
            HttpContext.Current.Session["aaa"] = (int)HttpContext.Current.Session["aaa"] + 1;
        }
        return HttpContext.Current.Session["aaa"].ToString();
    }

    [ActionName("SearchIPCounty")]
    [HttpGet]
    [HttpPost]
    public SearchIPCountyResult SearchIPCounty([FromBody] FromBody.SearchIPC fromBody)
    {
        SearchIPCountyResult returnValue = new SearchIPCountyResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            returnValue.ResultCode = APIResult.enumResult.SessionError;
            return returnValue;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        var IPresult = CodingControl.RequestJsonAPIByGet("http://ip-api.com/json/" + fromBody.IP + "?lang=zh-CN");
        if (string.IsNullOrEmpty(IPresult))
        {
            returnValue.ResultCode = APIResult.enumResult.NoData;
        }
        else
        {
            var jsonResult = Newtonsoft.Json.Linq.JObject.Parse(IPresult);
            if (jsonResult["status"].ToString() == "success")
            {
                returnValue.Result = new DBViewModel.IPCounty();
                returnValue.Result.Country = jsonResult["country"].ToString();
                returnValue.Result.City = jsonResult["city"].ToString();
                returnValue.Result.Region = jsonResult["regionName"].ToString();
                returnValue.Result.IP = fromBody.IP;
                returnValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                returnValue.ResultCode = APIResult.enumResult.Error;
            }
        }

        return returnValue;
    }

    [ActionName("Logout")]
    [HttpGet]
    [HttpPost]
    public void Logout([FromBody] string BID)
    {
        BackendDB backendDB = new BackendDB();
        //RedisCache.BIDContext.ClearBID(BID);

    }

    [ActionName("CreateSession")]
    [HttpGet]
    [HttpPost]
    public string CreateSession(string BID)
    {


        var AdminData = new RedisCache.BIDContext.BIDInfo();
        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);


        return JsonConvert.SerializeObject(AdminData);
    }

    [ActionName("UpdateBID")]
    [HttpGet]
    [HttpPost]
    public APIResult UpdateBID([FromBody] string BID)
    {


        APIResult returnValue = new APIResult();
        if (RedisCache.BIDContext.CheckBIDExist(BID) == true)
        {
            if (RedisCache.BIDContext.RefreshBID(BID) == true)
            {
                returnValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                returnValue.ResultCode = APIResult.enumResult.SessionError;
            }
        }
        else
        {
            returnValue.ResultCode = APIResult.enumResult.SessionError;
        }



        return returnValue;
    }

    [ActionName("CheckLoginPermission")]
    [HttpGet]
    [HttpPost]
    public APIResult CheckLoginPermission(string BID)
    {
        APIResult returnValue = new APIResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            returnValue.ResultCode = APIResult.enumResult.SessionError;
            return returnValue;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();


        if (backendDB.CheckLoginPermission(AdminData.forCompanyID))
        {
            returnValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            returnValue.ResultCode = APIResult.enumResult.VerificationError;
        }

        return returnValue;
    }


    [ActionName("CheckLoginIPByCompany")]
    [HttpGet]
    [HttpPost]
    public APIResult CheckLoginIPByCompany(string BID)
    {

        BackendDB backendDB = new BackendDB();
        APIResult returnValue = new APIResult();
        string fingerprint = GetFingerprint();

        try
        {
            if (!RedisCache.BIDContext.CheckBIDExist(BID))
            {
                returnValue.ResultCode = APIResult.enumResult.SessionError;
                return returnValue;
            }
            RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

            string UserIP = CodingControl.GetUserIP();
            //if (AdminData.CompanyType == 0)
            //{
            if (!Pay.IsTestSite)
            {

                if (!CodingControl.CheckXForwardedFor())
                {

                    BackendFunction backendFunction = new BackendFunction();
                    string IP = backendFunction.CheckIPInTW(UserIP);
                    int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "登入IP有误:" + UserIP + ",X-Forwarded:" + HttpContext.Current.Request.Headers["X-Forwarded-For"], IP, fingerprint);
                    backendDB.InsertBotSendLog(AdminData.CompanyCode, "登入帳號:" + AdminData.AdminAccount + ",登入IP有误:" + UserIP);
                    string XForwardIP = CodingControl.GetXForwardedFor();
                    CodingControl.WriteXFowardForIP2(AdminOP);

                    RedisCache.BIDContext.ClearBID(BID);
                    returnValue.ResultCode = APIResult.enumResult.VerificationError;
                    returnValue.Message = "";
                    return returnValue;
                }

                if (backendDB.CheckLoginIP(UserIP, AdminData.CompanyCode))
                {
                    returnValue.ResultCode = APIResult.enumResult.OK;
                }
                else
                {
                    BackendFunction backendFunction = new BackendFunction();
                    string IP = backendFunction.CheckIPInTW(UserIP);
                    int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "登入 IP 有误:" + UserIP + ",X-Forwarded:" + HttpContext.Current.Request.Headers["X-Forwarded-For"], IP, fingerprint);
                    backendDB.InsertBotSendLog(AdminData.CompanyCode, "登入帳號:" + AdminData.AdminAccount + ",登入IP有误:" + UserIP);

                    CodingControl.WriteXFowardForIP2(AdminOP);

                    RedisCache.BIDContext.ClearBID(BID);
                    returnValue.ResultCode = APIResult.enumResult.VerificationError;
                    returnValue.Message = "";
                }
            }

            //}
            //else {
            //    returnValue.ResultCode = APIResult.enumResult.OK;
            //}
        }
        catch (Exception ex)
        {
            CodingControl.WriteBlackList(ex.Message);
            throw;
        }


        return returnValue;
    }

    [ActionName("CheckPermission")]
    [HttpGet]
    [HttpPost]
    public APIResult CheckPermission(string BID, string PermissionName)
    {

        APIResult returnValue = new APIResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            returnValue.ResultCode = APIResult.enumResult.SessionError;
            return returnValue;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        if (backendDB.CheckPermission(PermissionName, AdminData.AdminID))
        {
            returnValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            returnValue.ResultCode = APIResult.enumResult.VerificationError;
        }

        return returnValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetPermissionTableResultbyAdminID")]
    public List<LayoutLeftSideBarResult> GetPermissionTableResultbyAdminID([FromBody] string BID)
    {
        List<LayoutLeftSideBarResult> _LayoutLeftSideBarResult = new List<LayoutLeftSideBarResult>();

        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            var layoutLeftSideBarResult = new LayoutLeftSideBarResult();
            layoutLeftSideBarResult.ResultCode = APIResult.enumResult.SessionError;
            _LayoutLeftSideBarResult.Add(layoutLeftSideBarResult);
            return _LayoutLeftSideBarResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        List<DBViewModel.LayoutLeftSideBarResult> layoutleftsidebars = backendDB.GetPermissionTableResultbyAdminID(AdminData.AdminID);
        if (layoutleftsidebars != null)
        {
            //取得大標籤
            var PermissionCategoryNames = (from p in layoutleftsidebars
                                           select p.PermissionCategoryName).Distinct();
            //根據大標籤取得下面的小分類
            foreach (var PermissionCategoryName in PermissionCategoryNames)
            {
                _LayoutLeftSideBarResult.Add(
                    new LayoutLeftSideBarResult
                    {
                        CategoryDescription = layoutleftsidebars.Where(w => w.PermissionCategoryName == PermissionCategoryName).FirstOrDefault().CategoryDescription,
                        PermissionCategoryName = PermissionCategoryName,
                        PermissionResults = (from p in layoutleftsidebars
                                             where p.PermissionCategoryName == PermissionCategoryName
                                             select p).ToList()
                    }
            );
            }
        }

        return _LayoutLeftSideBarResult;
    }

    #region 公司相關

    [HttpGet]
    [HttpPost]
    [ActionName("GetOffLineResult")]
    public OffLineCompanyResult GetOffLineResult([FromBody] FromBody.GetOffLineResultSet data)
    {
        OffLineCompanyResult _CompanyTableResult = new OffLineCompanyResult();


        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        //if (!CodingControl.CheckXForwardedFor())
        //{
        //    RedisCache.BIDContext.ClearBID(BID);
        //    _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
        //    _CompanyTableResult.Message = "";
        //    return _CompanyTableResult;
        //}

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        BackendDB backendDB = new BackendDB();
        List<DBViewModel.OffLineCompany> companys = backendDB.GetOffLineResult(data, AdminData.forCompanyID);
        if (companys != null)
        {
            _CompanyTableResult.CompanyResults = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyTableResult")]
    public CompanyTableResult GetCompanyTableResult([FromBody] string BID)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();


        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.Company> companys = backendDB.GetCompany(AdminData.forCompanyID, AdminData.CompanyType);
        if (companys != null)
        {
            _CompanyTableResult.CompanyResults = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyTableResultByCompanyType")]
    public CompanyTableResult GetCompanyTableResultByCompanyType([FromBody] string BID)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();


        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.Company> companys = backendDB.GetCompanyByCompanyType();
        if (companys != null)
        {
            _CompanyTableResult.CompanyResults = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyTableResult2")]
    public CompanyTableResult GetCompanyTableResult2([FromBody] string BID)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();


        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.Company> companys = backendDB.GetCompany2(AdminData.forCompanyID, AdminData.CompanyType);
        if (companys != null)
        {
            _CompanyTableResult.CompanyResults = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetAgentCompany")]
    public CompanyTableResult GetAgentCompany([FromBody] string BID)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();


        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.Company> companys = backendDB.GetAgentCompany();
        if (companys != null)
        {
            _CompanyTableResult.CompanyResults = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyByID")]
    public GetCompanyByIDResult GetCompanyByID([FromBody] string BID)
    {
        GetCompanyByIDResult _GetCompanyByIDResult = new GetCompanyByIDResult();
        DBModel.Company companyData = null;
        BackendDB backendDB = new BackendDB();
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _GetCompanyByIDResult.ResultCode = APIResult.enumResult.SessionError;
            return _GetCompanyByIDResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        companyData = backendDB.GetCompanyByID(AdminData.forCompanyID);

        if (companyData != null)
        {
            _GetCompanyByIDResult.CompanyData = companyData;
            _GetCompanyByIDResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _GetCompanyByIDResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _GetCompanyByIDResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetBankData")]
    public BankDatas GetBankData([FromBody] FromBody.GetBankDataSet data)
    {
        BankDatas retValue = new BankDatas() { BankDataList = new List<BankData>() };
        BackendDB backendDB = new BackendDB();
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //if (AdminData.CompanyType != 0)
        //{
        //    retValue.ResultCode = APIResult.enumResult.VerificationError;
        //    return retValue;
        //}

        var BankArray = new BackendFunction().GetWithdrawBankSettingData(data.CurrencyType);

        if (BankArray != null)
        {
            for (int i = 0; i < BankArray.Count; i++)
            {
                var BankData = new BankData();
                BankData.BankCode = BankArray[i]["BankName"].ToString();
                BankData.BankName = BankArray[i]["BankName"].ToString();
                retValue.BankDataList.Add(BankData);
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
            return retValue;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyAllServiceDetailData")]
    public GetCompanyAllServiceDetail GetCompanyAllServiceDetailData([FromBody] FromBody.Company fromBody)
    {
        GetCompanyAllServiceDetail retValue = new GetCompanyAllServiceDetail();
        BackendDB backendDB = new BackendDB();
        int companyID;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {
            companyID = fromBody.CompanyID;
        }
        else if (AdminData.CompanyType == 4)
        {
            companyID = AdminData.forCompanyID;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.WithdrawRelations = backendDB.GetCompanyWithdrawRelationResult(companyID);
        retValue.WithdrawLimits = backendDB.GetWithdrawLimitResult(new DBModel.WithdrawLimit() { WithdrawLimitType = 1, ProviderCode = "", CompanyID = companyID });
        retValue.CompanyServiceResults = backendDB.GetCompanyServiceTableByCompanyID(companyID);
        if (retValue.WithdrawRelations != null || retValue.WithdrawLimits != null || retValue.CompanyServiceResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertCompanyTableResult")]
    public InsertCompanyTable InsertCompanyTableResult([FromBody] FromBody.Company CompanyData)
    {
        InsertCompanyTable _CompanyTableResult = new InsertCompanyTable();
        int InsertAdminRoleID = 0;
        int InsertCompanyID = 0;
        BackendDB backendDB = new BackendDB();
        List<string> LstPermissions = new List<string>();
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(CompanyData.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(CompanyData.BID);

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(CompanyData.BID);
                _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
                _CompanyTableResult.Message = "";
                return _CompanyTableResult;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyTableResult;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(CompanyData.BID);
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            _CompanyTableResult.Message = "";
            return _CompanyTableResult;
        }

        //帳號重複
        if (backendDB.CheckAdminExistByLoginAccount(CompanyData.CompanyCode) > 0)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.DataExist;
            return _CompanyTableResult;
        }

        //帳號重複
        if (backendDB.GetCompanyByCode(CompanyData.CompanyCode) != null)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.DataExist;
            return _CompanyTableResult;
        }

        //if (CompanyData.WithdrawType == 0) {
        //    CompanyData.AutoWithdrawalServiceType = "";
        //}

        _CompanyTableResult.CompanyResult = backendDB.InsertCompany(CompanyData, AdminData.AdminID);

        InsertCompanyID = _CompanyTableResult.CompanyResult.CompanyID;

        if (InsertCompanyID <= 0)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.DataExist;
            return _CompanyTableResult;
        }

        //取得一般權限模組
        if (CompanyData.CompanyType == 2)
        {
            LstPermissions = backendDB.GetPermissionByRoleName("AgentClass", 1);
        }
        else if (CompanyData.CompanyType == 1)
        {
            LstPermissions = backendDB.GetPermissionByRoleName("ModuleNomalPermission", 1);
        }
        //建立後台角色
        InsertAdminRoleID = backendDB.InsertAdminRole(InsertCompanyID, CompanyData.CompanyCode, new List<string>(), LstPermissions);
        //建立後台帳號
        backendDB.InsertAdmin(InsertCompanyID, InsertAdminRoleID, CompanyData.CompanyCode, CompanyData.CompanyCode, "", "", 0);
        //建立公司錢包
        backendDB.InsertCompanyPoint(InsertCompanyID, CompanyData.CurrencyType);

        if (CompanyData.ParentCompanyID != 0)
        {
            //建立公司渠道
            backendDB.FastInsertCompanyServiceFromParentCompany(CompanyData.ParentCompanyID, InsertCompanyID, CompanyData.CurrencyType);
        }
        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增商户:" + CompanyData.CompanyCode, IP, fingerprint);

        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);

        _CompanyTableResult.Message = InsertCompanyID.ToString();
        _CompanyTableResult.ResultCode = APIResult.enumResult.OK;


        return _CompanyTableResult;
    }

    [HttpPost]
    [ActionName("UpdateCompanyWithdrawlType")]
    public CompanyTableResult UpdateCompanyWithdrawlType([FromBody] FromBody.Company CompanyData)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();
        int companyResult = 0;
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(CompanyData.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(CompanyData.BID);

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(CompanyData.BID);
                _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
                _CompanyTableResult.Message = "";
                return _CompanyTableResult;
            }
        }

        if (AdminData.CompanyType != 4)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyTableResult;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(CompanyData.BID);
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            _CompanyTableResult.Message = "";
            return _CompanyTableResult;
        }

        //if (CompanyData.WithdrawType == 0) {
        //    CompanyData.AutoWithdrawalServiceType = "";
        //}
        CompanyData.CompanyID = AdminData.forCompanyID;
        companyResult = backendDB.UpdateCompanyWithdrawlType(CompanyData);

        if (companyResult == -1)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.DataExist;
        }
        else if (companyResult != 0)
        {
            string WithdrawType = "";//代付API规则
            if (CompanyData.WithdrawType == 0)
            {
                WithdrawType = "后台审核";
            }
            else if (CompanyData.WithdrawType == 1)
            {
                WithdrawType = "自动代付";
            }


            string WithdrawAPIType = "未启用";//支援代付方式

            if (CompanyData.WithdrawAPIType == 1)
            {
                WithdrawAPIType = "后台申请";
            }
            else if (CompanyData.WithdrawAPIType == 2)
            {
                WithdrawAPIType = "API代付";
            }
            else if (CompanyData.WithdrawAPIType == 3)
            {
                WithdrawAPIType = "后台申请,API代付";
            }

            string BackendLoginIPType = "";//后台IP检查
            if (CompanyData.BackendLoginIPType == 0)
            {
                BackendLoginIPType = "启用";
            }
            else if (CompanyData.BackendLoginIPType == 1)
            {
                BackendLoginIPType = "停用";
            }


            string BackendWithdrawType = "";//后台送单是否经过审核
            if (CompanyData.BackendWithdrawType == 0)
            {
                BackendWithdrawType = "启用";
            }
            else if (CompanyData.BackendWithdrawType == 1)
            {
                BackendWithdrawType = "停用";
            }

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("修改商户,商户名称:{0},代付API规则:{1},代付通道代码:{2},代付API规则 :{3},后台IP检查:{4},后台送单是否经过审核:{5}", CompanyData.CompanyName, WithdrawType, CompanyData.AutoWithdrawalServiceType, WithdrawAPIType, BackendLoginIPType, BackendWithdrawType), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.Error;
        }

        return _CompanyTableResult;
    }

    [HttpPost]
    [ActionName("UpdateCompanyTableResult")]
    public CompanyTableResult UpdateCompanyTableResult([FromBody] FromBody.Company CompanyData)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();
        int companyResult = 0;
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(CompanyData.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(CompanyData.BID);

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(CompanyData.BID);
                _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
                _CompanyTableResult.Message = "";
                return _CompanyTableResult;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyTableResult;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(CompanyData.BID);
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            _CompanyTableResult.Message = "";
            return _CompanyTableResult;
        }

        //if (CompanyData.WithdrawType == 0) {
        //    CompanyData.AutoWithdrawalServiceType = "";
        //}

        companyResult = backendDB.UpdateCompany(CompanyData);

        if (companyResult == -1)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.DataExist;
        }
        else if (companyResult != 0)
        {
            string WithdrawType = "";//代付API规则
            if (CompanyData.WithdrawType == 0)
            {
                WithdrawType = "后台审核";
            }
            else if (CompanyData.WithdrawType == 1)
            {
                WithdrawType = "自动代付";
            }

            string CompanyState = "";
            if (CompanyData.CompanyState == 0)
            {
                CompanyState = "启用";
            }
            else if (CompanyData.CompanyState == 1)
            {
                CompanyState = "停用";
            }

            string WithdrawAPIType = "未启用";//支援代付方式

            if (CompanyData.WithdrawAPIType == 1)
            {
                WithdrawAPIType = "后台申请";
            }
            else if (CompanyData.WithdrawAPIType == 2)
            {
                WithdrawAPIType = "API代付";
            }
            else if (CompanyData.WithdrawAPIType == 3)
            {
                WithdrawAPIType = "后台申请,API代付";
            }

            string BackendLoginIPType = "";//后台IP检查
            if (CompanyData.BackendLoginIPType == 0)
            {
                BackendLoginIPType = "启用";
            }
            else if (CompanyData.BackendLoginIPType == 1)
            {
                BackendLoginIPType = "停用";
            }


            string BackendWithdrawType = "";//后台送单是否经过审核
            if (CompanyData.BackendWithdrawType == 0)
            {
                BackendWithdrawType = "启用";
            }
            else if (CompanyData.BackendWithdrawType == 1)
            {
                BackendWithdrawType = "停用";
            }

            //对应供应商群组
            string ProviderGroups = CompanyData.ProviderGroups;
            if (ProviderGroups == "0")
            {
                ProviderGroups = "不指定";
            }
            //是否开启确认商户送单功能
            string CheckCompanyWithdrawType = "";
            if (CompanyData.CheckCompanyWithdrawType == 0)
            {
                CheckCompanyWithdrawType = "启用";
            }
            else if (CompanyData.CheckCompanyWithdrawType == 1)
            {
                CheckCompanyWithdrawType = "停用";
            }

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("修改商户,商户代码:{0},商户名称:{1},代付API规则:{2},代付通道代码:{3},商户状态:{4},代付API规则 :{5},后台IP检查:{6},后台送单是否经过审核:{7},对应供应商群组:{8},是否开启确认商户送单功能:{9}", CompanyData.CompanyCode, CompanyData.CompanyName, WithdrawType, CompanyData.AutoWithdrawalServiceType, CompanyState, WithdrawAPIType, BackendLoginIPType, BackendWithdrawType, ProviderGroups, CheckCompanyWithdrawType), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            RedisCache.Company.UpdateCompanyByCode(CompanyData.CompanyCode);
            RedisCache.CompanyPoint.UpdateCompanyPointByID(CompanyData.CompanyID);
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.Error;
        }

        return _CompanyTableResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("UpdateAllCompanyKey")]
    public APIResult UpdateAllCompanyKey([FromBody] FromBody.Company fromBody)
    {
        APIResult result = new APIResult();

        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }

        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        var successCount = backendDB.UpdateAllCompanyRedis();

        if (successCount > 0)
        {
            result.ResultCode = APIResult.enumResult.OK;
            result.Message = successCount.ToString();
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("DisableCompanyByID")]
    public CompanyTableResult DisableCompanyByID([FromBody] FromBody.Company fromBody)
    {
        CompanyTableResult _CompanyTableResult = new CompanyTableResult();
        int companyResult = 0;
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }


        RedisCache.BIDContext.BIDInfo AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyTableResult;
        }

        companyResult = backendDB.DisableCompanyByID(fromBody.CompanyID);

        if (companyResult != 0)
        {
            string CompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "商户:" + CompanyName + ",状态为停用", IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.Error;
        }
        return _CompanyTableResult;
    }
    #endregion

    #region 會員相關
    [HttpPost]
    [ActionName("LoginByGoogle")]
    public LoginResult LoginByGoogle([FromBody] FromBody.Login fromBody)
    {
        LoginResult loginResult = new LoginResult();
        BackendDB backendDB = new BackendDB();

        BackendFunction backendFunction = new BackendFunction();
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog("LoginFail", "偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                //RedisCache.BIDContext.ClearBID(fromBody.BID);
                loginResult.ResultCode = APIResult.enumResult.Error;
                loginResult.Message = "";
                return loginResult;
            }
        }
        DBModel.AdminWithLoginPassword admin = backendFunction.CheckLogin(fromBody);

        if (admin != null)
        {
            loginResult.AdminAccount = admin.LoginAccount;
            loginResult.AdminID = admin.AdminID;
            loginResult.AdminType = admin.AdminType;
            loginResult.CompanyCode = admin.CompanyCode;
            loginResult.CompanyName = "";
            loginResult.CompanyType = admin.CompanyType;
            loginResult.forCompanyID = admin.forCompanyID;
            loginResult.SortKey = admin.SortKey;
            loginResult.RealName = admin.RealName;

            if (admin.CompanyType == 0 || admin.CompanyType == 3)
            {
                var adminModel = backendDB.GetAdminByLoginAccountWithGoogleKey(admin.LoginAccount);
                if (!Pay.IsTestSite)
                {
                    if (string.IsNullOrEmpty(adminModel.GoogleKey))
                    {
                        loginResult.Message = "尚未绑定 Google 验证器";
                        loginResult.ResultCode = APIResult.enumResult.GoogleKeyError;
                        loginResult.CheckGoogleKeySuccess = false;

                    }
                    else
                    {
                        //檢查google認證
                        if (backendFunction.CheckGoogleKey(adminModel.GoogleKey, fromBody.UserKey))
                        {
                            loginResult.ResultCode = APIResult.enumResult.OK;
                            loginResult.CheckGoogleKeySuccess = true;
                        }
                        else
                        {
                            loginResult.ResultCode = APIResult.enumResult.GoogleKeyError;
                            return loginResult;
                        }
                    }
                }
                else
                {
                    loginResult.ResultCode = APIResult.enumResult.OK;
                    loginResult.CheckGoogleKeySuccess = true;
                }
            }
            else
            {
                var companyModel = backendDB.GetCompanyByIDWithGooleKey(admin.forCompanyID);
                if (string.IsNullOrEmpty(companyModel.GoogleKey))
                {
                    loginResult.Message = "尚未绑定 Google 验证器";
                    loginResult.ResultCode = APIResult.enumResult.OK;
                    loginResult.CheckGoogleKeySuccess = false;
                }
                else
                {
                    if (!Pay.IsTestSite)
                    {
                        //檢查google認證
                        if (backendFunction.CheckGoogleKey(companyModel.GoogleKey, fromBody.UserKey))
                        {
                            loginResult.ResultCode = APIResult.enumResult.OK;
                            loginResult.CheckGoogleKeySuccess = true;
                        }
                        else
                        {
                            loginResult.ResultCode = APIResult.enumResult.GoogleKeyError;
                            return loginResult;
                        }
                    }
                    else
                    {
                        loginResult.ResultCode = APIResult.enumResult.OK;
                        loginResult.CheckGoogleKeySuccess = true;
                    }

                }


            }

            if (loginResult.ResultCode == APIResult.enumResult.OK)
            {

                HttpContext.Current.Response.Cookies.Add(new HttpCookie("BID", RedisCache.BIDContext.CreateBID(admin.CompanyCode, admin.LoginAccount, admin.RealName, admin.forAdminRoleID, CodingControl.GetUserIP(), loginResult.CheckGoogleKeySuccess)));
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("Fingerprint", fromBody.Fingerprint));
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());

                int AdminOP = backendDB.InsertAdminOPLog(admin.forCompanyID, admin.AdminID, 0, fromBody.LoginAccount + ",登入成功", IP, fromBody.Fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
            }
            else
            {
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(0, 0, 0, fromBody.LoginAccount + ",登入失败", IP, fromBody.Fingerprint);
                backendDB.InsertBotSendLog(admin.CompanyCode, "登入帳號:" + fromBody.LoginAccount + ",登入失败,IP:" + IP);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                loginResult.ResultCode = APIResult.enumResult.VerificationError;
            }
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(0, 0, 0, fromBody.LoginAccount + ",登入失败", IP, fromBody.Fingerprint);
            backendDB.InsertBotSendLog("", "登入帳號:" + fromBody.LoginAccount + ",登入失败,IP:" + IP);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            loginResult.ResultCode = APIResult.enumResult.VerificationError;
        }

        return loginResult;
    }

    [HttpPost]
    [ActionName("GetAdminTableResult")]
    public AdminTableResult GetAdminTableResult([FromBody] FromBody.GetAdminTableResult fromBody)
    {
        AdminTableResult _AdminTableResult = new AdminTableResult();
        LoginResult AdminData = new LoginResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminTableResult;
        }
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.AdminTableResult> admins = backendDB.GetAdminTableByCompanyID(fromBody.CompanyID);
        if (admins != null)
        {
            _AdminTableResult.AdminResults = admins;
            _AdminTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _AdminTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminTableResult;
    }

    [HttpPost]
    [ActionName("InsertAdmin")]
    public APIResult InsertAdmin([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "建立后台账号:" + fromBody.LoginAccount, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);

        DBretValue = backendDB.InsertAdmin(fromBody.CompanyID, fromBody.AdminroleID, fromBody.LoginAccount, fromBody.Password, fromBody.RealName, fromBody.Description, fromBody.AdminType);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateAdmin")]
    public APIResult UpdateAdmin([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改后台账号:" + fromBody.LoginAccount, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);

        DBretValue = backendDB.UpdateAdmin(fromBody.AdminID, fromBody.CompanyID, fromBody.AdminroleID, fromBody.Password, fromBody.RealName, fromBody.Description, fromBody.AdminType, fromBody.AdminState);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableAdmin")]
    public APIResult DisableAdmin([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }



        DBretValue = backendDB.DisableAdmin(fromBody.AdminID, fromBody.CompanyID);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "停用后台账号:" + fromBody.LoginAccount, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateLoginPassword")]
    public APIResult UpdateLoginPassword([FromBody] FromBody.UpdateLoginPassword fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改后台密码", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);

        DBretValue = backendDB.UpdateLoginPassword(AdminData.AdminID, fromBody.Password, fromBody.Newpassword);

        if (DBretValue >= 1)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    #endregion

    #region 专属供应商账号设定
    [HttpPost]
    [ActionName("InsertProxyProviderAcount")]
    public APIResult InsertProxyProviderAcount([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.InsertProxyProviderAcount(AdminData.forCompanyID, fromBody.AdminroleID, fromBody.LoginAccount, fromBody.Password, fromBody.RealName, fromBody.Description, fromBody.AdminType, fromBody.GroupID);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "专属供应商建立账号:" + fromBody.LoginAccount, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProxyProviderAcount")]
    public APIResult UpdateProxyProviderAcount([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }



        DBretValue = backendDB.UpdateProxyProviderAcount(fromBody.AdminID, AdminData.forCompanyID, fromBody.AdminroleID, fromBody.Password, fromBody.RealName, fromBody.Description, fromBody.AdminType, fromBody.AdminState, fromBody.GroupID);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "专属供应商修改后台账号:" + fromBody.LoginAccount, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableProxyProviderAcount")]
    public APIResult DisableProxyProviderAcount([FromBody] FromBody.InsertAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }



        DBretValue = backendDB.DisableProxyProviderAcount(fromBody.AdminID, AdminData.forCompanyID);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "专属供应商停用后台账号:" + fromBody.LoginAccount, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetProxyProviderRoleTableResult")]
    public AdminRoleTableResult GetProxyProviderRoleTableResult([FromBody] FromBody.GetAdminRoleTableResult fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AdminRoleTableResult _AdminRoleTableResult = new AdminRoleTableResult();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminRoleTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _AdminRoleTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.AdminRole> adminroles = backendDB.GetAdminRoleTableByCompanyID(AdminData.forCompanyID);
        if (adminroles != null)
        {
            _AdminRoleTableResult.AdminRoleResult = adminroles;
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminRoleTableResult;
    }

    [HttpPost]
    [ActionName("GetProxyProviderAcountResult")]
    public AdminTableResult GetProxyProviderAcountResult([FromBody] FromBody.GetAdminTableResult fromBody)
    {
        AdminTableResult _AdminTableResult = new AdminTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.AdminTableResult> admins = backendDB.GetProxyProviderAcountResult(AdminData.forCompanyID);
        if (admins != null)
        {
            _AdminTableResult.AdminResults = admins;
            _AdminTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _AdminTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminTableResult;
    }
    #endregion

    #region 专属供应商群组设定

    [HttpPost]
    [ActionName("GetProxyProviderGroupFrozenPoint")]
    public ProxyProviderGroupFrozenPointResult GetProxyProviderGroupFrozenPoint([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        ProxyProviderGroupFrozenPointResult _Result = new ProxyProviderGroupFrozenPointResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 3))
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroupFrozenPointHistory> datas = backendDB.GetProxyProviderGroupFrozenPoint(fromBody.ProviderCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupTableResultByAdmin")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupTableResultByAdmin([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupTableResultByAdmin(fromBody.ProviderCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetAllProxyProviderGroupTableResultByAdmin")]
    public ProxyProviderGroupTableResult GetAllProxyProviderGroupTableResultByAdmin([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetAllProxyProviderGroupTableResultByAdmin();
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupWeightByAdmin")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupWeightByAdmin([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupWeightByAdmin(fromBody.ProviderCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupOnWithdrawalAmountResultByAdmin")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupOnWithdrawalAmountResultByAdmin([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupOnWithdrawalAmountResultByAdmin(fromBody.ProviderCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupNameByAdmin")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupNameByAdmin([FromBody] string BID)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupTableResult("YE888");
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupName")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupName([FromBody] string BID)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupTableResult(AdminData.CompanyCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("GetProxyProviderGroupTableResult")]
    public ProxyProviderGroupTableResult GetProxyProviderGroupTableResult([FromBody] string BID)
    {
        ProxyProviderGroupTableResult _Result = new ProxyProviderGroupTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }

        BackendDB backendDB = new BackendDB();
        //专属供应商的 CompanyCode=ProviderCode
        List<DBModel.ProxyProviderGroup> datas = backendDB.GetProxyProviderGroupTableResult(AdminData.CompanyCode);
        if (datas != null)
        {
            _Result.Results = datas;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }
        return _Result;
    }

    [HttpPost]
    [ActionName("InsertProxyProviderGroup")]
    public APIResult InsertProxyProviderGroup([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        APIResult _Result = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _Result.ResultCode = APIResult.enumResult.VerificationError;
            return _Result;
        }


        //专属供应商的 CompanyCode = ProviderCode
        DBretValue = backendDB.InsertProxyProviderGroup(AdminData.CompanyCode, fromBody.GroupName, fromBody.MinAmount, fromBody.MaxAmount);

        if (DBretValue == -1)
        {
            _Result.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "建立专属供应商群组:" + fromBody.GroupName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            _Result.ResultCode = APIResult.enumResult.OK;
        }

        return _Result;
    }

    [HttpPost]
    [ActionName("UpdateProxyProviderGroup")]
    public APIResult UpdateProxyProviderGroup([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        //专属供应商的 CompanyCode = ProviderCode
        DBretValue = backendDB.UpdateProxyProviderGroup(AdminData.CompanyCode, fromBody.GroupName, fromBody.GroupID, fromBody.State, fromBody.MinAmount, fromBody.MaxAmount);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改专属供应商群组:" + fromBody.GroupName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProxyProviderGroupWeight")]
    public APIResult UpdateProxyProviderGroupWeight([FromBody] FromBody.ProxyProviderGroupWeightSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var ProviderDatas = backendDB.GetProviderServiceResult(fromBody.GroupData.First().forProviderCode, "OOB02", fromBody.CurrencyType);

        if (ProviderDatas == null)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "取得供应商资料失败";
            return retValue;
        }

        var ProviderData = ProviderDatas.First();
        var OnLineProviderGroup = fromBody.GroupData.Where(w => w.State == 0 && w.Weight != 0).ToList();

        if (OnLineProviderGroup.Count == 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "当前设定无组别可接未指定群组订单 ";
            return retValue;
        }

        if (OnLineProviderGroup.Max(m => m.MaxAmount) < ProviderData.MaxOnceAmount)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "限额设定有误(群组资料未包含最大限额),限额 " + ProviderData.MinOnceAmount.ToString("#.##") + "-" + ProviderData.MaxOnceAmount.ToString("#.##");
            return retValue;
        }

        if (OnLineProviderGroup.Min(m => m.MinAmount) > ProviderData.MinOnceAmount)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "限额设定有误(群组资料未包含最小限额),限额 " + ProviderData.MinOnceAmount.ToString("#.##") + "-" + ProviderData.MaxOnceAmount.ToString("#.##");
            return retValue;
        }

        //专属供应商的 CompanyCode = ProviderCode
        DBretValue = backendDB.UpdateProxyProviderGroupWeight(fromBody.GroupData);

        if (DBretValue <= 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "修改资料失败";
            return retValue;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改专属供应商权重", IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableProxyProviderGroup")]
    public APIResult DisableProxyProviderGroup([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        //专属供应商的 CompanyCode = ProviderCode
        DBretValue = backendDB.DisableProxyProviderGroup(fromBody.GroupID, AdminData.CompanyCode);

        if (DBretValue >= 1)
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "(停用/启用)专属供应商群组:" + fromBody.GroupName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }
    #endregion

    #region 後台角色
    [HttpPost]
    [ActionName("GetAdminRoleTableResult")]
    public AdminRoleTableResult GetAdminRoleTableResult([FromBody] FromBody.GetAdminRoleTableResult fromBody)
    {
        AdminRoleTableResult _AdminRoleTableResult = new AdminRoleTableResult();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminRoleTableResult;
        }
        BackendDB backendDB = new BackendDB();
        List<DBModel.AdminRole> adminroles = backendDB.GetAdminRoleTableByCompanyID(fromBody.CompanyID);
        if (adminroles != null)
        {
            _AdminRoleTableResult.AdminRoleResult = adminroles;
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _AdminRoleTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminRoleTableResult;
    }

    [HttpPost]
    [ActionName("GetPermissionByAdminRoleID")]
    public AdminRolePermissionResult GetPermissionByAdminRoleID([FromBody] FromBody.GetPermissionByAdminRoleID fromBody)
    {
        AdminRolePermissionResult _AdminRolePermissionResult = new AdminRolePermissionResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminRolePermissionResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminRolePermissionResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }
        //CompanyType 0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        List<DBViewModel.AdminRolePermission> adminrolepermission = backendDB.GetPermissionByAdminRoleID(fromBody.AdminRoleID, AdminData.CompanyType);
        if (adminrolepermission != null)
        {
            _AdminRolePermissionResult.AdminRolePermissions = adminrolepermission;
            _AdminRolePermissionResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _AdminRolePermissionResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminRolePermissionResult;
    }

    [HttpPost]
    [ActionName("InsertAdminRole")]
    public APIResult InsertAdminRole([FromBody] FromBody.InsertAdminRole fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }
        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            fromBody.AdminPermission = new List<string>();
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "增加后台角色:" + fromBody.RoleName, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);

        DBretValue = backendDB.InsertAdminRole(fromBody.CompanyID, fromBody.RoleName, fromBody.AdminPermission, fromBody.NormalPermission);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateAdminRole")]
    public APIResult UpdateAdminRole([FromBody] FromBody.UpdateAdminRole fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }
        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            fromBody.AdminPermission = new List<string>();
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改后台角色:" + fromBody.RoleName, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBretValue = backendDB.UpdateAdminRole(fromBody.CompanyID, fromBody.AdminRoleID, fromBody.RoleName, fromBody.AdminPermission, fromBody.NormalPermission);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("GetPermissionTableResult")]
    public PermissionTableResult GetPermissionTableResult([FromBody] string BID)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PermissionTableResult _PermissionTableResult = new PermissionTableResult();
        bool AdminType = false;

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _PermissionTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PermissionTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType == 0)
        {
            AdminType = true;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.Permission> permissiones = backendDB.GetPermissionTable(AdminType);

        if (permissiones != null)
        {

            _PermissionTableResult.PermissionResult = permissiones;
            _PermissionTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PermissionTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PermissionTableResult;
    }

    #endregion

    #region 權限設定

    [HttpPost]
    [ActionName("GetPermissionTableResultForPermissionSet")]
    public PermissionTableResult GetPermissionTableResultForPermissionSet(FromBody.PermissionSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PermissionTableResult _PermissionTableResult = new PermissionTableResult();
        bool AdminType = false;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PermissionTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PermissionTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType == 0)
        {
            AdminType = true;
        }
        else
        {
            _PermissionTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PermissionTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.Permission> permissiones = backendDB.GetPermissionTableByPermissionCategoryID(fromBody.PermissionCategoryID);

        if (permissiones != null)
        {

            _PermissionTableResult.PermissionResult = permissiones;
            _PermissionTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PermissionTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PermissionTableResult;
    }

    [HttpPost]
    [ActionName("GetAdminRolePermissionResult")]
    public AdminRolePermissionTableResult GetAdminRolePermissionResult(FromBody.PermissionSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AdminRolePermissionTableResult _AdminRolePermissionTableResult = new AdminRolePermissionTableResult();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AdminRolePermissionTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _AdminRolePermissionTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _AdminRolePermissionTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _AdminRolePermissionTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBViewModel.AdminRolePermissionResult> adminRolePermissionResults = backendDB.GetAdminRolePermissionResultByPermissionName(fromBody.PermissionName);

        if (adminRolePermissionResults != null)
        {
            _AdminRolePermissionTableResult.AdminRolePermissionResult = adminRolePermissionResults;
            _AdminRolePermissionTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _AdminRolePermissionTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AdminRolePermissionTableResult;
    }

    [HttpPost]
    [ActionName("InsertPermission")]
    public APIResult InsertPermission([FromBody] FromBody.PermissionSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "新增后台功能", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBretValue = backendDB.InsertPermission(fromBody);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdatePermission")]
    public APIResult UpdatePermission([FromBody] FromBody.PermissionSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "修改后台功能", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBretValue = backendDB.UpdatePermission(fromBody);

        if (DBretValue == 1)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DeletePermission")]
    public APIResult DeletePermission([FromBody] FromBody.PermissionSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "删除后台功能", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBretValue = backendDB.DeletePermission(fromBody);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdatePermissionRole")]
    public APIResult UpdatePermissionRole([FromBody] FromBody.PermissionSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改后台角色可用功能", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBretValue = backendDB.UpdatePermissionRole(fromBody.PermissionName, fromBody.PermissionRoles);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }


    #endregion

    #region 供應商總結報表
    [HttpPost]
    [ActionName("GetSummaryProviderByDateTableResult")]
    public SummaryProviderByDateTableResult GetSummaryProviderByDateTableResult(FromBody.SummaryProviderByDate fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        SummaryProviderByDateTableResult _SummaryProviderByDateTableResult = new SummaryProviderByDateTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _SummaryProviderByDateTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.SummaryProviderByDate> summaryProviderByDate = backendDB.GetSummaryProviderByDateTableResult(fromBody);

        if (summaryProviderByDate != null)
        {
            _SummaryProviderByDateTableResult.SummaryProviderByDateResults = summaryProviderByDate;
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _SummaryProviderByDateTableResult;
    }

    [HttpPost]
    [ActionName("GetProxySummaryProviderByDateTableResult")]
    public ProxySummaryProviderByDateTableResult GetProxySummaryProviderByDateTableResult(FromBody.SummaryProviderByDate fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        ProxySummaryProviderByDateTableResult _SummaryProviderByDateTableResult = new ProxySummaryProviderByDateTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _SummaryProviderByDateTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (!(AdminData.CompanyType == 3 || AdminData.CompanyType == 0))
        {
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _SummaryProviderByDateTableResult;
        }
        fromBody.ProviderCode = "YE888";
        BackendDB backendDB = new BackendDB();
        List<DBModel.ProxySummaryProviderByDate> summaryProviderByDate = backendDB.GetProxySummaryProviderByDateTableResult(fromBody);

        if (summaryProviderByDate != null)
        {
            _SummaryProviderByDateTableResult.SummaryProviderByDateResults = summaryProviderByDate;
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _SummaryProviderByDateTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _SummaryProviderByDateTableResult;
    }
    #endregion

    #region 代付紀錄
    [HttpPost]
    [ActionName("GetWithdrawalReport")]
    public WithdrawalTableResult GetWithdrawalReport([FromBody] FromBody.WithdrawalReportSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendDB backendDB = new BackendDB();
        fromBody.CompanyID = AdminData.forCompanyID;
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalReport(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }


    #endregion

    #region 金流交易報表
    [HttpPost]
    [ActionName("GetPaymentTransferLogResult")]
    public PaymentTransferLogResult GetPaymentTransferLogResult([FromBody] FromBody.PaymentTransferLogSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTransferLogResult _PaymentTransferLogResult = new PaymentTransferLogResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTransferLogResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTransferLogResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {

            _PaymentTransferLogResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTransferLogResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentTransferLog> _PaymentTable = backendDB.GetPaymentTransferLogResult(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTransferLogResult.PaymentTableResults = _PaymentTable;
            _PaymentTransferLogResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTransferLogResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTransferLogResult;
    }

    [HttpPost]
    [ActionName("GetDownOrderTransferLogResult")]
    public DownOrderTransferLogResult GetDownOrderTransferLogResult([FromBody] FromBody.DownOrderTransferLogSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DownOrderTransferLogResult _DownOrderTransferResult = new DownOrderTransferLogResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _DownOrderTransferResult.ResultCode = APIResult.enumResult.SessionError;
            return _DownOrderTransferResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {

            _DownOrderTransferResult.ResultCode = APIResult.enumResult.VerificationError;
            return _DownOrderTransferResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.DownOrderTransferLog> _DownOrderTable = backendDB.GetDownOrderTransferLogResult(fromBody);

        if (_DownOrderTable != null)
        {
            _DownOrderTransferResult.DownOrderTableResults = _DownOrderTable;
            _DownOrderTransferResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _DownOrderTransferResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _DownOrderTransferResult;
    }

    [HttpPost]
    [ActionName("GetDownOrderTransferLogResultV2")]
    public DownOrderTransferLogResultV2 GetDownOrderTransferLogResultV2([FromBody] FromBody.DownOrderTransferLogSetV2 fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DownOrderTransferLogResultV2 _DownOrderTransferResult = new DownOrderTransferLogResultV2();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _DownOrderTransferResult.ResultCode = (int)APIResult.enumResult.SessionError;
            return _DownOrderTransferResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {

            _DownOrderTransferResult.ResultCode = (int)APIResult.enumResult.VerificationError;
            return _DownOrderTransferResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.DownOrderTransferLogV2> _DownOrderTable = backendDB.GetDownOrderTransferLogResultV2(fromBody);

        if (_DownOrderTable != null)
        {

            _DownOrderTransferResult.draw = fromBody.draw;
            _DownOrderTransferResult.recordsTotal = _DownOrderTable.First().TotalCount;
            _DownOrderTransferResult.recordsFiltered = _DownOrderTable.First().TotalCount;
            _DownOrderTransferResult.data = _DownOrderTable;//分頁後的資料 


            _DownOrderTransferResult.ResultCode = (int)APIResult.enumResult.OK;
        }
        else
        {
            _DownOrderTransferResult.draw = fromBody.draw;
            _DownOrderTransferResult.recordsTotal = 0;
            _DownOrderTransferResult.recordsFiltered = 0;

            _DownOrderTransferResult.data = new List<DBModel.DownOrderTransferLogV2>();//分頁後的資料 

            _DownOrderTransferResult.ResultCode = (int)APIResult.enumResult.NoData;
        }

        return _DownOrderTransferResult;
    }

    [HttpPost]
    [ActionName("GetAbnormalPaymentTableResult")]
    public PaymentTableResult GetAbnormalPaymentTableResult(FromBody.GetAbnormalPaymentSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetAbnormalPaymentTableResult(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentReportReviewResult")]
    public PaymentTableResult GetPaymentReportReviewResult(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentReportReviewResult(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }


    [HttpPost]
    [ActionName("GetPaymentProviderReportReviewResult")]
    public PaymentTableResult GetPaymentProviderReportReviewResult(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();
        DBModel.Admin AdminModel;
        int GroupID = 0;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 3)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }

        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentProviderReportReviewResult(fromBody, AdminData.CompanyCode, GroupID);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentTableResultByWaitReview")]
    public PaymentTableResult GetPaymentTableResultByWaitReview(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentTableResultByWaitReview(fromBody.ProcessStatus);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }


    [HttpPost]
    [ActionName("GetProviderPaymentTableResultByWaitReview")]
    public PaymentTableResult GetProviderPaymentTableResultByWaitReview(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();
        DBModel.Admin AdminModel;
        int GroupID = 0;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 3)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();

        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }


        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetProviderPaymentTableResultByWaitReview(fromBody.ProcessStatus, AdminData.CompanyCode, GroupID);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentTableResultByAdmin")]
    public PaymentTableResult GetPaymentTableResultByAdmin(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {

        }
        else if (AdminData.CompanyType == 4)
        {
            fromBody.CompanyID = AdminData.forCompanyID;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentTableResultByAdmin(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentResultV2")]
    public DBModel.returnPaymentReportV2 GetPaymentResultV2(FromBody.GetPaymentForAdminV2 fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.returnPaymentReportV2 _PaymentTableResult = new DBModel.returnPaymentReportV2();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {

        }
        else if (AdminData.CompanyType == 4)
        {
            fromBody.CompanyID = AdminData.forCompanyID;
        }
        else
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReportV2> _PaymentTable = backendDB.GetPaymentResultV2(fromBody);

        if (_PaymentTable != null)
        {

            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = _PaymentTable.First().TotalCount;
            _PaymentTableResult.recordsFiltered = _PaymentTable.First().TotalCount;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = _PaymentTable;//分頁後的資料 
            DBModel.StatisticsPaymentAmount DbReturn = backendDB.GetPaymentPointBySearchFilter(fromBody);
            if (DbReturn != null)
            {
                _PaymentTableResult.StatisticsPaymentAmount = DbReturn;
            }

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = 0;
            _PaymentTableResult.recordsFiltered = 0;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = new List<DBModel.PaymentReportV2>();//分頁後的資料 

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.NoData;
        }

        return _PaymentTableResult;
    }



    [HttpPost]
    [ActionName("GetPaymentPointBySearchFilter")]
    public PaymentPointBySearchFilter GetPaymentPointBySearchFilter(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentPointBySearchFilter _PaymentTableResult = new PaymentPointBySearchFilter();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        DBModel.StatisticsPaymentAmount DbReturn = backendDB.GetPaymentPointBySearchFilter(fromBody);

        if (DbReturn != null)
        {
            _PaymentTableResult.Result = DbReturn;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentTableResultByLstPaymentID")]
    public PaymentTableResult GetPaymentTableResultByLstPaymentID(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentTableResultByLstPaymentID(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }


    [HttpPost]
    [ActionName("GetPaymentTableResult")]
    public PaymentTableResult GetPaymentTableResult(FromBody.GetPaymentForAdmin fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentTableResult(fromBody, AdminData.CompanyType, AdminData.forCompanyID);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPaymentTableResult2")]
    public PaymentTableResult GetPaymentTableResult2(FromBody.GetPayment fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetPaymentTableResult2(fromBody, AdminData.forCompanyID);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPatchPaymentTableResult")]
    public PatchPaymentTableResult GetPatchPaymentTableResult([FromBody] string BID)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PatchPaymentTableResult _PaymentTableResult = new PatchPaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentTable> _PaymentTable = backendDB.GetPatchPaymentTableResult();

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetPatchPaymentTableResultByDate")]
    public PatchPaymentTableResult GetPatchPaymentTableResultByDate(FromBody.PaymentTable fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PatchPaymentTableResult _PaymentTableResult = new PatchPaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentTable> _PaymentTable = backendDB.GetPatchPaymentTableResultByDate(fromBody);

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }


    [HttpPost]
    [ActionName("CreatePayment")]
    public CreatePatchPaymentResults CreatePayment([FromBody] FromBody.PaymentSet fromBody)
    {
        CreatePatchPaymentResults result = new CreatePatchPaymentResults();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.CreatePatchPayment DBreturn;
        int CompanyID = 0;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            CompanyID = fromBody.CompanyID;
        }
        else
        {
            CompanyID = AdminData.forCompanyID;
        }

        BackendDB backendDB = new BackendDB();
        DBreturn = backendDB.CreatePayment(CompanyID, fromBody.Amount, fromBody.Description, fromBody.CurrencyType);


        if (DBreturn != null && DBreturn.PatchPaymentState == 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(CompanyID, AdminData.AdminID, 1, "人工充值申请", IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.PaymentResult = DBreturn;
            result.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBreturn != null && DBreturn.PatchPaymentState == -2)
        {
            result.Message = "建立單失敗";
            result.ResultCode = APIResult.enumResult.Other;
        }
        else
        {
            result.Message = "其他錯誤";
            result.ResultCode = APIResult.enumResult.Other;
        }

        return result;
    }

    //[HttpPost]
    //[ActionName("CreatePatchPayment")]
    //public CreatePatchPaymentResults CreatePatchPayment([FromBody] FromBody.PaymentSet fromBody) {
    //    CreatePatchPaymentResults result = new CreatePatchPaymentResults();
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
    //    DBModel.CreatePatchPayment DBreturn;

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID)) {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else {
    //        AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    BackendFunction backendFunction = new BackendFunction();
    //    string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //    int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "建立补单,旧单号:" + fromBody.PaymentSerial, IP, fingerprint);
    //    string XForwardIP = CodingControl.GetXForwardedFor();
    //    CodingControl.WriteXFowardForIP(AdminOP);
    //    DBreturn = backendDB.CreatePatchPayment(fromBody.PaymentSerial, fromBody.Amount, fromBody.PatchDescription);

    //    if (DBreturn != null && DBreturn.PatchPaymentState == 0) {
    //        result.PaymentResult = DBreturn;
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else if (DBreturn != null && DBreturn.PatchPaymentState == -1) {
    //        result.Message = "交易序號不存在";
    //        result.ResultCode = APIResult.enumResult.Other;
    //    }
    //    else if (DBreturn != null && DBreturn.PatchPaymentState == -2) {
    //        result.Message = "建立補單失敗";
    //        result.ResultCode = APIResult.enumResult.Other;
    //    }
    //    else if (DBreturn != null && DBreturn.PatchPaymentState == -3) {
    //        result.Message = "此單狀態無法補單";
    //        result.ResultCode = APIResult.enumResult.Other;
    //    }
    //    else {
    //        result.Message = "其他錯誤";
    //        result.ResultCode = APIResult.enumResult.Other;
    //    }

    //    return result;
    //}


    [HttpPost]
    [ActionName("ChangeWithdrawalProcessStatus")]
    public APIResult ChangeWithdrawalProcessStatus([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }


        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "提现单成功转失败,单号:" + fromBody.PaymentSerial, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBreturn = backendDB.ChangeWithdrawalProcessStatus(fromBody.PaymentSerial, AdminData.AdminID);


        if (DBreturn == "审核完成")
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("CancelWithdrawalReviewToSuccess")]
    public APIResult CancelWithdrawalReviewToSuccess([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }


        DBreturn = backendDB.CancelWithdrawalReviewToSuccess(fromBody.PaymentSerial, AdminData.AdminID);


        if (DBreturn == "审核完成")
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "进行中转成功(提现),单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("CancelWithdrawalReviewToFail")]
    public APIResult CancelWithdrawalReviewToFail([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }


        DBreturn = backendDB.CancelWithdrawalReviewToFail(fromBody.PaymentSerial, AdminData.AdminID);


        if (DBreturn == "审核完成")
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "进行中转失败(提现),单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("CancelWithdrawalProviderReview")]
    public APIResult CancelWithdrawalProviderReview([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        int DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }


        DBreturn = backendDB.CancelWithdrawalProviderReview(fromBody.PaymentSerial);


        if (DBreturn == 0)
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "取消上游审核,单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            if (DBreturn == -1)
            {
                retValue.Message = "订单不存在";
            }
            else if (DBreturn == -2)
            {
                retValue.Message = "订单状态有误";
            }
            else if (DBreturn == -3)
            {
                retValue.Message = "锁定失败";
            }
            else if (DBreturn == -4)
            {
                retValue.Message = "此订单非专属供应商订单";
            }
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangeWithdrawalProcessStatusFailToSuccess")]
    public APIResult ChangeWithdrawalProcessStatusFailToSuccess([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "提现单失败转成功,单号:" + fromBody.PaymentSerial, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBreturn = backendDB.ChangeWithdrawalProcessStatusFailToSuccess(fromBody.PaymentSerial, AdminData.AdminID);


        if (DBreturn == "审核完成")
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangePaymentProcessStatusAndRealAmount")]
    public APIResult ChangePaymentProcessStatusAndRealAmount([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "未处理转成功,单号:" + fromBody.PaymentSerial, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBreturn = backendDB.ChangePaymentProcessStatus(fromBody.PaymentSerial, AdminData.AdminID, fromBody.RealAmount);


        if (DBreturn == "审核完成")
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangePaymentProcessStatusSuccessToFail")]
    public APIResult ChangePaymentProcessStatusSuccessToFail([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string DBreturn;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        BackendFunction backendFunction = new BackendFunction();

        if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
        {
            retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
            return retValue;
        }

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "成功转失败,单号:" + fromBody.PaymentSerial, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        DBreturn = backendDB.ChangePaymentProcessStatusSuccessToFail(fromBody.PaymentSerial, AdminData.AdminID);


        if (DBreturn == "审核完成")
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = DBreturn;
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ConfirmManualPayment")]
    public APIResult ConfirmManualPayment([FromBody] FromBody.PaymentSet fromBody)
    {
        UpdatePatmentResultByPatmentSerialResult result = new UpdatePatmentResultByPatmentSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;

                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;

            return result;
        }


        result.PatmentResult = backendDB.ConfirmManualPayment(fromBody.PaymentSerial, fromBody.ProcessStatus, AdminData.AdminID, fromBody.ProviderCode, fromBody.ServiceType, fromBody.GroupID);
        if (result.PatmentResult.Status >= 0)
        {

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工充值审核,单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpPost]
    [ActionName("ConfirmManualProviderPayment")]
    public UpdatePatmentResultByPatmentSerialResult ConfirmManualProviderPayment([FromBody] FromBody.PaymentSet fromBody)
    {
        UpdatePatmentResultByPatmentSerialResult result = new UpdatePatmentResultByPatmentSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBViewModel.AdminWithKey AdminModel = new DBViewModel.AdminWithKey();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();

        AdminModel = backendDB.GetAdminByLoginAdminIDWithKey(AdminData.AdminID);

        if (fromBody.ProcessStatus == 3)
        {
            if (string.IsNullOrEmpty(AdminModel.GoogleKey))
            {
                result.Message = "尚未绑定 Google 验证器";
                result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
                return result;

            }
            else
            {
                //檢查google認證
                BackendFunction backendFunction = new BackendFunction();
                if (!backendFunction.CheckGoogleKey(AdminModel.GoogleKey, fromBody.UserKey))
                {
                    result.Message = " Google 验证有误";
                    result.ResultCode = APIResult.enumResult.GoogleKeyError;
                    return result;
                }
            }
        }

        result.PatmentResult = backendDB.ConfirmManualProviderPayment(fromBody.PaymentSerial, fromBody.ProcessStatus, fromBody.PatchDescription, AdminData.AdminID);
        if (result.PatmentResult.Status >= 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "上游供应商审核,单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpPost]
    [ActionName("ConfirmModifyBankCradByPayment")]
    public APIResult ConfirmModifyBankCradByPayment([FromBody] FromBody.PaymentSet fromBody)
    {
        UpdatePatmentResultByPatmentSerialResult result = new UpdatePatmentResultByPatmentSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();

        if (backendDB.ConfirmModifyBankCradByPayment(fromBody.PaymentSerial, fromBody.PatchDescription) > 0)
        {
            result.ResultCode = APIResult.enumResult.OK;
            result.PatmentResult = new DBViewModel.UpdatePatmentResult();
            result.PatmentResult.PaymentData = backendDB.GetProxyProviderPaymentReportByPaymentSerial(fromBody.PaymentSerial);
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("ReSendPayment")]
    public APIResult ReSendPayment([FromBody] FromBody.PaymentTable fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var DbReturn = backendDB.ReSendPayment(fromBody.PaymentSerial, AdminData.forCompanyID);

        if (DbReturn != null)
        {
            switch (DbReturn.Status)
            {
                case BackendDB.ResultStatus.OK:
                    retValue.ResultCode = APIResult.enumResult.OK;
                    break;
                default:
                    retValue.ResultCode = APIResult.enumResult.Error;
                    retValue.Message = DbReturn.Message;
                    break;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "網路錯誤";
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("PaymentRecord")]
    public PaymentTransferLogResult PaymentRecord([FromBody] FromBody.PaymentTable fromBody)
    {
        PaymentTransferLogResult retValue = new PaymentTransferLogResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var DbReturn = backendDB.PaymentRecord(fromBody.PaymentSerial);

        if (DbReturn != null)
        {
            retValue.PaymentTableResults = DbReturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("ReSendWithdrawal")]
    public APIResult ReSendWithdrawal([FromBody] FromBody.WithdrawalPostSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBModel.Withdrawal WithdrawData = backendDB.GetWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);

        if (WithdrawData.DownUrl == "https://www.baidu.com/" || WithdrawData.DownUrl == "http://baidu.com")
        {

            retValue.ResultCode = APIResult.enumResult.OK;
            return retValue;
        }

        backendDB.ReSendWithdrawal(fromBody.WithdrawSerial, fromBody.isReSendWithdraw);
        retValue.ResultCode = APIResult.enumResult.OK;

        return retValue;
    }
    #endregion

    #region 結帳
    [HttpPost]
    [ActionName("GetAgentReceiveTableResult")]
    public AgentReceiveTableResult GetAgentReceiveTableResult(FromBody.AgentReceiveSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AgentReceiveTableResult Result = new AgentReceiveTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.AgentReceive> _AgentReceive = backendDB.GetAgentReceiveTableResult(fromBody);

        if (_AgentReceive != null)
        {
            Result.AgentReceiveTableResults = _AgentReceive;
            Result.AgentAmountResult = backendDB.GetAgentAmountResult(fromBody);
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetAgentCloseTableResult")]
    public AgentCloseTableResult GetAgentCloseTableResult(FromBody.AgentReceiveSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AgentCloseTableResult Result = new AgentCloseTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.AgentClose> _AgentReceive = backendDB.GetAgentCloseTableResult(fromBody);

        if (_AgentReceive != null)
        {
            Result.AgentCloseTableResults = _AgentReceive;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("SetAgentClose")]
    public APIResult SetAgentClose([FromBody] FromBody.GetBankDataSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        APIResult Result = new APIResult();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        BackendDB backendDB = new BackendDB();
        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "代理返佣结算", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        Result.Message = backendDB.SetAgentClose(AdminData.forCompanyID, fromBody.CurrencyType).ToString();
        //--0 = success
        //--1 = 處理筆數為0
        //--2 = 鎖定失敗
        //--3 = 加扣點失敗
        //--4 = 其他錯誤

        switch (Result.Message)
        {
            case "0":
                Result.ResultCode = APIResult.enumResult.OK;
                Result.Message = "结算完成";
                break;
            case "-1":
                Result.ResultCode = APIResult.enumResult.OK;
                Result.Message = "处理笔数为0";
                break;
            case "-2":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "锁定失败";
                break;
            case "-3":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "加扣點失敗";
                break;
            case "-4":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "其他错误";
                break;
            default:
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "其他错误";
                break;
        }

        return Result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("SetAgentCloseByAdmin")]
    public APIResult SetAgentCloseByAdmin([FromBody] FromBody.Company fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        APIResult Result = new APIResult();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        BackendFunction backendFunction = new BackendFunction();
        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "代理返佣结算(系统商操作)", IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        Result.Message = backendDB.SetAgentClose(fromBody.CompanyID, fromBody.CurrencyType).ToString();

        //--0 = success
        //--1 = 處理筆數為0
        //--2 = 鎖定失敗
        //--3 = 加扣點失敗
        //--4 = 其他錯誤

        switch (Result.Message)
        {
            case "0":
                Result.ResultCode = APIResult.enumResult.OK;
                Result.Message = "结算完成";
                break;
            case "-1":
                Result.ResultCode = APIResult.enumResult.OK;
                Result.Message = "处理笔数为0";
                break;
            case "-2":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "锁定失败";
                break;
            case "-3":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "加扣點失敗";
                break;
            case "-4":
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "其他错误";
                break;
            default:
                Result.ResultCode = APIResult.enumResult.Error;
                Result.Message = "其他错误";
                break;
        }

        return Result;
    }
    #endregion

    #region 公司渠道設定

    [HttpPost]
    [ActionName("GetGPayRelationResult")]
    public GPayRelationResult2 GetGPayRelationResult([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        GPayRelationResult2 _Result = new GPayRelationResult2();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _Result.ResultCode = APIResult.enumResult.SessionError;
            return _Result;
        }
        BackendDB backendDB = new BackendDB();

        var TableResult = backendDB.GetGPayRelationResult(fromBody.ServiceType, fromBody.CurrencyType, "", fromBody.CompanyID);

        if (TableResult != null)
        {

            _Result.GPayRelations = TableResult;
            _Result.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _Result.ResultCode = APIResult.enumResult.NoData;
        }

        return _Result;
    }

    [HttpPost]
    [ActionName("GetCompanyServiceTableResult")]
    public CompanyServiceTableResult GetCompanyServiceTableResult([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        CompanyServiceTableResult _CompanyServiceTableResult = new CompanyServiceTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServiceTableResult;
        }
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.CompanyServiceTableResult> companyServiceTableResult = backendDB.GetCompanyServiceTableByCompanyID(fromBody.CompanyID);
        if (companyServiceTableResult != null)
        {
            foreach (var item in companyServiceTableResult)
            {
                item.GPayRelations = backendDB.GetGPayRelationResult(item.ServiceType, item.CurrencyType, "", item.forCompanyID);
            }
            _CompanyServiceTableResult.CompanyServiceResults = companyServiceTableResult;
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _CompanyServiceTableResult;
    }

    [HttpPost]
    [ActionName("InsertCompanyServiceByEditView")]
    public APIResult InsertCompanyServiceByEditView([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";
        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.InsertCompanyServiceByEditView(fromBody, AdminData.CompanyType);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else if (DBretValue == -2)
        {
            retValue.ResultCode = APIResult.enumResult.UplineMaxDaliyAmountByUseError;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            #region 记录log

            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("新增商户支付方式,名称:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5}",
            ServiceTypeName, fromBody.CurrencyType, fromBody.CollectRate, fromBody.MaxDaliyAmount, fromBody.MinOnceAmount, fromBody.MaxOnceAmount
            ), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            #endregion
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateCompanyServiceByEditView")]
    public APIResult UpdateCompanyServiceByEditView([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";
        string fingerprint = GetFingerprint();

        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }



        DBretValue = backendDB.UpdateCompanyServiceByEditView(fromBody);

        if (DBretValue == -2)
        {
            retValue.ResultCode = APIResult.enumResult.UplineMaxDaliyAmountByUseError;
        }
        else if (DBretValue == -3)
        {
            retValue.ResultCode = APIResult.enumResult.MaxDaliyAmountByUseError;
        }
        else if (DBretValue == -4)
        {
            retValue.ResultCode = APIResult.enumResult.OfflineMaxDaliyAmountByUseError;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            #region 记录log

            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("修改商户支付方式,名称:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5}",
            ServiceTypeName, fromBody.CurrencyType, fromBody.CollectRate, fromBody.MaxDaliyAmount, fromBody.MinOnceAmount, fromBody.MaxOnceAmount
            ), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            #endregion
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateCompanyServiceWeightByEditView")]
    public APIResult UpdateCompanyServiceWeightByEditView([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";

        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.UpdateCompanyServiceWeightByEditView(fromBody);

        if (DBretValue > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("SetCompanyServiceRelationByEditView")]
    public APIResult SetCompanyServiceRelationByEditView([FromBody] FromBody.GPayRelationSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (fromBody.isAddRelation)
        {

            var CompanyServiceModel = backendDB.GetCompanyService(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);

            if (CompanyServiceModel == null)
            {
                retValue.ResultCode = APIResult.enumResult.Error;
                retValue.Message = "尚未设定商户支付通道";
                return retValue;
            }
            var ProviderServiceModel = backendDB.GetProviderServiceByProviderCodeAndServiceType(fromBody.ProviderCode, fromBody.ServiceType, fromBody.CurrencyType);
            if (ProviderServiceModel == null)
            {
                retValue.ResultCode = APIResult.enumResult.Error;
                retValue.Message = "尚未设定供应商支付通道";
                return retValue;
            }

            if (!((ProviderServiceModel.MinOnceAmount <= CompanyServiceModel.MinOnceAmount || CompanyServiceModel.MinOnceAmount <= ProviderServiceModel.MaxOnceAmount) || (ProviderServiceModel.MinOnceAmount <= CompanyServiceModel.MaxOnceAmount || CompanyServiceModel.MaxOnceAmount <= ProviderServiceModel.MaxOnceAmount)))
            {
                retValue.ResultCode = APIResult.enumResult.Error;
                retValue.Message = "上下限额不在供应商区间";
                return retValue;
            }

            if (CompanyServiceModel.CollectRate < ProviderServiceModel.CostRate)
            {
                retValue.ResultCode = APIResult.enumResult.Error;
                retValue.Message = "商户费率小于供应商费率";
                return retValue;
            }

        }

        DBretValue = backendDB.SetCompanyServiceRelationByEditView(fromBody);

        if (DBretValue > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.CompanyCodeNotExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetCompanyServiceRelationByEditView")]
    public CompanyCompanyServiceRelationResult GetCompanyServiceRelationByEditView([FromBody] FromBody.ProviderService fromBody)
    {
        CompanyCompanyServiceRelationResult _CompanyServiceTableResult = new CompanyCompanyServiceRelationResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServiceTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyServiceTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBViewModel.CompanyServiceRelation> companyServiceTableResult = backendDB.GetCompanyServiceRelationByEditView(fromBody.ServiceType, fromBody.ProviderCode, fromBody.CurrencyType);
        if (companyServiceTableResult != null)
        {

            _CompanyServiceTableResult.Results = companyServiceTableResult;
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _CompanyServiceTableResult;
    }

    [HttpPost]
    [ActionName("InsertCompanyService")]
    public APIResult InsertCompanyService([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";
        string ProviderName = "";
        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.InsertCompanyService(fromBody, AdminData.CompanyType);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else if (DBretValue == -2)
        {
            retValue.ResultCode = APIResult.enumResult.UplineMaxDaliyAmountByUseError;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            #region 记录log
            if (fromBody.ProviderCodeAndWeight != null && fromBody.ProviderCodeAndWeight.Count > 0)
            {
                for (int i = 0; i < fromBody.ProviderCodeAndWeight.Count; i++)
                {
                    ProviderCodes.Add(fromBody.ProviderCodeAndWeight[i].ProviderCode);
                }

                ProviderName = backendDB.GetProviderNameByListProviderCode(ProviderCodes);
            }

            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            string CompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("新增商户支付方式,名称:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5},对应供应商:{6},商户:{7}",
            ServiceTypeName, fromBody.CurrencyType, fromBody.CollectRate, fromBody.MaxDaliyAmount, fromBody.MinOnceAmount, fromBody.MaxOnceAmount, ProviderName, CompanyName
            ), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            #endregion
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateCompanyService")]
    public APIResult UpdateCompanyService([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";
        string ProviderName = "";
        List<string> ProviderCodes = new List<string>();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.UpdateCompanyService(fromBody, AdminData.CompanyType);

        if (DBretValue == -2)
        {
            retValue.ResultCode = APIResult.enumResult.UplineMaxDaliyAmountByUseError;
        }
        else if (DBretValue == -3)
        {
            retValue.ResultCode = APIResult.enumResult.MaxDaliyAmountByUseError;
        }
        else if (DBretValue == -4)
        {
            retValue.ResultCode = APIResult.enumResult.OfflineMaxDaliyAmountByUseError;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            #region 记录log
            if (fromBody.ProviderCodeAndWeight != null && fromBody.ProviderCodeAndWeight.Count > 0)
            {
                for (int i = 0; i < fromBody.ProviderCodeAndWeight.Count; i++)
                {
                    ProviderCodes.Add(fromBody.ProviderCodeAndWeight[i].ProviderCode);
                }

                ProviderName = backendDB.GetProviderNameByListProviderCode(ProviderCodes);
            }

            string strState = "取得失败";
            if (fromBody.State == 0)
            {
                strState = "启用";
            }
            else if (fromBody.State == 1)
            {
                strState = "停用";
            }

            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            string CompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, string.Format("修改商户支付方式,名称:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5},对应供应商:{6},状态:{7},商户:{8}",
            ServiceTypeName, fromBody.CurrencyType, fromBody.CollectRate, fromBody.MaxDaliyAmount, fromBody.MinOnceAmount, fromBody.MaxOnceAmount, ProviderName, strState, CompanyName
            ), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            #endregion
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableCompanyService")]
    public APIResult DisableCompanyService([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string ServiceTypeName = "";
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        int BeforeState = backendDB.GetCompanyServiceState(fromBody.ServiceType, fromBody.CurrencyType, fromBody.CompanyID);
        string StrBeforeState = "取得失败";
        if (BeforeState == 0)
        {
            StrBeforeState = "停用";
        }
        else if (BeforeState == 1)
        {
            StrBeforeState = "启用";
        }


        DBretValue = backendDB.DisableCompanyService(fromBody);

        if (DBretValue >= 1)
        {
            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            string CompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "(停用/启用)商户支付方式:" + ServiceTypeName + ",更改为:" + StrBeforeState + ",商户:" + CompanyName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }
    #endregion

    #region Currency
    [HttpGet]
    [HttpPost]
    [ActionName("GetCurrency")]
    public CurrencyResult GetCurrency([FromBody] string BID)
    {
        CurrencyResult retValue = new CurrencyResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.Currencies = backendDB.GetCurrency();
        retValue.ResultCode = APIResult.enumResult.OK;

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCurrencyByCompanyID")]
    public CurrencyResult GetCurrencyByCompanyID([FromBody] string BID)
    {
        CurrencyResult retValue = new CurrencyResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType == 0)
        {
            //帶0代表目前為最高權限,抓取系統所有幣別
            retValue.Currencies = backendDB.GetCurrencyByCompanyID(0);
        }
        else
        {
            //抓取目前登入公司幣別
            retValue.Currencies = backendDB.GetCurrencyByCompanyID(AdminData.forCompanyID);
        }


        retValue.ResultCode = APIResult.enumResult.OK;

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("InsertCurrency")]
    public APIResult InsertCurrency([FromBody] FromBody.CurrencyPostSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        var currencyModel = new DBModel.Currency() { CurrencyType = fromBody.Currency };
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        int DBretValue;
        DBretValue = backendDB.InsertCurrency(currencyModel);
        if (DBretValue > 0)
        {

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "新增币别:" + fromBody.Currency, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region ServiceType
    [HttpPost]
    [ActionName("DeleteProviderService")]
    public APIResult DeleteProviderService([FromBody] FromBody.DeleteProviderService fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string ProviderName = "";
        string ServiceTypeName = "";
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.DeleteProviderService(fromBody);

        if (DBretValue >= 1)
        {
            ProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "删除供应商支付类型,供应商名称:" + ProviderName + ",支付类型:" + ServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }


    [HttpGet]
    [HttpPost]
    [ActionName("GetServiceType")]
    public ServiceTypeResult GetServiceType([FromBody] FromBody.Company fromBody)
    {
        ServiceTypeResult retValue = new ServiceTypeResult();
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.ServiceTypeVM> DBretValue = new List<DBViewModel.ServiceTypeVM>();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        var CompanyTable = backendDB.GetCompanyByID(fromBody.CompanyID);
        if (CompanyTable != null)
        {
            if (AdminData.CompanyType == 0)
            {
                if (CompanyTable.ParentCompanyID == 0)
                {
                    //如果目前選擇公司為最上線公司且目前登入者為最高權限
                    DBretValue = backendDB.GetServiceType(0, 0);
                }
                else
                {
                    //抓取目前選擇公司已設定服務
                    DBretValue = backendDB.GetServiceType(CompanyTable.ParentCompanyID, CompanyTable.InsideLevel);
                }
            }
            else
            {
                //如果目前設定營運商為最上線
                if (CompanyTable.ParentCompanyID == 0)
                {
                    //抓取目前選擇公司已設定服務
                    DBretValue = backendDB.GetServiceType(CompanyTable.CompanyID, CompanyTable.InsideLevel + 1);
                }
                else
                {
                    //抓取目前選擇公司的上線已設定服務
                    DBretValue = backendDB.GetServiceType(CompanyTable.ParentCompanyID, CompanyTable.InsideLevel);
                }

            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }


        if (DBretValue != null)
        {
            retValue.ServiceTypes = DBretValue;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetServiceTypeAndProvicerCode")]
    public ProviderServiceTypeResult GetServiceTypeAndProvicerCode([FromBody] string BID)
    {
        ProviderServiceTypeResult retValue = new ProviderServiceTypeResult();
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.ServiceTypeVM> DBretValue = new List<DBViewModel.ServiceTypeVM>();
        List<DBModel.Provider> ProviderTypDBretValue = new List<DBModel.Provider>();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        retValue.ServiceTypes = backendDB.GetServiceType(0, 0);
        retValue.ProviderTypes = backendDB.GetProviderCodeResultByShowType();
        if (retValue.ProviderTypes != null && retValue.ServiceTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetSelectedCompanyService")]
    public SelectedCompanyService GetSelectedCompanyService([FromBody] FromBody.CompanyService fromBody)
    {

        SelectedCompanyService retValue = new SelectedCompanyService();
        BackendDB backendDB = new BackendDB();
        DBModel.CompanyService DBretValue = new DBModel.CompanyService();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        DBretValue = backendDB.GetSelectedCompanyService(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);

        if (DBretValue != null)
        {
            retValue.CompanyServiceResult = DBretValue;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("InsertServiceType")]
    public APIResult InsertServiceType([FromBody] FromBody.InsertServiceType fromBody)
    {

        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        var ServiceTypeModel = new DBModel.ServiceTypeModel()
        {
            ServiceType = fromBody.ServiceType,
            CurrencyType = fromBody.CurrencyType,
            AllowCollect = fromBody.AllowCollect,
            AllowPay = fromBody.AllowPay,
            ServiceTypeName = string.IsNullOrEmpty(fromBody.ServiceTypeName) ? "" : fromBody.ServiceTypeName,
            ServicePaymentType = fromBody.ServicePaymentType,
            ServiceSupplyType = fromBody.ServiceSupplyType
        };

        if (backendDB.InsertServiceType(ServiceTypeModel) > 0)
        {

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "新增支付类型:" + fromBody.ServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("UpdateServiceType")]
    public APIResult UpdateServiceType([FromBody] FromBody.InsertServiceType fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        var ServiceTypeModel = new DBModel.ServiceTypeModel()
        {
            ServiceType = fromBody.ServiceType,
            CurrencyType = fromBody.CurrencyType,
            AllowCollect = fromBody.AllowCollect,
            AllowPay = fromBody.AllowPay,
            ServiceTypeName = string.IsNullOrEmpty(fromBody.ServiceTypeName) ? "" : fromBody.ServiceTypeName,
            ServicePaymentType = fromBody.ServicePaymentType,
            ServiceSupplyType = fromBody.ServiceSupplyType
        };

        if (backendDB.UpdateServiceType(ServiceTypeModel) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "修改支付类型:" + fromBody.ServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region ProviderCode
    [HttpPost]
    [ActionName("GetProviderByServiceType")]
    public ProviderCodeResult GetProviderByServiceType([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        ProviderCodeResult retValue = new ProviderCodeResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.ProviderTypes = backendDB.GetProviderByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
        if (retValue.ProviderTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderCodeResult")]
    public ProviderCodeResult GetProviderCodeResult([FromBody] string BID)
    {
        ProviderCodeResult retValue = new ProviderCodeResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        if (AdminData.CompanyType == 4)
        {
            retValue.ProviderTypes = backendDB.GetProviderCodeResultByCompanyID(AdminData.forCompanyID, AdminData.CompanyType, "");
        }
        else
        {
            retValue.ProviderTypes = backendDB.GetProviderCodeResult();
        }


        if (retValue.ProviderTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderCodeResultByShowType")]
    public ProviderCodeResult GetProviderCodeResultByShowType([FromBody] string BID)
    {
        ProviderCodeResult retValue = new ProviderCodeResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        if (AdminData.CompanyType == 4)
        {
            retValue.ProviderTypes = backendDB.GetProviderCodeResultByCompanyID(AdminData.forCompanyID);
        }
        else
        {
            retValue.ProviderTypes = backendDB.GetProviderCodeResultByShowType();
        }


        if (retValue.ProviderTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProxyProviderData")]
    public ProxyProviderData GetProxyProviderData([FromBody] string BID)
    {
        ProxyProviderData retValue = new ProxyProviderData();
        BackendDB backendDB = new BackendDB();
        DBModel.Admin AdminModel;
        int GroupID = 0;
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }

        retValue.ProxyProvider = backendDB.GetProxyProviderByProviderCode(AdminData.CompanyCode);
        retValue.ProxyProviderGroup = backendDB.GetProxyProviderGroupByGroupID(AdminData.CompanyCode, GroupID);
        retValue.FrozenPoint = backendDB.GetProxyProviderGroupFrozenPoint(AdminData.CompanyCode, GroupID);
        if (retValue.ProxyProvider != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    //[HttpPost]
    //[ActionName("ChangeProviderGroup")]
    //public APIResult ChangeProviderGroup([FromBody] FromBody.ProxyProviderGroupSet fromBody) {
    //    APIResult result = new APIResult();
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
    //    DBModel.Admin AdminModel;
    //    string OrderGroupName = "";

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID)) {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else {
    //        AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    }

    //    if (AdminData.CompanyType != 3) {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }


    //    BackendDB backendDB = new BackendDB();

    //    AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
    //    int GroupID = 0;

    //    if (AdminModel != null) {
    //        GroupID = AdminModel.GroupID;
    //    }


    //    if (backendDB.CheckHandleByChangeGroup(fromBody.OrderSerial, GroupID) > 0) {
    //        OrderGroupName = backendDB.GetProxyProviderGroupNameByOrderSerial(fromBody.OrderSerial, 1);
    //        if (backendDB.UpdateProxyProviderOrderGroup(fromBody.OrderSerial, fromBody.GroupID, GroupID) > 0) {
    //            BackendFunction backendFunction = new BackendFunction();
    //            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //            string GroupName = backendDB.GetProxyProviderGroupNameByGroupID(fromBody.GroupID);
    //            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "专属供应商修改订单群组("+ OrderGroupName + "):" + fromBody.OrderSerial + ",更换至:" + GroupName, IP, fingerprint);
    //            string XForwardIP = CodingControl.GetXForwardedFor();
    //            CodingControl.WriteXFowardForIP(AdminOP);
    //            result.ResultCode = APIResult.enumResult.OK;

    //            if (GroupID == 0 || GroupID != fromBody.GroupID) {
    //                result.Message = "ChangeToOtherGroup";
    //            }

    //        }
    //        else {
    //            var WithdrawSerialModel = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.OrderSerial);
    //            if (WithdrawSerialModel.GroupID != GroupID)
    //            {   //群组已转移
    //                result.ResultCode = APIResult.enumResult.DataExist;
    //            }
    //            else if (WithdrawSerialModel.HandleByAdminID != 0)
    //            {    //已有人审核中
    //                result.ResultCode = APIResult.enumResult.DataDuplicate;
    //                result.Message = WithdrawSerialModel.RealName1;
    //            }
    //            else if (WithdrawSerialModel.Status != 1)
    //            {   //审核已完成
    //                result.ResultCode = APIResult.enumResult.Other;
    //            }
    //            else
    //            {
    //                result.ResultCode = APIResult.enumResult.Error;
    //            }
    //        }
    //    }
    //    else {
    //        var WithdrawSerialModel = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.OrderSerial);
    //        if (WithdrawSerialModel.GroupID != GroupID) {   //群组已转移
    //            result.ResultCode = APIResult.enumResult.DataExist;
    //        }
    //        else if (WithdrawSerialModel.HandleByAdminID != 0) {    //已有人审核中
    //            result.ResultCode = APIResult.enumResult.DataDuplicate;
    //            result.Message = WithdrawSerialModel.RealName1;
    //        }
    //        else if (WithdrawSerialModel.Status != 1) {   //审核已完成
    //            result.ResultCode = APIResult.enumResult.Other;
    //        }
    //        else {
    //            result.ResultCode = APIResult.enumResult.Error;
    //        }
    //    }
    //    return result;
    //}


    [HttpPost]
    [ActionName("ChangeProviderGroupByAdmin")]
    public UpdateWithdrawalTableResult ChangeProviderGroupByAdmin([FromBody] FromBody.ProxyProviderGroupSet fromBody)
    {
        UpdateWithdrawalTableResult result = new UpdateWithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string OrderGroupName = "";
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        if (backendDB.CheckHandleByChangeGroupByAdmin(fromBody.OrderSerial) > 0)
        {
            OrderGroupName = backendDB.GetProxyProviderGroupNameByOrderSerial(fromBody.OrderSerial, 1);
            if (backendDB.spUpdateProxyProviderOrderGroupByAdmin(fromBody.OrderSerial, fromBody.GroupID) == 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                string GroupName = backendDB.GetProxyProviderGroupNameByGroupID(fromBody.GroupID);
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "系統商修改订单群组(" + OrderGroupName + "):" + fromBody.OrderSerial + ",更换至:" + GroupName, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                var WithdrawSerialModel = backendDB.GetWithdrawalByWithdrawSerialByAdmin(fromBody.OrderSerial);
                result.WithdrawalResult = WithdrawSerialModel;
                result.ResultCode = APIResult.enumResult.OK;
                result.Message = "ChangeToOtherGroup";

            }
            else
            {
                var WithdrawSerialModel = backendDB.GetWithdrawalByWithdrawSerialByAdmin(fromBody.OrderSerial);
                result.WithdrawalResult = WithdrawSerialModel;
                if (WithdrawSerialModel.HandleByAdminID != 0)
                {    //已有人审核中
                    result.ResultCode = APIResult.enumResult.DataDuplicate;
                    result.Message = WithdrawSerialModel.RealName1;
                }
                else if (WithdrawSerialModel.Status != 1)
                {   //审核已完成
                    result.ResultCode = APIResult.enumResult.Other;
                }
                else
                {
                    result.ResultCode = APIResult.enumResult.Error;
                }
            }
        }
        else
        {
            var WithdrawSerialModel = backendDB.GetWithdrawalByWithdrawSerialByAdmin(fromBody.OrderSerial);
            result.WithdrawalResult = WithdrawSerialModel;
            if (WithdrawSerialModel.HandleByAdminID != 0)
            {    //已有人审核中
                result.ResultCode = APIResult.enumResult.DataDuplicate;
                result.Message = WithdrawSerialModel.RealName1;
            }
            else if (WithdrawSerialModel.Status != 1)
            {   //审核已完成
                result.ResultCode = APIResult.enumResult.Other;
            }
            else
            {
                result.ResultCode = APIResult.enumResult.Error;
            }
        }
        return result;
    }

    [HttpPost]
    [ActionName("ChangeProviderGroupWithdrawalsByAdmin")]
    public ChangeProviderGroupWithdrawalsByAdminReturn ChangeProviderGroupWithdrawalsByAdmin([FromBody] FromBody.ChangeProviderGroupOrdersByAdmin fromBody)
    {
        ChangeProviderGroupWithdrawalsByAdminReturn result = new ChangeProviderGroupWithdrawalsByAdminReturn();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        BackendFunction backendFunction = new BackendFunction();
        List<string> UpdateWithdrawSerials = new List<string>();
        string OrderGroupName = "";
        string IP = "";
        int AdminOP = 0;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }
        if (fromBody.Withdrawals.Count > 0)
        {
            for (int i = 0; i < fromBody.Withdrawals.Count; i++)
            {

                if (backendDB.CheckHandleByChangeGroupByAdmin(fromBody.Withdrawals[i]) > 0)
                {
                    OrderGroupName = backendDB.GetProxyProviderGroupNameByOrderSerial(fromBody.Withdrawals[i], 1);
                    if (backendDB.spUpdateProxyProviderOrderGroupByAdmin(fromBody.Withdrawals[i], fromBody.GroupID) == 0)
                    {

                        IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                        string GroupName = backendDB.GetProxyProviderGroupNameByGroupID(fromBody.GroupID);
                        AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "系統商修改订单群组(" + OrderGroupName + "):" + fromBody.Withdrawals[i] + ",更换至:" + GroupName, IP, fingerprint);

                        CodingControl.WriteXFowardForIP(AdminOP);
                        result.SuccessCount++;

                    }
                    else
                    {
                        result.FailCount++;
                    }
                }
                else
                {
                    result.FailCount++;
                }
            }
            IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());

            string strWithdrawals = "";
            for (int i = 0; i < fromBody.Withdrawals.Count(); i++)
            {
                strWithdrawals += fromBody.Withdrawals[i] + ",";
            }

            strWithdrawals = strWithdrawals.Substring(0, strWithdrawals.Length - 1);

            AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "系統商修改订单群组:" + strWithdrawals, IP, fingerprint);
            CodingControl.WriteXFowardForIP(AdminOP);

            result.ResultCode = APIResult.enumResult.OK;

            result.Withdrawals = backendDB.GetWithdrawalByWithdrawSerialsByAdmin(fromBody.Withdrawals);
        }
        else
        {
            result.ResultCode = APIResult.enumResult.NoData;
        }

        return result;
    }

    [HttpPost]
    [ActionName("UpdateProviderBankCard")]
    public APIResult UpdateProviderBankCard([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult result = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        var returnDB = backendDB.UpdateProviderBankCard(fromBody);
        if (returnDB == 1)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "調整供应商銀行卡:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
            result.Message = "其他错误";
        }

        return result;
    }

    [HttpPost]
    [ActionName("DeleteProviderBankCard")]
    public APIResult DeleteProviderBankCard([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult result = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        var returnDB = backendDB.DeleteProviderBankCard(fromBody);
        if (returnDB == 1)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "刪除供应商銀行卡:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
            result.Message = "其他错误";
        }

        return result;
    }

    [HttpPost]
    [ActionName("InsertProviderBankCard")]
    public APIResult InsertProviderBankCard([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        if (backendDB.InsertProviderBankCard(fromBody) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商銀行卡:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("UpdateProxyProviderPoint2")]
    public APIResult UpdateProxyProviderPoint2([FromBody] FromBody.PaymentSet fromBody)
    {
        APIResult result = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }


        var returnDB = backendDB.UpdateProxyProviderPoint2(fromBody.PaymentSerial);
        if (returnDB == 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "专属供应商余额调整,金额:" + fromBody.Amount + ",群组id:" + fromBody.GroupID, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
            switch (returnDB)
            {
                case -1:
                    result.Message = "群组不存在";
                    break;
                case -2:
                    result.Message = "此订单非专属供应商订单";
                    break;
                case -3:
                    result.Message = "供应商状态有误";
                    break;
                case -4:
                    result.Message = "鎖定失敗";
                    break;
                case -5:
                    result.Message = "订单不存在";
                    break;
                default:
                    result.Message = "其他错误";
                    break;
            }
        }

        return result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderCodeResultByProxyProvider")]
    public ProxyProviderResult GetProviderCodeResultByProxyProvider([FromBody] string BID)
    {
        ProxyProviderResult retValue = new ProxyProviderResult();
        BackendDB backendDB = new BackendDB();


        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.ProviderTypes = backendDB.getProviderCodeResultByProxyProvider();
        if (retValue.ProviderTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderCodeResultByProviderAPIType")]
    public ProviderCodeResult GetProviderCodeResultByProviderAPIType([FromBody] FromBody.UpdateProviderAPIType fromBody)
    {
        ProviderCodeResult retValue = new ProviderCodeResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.ProviderTypes = backendDB.GetProviderCodeResultByProviderAPIType(fromBody.setAPIType);
        if (retValue.ProviderTypes != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    //[HttpGet]
    //[ActionName("GetProviderPoint")]
    //public ProviderPointResult GetProviderPoint(string CurrencyType)
    //{
    //    ProviderPointResult retValue = new ProviderPointResult();
    //    BackendDB backendDB = new BackendDB();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        retValue.ResultCode = APIResult.enumResult.SessionError;
    //        return retValue;
    //    }
    //    var Lst_ProviderCode = new List<string>();
    //    var Providers = backendDB.GetProviderCodeResult();

    //    if (Providers != null)
    //    {
    //        foreach (var provider in Providers)
    //        {
    //            Lst_ProviderCode.Add(provider.ProviderCode);
    //            //供應商提供功能: 0 = 無 / 1 = 代收 / 2 = 代付 / 4 = 查詢 / 8 = 補單
    //            //if (provider.ProviderAPIType == 0)
    //            //{
    //            //    continue;
    //            //}
    //            ////查詢
    //            //if ((provider.ProviderAPIType & 4) == 4)
    //            //{
    //            //    Lst_ProviderCode.Add(provider.ProviderCode);
    //            //}
    //        }
    //        if (Lst_ProviderCode.Count > 0)
    //        {
    //            retValue.ProviderBalances = backendDB.GetProviderPoint(CurrencyType, Lst_ProviderCode);
    //        }

    //        if (retValue.ProviderBalances != null)
    //        {
    //            foreach (var data in retValue.ProviderBalances)
    //            {
    //                if (data.ProviderCode != null) {
    //                    data.ProviderName = Providers.Where(w => w.ProviderCode == data.ProviderCode).FirstOrDefault().ProviderName;
    //                }
    //            }
    //            retValue.ResultCode = APIResult.enumResult.OK;
    //        }
    //        else
    //        {
    //            retValue.ResultCode = APIResult.enumResult.NoData;
    //        }
    //    }
    //    else {
    //        retValue.ResultCode = APIResult.enumResult.NoData;
    //    }

    //    return retValue;
    //}

    [HttpPost]
    [ActionName("InsertProviderCodeResult")]
    public APIResult InsertProviderCodeResult([FromBody] FromBody.Provider ProviderModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderModel.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(ProviderModel.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }



        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(ProviderModel.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        List<DBModel.Provider> providers = backendDB.GetProviderCodeResult(ProviderModel.ProviderCode);
        if (providers == null)
        {
            if (backendDB.InsertProviderCode(ProviderModel) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                string isPrePaid = "取得失败";
                if (ProviderModel.isPrePaid == 0)
                {
                    isPrePaid = "是";
                }
                else if (ProviderModel.isPrePaid == 1)
                {
                    isPrePaid = "否";
                }

                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商:" + ProviderModel.ProviderName + ",是否为预付:" + isPrePaid, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("SetProxyProviderData")]
    public APIResult SetProxyProviderData([FromBody] FromBody.ProxyProvider ProviderModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderModel.BID);

        if (backendDB.SetProxyProviderData(ProviderModel) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "设定专属供应商:" + ProviderModel.forProviderCode, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProviderCodeResult")]
    public APIResult UpdateProviderCodeResult([FromBody] FromBody.Provider ProviderModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderModel.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(ProviderModel.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType == 0)
        {

        }
        else if (AdminData.CompanyType == 4)
        {
            ProviderModel.forCompanyID = AdminData.forCompanyID;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(ProviderModel.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        List<DBModel.Provider> providers = backendDB.GetProviderCodeResult(ProviderModel.ProviderCode);
        if (providers != null)
        {
            if (backendDB.UpdateProviderCode(ProviderModel) > 0)
            {
                string strProviderAPIType = "";
                if ((ProviderModel.ProviderAPIType & 1) == 1)
                {
                    strProviderAPIType += "代收,";
                }
                if ((ProviderModel.ProviderAPIType & 2) == 2)
                {
                    strProviderAPIType += "代付,";
                }

                if ((ProviderModel.ProviderAPIType & 4) == 4)
                {
                    strProviderAPIType += "查詢餘額,";
                }
                if ((ProviderModel.ProviderAPIType & 8) == 8)
                {
                    strProviderAPIType += "查詢單,";
                }
                if ((ProviderModel.ProviderAPIType & 16) == 16)
                {
                    strProviderAPIType += "補單,";
                }

                if (strProviderAPIType != "")
                {
                    strProviderAPIType = strProviderAPIType.Substring(0, strProviderAPIType.Length - 1);
                }
                else
                {
                    strProviderAPIType = "无";
                }
                string StrBeforeProviderState = "取得失败";
                if (ProviderModel.ProviderState == 0)
                {
                    StrBeforeProviderState = "启用";
                }
                else if (ProviderModel.ProviderState == 1)
                {
                    StrBeforeProviderState = "停用";
                }

                string isPrePaid = "取得失败";
                if (ProviderModel.isPrePaid == 0)
                {
                    isPrePaid = "是";
                }
                else if (ProviderModel.isPrePaid == 1)
                {
                    isPrePaid = "否";
                }

                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "修改供应商:" + ProviderModel.ProviderName + ",状态" + StrBeforeProviderState + ",提供功能:" + strProviderAPIType + ",是否为预付:" + isPrePaid, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangeProviderCodeState")]
    public APIResult ChangeProviderCodeState([FromBody] FromBody.Provider ProviderModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderModel.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(ProviderModel.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }


        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(ProviderModel.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        int BeforeProviderState = backendDB.GetProviderState(ProviderModel.ProviderCode);
        string StrBeforeProviderState = "取得失败";
        if (BeforeProviderState == 0)
        {
            StrBeforeProviderState = "停用";
        }
        else if (BeforeProviderState == 1)
        {
            StrBeforeProviderState = "启用";
        }


        if (backendDB.ChangeProviderCodeState(ProviderModel) > 0)
        {
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderModel.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "(停用/启用)供应商:" + ProviderName + ",更改为:" + StrBeforeProviderState, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangeProviderAPIType")]
    public APIResult ChangeProviderAPIType([FromBody] FromBody.UpdateProviderAPIType ProviderAPIModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderAPIModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderAPIModel.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(ProviderAPIModel.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }


        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(ProviderAPIModel.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        System.Data.DataTable DT = RedisCache.ProviderCode.GetProviderCode(ProviderAPIModel.ProviderCode);
        int setAPIType = 0;
        if (((int)DT.Rows[0]["ProviderAPIType"] & ProviderAPIModel.setAPIType) == ProviderAPIModel.setAPIType)
        {
            setAPIType = 0 - ProviderAPIModel.setAPIType;
        }
        else
        {
            setAPIType = ProviderAPIModel.setAPIType;
        }


        if (backendDB.ChangeProviderAPIType(ProviderAPIModel, setAPIType) > 0)
        {
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderAPIModel.ProviderCode);
            string APITypeName = "";
            string APITypeState = "";
            if (ProviderAPIModel.setAPIType == 1)
            {
                APITypeName = "充值";
            }
            else
            {
                APITypeName = "代付";
            }

            if (setAPIType > 0)
            {
                APITypeState = "启用";
            }
            else
            {
                APITypeState = "停用";
            }

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "(停用/启用)供应商功能,供应商名称" + ProviderName + ",类型:" + APITypeName + ",更改为:" + APITypeState, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region ProviderService
    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderServiceResult")]
    public ProviderServiceResult GetProviderServiceResult([FromBody] FromBody.ProviderService fromBody)
    {
        ProviderServiceResult retValue = new ProviderServiceResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0))
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.ProviderServices = backendDB.GetProviderServiceResultByProviderCode(fromBody.ProviderCode);
        if (retValue.ProviderServices != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertProviderServiceResult")]
    public APIResult InsertProviderServiceResult([FromBody] FromBody.ProviderService ProviderServiceModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(ProviderServiceModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderServiceModel.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        List<DBViewModel.ProviderServiceVM> providers = backendDB.GetProviderServiceResult(ProviderServiceModel.ProviderCode, ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
        if (providers == null)
        {
            var ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderServiceModel.ProviderCode);
            if (backendDB.InsertProviderService(ProviderServiceModel) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, string.Format("新增供应商支付方式,支付类型:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5},供应商:{6}",
                ServiceTypeName, ProviderServiceModel.CurrencyType, ProviderServiceModel.CostRate, ProviderServiceModel.MaxDaliyAmount, ProviderServiceModel.MinOnceAmount, ProviderServiceModel.MaxOnceAmount, ProviderName
                ), IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProviderServiceResult")]
    public APIResult UpdateProviderServiceResult([FromBody] FromBody.ProviderService ProviderServiceModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(ProviderServiceModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderServiceModel.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 4))
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }



        List<DBViewModel.ProviderServiceVM> providers = backendDB.GetProviderServiceResult(ProviderServiceModel.ProviderCode, ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
        if (providers != null)
        {
            if (backendDB.UpdateProviderService(ProviderServiceModel, AdminData.RealName, AdminData.forCompanyID) > 0)
            {
                string StrBeforeProviderState = "取得失败";
                if (ProviderServiceModel.State == 0)
                {
                    StrBeforeProviderState = "启用";
                }
                else if (ProviderServiceModel.State == 1)
                {
                    StrBeforeProviderState = "停用";
                }

                var ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
                var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderServiceModel.ProviderCode);
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, string.Format("修改供应商支付方式,支付类型:{0},币别:{1},费率:{2},每日用量:{3},最小值:{4},最大值:{5},供应商:{6},状态:{7}",
                ServiceTypeName, ProviderServiceModel.CurrencyType, ProviderServiceModel.CostRate, ProviderServiceModel.MaxDaliyAmount, ProviderServiceModel.MinOnceAmount, ProviderServiceModel.MaxOnceAmount, ProviderName
                , StrBeforeProviderState), IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("DisableProvider")]
    public APIResult DisableProvider([FromBody] FromBody.ProviderService ProviderServiceModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(ProviderServiceModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderServiceModel.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 4))
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        List<DBModel.Provider> data = backendDB.GetProviderCodeResult(ProviderServiceModel.ProviderCode);

        if (data[0].ProviderState == 1 && data[0].CollectType == 1)
        {
            if (!backendDB.CheckProxyProviderState(data[0].ProviderCode))
            {
                retValue.ResultCode = APIResult.enumResult.DataDuplicate;
                retValue.Message = "目前已有启用其他专属供应商";
                return retValue;
            }
        }

        int BeforeProviderState = backendDB.GetProviderState(ProviderServiceModel.ProviderCode);
        string StrBeforeProviderState = "取得失败";
        if (BeforeProviderState == 0)
        {
            StrBeforeProviderState = "停用";
        }
        else if (BeforeProviderState == 1)
        {
            StrBeforeProviderState = "启用";
        }




        if (backendDB.DisableProvider(ProviderServiceModel.ProviderCode) > 0)
        {
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderServiceModel.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "(停用/启用)供应商:" + ProviderName + ",更改为:" + StrBeforeProviderState, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableProviderServiceResult")]
    public APIResult DisableProviderServiceResult([FromBody] FromBody.ProviderService ProviderServiceModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(ProviderServiceModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderServiceModel.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 4))
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        int BeforeProviderState = backendDB.GetProviderServiceState(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType, ProviderServiceModel.ProviderCode);
        string StrBeforeProviderState = "取得失败";
        if (BeforeProviderState == 0)
        {
            StrBeforeProviderState = "停用";
        }
        else if (BeforeProviderState == 1)
        {
            StrBeforeProviderState = "启用";
        }

        if (backendDB.DisableProviderService(ProviderServiceModel, AdminData.RealName, AdminData.forCompanyID) > 0)
        {
            var ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderServiceModel.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, string.Format("(停用/启用)供应商支付类型,供应商:{0},支付类型:{1},更改为:{2}", ProviderName, ServiceTypeName, StrBeforeProviderState), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdatePrePaidPoint")]
    public APIResult UpdatePrePaidPoint([FromBody] FromBody.SetPrePaidPoint Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        int DBret = backendDB.UpdatePrePaidPoint(Model.ProviderCode, Model.CurrencyType, Model.Amount);

        if (DBret == 0)
        {
            var ProviderName = backendDB.GetProviderNameByProviderCode(Model.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, string.Format("调整预付额度,供应商:{0},金額:{1}", ProviderName, Model.Amount), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ChangeProviderServiceState")]
    public APIResult ChangeProviderServiceState([FromBody] FromBody.ProviderService ProviderServiceModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(ProviderServiceModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(ProviderServiceModel.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        int BeforeProviderState = backendDB.GetProviderServiceState(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType, ProviderServiceModel.ProviderCode);
        string StrBeforeProviderState = "取得失败";
        if (BeforeProviderState == 0)
        {
            StrBeforeProviderState = "停用";
        }
        else if (BeforeProviderState == 1)
        {
            StrBeforeProviderState = "启用";
        }


        if (backendDB.ChangeProviderServiceState(ProviderServiceModel, AdminData.RealName, AdminData.forCompanyID) > 0)
        {
            var ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(ProviderServiceModel.ServiceType, ProviderServiceModel.CurrencyType);
            var ProviderName = backendDB.GetProviderNameByProviderCode(ProviderServiceModel.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, string.Format("(停用/启用)供应商支付类型,供应商:{0},支付类型:{1},更改为:{2}", ProviderName, ServiceTypeName, StrBeforeProviderState), IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderServiceResult_Company")]
    public ProviderServiceResult GetProviderServiceResult_Company([FromBody] FromBody.ProviderService fromBody)
    {
        ProviderServiceResult retValue = new ProviderServiceResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        retValue.ProviderServices = backendDB.GetProviderServiceResult("", fromBody.ServiceType, fromBody.CurrencyType);
        if (retValue.ProviderServices != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderServiceGPayRelationByCompany")]
    public ProviderServiceResult GetProviderServiceGPayRelationByCompany([FromBody] FromBody.CompanyService fromBody)
    {
        ProviderServiceResult retValue = new ProviderServiceResult();
        BackendDB backendDB = new BackendDB();
        int CompanyID;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 4)
        {
            CompanyID = AdminData.forCompanyID;
        }
        else if (AdminData.CompanyType == 0)
        {
            CompanyID = fromBody.forCompanyID;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.ProviderServices = backendDB.GetProviderServiceGPayRelationByCompany(CompanyID, fromBody.ServiceType, fromBody.CurrencyType, AdminData.CompanyType);
        if (retValue.ProviderServices != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }
    #endregion

    #region PermissionCategory
    [HttpGet]
    [HttpPost]
    [ActionName("GetPermissionCategoryResult")]
    public PermissionCategoryResult GetPermissionCategoryResult([FromBody] string BID)
    {
        PermissionCategoryResult retValue = new PermissionCategoryResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.PermissionCategorys = backendDB.GetPermissionCategoryResult();
        if (retValue.PermissionCategorys != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertPermissionCategoryResult")]
    public APIResult InsertPermissionCategoryResult([FromBody] FromBody.PermissionCategory PermissionCategoryModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(PermissionCategoryModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(PermissionCategoryModel.BID);
        if (backendDB.InsertPermissionCategory(PermissionCategoryModel) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "新增功能群组:" + PermissionCategoryModel.Description, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdatePermissionCategoryResult")]
    public APIResult UpdatePermissionCategoryResult([FromBody] FromBody.PermissionCategory PermissionCategoryModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(PermissionCategoryModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(PermissionCategoryModel.BID);
        List<DBModel.PermissionCategory> permissioncategorys = backendDB.GetPermissionCategoryResult(PermissionCategoryModel.PermissionCategoryID);
        if (permissioncategorys != null)
        {
            if (backendDB.UpdatePermissionCategory(PermissionCategoryModel) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "修改功能群组:" + PermissionCategoryModel.Description, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DeletePermissionCategoryResult")]
    public APIResult DeletePermissionCategoryResult([FromBody] FromBody.PermissionCategory PermissionCategoryModel)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(PermissionCategoryModel.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(PermissionCategoryModel.BID);
        List<DBModel.PermissionCategory> permissioncategorys = backendDB.GetPermissionCategoryResult(PermissionCategoryModel.PermissionCategoryID);
        if (permissioncategorys != null)
        {
            if (backendDB.DeletePermissionCategory(PermissionCategoryModel) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "删除功能群组", IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }
    #endregion

    #region ProviderPointHistory
    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderPointHistory")]
    public ProviderPointHistory GetProviderPointHistory([FromBody] FromBody.GetProviderPointHistorySet fromBody)
    {
        ProviderPointHistory retValue = new ProviderPointHistory();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.ProviderPointHistoryResults = backendDB.GetProviderPointHistory(fromBody);

        if (retValue.ProviderPointHistoryResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderPrePaidHistory")]
    public ProviderPointHistory GetProviderPrePaidHistory([FromBody] FromBody.GetProviderPointHistorySet fromBody)
    {
        ProviderPointHistory retValue = new ProviderPointHistory();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.ProviderPointHistoryResults = backendDB.GetProviderPrePaidHistory(fromBody);

        if (retValue.ProviderPointHistoryResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    #endregion

    #region ProviderPoint

    [HttpGet]
    [HttpPost]
    [ActionName("GetAllProviderPoint")]
    public ProviderPointResult2 GetAllProviderPoint([FromBody] FromBody.Company fromBody)
    {
        ProviderPointResult2 retValue = new ProviderPointResult2();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {
            retValue.ProviderPointResults = backendDB.GetAllProviderPoint2();
            retValue.CompanyServicePointResults = backendDB.GetCompanyServicePointDetail(fromBody.CompanyID);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (retValue.ProviderPointResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetAllProviderPointBC")]
    public ProviderPointResult2 GetAllProviderPointBC([FromBody] FromBody.GetAllProviderPointBC fromBody)
    {
        ProviderPointResult2 retValue = new ProviderPointResult2();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {
            retValue.ProviderPointResults = backendDB.GetAllProviderPointBC(fromBody.CurrencyType, fromBody.ServiceType);
            retValue.CompanyServicePointResults = backendDB.GetCompanyServicePointDetail(fromBody.CompanyID);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (retValue.ProviderPointResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpGet]
    [HttpPost]
    [ActionName("GetAllProviderPoint2")]
    public ProviderPointResult GetAllProviderPoint2([FromBody] string BID)
    {
        ProviderPointResult retValue = new ProviderPointResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.ProviderPointResults = backendDB.GetAllProviderPoint();

        if (retValue.ProviderPointResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }
    #endregion

    #region CompanyPoint
    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyServicePointByServiceType")]
    public CompanyServicePoint GetCompanyServicePointByServiceType([FromBody] FromBody.ProviderService fromBody)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        List<DBViewModel.CompanyServicePointVM> companys = backendDB.GetCompanyServicePointDetail(AdminData.forCompanyID);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoints = companys.Where(w => w.CurrencyType == fromBody.CurrencyType).ToList();
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCanUseCompanyServicePoint")]
    public CompanyServicePoint GetCanUseCompanyServicePoint([FromBody] string BID)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        List<DBViewModel.CompanyServicePointVM> companys = backendDB.GetCanUseCompanyServicePoint(AdminData.forCompanyID);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoints = companys;
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }


    [HttpGet]
    [HttpPost]
    [ActionName("GetCanUseCompanyServicePointByService")]
    public CompanyServicePointByService GetCanUseCompanyServicePointByService([FromBody] FromBody.InsertServiceType fromBody)
    {
        CompanyServicePointByService _CompanyServicePointResult = new CompanyServicePointByService();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBViewModel.CompanyServicePointVM companys = backendDB.GetCanUseCompanyServicePointByService(AdminData.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoint = companys;
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyServicePointDetail")]
    public CompanyServicePoint GetCompanyServicePointDetail([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        List<DBViewModel.CompanyServicePointVM> companys = backendDB.GetCompanyServicePointDetail(AdminData.forCompanyID);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoints = companys.Where(w => w.CurrencyType == fromBody.CurrencyType).ToList();
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetAllCompanyServicePoint")]
    public CompanyServicePoint GetAllCompanyServicePoint([FromBody] FromBody.CompanyServiceSet fromBody)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyServicePointResult;
        }

        List<DBViewModel.CompanyServicePointVM> companys = backendDB.GetAllCompanyServicePoint(fromBody.CurrencyType);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoints = companys;
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyServicePointDetail2")]
    public CompanyServicePoint GetCompanyServicePointDetail2([FromBody] FromBody.Company fromBody)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();
        RedisCache.BIDContext.BIDInfo AdminData;
        BackendDB backendDB = new BackendDB();
        int CompanyID;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServicePointResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            CompanyID = fromBody.CompanyID;
        }
        else if (AdminData.CompanyType == 4)
        {
            CompanyID = AdminData.forCompanyID;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyServicePointResult;
        }

        var CompanyModel = RedisCache.Company.GetCompanyByID(CompanyID);

        List<DBViewModel.CompanyServicePointVM> companys = backendDB.GetCompanyServicePointDetail2(CompanyID, (string)CompanyModel.Rows[0]["CurrencyType"]);
        if (companys != null)
        {
            _CompanyServicePointResult.CompanyServicePoints = companys;
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyServicePointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyServicePointResult;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyPointTableResult")]
    public CompanyPointResult GetCompanyPointTableResult([FromBody] FromBody.Company fromBody)
    {
        CompanyPointResult _CompanyTableResult = new CompanyPointResult();

        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        //List<DBViewModel.CompanyPointVM> companys = backendDB.GetCompanyPointTableResult(seleCompanyID);
        List<DBViewModel.CompanyPointVM> companys = new List<DBViewModel.CompanyPointVM>();

        if (fromBody.CompanyID != 0)
            companys = backendDB.GetCompanyPointTableResult(fromBody.CompanyID);
        else
            companys = backendDB.GetAllCompanyPointTableResult(fromBody.CurrencyType);
        if (companys != null)
        {
            _CompanyTableResult.CompanyPoints = companys;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    //[HttpGet]
    //[HttpPost]
    //[ActionName("GetCompanyPointTableResultByFilter")]
    //public CompanyPointResult GetCompanyPointTableResultByFilter(int CompanyID,string ProviderCode)
    //{
    //    CompanyPointResult _CompanyTableResult = new CompanyPointResult();

    //    BackendDB backendDB = new BackendDB();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
    //        return _CompanyTableResult;
    //    }

    //    var AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);


    //    List<DBViewModel.CompanyPointVM> companys = new List<DBViewModel.CompanyPointVM>();

    //    if (seleCompanyID != 0)
    //        companys = backendDB.GetCompanyPointTableResult(seleCompanyID);
    //    else
    //        companys = backendDB.GetAllCompanyPointTableResult();
    //    if (companys != null)
    //    {
    //        _CompanyTableResult.CompanyPoints = companys.Where(w => w.CurrencyType == "JPY").ToList();
    //        _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
    //    }
    //    return _CompanyTableResult;
    //}

    [HttpGet]
    [HttpPost]
    [ActionName("GetCompanyPointAndCompanyServicePointResult")]
    public CompanyPointAndCompanyServicePointResult GetCompanyPointAndCompanyServicePointResult([FromBody] string BID)
    {
        CompanyPointAndCompanyServicePointResult _CompanyTableResult = new CompanyPointAndCompanyServicePointResult();

        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyTableResult;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        List<DBViewModel.CompanyPointVM> CompanyPoints = new List<DBViewModel.CompanyPointVM>();
        CompanyPoints = backendDB.GetCompanyPointDetailTableResult(AdminData.forCompanyID);

        List<DBViewModel.CompanyServicePointVM> CompanyServicePoints = new List<DBViewModel.CompanyServicePointVM>();
        CompanyServicePoints = backendDB.GetCanUseCompanyServicePoint(AdminData.forCompanyID);


        if (CompanyPoints != null && CompanyPoints != null)
        {
            _CompanyTableResult.CompanyPoints = CompanyPoints;
            _CompanyTableResult.CompanyServicePoints = CompanyServicePoints;
            _CompanyTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _CompanyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _CompanyTableResult;
    }

    [HttpPost]
    [ActionName("InsertCompanyPointTableResult")]
    public CompanyPointResult InsertCompanyPointTableResult([FromBody] FromBody.CompanyPoint CompanyPointData)
    {
        CompanyPointResult _CompanyPointResult = new CompanyPointResult();
        int companyResult = 0;
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(CompanyPointData.BID))
        {
            _CompanyPointResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyPointResult;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(CompanyPointData.BID);
        string[] CurrencyTypes = CompanyPointData.CurrencyType.Split(',');
        var CompanyName = backendDB.GetCompanyByID(CompanyPointData.forCompanyID).CompanyName;
        foreach (var currencytype in CurrencyTypes)
        {
            List<DBViewModel.CompanyPointVM> companypoint = backendDB.GetCompanyPointTableResult(CompanyPointData.forCompanyID, currencytype);
            if (companypoint == null || companypoint.Count == 0)
            {
                companyResult = backendDB.InsertCompanyPoint(CompanyPointData.forCompanyID, currencytype);
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增商户钱包,商户:" + CompanyName + ",币别:" + currencytype, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
            }
        }

        _CompanyPointResult.ResultCode = APIResult.enumResult.OK;
        return _CompanyPointResult;
    }

    //[HttpPost]
    //[ActionName("UpdateCompanyPointTableResult")]
    //public CompanyPointResult UpdateCompanyPointTableResult([FromBody] DBModel.CompanyPoint CompanyPointData)
    //{
    //    CompanyPointResult _CompanyPointResult = new CompanyPointResult();
    //    int companyResult = 0;
    //    BackendDB backendDB = new BackendDB();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        _CompanyPointResult.ResultCode = APIResult.enumResult.SessionError;
    //        return _CompanyPointResult;
    //    }

    //    var AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    List<DBViewModel.CompanyPointVM> companypoint = backendDB.GetCompanyPointTableResult(CompanyPointData.forCompanyID, CompanyPointData.CurrencyType);
    //    if (companypoint == null)
    //    {
    //        _CompanyPointResult.ResultCode = APIResult.enumResult.NoData;
    //    }
    //    else
    //    {
    //        companyResult = backendDB.UpdateCompanyPoint(CompanyPointData);
    //        _CompanyPointResult.ResultCode = APIResult.enumResult.OK;
    //        var CompanyName = backendDB.GetCompanyByID(CompanyPointData.forCompanyID).CompanyName;
    //        BackendFunction backendFunction = new BackendFunction();
    //        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增商户钱包,商户:" + CompanyName + ",币别:" + CompanyPointData.CurrencyType, IP, fingerprint);
    //    }
    //    return _CompanyPointResult;
    //}

    #endregion

    #region GPayRelation
    [HttpPost]
    [ActionName("GetGPayRelationByCompany")]
    public ProviderServiceResult GetGPayRelationByCompany([FromBody] FromBody.GPayRelation fromBody)
    {
        ProviderServiceResult retValue = new ProviderServiceResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.ProviderServices = backendDB.GetGPayRelationByCompany(fromBody.forCompanyID, fromBody.ServiceType, fromBody.CurrencyType);
        if (retValue.ProviderServices != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpPost]
    [ActionName("GetGPayRelationTableResult")]
    public GPayRelationResult GetGPayRelationTableResult([FromBody] FromBody.Provider fromBody)
    {
        GPayRelationResult retValue = new GPayRelationResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.GPayRelations = backendDB.GetGPayRelationTableResult(fromBody.ProviderCode);
        if (retValue.GPayRelations != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpPost]
    [ActionName("GetGPayRelationTableResultByServiceType")]
    public GPayRelationResult GetGPayRelationTableResultByServiceType([FromBody] FromBody.GPayRelation fromBody)
    {
        GPayRelationResult retValue = new GPayRelationResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.GPayRelations = backendDB.GetGPayRelationTableResultByServiceType(fromBody.ServiceType);
        if (retValue.GPayRelations != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpPost]
    [ActionName("GetGPayRelationTableResult2")]
    public List<DBModel.GPayRelation> GetGPayRelationTableResult2([FromBody] DBModel.GPayRelation fromBody)
    {
        List<DBModel.GPayRelation> retValue = new List<DBModel.GPayRelation>();
        BackendDB backendDB = new BackendDB();

        retValue = backendDB.GetGPayRelationResult(fromBody.ServiceType, fromBody.CurrencyType, "", fromBody.forCompanyID);

        return retValue;
    }

    #endregion

    #region GPayWithdrawRelation
    [HttpPost]
    [ActionName("GetCompanyWithdrawRelationResult")]
    public WithdrawLimitResult GetCompanyWithdrawRelationResult([FromBody] FromBody.GPayWithdrawRelationSet data)
    {
        WithdrawLimitResult retValue = new WithdrawLimitResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.WithdrawLimits = backendDB.GetCompanyWithdrawRelationResult(data.CompanyID);
        if (retValue.WithdrawLimits != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetApiWithdrawLimit")]
    public GetApiWithdrawLimitResult GetApiWithdrawLimit([FromBody] FromBody.GPayWithdrawRelationSet data)
    {
        GetApiWithdrawLimitResult retValue = new GetApiWithdrawLimitResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            retValue.ApiWithdrawLimitResults = backendDB.GetApiWithdrawLimit(data.CompanyID);
        }
        else if (AdminData.CompanyType == 4)
        {
            retValue.ApiWithdrawLimitResults = backendDB.GetApiWithdrawLimitByProviderCompanyID(data.CompanyID, AdminData.forCompanyID);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (retValue.ApiWithdrawLimitResults != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertGPayWithdrawRelation")]
    public APIResult InsertGPayWithdrawRelation([FromBody] FromBody.GPayWithdrawRelationSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.InsertGPayWithdrawRelation(fromBody, AdminData.CompanyType);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }

        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            var CompanyName = backendDB.GetCompanyByID(fromBody.CompanyID).CompanyName;
            var ProviderName = "";
            if (fromBody.ProviderCodeAndWeight != null && fromBody.ProviderCodeAndWeight.Count > 0)
            {
                ProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCodeAndWeight.First().ProviderCode);
            }

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增商户自动代付设定,商户名称:" + CompanyName + ",币别:" + fromBody.CurrencyType + ",最小值:" + fromBody.MinLimit + ",最大值:" + fromBody.MaxLimit + ",手续费:" + fromBody.Charge + ",对应供应商:" + ProviderName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateGPayWithdrawRelation")]
    public APIResult UpdateGPayWithdrawRelation([FromBody] FromBody.GPayWithdrawRelationSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.UpdateGPayWithdrawRelation(fromBody, AdminData.CompanyType);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            var CompanyName = backendDB.GetCompanyByID(fromBody.CompanyID).CompanyName;
            var ProviderName = "";
            if (fromBody.ProviderCodeAndWeight != null && fromBody.ProviderCodeAndWeight.Count > 0)
            {
                ProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCodeAndWeight.First().ProviderCode);
            }

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "修改商户自动代付设定,商户名称:" + CompanyName + ",币别:" + fromBody.CurrencyType + ",最小值:" + fromBody.MinLimit + ",最大值:" + fromBody.MaxLimit + ",手续费:" + fromBody.Charge + ",对应供应商:" + ProviderName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetGPayWithdrawRelationByCompanyID")]
    public GPayWithdrawRelationResult GetGPayWithdrawRelationByCompanyID([FromBody] FromBody.GPayWithdrawRelationSet data)
    {
        GPayWithdrawRelationResult retValue = new GPayWithdrawRelationResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        retValue.GPayWithdrawRelations = backendDB.GetGPayWithdrawRelationByCompanyID(data.CompanyID);
        if (retValue.GPayWithdrawRelations != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    #endregion

    #region SummaryCompanyByDate
    [HttpPost]
    [ActionName("GetCompanyServicePointHistoryResult")]
    public CompanyServicePointHistory GetCompanyServicePointHistoryResult([FromBody] FromBody.PaymentTable SearchData)
    {
        CompanyServicePointHistory retValue = new CompanyServicePointHistory();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.CompanyServicePointHistorys = backendDB.GetCompanyServicePointHistoryResult(SearchData);
        if (retValue.CompanyServicePointHistorys != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetCompanyServicePointLogResult")]
    public CompanyServicePointLog GetCompanyServicePointLogResult([FromBody] FromBody.PaymentTable SearchData)
    {
        CompanyServicePointLog retValue = new CompanyServicePointLog();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.CompanyServicePointLogs = backendDB.GetCompanyPointHistoryLogResult(SearchData);
        if (retValue.CompanyServicePointLogs != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetAgentCompanyServicePointLogResultByCompany")]
    public CompanyServicePointLog GetAgentCompanyServicePointLogResultByCompany([FromBody] FromBody.PaymentTable SearchData)
    {
        CompanyServicePointLog retValue = new CompanyServicePointLog();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = null;
        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        retValue.CompanyServicePointLogs = backendDB.GetCompanyServicePointLogResultByCompany(SearchData);
        if (retValue.CompanyServicePointLogs != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetCompanyServicePointLogResultByCompany")]
    public CompanyServicePointLog GetCompanyServicePointLogResultByCompany([FromBody] FromBody.PaymentTable SearchData)
    {
        CompanyServicePointLog retValue = new CompanyServicePointLog();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = null;
        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        SearchData.CompanyID = AdminData.forCompanyID;
        retValue.CompanyServicePointLogs = backendDB.GetCompanyServicePointLogResultByCompany(SearchData);
        if (retValue.CompanyServicePointLogs != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetManualPointLogResult")]
    public CompanyServiceAndProviderPointLog GetManualPointLogResult([FromBody] FromBody.PaymentTable SearchData)
    {
        CompanyServiceAndProviderPointLog retValue = new CompanyServiceAndProviderPointLog();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.CompanyServicePointLogs = backendDB.GetManualPointLogResult(SearchData);
        if (retValue.CompanyServicePointLogs != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpPost]
    [ActionName("GetSummaryCompanyByDateResultFlot")]
    public SummaryCompanyByDateFlot GetSummaryCompanyByDateResultFlot([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByDateFlot retValue = new SummaryCompanyByDateFlot();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }
        SearchData.CompanyID = AdminData.forCompanyID;
        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByDateResultFlot(SearchData);

        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;

        }

        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByDateResult")]
    public SummaryCompanyByDate GetSummaryCompanyByDateResult([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByDate retValue = new SummaryCompanyByDate();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByDateResult(SearchData);

        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            var AgentSummaryCompany = retValue.SummaryCompanyByDates.Where(w => w.CompanyType == 2 && w.SummaryAgentAmount != 0).ToList();
            retValue.SummaryCompanyByDates = retValue.SummaryCompanyByDates.Where(w => w.SummaryAmount != 0 || w.SummaryCompanyWithdrawalChargeAmount != 0).ToList();

            retValue.TotalPayAgentAmount = AgentSummaryCompany.Sum(s => s.SummaryAgentAmount);

            foreach (var data in retValue.SummaryCompanyByDates)
            {
                retValue.TotalAmount += data.SummaryAmount;
                retValue.TotalNetAmount += data.SummaryNetAmount;
                retValue.TotalProviderNetAmount += data.SummaryProviderNetAmount;
                retValue.TotalNetWithdrawAmount += (data.SummaryCompanyWithdrawalChargeAmount - data.SummaryProviderWithdrawalChargeAmount);
            }
        }

        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryBlockChainByDateResult")]
    public SummaryBlockChainByDate GetSummaryBlockChainByDateResult([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryBlockChainByDate retValue = new SummaryBlockChainByDate();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.SummaryBlockChainByDates = backendDB.GetSummaryBlockChainByDateResult(SearchData);

        if (retValue.SummaryBlockChainByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            foreach (var data in retValue.SummaryBlockChainByDates)
            {
                retValue.TotalAmount += data.SummaryProviderAmount;
                retValue.TotalNetAmount += data.UpSummaryBlockChainAmount - data.DownSummaryBlockChainAmount;
            }
        }

        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByHourResult")]
    public SummaryCompanyByHour GetSummaryCompanyByHourResult([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByHour retValue = new SummaryCompanyByHour();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        retValue.SummaryCompanyByHours = backendDB.GetSummaryCompanyByHourResult(SearchData);

        if (retValue.SummaryCompanyByHours != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByAgent")]
    public SummaryCompanyByDate GetSummaryCompanyByAgent([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByDate retValue = new SummaryCompanyByDate();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByAgent(SearchData);

        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            foreach (var data in retValue.SummaryCompanyByDates)
            {
                retValue.TotalAgentAmount += data.SummaryAgentAmount;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }


    [HttpPost]
    [ActionName("GetSummaryCompanyByAgent2")]
    public SummaryCompanyByDate GetSummaryCompanyByAgent2([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByDate retValue = new SummaryCompanyByDate();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByAgent2(SearchData, AdminData.forCompanyID);

        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            foreach (var data in retValue.SummaryCompanyByDates)
            {
                retValue.TotalAgentAmount += data.SummaryAgentAmount;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByAgentDownCompany")]
    public SummaryCompanyByDate GetSummaryCompanyByAgentDownCompany([FromBody] FromBody.PaymentTable SearchData)
    {
        SummaryCompanyByDate retValue = new SummaryCompanyByDate();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(SearchData.BID);
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByAgentDownCompany(SearchData, AdminData.forCompanyID);

        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            foreach (var data in retValue.SummaryCompanyByDates)
            {
                retValue.TotalAgentAmount += data.SummaryAmount;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByDateResultByCurrencyType")]
    public SummaryCompanyByDate GetSummaryCompanyByDateResultByCurrencyType([FromBody] FromBody.SummaryCompanyByDateResultByCurrencyTypeSet SearchData)
    {
        SummaryCompanyByDate retValue = new SummaryCompanyByDate();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByDateResultByCurrencyType(SearchData);
        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("GetSummaryCompanyByDateResultForChart")]
    public SummaryCompanyByDateResultForChart GetSummaryCompanyByDateResultForChart([FromBody] FromBody.SummaryCompanyByDateSet SearchData)
    {
        SummaryCompanyByDateResultForChart retValue = new SummaryCompanyByDateResultForChart();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.SummaryCompanyByDates = backendDB.GetSummaryCompanyByDateResultForChart(SearchData);
        if (retValue.SummaryCompanyByDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }
    #endregion

    #region PointHistory
    [HttpPost]
    [ActionName("GetCompanyPointHistoryTableResult")]
    public CompanyPointHistoryResult GetCompanyPointHistoryTableResult([FromBody] FromBody.CompanyPointHistoryeSet SearchData)
    {
        CompanyPointHistoryResult retValue = new CompanyPointHistoryResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(SearchData.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.CompanyPointHistoryDates = backendDB.GetCompanyPointHistoryTableResult(SearchData);
        if (retValue.CompanyPointHistoryDates != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }
    #endregion

    #region 銀行設定

    [HttpPost]
    [ActionName("GetBankCodeTableResult")]
    public BankCodeTableResult GetBankCodeTableResult([FromBody] FromBody.BankCodeSet fromBody)
    {
        BankCodeTableResult _BankCodeTableResult = new BankCodeTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _BankCodeTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _BankCodeTableResult;
        }
        BackendDB backendDB = new BackendDB();
        List<DBModel.BankCodeTable> bankCodeTableResult = backendDB.GetBankCodeTableResult();
        if (bankCodeTableResult != null)
        {
            _BankCodeTableResult.BankCodeResults = bankCodeTableResult;
            _BankCodeTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _BankCodeTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _BankCodeTableResult;
    }

    [HttpPost]
    [ActionName("InsertBankCode")]
    public APIResult InsertBankCode([FromBody] FromBody.BankCodeSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.InsertBankCode(fromBody);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "新增银行:" + fromBody.BankName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateBankCode")]
    public APIResult UpdateBankCode([FromBody] FromBody.BankCodeSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }



        DBretValue = backendDB.UpdateBankCode(fromBody);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "修改银行:" + fromBody.BankCode, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("DisableBankCode")]
    public APIResult DisableBankCode([FromBody] FromBody.BankCodeSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }



        DBretValue = backendDB.DisableBankCode(fromBody);

        if (DBretValue >= 1)
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 4, "停用银行:" + fromBody.BankCode, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }
    #endregion

    #region 銀行卡設定
    [HttpPost]
    [ActionName("GetBankCardTableResult")]
    public BankCardTableResult GetBankCardTableResult([FromBody] FromBody.BankCardSet fromBody)
    {
        BankCardTableResult _BankCardTableResult = new BankCardTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _BankCardTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _BankCardTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBViewModel.BankCardVM> bankCardTableResult = backendDB.GetBankCardTableResult(fromBody);
        if (bankCardTableResult != null)
        {
            _BankCardTableResult.BankCardResults = bankCardTableResult;
            _BankCardTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _BankCardTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _BankCardTableResult;
    }

    [HttpPost]
    [ActionName("InsertBankCard")]
    public APIResult InsertBankCard([FromBody] FromBody.BankCardSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string fingerprint = GetFingerprint();

        int DBretValue = -1;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.InsertBankCard(fromBody);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "新增银行卡:" + fromBody.BankCard, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateBankCard")]
    public APIResult UpdateBankCard([FromBody] FromBody.BankCardSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        DBretValue = backendDB.UpdateBankCard(fromBody);

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改银行卡:" + fromBody.BankCard, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        retValue.ResultCode = APIResult.enumResult.OK;


        return retValue;
    }

    [HttpPost]
    [ActionName("DeleteBankCard")]
    public APIResult DeleteBankCard([FromBody] FromBody.BankCardSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }



        DBretValue = backendDB.DeleteBankCard(fromBody);
        if (DBretValue >= 1)
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "删除银行卡:" + fromBody.BankCard, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }



    #endregion

    #region 區塊鏈地址設定
    [HttpPost]
    [ActionName("GetBlockChainAddressResult")]
    public CompanyBlockChainAddressResult GetBlockChainAddressResult([FromBody] FromBody.BlockChainAddressSet fromBody)
    {
        CompanyBlockChainAddressResult _BankCardTableResult = new CompanyBlockChainAddressResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        int CompanyID = 0;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _BankCardTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _BankCardTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            CompanyID = AdminData.forCompanyID;
        }
        else
        {
            CompanyID = fromBody.forCompanyID;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.CompanyBlockChainAddress> bankCardTableResult = backendDB.GetCompanyBlockChainAddressResult(CompanyID);
        if (bankCardTableResult != null)
        {
            _BankCardTableResult.CompanyBlockChainAddressResults = bankCardTableResult;
            _BankCardTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _BankCardTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _BankCardTableResult;
    }

    [HttpPost]
    [ActionName("InsertBlockChainAddress")]
    public APIResult InsertBlockChainAddress([FromBody] FromBody.BlockChainAddressSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string fingerprint = GetFingerprint();

        int DBretValue = -1;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        DBretValue = backendDB.InsertBlockChainAddress(fromBody, AdminData.forCompanyID);

        if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }
        else
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "新增区块链地址:" + fromBody.BlockChainAddress, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateBlockChainAddress")]
    public APIResult UpdateBlockChainAddress([FromBody] FromBody.BlockChainAddressSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        DBretValue = backendDB.UpdateBlockChainAddress(fromBody, AdminData.forCompanyID);

        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改区块链地址:" + fromBody.BlockChainAddress, IP, fingerprint);
        string XForwardIP = CodingControl.GetXForwardedFor();
        CodingControl.WriteXFowardForIP(AdminOP);
        retValue.ResultCode = APIResult.enumResult.OK;


        return retValue;
    }

    [HttpPost]
    [ActionName("DeleteBlockChainAddress")]
    public APIResult DeleteBlockChainAddress([FromBody] FromBody.BlockChainAddressSet fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        DBretValue = backendDB.DeleteBlockChainAddress(fromBody);
        if (DBretValue >= 1)
        {
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "删除区块链地址:" + fromBody.BlockChainAddress, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }



    #endregion

    #region 会员谷歌验证(只有系统商登入时使用)
    [HttpGet]
    [HttpPost]
    [ActionName("GetGoogleQrCodeByAdmin")]
    public GoogleQrCode GetGoogleQrCodeByAdmin([FromBody] FromBody.GoogleKeySetByAdmin fromBody)
    {
        GoogleQrCode retValue = new GoogleQrCode();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.GoogleQrCode Result = new DBModel.GoogleQrCode();
        DBModel.AdminWithGoogleKey _AdminData = null;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        _AdminData = backendDB.GetAdminByLoginAccountWithGoogleKey(fromBody.LoginAccount);

        if (_AdminData == null)
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
            return retValue;
        }


        //尚未綁定google認證
        if (string.IsNullOrEmpty(_AdminData.GoogleKey))
        {
            Result = backendFunction.GetGoogleQrCode(_AdminData.LoginAccount);
            Result.IsCreated = false;
            retValue.GoogleQrCodeResult = Result;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.IsCreated = true;
            retValue.GoogleQrCodeResult = Result;
            retValue.ResultCode = APIResult.enumResult.OK;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("SetGoogleQrCodeByAdmin")]
    public APIResult SetGoogleQrCodeByAdmin([FromBody] FromBody.GoogleKeySetByAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        //檢查google認證
        if (backendFunction.CheckGoogleKey(fromBody.GoogleKey, fromBody.UserKey))
        {
            //更新DB公司資料
            backendDB.UpdateAdminGoogleKey(fromBody.GoogleKey, fromBody.LoginAccount);
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "绑定谷歌验证,账号:" + fromBody.LoginAccount, IP, fingerprint);
            backendDB.InsertBotSendLog(AdminData.CompanyCode, "绑定谷歌验证,账号:" + fromBody.LoginAccount);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UnsetGoogleQrCodeByAdmin")]
    public APIResult UnsetGoogleQrCodeByAdmin([FromBody] FromBody.GoogleKeySetByAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.AdminWithGoogleKey _AdminData = null;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        _AdminData = backendDB.GetAdminByLoginAccountWithGoogleKey(fromBody.LoginAccount);

        //檢查google認證
        if (backendFunction.CheckGoogleKey(_AdminData.GoogleKey, fromBody.UserKey))
        {
            //更新DB公司資料
            backendDB.UpdateAdminGoogleKey("", fromBody.LoginAccount);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "解除谷歌验证,账号:" + fromBody.LoginAccount, IP, fingerprint);
            backendDB.InsertBotSendLog(AdminData.CompanyCode, "解除谷歌验证,账号:" + fromBody.LoginAccount);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [HttpGet]
    [ActionName("CheckGoogleKeyByAdmin")]
    public APIResult CheckGoogleKeyByAdmin(string BID, string UserKey, string LoginAccount)
    {
        APIResult retValue = new APIResult();
        DBModel.AdminWithGoogleKey _AdminData = null;

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        _AdminData = backendDB.GetAdminByLoginAccountWithGoogleKey(LoginAccount);

        if (string.IsNullOrEmpty(_AdminData.GoogleKey))
        {
            retValue.Message = "尚未绑定 Google 验证器";
            retValue.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
            return retValue;
        }

        //檢查google認證
        if (backendFunction.CheckGoogleKey(_AdminData.GoogleKey, UserKey))
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.GoogleKeyError;
        }

        return retValue;
    }


    #endregion

    #region 提現相關


    //[HttpPost]
    //[ActionName("RemoveGoogleQrCode")]
    //public APIResult RemoveGoogleQrCode([FromBody] FromBody.RemoveGoogleQrCode fromBody)
    //{
    //    APIResult retValue = new APIResult();
    //    BackendDB backendDB = new BackendDB();
    //    int DBreturn=0;
    //    //驗證權限
    //    RedisCache.BIDContext.BIDInfo AdminData;
    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        retValue.ResultCode = APIResult.enumResult.SessionError;
    //        return retValue;
    //    }
    //    else
    //    {
    //        AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        retValue.ResultCode = APIResult.enumResult.VerificationError;
    //        return retValue;
    //    }

    //    AdminData =  RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

    //    BackendFunction backendFunction = new BackendFunction();

    //    if (!backendFunction.CheckPassword(fromBody.Password, AdminData.AdminID))
    //    {
    //        retValue.ResultCode = APIResult.enumResult.PasswordEmpty;
    //        return retValue;
    //    }

    //    string CompanyName= backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);
    //    DBreturn = backendDB.UpdateCompanyGoogleKey("", fromBody.CompanyID);

    //    if (DBreturn >0)
    //    {

    //        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "解除谷歌验证,商户:" + CompanyName, IP, fingerprint);
    //        retValue.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        retValue.ResultCode = APIResult.enumResult.Error;
    //    }

    //    return retValue;
    //}

    [HttpPost]
    [ActionName("GetGoogleQrCode")]
    public GoogleQrCode GetGoogleQrCode([FromBody] string BID)
    {
        GoogleQrCode retValue = new GoogleQrCode();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.GoogleQrCode Result = new DBModel.GoogleQrCode();
        DBModel.CompanyWithGooleKey CompanyData = null;
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        CompanyData = backendDB.GetCompanyByIDWithGooleKey(AdminData.forCompanyID);
        //尚未綁定google認證
        if (string.IsNullOrEmpty(CompanyData.GoogleKey))
        {
            Result = backendFunction.GetGoogleQrCode(AdminData.CompanyName);
            Result.IsCreated = false;
            retValue.GoogleQrCodeResult = Result;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.IsCreated = true;
            retValue.GoogleQrCodeResult = Result;
            retValue.ResultCode = APIResult.enumResult.OK;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("SetGoogleQrCode")]
    public APIResult SetGoogleQrCode([FromBody] FromBody.GoogleKeySet fromBody)
    {
        APIResult retValue = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        //檢查google認證
        if (backendFunction.CheckGoogleKey(fromBody.GoogleKey, fromBody.UserKey))
        {
            //更新DB公司資料
            backendDB.UpdateCompanyGoogleKey(fromBody.GoogleKey, AdminData.forCompanyID);
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "绑定谷歌验证", IP, fingerprint);
            backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + "绑定谷歌验证");
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UnsetGoogleQrCode")]
    public APIResult UnsetGoogleQrCode([FromBody] FromBody.GoogleKeySet fromBody)
    {
        APIResult retValue = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.CompanyWithGooleKey CompanyData = null;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        CompanyData = backendDB.GetCompanyByIDWithGooleKey(AdminData.forCompanyID);

        //檢查google認證
        if (backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey))
        {
            //更新DB公司資料
            backendDB.UpdateCompanyGoogleKey("", AdminData.forCompanyID);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "解除谷歌验证", IP, fingerprint);
            backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + "解除谷歌验证");
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [HttpGet]
    [ActionName("CheckGoogleKey")]
    public APIResult CheckGoogleKey([FromBody] FromBody.GoogleKeySetByAdmin fromBody)
    {
        APIResult retValue = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.CompanyWithGooleKey CompanyData = null;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendFunction backendFunction = new BackendFunction();
        BackendDB backendDB = new BackendDB();

        CompanyData = backendDB.GetCompanyByIDWithGooleKey(AdminData.forCompanyID);

        if (string.IsNullOrEmpty(CompanyData.GoogleKey))
        {
            retValue.Message = "尚未绑定 Google 验证器";
            retValue.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
            return retValue;
        }

        //檢查google認證
        if (backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey))
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.GoogleKeyError;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetWithdrawalCount")]
    public APIResult GetWithdrawalCount([FromBody] string BID)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        retValue.Message = backendDB.GetWithdrawalCount();

        retValue.ResultCode = APIResult.enumResult.OK;

        return retValue;
    }

    [HttpPost]
    [ActionName("GetProviderOrderCount")]
    public GetProviderOrderCountResult GetProviderOrderCount([FromBody] string BID)
    {
        GetProviderOrderCountResult retValue = new GetProviderOrderCountResult() { Results = new DBModel.ProviderOrderCount() };
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.Admin AdminModel;
        int GroupID = 0;
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 3)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }
        var ProviderWithdrawalOrderCount = backendDB.GetProviderWithdrawalOrderCount(AdminData.CompanyCode, GroupID);

        if (ProviderWithdrawalOrderCount != null)
        {
            retValue.Results.WithdrawCount = ProviderWithdrawalOrderCount.TotalCount;
            retValue.Results.WithdrawCountByTimeEnd = ProviderWithdrawalOrderCount.TotalCountTimeEnd;
        }
        else
        {
            retValue.Results.WithdrawCount = 0;
            retValue.Results.WithdrawCountByTimeEnd = 0;
        }

        retValue.Results.PaymentCount = backendDB.GetProviderPaymentOrderCount(AdminData.CompanyCode, GroupID);
        retValue.ResultCode = APIResult.enumResult.OK;

        return retValue;
    }

    [HttpPost]
    [ActionName("GetRiskControlByPaymentSuccessCount")]
    public RiskControlByPaymentSuccessCount GetRiskControlByPaymentSuccessCount([FromBody] string BID)
    {
        RiskControlByPaymentSuccessCount retValue = new RiskControlByPaymentSuccessCount();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var DBreturn = backendDB.RiskControlByPaymentSuccessCount();

        if (DBreturn != null)
        {
            retValue.Results = DBreturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("GetRiskControlByWithdrawCount")]
    public APIResult GetRiskControlByWithdrawCount([FromBody] string BID)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var DBreturn = backendDB.RiskControlWithdrawalCount();

        if (DBreturn > 0)
        {
            retValue.Message = DBreturn.ToString();
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetRiskControlPayment")]
    public PaymentTableResult GetRiskControlPayment([FromBody] string BID)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        PaymentTableResult _PaymentTableResult = new PaymentTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 0)
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.PaymentReport> _PaymentTable = backendDB.GetRiskControlPayment();

        if (_PaymentTable != null)
        {
            _PaymentTableResult.PaymentTableResults = _PaymentTable;
            _PaymentTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetRiskControlWithdrawal")]
    public WithdrawalReportTableResult GetRiskControlWithdrawal([FromBody] string BID)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        WithdrawalReportTableResult _WithdrawalTableResult = new WithdrawalReportTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.RiskControlWithdrawalTable> _WithdrawalTable = backendDB.GetRiskControlWithdrawal();

        if (_WithdrawalTable != null)
        {
            _WithdrawalTableResult.WithdrawalTableResults = _WithdrawalTable;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("WithdrawalCreate")]
    public APIResult WithdrawalCreate([FromBody] FromBody.WithdrawalCreate fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        DBModel.WithdrawLimit _WithdrawLimit = new DBModel.WithdrawLimit();
        DBModel.CompanyWithGooleKey CompanyModel = new DBModel.CompanyWithGooleKey();
        List<DBModel.ConfigSetting> ConfigSetting = new List<DBModel.ConfigSetting>();
        string OldCurrencyType = "";
        int DBretValue = 0;
        decimal Charge = 0;
        decimal WithdrawalRate = 0;
        string OldServiceType = "";
        string WithdrawOption = "";
        string fingerprint = GetFingerprint();



        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //驗證權限
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }
        ConfigSetting = backendDB.GetConfigSettingFromRedis("WithdrawOption");

        if (ConfigSetting != null)
        {
            WithdrawOption = ConfigSetting.First().SettingValue;
            if (WithdrawOption == "1")
            {
                retValue.ResultCode = APIResult.enumResult.CompanyPointError;
                retValue.Message = "代付功能关闭中";
                return retValue;
            }
        }


        CompanyModel = backendDB.GetCompanyByIDWithGooleKey(AdminData.forCompanyID);

        if (CompanyModel == null)
        {
            retValue.ResultCode = APIResult.enumResult.CompanyPointError;
            retValue.Message = "找不到商户资讯";
            return retValue;
        }

        if (CompanyModel.CompanyState == 1)
        {
            retValue.ResultCode = APIResult.enumResult.CompanyPointError;
            retValue.Message = "商户停用中";
            return retValue;
        }

        if (string.IsNullOrEmpty(CompanyModel.GoogleKey))
        {
            retValue.Message = "尚未绑定 Google 验证器";
            retValue.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
            return retValue;

        }
        else
        {
            //檢查google認證
            if (!backendFunction.CheckGoogleKey(CompanyModel.GoogleKey, fromBody.UserKey))
            {
                retValue.Message = " Google 验证有误";
                retValue.ResultCode = APIResult.enumResult.GoogleKeyError;
                return retValue;
            }

        }

        if (!((CompanyModel.WithdrawAPIType & 1) == 1))
        {
            retValue.ResultCode = APIResult.enumResult.CompanyPointError;
            retValue.Message = "没有后台提现功能权限,请联系系统商";
            return retValue;
        }

        fromBody.WithdrawalData = fromBody.WithdrawalData.OrderBy(o => o.CurrencyType).ThenBy(t => t.ServiceType).ToList();

        #region 取得提現手續費
        OldServiceType = fromBody.WithdrawalData.First().ServiceType;
        OldCurrencyType = fromBody.WithdrawalData.First().CurrencyType;
        _WithdrawLimit.CompanyID = AdminData.forCompanyID;
        _WithdrawLimit.WithdrawLimitType = 1;
        _WithdrawLimit.ProviderCode = "";

        //0=上游手續費，API/1=提現手續費/2=代付手續費(下游)
        var withdrawLimitResult = backendDB.GetAllWithdrawLimitResultByCompanyID(_WithdrawLimit);

        if (withdrawLimitResult == null)
        {
            retValue.ResultCode = APIResult.enumResult.CompanyPointError;
            retValue.Message = "尚未设定提现手续费,请联系系统商";
            return retValue;
        }

        var tmpWithdrawLimitResult = withdrawLimitResult.Where(w => w.CurrencyType == OldCurrencyType && w.ServiceType == OldServiceType).ToList();
        if (tmpWithdrawLimitResult.Count > 0)
        {
            Charge = tmpWithdrawLimitResult.First().Charge;
            WithdrawalRate = tmpWithdrawLimitResult.First().Rate;
        }
        else
        {
            Charge = 0;
            WithdrawalRate = 0;
        }

        for (int i = 0; i < fromBody.WithdrawalData.Count(); i++)
        {

            if (OldCurrencyType != fromBody.WithdrawalData[i].CurrencyType || OldServiceType != fromBody.WithdrawalData[i].ServiceType)
            {
                OldCurrencyType = fromBody.WithdrawalData[i].CurrencyType;
                OldServiceType = fromBody.WithdrawalData[i].ServiceType;
                tmpWithdrawLimitResult = withdrawLimitResult.Where(w => w.CurrencyType == fromBody.WithdrawalData[i].CurrencyType && w.ServiceType == fromBody.WithdrawalData[i].ServiceType).ToList();
                if (tmpWithdrawLimitResult.Count > 0)
                {
                    Charge = tmpWithdrawLimitResult.First().Charge;
                    WithdrawalRate = tmpWithdrawLimitResult.First().Rate;
                }
                else
                {
                    Charge = 0;
                    WithdrawalRate = 0;
                }
            }
            fromBody.WithdrawalData[i].CollectCharge = Charge;

            if (fromBody.WithdrawalData[i].Amount < tmpWithdrawLimitResult.First().MinLimit || fromBody.WithdrawalData[i].Amount > tmpWithdrawLimitResult.First().MaxLimit)
            {
                retValue.ResultCode = APIResult.enumResult.CompanyPointError;
                retValue.Message = fromBody.WithdrawalData[i].ServiceTypeName + ",超過限額:" + tmpWithdrawLimitResult.First().MinLimit + "~" + tmpWithdrawLimitResult.First().MaxLimit + ",申請金額:" + fromBody.WithdrawalData[i].Amount;
                return retValue;
            }
        }

        #endregion

        #region 商户钱包额度检查
        var groupWithdrawalDatas = fromBody.WithdrawalData.GroupBy(g => g.CurrencyType)
                           .Select(g => new {
                               CurrencyType = g.Key,
                               TotalAmount = g.Sum(s => s.Amount + s.CollectCharge)
                           });

        foreach (var groupWithdrawalData in groupWithdrawalDatas)
        {
            //檢查錢包金額是否足夠
            var CanUseCompanyPointDT = backendDB.GetCanUseCompanyPoint(AdminData.forCompanyID, groupWithdrawalData.CurrencyType);

            //錢包檢查
            if (!(CanUseCompanyPointDT != null && CanUseCompanyPointDT.Rows.Count > 0))
            {
                retValue.ResultCode = APIResult.enumResult.CompanyPointError;
                retValue.Message = "尚無 " + groupWithdrawalData.CurrencyType + " 錢包";
                return retValue;
            }

            var CompanyPointModel = DataTableExtensions.ToList<DBModel.CompanyPoint>(CanUseCompanyPointDT).FirstOrDefault();

            //點數餘額檢查
            if (groupWithdrawalData.TotalAmount > (CompanyPointModel.CanUsePoint - CompanyPointModel.FrozenPoint))
            {
                retValue.ResultCode = APIResult.enumResult.CompanyPointError;
                retValue.Message = groupWithdrawalData.CurrencyType + " 錢包可用餘額不足";
                return retValue;
            }
        }
        #endregion

        #region 商户通道额度检查
        //var groupServicePointDatas = fromBody.WithdrawalData.GroupBy(g => new {
        //    g.CurrencyType,
        //    g.ServiceType
        //})
        //                   .Select(g => new
        //                   {
        //                       CurrencyType = g.Key.CurrencyType,
        //                       ServiceType = g.Key.ServiceType,
        //                       TotalAmount = g.Sum(s => s.Amount + s.CollectCharge)
        //                   });

        //foreach (var groupWithdrawalData in groupServicePointDatas)
        //{
        //    //檢查錢包金額是否足夠
        //    var CanUseCompanyServicePointModel = backendDB.GetCanUseCompanyServicePointByService(AdminData.forCompanyID, groupWithdrawalData.ServiceType);

        //    //錢包檢查
        //    if (CanUseCompanyServicePointModel == null)
        //    {
        //        string ServiceTypeName=  backendDB.GetServiceTypeNameByServiceType(CanUseCompanyServicePointModel.ServiceType);
        //        retValue.ResultCode = APIResult.enumResult.CompanyPointError;
        //        retValue.Message = "尚未建立 " + ServiceTypeName + " 通道";
        //        return retValue;
        //    }


        //    //點數餘額檢查
        //    if (groupWithdrawalData.TotalAmount > (CanUseCompanyServicePointModel.CanUsePoint - CanUseCompanyServicePointModel.FrozenPoint))
        //    {
        //        retValue.ResultCode = APIResult.enumResult.CompanyPointError;
        //        string ServiceTypeName = backendDB.GetServiceTypeNameByServiceType(CanUseCompanyServicePointModel.ServiceType);
        //        retValue.Message = ServiceTypeName + " 通道可用餘額不足";
        //        return retValue;
        //    }
        //}
        #endregion

        #region 提款银行卡每日额度及提款次数(目前不使用)
        //var clientDatas = (from w in fromBody.WithdrawalData
        //                   group w by new
        //                   {
        //                       w.CurrencyType,
        //                       w.BankCard,
        //                       w.BankCardName,
        //                   }
        //       into gcs
        //                   select new DBModel.WithdrawalTotalAmountsByDate()
        //                   {
        //                       CurrencyType = gcs.Key.CurrencyType,
        //                       BankCard = gcs.Key.BankCard,
        //                       BankCardName = gcs.Key.BankCardName,
        //                       TotalCount = gcs.Count(),
        //                       Amount = gcs.Sum(s => s.Amount)
        //                   }).ToList();

        //var dbDatas = backendDB.GetWithdrawalTotalAmountsByDate();

        //if (dbDatas != null)
        //{
        //    foreach (var clientData in clientDatas)
        //    {
        //        var tmpData = dbDatas.Where(w => w.BankCard == clientData.BankCard && w.BankCardName == clientData.BankCardName && w.CurrencyType == clientData.CurrencyType).ToList();
        //        if (tmpData.Count > 0)
        //        {
        //            clientData.Amount += tmpData.First().Amount;
        //            clientData.TotalCount += tmpData.First().TotalCount;
        //        }
        //    }
        //}

        //string errorMessageByCount = "";
        //string errorMessageByAmount = "";
        //foreach (var clientData in clientDatas)
        //{
        //    if (clientData.TotalCount > 15)
        //    {
        //        errorMessageByCount += clientData.BankCard + ",";
        //    }

        //    if (clientData.Amount > 200000)
        //    {
        //        errorMessageByAmount += clientData.BankCard + ",";
        //    }
        //}
        //if (!string.IsNullOrEmpty(errorMessageByCount))
        //{
        //    errorMessageByCount = errorMessageByCount.Substring(0, errorMessageByCount.Length - 1);
        //}

        //if (!string.IsNullOrEmpty(errorMessageByAmount))
        //{
        //    errorMessageByAmount = errorMessageByAmount.Substring(0, errorMessageByAmount.Length - 1);
        //}

        //if (!string.IsNullOrEmpty(errorMessageByAmount))
        //{
        //    retValue.ResultCode = APIResult.enumResult.CompanyPointError;
        //    retValue.Message = "銀行卡:" + errorMessageByAmount + "當日匯款金額已達上限";
        //    return retValue;
        //}

        //if (!string.IsNullOrEmpty(errorMessageByCount))
        //{
        //    retValue.ResultCode = APIResult.enumResult.CompanyPointError;
        //    retValue.Message = "銀行卡:" + errorMessageByCount + "匯款次數已達上限，請更換銀行戶頭";
        //    return retValue;
        //} 
        #endregion
        for (int i = 0; i < fromBody.WithdrawalData.Count; i++)
        {

            if (withdrawLimitResult != null)
            {
                tmpWithdrawLimitResult = withdrawLimitResult.Where(w => w.CurrencyType == fromBody.WithdrawalData[i].CurrencyType && w.ServiceType == fromBody.WithdrawalData[i].ServiceType).ToList();
                if (tmpWithdrawLimitResult.Count > 0)
                {
                    Charge = tmpWithdrawLimitResult.First().Charge;
                    WithdrawalRate = tmpWithdrawLimitResult.First().Rate;
                }
                else
                {
                    Charge = 0;
                    WithdrawalRate = 0;
                }

            }

            var spAddWithdrawalByBackendDownDataRet = backendDB.spAddWithdrawalByBackendDownData(0, AdminData.forCompanyID, "", fromBody.WithdrawalData[i].CurrencyType, fromBody.WithdrawalData[i].ServiceType, fromBody.WithdrawalData[i].Amount, Charge, WithdrawalRate, 0, 0, fromBody.WithdrawalData[i].BankCard, fromBody.WithdrawalData[i].BankCardName, fromBody.WithdrawalData[i].BankName, fromBody.WithdrawalData[i].BankBranchName, fromBody.WithdrawalData[i].OwnProvince, fromBody.WithdrawalData[i].OwnCity, 0, 0);

            if (spAddWithdrawalByBackendDownDataRet != 0)
            {
                DBretValue++;
            }
        }


        if (DBretValue > 0)
        {
            string strWithdrawSerials = "";
            for (int i = 0; i < fromBody.WithdrawalData.Count; i++)
            {
                strWithdrawSerials += fromBody.WithdrawalData[i].WithdrawSerial + ",";
            }
            strWithdrawSerials = strWithdrawSerials.Substring(0, strWithdrawSerials.Length - 1);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "申请提现单:" + strWithdrawSerials, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
            retValue.Message = DBretValue.ToString();
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "目前没有可建立订单";
            return retValue;
        }

        return retValue;
    }


    [HttpGet]
    [HttpPost]
    [ActionName("WithdrawalRecord")]
    public CompanyPointHistoryResult WithdrawalRecord([FromBody] FromBody.WithdrawalPostSet fromBody)
    {
        CompanyPointHistoryResult retValue = new CompanyPointHistoryResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        var DbReturn = backendDB.WithdrawalRecord(fromBody.WithdrawalSerial);

        if (DbReturn != null)
        {
            retValue.CompanyPointHistoryDates = DbReturn;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("TmpWithdrawalCreate")]
    public WithdrawalTableResult TmpWithdrawalCreate([FromBody] FromBody.WithdrawalCreate fromBody)
    {
        List<DBModel.Withdrawal> dbReturn = null;
        WithdrawalTableResult retValue = new WithdrawalTableResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        dbReturn = backendDB.TmpWithdrawalCreate(fromBody, AdminData.forCompanyID);

        if (dbReturn.Count > 0)
        {
            retValue.WithdrawalResults = dbReturn;
            retValue.ResultCode = APIResult.enumResult.OK;
            string strWithdrawSerials = "";
            for (int i = 0; i < dbReturn.Count; i++)
            {
                strWithdrawSerials += dbReturn[i].WithdrawSerial + ",";
            }
            strWithdrawSerials = strWithdrawSerials.Substring(0, strWithdrawSerials.Length - 1);

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "申请占存提现单:" + strWithdrawSerials, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "建单失败";
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("TmpWithdrawalUpdate")]
    public UpdateWithdrawalTableResult TmpWithdrawalUpdate([FromBody] FromBody.WithdrawalUpdate fromBody)
    {
        DBModel.Withdrawal dbReturn = null;
        UpdateWithdrawalTableResult retValue = new UpdateWithdrawalTableResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        dbReturn = backendDB.TmpWithdrawalUpdate(fromBody.WithdrawalData, AdminData.forCompanyID);

        if (dbReturn != null)
        {
            retValue.WithdrawalResult = dbReturn;
            retValue.ResultCode = APIResult.enumResult.OK;

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "修改占存提现单:" + dbReturn.WithdrawSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("WithdrawalUpdate")]
    public UpdateWithdrawalTableResult WithdrawalUpdate([FromBody] FromBody.WithdrawalUpdate fromBody)
    {
        DBModel.Withdrawal dbReturn = null;
        UpdateWithdrawalTableResult retValue = new UpdateWithdrawalTableResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        dbReturn = backendDB.WithdrawalUpdate(fromBody.WithdrawalData, AdminData.forCompanyID);

        if (dbReturn != null)
        {
            retValue.WithdrawalResult = dbReturn;
            retValue.ResultCode = APIResult.enumResult.OK;

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "重审提现单:" + dbReturn.WithdrawSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("GetWithdrawalTableResult")]
    public WithdrawalTableResult GetWithdrawalTableResult([FromBody] FromBody.WithdrawalSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalTableResult(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalTableResultByCompanyID")]
    public WithdrawalTableResult GetWithdrawalTableResultByCompanyID([FromBody] string BID)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        BackendDB backendDB = new BackendDB();

        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalTableResultByCompanyID(AdminData.forCompanyID);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalResultByWithdrawSerial")]
    public WithdrawalResultByWithdrawSerialResult GetWithdrawalResultByWithdrawSerial([FromBody] FromBody.WithdrawalSet fromBody)
    {
        //SignalR.Hubs.IHubContext C = SignalR.GlobalHost.ConnectionManager.GetHubContext<SkyPay.App_Code.HubClass>();
        //C.Clients.All.clientNoticeMessage("TEST");
        //var client = C.Clients.Client("MyHub");
        //if (client != null) {
        //    client.clientNoticeMessage("TEST2");
        //}

        WithdrawalResultByWithdrawSerialResult _WithdrawalTableResult = new WithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        //if (AdminData.CompanyType != 0) {
        //    _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
        //    return _WithdrawalTableResult;
        //}

        BackendDB backendDB = new BackendDB();
        DBModel.Withdrawal TableResult = backendDB.GetWithdrawalResultByWithdrawSerial(fromBody.WithdrawSerial);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResult = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("CheckHandleByAdmin")]
    public WithdrawalResultByWithdrawSerialResult CheckHandleByAdmin([FromBody] FromBody.WithdrawalSet fromBody)
    {

        WithdrawalResultByWithdrawSerialResult _WithdrawalTableResult = new WithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.Admin AdminModel;
        int GroupID = 0;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();
        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }

        //检查订单是否已转移至其他群组

        var WithdrawSerialModel = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);
        if (backendDB.CheckHandleByChangeGroup(fromBody.WithdrawSerial, GroupID) == 0)
        {

            if (WithdrawSerialModel.GroupID != GroupID)
            {   //群组已转移
                _WithdrawalTableResult.ResultCode = APIResult.enumResult.DataExist;
            }
            else if (WithdrawSerialModel.HandleByAdminID != 0)
            {    //已有人审核中
                _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
                _WithdrawalTableResult.WithdrawalResult = WithdrawSerialModel;
                _WithdrawalTableResult.Message = WithdrawSerialModel.RealName1;
            }
            else if (WithdrawSerialModel.Status != 1)
            {   //审核已完成
                _WithdrawalTableResult.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
            }
            else
            {
                _WithdrawalTableResult.ResultCode = APIResult.enumResult.Error;
            }

            return _WithdrawalTableResult;
        }



        //检查剩余额度是否足够
        var ProviderGroupModel = backendDB.GetProxyProviderGroupByGroupID(AdminData.CompanyCode, GroupID);

        var ProxyProviderModel = backendDB.GetProxyProviderByProviderCode(AdminData.CompanyCode);
        var FrozenPoint = backendDB.GetProxyProviderGroupFrozenPoint(AdminData.CompanyCode, GroupID);
        if (ProviderGroupModel == null || WithdrawSerialModel == null || ProxyProviderModel == null)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.CompanyPointError;
            return _WithdrawalTableResult;
        }

        var GetProviderWithdrawalByGroupAmountModel = backendDB.GetProviderWithdrawalByGroupAmount(GroupID);

        if (GetProviderWithdrawalByGroupAmountModel == null)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.CompanyPointError;
            return _WithdrawalTableResult;
        }

        if (WithdrawSerialModel.Amount + ProxyProviderModel.Charge + GetProviderWithdrawalByGroupAmountModel.TotalAmount + (GetProviderWithdrawalByGroupAmountModel.TotalCount * ProxyProviderModel.Charge) > (ProviderGroupModel.CanUsePoint - FrozenPoint))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.Other;
            _WithdrawalTableResult.Message = "<span style='font-size: 20px;font-weight: 500;'>可用金额:</span><span style='font-size: 20px;font-weight: 500;color:green'>" + (ProviderGroupModel.CanUsePoint - FrozenPoint).ToString("#.##") + "</span></br><span style='font-size: 20px;font-weight: 500;'>订单金额:</span><span style='font-size: 20px;font-weight: 500;color:brown'>" + (WithdrawSerialModel.Amount + ProxyProviderModel.Charge).ToString("#.##") + "</span></br><span style='font-size: 20px;font-weight: 500;'>出款中金额:</span><span style='font-size: 20px;font-weight: 500;color:blue'>" + (GetProviderWithdrawalByGroupAmountModel.TotalAmount + (GetProviderWithdrawalByGroupAmountModel.TotalCount * ProxyProviderModel.Charge)).ToString("#.##") + "</span>" + "</br><span style='font-size: 20px;font-weight: 500;'>处理后可用金额:</span><span style='font-size: 20px;font-weight: 500;color:red'>" + (ProviderGroupModel.CanUsePoint - FrozenPoint - (WithdrawSerialModel.Amount + ProxyProviderModel.Charge + GetProviderWithdrawalByGroupAmountModel.TotalAmount + (GetProviderWithdrawalByGroupAmountModel.TotalCount * ProxyProviderModel.Charge))).ToString("#.##") + "</span>";
            return _WithdrawalTableResult;
        }
        //修改订单审核人
        int checkHandleByAdmin = backendDB.ChangeWithdrawHandleByAdmin(fromBody.WithdrawSerial, AdminData.AdminID);
        if (checkHandleByAdmin > 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;
            DBModel.Withdrawal TableResult = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "订单出款,单号:" + TableResult.WithdrawSerial + ",出款人:" + TableResult.RealName1, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            _WithdrawalTableResult.WithdrawalResult = TableResult;
        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.Error;
        }

        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("ConfirmModifyBankCrad")]
    public WithdrawalResultByWithdrawSerialResult ConfirmModifyBankCrad([FromBody] FromBody.WithdrawalSet fromBody)
    {

        WithdrawalResultByWithdrawSerialResult _WithdrawalTableResult = new WithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();

        //检查剩余额度是否足够
        int confirmModifyBankCrad = backendDB.ConfirmModifyBankCrad(fromBody.WithdrawSerial, fromBody.BankDescription);
        if (confirmModifyBankCrad > 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;
            DBModel.Withdrawal TableResult = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);

            _WithdrawalTableResult.WithdrawalResult = TableResult;
        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.Error;
        }


        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("CancelCheckHandleByAdmin")]
    public WithdrawalResultByWithdrawSerialResult CancelCheckHandleByAdmin([FromBody] FromBody.WithdrawalSet fromBody)
    {

        WithdrawalResultByWithdrawSerialResult _WithdrawalTableResult = new WithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();
        //修改订单审核人
        int checkHandleByAdmin = backendDB.CancelCheckHandleByAdmin(fromBody.WithdrawSerial, AdminData.AdminID);
        if (checkHandleByAdmin > 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;
            DBModel.Withdrawal TableResult = backendDB.GetProviderWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "取消订单出款,单号:" + TableResult.WithdrawSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            _WithdrawalTableResult.WithdrawalResult = TableResult;
        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.Error;
        }



        return _WithdrawalTableResult;
    }

    //[HttpGet]
    //[HttpPost]
    //[ActionName("RemoveWithdrawal")]
    //public APIResult RemoveWithdrawal([FromBody] FromBody.WithdrawalPostSet fromBody)
    //{
    //    APIResult result = new APIResult();
    //    int DBreturn = -1;
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    }


    //    BackendDB backendDB = new BackendDB();
    //    DBreturn = backendDB.RemoveWithdrawal(fromBody.WithdrawID);
    //    if (DBreturn > 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.OK;
    //        BackendFunction backendFunction = new BackendFunction();
    //        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "删除占存提现单", IP, fingerprint);
    //        string XForwardIP = CodingControl.GetXForwardedFor();
    //        CodingControl.WriteXFowardForIP(AdminOP);
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    //[HttpPost]
    //[ActionName("RemoveAllWithdrawal")]
    //public APIResult RemoveAllWithdrawal([FromBody] FromBody.RemoveAllWithdrawal fromBody)
    //{
    //    APIResult result = new APIResult();
    //    int DBreturn = -1;
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
    //    }


    //    BackendDB backendDB = new BackendDB();
    //    DBreturn = backendDB.RemoveAllWithdrawal(fromBody.WithdrawIDs);
    //    if (DBreturn > 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.OK;
    //        BackendFunction backendFunction = new BackendFunction();
    //        string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
    //        int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "删除占存提现单", IP, fingerprint);
    //        string XForwardIP = CodingControl.GetXForwardedFor();
    //        CodingControl.WriteXFowardForIP(AdminOP);
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    [HttpPost]
    [ActionName("UpdateWithdrawalResultByWithdrawSerial")]
    public APIResult UpdateWithdrawalResultByWithdrawSerial([FromBody] FromBody.WithdrawalSet fromBody)
    {
        UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 4))
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();
        result.WithdrawalResult = backendDB.UpdateWithdrawalResultByWithdrawSerial(fromBody.Status, fromBody.WithdrawSerial, AdminData.AdminID, fromBody.ProviderCode, fromBody.WithdrawType, fromBody.ServiceType);
        if (result.WithdrawalResult.Status >= 0)
        {
            string strStatus = "";
            string strProviderName = "";
            string strServiceTypeName = "";
            string strWithdrawType = "";
            if (fromBody.Status == 3)
            {
                strStatus = "失败";
            }
            else
            {
                strStatus = "成功";
            }

            if (fromBody.WithdrawType == 0)
            {
                strWithdrawType = "人工";
            }
            else
            {
                strWithdrawType = "API付款";
            }
            strServiceTypeName = fromBody.ServiceType;
            strProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 5, "审核提现单,单号:" + fromBody.WithdrawSerial + ",审核状态:" + strStatus + ",供应商:" + strProviderName + ",付款方式:" + strWithdrawType + ",支付通道:" + strServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }


    [HttpPost]
    [ActionName("ConfirmManualWithdrawal")]
    public APIResult ConfirmManualWithdrawal([FromBody] FromBody.WithdrawalSet fromBody)
    {
        UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();


        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }


        result.WithdrawalResult = backendDB.ConfirmManualWithdrawal(fromBody.WithdrawSerial, fromBody.Status, AdminData.AdminID);
        if (result.WithdrawalResult.Status >= 0)
        {
            string strStatus = "";
            switch (fromBody.Status)
            {
                case 0:
                    strStatus = "取消";
                    break;
                case 2:
                    strStatus = "成功";
                    break;
                case 3:
                    strStatus = "失败";
                    break;
                default:
                    break;
            }
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 5, "人工审核确认,单号:" + fromBody.WithdrawSerial + ",审核状态:" + strStatus, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpPost]
    [ActionName("ConfirmManualWithdrawalForProxyProivder")]
    public APIResult ConfirmManualWithdrawalForProxyProivder([FromBody] FromBody.WithdrawalSet fromBody)
    {
        UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBViewModel.AdminWithKey AdminModel;
        int GroupID = 0;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();

        AdminModel = backendDB.GetAdminByLoginAdminIDWithKey(AdminData.AdminID);

        if (fromBody.Status == 3)
        {
            if (string.IsNullOrEmpty(AdminModel.GoogleKey))
            {
                result.Message = "尚未绑定 Google 验证器";
                result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
                return result;

            }
            else
            {
                //檢查google認證
                BackendFunction backendFunction = new BackendFunction();
                if (!backendFunction.CheckGoogleKey(AdminModel.GoogleKey, fromBody.UserKey))
                {
                    result.Message = " Google 验证有误";
                    result.ResultCode = APIResult.enumResult.GoogleKeyError;
                    return result;
                }
            }
        }

        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }
        //检查剩余额度是否足够
        var ProviderGroupModel = backendDB.GetProxyProviderGroupByGroupID(AdminData.CompanyCode, GroupID);
        var WithdrawSerialModel = backendDB.GetWithdrawalByWithdrawSerial(fromBody.WithdrawSerial);
        var ProxyProviderModel = backendDB.GetProxyProviderByProviderCode(AdminData.CompanyCode);
        var FrozenPoint = backendDB.GetProxyProviderGroupFrozenPoint(AdminData.CompanyCode, GroupID);
        if (ProviderGroupModel == null || WithdrawSerialModel == null || ProxyProviderModel == null)
        {
            result.ResultCode = APIResult.enumResult.CompanyPointError;
            return result;
        }

        if (WithdrawSerialModel.Amount + ProxyProviderModel.Charge > ProviderGroupModel.CanUsePoint - FrozenPoint)
        {
            result.ResultCode = APIResult.enumResult.Other;
            return result;
        }


        result.WithdrawalResult = backendDB.ConfirmManualWithdrawalForProxyProivder(AdminData.forCompanyID, fromBody.WithdrawSerial, fromBody.Status, AdminData.AdminID, fromBody.BankDescription);
        if (result.WithdrawalResult.Status >= 0)
        {
            string strStatus = "";
            switch (fromBody.Status)
            {
                case 0:
                    strStatus = "取消";
                    break;
                case 2:
                    strStatus = "成功";
                    break;
                case 3:
                    strStatus = "失败";
                    break;
                default:
                    break;
            }

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 5, "人工审核确认,单号:" + fromBody.WithdrawSerial + ",审核状态:" + strStatus, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpPost]
    [ActionName("ConfirmAutoWithdrawal")]
    public APIResult ConfirmAutoWithdrawal([FromBody] FromBody.WithdrawalSet fromBody)
    {
        UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();
        result.WithdrawalResult = backendDB.ConfirmAutoWithdrawal(fromBody.WithdrawSerial);
        if (result.WithdrawalResult.Status >= 0)
        {

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 5, "查询代付单状态:" + fromBody.WithdrawSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    [HttpPost]
    [ActionName("QueryAPIWithdrawal")]
    public APIResult QueryAPIWithdrawal([FromBody] FromBody.WithdrawalSet fromBody)
    {
        UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        BackendDB backendDB = new BackendDB();
        result.WithdrawalResult = backendDB.QueryAPIWithdrawal(fromBody.WithdrawSerial);
        if (result.WithdrawalResult.Status >= 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 5, "查询代付单状态:" + fromBody.WithdrawSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }
    //[HttpPost]
    //[ActionName("UpdateWithdrawalResultsByAdmin")]
    //public WithdrawalTableResult UpdateWithdrawalResultsByAdmin([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    WithdrawalTableResult result = new WithdrawalTableResult();
    //    List<DBModel.Withdrawal> WithdrawalResult;
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    WithdrawalResult = backendDB.UpdateWithdrawalResultsByAdmin(4, fromBody.WithdrawIDs, AdminData.AdminID, fromBody.ProviderCode);
    //    if (WithdrawalResult != null)
    //    {
    //        result.WithdrawalResults = WithdrawalResult;
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    //[HttpPost]
    //[ActionName("UpdateWithdrawalResultsByAdminCheck")]
    //public UpdateWithdrawalResultsByAdminCheckModel UpdateWithdrawalResultsByAdminCheck([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    UpdateWithdrawalResultsByAdminCheckModel result = new UpdateWithdrawalResultsByAdminCheckModel();
    //    DBModel.UpdateWithdrawalResultsByAdminCheck WithdrawalResult;
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    //var CompanyData = backendDB.GetCompanyByID(AdminData.forCompanyID);

    //    //檢查google認證
    //    //if (string.IsNullOrEmpty(CompanyData.GoogleKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
    //    //    return result;
    //    //}

    //    //if (!backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyError;
    //    //    return result;
    //    //}

    //    WithdrawalResult = backendDB.UpdateWithdrawalResultsByAdminCheck(fromBody.Status, fromBody.WithdrawIDs, AdminData.AdminID);
    //    if (WithdrawalResult.State == 0)
    //    {
    //        result.WithdrawalResult = WithdrawalResult;
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    //[HttpPost]
    //[ActionName("UpdateWithdrawalResultsByAdminDoubleCheck")]
    //public UpdateWithdrawalResultsByAdminCheckModel UpdateWithdrawalResultsByAdminDoubleCheck([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    UpdateWithdrawalResultsByAdminCheckModel result = new UpdateWithdrawalResultsByAdminCheckModel();
    //    DBModel.UpdateWithdrawalResultsByAdminCheck WithdrawalResult;
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    //var CompanyData = backendDB.GetCompanyByID(AdminData.forCompanyID);

    //    //檢查google認證
    //    //if (string.IsNullOrEmpty(CompanyData.GoogleKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
    //    //    return result;
    //    //}

    //    //if (!backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyError;
    //    //    return result;
    //    //}

    //    WithdrawalResult = backendDB.UpdateWithdrawalResultsByAdminDoubleCheck(fromBody.WithdrawIDs);
    //    if (WithdrawalResult.State == 0)
    //    {
    //        result.WithdrawalResult = WithdrawalResult;
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    //[HttpPost]
    //[ActionName("UpdateWithdrawalResultByWithdrawSerialCheck")]
    //public APIResult UpdateWithdrawalResultByWithdrawSerialCheck([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    //BackendFunction backendFunction = new BackendFunction();

    //    //var CompanyData = backendDB.GetCompanyByID(AdminData.forCompanyID);

    //    //檢查google認證
    //    //if (string.IsNullOrEmpty(CompanyData.GoogleKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
    //    //    return result;
    //    //}

    //    //if (!backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey)) {
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyError;
    //    //    return result;
    //    //}

    //    result.WithdrawalResult = backendDB.UpdateWithdrawalResultByWithdrawSerialCheck(fromBody.Status, fromBody.WithdrawSerial, AdminData.AdminID);
    //    if (!string.IsNullOrEmpty(result.WithdrawalResult.Message))
    //    {
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    //[HttpPost]
    //[ActionName("UpdateWithdrawalResultByWithdrawSerialDoubleCheck")]
    //public APIResult UpdateWithdrawalResultByWithdrawSerialDoubleCheck([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    UpdateWithdrawalResultByWithdrawSerialResult result = new UpdateWithdrawalResultByWithdrawSerialResult();
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        result.ResultCode = APIResult.enumResult.SessionError;
    //        return result;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        result.ResultCode = APIResult.enumResult.VerificationError;
    //        return result;
    //    }

    //    BackendDB backendDB = new BackendDB();
    //    BackendFunction backendFunction = new BackendFunction();

    //    var CompanyData = backendDB.GetCompanyByID(AdminData.forCompanyID);

    //    //檢查google認證
    //    //if (string.IsNullOrEmpty(CompanyData.GoogleKey))
    //    //{
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyEmpty;
    //    //    return result;
    //    //}

    //    //if (!backendFunction.CheckGoogleKey(CompanyData.GoogleKey, fromBody.UserKey))
    //    //{
    //    //    result.ResultCode = APIResult.enumResult.GoogleKeyError;
    //    //    return result;
    //    //}

    //    result.WithdrawalResult = backendDB.UpdateWithdrawalResultByWithdrawSerialDoubleCheck(fromBody.WithdrawSerial);
    //    if (result.WithdrawalResult.Message == "審核完成")
    //    {
    //        result.ResultCode = APIResult.enumResult.OK;
    //    }
    //    else
    //    {
    //        result.ResultCode = APIResult.enumResult.Error;
    //    }
    //    return result;
    //}

    [HttpPost]
    [ActionName("GetWithdrawalV2")]
    public DBModel.returnWithdrawalV2 GetWithdrawalV2(FromBody.WithdrawalSetV2 fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.returnWithdrawalV2 _PaymentTableResult = new DBModel.returnWithdrawalV2();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {

        }
        else if (AdminData.CompanyType == 4)
        {
            fromBody.CompanyID = AdminData.forCompanyID;
        }
        else
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();

        List<DBModel.WithdrawalV2> _Table = backendDB.GetWithdrawalV2(fromBody);

        if (_Table != null)
        {
            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = _Table.First().TotalCount;
            _PaymentTableResult.recordsFiltered = _Table.First().TotalCount;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = _Table;//分頁後的資料 

            var getWithdrawalBySearchFilter = backendDB.GetWithdrawalBySearchFilter(fromBody, true);
            if (getWithdrawalBySearchFilter != null)
            {
                _PaymentTableResult.TotalAmount = getWithdrawalBySearchFilter.TotalAmount;
                _PaymentTableResult.TotalHandlingFee = getWithdrawalBySearchFilter.TotalHandlingFee;
            }
            else
            {
                _PaymentTableResult.TotalAmount = 0;
                _PaymentTableResult.TotalHandlingFee = 0;
            }

            //DBModel.StatisticsPaymentAmount DbReturn = backendDB.GetPaymentPointBySearchFilter(fromBody);
            //if (DbReturn != null)
            //{
            //    _PaymentTableResult.StatisticsPaymentAmount = DbReturn;
            //}

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = 0;
            _PaymentTableResult.recordsFiltered = 0;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = new List<DBModel.WithdrawalV2>();//分頁後的資料 

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.NoData;
        }

        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalV3")]
    public DBModel.returnWithdrawalV2 GetWithdrawalV3(FromBody.WithdrawalSetV2 fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.returnWithdrawalV2 _PaymentTableResult = new DBModel.returnWithdrawalV2();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.SessionError;
            return _PaymentTableResult;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        if (AdminData.CompanyType == 0)
        {

        }
        else if (AdminData.CompanyType == 4)
        {
            fromBody.CompanyID = AdminData.forCompanyID;
        }
        else
        {
            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.VerificationError;
            return _PaymentTableResult;
        }

        BackendDB backendDB = new BackendDB();

        List<DBModel.WithdrawalV2> _Table = backendDB.GetWithdrawalV3(fromBody);

        if (_Table != null)
        {
            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = _Table.First().TotalCount;
            _PaymentTableResult.recordsFiltered = _Table.First().TotalCount;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = _Table;//分頁後的資料 

            var getWithdrawalBySearchFilter = backendDB.GetWithdrawalBySearchFilter(fromBody, false);
            if (getWithdrawalBySearchFilter != null)
            {
                _PaymentTableResult.TotalAmount = getWithdrawalBySearchFilter.TotalAmount;
                _PaymentTableResult.TotalHandlingFee = getWithdrawalBySearchFilter.TotalHandlingFee;
            }
            else
            {
                _PaymentTableResult.TotalAmount = 0;
                _PaymentTableResult.TotalHandlingFee = 0;
            }

            //DBModel.StatisticsPaymentAmount DbReturn = backendDB.GetPaymentPointBySearchFilter(fromBody);
            //if (DbReturn != null)
            //{
            //    _PaymentTableResult.StatisticsPaymentAmount = DbReturn;
            //}

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.OK;
        }
        else
        {
            _PaymentTableResult.draw = fromBody.draw;
            _PaymentTableResult.recordsTotal = 0;
            _PaymentTableResult.recordsFiltered = 0;
            _PaymentTableResult.IsAutoLoad = fromBody.IsAutoLoad;
            _PaymentTableResult.data = new List<DBModel.WithdrawalV2>();//分頁後的資料 

            _PaymentTableResult.ResultCode = (int)APIResult.enumResult.NoData;
        }

        return _PaymentTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalAdminTableResult")]
    public WithdrawalTableResult GetWithdrawalAdminTableResult([FromBody] FromBody.WithdrawalSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }


        BackendDB backendDB = new BackendDB();
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalAdminTableResult(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalAdminTableResult2")]
    public WithdrawalTableResult GetWithdrawalAdminTableResult2([FromBody] FromBody.WithdrawalSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }


        BackendDB backendDB = new BackendDB();
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalAdminTableResult2(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalAdminTableResultForProvider")]
    public WithdrawalTableResult GetWithdrawalAdminTableResultForProvider([FromBody] FromBody.WithdrawalSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.Admin AdminModel;
        int GroupID = 0;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();

        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }

        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalAdminTableResultForProvider(fromBody, AdminData.CompanyCode, GroupID);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("OnlySearchWithdrawalForProvider")]
    public WithdrawalTableResult OnlySearchWithdrawalForProvider([FromBody] FromBody.WithdrawalSet fromBody)
    {
        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();


        List<DBModel.Withdrawal> TableResult = backendDB.OnlySearchWithdrawalForProvider(fromBody, AdminData.CompanyCode);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalTableResultByLstStatus")]
    public WithdrawalTableResult GetWithdrawalTableResultByLstStatus([FromBody] FromBody.WithdrawalSet fromBody)
    {

        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }


        BackendDB backendDB = new BackendDB();
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalTableResultByLstStatus(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetProviderWithdrawalTableResultByStatus")]
    public WithdrawalTableResult GetProviderWithdrawalTableResultByStatus([FromBody] string BID)
    {

        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.Admin AdminModel;
        int GroupID = 0;

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();
        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);
        if (AdminModel != null)
        {
            GroupID = AdminModel.GroupID;
        }

        List<DBModel.Withdrawal> TableResult = backendDB.GetProviderWithdrawalTableResultByStatus(AdminData.CompanyCode, GroupID);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("OnlySearchProviderWithdrawalTableResultByStatus")]
    public WithdrawalTableResult OnlySearchProviderWithdrawalTableResultByStatus([FromBody] string BID)
    {

        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        DBModel.Admin AdminModel;

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        if (AdminData.CompanyType != 3)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }

        BackendDB backendDB = new BackendDB();
        AdminModel = backendDB.GetAdminByLoginAdminID(AdminData.AdminID);

        List<DBModel.Withdrawal> TableResult = backendDB.OnlySearchProviderWithdrawalTableResultByStatus(AdminData.CompanyCode);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    [HttpPost]
    [ActionName("GetWithdrawalTableResultByLstWithdrawID")]
    public WithdrawalTableResult GetWithdrawalTableResultByLstWithdrawID([FromBody] FromBody.WithdrawalSet fromBody)
    {

        WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawalTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawalTableResult;
        }


        BackendDB backendDB = new BackendDB();
        List<DBModel.Withdrawal> TableResult = backendDB.GetWithdrawalTableResultByLstWithdrawID(fromBody);
        if (TableResult != null)
        {
            _WithdrawalTableResult.WithdrawalResults = TableResult;
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawalTableResult;
    }

    //[HttpPost]
    //[ActionName("GetetWithdrawalAdminTableResultByCashier")]
    //public WithdrawalTableResult GetetWithdrawalAdminTableResultByCashier([FromBody] FromBody.WithdrawalSet fromBody)
    //{
    //    WithdrawalTableResult _WithdrawalTableResult = new WithdrawalTableResult();
    //    RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

    //    if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
    //    {
    //        _WithdrawalTableResult.ResultCode = APIResult.enumResult.SessionError;
    //        return _WithdrawalTableResult;
    //    }
    //    else
    //    {
    //        AdminData = (LoginResult)HttpContext.Current.Session["AdminData"];
    //    }

    //    if (AdminData.CompanyType != 0)
    //    {
    //        _WithdrawalTableResult.ResultCode = APIResult.enumResult.VerificationError;
    //        return _WithdrawalTableResult;
    //    }


    //    BackendDB backendDB = new BackendDB();
    //    List<DBModel.Withdrawal> TableResult = backendDB.GetetWithdrawalAdminTableResultByCashier(fromBody);
    //    if (TableResult != null)
    //    {
    //        _WithdrawalTableResult.WithdrawalResults = TableResult;
    //        _WithdrawalTableResult.ResultCode = APIResult.enumResult.OK;

    //    }
    //    else
    //    {
    //        _WithdrawalTableResult.ResultCode = APIResult.enumResult.NoData;
    //    }
    //    return _WithdrawalTableResult;
    //}
    #endregion

    #region 通知提示
    [HttpPost]
    [ActionName("GetBackendNotifyTableResult")]
    public BackendNotifyTableResult GetBackendNotifyTableResult([FromBody] string BID)
    {
        BackendNotifyTableResult _BackendNotifyTableResult = new BackendNotifyTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _BackendNotifyTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _BackendNotifyTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.BackendNotifyTable> bankCodeTableResult = backendDB.GetBackendNotifyTableResult(AdminData.forCompanyID);
        if (bankCodeTableResult != null)
        {
            _BackendNotifyTableResult.BackendNotifyResults = bankCodeTableResult;
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _BackendNotifyTableResult;
    }

    [HttpPost]
    [ActionName("GetBackendNotifyTableResult2")]
    public BackendNotifyTableResult GetBackendNotifyTableResult2([FromBody] string BID)
    {
        BackendNotifyTableResult _BackendNotifyTableResult = new BackendNotifyTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _BackendNotifyTableResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _BackendNotifyTableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.BackendNotifyTable> bankCodeTableResult = backendDB.GetBackendNotifyTableResult2(AdminData.forCompanyID, AdminData.AdminID);
        if (bankCodeTableResult != null)
        {
            _BackendNotifyTableResult.BackendNotifyResults = bankCodeTableResult;
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _BackendNotifyTableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _BackendNotifyTableResult;
    }

    #endregion

    #region 代付白名单
    [HttpPost]
    [ActionName("GetWithdrawalIPresult")]
    public WithdrawalIPTableResult GetWithdrawalIPresult([FromBody] FromBody.GetWithdrawalIPTableResult fromBody)
    {
        WithdrawalIPTableResult _TableResult = new WithdrawalIPTableResult();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _TableResult.ResultCode = APIResult.enumResult.SessionError;
            return _TableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.WithdrawalIP> data = backendDB.GetWithdrawalIPresult(fromBody.CompanyID);
        if (data != null)
        {
            _TableResult.Result = data;
            _TableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _TableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _TableResult;
    }

    [HttpPost]
    [ActionName("UpdateWithdrawalIPimg")]
    public APIResult UpdateWithdrawalIPimg([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        var returnDB = backendDB.UpdateImage(data.CompanyID, data.WithdrawalIP, data.ImageData, data.ImageName, data.Type, data.ImageID, 0);
        if (returnDB != "")
        {
            string ImageName = returnDB;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("InsertWithdrawalIP")]
    public APIResult InsertWithdrawalIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(data.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(data.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }
        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(data.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        if (backendDB.GetWithdrawalIP(data.CompanyID, data.WithdrawalIP) == null)
        {
            if (backendDB.InsertWithdrawalIP(data.CompanyID, data.WithdrawalIP, AdminData.AdminID) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商代付白名单 IP: " + data.WithdrawalIP
                , IP, fingerprint);

                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("DeleteWithdrawalIP")]
    public APIResult DeleteWithdrawalIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();



        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);
        //驗證權限
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(data.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }



        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(data.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.DeleteWithdrawalIP(data.CompanyID, data.WithdrawalIP) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "删除供应商代付白名单 IP: " + data.WithdrawalIP
            , IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("DeleteImageByImageID")]
    public APIResult DeleteImageByImageID([FromBody] FromBody.GetWithdrawalIPTableResult fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.DeleteImageByImageID(fromBody.ImageID) > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    #endregion

    #region 后台白名单
    [HttpPost]
    [ActionName("GetBackendIPresult")]
    public WithdrawalIPTableResult GetBackendIPresult([FromBody] FromBody.GetWithdrawalIPTableResult fromBody)
    {
        WithdrawalIPTableResult _TableResult = new WithdrawalIPTableResult();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _TableResult.ResultCode = APIResult.enumResult.SessionError;
            return _TableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.WithdrawalIP> data = backendDB.GetBackendIPresult(fromBody.CompanyID);
        if (data != null)
        {
            _TableResult.Result = data;
            _TableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _TableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _TableResult;
    }

    [HttpPost]
    [ActionName("UpdateBackendIPimg")]
    public APIResult UpdateBackendIPimg([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        var returnDB = backendDB.UpdateBackendIPImage(data.CompanyID, data.WithdrawalIP, data.ImageData, data.ImageName, data.Type, data.ImageID, 2);
        if (returnDB != "")
        {
            string ImageName = returnDB;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("InsertBackendIP")]
    public APIResult InsertBackendIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(data.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }
        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        if (backendDB.GetBackendIP(data.CompanyID, data.WithdrawalIP) == null)
        {
            if (backendDB.InsertBackendIP(data.CompanyID, data.WithdrawalIP) > 0)
            {
                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增后台白名单 IP: " + data.WithdrawalIP
                , IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("DeleteBackendIP")]
    public APIResult DeleteBackendIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(data.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }


        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.DeleteBackendIP(data.CompanyID, data.WithdrawalIP) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "删除后台白名单 IP: " + data.WithdrawalIP, IP, fingerprint
            );
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region 后台下发白名单
    [HttpPost]
    [ActionName("GetBackendWithdrawalIPresult")]
    public WithdrawalIPTableResult GetBackendWithdrawalIPresult([FromBody] FromBody.GetWithdrawalIPTableResult fromBody)
    {
        WithdrawalIPTableResult _TableResult = new WithdrawalIPTableResult();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _TableResult.ResultCode = APIResult.enumResult.SessionError;
            return _TableResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.WithdrawalIP> data = backendDB.GetBackendWithdrawalIPresult(fromBody.CompanyID);
        if (data != null)
        {
            _TableResult.Result = data;
            _TableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _TableResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _TableResult;
    }

    [HttpPost]
    [ActionName("UpdateBackendWithdrawalIPimg")]
    public APIResult UpdateBackendWithdrawalIPimg([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        var returnDB = backendDB.UpdateBackendWithdrawalIPimg(data.CompanyID, data.WithdrawalIP, data.ImageData, data.ImageName);
        if (returnDB != "")
        {
            string ImageName = returnDB;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("InsertBackendWithdrawalIP")]
    public APIResult InsertBackendWithdrawalIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        if (backendDB.GetBackendWithdrawalIP(data.CompanyID, data.WithdrawalIP) == null)
        {
            if (backendDB.InsertBackendWithdrawalIP(data.CompanyID, data.WithdrawalIP) > 0)
            {

                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增后台下发白名单 IP: " + data.WithdrawalIP
                , IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("DeleteBackendWithdrawalIP")]
    public APIResult DeleteBackendWithdrawalIP([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.DeleteBackendWithdrawalIP(data.CompanyID, data.WithdrawalIP) > 0)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "删除后台下发白名单 IP: " + data.WithdrawalIP, IP, fingerprint
            );
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region 供應商列表
    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderListResult")]
    public ProviderListResult GetProviderListResult([FromBody] string BID)
    {
        ProviderListResult retValue = new ProviderListResult();
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.ServiceData> ServiceDatas = null;
        List<DBViewModel.ProviderPointVM> ProviderPoints = null;
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.ProviderListResults = backendDB.GetProviderListResult();

        if (retValue.ProviderListResults != null)
        {
            ServiceDatas = backendDB.GetProviderListServiceData();
            ProviderPoints = backendDB.GetAllProviderPoint(true);

            foreach (var item in retValue.ProviderListResults)
            {
                if (ServiceDatas != null)
                {
                    item.ServiceDatas = ServiceDatas.Where(w => w.ProviderCode == item.ProviderCode).ToList();
                }

                if (ProviderPoints != null)
                {
                    item.ProviderListPoints = ProviderPoints.Where(w => w.ProviderCode == item.ProviderCode).ToList();
                }
            }

            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetAllProviderTotalResult")]
    public AllProviderTotalResult GetAllProviderTotalResult([FromBody] string BID)
    {
        AllProviderTotalResult retValue = new AllProviderTotalResult();
        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        retValue.AllProviderTotals = backendDB.GetAllProviderTotal().FirstOrDefault();

        if (retValue.AllProviderTotals != null)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }
    #endregion

    #region WithdrawLimit
    [HttpPost]
    [ActionName("GetWithdrawLimitResult")]
    public WithdrawLimitResult GetWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        WithdrawLimitResult _WithdrawLimitResult = new WithdrawLimitResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawLimitResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0 || AdminData.CompanyType == 4) && fromBody.WithdrawLimitType == 0)
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawLimitResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.WithdrawLimit> withdrawLimitResult = backendDB.GetWithdrawLimitResult(fromBody);
        if (withdrawLimitResult != null)
        {
            _WithdrawLimitResult.WithdrawLimits = withdrawLimitResult;
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawLimitResult;
    }

    [HttpPost]
    [ActionName("GetProviderBankCardResult")]
    public ProviderBankCardResult GetProviderBankCardResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        ProviderBankCardResult _WithdrawLimitResult = new ProviderBankCardResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawLimitResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0))
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawLimitResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.ProviderBankCard> withdrawLimitResult = backendDB.GetProviderBankCard(fromBody);
        if (withdrawLimitResult != null)
        {
            _WithdrawLimitResult.ProviderBankCards = withdrawLimitResult;
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawLimitResult;
    }

    [HttpPost]
    [ActionName("GetProviderWithdrawLimitResult")]
    public WithdrawLimitResult GetProviderWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        WithdrawLimitResult _WithdrawLimitResult = new WithdrawLimitResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.SessionError;
            return _WithdrawLimitResult;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (!(AdminData.CompanyType == 0))
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.VerificationError;
            return _WithdrawLimitResult;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.WithdrawLimit> withdrawLimitResult = backendDB.GetProviderWithdrawLimitResult(fromBody);
        if (withdrawLimitResult != null)
        {
            _WithdrawLimitResult.WithdrawLimits = withdrawLimitResult;
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _WithdrawLimitResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _WithdrawLimitResult;
    }

    [HttpPost]
    [ActionName("InsertProviderWithdrawLimitBlockChainRateResult")]
    public APIResult InsertProviderWithdrawLimitBlockChainRateResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        fromBody.WithdrawLimitType = 3;

        var boolCheckData = backendDB.CheckWithdrawLimitData(fromBody);
        //確認資料沒有重複
        if (!boolCheckData)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
            return retValue;
        }

        var DBreturn = backendDB.InsertProviderWithdrawLimitBlockChainRateResult(fromBody);
        if (DBreturn > 0)
        {
            string strProviderName = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商区块链代付限额,供应商名称:" + strProviderName + ",渠道代碼:" + fromBody.ServiceType + ",币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",浮动马数:" + fromBody.BlockChainRate, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProviderWithdrawLimitBlockChainRateResult")]
    public APIResult UpdateProviderWithdrawLimitBlockChainRateResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        fromBody.WithdrawLimitType = 3;

        var DBreturn = backendDB.UpdateProviderWithdrawLimitBlockChainRateResult(fromBody);
        if (DBreturn > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            string strProviderName = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商区块链代付限额,供应商名称:" + strProviderName + ",渠道代碼:" + fromBody.ServiceType + ",币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",区块链费率:" + fromBody.BlockChainRate, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertProviderWithdrawLimitResult")]
    public APIResult InsertProviderWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var boolCheckData = backendDB.CheckWithdrawLimitData(fromBody);
        //確認資料沒有重複
        if (!boolCheckData)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
            return retValue;
        }

        var DBreturn = backendDB.InsertProviderWithdrawLimitResult(fromBody);
        if (DBreturn > 0)
        {
            var strWithdrawLimitType = "";
            if (fromBody.WithdrawLimitType == 0)
            {
                strWithdrawLimitType = "後台下發";
            }
            else if (fromBody.WithdrawLimitType == 3)
            {
                strWithdrawLimitType = "API 代付";
            }
            string strProviderName = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商代付限额,供应商名称:" + strProviderName + ",類型:" + strWithdrawLimitType + ",币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",手续费:" + fromBody.Charge, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateProviderWithdrawLimitResult")]
    public APIResult UpdateProviderWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var DBreturn = backendDB.UpdateProviderWithdrawLimitResult(fromBody);
        if (DBreturn > 0)
        {
            var strWithdrawLimitType = "";
            if (fromBody.WithdrawLimitType == 0)
            {
                strWithdrawLimitType = "後台下發";
            }
            else if (fromBody.WithdrawLimitType == 3)
            {
                strWithdrawLimitType = "API 代付";
            }
            retValue.ResultCode = APIResult.enumResult.OK;
            string strProviderName = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(fromBody.ProviderCode);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "修改供应商代付限额,供应商名称:" + strProviderName + ",類型:" + strWithdrawLimitType + ",币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",手续费:" + fromBody.Charge, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("InsertCompanyWithdrawLimitResult")]
    public APIResult InsertCompanyWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        fromBody.ProviderCode = "";
        var boolCheckData = backendDB.CheckWithdrawLimitData(fromBody);
        //確認資料沒有重複
        if (!boolCheckData)
        {
            retValue.ResultCode = APIResult.enumResult.DataExist;
            return retValue;
        }

        var DBreturn = backendDB.InsertCompanyWithdrawLimitResult(fromBody);
        if (DBreturn > 0)
        {
            var strWithdrawLimitType = "";
            if (fromBody.WithdrawLimitType == 1)
            {
                strWithdrawLimitType = "後台下發";
            }
            else if (fromBody.WithdrawLimitType == 2)
            {
                strWithdrawLimitType = "API 代付";
            }
            string strCompanyName = "";
            string strServiceTypeName = "";
            strCompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);
            strServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增商户出款限额,商户名称:" + strCompanyName + ",類型:" + strWithdrawLimitType + ",币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",手续费(%):" + fromBody.Rate + ",手续费:" + fromBody.Charge + ",支付通道:" + strServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateCompanyWithdrawLimitResult")]
    public APIResult UpdateCompanyWithdrawLimitResult([FromBody] FromBody.WithdrawLimit fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType == 0)
        {
        }
        else if (AdminData.CompanyType == 4)
        {
            fromBody.CompanyID = AdminData.forCompanyID;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        fromBody.ProviderCode = "";

        var DBreturn = backendDB.UpdateCompanyWithdrawLimitResult(fromBody);
        if (DBreturn > 0)
        {
            var strWithdrawLimitType = "";
            if (fromBody.WithdrawLimitType == 1)
            {
                strWithdrawLimitType = "後台下發";
            }
            else if (fromBody.WithdrawLimitType == 2)
            {
                strWithdrawLimitType = "API 代付";
            }
            string strCompanyName = "";
            string strServiceTypeName = "";
            strCompanyName = backendDB.GetCompanyNameByCompanyID(fromBody.CompanyID);
            strServiceTypeName = backendDB.GetServiceTypeNameByServiceType(fromBody.ServiceType, fromBody.CurrencyType);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "修改商户提现限额,商户名称:" + strCompanyName + ",類型:" + strWithdrawLimitType + ", 币别:" + fromBody.CurrencyType + ",最低:" + fromBody.MinLimit + ",最高:" + fromBody.MaxLimit + ",手续费:" + fromBody.Charge + ",支付通道:" + strServiceTypeName, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }
    #endregion

    #region 操作Log

    [HttpPost]
    [ActionName("GetAdminOPLogResult")]
    public AdminOPLogResult GetAdminOPLogResult(FromBody.GetAdminOPLogResult fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AdminOPLogResult Result = new AdminOPLogResult();
        BackendDB backendDB = new BackendDB();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                Result.ResultCode = APIResult.enumResult.VerificationError;
                Result.Message = "";
                return Result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            Result.ResultCode = APIResult.enumResult.VerificationError;
            Result.Message = "";
            return Result;
        }


        List<DBViewModel.AdminOPLogVM> Table = backendDB.GetAdminOPLogResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetAdminOPLogResultByCompany")]
    public AdminOPLogResult GetAdminOPLogResultByCompany(FromBody.GetAdminOPLogResult fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        AdminOPLogResult Result = new AdminOPLogResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        fromBody.CompanyID = AdminData.forCompanyID;

        BackendDB backendDB = new BackendDB();
        List<DBViewModel.AdminOPLogVM> Table = backendDB.GetAdminOPLogResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }
    #endregion

    #region ManualHistory
    [HttpPost]
    [ActionName("GetProviderManualHistory")]
    public ProviderManualHistoryResult GetProviderManualHistory(FromBody.GetProviderManualHistory fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        ProviderManualHistoryResult Result = new ProviderManualHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {

        }



        BackendDB backendDB = new BackendDB();
        List<DBModel.ProviderManualHistory> Table = backendDB.GetProviderManualHistoryResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetOrderByCompanyManualHistoryByFrozenPoint")]
    public OrderByCompanyManualHistoryByFrozenPoint GetOrderByCompanyManualHistoryByFrozenPoint([FromBody] FromBody.TransactionPostSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        OrderByCompanyManualHistoryByFrozenPoint Result = new OrderByCompanyManualHistoryByFrozenPoint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        var Table = backendDB.GetOrderByCompanyManualHistoryByFrozenPoint(fromBody.TransactionSerial);

        if (Table != null)
        {
            Result.Result = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("GetOrderByCompanyManualHistory")]
    public OrderByCompanyManualHistoryResult GetOrderByCompanyManualHistory([FromBody] FromBody.TransactionPostSet fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        OrderByCompanyManualHistoryResult Result = new OrderByCompanyManualHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        var Table = backendDB.GetOrderByCompanyManualHistory(fromBody.TransactionSerial);

        if (Table != null)
        {
            Result.Result = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetCompanyManualHistory")]
    public CompanyManualHistoryResult GetCompanyManualHistory(FromBody.GetCompanyManualHistory fromBody)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        CompanyManualHistoryResult Result = new CompanyManualHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.CompanyManualHistory> Table = backendDB.GetCompanyManualHistory(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("InsertProviderManualHistory")]
    public APIResult InsertProviderManualHistory([FromBody] FromBody.ProviderManualHistory Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        //驗證權限

        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(Model.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(Model.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }
        var DBreturn = backendDB.InsertProviderManualHistory(Model, AdminData.AdminID);
        if (DBreturn == 0)
        {
            string strProviderName = "";
            string strType = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(Model.ProviderCode);
            if (Model.Type == 0)
            {
                strType = "代收";
            }
            else if (Model.Type == 1)
            {
                strType = "代付";
            }
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工提存-渠道金额修改,供应商名称:" + strProviderName + ",类型:" + strType + ",币别:" + Model.CurrencyType + ",额度:" + Model.Amount + ",对应单号:" + Model.TransactionSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            switch (DBreturn)
            {
                case -1:
                    retValue.Message = "後台操作人員不存在";
                    break;
                case -2:
                    retValue.Message = "供應商不存在";
                    break;
                case -3:
                    retValue.Message = "缺少相關說明";
                    break;
                case -4:
                    retValue.Message = "";
                    break;
                case -5:
                    retValue.Message = "鎖定失敗";
                    break;
                case -6:
                    retValue.Message = "異動記錄新增失敗";
                    break;
                case -7:
                    retValue.Message = "加扣點失敗";
                    break;
                case -8:
                    retValue.Message = "數值異常";
                    break;
                case -9:
                    retValue.Message = "供應商錢包不存在";
                    break;
                default:
                    retValue.Message = "其他錯誤";
                    break;
            }
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("InsertProviderManualHistoryByProfitAmount")]
    public APIResult InsertProviderManualHistoryByProfitAmount([FromBody] FromBody.ProviderManualHistory Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(Model.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(Model.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        if (Model.isModifyProfit)
        {
            Model.Amount = Model.Amount * -1;
            if (backendDB.UpdateProviderProfit(Model) > 0)
            {
                string strProviderName = "";
                string strType = "";
                strProviderName = backendDB.GetProviderNameByProviderCode(Model.ProviderCode);
                if (Model.Type == 0)
                {
                    strType = "代收";
                }
                else if (Model.Type == 1)
                {
                    strType = "代付";
                }

                BackendFunction backendFunction = new BackendFunction();
                string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
                int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工提存-毛利调整,供应商名称:" + strProviderName + ",类型:" + strType + ",币别:" + Model.CurrencyType + ",额度:" + Model.Amount, IP, fingerprint);
                string XForwardIP = CodingControl.GetXForwardedFor();
                CodingControl.WriteXFowardForIP(AdminOP);
                retValue.ResultCode = APIResult.enumResult.OK;
            }
            else
            {
                retValue.ResultCode = APIResult.enumResult.Error;
            }
            return retValue;
        }

        if (backendDB.InsertProviderManualHistoryByProfitAmount(Model, AdminData.AdminID) == 0)
        {
            string strProviderName = "";
            string strType = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(Model.ProviderCode);
            if (Model.Type == 0)
            {
                strType = "代收";
            }
            else if (Model.Type == 1)
            {
                strType = "代付";
            }

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工提存-毛利,供应商名称:" + strProviderName + ",类型:" + strType + ",币别:" + Model.CurrencyType + ",额度:" + Model.Amount + ",对应单号:" + Model.TransactionSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("InsertCompanyManualHistory")]
    public APIResult InsertCompanyManualHistory([FromBody] FromBody.CompanyManualHistory Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();


        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(Model.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(Model.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }
        var DBreturn = backendDB.InsertCompanyManualHistory(Model, AdminData.AdminID);
        if (DBreturn == 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            string strCompanyName = "";
            string strServiceTypeName = "";
            string strType = "";
            if (Model.Type == 0)
            {
                strType = "代收";
            }
            else if (Model.Type == 1)
            {
                strType = "代付";
            }
            strCompanyName = backendDB.GetCompanyNameByCompanyID(Model.forCompanyID);
            strServiceTypeName = backendDB.GetServiceTypeNameByServiceType(Model.ServiceType, Model.CurrencyType);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工提存-商户金额修改,商户名称:" + strCompanyName + ",类型:" + strType + ",币别:" + Model.CurrencyType + ",额度:" + Model.Amount + ",对应单号:" + Model.TransactionSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            switch (DBreturn)
            {
                case -1:
                    retValue.Message = "後台操作人員不存在";
                    break;
                case -2:
                    retValue.Message = "商戶錢包不存在";
                    break;
                case -3:
                    retValue.Message = "商戶支付方式有誤";
                    break;
                case -4:
                    retValue.Message = "缺少相關說明";
                    break;
                case -5:
                    retValue.Message = "鎖定失敗";
                    break;
                case -6:
                    retValue.Message = "異動記錄新增失敗";
                    break;
                case -7:
                    retValue.Message = "加扣點失敗";
                    break;
                default:
                    retValue.Message = "其他錯誤";
                    break;
            }
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("InsertManualHistory")]
    public APIResult InsertManualHistory([FromBody] FromBody.CompanyManualHistory Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();


        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(Model.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(Model.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }
        var DBreturn = backendDB.InsertManualHistory(Model, AdminData.AdminID);
        if (DBreturn == 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            string strCompanyName = "";
            string strServiceTypeName = "";
            string strType = "";
            if (Model.Type == 0)
            {
                strType = "代收";
            }
            else if (Model.Type == 1)
            {
                strType = "代付";
            }
            strCompanyName = backendDB.GetCompanyNameByCompanyID(Model.forCompanyID);
            var strProviderName = backendDB.GetProviderNameByProviderCode(Model.ProviderCode);
            strServiceTypeName = backendDB.GetServiceTypeNameByServiceType(Model.ServiceType, Model.CurrencyType);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工提存-金额修改,商户名称:" + strCompanyName + ",渠道名称:" + strProviderName + ",类型:" + strType + ",币别:" + Model.CurrencyType + ",额度:" + Model.Amount + ",对应单号:" + Model.TransactionSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            switch (DBreturn)
            {
                case -1:
                    retValue.Message = "後台操作人員不存在";
                    break;
                case -2:
                    retValue.Message = "商戶錢包不存在";
                    break;
                case -3:
                    retValue.Message = "商戶支付方式有誤";
                    break;
                case -4:
                    retValue.Message = "缺少相關說明";
                    break;
                case -5:
                    retValue.Message = "鎖定失敗";
                    break;
                case -6:
                    retValue.Message = "異動記錄新增失敗";
                    break;
                case -7:
                    retValue.Message = "加扣點失敗";
                    break;
                case -8:
                    retValue.Message = "供應商不存在";
                    break;
                case -9:
                    retValue.Message = "供應商錢包不存在";
                    break;
                case -10:
                    retValue.Message = "供應商加扣點失敗";
                    break;
                case -11:
                    retValue.Message = "供應商異動記錄新增失敗";
                    break;
                default:
                    retValue.Message = "其他錯誤";
                    break;
            }
        }

        return retValue;
    }
    #endregion

    #region 案件冻结

    [HttpPost]
    [ActionName("UpdateFrozenPointimg")]
    public APIResult UpdateFrozenPointimg([FromBody] FromBody.SetFrozenPointImageResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }
        var returnDB = backendDB.UpdateFrozenPointImg(data.FrozenID, data.ImageData, data.ImageName, data.Type, data.ImageID, 1);
        if (returnDB != "")
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }


        return retValue;
    }

    [HttpPost]
    [ActionName("InsertFrozenPoint")]
    public APIResult InsertFrozenPoint([FromBody] FromBody.FrozenPoint Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        int GroupID = 0;
        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(Model.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(Model.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        GroupID = backendDB.GetProxyProviderPaymentGroupID(Model.forPaymentSerial);
        Model.GroupID = GroupID;
        if (backendDB.InsertFrozenPoint(Model, Model.BoolActualProviderFrozenAmount, AdminData.AdminID) == 1)
        {
            string strCompanyName = "";
            string strProviderName = "";
            strProviderName = backendDB.GetProviderNameByProviderCode(Model.forProviderCode);
            strCompanyName = backendDB.GetCompanyNameByCompanyID(Model.forCompanyID);
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "冻结订单,单号:" + Model.forPaymentSerial + ",供应商:" + strProviderName + ",商户:" + strCompanyName + ",渠道凍結金額:" + Model.ProviderFrozenAmount + ",商户冻结金额:" + Model.CompanyFrozenAmount, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("ThawPoint")]
    public APIResult ThawPoint([FromBody] FromBody.ThawPoint fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        if (backendDB.ThawPoint(fromBody.FrozenID, AdminData.AdminID) == 1)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "订单解冻,单号:" + fromBody.FrozenID, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("GetCompanyFrozenPointHistory")]
    public FrozenPointHistoryResult GetCompanyFrozenPointHistory(FromBody.GetFrozenPointHistory fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        FrozenPointHistoryResult Result = new FrozenPointHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }
        fromBody.CompanyID = AdminData.forCompanyID;

        BackendDB backendDB = new BackendDB();
        List<DBModel.FrozenPointHistory> Table = backendDB.GetCompanyFrozenPointHistoryResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetFrozenPointHistory")]
    public FrozenPointHistoryResult GetFrozenPointHistory(FromBody.GetFrozenPointHistory fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        FrozenPointHistoryResult Result = new FrozenPointHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.FrozenPointHistory> Table = backendDB.GetFrozenPointHistoryResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetFrozenPointHistoryByProxyProvider")]
    public FrozenPointHistoryResult GetFrozenPointHistoryByProxyProvider(FromBody.GetFrozenPointHistory fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        FrozenPointHistoryResult Result = new FrozenPointHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 3)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        fromBody.ProviderCode = AdminData.CompanyCode;

        BackendDB backendDB = new BackendDB();
        List<DBModel.FrozenPointHistory> Table = backendDB.GetFrozenPointHistoryByProxyProvider(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("GetSumFrozenPoint")]
    public SumFrozenPointResult GetSumFrozenPoint([FromBody] FromBody.ProviderPostSet fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        SumFrozenPointResult Result = new SumFrozenPointResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            Result.ResultCode = APIResult.enumResult.VerificationError;
            return Result;
        }

        BackendDB backendDB = new BackendDB();
        DBModel.FrozenPointHistory Table = backendDB.GetSumFrozenPoint(fromBody.ProviderCode);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    #endregion

    #region 黑名單
    [HttpPost]
    [ActionName("InsertBlackList")]
    public APIResult InsertBlackList([FromBody] FromBody.BlackList Model)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(Model.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(Model.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.InsertBlackList(Model, AdminData.AdminID) == 1)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "新增黑名单,卡号" + Model.BankCard + ",持卡人:" + Model.BankCardName + ",IP:" + Model.UserIP, IP, fingerprint);
            RedisCache.BlackList.UpdateBlackList(Model.BankCard, Model.BankCardName);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetBlackListHistoryResult")]
    public BlackListHistoryResult GetBlackListHistory(FromBody.GetBlackList fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BlackListHistoryResult Result = new BlackListHistoryResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }
        else
        {

        }



        BackendDB backendDB = new BackendDB();
        List<DBModel.BlackList> Table = backendDB.GetBlackListResult(fromBody);

        if (Table != null)
        {
            Result.Results = Table;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    [HttpPost]
    [ActionName("CancelBlackList")]
    public APIResult CancelBlackList([FromBody] FromBody.BlackList fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (backendDB.CancelBlackList(fromBody.BlackListID, AdminData.AdminID) == 1)
        {
            var Model = backendDB.GetBlackListResult(fromBody.BlackListID);
            retValue.ResultCode = APIResult.enumResult.OK;
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 0, "解除黑名单,卡号" + Model.BankCard + ",持卡人:" + Model.BankCardName + ",IP:" + Model.UserIP, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }
    #endregion

    #region 利潤倉
    [HttpGet]
    [HttpPost]
    [ActionName("GetProviderProfitManualHistoryResult")]
    public ProviderProfitTableResult GetProviderProfitManualHistoryResult([FromBody] string BID)
    {
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        ProviderProfitTableResult Result = new ProviderProfitTableResult();

        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            Result.ResultCode = APIResult.enumResult.SessionError;
            return Result;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        BackendDB backendDB = new BackendDB();
        List<DBModel.ProviderManualHistory> ProviderManualHistory = backendDB.GetProviderProfitManualHistoryResult();

        if (ProviderManualHistory != null)
        {
            //Result.AgentReceiveTableResults = _AgentReceive;
            //Result.AgentAmountResult = backendDB.GetAgentAmountResult(fromBody);
            Result.ProviderProfitManualHistoryResult = ProviderManualHistory;
            Result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            Result.ResultCode = APIResult.enumResult.NoData;
        }
        return Result;
    }

    #endregion

    #region 供應商專卡
    [HttpPost]
    [ActionName("GetCompanyWhiteListProviderBankCard")]
    public GetCompanyWhiteListProviderBankCardResult GetCompanyWhiteListProviderBankCard(FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {

        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        GetCompanyWhiteListProviderBankCardResult retValue = new GetCompanyWhiteListProviderBankCardResult();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        BackendDB backendDB = new BackendDB();
        List<DBModel.CompanyWhiteList> Table = backendDB.GetCompanyWhiteListProviderBankCard(fromBody);

        if (Table != null)
        {
            retValue.Results = Table;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }
        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateCompanyWhiteListProviderBankCard")]
    public APIResult UpdateCompanyWhiteListProviderBankCard([FromBody] FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }


        DBretValue = backendDB.UpdateCompanyWhiteListProviderBankCard(fromBody);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "修改供應商專卡,白名單id :" + fromBody.WhiteListID + "卡片序號:" + fromBody.BankCardGUID + "描述:" + fromBody.Description + "用戶資訊:" + fromBody.UserIdentity, IP, fingerprint);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("InsertCompanyWhiteListProviderBankCard")]
    public APIResult InsertCompanyWhiteListProviderBankCard([FromBody] FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        DBretValue = backendDB.InsertCompanyWhiteListProviderBankCard(fromBody);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增供應商專卡,白名單id :" + fromBody.WhiteListID + "卡片序號:" + fromBody.BankCardGUID + "描述:" + fromBody.Description + "用戶資訊:" + fromBody.UserIdentity, IP, fingerprint);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "刪除失敗";
            return retValue;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }


    [HttpPost]
    [ActionName("DeleteCompanyWhiteListProviderBankCard")]
    public APIResult DeleteCompanyWhiteListProviderBankCard([FromBody] FromBody.GetCompanyWhiteListProviderBankCard fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }


        DBretValue = backendDB.DeleteCompanyWhiteListProviderBankCard(fromBody);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "刪除供應商專卡,白名單id :" + fromBody.WhiteListID, IP, fingerprint);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "刪除失敗";
            return retValue;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    #endregion

    #region 支付寶
    [HttpGet]
    [HttpPost]
    [ActionName("GetQrCodeContent")]
    public APIResult GetQrCodeContent([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        string qrContent = DecodeQrCode(data.ImageData);

        if (!string.IsNullOrEmpty(qrContent))
        {
            retValue.Message = qrContent;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.Message = "沒有偵測到 QRCode";
            retValue.ResultCode = APIResult.enumResult.Error;
        }
        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("UpdateAlipayImage")]
    public APIResult UpdateAlipayImage([FromBody] FromBody.GetWithdrawalIPTableResult data)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();

        //驗證權限
        RedisCache.BIDContext.BIDInfo AdminData;
        if (!RedisCache.BIDContext.CheckBIDExist(data.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        AdminData = RedisCache.BIDContext.GetBIDInfo(data.BID);

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        var returnDB = backendDB.UpdateAlipayImage(data.CompanyID, data.ImageData, data.ImageName, data.Type, data.ImageID, 3);
        if (returnDB != "")
        {
            string ImageName = returnDB;
            retValue.Message = ImageName;
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("InsertProviderAlipay")]
    public APIResult InsertProviderAlipay([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                retValue.ResultCode = APIResult.enumResult.VerificationError;
                retValue.Message = "";
                return retValue;
            }
        }

        //0 = 系統商/ 1 = 一般營運商 / 2 =代理營運商
        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            retValue.Message = "";
            return retValue;
        }

        var ret = backendDB.InsertProviderAlipay(fromBody);

        if (!string.IsNullOrEmpty(ret))
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "新增供应商支付寶:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("UpdateProviderAlipay")]
    public APIResult UpdateProviderAlipay([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult result = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        var returnDB = backendDB.UpdateProviderAlipay(fromBody);
        if (returnDB == 1)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "調整供应商支付寶:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
            result.Message = "其他错误";
        }

        return result;
    }

    [HttpGet]
    [HttpPost]
    [ActionName("DeleteProviderAlipay")]
    public APIResult DeleteProviderAlipay([FromBody] FromBody.ProviderBankCardData fromBody)
    {
        APIResult result = new APIResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;
                result.Message = "";
                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;
            result.Message = "";
            return result;
        }

        var returnDB = backendDB.DeleteProviderBankCard(fromBody);
        if (returnDB == 1)
        {
            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 2, "刪除供应商支付寶:" + fromBody.BankNumber, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
            result.Message = "其他错误";
        }

        return result;
    }

    public static string DecodeQrCode(byte[] imageData)
    {
        try
        {
            using (var ms = new MemoryStream(imageData))
            using (var bitmap = new Bitmap(ms))
            {
                var reader = new BarcodeReader
                {
                    AutoRotate = true,
                    TryInverted = true // 有些 QRCode 是反色的
                };

                var result = reader.Decode(bitmap);
                if (result != null)
                {
                    return result.Text; // QRCode 的內容
                }
                else
                {
                    return null; // 沒有辨識到 QRCode
                }
            }
        }
        catch (Exception ex)
        {
            // Log 錯誤
            return null;
        }
    }
    #endregion

    [HttpGet]
    [HttpPost]
    [ActionName("GetAgentPointResult")]
    public AgentReceiveResult GetAgentPointResult([FromBody] FromBody.Company fromBody)
    {
        AgentReceiveResult _AgentPointResult = new AgentReceiveResult();

        BackendDB backendDB = new BackendDB();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _AgentPointResult.ResultCode = APIResult.enumResult.SessionError;
            return _AgentPointResult;
        }

        _AgentPointResult.AgentReceiveResults = backendDB.GetCompanyAgentPointResult(fromBody.CompanyID, fromBody.CurrencyType);


        if (_AgentPointResult.AgentReceiveResults != null)
        {
            _AgentPointResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _AgentPointResult.ResultCode = APIResult.enumResult.NoData;
        }
        return _AgentPointResult;
    }

    #region TestPage
    [HttpGet]
    [HttpPost]
    [ActionName("GetTestServiceType")]
    public TestPageCompanyServiceResult GetTestServiceType([FromBody] string BID)
    {
        TestPageCompanyServiceResult retValue = new TestPageCompanyServiceResult();
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.ServiceTypeVM> DBretValue = new List<DBViewModel.ServiceTypeVM>();
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var CompanyTable = backendDB.GetCompanyByCode("VPayTest");
        if (CompanyTable != null)
        {
            retValue.ServiceTypes = backendDB.GetTestPageCompanyService(CompanyTable.CompanyID);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }


        return retValue;
    }

    public void SendPayment(decimal amount, string ServiceType, System.Web.UI.Page page)
    {
        BackendDB backendDB = new BackendDB();
        var CompanyModel = backendDB.GetCompanyWithKeyByCode("VPayTest");
        var CompanyCode = "VPayTest";
        var CurrencyType = "CNY";
        var ClientIP = "";
        var OrderID = Guid.NewGuid().ToString("N");
        var OrderDate = DateTime.Now;
        var OrderAmount = amount;
        var ReturnURL = "";
        var URL = "";


        if (Pay.IsTestSite)
        {
            //URL = "http://cn.richpay888.com:8088" + "/api/Gateway/RequirePayment";
            URL = "http://gpay.dev4.mts.idv.tw" + "/api/Gateway/RequirePayment";
            //http://43.129.228.251/api
            //URL = "http://43.129.228.251:80" + "/api/Gateway/RequirePayment";
        }
        else
        {
            URL = "https://www.richpay888.com" + "/api/Gateway/RequirePayment";
            //URL = "http://43.129.228.251:80" + "/api/Gateway/RequirePayment";
        }

        var Sign = CodingControl.GetGPaySign(OrderID, OrderAmount, OrderDate, ServiceType, CurrencyType, CompanyCode, CompanyModel.CompanyKey);

        System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();
        data.Add("CompanyCode", CompanyCode);
        data.Add("CurrencyType", CurrencyType);
        data.Add("ServiceType", ServiceType);
        data.Add("ClientIP", ClientIP);
        data.Add("OrderID", OrderID);
        data.Add("OrderDate", OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
        data.Add("OrderAmount", OrderAmount.ToString("#.##"));
        data.Add("ReturnURL", ReturnURL);
        data.Add("Sign", Sign);

        RedirectAndPOST(page, "http://DestUrl/Default.aspx", data);
    }
    #endregion

    #region TestPage2
    [HttpGet]
    [HttpPost]
    [ActionName("GetTestServiceType2")]
    public TestPageCompanyServiceResult GetTestServiceType2([FromBody] string BID)
    {
        TestPageCompanyServiceResult retValue = new TestPageCompanyServiceResult();
        BackendDB backendDB = new BackendDB();
        List<DBViewModel.ServiceTypeVM> DBretValue = new List<DBViewModel.ServiceTypeVM>();
        if (!RedisCache.BIDContext.CheckBIDExist(BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }

        var AdminData = RedisCache.BIDContext.GetBIDInfo(BID);

        //if (AdminData.CompanyType != 0)
        //{
        //    retValue.ResultCode = APIResult.enumResult.SessionError;
        //    return retValue;
        //}

        var CompanyTable = backendDB.GetCompanyByCode("test01");
        if (CompanyTable != null)
        {
            retValue.ServiceTypes = backendDB.GetTestPageCompanyService2(CompanyTable.CompanyID);
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.NoData;
        }


        return retValue;
    }

    public void SendPayment2(decimal amount, string ServiceType, System.Web.UI.Page page)
    {
        BackendDB backendDB = new BackendDB();
        var CompanyModel = backendDB.GetCompanyWithKeyByCode("test01");
        var CompanyCode = "test01";
        var CurrencyType = "CNY";
        var ClientIP = "";
        var OrderID = Guid.NewGuid().ToString("N");
        var OrderDate = DateTime.Now;
        var OrderAmount = amount;
        var ReturnURL = "";
        var URL = "";
        if (Pay.IsTestSite)
        {//http://cn.richpay888.com:8088
            ReturnURL = "http://gpay.dev4.mts.idv.tw" + "/api/ProviderResult/GPaySyncNotify?result=AAA";

        }
        else
        {
            ReturnURL = "https://www.richpay888.com" + "/api/ProviderResult/GPaySyncNotify?result=AAA";
        }

        if (Pay.IsTestSite)
        {
            URL = "http://cn.richpay888.com:8088" + "/api/Gateway/RequirePayment";
            //URL = "http://gpay.dev4.mts.idv.tw" +"/api/Gateway/RequirePayment"; 
        }
        else
        {
            URL = "https://www.richpay888.com" + "/api/Gateway/RequirePayment";
        }

        var Sign = CodingControl.GetGPaySign(OrderID, OrderAmount, OrderDate, ServiceType, CurrencyType, CompanyCode, CompanyModel.CompanyKey);

        System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();
        data.Add("CompanyCode", CompanyCode);
        data.Add("CurrencyType", CurrencyType);
        data.Add("ServiceType", ServiceType);
        data.Add("ClientIP", ClientIP);
        data.Add("OrderID", OrderID);
        data.Add("OrderDate", OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
        data.Add("OrderAmount", OrderAmount.ToString("#.##"));
        data.Add("ReturnURL", ReturnURL);
        data.Add("Sign", Sign);

        RedirectAndPOST(page, "http://DestUrl/Default.aspx", data);
    }
    #endregion

    #region RunPay

    [HttpPost]
    [ActionName("ConfirmManualPaymentForRunPay")]
    public APIResult ConfirmManualPaymentForRunPay([FromBody] FromBody.PaymentSet fromBody)
    {
        UpdatePatmentResultByPatmentSerialResult result = new UpdatePatmentResultByPatmentSerialResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendDB backendDB = new BackendDB();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            result.ResultCode = APIResult.enumResult.SessionError;
            return result;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (!Pay.IsTestSite)
        {
            if (!CodingControl.CheckXForwardedFor())
            {
                backendDB.InsertBotSendLog(AdminData.CompanyCode, "公司代碼:" + AdminData.CompanyCode + ",偵測到非反代IP活動：" + CodingControl.GetXForwardedFor());
                RedisCache.BIDContext.ClearBID(fromBody.BID);
                result.ResultCode = APIResult.enumResult.VerificationError;

                return result;
            }
        }

        if (AdminData.CompanyType != 0)
        {
            result.ResultCode = APIResult.enumResult.VerificationError;
            return result;
        }

        if (!backendDB.CheckLoginIP(CodingControl.GetUserIP(), AdminData.CompanyCode))
        {
            RedisCache.BIDContext.ClearBID(fromBody.BID);
            result.ResultCode = APIResult.enumResult.VerificationError;

            return result;
        }


        result.PatmentResult = backendDB.ConfirmManualPaymentForRunPay(fromBody.PaymentSerial, fromBody.ProcessStatus, AdminData.AdminID, fromBody.ProviderCode, fromBody.ServiceType, fromBody.GroupID);
        if (result.PatmentResult.Status >= 0)
        {

            BackendFunction backendFunction = new BackendFunction();
            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 1, "人工充值审核,单号:" + fromBody.PaymentSerial, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);

            result.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            result.ResultCode = APIResult.enumResult.Error;
        }
        return result;
    }

    #endregion

    #region BlockChainRate
    [HttpPost]
    [ActionName("UpdateBlockChainRate")]
    public APIResult UpdateBlockChainRate([FromBody] FromBody.BlockChainRate fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (fromBody.Rate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "区块链汇率必须 >= 0";
            return retValue;
        }

        if (fromBody.UpFloatRate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "上浮费率必须 >= 0";
            return retValue;
        }

        if (fromBody.DownFloatRate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "下浮费率必须 >= 0";
            return retValue;
        }

        if (fromBody.UpFloatRate > fromBody.DownFloatRate)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "下浮费率必须 > 上浮费率";
            return retValue;
        }

        DBretValue = backendDB.UpdateBlockChainRate(fromBody.Currency, fromBody.Rate, fromBody.DownFloatRate, fromBody.UpFloatRate);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "修改区块链汇率:" + fromBody.Rate + "下浮费率:" + fromBody.DownFloatRate + "上浮费率:" + fromBody.UpFloatRate + "币别:" + fromBody.Currency, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("CreateBlockChainRate")]
    public APIResult CreateBlockChainRate([FromBody] FromBody.BlockChainRate fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        if (fromBody.Rate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "区块链汇率必须 >= 0";
            return retValue;
        }

        if (fromBody.UpFloatRate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "上浮费率必须 >= 0";
            return retValue;
        }

        if (fromBody.DownFloatRate < 0)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "下浮费率必须 >= 0";
            return retValue;
        }

        if (fromBody.UpFloatRate > fromBody.DownFloatRate)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "下浮费率必须 > 上浮费率";
            return retValue;
        }

        DBretValue = backendDB.CreateBlockChainRate(fromBody.Currency, fromBody.Rate, fromBody.DownFloatRate, fromBody.UpFloatRate);

        if (DBretValue >= 1)
        {

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "新增区块链汇率:" + fromBody.Rate + "下浮费率:" + fromBody.DownFloatRate + "上浮费率:" + fromBody.UpFloatRate + "币别:" + fromBody.Currency, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else if (DBretValue == -1)
        {
            retValue.ResultCode = APIResult.enumResult.Error;
            retValue.Message = "此币别资料已存在";
            return retValue;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetBlockChainRate")]
    public BlockChainRateResult GetBlockChainRate([FromBody] FromBody.BlockChainRate fromBody)
    {
        BlockChainRateResult _BlockChainRateResult = new BlockChainRateResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string fingerprint = GetFingerprint();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.SessionError;
            return _BlockChainRateResult;
        }

        if (AdminData.CompanyType != 0)
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.VerificationError;
            return _BlockChainRateResult;
        }

        BackendDB backendDB = new BackendDB();
        var DT = RedisCache.BlockChainRate.GetBlockChainRate(fromBody.Currency);

        if (DT != null)
        {
            _BlockChainRateResult.BlockChainRateResults = DataTableExtensions.ToList<DBModel.BlockChainRate>(DT).ToList();
            _BlockChainRateResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _BlockChainRateResult;
    }

    [HttpPost]
    [ActionName("GetAllBlockChainRate")]
    public BlockChainRateResult GetAllBlockChainRate([FromBody] FromBody.BlockChainRate fromBody)
    {
        BlockChainRateResult _BlockChainRateResult = new BlockChainRateResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.SessionError;
            return _BlockChainRateResult;
        }

        if (AdminData.CompanyType != 0)
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.VerificationError;
            return _BlockChainRateResult;
        }

        BackendDB backendDB = new BackendDB();
        var DT = RedisCache.BlockChainRate.GetAllBlockChainRate();

        if (DT != null)
        {
            _BlockChainRateResult.BlockChainRateResults = DataTableExtensions.ToList<DBModel.BlockChainRate>(DT).ToList();
            _BlockChainRateResult.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            _BlockChainRateResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _BlockChainRateResult;
    }
    #endregion

    #region ConfigSetting
    [HttpPost]
    [ActionName("UpdateWithdrawOption")]
    public APIResult UpdateWithdrawOption([FromBody] FromBody.ConfigSetting fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string WithdrawOption = "";
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.UpdateWithdrawOption(fromBody.SettingValue);

        if (DBretValue >= 1)
        {
            if (fromBody.SettingValue == "0")
            {
                WithdrawOption = "开启";
            }
            else
            {
                WithdrawOption = "关闭";
            }

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "设定代付总开关:" + WithdrawOption, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("UpdateWebsiteOption")]
    public APIResult UpdateWebsiteOption([FromBody] FromBody.ConfigSetting fromBody)
    {
        APIResult retValue = new APIResult();
        BackendDB backendDB = new BackendDB();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();
        string WithdrawOption = "";
        BackendFunction backendFunction = new BackendFunction();
        int DBretValue = -1;
        string fingerprint = GetFingerprint();
        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            retValue.ResultCode = APIResult.enumResult.SessionError;
            return retValue;
        }
        else
        {
            AdminData = RedisCache.BIDContext.GetBIDInfo(fromBody.BID);
        }

        if (AdminData.CompanyType != 0)
        {
            retValue.ResultCode = APIResult.enumResult.VerificationError;
            return retValue;
        }

        DBretValue = backendDB.UpdateWebsiteOption(fromBody.SettingValue);

        if (DBretValue >= 1)
        {
            if (fromBody.SettingValue == "0")
            {
                WithdrawOption = "开启";
            }
            else
            {
                WithdrawOption = "关闭";
            }

            string IP = backendFunction.CheckIPInTW(CodingControl.GetUserIP());
            int AdminOP = backendDB.InsertAdminOPLog(AdminData.forCompanyID, AdminData.AdminID, 3, "设定網站开关:" + WithdrawOption, IP, fingerprint);
            string XForwardIP = CodingControl.GetXForwardedFor();
            CodingControl.WriteXFowardForIP(AdminOP);
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Other;
        }

        return retValue;
    }

    [HttpPost]
    [ActionName("GetConfigSetting")]
    public ConfigSettingTableResult GetConfigSetting([FromBody] FromBody.ConfigSetting fromBody)
    {
        ConfigSettingTableResult _CompanyServiceTableResult = new ConfigSettingTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServiceTableResult;
        }

        if (AdminData.CompanyType != 0)
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.VerificationError;
            return _CompanyServiceTableResult;
        }

        BackendDB backendDB = new BackendDB();
        var DBreturn = backendDB.GetConfigSetting(fromBody.SettingKey);
        if (DBreturn != null)
        {

            _CompanyServiceTableResult.ConfigSettingResults = DBreturn;
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _CompanyServiceTableResult;
    }

    [HttpPost]
    [ActionName("GetConfigSettingFormServer")]
    public ConfigSettingTableResult GetConfigSettingFormServer([FromBody] FromBody.ConfigSetting fromBody)
    {
        ConfigSettingTableResult _CompanyServiceTableResult = new ConfigSettingTableResult();
        RedisCache.BIDContext.BIDInfo AdminData = new RedisCache.BIDContext.BIDInfo();

        if (!RedisCache.BIDContext.CheckBIDExist(fromBody.BID))
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.SessionError;
            return _CompanyServiceTableResult;
        }

        BackendDB backendDB = new BackendDB();
        var DBreturn = backendDB.GetConfigSettingFormServer();
        if (DBreturn != null)
        {
            _CompanyServiceTableResult.Message = DBreturn;
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.OK;

        }
        else
        {
            _CompanyServiceTableResult.ResultCode = APIResult.enumResult.NoData;
        }

        return _CompanyServiceTableResult;
    }
    #endregion

    public static void RedirectAndPOST(System.Web.UI.Page page, string destinationUrl,
                                   System.Collections.Specialized.NameValueCollection data)
    {
        //Prepare the Posting form
        string strForm = PreparePOSTForm(destinationUrl, data);
        //Add a literal control the specified page holding 
        //the Post Form, this is to submit the Posting form with the request.
        page.Controls.Add(new System.Web.UI.LiteralControl(strForm));
    }

    private static String PreparePOSTForm(string url, System.Collections.Specialized.NameValueCollection data)
    {
        //Set a name for the form
        string formID = "PostForm";
        //Build the form using the specified data to be posted.
        System.Text.StringBuilder strForm = new System.Text.StringBuilder();
        strForm.Append("<form id=\"" + formID + "\" name=\"" +
                       formID + "\" action=\"" + url +
                       "\" method=\"POST\">");

        foreach (string key in data)
        {
            strForm.Append("<input type=\"hidden\" name=\"" + key +
                           "\" value=\"" + data[key] + "\">");
        }

        strForm.Append("</form>");
        //Build the JavaScript which will do the Posting operation.
        System.Text.StringBuilder strScript = new System.Text.StringBuilder();
        strScript.Append("<script language='javascript'>");
        strScript.Append("var v" + formID + " = document." +
                         formID + ";");
        strScript.Append("v" + formID + ".submit();");
        strScript.Append("</script>");
        //Return the form and the script concatenated.
        //(The order is important, Form then JavaScript)
        return strForm.ToString() + strScript.ToString();
    }

    #region 回傳
    public class LoginResult : APIResult
    {
        public string BID;
        public int AdminID;
        public string AdminAccount;
        public string RealName;
        public int forCompanyID;
        public int AdminType;
        public string CompanyCode;
        public string CompanyName;
        public int CompanyType;
        public string SortKey;
        public bool CheckGoogleKeySuccess;
    }

    public class CurrencyResult : APIResult
    {
        public List<DBModel.Currency> Currencies;
    }

    public class ServiceTypeResult : APIResult
    {
        public List<DBViewModel.ServiceTypeVM> ServiceTypes;
    }

    public class ProviderServiceTypeResult : APIResult
    {
        public List<DBViewModel.ServiceTypeVM> ServiceTypes;
        public List<DBModel.Provider> ProviderTypes;
    }

    public class TestPageCompanyServiceResult : APIResult
    {
        public List<DBViewModel.TestPageCompanyService> ServiceTypes;
    }

    public class CreatePatchPaymentResults : APIResult
    {
        public DBModel.CreatePatchPayment PaymentResult;
    }


    public class AdminTableResult : APIResult
    {
        public List<DBViewModel.AdminTableResult> AdminResults;
    }

    public class ProxyProviderGroupTableResult : APIResult
    {
        public List<DBModel.ProxyProviderGroup> Results;
    }

    public class ProxyProviderGroupFrozenPointResult : APIResult
    {
        public List<DBModel.ProxyProviderGroupFrozenPointHistory> Results;
    }


    public class CompanyServiceTableResult : APIResult
    {
        public List<DBViewModel.CompanyServiceTableResult> CompanyServiceResults;
    }

    public class ConfigSettingTableResult : APIResult
    {
        public List<DBModel.ConfigSetting> ConfigSettingResults;
    }

    public class BlockChainRateResult : APIResult
    {
        public List<DBModel.BlockChainRate> BlockChainRateResults;
    }

    public class CompanyCompanyServiceRelationResult : APIResult
    {
        public List<DBViewModel.CompanyServiceRelation> Results;
    }


    public class SelectedCompanyService : APIResult
    {
        public DBModel.CompanyService CompanyServiceResult;
    }

    public class BankCodeTableResult : APIResult
    {
        public List<DBModel.BankCodeTable> BankCodeResults;
    }

    public class BackendNotifyTableResult : APIResult
    {
        public List<DBModel.BackendNotifyTable> BackendNotifyResults;
    }

    public class WithdrawLimitResult : APIResult
    {
        public List<DBModel.WithdrawLimit> WithdrawLimits;
    }

    public class ProviderBankCardResult : APIResult
    {
        public List<DBModel.ProviderBankCard> ProviderBankCards;
    }

    public class WithdrawalTableResult : APIResult
    {
        public List<DBModel.Withdrawal> WithdrawalResults;
    }

    public class RiskControlByPaymentSuccessCount : APIResult
    {
        public List<DBModel.RiskControlByPaymentSuccessCount> Results;
    }

    public class GetProviderOrderCountResult : APIResult
    {
        public DBModel.ProviderOrderCount Results;
    }

    public class UpdateWithdrawalTableResult : APIResult
    {
        public DBModel.Withdrawal WithdrawalResult;
    }

    public class ChangeProviderGroupWithdrawalsByAdminReturn : APIResult
    {
        public List<DBModel.Withdrawal> Withdrawals;
        public int SuccessCount;
        public int FailCount;
    }

    public class UpdateWithdrawalResultsByAdminCheckModel : APIResult
    {
        public DBModel.UpdateWithdrawalResultsByAdminCheck WithdrawalResult;
    }

    public class GoogleQrCode : APIResult
    {
        public DBModel.GoogleQrCode GoogleQrCodeResult;
    }

    public class WithdrawalResultByWithdrawSerialResult : APIResult
    {
        public DBModel.Withdrawal WithdrawalResult;
    }

    public class UpdateWithdrawalResultByWithdrawSerialResult : APIResult
    {
        public DBViewModel.UpdateWithdrawalResult WithdrawalResult;
    }

    public class UpdatePatmentResultByPatmentSerialResult : APIResult
    {
        public DBViewModel.UpdatePatmentResult PatmentResult;
    }

    public class BankCardTableResult : APIResult
    {
        public List<DBViewModel.BankCardVM> BankCardResults;
    }

    public class CompanyBlockChainAddressResult : APIResult
    {
        public List<DBModel.CompanyBlockChainAddress> CompanyBlockChainAddressResults;
    }



    public class CompanyTableResult : APIResult
    {
        public List<DBModel.Company> CompanyResults;
    }

    public class InsertCompanyTable : APIResult
    {
        public DBViewModel.InsertCompanyReturn CompanyResult;
    }

    public class OffLineCompanyResult : APIResult
    {
        public List<DBViewModel.OffLineCompany> CompanyResults;
    }


    public class GetCompanyByIDResult : APIResult
    {
        public DBModel.Company CompanyData;
    }

    public class GetCompanyAllServiceDetail : APIResult
    {
        public List<DBViewModel.CompanyServiceTableResult> CompanyServiceResults;
        public List<DBModel.WithdrawLimit> WithdrawLimits;
        public List<DBModel.WithdrawLimit> WithdrawAPILimits;
        public List<DBModel.WithdrawLimit> WithdrawRelations;
    }

    public class BankData
    {
        public string BankName;
        public string BankCode;
    }

    public class BankDatas : APIResult
    {
        public List<BankData> BankDataList;
    }

    public class AdminRoleTableResult : APIResult
    {
        public List<DBModel.AdminRole> AdminRoleResult;
    }

    public class WithdrawalIPTableResult : APIResult
    {
        public List<DBModel.WithdrawalIP> Result;
    }


    public class AdminRolePermissionResult : APIResult
    {
        public List<DBViewModel.AdminRolePermission> AdminRolePermissions;
    }


    public class LayoutLeftSideBarResult : APIResult
    {
        public string CategoryDescription;
        public string PermissionCategoryName;
        public List<DBViewModel.LayoutLeftSideBarResult> PermissionResults;
    }
    public class PermissionTableResult : APIResult
    {
        public List<DBModel.Permission> PermissionResult;
    }

    public class SummaryProviderByDateTableResult : APIResult
    {
        public List<DBModel.SummaryProviderByDate> SummaryProviderByDateResults;
    }

    public class ProxySummaryProviderByDateTableResult : APIResult
    {
        public List<DBModel.ProxySummaryProviderByDate> SummaryProviderByDateResults;
    }

    public class PaymentTableResult : APIResult
    {
        public List<DBModel.PaymentReport> PaymentTableResults;
    }


    public class PaymentReportV2 : APIResult
    {
        public DBModel.returnPaymentReportV2 PaymentTableResults;
    }

    public class WithdrawalReportTableResult : APIResult
    {
        public List<DBModel.RiskControlWithdrawalTable> WithdrawalTableResults;
    }


    public class PaymentPointBySearchFilter : APIResult
    {
        public DBModel.StatisticsPaymentAmount Result;
    }


    public class PaymentTransferLogResult : APIResult
    {
        public List<DBModel.PaymentTransferLog> PaymentTableResults;
    }

    public class DownOrderTransferLogResult : APIResult
    {
        public List<DBModel.DownOrderTransferLog> DownOrderTableResults;
    }

    public class DownOrderTransferLogResultV2
    {
        public List<DBModel.DownOrderTransferLogV2> data;
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public bool IsAutoLoad { get; set; }
        public int ResultCode;
    }


    public class PatchPaymentTableResult : APIResult
    {
        public List<DBModel.PaymentTable> PaymentTableResults;
    }

    public class AgentReceiveTableResult : APIResult
    {
        public List<DBModel.AgentReceive> AgentReceiveTableResults;
        public DBViewModel.AgentReceiveVM AgentAmountResult;
    }

    public class ProviderProfitTableResult : APIResult
    {
        public List<DBModel.ProviderManualHistory> ProviderProfitManualHistoryResult;
    }

    public class AgentCloseTableResult : APIResult
    {
        public List<DBModel.AgentClose> AgentCloseTableResults;
    }

    public class AdminRolePermissionTableResult : APIResult
    {
        public List<DBViewModel.AdminRolePermissionResult> AdminRolePermissionResult;
    }

    public class ProviderPointResult : APIResult
    {
        public List<DBViewModel.ProviderPointVM> ProviderPointResults;
    }

    public class ProviderCodeResult : APIResult
    {
        public List<DBModel.Provider> ProviderTypes;
    }

    public class ProxyProviderResult : APIResult
    {
        public List<DBModel.ProxyProvider> ProviderTypes;
    }

    public class ProxyProviderData : APIResult
    {
        public DBModel.ProxyProvider ProxyProvider;
        public DBModel.ProxyProviderGroup ProxyProviderGroup;
        public decimal FrozenPoint;
    }

    public class GPayRelationResult : APIResult
    {
        public List<DBViewModel.GPayRelationResult> GPayRelations;
    }

    public class GPayRelationResult2 : APIResult
    {
        public List<DBModel.GPayRelation> GPayRelations;
    }


    public class GPayWithdrawRelationResult : APIResult
    {
        public List<DBViewModel.GPayWithdrawRelation> GPayWithdrawRelations;
    }


    public class GetApiWithdrawLimitResult : APIResult
    {
        public List<DBViewModel.ApiWithdrawLimit> ApiWithdrawLimitResults;
    }

    public class GetParentApiWithdrawLimitResult : APIResult
    {
        public DBModel.WithdrawLimit ParentApiWithdrawLimitResult;
    }

    public class ProviderServiceResult : APIResult
    {
        public List<DBViewModel.ProviderServiceVM> ProviderServices;
    }

    public class PermissionCategoryResult : APIResult
    {
        public List<DBModel.PermissionCategory> PermissionCategorys;
    }

    public class CompanyPointResult : APIResult
    {
        public List<DBViewModel.CompanyPointVM> CompanyPoints;
    }

    public class CompanyPointAndCompanyServicePointResult : APIResult
    {
        public List<DBViewModel.CompanyPointVM> CompanyPoints;
        public List<DBViewModel.CompanyServicePointVM> CompanyServicePoints;
    }


    public class CompanyServicePoint : APIResult
    {
        public List<DBViewModel.CompanyServicePointVM> CompanyServicePoints;
    }

    public class CompanyServicePointByService : APIResult
    {
        public DBViewModel.CompanyServicePointVM CompanyServicePoint;
    }

    public class SummaryCompanyByDate : APIResult
    {
        public List<DBModel.SummaryCompanyByDate> SummaryCompanyByDates;
        public decimal TotalAmount;
        public decimal TotalNetAmount;
        public decimal TotalProviderNetAmount;
        public decimal TotalPayAgentAmount;
        public decimal TotalAgentAmount;
        public decimal TotalNetWithdrawAmount;
    }

    public class SummaryBlockChainByDate : APIResult
    {
        public List<DBModel.SummaryBlockChainByDate> SummaryBlockChainByDates;
        public decimal TotalAmount;
        public decimal TotalNetAmount;
    }

    public class SummaryCompanyByDateFlot : APIResult
    {
        public List<DBModel.SummaryCompanyByDateFlot> SummaryCompanyByDates;
    }


    public class SummaryCompanyByHour : APIResult
    {
        public List<DBModel.SummaryCompanyByHour> SummaryCompanyByHours;

    }


    public class CompanyServicePointHistory : APIResult
    {
        public List<DBModel.CompanyServicePointHistory> CompanyServicePointHistorys;
    }

    public class CompanyServicePointLog : APIResult
    {
        public List<DBModel.CompanyServicePointLog> CompanyServicePointLogs;
    }

    public class CompanyServiceAndProviderPointLog : APIResult
    {
        public List<DBModel.CompanyServiceAndProviderPointLog> CompanyServicePointLogs;
    }


    public class SummaryCompanyByDateResultForChart : APIResult
    {
        public List<DBModel.SummaryCompanyByDateChartData> SummaryCompanyByDates;
    }

    public class CompanyPointHistoryResult : APIResult
    {
        public List<DBModel.CompanyPointHistory> CompanyPointHistoryDates;
    }

    public class ProviderPointResult2 : APIResult
    {
        public List<DBViewModel.ProviderPointVM> ProviderPointResults;
        public List<DBViewModel.CompanyServicePointVM> CompanyServicePointResults;
    }

    public class ProviderPointHistory : APIResult
    {
        public List<DBViewModel.ProviderPointHistory> ProviderPointHistoryResults;
    }

    public class ProxyProviderPointHistory : APIResult
    {
        public List<DBViewModel.ProxyProviderPointHistory> ProviderPointHistoryResults;
    }

    public class ProviderListResult : APIResult
    {
        public List<DBViewModel.ProviderListResult> ProviderListResults;
    }

    public class AllProviderTotalResult : APIResult
    {
        public DBViewModel.AllProviderTotal AllProviderTotals;
    }

    public class AdminOPLogResult : APIResult
    {
        public List<DBViewModel.AdminOPLogVM> Results;
    }

    public class ProviderManualHistoryResult : APIResult
    {
        public List<DBModel.ProviderManualHistory> Results;
    }

    public class GetCompanyWhiteListProviderBankCardResult : APIResult
    {
        public List<DBModel.CompanyWhiteList> Results;
    }

    public class FrozenPointHistoryResult : APIResult
    {
        public List<DBModel.FrozenPointHistory> Results;
    }

    public class SumFrozenPointResult : APIResult
    {
        public DBModel.FrozenPointHistory Results;
    }

    public class BlackListHistoryResult : APIResult
    {
        public List<DBModel.BlackList> Results;
    }

    public class CompanyManualHistoryResult : APIResult
    {
        public List<DBModel.CompanyManualHistory> Results;
    }

    public class OrderByCompanyManualHistoryByFrozenPoint : APIResult
    {
        public DBModel.PaymentReport Result;
        public List<DBModel.ProxyProviderGroup> GroupResult;
    }

    public class OrderByCompanyManualHistoryResult : APIResult
    {
        public DBModel.PaymentReport Result;
    }


    public class AgentReceiveResult : APIResult
    {
        public DBViewModel.AgentReceiveVM AgentReceiveResults;
    }

    public class SearchIPCountyResult : APIResult
    {
        public DBViewModel.IPCounty Result;
    }

    public class GetOnlineList
    {
        public string BID { set; get; }
        public int CompanyID { set; get; }
    }

    public class KickBackendUserSID
    {
        public string BID { set; get; }
        public string SessionID { set; get; }
    }

    public class OnlineListResult : APIResult
    {
        public List<OnlineList> OnlineResults;
    }

    public class OnlineList
    {
        public string LoginAccount { get; set; }
        public string UserIP { get; set; }
        public string LoginDate { get; set; }
        public string SessionID { get; set; }
    }


    public class APIResult
    {
        public enum enumResult
        {
            OK = 0,
            LoginAccountEmpty,
            PasswordEmpty,
            CompanyCodeNotExist,
            VerificationError,
            NoData,
            Error,
            SessionError,
            DataExist,
            DataDuplicate, //資料重複
            UplineMaxDaliyAmountByUseError,
            MaxDaliyAmountByUseError,
            OfflineMaxDaliyAmountByUseError,
            GoogleKeyEmpty,
            GoogleKeyError,
            CompanyPointError,
            Other = 99
        }
        public enumResult ResultCode;
        public string Message;
    }

    public class UploadQrcodeResult : APIResult
    {
        public string QrcodeContent;
    }
    #endregion

    public static string GetFingerprint()
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies["Fingerprint"];

        if (cookie != null)
        {
            return cookie.Value;
        }

        return "";
    }
}