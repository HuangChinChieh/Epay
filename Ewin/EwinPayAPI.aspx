<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Common.cs" Inherits="Common" %>

<%
    string paymentResult = "";

    string Method = Request.Params["Method"];
    string CompanyCode = Request.Params["CompanyCode"];
    string Timestamp = Request.Params["Timestamp"];
    string Sign = Request.Params["Sign"];

    APIResult R = new APIResult() { ResultState = APIResult.enumResultCode.ERR };

    if (Method == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter Method not Exist";
        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
        Response.Flush();
        Response.End();
    }

    if (Timestamp == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message =  "The parameter Timestamp not Exist";
        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
        Response.Flush();
        Response.End();
    }

    if (CompanyCode == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message =  "The parameter CompanyCode not Exist";
        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
        Response.Flush();
        Response.End();
    }

    if (Sign == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter Sign not Exist";
        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
        Response.Flush();
        Response.End();
    }

    if (!Common.CheckTimestamp(long.Parse(Timestamp)))
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "Timestamp Expired";
        Response.Write(R);
        Response.Flush();
        Response.End();
    }

    if (Method=="GetGPayRelation")
    {
        if (Common.CheckEPaySign(CompanyCode, Method, Sign, Timestamp))
        {
            var CompanyModel = Common.GetCompanyIDAndCurrencyTypeByCompanyCode(CompanyCode);

            if (CompanyModel == null)
            {
                R.ResultState = Common.APIResult.enumResultCode.ERR;
                R.Message = "Company Code Error";
                Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
                Response.Flush();
                Response.End();
            }
            else
            {
                var LstGPayRelation = Common.GetGPayRelation(CompanyModel.CompanyID, CompanyModel.CurrencyType);
                if (LstGPayRelation == null)
                {
                    R.ResultState = Common.APIResult.enumResultCode.ERR;
                    R.Message = "No Data";
                    Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
                    Response.Flush();
                    Response.End();
                }
                else
                {

                    R.ResultState = Common.APIResult.enumResultCode.OK;
                    R.Message = Newtonsoft.Json.JsonConvert.SerializeObject(LstGPayRelation);
                    Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
                    Response.Flush();
                    Response.End();
                }
            }
        }
        else
        {
            R.ResultState = APIResult.enumResultCode.ERR;
            R.Message = "Sign Fail";
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
            Response.Flush();
            Response.End();
        }
    }
    else { 
            R.ResultState = APIResult.enumResultCode.ERR;
            R.Message = "Method Not Exist";
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(R));
            Response.Flush();
            Response.End();
    }

    //}
    //else
    //{
    //    R.ResultState = APIResult.enumResultCode.ERR;
    //    R.Message = "IP Fail:" + InIP;
    //}



%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>

<body>
</body>
</html>


