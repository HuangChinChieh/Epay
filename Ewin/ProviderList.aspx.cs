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


public partial class ProviderList: System.Web.UI.Page
{
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string HeartBeat(string test)
    {
        return test;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static APIResult ChangeProviderServiceState(string ServiceType, string CurrencyType,string ProviderCode)
    {
        APIResult retValue = new APIResult();

        if (Common.ChangeProviderServiceState(ProviderCode, ServiceType, CurrencyType) > 0)
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
    public static APIResult ChangeProviderAPIType(string ProviderCode,int setAPIType)
    {
        APIResult retValue = new APIResult();
        
        System.Data.DataTable DT =Common.RedisCache.ProviderCode.GetProviderCode(ProviderCode);

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
    public static APIResult UpdateProviderWithdrawLimitResult(int CompanyID, string ProviderCode, decimal MaxLimit, decimal MinLimit, decimal Charge)
    {
        APIResult retValue = new APIResult();
        string CurrencyType= Common.GetCurrencyTypeByCompanyID(CompanyID);
        var DBreturn = Common.UpdateProviderWithdrawLimitResult(CurrencyType, ProviderCode, MaxLimit, MinLimit, Charge);
        if (DBreturn > 0)
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
    public static ProviderListResults GetProviderListResult(int CompanyID)
    {
        ProviderListResults retValue = new ProviderListResults();
        retValue.ProviderListResult = Common.GetProviderListResult(CompanyID);

        if (retValue.ProviderListResult != null)
        {
            var ServiceDatas = Common.GetProviderListServiceData(CompanyID);
            var ProviderPoints = Common.GetAllProviderPoint(CompanyID);

            foreach (var item in retValue.ProviderListResult)
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static APIResult UpdateProviderServiceResult(int CompanyID, string ProviderCode,decimal CostRate,string ServiceType)
    {
        APIResult retValue = new APIResult();
        string CurrencyType = Common.GetCurrencyTypeByCompanyID(CompanyID);

        if (Common.UpdateProviderService(ProviderCode, ServiceType, CurrencyType,CostRate) > 0)
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
    public static APIResult InsertManualHistory(int CompanyID, decimal Amount, string Description, string ProviderCode)
    {
        APIResult retValue = new APIResult();

        var DBreturn = Common.InsertManualHistory("PHP01",0, "PHP", Amount, Description, ProviderCode, CompanyID,1);
        if (DBreturn == 0)
        {
            retValue.ResultCode = APIResult.enumResult.OK;
            string strCompanyName = "";
            string strType = "";

            strType = "代收";

            var strProviderName = ProviderCode;
            string IP = Common.GetUserIP();
            int AdminOP = Common.InsertAdminOPLog(CompanyID, 0, 1, "人工提存-金额修改,商户名称:" + strCompanyName + ",渠道名称:" + strProviderName + ",类型:" + strType + ",币别:" + "PHP" + ",额度:" + Amount , IP);
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

    public class ProviderListResults: APIResult
    {
        public List<Common.ProviderListResult> ProviderListResult { get; set; }
    }
}
