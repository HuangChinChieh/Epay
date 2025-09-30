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


public partial class WithdrawReview : System.Web.UI.Page
{
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string HeartBeat(string test)
    {
        return test;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ProviderPointResult GetWithdrawDetailPointResult(int CompanyID, string CurrencyType)
    {
        ProviderPointResult retValue = new ProviderPointResult();

        retValue.ProviderPointResults = Common.GetAllProviderPointByCompanyID(CompanyID, CurrencyType);
        //retValue.CompanyServicePointResults = Common.GetCompanyServicePointDetail(CompanyID, CurrencyType);

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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static APIResult ChangeProviderCodeState(string ProviderCode)
    {
        APIResult retValue = new APIResult();


        if (Common.ChangeProviderCodeState(ProviderCode) > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static APIResult ChangeProviderAPIType(string ProviderCode, int setAPIType)
    {
        APIResult retValue = new APIResult();

        System.Data.DataTable DT = Common.RedisCache.ProviderCode.GetProviderCode(ProviderCode);

        if (((int)DT.Rows[0]["ProviderAPIType"] & setAPIType) == setAPIType)
        {
            setAPIType = 0 - setAPIType;
        }

        if (Common.ChangeProviderAPIType(ProviderCode, setAPIType) > 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
        }
        else
        {
            retValue.ResultCode = APIResult.enumResult.Error;
        }

        return retValue;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static Common.APIResult UpdateWithdrawalResultByWithdrawSerial(string WithdrawSerial, int Status, string ProviderCode, string ServiceType)
    {
        Common.APIResult result = new Common.APIResult();
        result = Common.UpdateWithdrawalResultByWithdrawSerial(Status, WithdrawSerial, ProviderCode, 1, ServiceType);

        return result;
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static WithdrawalResult GetWithdrawalByOrderID(string OrderID, int CompanyID)
    {
        WithdrawalResult result = new WithdrawalResult() { ResultCode= APIResult.enumResult.DataExist };
      
        Common.Withdrawal withdrawal = Common.GetWithdrawalByOrderID(OrderID, CompanyID);
        if (withdrawal!=null)
        {
            result.data = withdrawal;
            result.ResultCode = APIResult.enumResult.OK;
        }
        return result;
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

    public class ProviderPointResult : APIResult
    {
        public List<Common.ProviderPointVM> ProviderPointResults;
        public List<Common.CompanyServicePointVM> CompanyServicePointResults;
    }

    public class WithdrawalResult : APIResult
    {
        public Common.Withdrawal data;
    }
}
