using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlClient;
using System.Data;


public partial class SetEPayCompanyService : System.Web.UI.Page
{
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string HeartBeat(string test)
    {
        return test;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static GetCompanyAllServiceDetail GetCompanyAllServiceDetailData(int CompanyID)
    {
        GetCompanyAllServiceDetail retValue = new GetCompanyAllServiceDetail();

        retValue.WithdrawRelations = Common.GetCompanyWithdrawRelationResult(CompanyID);
        retValue.WithdrawLimits = Common.GetWithdrawLimitResult(new Common.WithdrawLimit() { WithdrawLimitType = 1, ProviderCode = "", CompanyID = CompanyID });
        retValue.CompanyServiceResults = Common.GetCompanyServiceTableByCompanyID(CompanyID);
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static CompanyServicePoint GetCompanyServicePointDetail2(int CompanyID)
    {
        CompanyServicePoint _CompanyServicePointResult = new CompanyServicePoint();

        string CurrencyType = Common.GetCurrencyByCompanyID(CompanyID);
        List<Common.CompanyServicePointVM> companys = Common.GetCompanyServicePointDetail2(CompanyID, CurrencyType);
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static GPayRelationResult2 GetGPayRelationResult(int CompanyID,string ServiceType,string CurrencyType)
    {
        GPayRelationResult2 _Result = new GPayRelationResult2();

        var TableResult = Common.GetGPayRelationResult(ServiceType, CurrencyType, "", CompanyID);

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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static APIResult DisableCompanyService(int CompanyID,string ServiceType,string CurrencyType)
    {
        APIResult retValue = new APIResult();
  
        int DBretValue = Common.DisableCompanyService(CompanyID, ServiceType, CurrencyType);

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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static CompanyPointResult GetCompanyPointTableResult(int CompanyID)
    {
        CompanyPointResult _CompanyTableResult = new CompanyPointResult();

        //List<DBViewModel.CompanyPointVM> companys = backendDB.GetCompanyPointTableResult(seleCompanyID);
        List<Common.CompanyPointVM> companys = new List<Common.CompanyPointVM>();

        string CurrencyType = Common.GetCurrencyByCompanyID(CompanyID);
        companys = Common.GetCompanyPointTableResult(CompanyID, CurrencyType);
       
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

    public class GetCompanyAllServiceDetail : APIResult
    {
        public List<Common.CompanyServiceTableResult> CompanyServiceResults;
        public List<Common.WithdrawLimit> WithdrawLimits;
        public List<Common.WithdrawLimit> WithdrawRelations;
    }

    public class CompanyServicePoint : APIResult
    {
        public List<Common.CompanyServicePointVM> CompanyServicePoints;
    }

    public class CompanyPointResult : APIResult
    {
        public List<Common.CompanyPointVM> CompanyPoints;
    }


    public class GPayRelationResult2 : APIResult
    {
        public List<Common.GPayRelation> GPayRelations;
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
}
